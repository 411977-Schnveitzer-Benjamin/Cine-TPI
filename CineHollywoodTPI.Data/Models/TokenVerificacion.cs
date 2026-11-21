using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class TokenVerificacion
{
    public int IdToken { get; set; }

    public int? IdUsuario { get; set; }

    public string? Token { get; set; }

    public DateTime? FechaExpiracion { get; set; }

    public bool? Usado { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
