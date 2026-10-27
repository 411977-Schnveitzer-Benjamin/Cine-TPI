namespace CineHollywoodTPI.Data.DTOs
{
    public class PeliculaUpdateDTO
    {
        public int IdPelicula { get; set; } // Obligatorio para el WHERE en SQL
        public string Titulo { get; set; } = null!;
        public int? DuracionMinutos { get; set; }
        public int? IdClasificacion { get; set; }
    }
}
