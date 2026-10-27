using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class CanalVentum
{
    public int IdCanal { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
