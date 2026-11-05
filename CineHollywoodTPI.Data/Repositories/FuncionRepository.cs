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

        // Query base con todo lo necesario para mostrar la función y calcular su precio
        private IQueryable<Funcion> QueryCompleta() =>
            _context.Funcion
                .Include(f => f.IdPeliculaNavigation)
                .Include(f => f.IdFormatoNavigation)
                .Include(f => f.IdPromocionNavigation)
                .Include(f => f.IdSalaNavigation)
                    .ThenInclude(s => s!.IdTipoSalaNavigation)
                        .ThenInclude(t => t!.PrecioTipoSalas);

        // 1. Funciones activas (opcional: de una película y/o desde una fecha)
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

        // 2. Buscar una función específica por su ID
        public async Task<Funcion?> GetByIdAsync(int id)
        {
            return await QueryCompleta()
                .AsSplitQuery()
                .FirstOrDefaultAsync(f => f.IdFuncion == id);
        }

        // 3. Butacas de la sala de la función, marcando cuáles ya están vendidas
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
            });
        }

        // 4. Valida que no haya otra función activa en la misma sala que se pise en horario
        public async Task<bool> ExisteSuperposicionAsync(Funcion funcion)
        {
            if (funcion.Fecha == null || funcion.HoraInicio == null || funcion.IdSala == null)
                return false;

            var duracionNueva = await _context.Pelicula
                .Where(p => p.IdPelicula == funcion.IdPelicula)
                .Select(p => p.DuracionMinutos)
                .FirstOrDefaultAsync() ?? 120;

            var otras = await _context.Funcion
                .Include(f => f.IdPeliculaNavigation)
                .Where(f => f.Activo &&
                            f.IdFuncion != funcion.IdFuncion &&
                            f.IdSala == funcion.IdSala &&
                            f.Fecha == funcion.Fecha)
                .ToListAsync();

            var inicioNueva = funcion.Fecha.Value.ToDateTime(funcion.HoraInicio.Value);
            var finNueva = inicioNueva.AddMinutes(duracionNueva);

            return otras.Any(o =>
            {
                if (o.HoraInicio == null) return false;
                var inicio = o.Fecha!.Value.ToDateTime(o.HoraInicio.Value);
                var fin = inicio.AddMinutes(o.IdPeliculaNavigation?.DuracionMinutos ?? 120);
                return inicioNueva < fin && inicio < finNueva;
            });
        }

        // 5. Agregar una nueva función a la grilla
        public async Task<bool> AddAsync(Funcion funcion)
        {
            funcion.Activo = true;
            _context.Funcion.Add(funcion);
            return await _context.SaveChangesAsync() > 0;
        }

        // 6. Modificar horarios o salas de una función
        public async Task<bool> UpdateAsync(Funcion funcion)
        {
            // Si la entidad ya viene trackeada (la trajimos con GetByIdAsync) alcanza con guardar.
            if (_context.Entry(funcion).State == EntityState.Detached)
                _context.Funcion.Update(funcion);

            await _context.SaveChangesAsync(); // 0 filas = no había cambios, no es un error
            return true;
        }

        // 7. Borrado lógico de la función (por ejemplo, si se cancela)
        public async Task<bool> DeleteLogicoAsync(int id)
        {
            var funcion = await _context.Funcion.FindAsync(id);
            if (funcion == null || !funcion.Activo) return false;

            funcion.Activo = false;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
