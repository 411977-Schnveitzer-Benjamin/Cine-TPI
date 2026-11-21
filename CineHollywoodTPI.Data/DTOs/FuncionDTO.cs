namespace CineHollywoodTPI.Data.DTOs
{
    public class FuncionDTO
    {
        public int IdFuncion { get; set; }
        public int? IdPelicula { get; set; }
        public int? IdSala { get; set; }
        public int? IdFormato { get; set; }
        public int? IdPromocion { get; set; }
        public DateOnly? Fecha { get; set; }
        public TimeOnly? HoraInicio { get; set; }
        public bool Activo { get; set; }

        public string? PeliculaTitulo { get; set; }
        public int? DuracionMinutos { get; set; }
        public string? SalaNombre { get; set; }
        public string? TipoSala { get; set; }
        public string? FormatoDescripcion { get; set; }
        public string? PromocionDescripcion { get; set; }
        public decimal? PorcentajeDescuento { get; set; }

        public string? Clasificacion { get; set; }
        public bool EsParaMayores { get; set; }
        public bool TieneEntradasVendidas { get; set; }

        public decimal PrecioEntrada { get; set; }

        public decimal PrecioSinDescuento { get; set; }

        public List<PrecioPorTipoDTO> Precios { get; set; } = new();
    }

    public class PrecioPorTipoDTO
    {
        public int IdTipoEspectador { get; set; }
        public string? TipoEspectador { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioSinDescuento { get; set; }
        public string? Descuento { get; set; }
        public bool Permitido { get; set; } = true;
    }
}
