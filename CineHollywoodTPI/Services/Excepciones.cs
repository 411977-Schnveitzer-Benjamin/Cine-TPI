namespace CineHollywoodTPI.API.Services
{
    // Excepciones que lanzan los servicios cuando no se cumple una regla.
    // El middleware ManejoErroresMiddleware las convierte en la respuesta HTTP correspondiente,
    // así los controllers no necesitan try/catch.

    /// <summary>400 Bad Request: los datos no cumplen una regla de negocio.</summary>
    public class NegocioException(string mensaje) : Exception(mensaje);

    /// <summary>404 Not Found: no existe lo que se pidió.</summary>
    public class NoEncontradoException(string mensaje) : Exception(mensaje);

    /// <summary>409 Conflict: choca con algo que ya existe (email repetido, butaca recién vendida...).</summary>
    public class ConflictoException(string mensaje) : Exception(mensaje);

    /// <summary>401 Unauthorized: credenciales inválidas.</summary>
    public class NoAutorizadoException(string mensaje) : Exception(mensaje);

    /// <summary>403 Forbidden: está logueado pero no puede acceder a ese recurso.</summary>
    public class AccesoDenegadoException(string mensaje) : Exception(mensaje);
}
