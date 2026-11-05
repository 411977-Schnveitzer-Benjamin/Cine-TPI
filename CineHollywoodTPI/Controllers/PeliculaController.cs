using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // Solo HTTP: recibe el pedido, llama al servicio y devuelve la respuesta.
    // Las reglas de negocio están en PeliculaService; los errores los maneja ManejoErroresMiddleware.
    [ApiController]
    [Route("api/[controller]")]
    public class PeliculaController : ControllerBase
    {
        private readonly IPeliculaService _peliculaService;

        public PeliculaController(IPeliculaService peliculaService)
        {
            _peliculaService = peliculaService;
        }

        // GET: api/pelicula
        [HttpGet]
        public async Task<IActionResult> GetActivas() => Ok(await _peliculaService.GetActivasAsync());

        // GET: api/pelicula/cartelera
        [HttpGet("cartelera")]
        public async Task<IActionResult> GetCartelera() => Ok(await _peliculaService.GetCarteleraAsync());

        // GET: api/pelicula/estrenos
        [HttpGet("estrenos")]
        public async Task<IActionResult> GetEstrenos() => Ok(await _peliculaService.GetEstrenosAsync());

        // GET: api/pelicula/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id) => Ok(await _peliculaService.GetPorIdAsync(id));

        // GET: api/pelicula/buscar?nombre=batman
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string nombre) => Ok(await _peliculaService.BuscarAsync(nombre));

        // POST: api/pelicula  -> 201 Created + header Location: /api/pelicula/{id}
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PeliculaInsertDTO dto)
        {
            var id = await _peliculaService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id }, new { mensaje = "Película creada con éxito!", id });
        }

        // PUT: api/pelicula/5
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] PeliculaUpdateDTO dto)
        {
            await _peliculaService.ModificarAsync(id, dto);
            return Ok(new { mensaje = "Película modificada con éxito!" });
        }

        // DELETE: api/pelicula/5  (baja lógica)
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            await _peliculaService.DarDeBajaAsync(id);
            return Ok(new { mensaje = "Película dada de baja correctamente." });
        }

        // POST: api/pelicula/5/poster   (multipart/form-data, campo "archivo")
        [Authorize(Roles = "Administrador")]
        [HttpPost("{id:int}/poster")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> SubirPoster(int id, IFormFile archivo)
        {
            var posterUrl = await _peliculaService.SubirPosterAsync(id, archivo);
            return Ok(new { mensaje = "Póster guardado.", posterUrl });
        }

        // POST: api/pelicula/5/poster-url   { "url": "https://..." }
        [Authorize(Roles = "Administrador")]
        [HttpPost("{id:int}/poster-url")]
        public async Task<IActionResult> DescargarPoster(int id, [FromBody] PosterUrlDTO dto)
        {
            var posterUrl = await _peliculaService.DescargarPosterAsync(id, dto?.Url ?? "");
            return Ok(new { mensaje = "Póster guardado.", posterUrl });
        }
    }
}
