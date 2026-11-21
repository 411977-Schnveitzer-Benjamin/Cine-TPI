using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class EmpleadoTurno
{
    public int IdEmpleado { get; set; }

    public int IdTurno { get; set; }

    public DateOnly Fecha { get; set; }

    public virtual Empleado IdEmpleadoNavigation { get; set; } = null!;

    public virtual Turno IdTurnoNavigation { get; set; } = null!;
}
