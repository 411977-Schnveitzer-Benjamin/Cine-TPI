using CineHollywoodTPI.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // Endpoints de solo lectura para llenar los combos (select) del frontend. Son públicos.
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogoController : ControllerBase
    {
        private readonly ICatalogoRepository _catalogo;

        public CatalogoController(ICatalogoRepository catalogo)
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
