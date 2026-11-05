using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginRespuestaDTO> LoginAsync(LoginDTO dto);
        Task<LoginRespuestaDTO> RegistrarAsync(RegistroDTO dto);
        Task<bool> CrearAdminInicialAsync(string email, string password);
    }
}
