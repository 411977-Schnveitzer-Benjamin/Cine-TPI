using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Turno
{
    public int IdTurno { get; set; }

    public string? Descripcion { get; set; }

    public TimeOnly? HoraDesde { get; set; }

    public TimeOnly? HoraHasta { get; set; }

    public virtual ICollection<EmpleadoTurno> EmpleadoTurnos { get; set; } = new List<EmpleadoTurno>();
}
