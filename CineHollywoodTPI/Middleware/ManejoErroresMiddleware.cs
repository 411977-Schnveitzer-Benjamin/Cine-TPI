using CineHollywoodTPI.API.Services;

namespace CineHollywoodTPI.API.Middleware
{
    // Atrapa las excepciones que salen de los servicios y arma la respuesta HTTP.
    // Gracias a esto los controllers no tienen try/catch: si una regla no se cumple,
    // el servicio lanza (por ej.) NegocioException("mensaje") y acá se devuelve 400 con ese mensaje.
    public class ManejoErroresMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejoErroresMiddleware> _logger;

        public ManejoErroresMiddleware(RequestDelegate next, ILogger<ManejoErroresMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var (status, mensaje) = ex switch
                {
                    NegocioException => (StatusCodes.Status400BadRequest, ex.Message),
                    NoEncontradoException => (StatusCodes.Status404NotFound, ex.Message),
                    ConflictoException => (StatusCodes.Status409Conflict, ex.Message),
                    NoAutorizadoException => (StatusCodes.Status401Unauthorized, ex.Message),
                    AccesoDenegadoException => (StatusCodes.Status403Forbidden, ex.Message),
                    _ => (StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}")
                };

                if (status == StatusCodes.Status500InternalServerError)
                    _logger.LogError(ex, "Error no controlado en {Ruta}", context.Request.Path);

                if (context.Response.HasStarted) throw;

                context.Response.StatusCode = status;
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync(mensaje);
            }
        }
    }
}
