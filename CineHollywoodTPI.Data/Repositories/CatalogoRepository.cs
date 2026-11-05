using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class CatalogoRepository : ICatalogoRepository
    {
        private readonly SistemaCineContext _context;

        public CatalogoRepository(SistemaCineContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemDTO>> GetClasificacionesAsync() =>
            await _context.Clasificaciones
                .OrderBy(c => c.Descripcion)
                .Select(c => new ItemDTO { Id = c.IdClasificacion, Descripcion = c.Descripcion })
                .ToListAsync();

        public async Task<IEnumerable<ItemDTO>> GetFormatosAsync() =>
            await _context.Formatos
                .OrderBy(f => f.Descripcion)
                .Select(f => new ItemDTO { Id = f.IdFormato, Descripcion = f.Descripcion })
                .ToListAsync();

        public async Task<IEnumerable<ItemDTO>> GetTiposEspectadorAsync() =>
            await _context.TipoEspectadors
                .OrderBy(t => t.IdTipoEspectador)
                .Select(t => new ItemDTO { Id = t.IdTipoEspectador, Descripcion = t.Descripcion })
                .ToListAsync();

        public async Task<IEnumerable<ItemDTO>> GetFormasPagoAsync() =>
            await _context.FormaPagos
                .OrderBy(f => f.IdFormaPago)
                .Select(f => new ItemDTO { Id = f.IdFormaPago, Descripcion = f.Descripcion })
                .ToListAsync();

        public async Task<IEnumerable<ItemDTO>> GetCanalesAsync() =>
            await _context.CanalVenta
                .OrderBy(c => c.IdCanal)
                .Select(c => new ItemDTO { Id = c.IdCanal, Descripcion = c.Descripcion })
                .ToListAsync();

        public async Task<IEnumerable<SalaDTO>> GetSalasAsync() =>
            await _context.Salas
                .OrderBy(s => s.Nombre)
                .Select(s => new SalaDTO
                {
                    IdSala = s.IdSala,
                    Nombre = s.Nombre,
                    TipoSala = s.IdTipoSalaNavigation != null ? s.IdTipoSalaNavigation.Descripcion : null,
                    CantidadButacas = s.Butacas.Count()
                })
                .ToListAsync();

        public async Task<IEnumerable<ItemDTO>> GetTiposSalaAsync() =>
            await _context.TipoSalas
                .OrderBy(t => t.Descripcion)
                .Select(t => new ItemDTO { Id = t.IdTipoSala, Descripcion = t.Descripcion })
                .ToListAsync();

        public async Task<IEnumerable<PromocionDTO>> GetPromocionesAsync() =>
            await _context.Promociones
                .OrderBy(p => p.Descripcion)
                .Select(p => new PromocionDTO
                {
                    IdPromocion = p.IdPromocion,
                    Descripcion = p.Descripcion,
                    PorcentajeDescuento = p.PorcentajeDescuento
                })
                .ToListAsync();
    }
}
