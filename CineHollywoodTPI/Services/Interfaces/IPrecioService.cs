using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Models;

namespace CineHollywoodTPI.API.Services.Interfaces
{
    public interface IPrecioService
    {
        Task<IEnumerable<PrecioDTO>> GetAllAsync();
        Task<int> CrearAsync(PrecioInsertDTO dto);
        Task EliminarAsync(int idPrecio);

        decimal PrecioBase(Funcion funcion);
        decimal PrecioEntrada(Funcion funcion, int idTipoEspectador);
        PrecioPorTipoDTO DetallePrecio(Funcion funcion, ItemDTO tipoEspectador);
    }
}
