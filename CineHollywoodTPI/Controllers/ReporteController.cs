using CineHollywoodTPI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("peliculas-mas-vendidas")]
        public async Task<IActionResult> PeliculasMasVendidas([FromQuery] int top = 5, [FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null) =>
            Ok(await _reporteService.GetPeliculasMasVendidasAsync(top, desde, hasta));

        [HttpGet("ventas-mensuales")]
        public async Task<IActionResult> VentasMensuales([FromQuery] int? anio = null) =>
            Ok(await _reporteService.GetVentasMensualesAsync(anio));

        [HttpGet("anios")]
        public async Task<IActionResult> Anios() => Ok(await _reporteService.GetAniosAsync());
    }
}
