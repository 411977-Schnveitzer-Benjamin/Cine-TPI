using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    // Solo acceso a datos de VENTA. Las validaciones y precios están en VentaService.
    public interface IVentaRepository
    {
        Task<IEnumerable<VentaListadoDTO>> GetAllAsync(int? idCliente = null); // Listado (maestro)
        Task<VentaDetalleDTO?> GetDetalleAsync(int id); // Maestro + detalle de entradas
        Task<ResumenVentasDTO> GetTotalesAsync(); // Totales del negocio para dashboard

        // Transacción Maestro-Detalle: VENTA + ENTRADAs + VentaxFormaPago.
        // Dentro de la transacción vuelve a verificar que ninguna butaca se haya vendido
        // mientras tanto (dos compras simultáneas). Si pasa eso hace ROLLBACK y devuelve null.
        Task<int?> RegistrarVentaAsync(Venta venta, IEnumerable<Entrada> entradas, IEnumerable<VentaxFormaPago> formasPago);
    }
}
