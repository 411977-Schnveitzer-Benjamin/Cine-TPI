using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class PeliculaRepository : IPeliculaRepository
    {
        private readonly SistemaCineContext _context;

        // Inyectamos el DbContext generado por el Scaffold
        public PeliculaRepository(SistemaCineContext context)
        {
            _context = context;
        }

        // 1. Traer solo las películas activas (Para el listado del ABM)
        public async Task<IEnumerable<Pelicula>> GetAllActiveAsync()
        {
            return await _context.Pelicula
                .Where(p => p.Activo) // Filtramos por el campo BIT que agregamos
                .Include(p => p.IdClasificacionNavigation) // Trae los datos de la tabla Clasificación de forma automática
                .ToListAsync();
        }
        public async Task<IEnumerable<Pelicula>> GetByNombreAsync(string nombre)
        {
            return await _context.Pelicula
                .Where(p => p.Activo && p.Titulo.ToLower().Contains(nombre.ToLower()))
                .Include(p => p.IdClasificacionNavigation)
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
            pelicula.Activo = true; // Nos aseguramos de que nazca activa
            _context.Pelicula.Add(pelicula);
            return await _context.SaveChangesAsync() > 0; // Devuelve true si se impactó la DB
        }

        // 4. Modificar una película existente
        public async Task<bool> UpdateAsync(Pelicula pelicula)
        {
            _context.Pelicula.Update(pelicula);
            return await _context.SaveChangesAsync() > 0;
        }

        // 5. BORRADO LÓGICO: Cambia el estado a inactivo (0)
        public async Task<bool> DeleteLogicoAsync(int id)
        {
            var pelicula = await _context.Pelicula.FindAsync(id);
            if (pelicula == null) return false;

            pelicula.Activo = false; // Desactivación lógica
            _context.Pelicula.Update(pelicula);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}