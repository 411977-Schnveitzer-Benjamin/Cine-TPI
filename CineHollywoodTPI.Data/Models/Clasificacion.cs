using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Clasificacion
{
    public int IdClasificacion { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Pelicula> Peliculas { get; set; } = new List<Pelicula>();
}
