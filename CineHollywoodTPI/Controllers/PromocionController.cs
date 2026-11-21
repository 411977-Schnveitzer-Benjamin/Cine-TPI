using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class PromocionController : ControllerBase
    {
        private readonly IPromocionService _promocionService;

        public PromocionController(IPromocionService promocionService)
        {
            _promocionService = promocionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _promocionService.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] PromocionGuardarDTO dto)
        {
            var id = await _promocionService.CrearAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new { mensaje = "Promoción creada.", id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] PromocionGuardarDTO dto)
        {
            await _promocionService.ModificarAsync(id, dto);
            return Ok(new { mensaje = "Promoción modificada con éxito." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var desvinculadas = await _promocionService.EliminarAsync(id);
            var detalle = desvinculadas switch
            {
                0 => "",
                1 => " Se quitó de 1 función.",
                _ => $" Se quitó de {desvinculadas} funciones."
            };
            return Ok(new { mensaje = "Promoción eliminada." + detalle });
        }
    }
}
