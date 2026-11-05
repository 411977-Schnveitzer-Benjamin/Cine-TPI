using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.Extensions.Options;

namespace CineHollywoodTPI.API.Services
{
    // Reglas de funciones:
    //  - Película, sala, formato, fecha y hora obligatorios; la película tiene que estar activa.
    //  - No se puede programar en una fecha pasada.
    //  - No se puede superponer con otra función de la misma sala (según la duración de la película).
    //  - Si ya tiene entradas vendidas: no se puede cambiar película, sala, fecha ni hora, ni cancelarla.
    //  - Una función "empezada" (fecha + hora ya pasaron) no aparece como próxima y no se vende.
    public class FuncionService : IFuncionService
    {
        private readonly IFuncionRepository _funcionRepository;
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly ICatalogoRepository _catalogoRepository;
        private readonly IPrecioService _precioService;
        private readonly ReglasNegocioOptions _reglas;

        public FuncionService(
            IFuncionRepository funcionRepository,
            IPeliculaRepository peliculaRepository,
            ICatalogoRepository catalogoRepository,
            IPrecioService precioService,
            IOptions<ReglasNegocioOptions> reglas)
        {
            _funcionRepository = funcionRepository;
            _peliculaRepository = peliculaRepository;
            _catalogoRepository = catalogoRepository;
            _precioService = precioService;
            _reglas = reglas.Value;
        }

        // ===================== Reglas reutilizables =====================

        public bool YaEmpezo(Funcion f)
        {
            if (f.Fecha == null) return false;
            var inicio = f.Fecha.Value.ToDateTime(f.HoraInicio ?? TimeOnly.MinValue);
            return inicio <= DateTime.Now;
        }

        public bool EsParaMayores(Funcion f) =>
            f.IdPeliculaNavigation?.IdClasificacionNavigation?.Descripcion?
                .Contains(_reglas.TextoClasificacionMayores) == true;

        // ===================== Consultas =====================

        public async Task<IEnumerable<FuncionDTO>> GetActivasAsync(int? idPelicula, bool soloProximas)
        {
            DateOnly? desde = soloProximas ? DateOnly.FromDateTime(DateTime.Today) : null;
            var funciones = (await _funcionRepository.GetAllActiveAsync(idPelicula, desde)).ToList();

            // "Próximas" = que todavía no empezaron (no solo por fecha, también por hora)
            if (soloProximas)
                funciones = funciones.Where(f => !YaEmpezo(f)).ToList();

            var conVentas = await _funcionRepository.GetIdsConEntradasVendidasAsync(funciones.Select(f => f.IdFuncion));
            var tipos = (await _catalogoRepository.GetTiposEspectadorAsync()).ToList();

            return funciones.Select(f => ToDto(f, tipos, conVentas.Contains(f.IdFuncion))).ToList();
        }

        public async Task<FuncionDTO> GetPorIdAsync(int id)
        {
            var funcion = await _funcionRepository.GetByIdAsync(id);
            if (funcion == null || !funcion.Activo)
                throw new NoEncontradoException($"No se encontró ninguna función activa con el ID {id}.");

            var tipos = (await _catalogoRepository.GetTiposEspectadorAsync()).ToList();
            return ToDto(funcion, tipos, await _funcionRepository.TieneEntradasVendidasAsync(id));
        }

        public async Task<IEnumerable<ButacaEstadoDTO>> GetButacasAsync(int idFuncion) =>
            await _funcionRepository.GetButacasAsync(idFuncion);

        private FuncionDTO ToDto(Funcion f, List<ItemDTO> tiposEspectador, bool tieneVentas)
        {
            var paraMayores = EsParaMayores(f);

            return new FuncionDTO
            {
                IdFuncion = f.IdFuncion,
                IdPelicula = f.IdPelicula,
                IdSala = f.IdSala,
                IdFormato = f.IdFormato,
                IdPromocion = f.IdPromocion,
                Fecha = f.Fecha,
                HoraInicio = f.HoraInicio,
                Activo = f.Activo,
                PeliculaTitulo = f.IdPeliculaNavigation?.Titulo,
                DuracionMinutos = f.IdPeliculaNavigation?.DuracionMinutos,
                Clasificacion = f.IdPeliculaNavigation?.IdClasificacionNavigation?.Descripcion,
                EsParaMayores = paraMayores,
                SalaNombre = f.IdSalaNavigation?.Nombre,
                TipoSala = f.IdSalaNavigation?.IdTipoSalaNavigation?.Descripcion,
                FormatoDescripcion = f.IdFormatoNavigation?.Descripcion,
                PromocionDescripcion = f.IdPromocionNavigation?.Descripcion,
                PorcentajeDescuento = f.IdPromocionNavigation?.PorcentajeDescuento,
                TieneEntradasVendidas = tieneVentas,
                PrecioEntrada = _precioService.PrecioEntrada(f, 1),
                Precios = tiposEspectador.Select(t =>
                {
                    var detalle = _precioService.DetallePrecio(f, t);
                    detalle.Permitido = !(paraMayores && t.Id == _reglas.IdTipoEspectadorMenor);
                    return detalle;
                }).ToList()
            };
        }

