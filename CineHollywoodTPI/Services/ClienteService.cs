using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.API.Services
{
    // Reglas de clientes:
    //  - Nombre y apellido obligatorios; DNI solo números (mínimo 6) y no repetido.
    //  - La fecha de nacimiento no puede ser futura; la altura no puede ser negativa.
    //  - Baja lógica (activo = 0).
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
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

        private static void Validar(string? nombre, string? apellido, string? dni, DateOnly? nacimiento, int? altura)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(apellido))
                throw new NegocioException("Nombre y apellido son obligatorios.");
            if (string.IsNullOrWhiteSpace(dni) || dni.Trim().Length < 6 || !dni.Trim().All(char.IsDigit))
                throw new NegocioException("El DNI tiene que tener al menos 6 dígitos (solo números).");
            if (nacimiento.HasValue && nacimiento.Value > DateOnly.FromDateTime(DateTime.Today))
                throw new NegocioException("La fecha de nacimiento no puede ser futura.");
            if (altura is < 0)
                throw new NegocioException("La altura no puede ser negativa.");
        }

        public async Task<IEnumerable<ClienteDTO>> GetActivosAsync() =>
            (await _clienteRepository.GetAllActiveAsync())
                .Select(ToDto)
                .OrderBy(c => c.Apellido).ThenBy(c => c.Nombre)
                .ToList();

        public async Task<ClienteDTO> GetPorIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null || !cliente.Activo)
                throw new NoEncontradoException($"No se encontró ningún cliente activo con el ID {id}.");
            return ToDto(cliente);
        }

        public async Task<int> CrearAsync(ClienteInsertDTO dto)
        {
            if (dto == null) throw new NegocioException("Los datos enviados para el cliente son inválidos.");
            Validar(dto.Nombre, dto.Apellido, dto.NroDocumento, dto.FechaNacimiento, dto.Altura);

            if (await _clienteRepository.ExisteDocumentoAsync(dto.NroDocumento!.Trim()))
                throw new ConflictoException("Ya existe una persona registrada con ese DNI.");

            var nuevo = new Cliente
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

            if (!await _clienteRepository.AddAsync(nuevo))
                throw new Exception("No se pudo guardar el cliente en la base de datos.");

            return nuevo.IdCliente;
        }

        public async Task ModificarAsync(int id, ClienteUpdateDTO dto)
        {
            if (dto == null || id != dto.IdCliente)
                throw new NegocioException("Los identificadores de la petición no coinciden.");
            Validar(dto.Nombre, dto.Apellido, dto.NroDocumento, dto.FechaNacimiento, dto.Altura);

            var cliente = await _clienteRepository.GetByIdAsync(id)
                ?? throw new NoEncontradoException($"No existe el cliente con ID {id}.");

            if (await _clienteRepository.ExisteDocumentoAsync(dto.NroDocumento!.Trim(), excluirIdPersona: id))
                throw new ConflictoException("Ya existe otra persona registrada con ese DNI.");

            cliente.EsJubilado = dto.EsJubilado ?? false;
            cliente.Calle = dto.Calle?.Trim();
            cliente.Altura = dto.Altura;
            cliente.Activo = dto.Activo;
            cliente.IdClienteNavigation.Nombre = dto.Nombre!.Trim();
            cliente.IdClienteNavigation.Apellido = dto.Apellido!.Trim();
            cliente.IdClienteNavigation.FechaNacimiento = dto.FechaNacimiento;
            cliente.IdClienteNavigation.NroDocumento = dto.NroDocumento.Trim();

            await _clienteRepository.UpdateAsync(cliente);
        }

        public async Task DarDeBajaAsync(int id)
        {
            if (!await _clienteRepository.DeleteLogicoAsync(id))
                throw new NoEncontradoException($"No existe el cliente con ID {id} o ya está dado de baja.");
        }
    }
}
