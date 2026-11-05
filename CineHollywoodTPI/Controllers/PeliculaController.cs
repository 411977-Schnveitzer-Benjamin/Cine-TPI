using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;
using CineHollywoodTPI.API.Services;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // La URL base será: api/pelicula
    public class PeliculaController : ControllerBase
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly PosterService _posterService;

        public PeliculaController(IPeliculaRepository peliculaRepository, PosterService posterService)
        {
            _peliculaRepository = peliculaRepository;
            _posterService = posterService;
        }

        private static PeliculaDTO ToDto(Pelicula p) => new()
        {
            IdPelicula = p.IdPelicula,
            Titulo = p.Titulo ?? "",
            DuracionMinutos = p.DuracionMinutos,
            IdClasificacion = p.IdClasificacion,
            ClasificacionDescripcion = p.IdClasificacionNavigation?.Descripcion,
            PosterUrl = p.PosterUrl,
            Activo = p.Activo
        };

        private static string? Validar(string? titulo, int? duracion)
        {
            if (string.IsNullOrWhiteSpace(titulo)) return "El título es obligatorio.";
            if (titulo.Length > 200) return "El título no puede superar los 200 caracteres.";
            if (duracion is null or <= 0 or > 600) return "La duración tiene que estar entre 1 y 600 minutos.";
            return null;
        }

        // ============================================================================
        // CONSULTAS (públicas)
        // ============================================================================

        // GET: api/pelicula
        [HttpGet]
        public async Task<IActionResult> GetActivas()
        {
            try
            {
                var peliculas = await _peliculaRepository.GetAllActiveAsync();
                return Ok(peliculas.Select(ToDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/pelicula/cartelera  -> con funciones desde hoy
        [HttpGet("cartelera")]
        public async Task<IActionResult> GetCartelera()
        {
            try
            {
                var peliculas = await _peliculaRepository.GetEnCarteleraAsync(DateOnly.FromDateTime(DateTime.Today));
                return Ok(peliculas.Select(ToDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/pelicula/estrenos  -> activas sin funciones programadas
        [HttpGet("estrenos")]
        public async Task<IActionResult> GetEstrenos()
        {
            try
            {
                var peliculas = await _peliculaRepository.GetProximosEstrenosAsync(DateOnly.FromDateTime(DateTime.Today));
                return Ok(peliculas.Select(ToDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/pelicula/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            try
            {
                var p = await _peliculaRepository.GetByIdAsync(id);
                if (p == null || !p.Activo)
                    return NotFound($"No se encontró ninguna película activa con el ID {id}.");

                return Ok(ToDto(p));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por ID: {ex.Message}");
            }
        }

        // GET: api/pelicula/buscar?nombre=batman
        [HttpGet("buscar")]
        public async Task<IActionResult> GetPorNombre([FromQuery] string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return BadRequest("El parámetro de búsqueda 'nombre' no puede estar vacío.");

                var peliculas = await _peliculaRepository.GetByNombreAsync(nombre);
                return Ok(peliculas.Select(ToDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por nombre: {ex.Message}");
            }
        }

        // ============================================================================
        // ABM (solo Administrador)
        // ============================================================================

        // POST: api/pelicula
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PeliculaInsertDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Los datos enviados para la nueva película son inválidos.");

                var error = Validar(dto.Titulo, dto.DuracionMinutos);
                if (error != null) return BadRequest(error);

                var nuevaPelicula = new Pelicula
                {
                    Titulo = dto.Titulo.Trim(),
                    DuracionMinutos = dto.DuracionMinutos,
                    IdClasificacion = dto.IdClasificacion,
                    PosterUrl = string.IsNullOrWhiteSpace(dto.PosterUrl) ? null : dto.PosterUrl.Trim()
                };

                var resultado = await _peliculaRepository.AddAsync(nuevaPelicula);
                if (!resultado)
                    return StatusCode(500, "No se pudo impactar el registro en la base de datos.");

                return Ok(new { mensaje = "Película creada con éxito!", id = nuevaPelicula.IdPelicula });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar guardar: {ex.Message}");
            }
        }

        // PUT: api/pelicula/5
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] PeliculaUpdateDTO dto)
        {
            try
            {
                if (dto == null || id != dto.IdPelicula)
                    return BadRequest("Los identificadores de la petición no coinciden.");

                var error = Validar(dto.Titulo, dto.DuracionMinutos);
                if (error != null) return BadRequest(error);

                var peliculaExistente = await _peliculaRepository.GetByIdAsync(id);
                if (peliculaExistente == null)
                    return NotFound($"No se encontró ninguna película con el ID {id}.");

                peliculaExistente.Titulo = dto.Titulo.Trim();
                peliculaExistente.DuracionMinutos = dto.DuracionMinutos;
                peliculaExistente.IdClasificacion = dto.IdClasificacion;
                var posterAnterior = peliculaExistente.PosterUrl;
                peliculaExistente.PosterUrl = string.IsNullOrWhiteSpace(dto.PosterUrl) ? null : dto.PosterUrl.Trim();

                await _peliculaRepository.UpdateAsync(peliculaExistente);

                if (posterAnterior != peliculaExistente.PosterUrl)
                    _posterService.BorrarSiEsLocal(posterAnterior);
                return Ok(new { mensaje = "Película modificada con éxito!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar actualizar: {ex.Message}");
            }
        }

        // ============================================================================
        // PÓSTER: la imagen se guarda en el servidor (wwwroot/posters de la API)
        // ============================================================================

        // POST: api/pelicula/5/poster   (multipart/form-data, campo "archivo")
        [Authorize(Roles = "Administrador")]
        [HttpPost("{id:int}/poster")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        public async Task<IActionResult> SubirPoster(int id, IFormFile archivo)
        {
            return await ReemplazarPosterAsync(id, () => _posterService.GuardarArchivoAsync(archivo));
        }

        // POST: api/pelicula/5/poster-url   { "url": "https://..." }  -> la API descarga la imagen
        [Authorize(Roles = "Administrador")]
        [HttpPost("{id:int}/poster-url")]
        public async Task<IActionResult> DescargarPoster(int id, [FromBody] PosterUrlDTO dto)
        {
            return await ReemplazarPosterAsync(id, () => _posterService.DescargarDesdeUrlAsync(dto?.Url ?? ""));
        }

        private async Task<IActionResult> ReemplazarPosterAsync(int id, Func<Task<string>> guardar)
        {
            var pelicula = await _peliculaRepository.GetByIdAsync(id);
            if (pelicula == null || !pelicula.Activo)
                return NotFound($"No se encontró ninguna película activa con el ID {id}.");

            try
            {
                var anterior = pelicula.PosterUrl;
                pelicula.PosterUrl = await guardar();
                await _peliculaRepository.UpdateAsync(pelicula);
                _posterService.BorrarSiEsLocal(anterior);

                return Ok(new { mensaje = "Póster guardado.", posterUrl = pelicula.PosterUrl });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException)
            {
                return BadRequest("No se pudo descargar la imagen desde ese link. Probá descargarla y subir el archivo.");
            }
            catch (TaskCanceledException)
            {
                return BadRequest("El sitio de la imagen tardó demasiado en responder. Probá descargarla y subir el archivo.");
            }
        }

        // DELETE: api/pelicula/5  (baja lógica)
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            try
            {
                var resultado = await _peliculaRepository.DeleteLogicoAsync(id);
                if (!resultado)
                    return NotFound($"No se pudo procesar la baja. No existe la película con ID {id} o ya se encuentra inactiva.");

                return Ok(new { mensaje = "Película dada de baja de forma lógica correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al procesar la baja: {ex.Message}");
            }
        }
    }
}
