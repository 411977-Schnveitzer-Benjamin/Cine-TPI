using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IReporteRepository
    {
        Task<IEnumerable<PeliculaMasVendidaDTO>> GetPeliculasMasVendidasAsync(int top, DateTime? desde, DateTime? hasta);

        Task<IEnumerable<VentaMensualDTO>> GetVentasMensualesAsync(int anio);

        Task<IEnumerable<int>> GetAniosConVentasAsync();
    }
}
