using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class TipoMembresium
{
    public int IdTipoMembresia { get; set; }

    public string? Nombre { get; set; }

    public decimal? PrecioMensual { get; set; }

    public int? EntradasGratisPorMes { get; set; }

    public virtual ICollection<ClienteMembresium> ClienteMembresia { get; set; } = new List<ClienteMembresium>();

    public virtual ICollection<Beneficio> IdBeneficios { get; set; } = new List<Beneficio>();
}
