using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using CineHollywoodTPI.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionController : ControllerBase
    {
        private readonly IFuncionRepository _funcionRepository;
        private readonly IPeliculaRepository _peliculaRepository;

        public FuncionController(IFuncionRepository funcionRepository, IPeliculaRepository peliculaRepository)
        {
            _funcionRepository = funcionRepository;
            _peliculaRepository = peliculaRepository;
        }

        private static FuncionDTO ToDto(Funcion f) => new()
        {
            IdFuncion = f.IdFuncion,
            IdPelicula = f.IdPelicula,
            IdSala = f.IdSala,
            IdFormato = f.IdFormato,
            IdPromocion = f.IdPromocion,
            Fecha = f.Fecha,
            HoraInicio = f.HoraInicio,
            Activo = f.Activo,
            PeliculaTitulo = f.IdPeliculaNavigation?.Titulo,
            DuracionMinutos = f.IdPeliculaNavigation?.DuracionMinutos,
            SalaNombre = f.IdSalaNavigation?.Nombre,
            TipoSala = f.IdSalaNavigation?.IdTipoSalaNavigation?.Descripcion,
            FormatoDescripcion = f.IdFormatoNavigation?.Descripcion,
            PromocionDescripcion = f.IdPromocionNavigation?.Descripcion,
            PorcentajeDescuento = f.IdPromocionNavigation?.PorcentajeDescuento,
            PrecioEntrada = PrecioHelper.CalcularPrecioEntrada(f)
        };

        // Validaciones comunes de alta y modificación
        private async Task<string?> ValidarAsync(Funcion f)
        {
            if (f.IdPelicula == null || f.IdSala == null || f.IdFormato == null || f.Fecha == null || f.HoraInicio == null)
                return "Película, sala, formato, fecha y hora son obligatorios.";

            var pelicula = await _peliculaRepository.GetByIdAsync(f.IdPelicula.Value);
            if (pelicula == null || !pelicula.Activo)
                return "La película seleccionada no existe o está dada de baja.";

            if (await _funcionRepository.ExisteSuperposicionAsync(f))
                return "Ya hay otra función en esa sala que se superpone con ese horario.";

            return null;
        }

        // GET: api/funcion?idPelicula=3&soloProximas=true
        [HttpGet]
        public async Task<IActionResult> GetActivas([FromQuery] int? idPelicula = null, [FromQuery] bool soloProximas = false)
        {
            try
            {
                DateOnly? desde = soloProximas ? DateOnly.FromDateTime(DateTime.Today) : null;
                var funciones = await _funcionRepository.GetAllActiveAsync(idPelicula, desde);
                return Ok(funciones.Select(ToDto));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/funcion/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            try
            {
                var funcion = await _funcionRepository.GetByIdAsync(id);
                if (funcion == null || !funcion.Activo)
                    return NotFound($"No se encontró ninguna función activa con el ID {id}.");

                return Ok(ToDto(funcion));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por ID: {ex.Message}");
            }
        }

        // GET: api/funcion/5/butacas  -> mapa de butacas con su estado
        [HttpGet("{id:int}/butacas")]
        public async Task<IActionResult> GetButacas(int id)
        {
            try
            {
                return Ok(await _funcionRepository.GetButacasAsync(id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las butacas: {ex.Message}");
            }
        }

        // POST: api/funcion
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] FuncionInsertDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Los datos enviados para la función son inválidos.");

                var nuevaFuncion = new Funcion
                {
                    IdPelicula = dto.IdPelicula,
                    IdSala = dto.IdSala,
                    IdFormato = dto.IdFormato,
                    IdPromocion = dto.IdPromocion,
                    Fecha = dto.Fecha,
                    HoraInicio = dto.HoraInicio,
                    Activo = true
                };

                if (nuevaFuncion.Fecha < DateOnly.FromDateTime(DateTime.Today))
                    return BadRequest($"La fecha {nuevaFuncion.Fecha:dd/MM/yyyy} ya pasó. No se puede programar una función en el pasado.");

                var error = await ValidarAsync(nuevaFuncion);
                if (error != null) return BadRequest(error);

                var resultado = await _funcionRepository.AddAsync(nuevaFuncion);
                if (!resultado)
                    return StatusCode(500, "No se pudo guardar la función en la base de datos.");

                return Ok(new { mensaje = "Función creada con éxito!", id = nuevaFuncion.IdFuncion });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar guardar: {ex.Message}");
            }
        }

        // PUT: api/funcion/5
        [Authorize(Roles = "Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] FuncionUpdateDTO dto)
        {
            try
            {
                if (dto == null || id != dto.IdFuncion)
                    return BadRequest("Los identificadores de la petición no coinciden.");

                var funcionExistente = await _funcionRepository.GetByIdAsync(id);
                if (funcionExistente == null)
                    return NotFound($"No existe la función con ID {id}.");

                funcionExistente.IdPelicula = dto.IdPelicula;
                funcionExistente.IdSala = dto.IdSala;
                funcionExistente.IdFormato = dto.IdFormato;
                funcionExistente.IdPromocion = dto.IdPromocion;
                funcionExistente.Fecha = dto.Fecha;
                funcionExistente.HoraInicio = dto.HoraInicio;
                funcionExistente.Activo = dto.Activo;

                var error = await ValidarAsync(funcionExistente);
                if (error != null) return BadRequest(error);

                await _funcionRepository.UpdateAsync(funcionExistente);
                return Ok(new { mensaje = "Función modificada con éxito!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar actualizar: {ex.Message}");
            }
        }

        // DELETE: api/funcion/5  (baja lógica)
        [Authorize(Roles = "Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            try
            {
                var resultado = await _funcionRepository.DeleteLogicoAsync(id);
                if (!resultado)
                    return NotFound($"No se pudo procesar la baja. No existe la función con ID {id} o ya se encuentra inactiva.");

                return Ok(new { mensaje = "Función dada de baja de forma lógica correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al procesar la baja: {ex.Message}");
            }
        }
    }
}
