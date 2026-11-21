using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IPeliculaRepository
    {
        Task<IEnumerable<Pelicula>> GetAllActiveAsync();
        Task<IEnumerable<Pelicula>> GetByNombreAsync(string nombre);
        Task<IEnumerable<Pelicula>> GetEnCarteleraAsync(DateOnly desde);
        Task<IEnumerable<Pelicula>> GetProximosEstrenosAsync(DateOnly desde);
        Task<Pelicula?> GetByIdAsync(int id);
        Task<bool> AddAsync(Pelicula pelicula);
        Task<bool> UpdateAsync(Pelicula pelicula);
        Task<bool> DeleteLogicoAsync(int id);
    }
}
