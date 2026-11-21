using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface IReporteService
    {
        Task<IEnumerable<PeliculaMasVendidaDTO>> GetPeliculasMasVendidasAsync(int top, DateTime? desde, DateTime? hasta);
        Task<IEnumerable<VentaMensualDTO>> GetVentasMensualesAsync(int? anio);
        Task<IEnumerable<int>> GetAniosAsync();
    }
}
