namespace CineHollywoodTPI.Data.DTOs
{
    public class PeliculaDTO
    {
        public int IdPelicula { get; set; }
        public string Titulo { get; set; } = null!;
        public int? DuracionMinutos { get; set; }
        public string? ClasificacionDescripcion { get; set; } // Para mostrar "Apta para todo público", "Mayores de 13", etc.
        public bool Activo { get; set; }
    }
}
