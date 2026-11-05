using CineHollywoodTPI.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // Reportes para el dashboard del administrador
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ReporteController : ControllerBase
    {
        private readonly IReporteRepository _reporteRepository;

        public ReporteController(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        // GET: api/reporte/peliculas-mas-vendidas?top=5&desde=2026-01-01&hasta=2026-12-31
        [HttpGet("peliculas-mas-vendidas")]
        public async Task<IActionResult> PeliculasMasVendidas([FromQuery] int top = 5, [FromQuery] DateTime? desde = null, [FromQuery] DateTime? hasta = null)
        {
            try
            {
                if (top < 1 || top > 50) top = 5;
                return Ok(await _reporteRepository.GetPeliculasMasVendidasAsync(top, desde, hasta));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el reporte: {ex.Message}");
            }
        }

        // GET: api/reporte/ventas-mensuales?anio=2026
        [HttpGet("ventas-mensuales")]
        public async Task<IActionResult> VentasMensuales([FromQuery] int? anio = null)
        {
            try
            {
                return Ok(await _reporteRepository.GetVentasMensualesAsync(anio ?? DateTime.Today.Year));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar el reporte: {ex.Message}");
            }
        }

        // GET: api/reporte/anios
        [HttpGet("anios")]
        public async Task<IActionResult> Anios()
        {
            var anios = (await _reporteRepository.GetAniosConVentasAsync()).ToList();
            if (!anios.Contains(DateTime.Today.Year)) anios.Insert(0, DateTime.Today.Year);
            return Ok(anios);
        }
    }
}
