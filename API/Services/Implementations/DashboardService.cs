using API.Data;
using API.DTOs;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio de Dashboard (Módulo de Inicio).
/// Calcula métricas de ventas, compras, merma, stock y movimientos recientes.
/// Los tipos "Venta", "Compra" y "Merma" se identifican por el nombre del TipoMovimiento.
/// El monto se calcula como: Cantidad × Precio del Producto.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<DashboardDto> GetDashboardAsync()
    {
        var ventasTotales = await GetVentasTotalesAsync();
        var comprasTotales = await GetComprasTotalesAsync();
        var productosEnStock = await GetProductosEnStockAsync();
        var mermaTotal = await GetMermaTotalAsync();
        var ventasVsCompras = await GetVentasVsComprasPorMesAsync();
        var ventasPorCategoria = await GetVentasPorCategoriaAsync();
        var alertas = await GetAlertasInventarioAsync();
        var movimientosRecientes = await GetMovimientosRecientesAsync();

        return new DashboardDto
        {
            VentasTotales = ventasTotales,
            ComprasTotales = comprasTotales,
            ProductosEnStock = productosEnStock,
            MermaTotal = mermaTotal,
            VentasVsComprasPorMes = ventasVsCompras,
            VentasPorCategoria = ventasPorCategoria,
            AlertasInventario = alertas,
            MovimientosRecientes = movimientosRecientes
        };
    }

    /// <inheritdoc />
    public async Task<ResumenTarjetaDto> GetVentasTotalesAsync()
    {
        return await GetResumenPorTipoAsync("Venta");
    }

    /// <inheritdoc />
    public async Task<ResumenTarjetaDto> GetComprasTotalesAsync()
    {
        return await GetResumenPorTipoAsync("Compra");
    }

    /// <inheritdoc />
    public async Task<ResumenTarjetaDto> GetMermaTotalAsync()
    {
        return await GetResumenPorTipoAsync("Merma");
    }

    /// <inheritdoc />
    public async Task<ResumenStockDto> GetProductosEnStockAsync()
    {
        var inventarios = await _context.Inventarios.ToListAsync();

        return new ResumenStockDto
        {
            CantidadTotal = inventarios.Sum(i => i.StockActual),
            CantidadStockBajo = inventarios.Count(i => i.StockActual <= i.StockMinimo)
        };
    }

    /// <inheritdoc />
    public async Task<List<VentasVsComprasMesDto>> GetVentasVsComprasPorMesAsync()
    {
        var hace12Meses = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-11);

        var movimientos = await _context.MovimientosInventario
            .Include(m => m.TipoMovimiento)
            .Include(m => m.Producto)
            .Where(m => m.Fecha >= hace12Meses &&
                        (m.TipoMovimiento.Nombre == "Venta" || m.TipoMovimiento.Nombre == "Compra"))
            .ToListAsync();

        // Generar los últimos 12 meses
        var resultado = new List<VentasVsComprasMesDto>();
        for (int i = 0; i < 12; i++)
        {
            var mes = hace12Meses.AddMonths(i);
            var mesKey = mes.ToString("yyyy-MM");

            var movimientosMes = movimientos
                .Where(m => m.Fecha.Year == mes.Year && m.Fecha.Month == mes.Month);

            resultado.Add(new VentasVsComprasMesDto
            {
                Mes = mesKey,
                TotalVentas = movimientosMes
                    .Where(m => m.TipoMovimiento.Nombre == "Venta")
                    .Sum(m => m.Cantidad * m.Producto.Precio),
                TotalCompras = movimientosMes
                    .Where(m => m.TipoMovimiento.Nombre == "Compra")
                    .Sum(m => m.Cantidad * m.Producto.Precio)
            });
        }

        return resultado;
    }

    /// <inheritdoc />
    public async Task<List<VentasPorCategoriaDto>> GetVentasPorCategoriaAsync()
    {
        var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        var ventas = await _context.MovimientosInventario
            .Include(m => m.TipoMovimiento)
            .Include(m => m.Producto)
            .Where(m => m.TipoMovimiento.Nombre == "Venta" && m.Fecha >= inicioMes)
            .ToListAsync();

        return ventas
            .GroupBy(m => m.Producto.Nombre)
            .Select(g => new VentasPorCategoriaDto
            {
                Categoria = g.Key,
                TotalVentas = g.Sum(m => m.Cantidad * m.Producto.Precio)
            })
            .OrderByDescending(v => v.TotalVentas)
            .Take(10)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<List<AlertaInventarioDto>> GetAlertasInventarioAsync()
    {
        var alertas = await _context.Inventarios
            .Include(i => i.Producto)
            .Include(i => i.Sucursal)
            .Where(i => i.StockActual <= i.StockMinimo)
            .OrderBy(i => i.StockActual)
            .ToListAsync();

        return alertas.Select(i => new AlertaInventarioDto
        {
            IdInventario = i.IdInventario,
            NombreProducto = i.Producto.Nombre,
            NumeroParte = i.Producto.NumeroParte,
            NombreSucursal = i.Sucursal.Nombre,
            StockActual = i.StockActual,
            StockMinimo = i.StockMinimo
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<List<MovimientoRecienteDto>> GetMovimientosRecientesAsync()
    {
        var movimientos = await _context.MovimientosInventario
            .Include(m => m.Producto)
            .Include(m => m.TipoMovimiento)
            .Include(m => m.Usuario)
            .Include(m => m.SucursalOrigen)
            .Include(m => m.SucursalDestino)
            .OrderByDescending(m => m.Fecha)
            .Take(10)
            .ToListAsync();

        return movimientos.Select(m => new MovimientoRecienteDto
        {
            IdMovimiento = m.IdMovimiento,
            NombreProducto = m.Producto.Nombre,
            TipoMovimiento = m.TipoMovimiento.Nombre,
            Cantidad = m.Cantidad,
            SucursalOrigen = m.SucursalOrigen?.Nombre,
            SucursalDestino = m.SucursalDestino?.Nombre,
            NombreUsuario = m.Usuario.Nombre,
            Fecha = m.Fecha
        }).ToList();
    }

    // ─── Método auxiliar ───

    /// <summary>
    /// Calcula el resumen (monto actual, anterior y % de cambio) para un tipo de movimiento.
    /// </summary>
    private async Task<ResumenTarjetaDto> GetResumenPorTipoAsync(string nombreTipo)
    {
        var ahora = DateTime.Now;
        var inicioMesActual = new DateTime(ahora.Year, ahora.Month, 1);
        var inicioMesAnterior = inicioMesActual.AddMonths(-1);

        var movimientos = await _context.MovimientosInventario
            .Include(m => m.TipoMovimiento)
            .Include(m => m.Producto)
            .Where(m => m.TipoMovimiento.Nombre == nombreTipo &&
                        m.Fecha >= inicioMesAnterior)
            .ToListAsync();

        var montoActual = movimientos
            .Where(m => m.Fecha >= inicioMesActual)
            .Sum(m => m.Cantidad * m.Producto.Precio);

        var montoAnterior = movimientos
            .Where(m => m.Fecha >= inicioMesAnterior && m.Fecha < inicioMesActual)
            .Sum(m => m.Cantidad * m.Producto.Precio);

        decimal? porcentajeCambio = null;
        if (montoAnterior > 0)
        {
            porcentajeCambio = Math.Round(((montoActual - montoAnterior) / montoAnterior) * 100, 2);
        }

        return new ResumenTarjetaDto
        {
            MontoActual = montoActual,
            MontoAnterior = montoAnterior,
            PorcentajeCambio = porcentajeCambio
        };
    }
}
