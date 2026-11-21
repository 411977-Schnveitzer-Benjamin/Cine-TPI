using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IFuncionRepository
    {
        Task<IEnumerable<Funcion>> GetAllActiveAsync(int? idPelicula = null, DateOnly? desde = null);
        Task<Funcion?> GetByIdAsync(int id);
        Task<IEnumerable<ButacaEstadoDTO>> GetButacasAsync(int idFuncion);
        Task<IEnumerable<Funcion>> GetActivasDeSalaEnFechaAsync(int idSala, DateOnly fecha, int? excluirIdFuncion);
        Task<bool> TieneEntradasVendidasAsync(int idFuncion);
        Task<HashSet<int>> GetIdsConEntradasVendidasAsync(IEnumerable<int> idsFuncion);
        Task<bool> AddAsync(Funcion funcion);
        Task<bool> UpdateAsync(Funcion funcion);
        Task<bool> DeleteLogicoAsync(int id);
    }
}
