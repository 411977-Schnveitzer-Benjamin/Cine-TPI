using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class FuncionRepository : IFuncionRepository
    {
        private readonly SistemaCineContext _context;

        public FuncionRepository(SistemaCineContext context)
        {
            _context = context;
        }

        // 1. Traer funciones activas con todos sus datos relacionados (Crucial para la Landing Page)
        public async Task<IEnumerable<Funcion>> GetAllActiveAsync()
        {
            return await _context.Funcion
                .Where(f => f.Activo)
                .Include(f => f.IdPeliculaNavigation) // Para saber qué película se proyecta
                .Include(f => f.IdSalaNavigation)     // Para saber en qué sala
                .Include(f => f.IdFormatoNavigation)  // Para saber si es 2D, 3D, etc.
                .ToListAsync();
        }

        // 2. Buscar una función específica por su ID
        public async Task<Funcion?> GetByIdAsync(int id)
        {
            return await _context.Funcion
                .Include(f => f.IdPeliculaNavigation)
                .Include(f => f.IdSalaNavigation)
                .Include(f => f.IdFormatoNavigation)
                .FirstOrDefaultAsync(f => f.IdFuncion == id);
        }

        // 3. Agregar una nueva función a la grilla
        public async Task<bool> AddAsync(Funcion funcion)
        {
            funcion.Activo = true; // Nace activa por defecto
            _context.Funcion.Add(funcion);
            return await _context.SaveChangesAsync() > 0;
        }

        // 4. Modificar horarios o salas de una función
        public async Task<bool> UpdateAsync(Funcion funcion)
        {
            _context.Funcion.Update(funcion);
            return await _context.SaveChangesAsync() > 0;
        }

        // 5. Borrado lógico de la función (por ejemplo, si se cancela una función)
        public async Task<bool> DeleteLogicoAsync(int id)
        {
            var funcion = await _context.Funcion.FindAsync(id);
            if (funcion == null) return false;

            funcion.Activo = false; // Desactivación lógica
            _context.Funcion.Update(funcion);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
