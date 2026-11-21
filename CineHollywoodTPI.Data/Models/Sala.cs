using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Sala
{
    public int IdSala { get; set; }

    public int? IdComplejo { get; set; }

    public int? IdTipoSala { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Butaca> Butacas { get; set; } = new List<Butaca>();

    public virtual ICollection<Funcion> Funcions { get; set; } = new List<Funcion>();

    public virtual Complejo? IdComplejoNavigation { get; set; }

    public virtual TipoSala? IdTipoSalaNavigation { get; set; }
}
