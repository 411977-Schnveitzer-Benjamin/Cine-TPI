using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface IFuncionService
    {
        Task<IEnumerable<FuncionDTO>> GetActivasAsync(int? idPelicula, bool soloProximas);
        Task<FuncionDTO> GetPorIdAsync(int id);
        Task<IEnumerable<ButacaEstadoDTO>> GetButacasAsync(int idFuncion);
        Task<int> CrearAsync(FuncionInsertDTO dto);
        Task ModificarAsync(int id, FuncionUpdateDTO dto);
        Task CancelarAsync(int id);

        // Reglas reutilizadas por VentaService
        bool YaEmpezo(Funcion funcion);
        bool EsParaMayores(Funcion funcion);
    }
}
