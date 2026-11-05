using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    // Listas de solo lectura para los combos del frontend
    public interface ICatalogoService
    {
        Task<IEnumerable<ItemDTO>> GetClasificacionesAsync();
        Task<IEnumerable<ItemDTO>> GetFormatosAsync();
        Task<IEnumerable<ItemDTO>> GetTiposEspectadorAsync();
        Task<IEnumerable<ItemDTO>> GetFormasPagoAsync();
        Task<IEnumerable<ItemDTO>> GetCanalesAsync();
        Task<IEnumerable<SalaDTO>> GetSalasAsync();
        Task<IEnumerable<ItemDTO>> GetTiposSalaAsync();
        Task<IEnumerable<PromocionDTO>> GetPromocionesAsync();
    }
}
