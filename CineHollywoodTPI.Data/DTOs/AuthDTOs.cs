namespace CineHollywoodTPI.Data.DTOs
{
    public class LoginDTO
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class RegistroDTO
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string NroDocumento { get; set; } = null!;
        public DateOnly? FechaNacimiento { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginRespuestaDTO
    {
        public string Token { get; set; } = null!;
        public DateTime Expira { get; set; }
        public int IdUsuario { get; set; }
        public string? Email { get; set; }
        public string? Nombre { get; set; }
        public string? Rol { get; set; }
        public int? IdCliente { get; set; }
    }
}
