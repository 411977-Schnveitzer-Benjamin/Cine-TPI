using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class FormaPago
{
    public int IdFormaPago { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<VentaxFormaPago> VentaxFormaPagos { get; set; } = new List<VentaxFormaPago>();
}
