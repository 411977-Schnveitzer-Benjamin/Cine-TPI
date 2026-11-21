using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class RolUsuario
{
    public int IdRol { get; set; }

    public string? Nombre { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
