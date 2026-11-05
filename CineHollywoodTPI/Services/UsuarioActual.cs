using System.Security.Claims;

namespace CineHollywoodTPI.API.Services
{
    // Datos del usuario logueado que necesitan los servicios.
    // Se arma en el controller a partir del token (así los servicios no dependen de HTTP).
    public record UsuarioActual(int IdUsuario, bool EsAdmin, int? IdCliente)
    {
        public static UsuarioActual Desde(ClaimsPrincipal user) => new(
            int.TryParse(user.FindFirstValue(ClaimsCine.IdUsuario), out var idU) ? idU : 0,
            user.IsInRole("Administrador"),
            int.TryParse(user.FindFirstValue(ClaimsCine.IdCliente), out var idC) ? idC : null);
    }
}
