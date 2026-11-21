namespace CineHollywoodTPI.Data.DTOs
{
    public class FuncionInsertDTO
    {
        public int? IdPelicula { get; set; }
        public int? IdSala { get; set; }
        public int? IdFormato { get; set; }
        public int? IdPromocion { get; set; }
        public DateOnly? Fecha { get; set; }
        public TimeOnly? HoraInicio { get; set; }
    }
}
