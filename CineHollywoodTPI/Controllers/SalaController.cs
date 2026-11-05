using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // ABM de salas (con generación automática de butacas). Solo Administrador.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class SalaController : ControllerBase
    {
        private readonly ISalaRepository _salaRepository;

        public SalaController(ISalaRepository salaRepository)
        {
            _salaRepository = salaRepository;
        }

        // GET: api/sala
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _salaRepository.GetAllAsync());

        // GET: api/sala/complejos
        [HttpGet("complejos")]
        public async Task<IActionResult> GetComplejos() => Ok(await _salaRepository.GetComplejosAsync());

        // POST: api/sala
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SalaGuardarDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest("Los datos de la sala son inválidos.");
                var id = await _salaRepository.CrearAsync(dto);
                return Ok(new { mensaje = $"Sala creada con {dto.Filas * dto.ButacasPorFila} butacas.", id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al crear la sala: {ex.Message}");
            }
        }

        // PUT: api/sala/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] SalaGuardarDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest("Los datos de la sala son inválidos.");
                if (!await _salaRepository.ActualizarAsync(id, dto))
                    return NotFound($"No existe la sala con ID {id}.");
                return Ok(new { mensaje = "Sala modificada con éxito." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al modificar la sala: {ex.Message}");
            }
        }

        // DELETE: api/sala/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                if (!await _salaRepository.EliminarAsync(id))
                    return NotFound($"No existe la sala con ID {id}.");
                return Ok(new { mensaje = "Sala eliminada." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al eliminar la sala: {ex.Message}");
            }
        }
    }
}
