using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IUsuarioRepository
    {
        // Trae el usuario con su Persona, Cliente y Rol (para el login / armar el token)
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> EmailExisteAsync(string email);
        Task<int?> GetIdRolAsync(string nombreRol);

        // Alta de un cliente que se registra desde la web: PERSONA + CLIENTE + USUARIO en una transacción.
        // Si ya existe una persona con ese DNI (ej. cargada en boletería) se reutiliza.
        Task<Usuario> RegistrarClienteAsync(Persona datosPersona, string email, string passwordHash);

        // Crea el usuario administrador inicial si todavía no hay ninguno
        Task<bool> CrearAdminSiNoExisteAsync(string email, string passwordHash);
    }
}
