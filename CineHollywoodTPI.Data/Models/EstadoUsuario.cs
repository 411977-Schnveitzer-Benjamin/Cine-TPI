using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class EstadoUsuario
{
    public int IdEstadoUsuario { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
