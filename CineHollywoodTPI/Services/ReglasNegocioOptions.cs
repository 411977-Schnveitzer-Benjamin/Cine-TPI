namespace CineHollywoodTPI.API.Services
{
    public class ReglasNegocioOptions
    {
        public const string Seccion = "ReglasNegocio";

        public decimal PorcentajeDescuentoJubiladoMenor { get; set; } = 15m;
        public int[] IdsTipoEspectadorConDescuento { get; set; } = [2, 3];
        public int IdTipoEspectadorMenor { get; set; } = 3;

        public string TextoClasificacionMayores { get; set; } = "18";
        public int EdadMayoria { get; set; } = 18;

        public int MaxEntradasPorCompra { get; set; } = 10;

        public int IdCanalBoleteria { get; set; } = 1;
        public int IdCanalWeb { get; set; } = 2;
    }
}
