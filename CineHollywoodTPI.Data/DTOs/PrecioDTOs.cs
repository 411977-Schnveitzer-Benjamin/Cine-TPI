namespace CineHollywoodTPI.Data.DTOs
{
    public class PrecioDTO
    {
        public int IdPrecio { get; set; }
        public int? IdTipoSala { get; set; }
        public string? TipoSala { get; set; }
        public decimal? Precio { get; set; }
        public DateOnly? FechaDesde { get; set; }
        public DateOnly? FechaHasta { get; set; }
        public string Estado { get; set; } = "";
        public bool SePuedeEliminar { get; set; }
    }

    public class PrecioInsertDTO
    {
        public int IdTipoSala { get; set; }
        public decimal Precio { get; set; }
        public DateOnly FechaDesde { get; set; }
    }
}
