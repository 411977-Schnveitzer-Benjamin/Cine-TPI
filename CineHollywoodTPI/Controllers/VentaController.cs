using System.Security.Claims;
using CineHollywoodTPI.API.Services;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Todas las operaciones de venta requieren estar logueado
    public class VentaController : ControllerBase
    {
        private const int CanalBoleteria = 1;
        private const int CanalWeb = 2;

        private readonly IVentaRepository _ventaRepository;
        private readonly IClienteRepository _clienteRepository;

        public VentaController(IVentaRepository ventaRepository, IClienteRepository clienteRepository)
        {
            _ventaRepository = ventaRepository;
            _clienteRepository = clienteRepository;
        }

        private bool EsAdmin => User.IsInRole("Administrador");

        private int? IdClienteDelToken =>
            int.TryParse(User.FindFirstValue(ClaimsCine.IdCliente), out var id) ? id : null;

        // GET: api/venta  -> admin ve todas; un cliente ve solo las suyas
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (EsAdmin)
                    return Ok(await _ventaRepository.GetAllAsync());

                if (IdClienteDelToken is not int idCliente)
                    return Ok(Array.Empty<VentaListadoDTO>());

                return Ok(await _ventaRepository.GetAllAsync(idCliente));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/venta/totales
        [Authorize(Roles = "Administrador")]
        [HttpGet("totales")]
        public async Task<IActionResult> GetTotalesNegocio()
        {
            try
            {
                return Ok(await _ventaRepository.GetTotalesAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al calcular los totales del negocio: {ex.Message}");
            }
        }

        // GET: api/venta/5  -> maestro + detalle
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var venta = await _ventaRepository.GetDetalleAsync(id);
                if (venta == null)
                    return NotFound($"No existe una venta con ID {id}.");

                // Un cliente solo puede ver sus propias compras
                if (!EsAdmin && venta.IdCliente != IdClienteDelToken)
                    return Forbid();

                return Ok(venta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por ID: {ex.Message}");
            }
        }

        // POST: api/venta  -> TRANSACCIÓN maestro-detalle
        // - Cliente logueado: la venta se hace a su nombre por el canal Web.
        // - Administrador: vende en boletería y tiene que indicar el cliente.
        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VentaCompletaDTO dto)
        {
            try
            {
                if (dto == null || dto.Entradas == null || dto.Entradas.Count == 0)
                    return BadRequest("La venta tiene que tener al menos una entrada.");

                if (dto.Entradas.Count > 10)
                    return BadRequest("No se pueden vender más de 10 entradas en una misma operación.");

                int? idCliente;
                int idCanal;

                if (EsAdmin)
                {
                    idCliente = dto.IdCliente;
                    idCanal = dto.IdCanal ?? CanalBoleteria;

                    if (idCliente == null)
                        return BadRequest("Seleccioná el cliente de la venta.");

                    var cliente = await _clienteRepository.GetByIdAsync(idCliente.Value);
                    if (cliente == null || !cliente.Activo)
                        return BadRequest("El cliente seleccionado no existe o está dado de baja.");
                }
                else
                {
                    idCliente = IdClienteDelToken;
                    idCanal = CanalWeb;

                    if (idCliente == null)
                        return BadRequest("Tu usuario no tiene un cliente asociado para registrar compras.");
                }

                var venta = new Venta
                {
                    IdCliente = idCliente,
                    IdEmpleado = EsAdmin ? dto.IdEmpleado : null,
                    IdCanal = idCanal,
                    FechaHora = DateTime.Now
                };

                // El precio NO se toma del navegador: lo calcula el repositorio
                var entradas = dto.Entradas.Select(e => new Entrada
                {
                    IdFuncion = e.IdFuncion,
                    IdButaca = e.IdButaca,
                    IdTipoEspectador = e.IdTipoEspectador
                }).ToList();

                var formasPago = (dto.FormasPago ?? new List<FormaPagoVentaDTO>())
                    .Where(fp => fp.IdFormaPago != null)
                    .Select(fp => new VentaxFormaPago
                    {
                        IdFormaPago = fp.IdFormaPago,
                        Monto = fp.Monto
                    }).ToList();

                var idVenta = await _ventaRepository.RegistrarVentaAsync(venta, entradas, formasPago);

                return Ok(new { mensaje = "Venta registrada con éxito.", idVenta });
            }
            catch (InvalidOperationException ex)
            {
                // Errores de validación de negocio (butaca ocupada, función inexistente, etc.)
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al registrar la venta. La operación fue revertida. {ex.Message}");
            }
        }
    }
}
