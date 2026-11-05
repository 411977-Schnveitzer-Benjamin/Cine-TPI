using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteDTO>> GetActivosAsync();
        Task<ClienteDTO> GetPorIdAsync(int id);
        Task<int> CrearAsync(ClienteInsertDTO dto);
        Task ModificarAsync(int id, ClienteUpdateDTO dto);
        Task DarDeBajaAsync(int id);
    }
}
