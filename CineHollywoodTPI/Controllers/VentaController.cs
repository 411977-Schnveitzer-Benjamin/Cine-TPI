using CineHollywoodTPI.API.Services;
using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        private UsuarioActual Usuario => UsuarioActual.Desde(User);

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _ventaService.GetVentasAsync(Usuario));

        [Authorize(Roles = "Administrador")]
        [HttpGet("totales")]
        public async Task<IActionResult> GetTotales() => Ok(await _ventaService.GetTotalesAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _ventaService.GetDetalleAsync(id, Usuario));

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VentaCompletaDTO dto)
        {
            var idVenta = await _ventaService.RegistrarAsync(dto, Usuario);
            return CreatedAtAction(nameof(GetById), new { id = idVenta }, new { mensaje = "Venta registrada con éxito.", idVenta });
        }
    }
}
