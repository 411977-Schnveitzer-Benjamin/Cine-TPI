using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Director
{
    public int IdDirector { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public virtual ICollection<Pelicula> IdPeliculas { get; set; } = new List<Pelicula>();
}
