using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllActiveAsync(); // Clientes con activo = 1
        Task<Cliente?> GetByIdAsync(int id);
        Task<bool> AddAsync(Cliente cliente);
        Task<bool> UpdateAsync(Cliente cliente);
        Task<bool> DeleteLogicoAsync(int id); // Cambia el campo activo a 0
    }
}