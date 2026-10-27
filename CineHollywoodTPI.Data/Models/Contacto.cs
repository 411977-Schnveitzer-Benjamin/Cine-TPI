using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Contacto
{
    public int IdContacto { get; set; }

    public int? IdPersona { get; set; }

    public int? IdTipoContacto { get; set; }

    public string? Valor { get; set; }

    public virtual Persona? IdPersonaNavigation { get; set; }

    public virtual TipoContacto? IdTipoContactoNavigation { get; set; }
}
