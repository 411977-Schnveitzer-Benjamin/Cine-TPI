using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IPeliculaRepository
    {
        Task<IEnumerable<Pelicula>> GetAllActiveAsync(); // Trae solo las películas con activo = 1
        Task<IEnumerable<Pelicula>> GetByNombreAsync(string nombre);
        Task<IEnumerable<Pelicula>> GetEnCarteleraAsync(DateOnly desde); // Activas con funciones desde esa fecha
        Task<IEnumerable<Pelicula>> GetProximosEstrenosAsync(DateOnly desde); // Activas SIN funciones desde esa fecha
        Task<Pelicula?> GetByIdAsync(int id);
        Task<bool> AddAsync(Pelicula pelicula);
        Task<bool> UpdateAsync(Pelicula pelicula);
        Task<bool> DeleteLogicoAsync(int id); // Cambia el campo activo a 0
    }
}
