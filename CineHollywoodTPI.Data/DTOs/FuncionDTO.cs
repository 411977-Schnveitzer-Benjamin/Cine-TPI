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

        public string? Clasificacion { get; set; }
        public bool EsParaMayores { get; set; }          // película +18
        public bool TieneEntradasVendidas { get; set; }  // si es true no se puede mover ni cancelar

        // Precio de la entrada "General" ya calculado (precio de la sala con la promoción aplicada)
        public decimal PrecioEntrada { get; set; }

        // Precio final según el tipo de espectador (General, Jubilado, Menor...), lo calcula el servidor
        public List<PrecioPorTipoDTO> Precios { get; set; } = new();
    }

    public class PrecioPorTipoDTO
    {
        public int IdTipoEspectador { get; set; }
        public string? TipoEspectador { get; set; }
        public decimal Precio { get; set; }
        public string? Descuento { get; set; } // texto: "Promo 2x1 (-50%)", "Jubilado / Menor (-15%)" o null
        public bool Permitido { get; set; } = true; // false: ej. "Menor" en una película +18
    }
}
