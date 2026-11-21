using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public int? IdCliente { get; set; }

    public int? IdFuncion { get; set; }

    public int? IdButaca { get; set; }

    public int? IdEstadoReserva { get; set; }

    public DateTime? FechaHoraReserva { get; set; }

    public DateTime? FechaHoraVencimiento { get; set; }

    public virtual ICollection<Entrada> Entrada { get; set; } = new List<Entrada>();

    public virtual Butaca? IdButacaNavigation { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual EstadoReserva? IdEstadoReservaNavigation { get; set; }

    public virtual Funcion? IdFuncionNavigation { get; set; }
}
