using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Venta
{
    public int IdVenta { get; set; }

    public int? IdCliente { get; set; }

    public int? IdEmpleado { get; set; }

    public int? IdCanal { get; set; }

    public DateTime? FechaHora { get; set; }

    public virtual ICollection<DetalleVentaConfiterium> DetalleVentaConfiteria { get; set; } = new List<DetalleVentaConfiterium>();

    public virtual ICollection<Entrada> Entrada { get; set; } = new List<Entrada>();

    public virtual CanalVentum? IdCanalNavigation { get; set; }

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Empleado? IdEmpleadoNavigation { get; set; }

    public virtual ICollection<VentaxFormaPago> VentaxFormaPagos { get; set; } = new List<VentaxFormaPago>();
}
