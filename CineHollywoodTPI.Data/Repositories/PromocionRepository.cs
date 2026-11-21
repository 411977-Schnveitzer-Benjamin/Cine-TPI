using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class PromocionRepository : IPromocionRepository
    {
        private readonly SistemaCineContext _context;

        public PromocionRepository(SistemaCineContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PromocionDetalleDTO>> GetAllAsync(DateOnly hoy) =>
            await _context.Promociones
                .OrderBy(p => p.Descripcion)
                .Select(p => new PromocionDetalleDTO
                {
                    IdPromocion = p.IdPromocion,
                    Descripcion = p.Descripcion,
                    PorcentajeDescuento = p.PorcentajeDescuento,
                    CantidadFunciones = p.Funcions.Count(),
                    FuncionesProximas = p.Funcions.Count(f => f.Activo && f.Fecha >= hoy)
                })
                .ToListAsync();

        public async Task<Promocion?> GetByIdAsync(int idPromocion) =>
            await _context.Promociones.FirstOrDefaultAsync(p => p.IdPromocion == idPromocion);

        public async Task<bool> ExisteDescripcionAsync(string descripcion, int? excluirIdPromocion)
        {
            var texto = descripcion.Trim();
            return await _context.Promociones.AnyAsync(p =>
                p.Descripcion == texto && (excluirIdPromocion == null || p.IdPromocion != excluirIdPromocion));
        }

        public async Task<int> CrearAsync(Promocion promocion)
        {
            _context.Promociones.Add(promocion);
            await _context.SaveChangesAsync();
            return promocion.IdPromocion;
        }

        public async Task ActualizarAsync(Promocion promocion)
        {
            await _context.SaveChangesAsync();
        }

        public async Task<int> EliminarAsync(Promocion promocion)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var funciones = await _context.Funcion
                    .Where(f => f.IdPromocion == promocion.IdPromocion)
                    .ToListAsync();
                foreach (var f in funciones)
                    f.IdPromocion = null;

                _context.Promociones.Remove(promocion);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return funciones.Count;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
