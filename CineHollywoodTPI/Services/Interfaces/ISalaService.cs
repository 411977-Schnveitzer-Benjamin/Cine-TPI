using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface ISalaService
    {
        Task<IEnumerable<SalaDetalleDTO>> GetAllAsync();
        Task<IEnumerable<ItemDTO>> GetComplejosAsync();
        Task<int> CrearAsync(SalaGuardarDTO dto);
        Task ModificarAsync(int id, SalaGuardarDTO dto);
        Task EliminarAsync(int id);
    }
}
