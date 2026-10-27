using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Sucursal
{
    public int IdSucursal { get; set; }

    public int? IdLocalidad { get; set; }

    public string? Calle { get; set; }

    public int? Numero { get; set; }

    public virtual ICollection<Complejo> Complejos { get; set; } = new List<Complejo>();

    public virtual Localidad? IdLocalidadNavigation { get; set; }
}
