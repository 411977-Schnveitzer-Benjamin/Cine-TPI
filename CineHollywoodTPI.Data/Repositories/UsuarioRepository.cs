using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public const string RolAdministrador = "Administrador";
        public const string RolCliente = "Cliente";
        private const int EstadoActivo = 1;

        private readonly SistemaCineContext _context;

        public UsuarioRepository(SistemaCineContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            var normalizado = email.Trim().ToLower();
            return await _context.Usuarios
                .Include(u => u.IdRolNavigation)
                .Include(u => u.IdEstadoUsuarioNavigation)
                .Include(u => u.IdPersonaNavigation!).ThenInclude(p => p.Cliente)
                .FirstOrDefaultAsync(u => u.Email!.ToLower() == normalizado);
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            var normalizado = email.Trim().ToLower();
            return await _context.Usuarios.AnyAsync(u => u.Email!.ToLower() == normalizado);
        }

        public async Task<int?> GetIdRolAsync(string nombreRol)
        {
            return await _context.RolUsuarios
                .Where(r => r.Nombre == nombreRol)
                .Select(r => (int?)r.IdRol)
                .FirstOrDefaultAsync();
        }

        public async Task<Usuario> RegistrarClienteAsync(Persona datosPersona, string email, string passwordHash)
        {
            var idRolCliente = await GetIdRolAsync(RolCliente)
                ?? throw new InvalidOperationException($"No existe el rol '{RolCliente}' en la tabla ROL_USUARIO.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // ¿Ya existe la persona (por DNI)? Ej: un cliente cargado antes en boletería
                var persona = await _context.Personas
                    .Include(p => p.Cliente)
                    .Include(p => p.Usuario)
                    .FirstOrDefaultAsync(p => p.NroDocumento == datosPersona.NroDocumento);

                if (persona?.Usuario != null)
                    throw new InvalidOperationException("Ya existe una cuenta registrada con ese DNI.");

                if (persona == null)
                {
                    persona = datosPersona;
                    _context.Personas.Add(persona);
                }
                else
                {
                    persona.Nombre = datosPersona.Nombre;
                    persona.Apellido = datosPersona.Apellido;
                    persona.FechaNacimiento ??= datosPersona.FechaNacimiento;
                }

                if (persona.Cliente == null)
                {
                    persona.Cliente = new Cliente { Activo = true, EsJubilado = false };
                }
                else
                {
                    persona.Cliente.Activo = true;
                }

                var usuario = new Usuario
                {
                    IdPersonaNavigation = persona,
                    IdRol = idRolCliente,
                    Email = email.Trim().ToLower(),
                    PasswordHash = passwordHash,
                    IdEstadoUsuario = EstadoActivo,
                    FechaRegistro = DateTime.Now
                };
                _context.Usuarios.Add(usuario);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Recargamos con rol y cliente para devolverlo completo
                return (await GetByEmailAsync(usuario.Email))!;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CrearAdminSiNoExisteAsync(string email, string passwordHash)
        {
            var idRolAdmin = await GetIdRolAsync(RolAdministrador);
            if (idRolAdmin == null) return false;

            if (await _context.Usuarios.AnyAsync(u => u.IdRol == idRolAdmin))
                return false;

            _context.Usuarios.Add(new Usuario
            {
                IdPersonaNavigation = new Persona { Nombre = "Admin", Apellido = "Sistema" },
                IdRol = idRolAdmin,
                Email = email.Trim().ToLower(),
                PasswordHash = passwordHash,
                IdEstadoUsuario = EstadoActivo,
                FechaRegistro = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
