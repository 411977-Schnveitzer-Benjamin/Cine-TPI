using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class VentaxFormaPago
{
    public int IdVentaxformapago { get; set; }

    public int? IdVenta { get; set; }

    public int? IdFormaPago { get; set; }

    public decimal? Monto { get; set; }

    public virtual FormaPago? IdFormaPagoNavigation { get; set; }

    public virtual Venta? IdVentaNavigation { get; set; }
}
