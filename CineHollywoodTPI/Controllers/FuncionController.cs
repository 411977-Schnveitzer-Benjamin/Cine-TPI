using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionController : ControllerBase
    {
        private readonly IFuncionService _funcionService;

        public FuncionController(IFuncionService funcionService)
        {
            _funcionService = funcionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivas([FromQuery] int? idPelicula = null, [FromQuery] bool soloProximas = false) =>
            Ok(await _funcionService.GetActivasAsync(idPelicula, soloProximas));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id) => Ok(await _funcionService.GetPorIdAsync(id));

        [HttpGet("{id:int}/butacas")]
        public async Task<IActionResult> GetButacas(int id) => Ok(await _funcionService.GetButacasAsync(id));

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] FuncionInsertDTO dto)
        {
            var id = await _funcionService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id }, new { mensaje = "Función creada con éxito!", id });
        }

        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] FuncionUpdateDTO dto)
        {
            await _funcionService.ModificarAsync(id, dto);
            return Ok(new { mensaje = "Función modificada con éxito!" });
        }

        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Cancelar(int id)
        {
            await _funcionService.CancelarAsync(id);
            return Ok(new { mensaje = "Función cancelada correctamente." });
        }
    }
}
