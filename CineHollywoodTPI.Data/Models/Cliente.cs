using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Cliente
{
    public int IdCliente { get; set; }

    public bool? EsJubilado { get; set; }

    public string? Calle { get; set; }

    public int? Altura { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<ClienteMembresium> ClienteMembresia { get; set; } = new List<ClienteMembresium>();

    public virtual Persona IdClienteNavigation { get; set; } = null!;

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
