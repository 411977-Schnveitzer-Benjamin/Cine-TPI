namespace CineHollywoodTPI.API.Services
{

    public class NegocioException(string mensaje) : Exception(mensaje);

    public class NoEncontradoException(string mensaje) : Exception(mensaje);

    public class ConflictoException(string mensaje) : Exception(mensaje);

    public class NoAutorizadoException(string mensaje) : Exception(mensaje);

    public class AccesoDenegadoException(string mensaje) : Exception(mensaje);
}
