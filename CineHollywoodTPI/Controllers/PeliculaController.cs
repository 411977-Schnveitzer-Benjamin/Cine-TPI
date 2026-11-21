using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeliculaController : ControllerBase
    {
        private readonly IPeliculaService _peliculaService;

        public PeliculaController(IPeliculaService peliculaService)
        {
            _peliculaService = peliculaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivas() => Ok(await _peliculaService.GetActivasAsync());

        [HttpGet("cartelera")]
        public async Task<IActionResult> GetCartelera() => Ok(await _peliculaService.GetCarteleraAsync());

        [HttpGet("estrenos")]
        public async Task<IActionResult> GetEstrenos() => Ok(await _peliculaService.GetEstrenosAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id) => Ok(await _peliculaService.GetPorIdAsync(id));

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string nombre) => Ok(await _peliculaService.BuscarAsync(nombre));

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PeliculaInsertDTO dto)
        {
            var id = await _peliculaService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id }, new { mensaje = "Película creada con éxito!", id });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] PeliculaUpdateDTO dto)
        {
            await _peliculaService.ModificarAsync(id, dto);
            return Ok(new { mensaje = "Película modificada con éxito!" });
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            await _peliculaService.DarDeBajaAsync(id);
            return Ok(new { mensaje = "Película dada de baja correctamente." });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("{id:int}/poster")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> SubirPoster(int id, IFormFile archivo)
        {
            var posterUrl = await _peliculaService.SubirPosterAsync(id, archivo);
            return Ok(new { mensaje = "Póster guardado.", posterUrl });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPost("{id:int}/poster-url")]
        public async Task<IActionResult> DescargarPoster(int id, [FromBody] PosterUrlDTO dto)
        {
            var posterUrl = await _peliculaService.DescargarPosterAsync(id, dto?.Url ?? "");
            return Ok(new { mensaje = "Póster guardado.", posterUrl });
        }
    }
}
