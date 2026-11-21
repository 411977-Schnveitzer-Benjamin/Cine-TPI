using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Idioma
{
    public int IdIdioma { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<PeliculaIdioma> PeliculaIdiomas { get; set; } = new List<PeliculaIdioma>();
}
