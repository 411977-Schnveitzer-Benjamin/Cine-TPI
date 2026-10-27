using System;
using System.Collections.Generic;

namespace CineHollywoodTPI.Data.Models;

public partial class Funcion
{
    public int IdFuncion { get; set; }

    public int? IdPelicula { get; set; }

    public int? IdSala { get; set; }

    public int? IdFormato { get; set; }

    public int? IdPromocion { get; set; }

    public DateOnly? Fecha { get; set; }

    public TimeOnly? HoraInicio { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<Entrada> Entrada { get; set; } = new List<Entrada>();

    public virtual Formato? IdFormatoNavigation { get; set; }

    public virtual Pelicula? IdPeliculaNavigation { get; set; }

    public virtual Promocion? IdPromocionNavigation { get; set; }

    public virtual Sala? IdSalaNavigation { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
