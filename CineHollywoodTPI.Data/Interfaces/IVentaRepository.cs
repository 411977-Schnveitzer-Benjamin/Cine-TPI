using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Interfaces
{
    public interface IVentaRepository
    {
        Task<IEnumerable<VentaListadoDTO>> GetAllAsync(int? idCliente = null); // Listado (maestro)
        Task<VentaDetalleDTO?> GetDetalleAsync(int id); // Maestro + detalle de entradas
        Task<ResumenVentasDTO> GetTotalesAsync(); // Totales del negocio para dashboard

        // Transacción Maestro-Detalle: VENTA + ENTRADAs + VENTAxFORMA_PAGO.
        // Valida butacas y calcula precios del lado del servidor. Devuelve el id de la venta creada.
        // Si algo no es válido lanza InvalidOperationException y no se guarda nada (rollback).
        Task<int> RegistrarVentaAsync(
            Venta venta,
            IEnumerable<Entrada> entradas,
            IEnumerable<VentaxFormaPago>? formasPago = null);
    }
}
