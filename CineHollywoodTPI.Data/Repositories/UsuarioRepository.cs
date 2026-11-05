using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
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

        public async Task<int?> GetIdRolAsync(string nombreRol) =>
            await _context.RolUsuarios
                .Where(r => r.Nombre == nombreRol)
                .Select(r => (int?)r.IdRol)
                .FirstOrDefaultAsync();

        public async Task<bool> ExisteUsuarioConRolAsync(int idRol) =>
            await _context.Usuarios.AnyAsync(u => u.IdRol == idRol);

        public async Task<Persona?> GetPersonaPorDniAsync(string nroDocumento) =>
            await _context.Personas
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.NroDocumento == nroDocumento);

        public async Task AgregarAsync(Usuario usuario)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
