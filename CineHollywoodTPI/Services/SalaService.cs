using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.API.Services
{
    // Reglas de salas:
    //  - Nombre obligatorio y único dentro del complejo; tipo de sala y complejo existentes.
    //  - Butacas: 1 a 26 filas (A..Z) y 1 a 40 por fila; se generan automáticamente.
    //  - Si la sala ya tiene entradas vendidas o reservas, no se puede cambiar la cantidad de butacas.
    //  - Si la sala tiene funciones cargadas, no se puede eliminar.
    public class SalaService : ISalaService
    {
        private const int MaxFilas = 26;
        private const int MaxButacasPorFila = 40;

        private readonly ISalaRepository _salaRepository;

        public SalaService(ISalaRepository salaRepository)
        {
            _salaRepository = salaRepository;
        }

        public async Task<IEnumerable<SalaDetalleDTO>> GetAllAsync() => await _salaRepository.GetAllAsync();

        public async Task<IEnumerable<ItemDTO>> GetComplejosAsync() => await _salaRepository.GetComplejosAsync();

        private async Task ValidarAsync(SalaGuardarDTO dto, int? idSalaActual)
        {
            if (dto == null) throw new NegocioException("Los datos de la sala son inválidos.");
            if (string.IsNullOrWhiteSpace(dto.Nombre) || dto.Nombre.Trim().Length > 100)
                throw new NegocioException("El nombre es obligatorio (máx. 100 caracteres).");
            if (!await _salaRepository.ExisteTipoSalaAsync(dto.IdTipoSala))
                throw new NegocioException("El tipo de sala no existe.");
            if (dto.IdComplejo != null && !await _salaRepository.ExisteComplejoAsync(dto.IdComplejo.Value))
                throw new NegocioException("El complejo no existe.");
            if (dto.Filas < 1 || dto.Filas > MaxFilas)
                throw new NegocioException($"La cantidad de filas tiene que estar entre 1 y {MaxFilas}.");
            if (dto.ButacasPorFila < 1 || dto.ButacasPorFila > MaxButacasPorFila)
                throw new NegocioException($"Las butacas por fila tienen que estar entre 1 y {MaxButacasPorFila}.");
            if (await _salaRepository.ExisteNombreAsync(dto.Nombre, dto.IdComplejo, idSalaActual))
                throw new ConflictoException("Ya existe una sala con ese nombre en el complejo.");
        }

        // Genera las butacas A1..A{n}, B1..B{n}, etc.
        private static List<Butaca> GenerarButacas(int filas, int porFila) =>
            Enumerable.Range(0, filas).SelectMany(f =>
                Enumerable.Range(1, porFila).Select(n => new Butaca
                {
                    Fila = ((char)('A' + f)).ToString(),
                    Numero = n,
                    Habilitada = true
                })).ToList();

        public async Task<int> CrearAsync(SalaGuardarDTO dto)
        {
            await ValidarAsync(dto, null);

            var sala = new Sala
            {
                Nombre = dto.Nombre.Trim(),
                IdTipoSala = dto.IdTipoSala,
                IdComplejo = dto.IdComplejo
            };
            foreach (var b in GenerarButacas(dto.Filas, dto.ButacasPorFila))
                sala.Butacas.Add(b);

            return await _salaRepository.CrearAsync(sala);
        }

        public async Task ModificarAsync(int id, SalaGuardarDTO dto)
        {
            var sala = await _salaRepository.GetByIdAsync(id)
                ?? throw new NoEncontradoException($"No existe la sala con ID {id}.");

            await ValidarAsync(dto, id);

            var filasActuales = sala.Butacas.Select(b => b.Fila).Distinct().Count();
            var porFilaActual = sala.Butacas.Max(b => (int?)b.Numero) ?? 0;
            var cambiaDistribucion = filasActuales != dto.Filas || porFilaActual != dto.ButacasPorFila;

            if (cambiaDistribucion && await _salaRepository.TieneButacasUsadasAsync(id))
                throw new NegocioException(
                    "La sala ya tiene entradas vendidas o reservas: no se puede cambiar la cantidad de butacas. Solo se pueden modificar nombre, tipo y complejo.");

            sala.Nombre = dto.Nombre.Trim();
            sala.IdTipoSala = dto.IdTipoSala;
            sala.IdComplejo = dto.IdComplejo;

            await _salaRepository.ActualizarAsync(sala, cambiaDistribucion ? GenerarButacas(dto.Filas, dto.ButacasPorFila) : null);
        }

        public async Task EliminarAsync(int id)
        {
            var sala = await _salaRepository.GetByIdAsync(id)
                ?? throw new NoEncontradoException($"No existe la sala con ID {id}.");

            if (await _salaRepository.TieneFuncionesAsync(id))
                throw new NegocioException("La sala tiene funciones cargadas (aunque estén canceladas o ya hayan pasado), por eso no se puede eliminar.");

            await _salaRepository.EliminarAsync(sala);
        }
    }
}
