using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllActiveAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task<bool> AddAsync(Cliente cliente);
        Task<bool> UpdateAsync(Cliente cliente);
        Task<bool> DeleteLogicoAsync(int id);
        Task<bool> ExisteDocumentoAsync(string nroDocumento, int? excluirIdPersona = null);
    }
}
