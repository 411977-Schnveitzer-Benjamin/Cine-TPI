namespace CineHollywoodTPI.Data.DTOs
{
    public class ClienteDTO
    {
        public int IdCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        public string? NroDocumento { get; set; }
        public bool? EsJubilado { get; set; }
        public string? Calle { get; set; }
        public int? Altura { get; set; }
        public bool Activo { get; set; }
    }
}
