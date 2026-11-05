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

        // Datos descriptivos para mostrar en pantalla (sin tener que hacer otra consulta)
        public string? PeliculaTitulo { get; set; }
        public int? DuracionMinutos { get; set; }
        public string? SalaNombre { get; set; }
        public string? TipoSala { get; set; }
        public string? FormatoDescripcion { get; set; }
        public string? PromocionDescripcion { get; set; }
        public decimal? PorcentajeDescuento { get; set; }

        // Precio de la entrada ya calculado (precio de la sala con la promoción aplicada)
        public decimal PrecioEntrada { get; set; }
    }
}
