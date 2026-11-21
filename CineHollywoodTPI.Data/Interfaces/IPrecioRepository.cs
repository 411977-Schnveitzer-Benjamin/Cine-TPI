using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IPrecioRepository
    {
        Task<IEnumerable<PrecioTipoSala>> GetAllAsync();
        Task<PrecioTipoSala?> GetByIdAsync(int idPrecio);
        Task<List<PrecioTipoSala>> GetPorTipoSalaAsync(int idTipoSala);

        Task<int> CrearAsync(PrecioTipoSala nuevo);

        Task EliminarAsync(PrecioTipoSala precio);
    }
}
