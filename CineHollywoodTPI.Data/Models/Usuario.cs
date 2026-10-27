using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int? IdPersona { get; set; }

    public int? IdRol { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public int? IdEstadoUsuario { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual EstadoUsuario? IdEstadoUsuarioNavigation { get; set; }

    public virtual Persona? IdPersonaNavigation { get; set; }

    public virtual RolUsuario? IdRolNavigation { get; set; }

    public virtual ICollection<TokenVerificacion> TokenVerificacions { get; set; } = new List<TokenVerificacion>();
}
