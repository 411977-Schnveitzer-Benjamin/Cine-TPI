using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class ProductoConfiterium
{
    public int IdProducto { get; set; }

    public int? IdCategoriaProd { get; set; }

    public string? Nombre { get; set; }

    public decimal? PrecioActual { get; set; }

    public int? Stock { get; set; }

    public virtual ICollection<DetalleVentaConfiterium> DetalleVentaConfiteria { get; set; } = new List<DetalleVentaConfiterium>();

    public virtual CategoriaProducto? IdCategoriaProdNavigation { get; set; }
}
