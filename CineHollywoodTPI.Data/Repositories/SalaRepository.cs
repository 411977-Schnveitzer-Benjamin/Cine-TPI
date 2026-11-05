using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class SalaRepository : ISalaRepository
    {
        private readonly SistemaCineContext _context;

        public SalaRepository(SistemaCineContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalaDetalleDTO>> GetAllAsync()
        {
            var salas = await _context.Salas
                .OrderBy(s => s.Nombre)
                .Select(s => new
                {
                    s.IdSala,
                    s.Nombre,
                    s.IdTipoSala,
                    TipoSala = s.IdTipoSalaNavigation != null ? s.IdTipoSalaNavigation.Descripcion : null,
                    s.IdComplejo,
                    Complejo = s.IdComplejoNavigation != null ? s.IdComplejoNavigation.Nombre : null,
                    Filas = s.Butacas.Select(b => b.Fila).Distinct().Count(),
                    MaxNumero = s.Butacas.Max(b => (int?)b.Numero) ?? 0,
                    CantidadButacas = s.Butacas.Count(),
                    CantidadFunciones = s.Funcions.Count(f => f.Activo),
                    TieneVentas = s.Butacas.Any(b => b.Entrada.Any() || b.Reservas.Any())
                })
                .ToListAsync();

            return salas.Select(s => new SalaDetalleDTO
            {
                IdSala = s.IdSala,
                Nombre = s.Nombre,
                IdTipoSala = s.IdTipoSala,
                TipoSala = s.TipoSala,
                IdComplejo = s.IdComplejo,
                Complejo = s.Complejo,
                CantidadButacas = s.CantidadButacas,
                CantidadFilas = s.Filas,
                ButacasPorFila = s.MaxNumero,
                CantidadFunciones = s.CantidadFunciones,
                TieneEntradasVendidas = s.TieneVentas
            }).ToList();
        }

        public async Task<IEnumerable<ItemDTO>> GetComplejosAsync() =>
            await _context.Complejos
                .OrderBy(c => c.Nombre)
                .Select(c => new ItemDTO { Id = c.IdComplejo, Descripcion = c.Nombre })
                .ToListAsync();

        public async Task<Sala?> GetByIdAsync(int idSala) =>
            await _context.Salas
                .Include(s => s.Butacas)
                .FirstOrDefaultAsync(s => s.IdSala == idSala);

        public async Task<bool> ExisteTipoSalaAsync(int idTipoSala) =>
            await _context.TipoSalas.AnyAsync(t => t.IdTipoSala == idTipoSala);

        public async Task<bool> ExisteComplejoAsync(int idComplejo) =>
            await _context.Complejos.AnyAsync(c => c.IdComplejo == idComplejo);

        public async Task<bool> ExisteNombreAsync(string nombre, int? idComplejo, int? excluirIdSala)
        {
            var n = nombre.Trim().ToLower();
            return await _context.Salas.AnyAsync(s =>
                s.IdSala != excluirIdSala &&
                s.IdComplejo == idComplejo &&
                s.Nombre!.ToLower() == n);
        }

        public async Task<bool> TieneButacasUsadasAsync(int idSala) =>
            await _context.Entrada.AnyAsync(e => e.IdButacaNavigation != null && e.IdButacaNavigation.IdSala == idSala)
            || await _context.Reservas.AnyAsync(r => r.IdButacaNavigation != null && r.IdButacaNavigation.IdSala == idSala);

        public async Task<bool> TieneFuncionesAsync(int idSala) =>
            await _context.Funcion.AnyAsync(f => f.IdSala == idSala);

        public async Task<int> CrearAsync(Sala sala)
        {
            // Sala + todas sus butacas se insertan en un solo SaveChanges (atómico)
            _context.Salas.Add(sala);
            await _context.SaveChangesAsync();
            return sala.IdSala;
        }

        public async Task ActualizarAsync(Sala sala, IEnumerable<Butaca>? nuevasButacas)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (nuevasButacas != null)
                {
                    _context.Butacas.RemoveRange(sala.Butacas);
                    foreach (var b in nuevasButacas)
                        sala.Butacas.Add(b);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task EliminarAsync(Sala sala)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Butacas.RemoveRange(sala.Butacas);
                _context.Salas.Remove(sala);
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
