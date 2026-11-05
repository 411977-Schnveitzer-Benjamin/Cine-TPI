using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    // Solo acceso a datos de SALA / BUTACA. Las reglas están en SalaService.
    public interface ISalaRepository
    {
        Task<IEnumerable<SalaDetalleDTO>> GetAllAsync();
        Task<IEnumerable<ItemDTO>> GetComplejosAsync();
        Task<Sala?> GetByIdAsync(int idSala); // incluye butacas

        Task<bool> ExisteTipoSalaAsync(int idTipoSala);
        Task<bool> ExisteComplejoAsync(int idComplejo);
        Task<bool> ExisteNombreAsync(string nombre, int? idComplejo, int? excluirIdSala);
        Task<bool> TieneButacasUsadasAsync(int idSala); // entradas vendidas o reservas
        Task<bool> TieneFuncionesAsync(int idSala);

        Task<int> CrearAsync(Sala sala); // la sala viene con sus butacas
        Task ActualizarAsync(Sala sala, IEnumerable<Butaca>? nuevasButacas); // si vienen butacas, reemplaza las anteriores
        Task EliminarAsync(Sala sala);
    }
}
