using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.Data.Interfaces
{
    // Tablas chicas de referencia que se usan para llenar los combos del frontend
    public interface ICatalogoRepository
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
