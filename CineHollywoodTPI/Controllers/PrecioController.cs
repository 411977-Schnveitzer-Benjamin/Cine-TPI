using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // Gestión de precios de las entradas por tipo de sala (solo Administrador)
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class PrecioController : ControllerBase
    {
        private readonly IPrecioRepository _precioRepository;

        public PrecioController(IPrecioRepository precioRepository)
        {
            _precioRepository = precioRepository;
        }

        // GET: api/precio
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await _precioRepository.GetAllAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los precios: {ex.Message}");
            }
        }

        // POST: api/precio   { idTipoSala, precio, fechaDesde }
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PrecioInsertDTO dto)
        {
            try
            {
                if (dto == null) return BadRequest("Los datos del precio son inválidos.");

                var id = await _precioRepository.CrearAsync(dto.IdTipoSala, dto.Precio, dto.FechaDesde);
                return Ok(new { mensaje = "Precio cargado con éxito.", id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al guardar el precio: {ex.Message}");
            }
        }

        // DELETE: api/precio/5  (solo precios que todavía no empezaron o empiezan hoy)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                if (!await _precioRepository.EliminarAsync(id))
                    return NotFound($"No existe el precio con ID {id}.");

                return Ok(new { mensaje = "Precio eliminado." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al eliminar el precio: {ex.Message}");
            }
        }
    }
}
