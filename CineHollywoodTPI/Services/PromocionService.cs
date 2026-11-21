using CineHollywoodTPI.API.Services.Interfaces;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.API.Services
{
    public class PromocionService : IPromocionService
    {
        private const int MaxDescripcion = 100;

        private readonly IPromocionRepository _promocionRepository;

        public PromocionService(IPromocionRepository promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }

        private static DateOnly Hoy => DateOnly.FromDateTime(DateTime.Today);

        public async Task<IEnumerable<PromocionDetalleDTO>> GetAllAsync() => await _promocionRepository.GetAllAsync(Hoy);

        private async Task ValidarAsync(PromocionGuardarDTO dto, int? idPromocionActual)
        {
            if (dto == null) throw new NegocioException("Los datos de la promoción son inválidos.");
            if (string.IsNullOrWhiteSpace(dto.Descripcion) || dto.Descripcion.Trim().Length > MaxDescripcion)
                throw new NegocioException($"La descripción es obligatoria (máx. {MaxDescripcion} caracteres).");
            if (dto.PorcentajeDescuento <= 0 || dto.PorcentajeDescuento > 100)
                throw new NegocioException("El descuento tiene que ser mayor a 0% y como máximo 100%.");
            if (decimal.Round(dto.PorcentajeDescuento, 2) != dto.PorcentajeDescuento)
                throw new NegocioException("El descuento admite hasta 2 decimales.");
            if (await _promocionRepository.ExisteDescripcionAsync(dto.Descripcion, idPromocionActual))
                throw new ConflictoException("Ya existe una promoción con esa descripción.");
        }

        public async Task<int> CrearAsync(PromocionGuardarDTO dto)
        {
            await ValidarAsync(dto, null);

            return await _promocionRepository.CrearAsync(new Promocion
            {
                Descripcion = dto.Descripcion.Trim(),
                PorcentajeDescuento = dto.PorcentajeDescuento
            });
        }

        public async Task ModificarAsync(int id, PromocionGuardarDTO dto)
        {
            var promocion = await _promocionRepository.GetByIdAsync(id)
                ?? throw new NoEncontradoException($"No existe la promoción con ID {id}.");

            await ValidarAsync(dto, id);

            promocion.Descripcion = dto.Descripcion.Trim();
            promocion.PorcentajeDescuento = dto.PorcentajeDescuento;
            await _promocionRepository.ActualizarAsync(promocion);
        }

        public async Task<int> EliminarAsync(int id)
        {
            var promocion = await _promocionRepository.GetByIdAsync(id)
                ?? throw new NoEncontradoException($"No existe la promoción con ID {id}.");

            return await _promocionRepository.EliminarAsync(promocion);
        }
    }
}
