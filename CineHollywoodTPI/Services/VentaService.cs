using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.Extensions.Options;

namespace CineHollywoodTPI.API.Services
{
    // Reglas de ventas:
    //  - Cliente logueado: compra a su nombre por el canal Web.
    //    Administrador: vende en boletería y tiene que elegir un cliente activo.
    //  - Entre 1 y 10 entradas por compra.
    //  - La función tiene que existir, estar activa y NO haber empezado (fecha + hora).
    //  - La butaca tiene que ser de la sala de la función, estar habilitada y libre (sin repetir).
    //  - Película +18: no se venden entradas de tipo "Menor" ni a clientes menores de edad.
    //  - Precio calculado en el servidor: promoción de la función, o 15% para Jubilado / Menor.
    //  - Formas de pago: una sola sin monto paga el total; si son varias, tienen que sumar el total.
    //  - Un cliente solo puede ver sus propias compras.
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepository;
        private readonly IFuncionRepository _funcionRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly ICatalogoRepository _catalogoRepository;
        private readonly IFuncionService _funcionService;
        private readonly IPrecioService _precioService;
        private readonly ReglasNegocioOptions _reglas;

        public VentaService(
            IVentaRepository ventaRepository,
            IFuncionRepository funcionRepository,
            IClienteRepository clienteRepository,
            ICatalogoRepository catalogoRepository,
            IFuncionService funcionService,
            IPrecioService precioService,
            IOptions<ReglasNegocioOptions> reglas)
        {
            _ventaRepository = ventaRepository;
            _funcionRepository = funcionRepository;
            _clienteRepository = clienteRepository;
            _catalogoRepository = catalogoRepository;
            _funcionService = funcionService;
            _precioService = precioService;
            _reglas = reglas.Value;
        }

        // ===================== Consultas =====================

        public async Task<IEnumerable<VentaListadoDTO>> GetVentasAsync(UsuarioActual usuario)
        {
            if (usuario.EsAdmin) return await _ventaRepository.GetAllAsync();
            if (usuario.IdCliente == null) return Array.Empty<VentaListadoDTO>();
            return await _ventaRepository.GetAllAsync(usuario.IdCliente);
        }

        public async Task<VentaDetalleDTO> GetDetalleAsync(int id, UsuarioActual usuario)
        {
            var venta = await _ventaRepository.GetDetalleAsync(id)
                ?? throw new NoEncontradoException($"No existe una venta con ID {id}.");

            if (!usuario.EsAdmin && venta.IdCliente != usuario.IdCliente)
                throw new AccesoDenegadoException("Solo podés ver tus propias compras.");

            return venta;
        }

        public Task<ResumenVentasDTO> GetTotalesAsync() => _ventaRepository.GetTotalesAsync();

        // ===================== Registrar venta =====================

        public async Task<int> RegistrarAsync(VentaCompletaDTO dto, UsuarioActual usuario)
        {
            if (dto?.Entradas == null || dto.Entradas.Count == 0)
                throw new NegocioException("La venta tiene que tener al menos una entrada.");
            if (dto.Entradas.Count > _reglas.MaxEntradasPorCompra)
                throw new NegocioException($"No se pueden vender más de {_reglas.MaxEntradasPorCompra} entradas en una misma operación.");

            // 1. ¿A nombre de quién y por qué canal?
            var (cliente, idCanal) = await ResolverClienteYCanalAsync(dto, usuario);
            var clienteEsMenor = EsMenorDeEdad(cliente.IdClienteNavigation?.FechaNacimiento);

            // 2. Validar cada entrada y calcular su precio
            var tiposValidos = (await _catalogoRepository.GetTiposEspectadorAsync()).Select(t => t.Id).ToHashSet();
            var funciones = new Dictionary<int, Funcion>();
            var butacasPorFuncion = new Dictionary<int, Dictionary<int, ButacaEstadoDTO>>();
            var elegidas = new HashSet<(int, int)>();
            var entradas = new List<Entrada>();

            foreach (var e in dto.Entradas)
            {
                var funcion = await ObtenerFuncionValidaAsync(e.IdFuncion, funciones);

                if (!butacasPorFuncion.TryGetValue(funcion.IdFuncion, out var butacas))
                {
                    butacas = (await _funcionRepository.GetButacasAsync(funcion.IdFuncion)).ToDictionary(b => b.IdButaca);
                    butacasPorFuncion[funcion.IdFuncion] = butacas;
                }

                if (e.IdButaca == null || !butacas.TryGetValue(e.IdButaca.Value, out var butaca))
                    throw new NegocioException($"La butaca {e.IdButaca} no pertenece a la sala de la función.");

                var nombreButaca = $"{butaca.Fila}{butaca.Numero}";
                if (!butaca.Habilitada)
                    throw new NegocioException($"La butaca {nombreButaca} no está habilitada.");
                if (butaca.Ocupada || !elegidas.Add((funcion.IdFuncion, butaca.IdButaca)))
                    throw new ConflictoException($"La butaca {nombreButaca} ya está ocupada para esa función.");

                if (e.IdTipoEspectador == null || !tiposValidos.Contains(e.IdTipoEspectador.Value))
                    throw new NegocioException($"La entrada de la butaca {nombreButaca} no tiene un tipo de espectador válido.");

                // Regla +18
                if (_funcionService.EsParaMayores(funcion))
                {
                    var titulo = funcion.IdPeliculaNavigation?.Titulo;
                    if (e.IdTipoEspectador == _reglas.IdTipoEspectadorMenor)
                        throw new NegocioException($"\"{titulo}\" es apta para mayores de {_reglas.EdadMayoria}: no se pueden vender entradas de Menor.");
                    if (clienteEsMenor)
                        throw new NegocioException($"\"{titulo}\" es apta para mayores de {_reglas.EdadMayoria} y el cliente es menor de edad.");
                }

                entradas.Add(new Entrada
                {
                    IdFuncion = funcion.IdFuncion,
                    IdButaca = butaca.IdButaca,
                    IdTipoEspectador = e.IdTipoEspectador,
                    // El precio SIEMPRE lo calcula el servidor (promo o 15% jubilado/menor)
                    PrecioFinal = _precioService.PrecioEntrada(funcion, e.IdTipoEspectador.Value)
                });
            }

            // 3. Formas de pago
            var total = entradas.Sum(x => x.PrecioFinal ?? 0m);
            var pagos = ValidarFormasPago(dto.FormasPago, total, await _catalogoRepository.GetFormasPagoAsync());

            // 4. Guardar maestro + detalle en una transacción
            var venta = new Venta
            {
                IdCliente = cliente.IdCliente,
                IdEmpleado = usuario.EsAdmin ? dto.IdEmpleado : null,
                IdCanal = idCanal,
                FechaHora = DateTime.Now
            };

            return await _ventaRepository.RegistrarVentaAsync(venta, entradas, pagos)
                ?? throw new ConflictoException("Una de las butacas se acaba de vender. Elegí otra y volvé a intentar.");
        }

