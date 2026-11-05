using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // ABM de clientes (solo Administrador). Las reglas están en ClienteService.
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

        // GET: api/cliente
        [HttpGet]
        public async Task<IActionResult> GetActivos() => Ok(await _clienteService.GetActivosAsync());

        // GET: api/cliente/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id) => Ok(await _clienteService.GetPorIdAsync(id));

        // POST: api/cliente  -> 201 Created
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ClienteInsertDTO dto)
        {
            var id = await _clienteService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id }, new { mensaje = "Cliente creado con éxito!", id });
        }

        // PUT: api/cliente/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] ClienteUpdateDTO dto)
        {
            await _clienteService.ModificarAsync(id, dto);
            return Ok(new { mensaje = "Cliente modificado con éxito!" });
        }

        // DELETE: api/cliente/5  (baja lógica)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            await _clienteService.DarDeBajaAsync(id);
            return Ok(new { mensaje = "Cliente dado de baja correctamente." });
        }
    }
}
