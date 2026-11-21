using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class TipoEspectador
{
    public int IdTipoEspectador { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Entrada> Entrada { get; set; } = new List<Entrada>();
}
