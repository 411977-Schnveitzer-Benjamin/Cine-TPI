using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    // Solo acceso a datos de USUARIO / PERSONA. Las reglas están en AuthService.
    public interface IUsuarioRepository
    {
        // Trae el usuario con su Persona, Cliente, Rol y Estado (para el login / armar el token)
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> EmailExisteAsync(string email);
        Task<int?> GetIdRolAsync(string nombreRol);
        Task<bool> ExisteUsuarioConRolAsync(int idRol);

        // Persona por DNI con su Cliente y su Usuario (para reutilizarla al registrarse)
        Task<Persona?> GetPersonaPorDniAsync(string nroDocumento);

        // Guarda el usuario (y su persona/cliente si son nuevos o se modificaron) en una transacción
        Task AgregarAsync(Usuario usuario);
    }
}
