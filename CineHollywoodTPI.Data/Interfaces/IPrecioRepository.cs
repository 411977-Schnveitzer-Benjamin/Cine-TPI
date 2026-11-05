using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    // Solo acceso a datos de PRECIO_TIPO_SALA. Las reglas están en PrecioService.
    public interface IPrecioRepository
    {
        Task<IEnumerable<PrecioTipoSala>> GetAllAsync(); // incluye el tipo de sala
        Task<PrecioTipoSala?> GetByIdAsync(int idPrecio);
        Task<List<PrecioTipoSala>> GetPorTipoSalaAsync(int idTipoSala);

        // Guarda el precio nuevo y los cambios de fecha_hasta de los anteriores en una transacción
        Task<int> CrearAsync(PrecioTipoSala nuevo);

        // Elimina el precio y guarda el cambio del anterior (si se modificó) en una transacción
        Task EliminarAsync(PrecioTipoSala precio);
    }
}
