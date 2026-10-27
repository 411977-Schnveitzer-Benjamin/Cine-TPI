using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class CategoriaProducto
{
    public int IdCategoriaProd { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<ProductoConfiterium> ProductoConfiteria { get; set; } = new List<ProductoConfiterium>();
}
