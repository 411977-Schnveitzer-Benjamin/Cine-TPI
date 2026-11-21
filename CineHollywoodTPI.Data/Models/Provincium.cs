using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Provincium
{
    public int IdProvincia { get; set; }

    public int? IdPais { get; set; }

    public string? Nombre { get; set; }

    public virtual Pai? IdPaisNavigation { get; set; }

    public virtual ICollection<Localidad> Localidads { get; set; } = new List<Localidad>();
}
