using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Butaca
{
    public int IdButaca { get; set; }

    public int? IdSala { get; set; }

    public string? Fila { get; set; }

    public int? Numero { get; set; }

    public bool? Habilitada { get; set; }

    public virtual ICollection<Entrada> Entrada { get; set; } = new List<Entrada>();

    public virtual Sala? IdSalaNavigation { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
