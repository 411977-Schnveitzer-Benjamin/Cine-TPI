using System.Security.Claims;
using CineHollywoodTPI.API.Services;
using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto) => Ok(await _authService.LoginAsync(dto));

        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroDTO dto) =>
            StatusCode(StatusCodes.Status201Created, await _authService.RegistrarAsync(dto));

        [Authorize]
        [HttpGet("yo")]
        public IActionResult Yo() => Ok(new
        {
            idUsuario = User.FindFirstValue(ClaimsCine.IdUsuario),
            nombre = User.Identity?.Name,
            email = User.FindFirstValue(ClaimTypes.Email),
            rol = User.FindFirstValue(ClaimTypes.Role),
            idCliente = User.FindFirstValue(ClaimsCine.IdCliente)
        });
    }
}