        // ===================== ABM =====================

        private async Task ValidarAsync(Funcion f)
        {
            if (f.IdPelicula == null || f.IdSala == null || f.IdFormato == null || f.Fecha == null || f.HoraInicio == null)
                throw new NegocioException("Película, sala, formato, fecha y hora son obligatorios.");

            var pelicula = await _peliculaRepository.GetByIdAsync(f.IdPelicula.Value);
            if (pelicula == null || !pelicula.Activo)
                throw new NegocioException("La película seleccionada no existe o está dada de baja.");

            var duracionNueva = pelicula.DuracionMinutos ?? 120;
            var inicioNueva = f.Fecha.Value.ToDateTime(f.HoraInicio.Value);
            var finNueva = inicioNueva.AddMinutes(duracionNueva);

            var otras = await _funcionRepository.GetActivasDeSalaEnFechaAsync(f.IdSala.Value, f.Fecha.Value, f.IdFuncion == 0 ? null : f.IdFuncion);
            var choca = otras.FirstOrDefault(o =>
            {
                if (o.HoraInicio == null) return false;
                var inicio = o.Fecha!.Value.ToDateTime(o.HoraInicio.Value);
                var fin = inicio.AddMinutes(o.IdPeliculaNavigation?.DuracionMinutos ?? 120);
                return inicioNueva < fin && inicio < finNueva;
            });

            if (choca != null)
                throw new ConflictoException(
                    $"Se superpone con \"{choca.IdPeliculaNavigation?.Titulo}\" de las {choca.HoraInicio:HH\\:mm} en esa sala.");
        }

        public async Task<int> CrearAsync(FuncionInsertDTO dto)
        {
            if (dto == null) throw new NegocioException("Los datos enviados para la función son inválidos.");

            if (dto.Fecha < DateOnly.FromDateTime(DateTime.Today))
                throw new NegocioException($"La fecha {dto.Fecha:dd/MM/yyyy} ya pasó. No se puede programar una función en el pasado.");

            var nueva = new Funcion
            {
                IdPelicula = dto.IdPelicula,
                IdSala = dto.IdSala,
                IdFormato = dto.IdFormato,
                IdPromocion = dto.IdPromocion,
                Fecha = dto.Fecha,
                HoraInicio = dto.HoraInicio,
                Activo = true
            };

            if (YaEmpezo(nueva))
                throw new NegocioException("Ese horario ya pasó.");

            await ValidarAsync(nueva);
            await _funcionRepository.AddAsync(nueva);
            return nueva.IdFuncion;
        }

        public async Task ModificarAsync(int id, FuncionUpdateDTO dto)
        {
            if (dto == null || id != dto.IdFuncion)
                throw new NegocioException("Los identificadores de la petición no coinciden.");

            var funcion = await _funcionRepository.GetByIdAsync(id)
                ?? throw new NoEncontradoException($"No existe la función con ID {id}.");

            // Regla: con entradas vendidas no se puede mover la función (la gente ya compró para ese día/sala)
            if (await _funcionRepository.TieneEntradasVendidasAsync(id))
            {
                var cambiaAlgoCritico =
                    funcion.IdPelicula != dto.IdPelicula ||
                    funcion.IdSala != dto.IdSala ||
                    funcion.Fecha != dto.Fecha ||
                    funcion.HoraInicio != dto.HoraInicio ||
                    !dto.Activo;

                if (cambiaAlgoCritico)
                    throw new NegocioException(
                        "La función ya tiene entradas vendidas: no se puede cambiar la película, la sala, la fecha ni la hora, ni cancelarla. Solo se puede modificar el formato y la promoción.");
            }

            funcion.IdPelicula = dto.IdPelicula;
            funcion.IdSala = dto.IdSala;
            funcion.IdFormato = dto.IdFormato;
            funcion.IdPromocion = dto.IdPromocion;
            funcion.Fecha = dto.Fecha;
            funcion.HoraInicio = dto.HoraInicio;
            funcion.Activo = dto.Activo;

            await ValidarAsync(funcion);
            await _funcionRepository.UpdateAsync(funcion);
        }

        public async Task CancelarAsync(int id)
        {
            if (await _funcionRepository.TieneEntradasVendidasAsync(id))
                throw new NegocioException("La función ya tiene entradas vendidas: no se puede cancelar.");

            if (!await _funcionRepository.DeleteLogicoAsync(id))
                throw new NoEncontradoException($"No existe la función con ID {id} o ya está cancelada.");
        }
    }
}
