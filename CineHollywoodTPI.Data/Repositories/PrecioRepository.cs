using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    // Precios de las entradas por tipo de sala (tabla PRECIO_TIPO_SALA).
    // Las ventas ya registradas no cambian: cada ENTRADA guarda su precio_final.
    public class PrecioRepository : IPrecioRepository
    {
        private readonly SistemaCineContext _context;

        public PrecioRepository(SistemaCineContext context)
        {
            _context = context;
        }

        private static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Today);

        public async Task<IEnumerable<PrecioDTO>> GetAllAsync()
        {
            var precios = await _context.PrecioTipoSalas
                .Include(p => p.IdTipoSalaNavigation)
                .OrderBy(p => p.IdTipoSalaNavigation!.Descripcion)
                .ThenByDescending(p => p.FechaDesde)
                .AsNoTracking()
                .ToListAsync();

            var hoy = Hoy;
            return precios.Select(p => new PrecioDTO
            {
                IdPrecio = p.IdPrecio,
                IdTipoSala = p.IdTipoSala,
                TipoSala = p.IdTipoSalaNavigation?.Descripcion,
                Precio = p.Precio,
                FechaDesde = p.FechaDesde,
                FechaHasta = p.FechaHasta,
                Estado = p.FechaDesde > hoy ? "Programado"
                       : (p.FechaHasta == null || p.FechaHasta >= hoy) ? "Vigente"
                       : "Histórico",
                SePuedeEliminar = p.FechaDesde >= hoy
            });
        }

        public async Task<int> CrearAsync(int idTipoSala, decimal precio, DateOnly fechaDesde)
        {
            if (precio <= 0 || precio > 99_999_999m)
                throw new InvalidOperationException("El precio tiene que ser mayor a cero.");
            if (fechaDesde < Hoy)
                throw new InvalidOperationException("La fecha de inicio no puede ser anterior a hoy.");
            if (!await _context.TipoSalas.AnyAsync(t => t.IdTipoSala == idTipoSala))
                throw new InvalidOperationException("El tipo de sala no existe.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var delTipo = await _context.PrecioTipoSalas
                    .Where(p => p.IdTipoSala == idTipoSala)
                    .ToListAsync();

                if (delTipo.Any(p => p.FechaDesde >= fechaDesde))
                    throw new InvalidOperationException(
                        "Ya hay un precio cargado que empieza en esa fecha o después. Eliminalo primero o elegí otra fecha.");

                // Cerramos el precio que seguiría vigente en la nueva fecha
                foreach (var anterior in delTipo.Where(p => p.FechaHasta == null || p.FechaHasta >= fechaDesde))
                    anterior.FechaHasta = fechaDesde.AddDays(-1);

                var nuevo = new PrecioTipoSala
                {
                    IdTipoSala = idTipoSala,
                    Precio = Math.Round(precio, 2),
                    FechaDesde = fechaDesde,
                    FechaHasta = null // sin fecha de fin: vale hasta que se cargue otro
                };
                _context.PrecioTipoSalas.Add(nuevo);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return nuevo.IdPrecio;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> EliminarAsync(int idPrecio)
        {
            var precio = await _context.PrecioTipoSalas.FindAsync(idPrecio);
            if (precio == null) return false;

            if (precio.FechaDesde < Hoy)
                throw new InvalidOperationException("No se puede eliminar un precio que ya se aplicó. Cargá uno nuevo en su lugar.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // El precio anterior (el que cerramos al cargar este) vuelve a cubrir el período
                var anterior = await _context.PrecioTipoSalas
                    .Where(p => p.IdTipoSala == precio.IdTipoSala && p.FechaDesde < precio.FechaDesde)
                    .OrderByDescending(p => p.FechaDesde)
                    .FirstOrDefaultAsync();

                if (anterior != null && anterior.FechaHasta == precio.FechaDesde!.Value.AddDays(-1))
                    anterior.FechaHasta = precio.FechaHasta;

                _context.PrecioTipoSalas.Remove(precio);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
