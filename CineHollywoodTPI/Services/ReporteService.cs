using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;

namespace CineHollywoodTPI.API.Services
{
    // Reglas de reportes:
    //  - Top entre 1 y 50; "desde" no puede ser mayor que "hasta".
    //  - Año entre 2000 y el año que viene (si no se indica, el actual).
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository _reporteRepository;

        public ReporteService(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        public async Task<IEnumerable<PeliculaMasVendidaDTO>> GetPeliculasMasVendidasAsync(int top, DateTime? desde, DateTime? hasta)
        {
            if (top < 1 || top > 50)
                throw new NegocioException("La cantidad del ranking tiene que estar entre 1 y 50.");
            if (desde.HasValue && hasta.HasValue && desde > hasta)
                throw new NegocioException("La fecha 'desde' no puede ser mayor que 'hasta'.");

            return await _reporteRepository.GetPeliculasMasVendidasAsync(top, desde, hasta);
        }

        public async Task<IEnumerable<VentaMensualDTO>> GetVentasMensualesAsync(int? anio)
        {
            var a = anio ?? DateTime.Today.Year;
            if (a < 2000 || a > DateTime.Today.Year + 1)
                throw new NegocioException("El año no es válido.");

            return await _reporteRepository.GetVentasMensualesAsync(a);
        }

        public async Task<IEnumerable<int>> GetAniosAsync()
        {
            var anios = (await _reporteRepository.GetAniosConVentasAsync()).ToList();
            if (!anios.Contains(DateTime.Today.Year)) anios.Add(DateTime.Today.Year);
            return anios.OrderByDescending(a => a).ToList();
        }
    }
}
