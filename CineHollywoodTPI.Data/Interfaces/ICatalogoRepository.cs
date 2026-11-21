using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.Data.Interfaces
{
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
