using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Genero
{
    public int IdGenero { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Pelicula> IdPeliculas { get; set; } = new List<Pelicula>();
}
