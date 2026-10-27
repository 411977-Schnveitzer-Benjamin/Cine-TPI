using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivos()
        {
            try
            {
                var clientes = await _clienteRepository.GetAllActiveAsync();

                var listaDto = clientes.Select(c => new ClienteDTO
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.IdClienteNavigation?.Nombre,
                    Apellido = c.IdClienteNavigation?.Apellido,
                    FechaNacimiento = c.IdClienteNavigation?.FechaNacimiento,
                    NroDocumento = c.IdClienteNavigation?.NroDocumento,
                    EsJubilado = c.EsJubilado,
                    Calle = c.Calle,
                    Altura = c.Altura,
                    Activo = c.Activo
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
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente == null || !cliente.Activo)
                    return NotFound($"No se encontró ningún cliente activo con el ID {id}.");

                var dto = new ClienteDTO
                {
                    IdCliente = cliente.IdCliente,
                    Nombre = cliente.IdClienteNavigation?.Nombre,
                    Apellido = cliente.IdClienteNavigation?.Apellido,
                    FechaNacimiento = cliente.IdClienteNavigation?.FechaNacimiento,
                    NroDocumento = cliente.IdClienteNavigation?.NroDocumento,
                    EsJubilado = cliente.EsJubilado,
                    Calle = cliente.Calle,
                    Altura = cliente.Altura,
                    Activo = cliente.Activo
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por ID: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ClienteInsertDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Los datos enviados para el cliente son inválidos.");

                var nuevoCliente = new Cliente
                {
                    IdClienteNavigation = new Persona
                    {
                        Nombre = dto.Nombre,
                        Apellido = dto.Apellido,
                        FechaNacimiento = dto.FechaNacimiento,
                        NroDocumento = dto.NroDocumento
                    },
                    EsJubilado = dto.EsJubilado,
                    Calle = dto.Calle,
                    Altura = dto.Altura,
                    Activo = true
                };

                var resultado = await _clienteRepository.AddAsync(nuevoCliente);
                if (!resultado)
                    return StatusCode(500, "No se pudo guardar el cliente en la base de datos.");

                return Ok(new { mensaje = "Cliente creado con éxito!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar guardar: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] ClienteUpdateDTO dto)
        {
            try
            {
                if (dto == null || id != dto.IdCliente)
                    return BadRequest("Los identificadores de la petición no coinciden.");

                var clienteExistente = await _clienteRepository.GetByIdAsync(id);
                if (clienteExistente == null)
                    return NotFound($"No existe el cliente con ID {id}.");

                clienteExistente.EsJubilado = dto.EsJubilado;
                clienteExistente.Calle = dto.Calle;
                clienteExistente.Altura = dto.Altura;
                clienteExistente.Activo = dto.Activo;

                if (clienteExistente.IdClienteNavigation != null)
                {
                    clienteExistente.IdClienteNavigation.Nombre = dto.Nombre;
                    clienteExistente.IdClienteNavigation.Apellido = dto.Apellido;
                    clienteExistente.IdClienteNavigation.FechaNacimiento = dto.FechaNacimiento;
                    clienteExistente.IdClienteNavigation.NroDocumento = dto.NroDocumento;
                }

                var resultado = await _clienteRepository.UpdateAsync(clienteExistente);
                if (!resultado)
                    return StatusCode(500, "No se pudieron persistir los cambios del cliente.");

                return Ok(new { mensaje = "Cliente modificado con éxito!" });
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
                var resultado = await _clienteRepository.DeleteLogicoAsync(id);
                if (!resultado)
                    return NotFound($"No se pudo procesar la baja. No existe el cliente con ID {id} o ya se encuentra inactivo.");

                return Ok(new { mensaje = "Cliente dado de baja de forma lógica correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al procesar la baja: {ex.Message}");
            }
        }
    }
}
