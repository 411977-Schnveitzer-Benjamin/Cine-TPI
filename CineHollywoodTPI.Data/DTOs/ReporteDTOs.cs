namespace CineHollywoodTPI.Data.DTOs
{
    public class PeliculaMasVendidaDTO
    {
        public int IdPelicula { get; set; }
        public string? Titulo { get; set; }
        public int CantidadEntradas { get; set; }
        public decimal TotalRecaudado { get; set; }
    }

    public class VentaMensualDTO
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public int CantidadVentas { get; set; }
        public int CantidadEntradas { get; set; }
        public decimal TotalRecaudado { get; set; }
    }
}
