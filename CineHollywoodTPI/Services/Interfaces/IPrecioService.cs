using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface IPrecioService
    {
        // ABM de precios por tipo de sala
        Task<IEnumerable<PrecioDTO>> GetAllAsync();
        Task<int> CrearAsync(PrecioInsertDTO dto);
        Task EliminarAsync(int idPrecio);

        // Cálculo del precio de una entrada.
        // La función tiene que venir con Sala -> TipoSala -> PrecioTipoSalas y Promocion.
        decimal PrecioBase(Funcion funcion);
        decimal PrecioEntrada(Funcion funcion, int idTipoEspectador);
        PrecioPorTipoDTO DetallePrecio(Funcion funcion, ItemDTO tipoEspectador);
    }
}
