using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    // Solo acceso a datos de FUNCION. Las reglas de negocio están en FuncionService.
    public interface IFuncionRepository
    {
        // Funciones activas; opcionalmente filtradas por película y/o desde una fecha
        Task<IEnumerable<Funcion>> GetAllActiveAsync(int? idPelicula = null, DateOnly? desde = null);
        Task<Funcion?> GetByIdAsync(int id);
        Task<IEnumerable<ButacaEstadoDTO>> GetButacasAsync(int idFuncion); // Butacas de la sala con libre/ocupada
        Task<IEnumerable<Funcion>> GetActivasDeSalaEnFechaAsync(int idSala, DateOnly fecha, int? excluirIdFuncion);
        Task<bool> TieneEntradasVendidasAsync(int idFuncion);
        Task<HashSet<int>> GetIdsConEntradasVendidasAsync(IEnumerable<int> idsFuncion);
        Task<bool> AddAsync(Funcion funcion);
        Task<bool> UpdateAsync(Funcion funcion);
        Task<bool> DeleteLogicoAsync(int id); // Cambia el campo activo a 0
    }
}
