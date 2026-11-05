namespace CineHollywoodTPI.Data.DTOs
{
    // Sala con su tipo y la distribución de butacas (para el ABM de salas)
    public class SalaDetalleDTO
    {
        public int IdSala { get; set; }
        public string? Nombre { get; set; }
        public int? IdTipoSala { get; set; }
        public string? TipoSala { get; set; }
        public int? IdComplejo { get; set; }
        public string? Complejo { get; set; }
        public int CantidadButacas { get; set; }
        public int CantidadFilas { get; set; }
        public int ButacasPorFila { get; set; }
        public int CantidadFunciones { get; set; }
        public bool TieneEntradasVendidas { get; set; } // si es true no se puede cambiar la distribución
    }

    public class SalaGuardarDTO
    {
        public string Nombre { get; set; } = null!;
        public int IdTipoSala { get; set; }
        public int? IdComplejo { get; set; }
        public int Filas { get; set; }           // A, B, C... (máx. 26)
        public int ButacasPorFila { get; set; }  // 1..40
    }
}
