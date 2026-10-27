using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Promocion
{
    public int IdPromocion { get; set; }

    public string? Descripcion { get; set; }

    public decimal? PorcentajeDescuento { get; set; }

    public virtual ICollection<Funcion> Funcions { get; set; } = new List<Funcion>();
}
