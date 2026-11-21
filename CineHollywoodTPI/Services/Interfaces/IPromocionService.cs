using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface IPromocionService
    {
        Task<IEnumerable<PromocionDetalleDTO>> GetAllAsync();
        Task<int> CrearAsync(PromocionGuardarDTO dto);
        Task ModificarAsync(int id, PromocionGuardarDTO dto);
        Task<int> EliminarAsync(int id);
    }
}
