using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IFuncionRepository
    {
        Task<IEnumerable<Funcion>> GetAllActiveAsync(); // Trae las funciones con activo = 1
        Task<Funcion?> GetByIdAsync(int id);
        Task<bool> AddAsync(Funcion funcion);
        Task<bool> UpdateAsync(Funcion funcion);
        Task<bool> DeleteLogicoAsync(int id); // Cambia el campo activo a 0
    }
}