using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineHollywoodTPI.API.Controllers
{
    // ABM de clientes: solo para el Administrador
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        private static ClienteDTO ToDto(Cliente c) => new()
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
        };

        private static string? Validar(string? nombre, string? apellido, string? dni, DateOnly? nacimiento, int? altura)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
                return "Nombre y apellido son obligatorios.";
            if (string.IsNullOrWhiteSpace(dni) || dni.Trim().Length < 6 || !dni.Trim().All(char.IsDigit))
                return "El DNI tiene que tener al menos 6 dígitos (solo números).";
            if (nacimiento.HasValue && nacimiento.Value > DateOnly.FromDateTime(DateTime.Today))
                return "La fecha de nacimiento no puede ser futura.";
            if (altura is < 0)
                return "La altura no puede ser negativa.";
            return null;
        }

        // GET: api/cliente
        [HttpGet]
        public async Task<IActionResult> GetActivos()
        {
            try
            {
                var clientes = await _clienteRepository.GetAllActiveAsync();
                return Ok(clientes.Select(ToDto).OrderBy(c => c.Apellido).ThenBy(c => c.Nombre));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        // GET: api/cliente/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            try
            {
                var cliente = await _clienteRepository.GetByIdAsync(id);
                if (cliente == null || !cliente.Activo)
                    return NotFound($"No se encontró ningún cliente activo con el ID {id}.");

                return Ok(ToDto(cliente));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al buscar por ID: {ex.Message}");
            }
        }

        // POST: api/cliente
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ClienteInsertDTO dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Los datos enviados para el cliente son inválidos.");

                var error = Validar(dto.Nombre, dto.Apellido, dto.NroDocumento, dto.FechaNacimiento, dto.Altura);
                if (error != null) return BadRequest(error);

                if (await _clienteRepository.ExisteDocumentoAsync(dto.NroDocumento!.Trim()))
                    return Conflict("Ya existe una persona registrada con ese DNI.");

                var nuevoCliente = new Cliente
                {
                    IdClienteNavigation = new Persona
                    {
                        Nombre = dto.Nombre!.Trim(),
                        Apellido = dto.Apellido!.Trim(),
                        FechaNacimiento = dto.FechaNacimiento,
                        NroDocumento = dto.NroDocumento.Trim()
                    },
                    EsJubilado = dto.EsJubilado ?? false,
                    Calle = dto.Calle?.Trim(),
                    Altura = dto.Altura,
                    Activo = true
                };

                var resultado = await _clienteRepository.AddAsync(nuevoCliente);
                if (!resultado)
                    return StatusCode(500, "No se pudo guardar el cliente en la base de datos.");

                return Ok(new { mensaje = "Cliente creado con éxito!", id = nuevoCliente.IdCliente });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar guardar: {ex.Message}");
            }
        }

        // PUT: api/cliente/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, [FromBody] ClienteUpdateDTO dto)
        {
            try
            {
                if (dto == null || id != dto.IdCliente)
                    return BadRequest("Los identificadores de la petición no coinciden.");

                var error = Validar(dto.Nombre, dto.Apellido, dto.NroDocumento, dto.FechaNacimiento, dto.Altura);
                if (error != null) return BadRequest(error);

                var clienteExistente = await _clienteRepository.GetByIdAsync(id);
                if (clienteExistente == null)
                    return NotFound($"No existe el cliente con ID {id}.");

                if (await _clienteRepository.ExisteDocumentoAsync(dto.NroDocumento!.Trim(), excluirIdPersona: id))
                    return Conflict("Ya existe otra persona registrada con ese DNI.");

                clienteExistente.EsJubilado = dto.EsJubilado ?? false;
                clienteExistente.Calle = dto.Calle?.Trim();
                clienteExistente.Altura = dto.Altura;
                clienteExistente.Activo = dto.Activo;

                clienteExistente.IdClienteNavigation.Nombre = dto.Nombre!.Trim();
                clienteExistente.IdClienteNavigation.Apellido = dto.Apellido!.Trim();
                clienteExistente.IdClienteNavigation.FechaNacimiento = dto.FechaNacimiento;
                clienteExistente.IdClienteNavigation.NroDocumento = dto.NroDocumento.Trim();

                await _clienteRepository.UpdateAsync(clienteExistente);
                return Ok(new { mensaje = "Cliente modificado con éxito!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al intentar actualizar: {ex.Message}");
            }
        }

        // DELETE: api/cliente/5  (baja lógica)
        [HttpDelete("{id:int}")]
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
