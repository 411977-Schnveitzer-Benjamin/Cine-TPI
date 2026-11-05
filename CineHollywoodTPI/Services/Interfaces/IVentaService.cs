using CineHollywoodTPI.Data.DTOs;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface IVentaService
    {
        Task<IEnumerable<VentaListadoDTO>> GetVentasAsync(UsuarioActual usuario);
        Task<VentaDetalleDTO> GetDetalleAsync(int id, UsuarioActual usuario);
        Task<ResumenVentasDTO> GetTotalesAsync();
        Task<int> RegistrarAsync(VentaCompletaDTO dto, UsuarioActual usuario);
    }
}
