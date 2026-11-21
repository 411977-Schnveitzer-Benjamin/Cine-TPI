namespace CineHollywoodTPI.Data.DTOs
{
    public class PeliculaUpdateDTO
    {
        public int IdPelicula { get; set; }
        public string Titulo { get; set; } = null!;
        public int? DuracionMinutos { get; set; }
        public int? IdClasificacion { get; set; }
        public string? PosterUrl { get; set; }
    }

    public class PosterUrlDTO
    {
        public string Url { get; set; } = null!;
    }
}
