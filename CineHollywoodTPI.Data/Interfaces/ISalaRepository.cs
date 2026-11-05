using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface ISalaRepository
    {
        Task<IEnumerable<SalaDetalleDTO>> GetAllAsync();
        Task<IEnumerable<ItemDTO>> GetComplejosAsync();

        // Crea la sala y genera sus butacas (filas x butacas por fila) en una transacción
        Task<int> CrearAsync(SalaGuardarDTO dto);

        // Modifica nombre/tipo/complejo. La distribución de butacas solo se puede
        // cambiar si la sala no tiene entradas vendidas ni reservas.
        Task<bool> ActualizarAsync(int idSala, SalaGuardarDTO dto);

        // Solo si la sala no tiene funciones cargadas
        Task<bool> EliminarAsync(int idSala);
    }
}
