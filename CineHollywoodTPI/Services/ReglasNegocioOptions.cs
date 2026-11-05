namespace CineHollywoodTPI.API.Services
{
    // Parámetros de las reglas de negocio. Se leen de appsettings.json (sección "ReglasNegocio"),
    // así se pueden cambiar sin tocar el código.
    public class ReglasNegocioOptions
    {
        public const string Seccion = "ReglasNegocio";

        // Descuento para entradas de Jubilado o Menor (solo si la función NO tiene promoción)
        public decimal PorcentajeDescuentoJubiladoMenor { get; set; } = 15m;
        public int[] IdsTipoEspectadorConDescuento { get; set; } = [2, 3]; // Jubilado, Menor
        public int IdTipoEspectadorMenor { get; set; } = 3;

        // Películas para mayores: clasificación que contenga este texto (ej. "+18")
        public string TextoClasificacionMayores { get; set; } = "18";
        public int EdadMayoria { get; set; } = 18;

        public int MaxEntradasPorCompra { get; set; } = 10;

        public int IdCanalBoleteria { get; set; } = 1;
        public int IdCanalWeb { get; set; } = 2;
    }
}
