using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly SistemaCineContext _context;

        public ClienteRepository(SistemaCineContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cliente>> GetAllActiveAsync()
        {
            return await _context.Cliente
                .Where(c => c.Activo)
                .Include(c => c.IdClienteNavigation)
                .ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Cliente
                .Include(c => c.IdClienteNavigation)
                .FirstOrDefaultAsync(c => c.IdCliente == id);
        }

        public async Task<bool> AddAsync(Cliente cliente)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                cliente.Activo = true;

                _context.Cliente.Add(cliente);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            if (_context.Entry(cliente).State == EntityState.Detached)
                _context.Cliente.Update(cliente);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLogicoAsync(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null || !cliente.Activo) return false;

            cliente.Activo = false;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExisteDocumentoAsync(string nroDocumento, int? excluirIdPersona = null)
        {
            return await _context.Personas.AnyAsync(p =>
                p.NroDocumento == nroDocumento &&
                (excluirIdPersona == null || p.IdPersona != excluirIdPersona));
        }
    }
}
