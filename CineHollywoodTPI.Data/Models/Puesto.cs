using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Puesto
{
    public int IdPuesto { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
