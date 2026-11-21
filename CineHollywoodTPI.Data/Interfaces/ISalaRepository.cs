using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface ISalaRepository
    {
        Task<IEnumerable<SalaDetalleDTO>> GetAllAsync();
        Task<IEnumerable<ItemDTO>> GetComplejosAsync();
        Task<Sala?> GetByIdAsync(int idSala);

        Task<bool> ExisteTipoSalaAsync(int idTipoSala);
        Task<bool> ExisteComplejoAsync(int idComplejo);
        Task<bool> ExisteNombreAsync(string nombre, int? idComplejo, int? excluirIdSala);
        Task<bool> TieneButacasUsadasAsync(int idSala);
        Task<bool> TieneFuncionesAsync(int idSala);

        Task<int> CrearAsync(Sala sala);
        Task ActualizarAsync(Sala sala, IEnumerable<Butaca>? nuevasButacas);
        Task EliminarAsync(Sala sala);
    }
}
