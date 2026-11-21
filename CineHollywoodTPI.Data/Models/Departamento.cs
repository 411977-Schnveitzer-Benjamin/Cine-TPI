using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Departamento
{
    public int IdDepartamento { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
