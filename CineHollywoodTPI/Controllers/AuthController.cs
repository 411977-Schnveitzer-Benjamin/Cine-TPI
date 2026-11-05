using System.Security.Claims;
using CineHollywoodTPI.API.Services;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private const int EstadoActivo = 1;

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly TokenService _tokenService;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public AuthController(IUsuarioRepository usuarioRepository, TokenService tokenService, IPasswordHasher<Usuario> passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        // POST: api/auth/login  ->  devuelve el JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Ingresá el email y la contraseña.");

            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);

            // Mismo mensaje para "no existe" y "contraseña incorrecta" (no damos pistas)
            if (usuario == null || string.IsNullOrEmpty(usuario.PasswordHash) || !PasswordValida(usuario, dto.Password))
                return Unauthorized("Email o contraseña incorrectos.");

            if (usuario.IdEstadoUsuario != EstadoActivo)
                return Unauthorized("El usuario está inactivo.");

            return Ok(_tokenService.GenerarToken(usuario));
        }

        // POST: api/auth/registro  ->  crea Persona + Cliente + Usuario y devuelve el JWT (queda logueado)
        [HttpPost("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroDTO dto)
        {
            if (dto == null ||
                string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Apellido) ||
                string.IsNullOrWhiteSpace(dto.NroDocumento) ||
                string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Completá todos los campos obligatorios.");

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                return BadRequest("La contraseña tiene que tener al menos 6 caracteres.");

            if (await _usuarioRepository.EmailExisteAsync(dto.Email))
                return Conflict("Ya existe una cuenta con ese email.");

            try
            {
                var persona = new Persona
                {
                    Nombre = dto.Nombre.Trim(),
                    Apellido = dto.Apellido.Trim(),
                    NroDocumento = dto.NroDocumento.Trim(),
                    FechaNacimiento = dto.FechaNacimiento
                };

                var hash = _passwordHasher.HashPassword(null!, dto.Password);
                var usuario = await _usuarioRepository.RegistrarClienteAsync(persona, dto.Email, hash);

                return Ok(_tokenService.GenerarToken(usuario));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al registrar: {ex.Message}");
            }
        }

        // GET: api/auth/yo  ->  sirve para verificar que el token sigue siendo válido
        [Authorize]
        [HttpGet("yo")]
        public IActionResult Yo()
        {
            return Ok(new
            {
                idUsuario = User.FindFirstValue(ClaimsCine.IdUsuario),
                nombre = User.Identity?.Name,
                email = User.FindFirstValue(ClaimTypes.Email),
                rol = User.FindFirstValue(ClaimTypes.Role),
                idCliente = User.FindFirstValue(ClaimsCine.IdCliente)
            });
        }

        private bool PasswordValida(Usuario usuario, string password)
        {
            try
            {
                var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash!, password);
                return resultado != PasswordVerificationResult.Failed;
            }
            catch (FormatException)
            {
                // Hash con formato inválido (ej. datos de prueba "HASH_FAKE_123")
                return false;
            }
        }
    }
}
