namespace CineHollywoodTPI.Data.DTOs
{
    public class PeliculaUpdateDTO
    {
        public int IdPelicula { get; set; } // Obligatorio para el WHERE en SQL
        public string Titulo { get; set; } = null!;
        public int? DuracionMinutos { get; set; }
        public int? IdClasificacion { get; set; }
        public string? PosterUrl { get; set; }
    }

    // Link de una imagen para que la API la descargue y la guarde como póster
    public class PosterUrlDTO
    {
        public string Url { get; set; } = null!;
    }
}
