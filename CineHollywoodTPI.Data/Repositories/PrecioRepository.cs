using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class PrecioRepository : IPrecioRepository
    {
        private readonly SistemaCineContext _context;

        public PrecioRepository(SistemaCineContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PrecioTipoSala>> GetAllAsync() =>
            await _context.PrecioTipoSalas
                .Include(p => p.IdTipoSalaNavigation)
                .OrderBy(p => p.IdTipoSalaNavigation!.Descripcion)
                .ThenByDescending(p => p.FechaDesde)
                .AsNoTracking()
                .ToListAsync();

        public async Task<PrecioTipoSala?> GetByIdAsync(int idPrecio) =>
            await _context.PrecioTipoSalas.FindAsync(idPrecio);

        // Devuelve las entidades trackeadas: el servicio puede modificarles fecha_hasta
        public async Task<List<PrecioTipoSala>> GetPorTipoSalaAsync(int idTipoSala) =>
            await _context.PrecioTipoSalas
                .Where(p => p.IdTipoSala == idTipoSala)
                .OrderBy(p => p.FechaDesde)
                .ToListAsync();

        public async Task<int> CrearAsync(PrecioTipoSala nuevo)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.PrecioTipoSalas.Add(nuevo);
                await _context.SaveChangesAsync(); // guarda también los anteriores modificados
                await transaction.CommitAsync();
                return nuevo.IdPrecio;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task EliminarAsync(PrecioTipoSala precio)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.PrecioTipoSalas.Remove(precio);
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
