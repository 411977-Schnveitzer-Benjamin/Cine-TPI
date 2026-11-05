using CineHollywoodTPI.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // Reportes del dashboard (solo Administrador). Las validaciones están en ReporteService.
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

        // GET: api/reporte/peliculas-mas-vendidas?top=5&desde=2026-01-01&hasta=2026-12-31
        [HttpGet("peliculas-mas-vendidas")]
        public async Task<IActionResult> PeliculasMasVendidas([FromQuery] int top = 5, [FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null) =>
            Ok(await _reporteService.GetPeliculasMasVendidasAsync(top, desde, hasta));

        // GET: api/reporte/ventas-mensuales?anio=2026
        [HttpGet("ventas-mensuales")]
        public async Task<IActionResult> VentasMensuales([FromQuery] int? anio = null) =>
            Ok(await _reporteService.GetVentasMensualesAsync(anio));

        // GET: api/reporte/anios
        [HttpGet("anios")]
        public async Task<IActionResult> Anios() => Ok(await _reporteService.GetAniosAsync());
    }
}
