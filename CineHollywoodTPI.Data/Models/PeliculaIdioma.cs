using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class PeliculaIdioma
{
    public int IdPelicula { get; set; }

    public int IdIdioma { get; set; }

    public bool? EsDoblada { get; set; }

    public virtual Idioma IdIdiomaNavigation { get; set; } = null!;

    public virtual Pelicula IdPeliculaNavigation { get; set; } = null!;
}
