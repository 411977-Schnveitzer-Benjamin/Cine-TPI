using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class PrecioTipoSala
{
    public int IdPrecio { get; set; }

    public int? IdTipoSala { get; set; }

    public decimal? Precio { get; set; }

    public DateOnly? FechaDesde { get; set; }

    public DateOnly? FechaHasta { get; set; }

    public virtual TipoSala? IdTipoSalaNavigation { get; set; }
}
