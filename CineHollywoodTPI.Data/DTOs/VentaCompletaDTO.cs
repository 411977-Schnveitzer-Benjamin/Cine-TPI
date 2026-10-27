namespace CineHollywoodTPI.Data.DTOs
{
    public class VentaCompletaDTO
    {
        public int? IdCliente { get; set; }
        public int? IdEmpleado { get; set; }
        public int? IdCanal { get; set; }
        public DateTime? FechaHora { get; set; }

        public List<EntradaVentaDTO> Entradas { get; set; } = new();
        public List<FormaPagoVentaDTO>? FormasPago { get; set; } = new();
    }

    public class EntradaVentaDTO
    {
        public int? IdReserva { get; set; }
        public int? IdFuncion { get; set; }
        public int? IdButaca { get; set; }
        public int? IdTipoEspectador { get; set; }
        public decimal? PrecioFinal { get; set; }
    }

    public class FormaPagoVentaDTO
    {
        public int? IdFormaPago { get; set; }
        public decimal? Monto { get; set; }
    }
}
