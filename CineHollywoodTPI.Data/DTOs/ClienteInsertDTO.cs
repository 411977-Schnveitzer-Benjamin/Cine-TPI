namespace CineHollywoodTPI.Data.DTOs
{
    public class ClienteInsertDTO
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        public string? NroDocumento { get; set; }
        public bool? EsJubilado { get; set; }
        public string? Calle { get; set; }
        public int? Altura { get; set; }
    }
}