        private async Task<(Cliente cliente, int idCanal)> ResolverClienteYCanalAsync(VentaCompletaDTO dto, UsuarioActual usuario)
        {
            int idCliente;
            int idCanal;

            if (usuario.EsAdmin)
            {
                idCliente = dto.IdCliente ?? throw new NegocioException("Seleccioná el cliente de la venta.");
                idCanal = dto.IdCanal ?? _reglas.IdCanalBoleteria;
            }
            else
            {
                idCliente = usuario.IdCliente ?? throw new NegocioException("Tu usuario no tiene un cliente asociado para registrar compras.");
                idCanal = _reglas.IdCanalWeb;
            }

            var cliente = await _clienteRepository.GetByIdAsync(idCliente);
            if (cliente == null || !cliente.Activo)
                throw new NegocioException("El cliente no existe o está dado de baja.");

            return (cliente, idCanal);
        }

        private async Task<Funcion> ObtenerFuncionValidaAsync(int? idFuncion, Dictionary<int, Funcion> cache)
        {
            if (idFuncion == null) throw new NegocioException("Hay una entrada sin función.");
            if (cache.TryGetValue(idFuncion.Value, out var enCache)) return enCache;

            var funcion = await _funcionRepository.GetByIdAsync(idFuncion.Value);
            if (funcion == null || !funcion.Activo)
                throw new NegocioException($"La función {idFuncion} no existe o fue cancelada.");
            if (_funcionService.YaEmpezo(funcion))
                throw new NegocioException(
                    $"La función de \"{funcion.IdPeliculaNavigation?.Titulo}\" ({funcion.Fecha:dd/MM} {funcion.HoraInicio:HH\\:mm}) ya empezó: no se pueden vender entradas.");

            cache[funcion.IdFuncion] = funcion;
            return funcion;
        }

        private static List<VentaxFormaPago> ValidarFormasPago(List<FormaPagoVentaDTO>? formas, decimal total, IEnumerable<ItemDTO> formasValidas)
        {
            var validas = formasValidas.Select(f => f.Id).ToHashSet();
            var pagos = (formas ?? new()).Where(f => f.IdFormaPago != null).ToList();

            if (pagos.Count == 0)
                throw new NegocioException("Indicá al menos una forma de pago.");
            if (pagos.Any(p => !validas.Contains(p.IdFormaPago!.Value)))
                throw new NegocioException("Hay una forma de pago que no existe.");

            // Una sola forma de pago sin monto: paga el total
            if (pagos.Count == 1 && (pagos[0].Monto ?? 0) == 0)
                pagos[0].Monto = total;

            if (pagos.Sum(p => p.Monto ?? 0m) != total)
                throw new NegocioException($"La suma de las formas de pago no coincide con el total (${total:N2}).");

            return pagos.Select(p => new VentaxFormaPago { IdFormaPago = p.IdFormaPago, Monto = p.Monto }).ToList();
        }

        private bool EsMenorDeEdad(DateOnly? nacimiento)
        {
            if (nacimiento == null) return false; // sin dato no se puede saber: no se bloquea
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var edad = hoy.Year - nacimiento.Value.Year;
            if (nacimiento.Value > hoy.AddYears(-edad)) edad--;
            return edad < _reglas.EdadMayoria;
        }
    }
}
