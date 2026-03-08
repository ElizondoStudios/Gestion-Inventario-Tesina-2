using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para el módulo de Inicio (Dashboard).
/// Provee métricas, gráficas y alertas para la vista principal del sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Obtiene todas las métricas del dashboard en una sola llamada.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> GetDashboard()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();
        return Ok(dashboard);
    }

    /// <summary>
    /// Obtiene el resumen de ventas totales del mes actual con % vs mes anterior.
    /// </summary>
    [HttpGet("ventas-totales")]
    public async Task<ActionResult<ResumenTarjetaDto>> GetVentasTotales()
    {
        var result = await _dashboardService.GetVentasTotalesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el resumen de compras totales del mes actual con % vs mes anterior.
    /// </summary>
    [HttpGet("compras-totales")]
    public async Task<ActionResult<ResumenTarjetaDto>> GetComprasTotales()
    {
        var result = await _dashboardService.GetComprasTotalesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene la cantidad total de productos en stock y cuántos tienen stock bajo.
    /// </summary>
    [HttpGet("productos-stock")]
    public async Task<ActionResult<ResumenStockDto>> GetProductosEnStock()
    {
        var result = await _dashboardService.GetProductosEnStockAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el resumen de merma total del mes actual con % vs mes anterior.
    /// </summary>
    [HttpGet("merma-total")]
    public async Task<ActionResult<ResumenTarjetaDto>> GetMermaTotal()
    {
        var result = await _dashboardService.GetMermaTotalAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene los totales de ventas vs compras agrupados por mes (últimos 12 meses).
    /// </summary>
    [HttpGet("ventas-vs-compras")]
    public async Task<ActionResult<List<VentasVsComprasMesDto>>> GetVentasVsCompras()
    {
        var result = await _dashboardService.GetVentasVsComprasPorMesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene las ventas agrupadas por categoría de producto (top 10).
    /// </summary>
    [HttpGet("ventas-por-categoria")]
    public async Task<ActionResult<List<VentasPorCategoriaDto>>> GetVentasPorCategoria()
    {
        var result = await _dashboardService.GetVentasPorCategoriaAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene las alertas de inventario (productos con stock bajo).
    /// </summary>
    [HttpGet("alertas-inventario")]
    public async Task<ActionResult<List<AlertaInventarioDto>>> GetAlertasInventario()
    {
        var result = await _dashboardService.GetAlertasInventarioAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene los movimientos más recientes (últimos 10).
    /// </summary>
    [HttpGet("movimientos-recientes")]
    public async Task<ActionResult<List<MovimientoRecienteDto>>> GetMovimientosRecientes()
    {
        var result = await _dashboardService.GetMovimientosRecientesAsync();
        return Ok(result);
    }
}
