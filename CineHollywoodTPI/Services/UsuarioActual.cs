using System.Security.Claims;

namespace CineHollywoodTPI.API.Services
{
    public record UsuarioActual(int IdUsuario, bool EsAdmin, int? IdCliente)
    {
        public static UsuarioActual Desde(ClaimsPrincipal user) => new(
            int.TryParse(user.FindFirstValue(ClaimsCine.IdUsuario), out var idU) ? idU : 0,
            user.IsInRole("Administrador"),
            int.TryParse(user.FindFirstValue(ClaimsCine.IdCliente), out var idC) ? idC : null);
    }
}
