using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IPromocionRepository
    {
        Task<IEnumerable<PromocionDetalleDTO>> GetAllAsync(DateOnly hoy);
        Task<Promocion?> GetByIdAsync(int idPromocion);
        Task<bool> ExisteDescripcionAsync(string descripcion, int? excluirIdPromocion);

        Task<int> CrearAsync(Promocion promocion);
        Task ActualizarAsync(Promocion promocion);

        Task<int> EliminarAsync(Promocion promocion);
    }
}
