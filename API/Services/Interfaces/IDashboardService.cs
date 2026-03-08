using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para el módulo de Inicio (Dashboard).
/// Proporciona métricas, gráficas y alertas para la vista principal.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Obtiene todas las métricas del dashboard en una sola llamada.
    /// </summary>
    Task<DashboardDto> GetDashboardAsync();

    /// <summary>
    /// Obtiene el resumen de ventas totales del mes actual con porcentaje vs mes anterior.
    /// </summary>
    Task<ResumenTarjetaDto> GetVentasTotalesAsync();

    /// <summary>
    /// Obtiene el resumen de compras totales del mes actual con porcentaje vs mes anterior.
    /// </summary>
    Task<ResumenTarjetaDto> GetComprasTotalesAsync();

    /// <summary>
    /// Obtiene la cantidad total de productos en stock y cuántos tienen stock bajo.
    /// </summary>
    Task<ResumenStockDto> GetProductosEnStockAsync();

    /// <summary>
    /// Obtiene el resumen de merma total del mes actual con porcentaje vs mes anterior.
    /// </summary>
    Task<ResumenTarjetaDto> GetMermaTotalAsync();

    /// <summary>
    /// Obtiene los totales de ventas vs compras agrupados por mes (últimos 12 meses).
    /// </summary>
    Task<List<VentasVsComprasMesDto>> GetVentasVsComprasPorMesAsync();

    /// <summary>
    /// Obtiene las ventas agrupadas por producto (top 10 por monto).
    /// </summary>
    Task<List<VentasPorCategoriaDto>> GetVentasPorCategoriaAsync();

    /// <summary>
    /// Obtiene las alertas de inventario (productos con stock bajo).
    /// </summary>
    Task<List<AlertaInventarioDto>> GetAlertasInventarioAsync();

    /// <summary>
    /// Obtiene los movimientos más recientes (últimos 10).
    /// </summary>
    Task<List<MovimientoRecienteDto>> GetMovimientosRecientesAsync();
}
