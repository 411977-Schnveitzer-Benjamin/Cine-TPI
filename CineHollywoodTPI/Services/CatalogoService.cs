using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;

namespace CineHollywoodTPI.API.Services
{
    // Catálogos de solo lectura. No tienen reglas de negocio, pero pasan por la capa de servicio
    // igual que el resto para respetar la arquitectura: Controller -> Service -> Repository.
    public class CatalogoService : ICatalogoService
    {
        private readonly ICatalogoRepository _catalogo;

        public CatalogoService(ICatalogoRepository catalogo)
        {
            _catalogo = catalogo;
        }

        public Task<IEnumerable<ItemDTO>> GetClasificacionesAsync() => _catalogo.GetClasificacionesAsync();
        public Task<IEnumerable<ItemDTO>> GetFormatosAsync() => _catalogo.GetFormatosAsync();
        public Task<IEnumerable<ItemDTO>> GetTiposEspectadorAsync() => _catalogo.GetTiposEspectadorAsync();
        public Task<IEnumerable<ItemDTO>> GetFormasPagoAsync() => _catalogo.GetFormasPagoAsync();
        public Task<IEnumerable<ItemDTO>> GetCanalesAsync() => _catalogo.GetCanalesAsync();
        public Task<IEnumerable<SalaDTO>> GetSalasAsync() => _catalogo.GetSalasAsync();
        public Task<IEnumerable<ItemDTO>> GetTiposSalaAsync() => _catalogo.GetTiposSalaAsync();
        public Task<IEnumerable<PromocionDTO>> GetPromocionesAsync() => _catalogo.GetPromocionesAsync();
    }
}
