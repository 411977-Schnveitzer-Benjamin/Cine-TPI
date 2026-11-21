using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class TipoSala
{
    public int IdTipoSala { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<PrecioTipoSala> PrecioTipoSalas { get; set; } = new List<PrecioTipoSala>();

    public virtual ICollection<Sala> Salas { get; set; } = new List<Sala>();
}
