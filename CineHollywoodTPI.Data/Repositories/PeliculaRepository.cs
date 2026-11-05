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

        // 1. Traer solo las películas activas (Para el listado del ABM)
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

        // Cartelera = películas activas que tienen al menos una función activa desde hoy
        public async Task<IEnumerable<Pelicula>> GetEnCarteleraAsync(DateOnly desde)
        {
            return await _context.Pelicula
                .Where(p => p.Activo && p.Funcions.Any(f => f.Activo && f.Fecha >= desde))
                .Include(p => p.IdClasificacionNavigation)
                .OrderBy(p => p.Titulo)
                .ToListAsync();
        }

        // Próximos estrenos = películas activas que todavía no tienen funciones programadas
        public async Task<IEnumerable<Pelicula>> GetProximosEstrenosAsync(DateOnly desde)
        {
            return await _context.Pelicula
                .Where(p => p.Activo && !p.Funcions.Any(f => f.Activo && f.Fecha >= desde))
                .Include(p => p.IdClasificacionNavigation)
                .OrderBy(p => p.Titulo)
                .ToListAsync();
        }

        // 2. Buscar una película por su ID
        public async Task<Pelicula?> GetByIdAsync(int id)
        {
            return await _context.Pelicula
                .Include(p => p.IdClasificacionNavigation)
                .FirstOrDefaultAsync(p => p.IdPelicula == id);
        }

        // 3. Insertar una nueva película
        public async Task<bool> AddAsync(Pelicula pelicula)
        {
            pelicula.Activo = true;
            _context.Pelicula.Add(pelicula);
            return await _context.SaveChangesAsync() > 0;
        }

        // 4. Modificar una película existente
        public async Task<bool> UpdateAsync(Pelicula pelicula)
        {
            // Si la entidad ya viene trackeada (la trajimos con GetByIdAsync) alcanza con guardar.
            if (_context.Entry(pelicula).State == EntityState.Detached)
                _context.Pelicula.Update(pelicula);

            await _context.SaveChangesAsync(); // 0 filas = no había cambios, no es un error
            return true;
        }

        // 5. BORRADO LÓGICO: Cambia el estado a inactivo (0)
        public async Task<bool> DeleteLogicoAsync(int id)
        {
            var pelicula = await _context.Pelicula.FindAsync(id);
            if (pelicula == null || !pelicula.Activo) return false;

            pelicula.Activo = false;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
