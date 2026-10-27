using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class EstadoReserva
{
    public int IdEstadoReserva { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
