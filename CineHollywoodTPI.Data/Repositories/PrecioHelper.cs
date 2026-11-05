using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.Data.Repositories
{
    // Calcula el precio de una entrada para una función:
    // precio vigente del tipo de sala (tabla PRECIO_TIPO_SALA) menos el % de la promoción de la función.
    // La función tiene que venir con Include de Sala -> TipoSala -> PrecioTipoSalas y de Promocion.
    public static class PrecioHelper
    {
        public static decimal CalcularPrecioEntrada(Funcion funcion)
        {
            var precios = funcion.IdSalaNavigation?.IdTipoSalaNavigation?.PrecioTipoSalas
                          ?? new List<PrecioTipoSala>();

            var fecha = funcion.Fecha ?? DateOnly.FromDateTime(DateTime.Today);

            // Primero buscamos el precio vigente para la fecha de la función;
            // si no hay ninguno, usamos el más reciente cargado.
            var precioVigente = precios
                .Where(p => (p.FechaDesde == null || p.FechaDesde <= fecha) &&
                            (p.FechaHasta == null || p.FechaHasta >= fecha))
                .OrderByDescending(p => p.FechaDesde)
                .FirstOrDefault()
                ?? precios.OrderByDescending(p => p.FechaDesde).FirstOrDefault();

            var precioBase = precioVigente?.Precio ?? 0m;

            var descuento = funcion.IdPromocionNavigation?.PorcentajeDescuento ?? 0m;
            var precioFinal = precioBase * (1 - descuento / 100m);

            return Math.Round(precioFinal, 2);
        }
    }
}
