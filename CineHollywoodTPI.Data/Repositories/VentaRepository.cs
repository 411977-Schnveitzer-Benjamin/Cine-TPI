using System.Data;
using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly SistemaCineContext _context;

        public VentaRepository(SistemaCineContext context)
        {
            _context = context;
        }

        // 1. Listado de ventas (maestro) con totales calculados desde el detalle
        public async Task<IEnumerable<VentaListadoDTO>> GetAllAsync(int? idCliente = null)
        {
            var query = _context.Venta.AsQueryable();
            if (idCliente.HasValue)
                query = query.Where(v => v.IdCliente == idCliente.Value);

            var ventas = await query
                .OrderByDescending(v => v.FechaHora)
                .Select(v => new
                {
                    v.IdVenta,
                    v.FechaHora,
                    v.IdCliente,
                    Nombre = v.IdClienteNavigation != null ? v.IdClienteNavigation.IdClienteNavigation.Nombre : null,
                    Apellido = v.IdClienteNavigation != null ? v.IdClienteNavigation.IdClienteNavigation.Apellido : null,
                    Canal = v.IdCanalNavigation != null ? v.IdCanalNavigation.Descripcion : null,
                    FormasPago = v.VentaxFormaPagos.Select(fp => fp.IdFormaPagoNavigation!.Descripcion).ToList(),
                    CantidadEntradas = v.Entrada.Count(),
                    Total = v.Entrada.Sum(e => e.PrecioFinal ?? 0m)
                })
                .ToListAsync();

            return ventas.Select(v => new VentaListadoDTO
            {
                IdVenta = v.IdVenta,
                FechaHora = v.FechaHora,
                IdCliente = v.IdCliente,
                Cliente = $"{v.Apellido}, {v.Nombre}".Trim(' ', ','),
                Canal = v.Canal,
                FormasPago = string.Join(" + ", v.FormasPago.Where(f => f != null)),
                CantidadEntradas = v.CantidadEntradas,
                Total = v.Total
            });
        }

        // 2. Una venta con todo su detalle (para el comprobante)
        public async Task<VentaDetalleDTO?> GetDetalleAsync(int id)
        {
            var venta = await _context.Venta
                .Include(v => v.IdClienteNavigation!).ThenInclude(c => c.IdClienteNavigation)
                .Include(v => v.IdCanalNavigation)
                .Include(v => v.VentaxFormaPagos).ThenInclude(fp => fp.IdFormaPagoNavigation)
                .Include(v => v.Entrada).ThenInclude(e => e.IdFuncionNavigation!).ThenInclude(f => f.IdPeliculaNavigation)
                .Include(v => v.Entrada).ThenInclude(e => e.IdFuncionNavigation!).ThenInclude(f => f.IdSalaNavigation)
                .Include(v => v.Entrada).ThenInclude(e => e.IdButacaNavigation)
                .Include(v => v.Entrada).ThenInclude(e => e.IdTipoEspectadorNavigation)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.IdVenta == id);

            if (venta == null) return null;

            var persona = venta.IdClienteNavigation?.IdClienteNavigation;

            return new VentaDetalleDTO
            {
                IdVenta = venta.IdVenta,
                FechaHora = venta.FechaHora,
                IdCliente = venta.IdCliente,
                Cliente = persona != null ? $"{persona.Apellido}, {persona.Nombre}" : null,
                Canal = venta.IdCanalNavigation?.Descripcion,
                FormasPago = string.Join(" + ", venta.VentaxFormaPagos
                    .Select(fp => $"{fp.IdFormaPagoNavigation?.Descripcion} (${fp.Monto:N2})")),
                CantidadEntradas = venta.Entrada.Count,
                Total = venta.Entrada.Sum(e => e.PrecioFinal ?? 0m),
                Entradas = venta.Entrada.Select(e => new EntradaDetalleDTO
                {
                    IdEntrada = e.IdEntrada,
                    IdFuncion = e.IdFuncion,
                    Pelicula = e.IdFuncionNavigation?.IdPeliculaNavigation?.Titulo,
                    Fecha = e.IdFuncionNavigation?.Fecha,
                    HoraInicio = e.IdFuncionNavigation?.HoraInicio,
                    Sala = e.IdFuncionNavigation?.IdSalaNavigation?.Nombre,
                    Butaca = e.IdButacaNavigation != null
                        ? $"{e.IdButacaNavigation.Fila}{e.IdButacaNavigation.Numero}"
                        : null,
                    TipoEspectador = e.IdTipoEspectadorNavigation?.Descripcion,
                    PrecioFinal = e.PrecioFinal
                }).ToList()
            };
        }

        // 3. Totales del negocio para el dashboard
        public async Task<ResumenVentasDTO> GetTotalesAsync()
        {
            var totalVentas = await _context.Venta.CountAsync();
            var totalEntradas = await _context.Entrada.CountAsync(e => e.IdVenta != null);
            var totalRecaudado = await _context.Entrada
                .Where(e => e.IdVenta != null)
                .SumAsync(e => e.PrecioFinal ?? 0m);

            return new ResumenVentasDTO
            {
                TotalVentas = totalVentas,
                TotalEntradas = totalEntradas,
                TotalRecaudado = totalRecaudado,
                FechaConsulta = DateTime.Now
            };
        }

        // 4. TRANSACCIÓN CRÍTICA: Registrar la venta (maestro) y sus entradas (detalle) juntas.
        //    Las validaciones de negocio y los precios ya los hizo VentaService.
        //    Si cualquier paso falla se hace ROLLBACK y no queda nada a medias.
        public async Task<int?> RegistrarVentaAsync(Venta venta, IEnumerable<Entrada> entradas, IEnumerable<VentaxFormaPago> formasPago)
        {
            var listaEntradas = entradas.ToList();
            var listaPagos = formasPago.ToList();

            // Serializable: bloquea el rango leído, así dos ventas simultáneas no venden la misma butaca
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // Control de integridad dentro de la transacción: ¿alguna butaca se vendió recién?
                var idsFuncion = listaEntradas.Select(e => e.IdFuncion).Distinct().ToList();
                var idsButaca = listaEntradas.Select(e => e.IdButaca).Distinct().ToList();
                var yaVendidas = await _context.Entrada
                    .Where(e => idsFuncion.Contains(e.IdFuncion) && idsButaca.Contains(e.IdButaca))
                    .Select(e => new { e.IdFuncion, e.IdButaca })
                    .ToListAsync();

                if (listaEntradas.Any(n => yaVendidas.Any(v => v.IdFuncion == n.IdFuncion && v.IdButaca == n.IdButaca)))
                {
                    await transaction.RollbackAsync();
                    return null;
                }

                // --- Maestro ---
                venta.FechaHora ??= DateTime.Now;
                _context.Venta.Add(venta);
                await _context.SaveChangesAsync();

                // --- Detalle ---
                foreach (var entrada in listaEntradas)
                {
                    entrada.IdVenta = venta.IdVenta;
                    _context.Entrada.Add(entrada);
                }

                foreach (var pago in listaPagos)
                {
                    pago.IdVenta = venta.IdVenta;
                    _context.VentaxFormaPagos.Add(pago);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return venta.IdVenta;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
