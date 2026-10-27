using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class ClienteMembresium
{
    public int IdClienteMembresia { get; set; }

    public int? IdCliente { get; set; }

    public int? IdTipoMembresia { get; set; }

    public DateOnly? FechaInicio { get; set; }

    public DateOnly? FechaProximoCobro { get; set; }

    public bool? Estado { get; set; }

    public int? EntradasGratisUsadasMes { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual TipoMembresium? IdTipoMembresiaNavigation { get; set; }
}
