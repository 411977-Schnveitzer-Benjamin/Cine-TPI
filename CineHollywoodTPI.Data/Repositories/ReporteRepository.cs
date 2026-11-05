using CineHollywoodTPI.Data.DTOs;
using CineHollywoodTPI.Data.Interfaces;
using CineHollywoodTPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CineHollywoodTPI.Data.Repositories
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly SistemaCineContext _context;

        public ReporteRepository(SistemaCineContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PeliculaMasVendidaDTO>> GetPeliculasMasVendidasAsync(int top, DateTime? desde, DateTime? hasta)
        {
            var entradas = _context.Entrada
                .Where(e => e.IdVenta != null && e.IdFuncionNavigation != null && e.IdFuncionNavigation.IdPelicula != null);

            if (desde.HasValue)
                entradas = entradas.Where(e => e.IdVentaNavigation!.FechaHora >= desde.Value);

            if (hasta.HasValue)
            {
                var hastaFinDelDia = hasta.Value.Date.AddDays(1);
                entradas = entradas.Where(e => e.IdVentaNavigation!.FechaHora < hastaFinDelDia);
            }

            return await entradas
                .GroupBy(e => new
                {
                    IdPelicula = e.IdFuncionNavigation!.IdPelicula!.Value,
                    e.IdFuncionNavigation.IdPeliculaNavigation!.Titulo
                })
                .Select(g => new PeliculaMasVendidaDTO
                {
                    IdPelicula = g.Key.IdPelicula,
                    Titulo = g.Key.Titulo,
                    CantidadEntradas = g.Count(),
                    TotalRecaudado = g.Sum(e => e.PrecioFinal ?? 0m)
                })
                .OrderByDescending(r => r.CantidadEntradas)
                .ThenByDescending(r => r.TotalRecaudado)
                .Take(top)
                .ToListAsync();
        }

        public async Task<IEnumerable<VentaMensualDTO>> GetVentasMensualesAsync(int anio)
        {
            var desde = new DateTime(anio, 1, 1);
            var hasta = desde.AddYears(1);

            // Traemos una fila por venta con sus totales (SQL) y agrupamos por mes en memoria
            var ventas = await _context.Venta
                .Where(v => v.FechaHora >= desde && v.FechaHora < hasta)
                .Select(v => new
                {
                    Mes = v.FechaHora!.Value.Month,
                    Entradas = v.Entrada.Count(),
                    Total = v.Entrada.Sum(e => e.PrecioFinal ?? 0m)
                })
                .ToListAsync();

            return Enumerable.Range(1, 12).Select(mes =>
            {
                var delMes = ventas.Where(v => v.Mes == mes).ToList();
                return new VentaMensualDTO
                {
                    Anio = anio,
                    Mes = mes,
                    CantidadVentas = delMes.Count,
                    CantidadEntradas = delMes.Sum(v => v.Entradas),
                    TotalRecaudado = delMes.Sum(v => v.Total)
                };
            }).ToList();
        }

        public async Task<IEnumerable<int>> GetAniosConVentasAsync()
        {
            return await _context.Venta
                .Where(v => v.FechaHora != null)
                .Select(v => v.FechaHora!.Value.Year)
                .Distinct()
                .OrderByDescending(a => a)
                .ToListAsync();
        }
    }
}
