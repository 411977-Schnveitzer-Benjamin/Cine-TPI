using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Complejo
{
    public int IdComplejo { get; set; }

    public int? IdSucursal { get; set; }

    public string? Nombre { get; set; }

    public virtual Sucursal? IdSucursalNavigation { get; set; }

    public virtual ICollection<Sala> Salas { get; set; } = new List<Sala>();
}
