using CineHollywoodTPI.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // Listas de solo lectura para los combos del frontend. Son públicas.
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoService _catalogo;

        public CatalogoController(ICatalogoService catalogo)
        {
            _catalogo = catalogo;
        }

        [HttpGet("clasificaciones")]
        public async Task<IActionResult> Clasificaciones() => Ok(await _catalogo.GetClasificacionesAsync());

        [HttpGet("formatos")]
        public async Task<IActionResult> Formatos() => Ok(await _catalogo.GetFormatosAsync());

        [HttpGet("tipos-espectador")]
        public async Task<IActionResult> TiposEspectador() => Ok(await _catalogo.GetTiposEspectadorAsync());

        [HttpGet("formas-pago")]
        public async Task<IActionResult> FormasPago() => Ok(await _catalogo.GetFormasPagoAsync());

        [HttpGet("canales")]
        public async Task<IActionResult> Canales() => Ok(await _catalogo.GetCanalesAsync());

        [HttpGet("salas")]
        public async Task<IActionResult> Salas() => Ok(await _catalogo.GetSalasAsync());

        [HttpGet("tipos-sala")]
        public async Task<IActionResult> TiposSala() => Ok(await _catalogo.GetTiposSalaAsync());

        [HttpGet("promociones")]
        public async Task<IActionResult> Promociones() => Ok(await _catalogo.GetPromocionesAsync());
    }
}
