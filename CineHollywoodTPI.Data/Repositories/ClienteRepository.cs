using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly SistemaCineContext _context;

        public ClienteRepository(SistemaCineContext context)
        {
            _context = context;
        }

        // 1. Traer todos los clientes activos con sus datos personales (Nombre, Apellido, Documento)
        public async Task<IEnumerable<Cliente>> GetAllActiveAsync()
        {
            return await _context.Cliente
                .Where(c => c.Activo)
                .Include(c => c.IdClienteNavigation) // ¡Clave! Trae los datos heredados de la tabla PERSONA
                .ToListAsync();
        }

        // 2. Buscar un cliente específico por su ID
        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _context.Cliente
                .Include(c => c.IdClienteNavigation)
                .FirstOrDefaultAsync(c => c.IdCliente == id);
        }

        // 3. AGREGAR CLIENTE (Con transacción para asegurar que se creen Persona y Cliente juntos)
        public async Task<bool> AddAsync(Cliente cliente)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                cliente.Activo = true; // Aseguramos que nazca activo

                // 1. Si viene con la navegación de persona cargada, EF Core da de alta ambas automáticamente.
                // Si la persona ya existe, solo asociamos el ID.
                _context.Cliente.Add(cliente);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // 4. Modificar datos del cliente
        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            // EF Core se encarga de actualizar tanto la tabla CLIENTE como la tabla PERSONA vinculada
            _context.Cliente.Update(cliente);
            return await _context.SaveChangesAsync() > 0;
        }

        // 5. BORRADO LÓGICO: Desactivar al cliente del sistema
        public async Task<bool> DeleteLogicoAsync(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null) return false;

            cliente.Activo = false; // Desactivación lógica
            _context.Cliente.Update(cliente);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
