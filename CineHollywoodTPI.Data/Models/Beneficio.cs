using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Beneficio
{
    public int IdBeneficio { get; set; }

    public string? Descripcion { get; set; }

    public bool? AplicaAJubilado { get; set; }

    public virtual ICollection<TipoMembresium> IdTipoMembresia { get; set; } = new List<TipoMembresium>();
}
