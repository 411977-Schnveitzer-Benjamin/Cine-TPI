using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public int? IdPuesto { get; set; }

    public int? IdDepartamento { get; set; }

    public string? Legajo { get; set; }

    public DateOnly? FechaIngreso { get; set; }

    public virtual ICollection<EmpleadoTurno> EmpleadoTurnos { get; set; } = new List<EmpleadoTurno>();

    public virtual Departamento? IdDepartamentoNavigation { get; set; }

    public virtual Persona IdEmpleadoNavigation { get; set; } = null!;

    public virtual Puesto? IdPuestoNavigation { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
