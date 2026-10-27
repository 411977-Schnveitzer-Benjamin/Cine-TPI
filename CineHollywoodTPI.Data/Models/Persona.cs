using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Persona
{
    public int IdPersona { get; set; }

    public string? Apellido { get; set; }

    public string? Nombre { get; set; }

    public DateOnly? FechaNacimiento { get; set; }

    public string? NroDocumento { get; set; }

    public virtual Cliente? Cliente { get; set; }

    public virtual ICollection<Contacto> Contactos { get; set; } = new List<Contacto>();

    public virtual Empleado? Empleado { get; set; }

    public virtual Usuario? Usuario { get; set; }
}
