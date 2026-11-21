using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Entrada
{
    public int IdEntrada { get; set; }

    public int? IdVenta { get; set; }

    public int? IdReserva { get; set; }

    public int? IdFuncion { get; set; }

    public int? IdButaca { get; set; }

    public int? IdTipoEspectador { get; set; }

    public decimal? PrecioFinal { get; set; }

    public virtual Butaca? IdButacaNavigation { get; set; }

    public virtual Funcion? IdFuncionNavigation { get; set; }

    public virtual Reserva? IdReservaNavigation { get; set; }

    public virtual TipoEspectador? IdTipoEspectadorNavigation { get; set; }

    public virtual Venta? IdVentaNavigation { get; set; }
}
