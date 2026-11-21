namespace CineHollywoodTPI.Data.DTOs
{
    public class PromocionDetalleDTO
    {
        public int IdPromocion { get; set; }
        public string? Descripcion { get; set; }
        public decimal? PorcentajeDescuento { get; set; }
        public int CantidadFunciones { get; set; }
        public int FuncionesProximas { get; set; }
    }

    public class PromocionGuardarDTO
    {
        public string Descripcion { get; set; } = null!;
        public decimal PorcentajeDescuento { get; set; }
    }
}
