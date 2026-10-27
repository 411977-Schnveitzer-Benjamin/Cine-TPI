using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class DetalleVentaConfiterium
{
    public int IdDetalleConfiteria { get; set; }

    public int? IdVenta { get; set; }

    public int? IdProducto { get; set; }

    public int? Cantidad { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public virtual ProductoConfiterium? IdProductoNavigation { get; set; }

    public virtual Venta? IdVentaNavigation { get; set; }
}
