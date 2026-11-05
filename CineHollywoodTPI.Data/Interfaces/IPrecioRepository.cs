using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IPrecioRepository
    {
        Task<IEnumerable<PrecioDTO>> GetAllAsync();

        // Carga un precio nuevo para un tipo de sala desde una fecha.
        // El precio que estaba vigente se cierra el día anterior (en una transacción).
        // Lanza InvalidOperationException si los datos no son válidos.
        Task<int> CrearAsync(int idTipoSala, decimal precio, DateOnly fechaDesde);

        // Elimina un precio que todavía no empezó (o empieza hoy) y reabre el anterior.
        Task<bool> EliminarAsync(int idPrecio);
    }
}
