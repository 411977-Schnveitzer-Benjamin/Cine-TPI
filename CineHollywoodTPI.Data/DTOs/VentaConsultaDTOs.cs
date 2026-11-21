namespace CineHollywoodTPI.Data.DTOs
{
    public class VentaListadoDTO
    {
        public int IdVenta { get; set; }
        public DateTime? FechaHora { get; set; }
        public int? IdCliente { get; set; }
        public string? Cliente { get; set; }
        public string? Canal { get; set; }
        public string? FormasPago { get; set; }
        public int CantidadEntradas { get; set; }
        public decimal Total { get; set; }
    }

    public class VentaDetalleDTO : VentaListadoDTO
    {
        public List<EntradaDetalleDTO> Entradas { get; set; } = new();
    }

    public class EntradaDetalleDTO
    {
        public int IdEntrada { get; set; }
        public int? IdFuncion { get; set; }
        public string? Pelicula { get; set; }
        public DateOnly? Fecha { get; set; }
        public TimeOnly? HoraInicio { get; set; }
        public string? Sala { get; set; }
        public string? Butaca { get; set; }
        public string? TipoEspectador { get; set; }
        public decimal? PrecioFinal { get; set; }
    }
}
