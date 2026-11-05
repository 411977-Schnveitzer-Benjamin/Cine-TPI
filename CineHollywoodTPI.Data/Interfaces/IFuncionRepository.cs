using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IFuncionRepository
    {
        // Funciones activas; opcionalmente filtradas por película y/o desde una fecha
        Task<IEnumerable<Funcion>> GetAllActiveAsync(int? idPelicula = null, DateOnly? desde = null);
        Task<Funcion?> GetByIdAsync(int id);
        Task<IEnumerable<ButacaEstadoDTO>> GetButacasAsync(int idFuncion); // Mapa de butacas libre/ocupada
        Task<bool> ExisteSuperposicionAsync(Funcion funcion); // Otra función en la misma sala y horario
        Task<bool> AddAsync(Funcion funcion);
        Task<bool> UpdateAsync(Funcion funcion);
        Task<bool> DeleteLogicoAsync(int id); // Cambia el campo activo a 0
    }
}
