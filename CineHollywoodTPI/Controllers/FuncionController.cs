using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionController : ControllerBase
    {
        private readonly IFuncionRepository _funcionRepository;

        public FuncionController(IFuncionRepository funcionRepository)
        {
            _funcionRepository = funcionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivas()
        {
            try
            {
                var funciones = await _funcionRepository.GetAllActiveAsync();

                var listaDto = funciones.Select(f => new FuncionDTO
                {
                    IdFuncion = f.IdFuncion,
                    IdPelicula = f.IdPelicula,
                    IdSala = f.IdSala,
                    IdFormato = f.IdFormato,
                    IdPromocion = f.IdPromocion,
                    Fecha = f.Fecha,
                    HoraInicio = f.HoraInicio,
                    Activo = f.Activo
                });

                return Ok(listaDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            try
            {
                var funcion = await _funcionRepository.GetByIdAsync(id);
                if (funcion == null || !funcion.Activo)
                    return NotFound($"No se encontró ninguna función activa con el ID {id}.");

                var dto = new FuncionDTO
                {
                    IdFuncion = funcion.IdFuncion,
                    IdPelicula = funcion.IdPelicula,
                    IdSala = funcion.IdSala,
                    IdFormato = funcion.IdFormato,
                    IdPromocion = funcion.IdPromocion,
                    Fecha = funcion.Fecha,
                    HoraInicio = funcion.HoraInicio,
                    Activo = funcion.Activo
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por ID: {ex.Message}");
            }
        }

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

                var resultado = await _funcionRepository.AddAsync(nuevaFuncion);
                if (!resultado)
                    return StatusCode(500, "No se pudo guardar la función en la base de datos.");

                return Ok(new { mensaje = "Función creada con éxito!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar guardar: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
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

                var resultado = await _funcionRepository.UpdateAsync(funcionExistente);
                if (!resultado)
                    return StatusCode(500, "No se pudieron persistir los cambios de la función.");

                return Ok(new { mensaje = "Función modificada con éxito!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar actualizar: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
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
