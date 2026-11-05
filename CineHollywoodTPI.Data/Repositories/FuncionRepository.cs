using CineHollywoodTPI.Data.DTOs;
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

        // Query base con todo lo necesario para mostrar la función, calcular su precio
        // y validar la clasificación de la película
        private IQueryable<Funcion> QueryCompleta() =>
            _context.Funcion
                .Include(f => f.IdPeliculaNavigation!)
                    .ThenInclude(p => p.IdClasificacionNavigation)
                .Include(f => f.IdFormatoNavigation)
                .Include(f => f.IdPromocionNavigation)
                .Include(f => f.IdSalaNavigation!)
                    .ThenInclude(s => s.IdTipoSalaNavigation!)
                        .ThenInclude(t => t.PrecioTipoSalas);

        public async Task<IEnumerable<Funcion>> GetAllActiveAsync(int? idPelicula = null, DateOnly? desde = null)
        {
            var query = QueryCompleta().Where(f => f.Activo);

            if (idPelicula.HasValue)
                query = query.Where(f => f.IdPelicula == idPelicula.Value);

            if (desde.HasValue)
                query = query.Where(f => f.Fecha >= desde.Value);

            return await query
                .OrderBy(f => f.Fecha)
                .ThenBy(f => f.HoraInicio)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Funcion?> GetByIdAsync(int id)
        {
            return await QueryCompleta()
                .AsSplitQuery()
                .FirstOrDefaultAsync(f => f.IdFuncion == id);
        }

        public async Task<IEnumerable<ButacaEstadoDTO>> GetButacasAsync(int idFuncion)
        {
            var funcion = await _context.Funcion.FindAsync(idFuncion);
            if (funcion == null) return Enumerable.Empty<ButacaEstadoDTO>();

            var ocupadas = await _context.Entrada
                .Where(e => e.IdFuncion == idFuncion && e.IdButaca != null)
                .Select(e => e.IdButaca!.Value)
                .ToListAsync();

            var butacas = await _context.Butacas
                .Where(b => b.IdSala == funcion.IdSala)
                .OrderBy(b => b.Fila)
                .ThenBy(b => b.Numero)
                .ToListAsync();

            return butacas.Select(b => new ButacaEstadoDTO
            {
                IdButaca = b.IdButaca,
                Fila = b.Fila,
                Numero = b.Numero,
                Habilitada = b.Habilitada ?? true,
                Ocupada = ocupadas.Contains(b.IdButaca)
            }).ToList();
        }

        public async Task<IEnumerable<Funcion>> GetActivasDeSalaEnFechaAsync(int idSala, DateOnly fecha, int? excluirIdFuncion)
        {
            return await _context.Funcion
                .Include(f => f.IdPeliculaNavigation)
                .Where(f => f.Activo &&
                            f.IdSala == idSala &&
                            f.Fecha == fecha &&
                            (excluirIdFuncion == null || f.IdFuncion != excluirIdFuncion))
                .ToListAsync();
        }

        public async Task<bool> TieneEntradasVendidasAsync(int idFuncion)
        {
            return await _context.Entrada.AnyAsync(e => e.IdFuncion == idFuncion);
        }

        public async Task<HashSet<int>> GetIdsConEntradasVendidasAsync(IEnumerable<int> idsFuncion)
        {
            var ids = idsFuncion.ToList();
            var conVentas = await _context.Entrada
                .Where(e => e.IdFuncion != null && ids.Contains(e.IdFuncion.Value))
                .Select(e => e.IdFuncion!.Value)
                .Distinct()
                .ToListAsync();
            return conVentas.ToHashSet();
        }

        public async Task<bool> AddAsync(Funcion funcion)
        {
            funcion.Activo = true;
            _context.Funcion.Add(funcion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Funcion funcion)
        {
            // Si la entidad ya viene trackeada (la trajimos con GetByIdAsync) alcanza con guardar.
            if (_context.Entry(funcion).State == EntityState.Detached)
                _context.Funcion.Update(funcion);

            await _context.SaveChangesAsync(); // 0 filas = no había cambios, no es un error
            return true;
        }

        public async Task<bool> DeleteLogicoAsync(int id)
        {
            var funcion = await _context.Funcion.FindAsync(id);
            if (funcion == null || !funcion.Activo) return false;

            funcion.Activo = false;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
