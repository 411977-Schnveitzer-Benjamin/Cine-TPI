using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.Extensions.Options;

namespace CineHollywoodTPI.API.Services
{
    // Reglas de precios:
    //  - Precio base = precio vigente del TIPO DE SALA para la fecha de la función (PRECIO_TIPO_SALA).
    //  - Si la función tiene PROMOCIÓN: se aplica solo la promo (a todos los tipos de espectador).
    //  - Si NO tiene promoción: Jubilado y Menor tienen 15% de descuento (configurable).
    //  - Al cargar un precio nuevo, el vigente se cierra el día anterior.
    public class PrecioService : IPrecioService
    {
        private readonly IPrecioRepository _precioRepository;
        private readonly ISalaRepository _salaRepository;
        private readonly ReglasNegocioOptions _reglas;

        public PrecioService(IPrecioRepository precioRepository, ISalaRepository salaRepository, IOptions<ReglasNegocioOptions> reglas)
        {
            _precioRepository = precioRepository;
            _salaRepository = salaRepository;
            _reglas = reglas.Value;
        }

        private static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Today);

        // ===================== CÁLCULO =====================

        public decimal PrecioBase(Funcion funcion)
        {
            var precios = funcion.IdSalaNavigation?.IdTipoSalaNavigation?.PrecioTipoSalas
                          ?? new List<PrecioTipoSala>();
            var fecha = funcion.Fecha ?? Hoy;

            // Precio vigente para la fecha de la función; si no hay, el más reciente cargado
            var vigente = precios
                .Where(p => (p.FechaDesde == null || p.FechaDesde <= fecha) &&
                            (p.FechaHasta == null || p.FechaHasta >= fecha))
                .OrderByDescending(p => p.FechaDesde)
                .FirstOrDefault()
                ?? precios.OrderByDescending(p => p.FechaDesde).FirstOrDefault();

            return vigente?.Precio ?? 0m;
        }

        public decimal PrecioEntrada(Funcion funcion, int idTipoEspectador)
        {
            var porcentaje = PorcentajeDescuento(funcion, idTipoEspectador, out _);
            return Math.Round(PrecioBase(funcion) * (1 - porcentaje / 100m), 2);
        }

        public PrecioPorTipoDTO DetallePrecio(Funcion funcion, ItemDTO tipoEspectador)
        {
            var porcentaje = PorcentajeDescuento(funcion, tipoEspectador.Id, out var motivo);
            return new PrecioPorTipoDTO
            {
                IdTipoEspectador = tipoEspectador.Id,
                TipoEspectador = tipoEspectador.Descripcion,
                Precio = Math.Round(PrecioBase(funcion) * (1 - porcentaje / 100m), 2),
                // "-15%" para jubilado/menor, "2x1 Lunes y Martes -50%" para una promo
                Descuento = porcentaje <= 0 ? null
                          : motivo != null ? $"{motivo} -{porcentaje:0.##}%"
                          : $"-{porcentaje:0.##}%"
            };
        }

        // Decide qué descuento corresponde: la promoción tiene prioridad y NO se acumula
        private decimal PorcentajeDescuento(Funcion funcion, int idTipoEspectador, out string? motivo)
        {
            var promo = funcion.IdPromocionNavigation;
            if (promo?.PorcentajeDescuento > 0)
            {
                motivo = promo.Descripcion;
                return promo.PorcentajeDescuento.Value;
            }

            if (_reglas.IdsTipoEspectadorConDescuento.Contains(idTipoEspectador))
            {
                motivo = null; // el nombre del tipo (Jubilado / Menor) ya se muestra al lado
                return _reglas.PorcentajeDescuentoJubiladoMenor;
            }

            motivo = null;
            return 0m;
        }

        // ===================== ABM =====================

        public async Task<IEnumerable<PrecioDTO>> GetAllAsync()
        {
            var hoy = Hoy;
            return (await _precioRepository.GetAllAsync()).Select(p => new PrecioDTO
            {
                IdPrecio = p.IdPrecio,
                IdTipoSala = p.IdTipoSala,
                TipoSala = p.IdTipoSalaNavigation?.Descripcion,
                Precio = p.Precio,
                FechaDesde = p.FechaDesde,
                FechaHasta = p.FechaHasta,
                Estado = p.FechaDesde > hoy ? "Programado"
                       : (p.FechaHasta == null || p.FechaHasta >= hoy) ? "Vigente"
                       : "Histórico",
                SePuedeEliminar = p.FechaDesde >= hoy
            }).ToList();
        }

        public async Task<int> CrearAsync(PrecioInsertDTO dto)
        {
            if (dto == null) throw new NegocioException("Los datos del precio son inválidos.");
            if (dto.Precio <= 0 || dto.Precio > 99_999_999m)
                throw new NegocioException("El precio tiene que ser mayor a cero.");
            if (dto.FechaDesde < Hoy)
                throw new NegocioException("La fecha de inicio no puede ser anterior a hoy.");
            if (!await _salaRepository.ExisteTipoSalaAsync(dto.IdTipoSala))
                throw new NegocioException("El tipo de sala no existe.");

            var delTipo = await _precioRepository.GetPorTipoSalaAsync(dto.IdTipoSala);

            if (delTipo.Any(p => p.FechaDesde >= dto.FechaDesde))
                throw new ConflictoException("Ya hay un precio cargado que empieza en esa fecha o después. Eliminalo primero o elegí otra fecha.");

            // El precio que seguiría vigente se cierra el día anterior al nuevo
            foreach (var anterior in delTipo.Where(p => p.FechaHasta == null || p.FechaHasta >= dto.FechaDesde))
                anterior.FechaHasta = dto.FechaDesde.AddDays(-1);

            return await _precioRepository.CrearAsync(new PrecioTipoSala
            {
                IdTipoSala = dto.IdTipoSala,
                Precio = Math.Round(dto.Precio, 2),
                FechaDesde = dto.FechaDesde,
                FechaHasta = null // vale hasta que se cargue otro
            });
        }

        public async Task EliminarAsync(int idPrecio)
        {
            var precio = await _precioRepository.GetByIdAsync(idPrecio)
                ?? throw new NoEncontradoException($"No existe el precio con ID {idPrecio}.");

            if (precio.FechaDesde < Hoy)
                throw new NegocioException("No se puede eliminar un precio que ya se aplicó. Cargá uno nuevo en su lugar.");

            // El precio anterior (el que se cerró al cargar este) vuelve a cubrir el período
            var anterior = (await _precioRepository.GetPorTipoSalaAsync(precio.IdTipoSala ?? 0))
                .Where(p => p.FechaDesde < precio.FechaDesde)
                .OrderByDescending(p => p.FechaDesde)
                .FirstOrDefault();

            if (anterior != null && anterior.FechaHasta == precio.FechaDesde!.Value.AddDays(-1))
                anterior.FechaHasta = precio.FechaHasta;

            await _precioRepository.EliminarAsync(precio);
        }
    }
}
