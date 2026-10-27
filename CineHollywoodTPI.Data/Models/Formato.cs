using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Formato
{
    public int IdFormato { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Funcion> Funcions { get; set; } = new List<Funcion>();
}
