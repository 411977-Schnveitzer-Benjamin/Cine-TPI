using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;
using Microsoft.IdentityModel.Tokens;

namespace CineHollywoodTPI.API.Services
{
    // Nombres de los claims propios que viajan dentro del token
    public static class ClaimsCine
    {
        public const string IdUsuario = "idUsuario";
        public const string IdCliente = "idCliente";
    }

    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public LoginRespuestaDTO GenerarToken(Usuario usuario)
        {
            var persona = usuario.IdPersonaNavigation;
            var rol = usuario.IdRolNavigation?.Nombre ?? "Cliente";
            var nombre = persona != null ? $"{persona.Nombre} {persona.Apellido}".Trim() : usuario.Email;
            var idCliente = persona?.Cliente?.IdCliente;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Name, nombre ?? ""),
                new(ClaimTypes.Email, usuario.Email ?? ""),
                new(ClaimTypes.Role, rol),
                new(ClaimsCine.IdUsuario, usuario.IdUsuario.ToString())
            };

            if (idCliente.HasValue)
                claims.Add(new Claim(ClaimsCine.IdCliente, idCliente.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expira = DateTime.UtcNow.AddMinutes(_config.GetValue("Jwt:ExpiraMinutos", 120));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expira,
                signingCredentials: credenciales);

            return new LoginRespuestaDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expira = expira,
                IdUsuario = usuario.IdUsuario,
                Email = usuario.Email,
                Nombre = nombre,
                Rol = rol,
                IdCliente = idCliente
            };
        }
    }
}
