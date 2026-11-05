using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IReporteRepository
    {
        // Ranking de películas por cantidad de entradas vendidas (opcional: rango de fechas de venta)
        Task<IEnumerable<PeliculaMasVendidaDTO>> GetPeliculasMasVendidasAsync(int top, DateTime? desde, DateTime? hasta);

        // Ventas agrupadas mes a mes para un año (siempre devuelve los 12 meses)
        Task<IEnumerable<VentaMensualDTO>> GetVentasMensualesAsync(int anio);

        // Años en los que hay ventas (para el selector del dashboard)
        Task<IEnumerable<int>> GetAniosConVentasAsync();
    }
}
