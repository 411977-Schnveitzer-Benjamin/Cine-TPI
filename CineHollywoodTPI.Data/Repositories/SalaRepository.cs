using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class SalaRepository : ISalaRepository
    {
        private const int MaxFilas = 26;          // A..Z
        private const int MaxButacasPorFila = 40;

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
            });
        }

        public async Task<IEnumerable<ItemDTO>> GetComplejosAsync() =>
            await _context.Complejos
                .OrderBy(c => c.Nombre)
                .Select(c => new ItemDTO { Id = c.IdComplejo, Descripcion = c.Nombre })
                .ToListAsync();

        private async Task ValidarAsync(SalaGuardarDTO dto, int? idSalaActual)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length > 100)
                throw new InvalidOperationException("El nombre es obligatorio (máx. 100 caracteres).");
            if (!await _context.TipoSalas.AnyAsync(t => t.IdTipoSala == dto.IdTipoSala))
                throw new InvalidOperationException("El tipo de sala no existe.");
            if (dto.IdComplejo != null && !await _context.Complejos.AnyAsync(c => c.IdComplejo == dto.IdComplejo))
                throw new InvalidOperationException("El complejo no existe.");
            if (dto.Filas < 1 || dto.Filas > MaxFilas)
                throw new InvalidOperationException($"La cantidad de filas tiene que estar entre 1 y {MaxFilas}.");
            if (dto.ButacasPorFila < 1 || dto.ButacasPorFila > MaxButacasPorFila)
                throw new InvalidOperationException($"Las butacas por fila tienen que estar entre 1 y {MaxButacasPorFila}.");

            var nombre = dto.Nombre.Trim().ToLower();
            if (await _context.Salas.AnyAsync(s => s.IdSala != idSalaActual &&
                                                   s.IdComplejo == dto.IdComplejo &&
                                                   s.Nombre!.ToLower() == nombre))
                throw new InvalidOperationException("Ya existe una sala con ese nombre en el complejo.");
        }

        private static IEnumerable<Butaca> GenerarButacas(int filas, int porFila) =>
            Enumerable.Range(0, filas).SelectMany(f =>
                Enumerable.Range(1, porFila).Select(n => new Butaca
                {
                    Fila = ((char)('A' + f)).ToString(),
                    Numero = n,
                    Habilitada = true
                }));

        public async Task<int> CrearAsync(SalaGuardarDTO dto)
        {
            await ValidarAsync(dto, null);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var sala = new Sala
                {
                    Nombre = dto.Nombre.Trim(),
                    IdTipoSala = dto.IdTipoSala,
                    IdComplejo = dto.IdComplejo
                };
                foreach (var b in GenerarButacas(dto.Filas, dto.ButacasPorFila))
                    sala.Butacas.Add(b);

                _context.Salas.Add(sala);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return sala.IdSala;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> ActualizarAsync(int idSala, SalaGuardarDTO dto)
        {
            var sala = await _context.Salas
                .Include(s => s.Butacas)
                .FirstOrDefaultAsync(s => s.IdSala == idSala);
            if (sala == null) return false;

            await ValidarAsync(dto, idSala);

            var filasActuales = sala.Butacas.Select(b => b.Fila).Distinct().Count();
            var porFilaActual = sala.Butacas.Max(b => (int?)b.Numero) ?? 0;
            var cambiaDistribucion = filasActuales != dto.Filas || porFilaActual != dto.ButacasPorFila;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                sala.Nombre = dto.Nombre.Trim();
                sala.IdTipoSala = dto.IdTipoSala;
                sala.IdComplejo = dto.IdComplejo;

                if (cambiaDistribucion)
                {
                    var idsButacas = sala.Butacas.Select(b => b.IdButaca).ToList();
                    var usadas = await _context.Entrada.AnyAsync(e => e.IdButaca != null && idsButacas.Contains(e.IdButaca.Value))
                              || await _context.Reservas.AnyAsync(r => r.IdButaca != null && idsButacas.Contains(r.IdButaca.Value));
                    if (usadas)
                        throw new InvalidOperationException(
                            "La sala ya tiene entradas vendidas o reservas: no se puede cambiar la cantidad de butacas. Solo se pueden modificar nombre, tipo y complejo.");

                    _context.Butacas.RemoveRange(sala.Butacas);
                    foreach (var b in GenerarButacas(dto.Filas, dto.ButacasPorFila))
                        sala.Butacas.Add(b);
                }

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

        public async Task<bool> EliminarAsync(int idSala)
        {
            var sala = await _context.Salas
                .Include(s => s.Butacas)
                .FirstOrDefaultAsync(s => s.IdSala == idSala);
            if (sala == null) return false;

            if (await _context.Funcion.AnyAsync(f => f.IdSala == idSala))
                throw new InvalidOperationException("La sala tiene funciones cargadas (aunque estén canceladas o ya hayan pasado), por eso no se puede eliminar.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Butacas.RemoveRange(sala.Butacas);
                _context.Salas.Remove(sala);
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
