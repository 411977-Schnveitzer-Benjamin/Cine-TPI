using Microsoft.AspNetCore.Mvc;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models; // Asegúrate de que este using apunte a tus modelos mapeados

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // La URL base será: api/pelicula
    public class PeliculaController : ControllerBase
    {
        private readonly IPeliculaRepository _peliculaRepository;

        // Inyectamos el repositorio registrado en el Program.cs
        public PeliculaController(IPeliculaRepository peliculaRepository)
        {
            _peliculaRepository = peliculaRepository;
        }

        // ============================================================================
        // 1. CONSULTA - TRAER TODAS LAS PELÍCULAS ACTIVAS
        // ============================================================================
        // GET: api/pelicula
        [HttpGet]
        public async Task<IActionResult> GetActivas()
        {
            try
            {
                // Recuerda verificar si en tu contexto o repositorio figura en plural o singular (ej. Pelicula o Peliculas)
                var peliculas = await _peliculaRepository.GetAllActiveAsync();

                // Mapeamos los modelos pesados de la DB a estructuras DTO livianas para el navegador
                var listaDto = peliculas.Select(p => new PeliculaDTO
                {
                    IdPelicula = p.IdPelicula,
                    Titulo = p.Titulo,
                    DuracionMinutos = p.DuracionMinutos,
                    ClasificacionDescripcion = p.IdClasificacionNavigation?.Descripcion,
                    Activo = p.Activo
                });

                return Ok(listaDto); // Retorna código HTTP 200 con el listado JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // ============================================================================
        // 1.B. CONSULTA - OBTENER UNA PELÍCULA POR SU ID EXACTO
        // ============================================================================
        // GET: api/pelicula/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            try
            {
                var p = await _peliculaRepository.GetByIdAsync(id);
                if (p == null || !p.Activo)
                    return NotFound($"No se encontró ninguna película activa con el ID {id}.");

                // Transformamos el resultado único a DTO
                var dto = new PeliculaDTO
                {
                    IdPelicula = p.IdPelicula,
                    Titulo = p.Titulo,
                    DuracionMinutos = p.DuracionMinutos,
                    ClasificacionDescripcion = p.IdClasificacionNavigation?.Descripcion,
                    Activo = p.Activo
                };

                return Ok(dto); // Devuelve un objeto JSON único, no una lista
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por ID: {ex.Message}");
            }
        }

        // ============================================================================
        // 1.C. CONSULTA - FILTRAR PELÍCULAS POR NOMBRE / TÍTULO
        // ============================================================================
        // GET: api/pelicula/buscar?nombre=batman
        [HttpGet("buscar")]
        public async Task<IActionResult> GetPorNombre([FromQuery] string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return BadRequest("El parámetro de búsqueda 'nombre' no puede estar vacío.");

                var peliculas = await _peliculaRepository.GetByNombreAsync(nombre);

                var listaDto = peliculas.Select(p => new PeliculaDTO
                {
                    IdPelicula = p.IdPelicula,
                    Titulo = p.Titulo,
                    DuracionMinutos = p.DuracionMinutos,
                    ClasificacionDescripcion = p.IdClasificacionNavigation?.Descripcion,
                    Activo = p.Activo
                });

                return Ok(listaDto); // Devuelve la lista de coincidencias
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por nombre: {ex.Message}");
            }
        }

        // ============================================================================
        // 2. ALTA - CREAR UNA NUEVA PELÍCULA
        // ============================================================================
        // POST: api/pelicula
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PeliculaInsertDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Los datos enviados para la nueva película son inválidos.");

                // Convertimos los datos puros del DTO al modelo formal de la base de datos
                var nuevaPelicula = new Pelicula
                {
                    Titulo = dto.Titulo,
                    DuracionMinutos = dto.DuracionMinutos,
                    IdClasificacion = dto.IdClasificacion
                    // El repositorio se encarga de asignarle Activo = true por defecto al insertar
                };

                var resultado = await _peliculaRepository.AddAsync(nuevaPelicula);
                if (!resultado)
                    return StatusCode(500, "No se pudo impactar el registro en la base de datos.");

                return Ok(new { mensaje = "Película creada con éxito!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar guardar: {ex.Message}");
            }
        }

        // ============================================================================
        // 3. MODIFICACIÓN - EDITAR UNA PELÍCULA EXISTENTE
        // ============================================================================
        // PUT: api/pelicula/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] PeliculaUpdateDTO dto)
        {
            try
            {
                // Validación de seguridad elemental: que el ID de la URL coincida con el ID del cuerpo JSON
                if (dto == null || id != dto.IdPelicula)
                    return BadRequest("Los identificadores de la petición no coinciden.");

                // Corroboramos si el registro existe físicamente en SQL Server antes de operar
                var peliculaExistente = await _peliculaRepository.GetByIdAsync(id);
                if (peliculaExistente == null)
                    return NotFound($"No se encontró ninguna película con el ID {id}.");

                // Traspasamos las modificaciones del DTO al modelo original
                peliculaExistente.Titulo = dto.Titulo;
                peliculaExistente.DuracionMinutos = dto.DuracionMinutos;
                peliculaExistente.IdClasificacion = dto.IdClasificacion;

                var resultado = await _peliculaRepository.UpdateAsync(peliculaExistente);
                if (!resultado)
                    return StatusCode(500, "No se pudieron persistir los cambios de la película.");

                return Ok(new { mensaje = "Película modificada con éxito!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar actualizar: {ex.Message}");
            }
        }

        // ============================================================================
        // 4. BAJA LÓGICA - DESACTIVAR UNA PELÍCULA
        // ============================================================================
        // DELETE: api/pelicula/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            try
            {
                // Invoca la rutina del repositorio que conmuta el campo BIT activo a 0
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
