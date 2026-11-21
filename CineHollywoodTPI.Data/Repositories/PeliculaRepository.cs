using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class PeliculaRepository : IPeliculaRepository
    {
        private readonly SistemaCineContext _context;

        public PeliculaRepository(SistemaCineContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pelicula>> GetAllActiveAsync()
        {
            return await _context.Pelicula
                .Where(p => p.Activo)
                .Include(p => p.IdClasificacionNavigation)
                .OrderBy(p => p.Titulo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pelicula>> GetByNombreAsync(string nombre)
        {
            return await _context.Pelicula
                .Where(p => p.Activo && p.Titulo!.ToLower().Contains(nombre.ToLower()))
                .Include(p => p.IdClasificacionNavigation)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pelicula>> GetEnCarteleraAsync(DateOnly desde)
        {
            return await _context.Pelicula
                .Where(p => p.Activo && p.Funcions.Any(f => f.Activo && f.Fecha >= desde))
                .Include(p => p.IdClasificacionNavigation)
                .OrderBy(p => p.Titulo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pelicula>> GetProximosEstrenosAsync(DateOnly desde)
        {
            return await _context.Pelicula
                .Where(p => p.Activo && !p.Funcions.Any(f => f.Activo && f.Fecha >= desde))
                .Include(p => p.IdClasificacionNavigation)
                .OrderBy(p => p.Titulo)
                .ToListAsync();
        }

        public async Task<Pelicula?> GetByIdAsync(int id)
        {
            return await _context.Pelicula
                .Include(p => p.IdClasificacionNavigation)
                .FirstOrDefaultAsync(p => p.IdPelicula == id);
        }

        public async Task<bool> AddAsync(Pelicula pelicula)
        {
            pelicula.Activo = true;
            _context.Pelicula.Add(pelicula);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Pelicula pelicula)
        {
            if (_context.Entry(pelicula).State == EntityState.Detached)
                _context.Pelicula.Update(pelicula);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLogicoAsync(int id)
        {
            var pelicula = await _context.Pelicula.FindAsync(id);
            if (pelicula == null || !pelicula.Activo) return false;

            pelicula.Activo = false;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
