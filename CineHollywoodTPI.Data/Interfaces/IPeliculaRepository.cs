using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IPeliculaRepository
    {
        Task<IEnumerable<Pelicula>> GetAllActiveAsync(); // Trae solo las películas con activo = 1
        Task<IEnumerable<Pelicula>> GetByNombreAsync(string nombre);
        Task<Pelicula?> GetByIdAsync(int id);
        Task<bool> AddAsync(Pelicula pelicula);
        Task<bool> UpdateAsync(Pelicula pelicula);
        Task<bool> DeleteLogicoAsync(int id); // Cambia el campo activo a 0
    }
}