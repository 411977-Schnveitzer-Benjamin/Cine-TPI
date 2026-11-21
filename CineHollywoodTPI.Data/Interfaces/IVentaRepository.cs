using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IVentaRepository
    {
        Task<IEnumerable<VentaListadoDTO>> GetAllAsync(int? idCliente = null);
        Task<VentaDetalleDTO?> GetDetalleAsync(int id);
        Task<ResumenVentasDTO> GetTotalesAsync();

        Task<int?> RegistrarVentaAsync(Venta venta, IEnumerable<Entrada> entradas, IEnumerable<VentaxFormaPago> formasPago);
    }
}
