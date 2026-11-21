using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class TipoContacto
{
    public int IdTipoContacto { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();
}
