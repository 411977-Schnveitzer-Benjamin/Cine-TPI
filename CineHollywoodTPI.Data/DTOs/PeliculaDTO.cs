namespace CineHollywoodTPI.Data.DTOs
{
    public class PeliculaDTO
    {
        public int IdPelicula { get; set; }
        public string Titulo { get; set; } = null!;
        public int? DuracionMinutos { get; set; }
        public int? IdClasificacion { get; set; }
        public string? ClasificacionDescripcion { get; set; } // Para mostrar "ATP", "+13", etc.
        public string? PosterUrl { get; set; }
        public bool Activo { get; set; }
    }
}
