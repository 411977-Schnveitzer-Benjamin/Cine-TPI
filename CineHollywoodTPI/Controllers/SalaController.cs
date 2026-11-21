using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class SalaController : ControllerBase
    {
        private readonly ISalaService _salaService;

        public SalaController(ISalaService salaService)
        {
            _salaService = salaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _salaService.GetAllAsync());

        [HttpGet("complejos")]
        public async Task<IActionResult> GetComplejos() => Ok(await _salaService.GetComplejosAsync());

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] SalaGuardarDTO dto)
        {
            var id = await _salaService.CrearAsync(dto);
            return StatusCode(StatusCodes.Status201Created, new { mensaje = $"Sala creada con {dto.Filas * dto.ButacasPorFila} butacas.", id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] SalaGuardarDTO dto)
        {
            await _salaService.ModificarAsync(id, dto);
            return Ok(new { mensaje = "Sala modificada con éxito." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _salaService.EliminarAsync(id);
            return Ok(new { mensaje = "Sala eliminada." });
        }
    }
}
