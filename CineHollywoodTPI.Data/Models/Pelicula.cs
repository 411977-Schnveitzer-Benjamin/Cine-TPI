using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Pelicula
{
    public int IdPelicula { get; set; }

    public int? IdClasificacion { get; set; }

    public string? Titulo { get; set; }

    public int? DuracionMinutos { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Funcion> Funcions { get; set; } = new List<Funcion>();

    public virtual Clasificacion? IdClasificacionNavigation { get; set; }

    public virtual ICollection<PeliculaIdioma> PeliculaIdiomas { get; set; } = new List<PeliculaIdioma>();

    public virtual ICollection<Actor> IdActors { get; set; } = new List<Actor>();

    public virtual ICollection<Director> IdDirectors { get; set; } = new List<Director>();

    public virtual ICollection<Genero> IdGeneros { get; set; } = new List<Genero>();
}
