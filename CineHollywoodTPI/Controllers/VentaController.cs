using CineHollywoodTPI.API.Services;
using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // Solo HTTP. Toda la lógica de la venta (validaciones, descuentos, transacción) está en VentaService.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // todas las operaciones de venta requieren estar logueado
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        // Quién hace el pedido (sale del token JWT)
        private UsuarioActual Usuario => UsuarioActual.Desde(User);

        // GET: api/venta  -> admin: todas / cliente: solo las suyas
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _ventaService.GetVentasAsync(Usuario));

        // GET: api/venta/totales
        [Authorize(Roles = "Administrador")]
        [HttpGet("totales")]
        public async Task<IActionResult> GetTotales() => Ok(await _ventaService.GetTotalesAsync());

        // GET: api/venta/5  -> maestro + detalle
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _ventaService.GetDetalleAsync(id, Usuario));

        // POST: api/venta  -> transacción maestro-detalle, 201 Created
        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VentaCompletaDTO dto)
        {
            var idVenta = await _ventaService.RegistrarAsync(dto, Usuario);
            return CreatedAtAction(nameof(GetById), new { id = idVenta }, new { mensaje = "Venta registrada con éxito.", idVenta });
        }
    }
}
