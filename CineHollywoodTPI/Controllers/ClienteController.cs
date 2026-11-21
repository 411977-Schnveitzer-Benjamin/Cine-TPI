using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivos() => Ok(await _clienteService.GetActivosAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id) => Ok(await _clienteService.GetPorIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ClienteInsertDTO dto)
        {
            var id = await _clienteService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id }, new { mensaje = "Cliente creado con éxito!", id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] ClienteUpdateDTO dto)
        {
            await _clienteService.ModificarAsync(id, dto);
            return Ok(new { mensaje = "Cliente modificado con éxito!" });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            await _clienteService.DarDeBajaAsync(id);
            return Ok(new { mensaje = "Cliente dado de baja correctamente." });
        }
    }
}
