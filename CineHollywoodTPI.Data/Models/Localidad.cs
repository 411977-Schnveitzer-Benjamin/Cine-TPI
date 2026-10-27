using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Localidad
{
    public int IdLocalidad { get; set; }

    public int? IdProvincia { get; set; }

    public string? Nombre { get; set; }

    public virtual Provincium? IdProvinciaNavigation { get; set; }

    public virtual ICollection<Sucursal> Sucursals { get; set; } = new List<Sucursal>();
}
