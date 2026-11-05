using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // Precios de las entradas por tipo de sala (solo Administrador). Las reglas están en PrecioService.
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class PrecioController : ControllerBase
    {
        private readonly IPrecioService _precioService;

        public PrecioController(IPrecioService precioService)
        {
            _precioService = precioService;
        }

        // GET: api/precio
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _precioService.GetAllAsync());

        // POST: api/precio   { idTipoSala, precio, fechaDesde }
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PrecioInsertDTO dto)
        {
            var id = await _precioService.CrearAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new { mensaje = "Precio cargado con éxito.", id });
        }

        // DELETE: api/precio/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _precioService.EliminarAsync(id);
            return Ok(new { mensaje = "Precio eliminado." });
        }
    }
}
