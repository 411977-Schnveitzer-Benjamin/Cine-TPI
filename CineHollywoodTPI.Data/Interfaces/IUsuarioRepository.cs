using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> EmailExisteAsync(string email);
        Task<int?> GetIdRolAsync(string nombreRol);
        Task<bool> ExisteUsuarioConRolAsync(int idRol);

        Task<Persona?> GetPersonaPorDniAsync(string nroDocumento);

        Task AgregarAsync(Usuario usuario);
    }
}
