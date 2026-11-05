namespace CineHollywoodTPI.Data.DTOs
{
    // Item genérico para llenar <select> (clasificaciones, formatos, formas de pago, etc.)
    public class ItemDTO
    {
        public int Id { get; set; }
        public string? Descripcion { get; set; }
    }

    public class SalaDTO
    {
        public int IdSala { get; set; }
        public string? Nombre { get; set; }
        public string? TipoSala { get; set; }
        public int CantidadButacas { get; set; }
    }

    public class PromocionDTO
    {
        public int IdPromocion { get; set; }
        public string? Descripcion { get; set; }
        public decimal? PorcentajeDescuento { get; set; }
    }

    // Estado de una butaca para una función puntual (libre / ocupada)
    public class ButacaEstadoDTO
    {
        public int IdButaca { get; set; }
        public string? Fila { get; set; }
        public int? Numero { get; set; }
        public bool Habilitada { get; set; }
        public bool Ocupada { get; set; }
    }
}
