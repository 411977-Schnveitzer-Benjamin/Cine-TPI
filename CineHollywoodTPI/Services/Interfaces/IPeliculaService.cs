using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface IPeliculaService
    {
        Task<IEnumerable<PeliculaDTO>> GetActivasAsync();
        Task<IEnumerable<PeliculaDTO>> GetCarteleraAsync();
        Task<IEnumerable<PeliculaDTO>> GetEstrenosAsync();
        Task<IEnumerable<PeliculaDTO>> BuscarAsync(string nombre);
        Task<PeliculaDTO> GetPorIdAsync(int id);
        Task<int> CrearAsync(PeliculaInsertDTO dto);
        Task ModificarAsync(int id, PeliculaUpdateDTO dto);
        Task DarDeBajaAsync(int id);
        Task<string> SubirPosterAsync(int id, IFormFile archivo);
        Task<string> DescargarPosterAsync(int id, string url);
    }
}
