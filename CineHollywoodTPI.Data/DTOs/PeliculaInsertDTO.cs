namespace CineHollywoodTPI.Data.DTOs
{
    public class PeliculaInsertDTO
    {
        public string Titulo { get; set; } = null!;
        public int? DuracionMinutos { get; set; }
        public int? IdClasificacion { get; set; }
    }
}
