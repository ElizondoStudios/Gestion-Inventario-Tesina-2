namespace API.DTOs;

// ─── Dashboard: Módulo de Inicio ───

/// <summary>
/// DTO principal del dashboard con todas las métricas.
/// </summary>
public class DashboardDto
{
    public ResumenTarjetaDto VentasTotales { get; set; } = null!;
    public ResumenTarjetaDto ComprasTotales { get; set; } = null!;
    public ResumenStockDto ProductosEnStock { get; set; } = null!;
    public ResumenTarjetaDto MermaTotal { get; set; } = null!;
    public List<VentasVsComprasMesDto> VentasVsComprasPorMes { get; set; } = [];
    public List<VentasPorCategoriaDto> VentasPorCategoria { get; set; } = [];
    public List<AlertaInventarioDto> AlertasInventario { get; set; } = [];
    public List<MovimientoRecienteDto> MovimientosRecientes { get; set; } = [];
}

/// <summary>
/// Tarjeta resumen con monto actual y porcentaje de cambio respecto al mes anterior.
/// </summary>
public class ResumenTarjetaDto
{
    /// <summary>
    /// Monto total del mes actual.
    /// </summary>
    public decimal MontoActual { get; set; }

    /// <summary>
    /// Monto total del mes anterior.
    /// </summary>
    public decimal MontoAnterior { get; set; }

    /// <summary>
    /// Porcentaje de cambio respecto al mes anterior.
    /// Positivo = aumento, Negativo = disminución, null = sin datos previos.
    /// </summary>
    public decimal? PorcentajeCambio { get; set; }
}

/// <summary>
/// Tarjeta resumen de stock de productos.
/// </summary>
public class ResumenStockDto
{
    /// <summary>
    /// Cantidad total de productos en stock (suma de StockActual de todos los inventarios).
    /// </summary>
    public int CantidadTotal { get; set; }

    /// <summary>
    /// Cantidad de registros de inventario con stock bajo (StockActual &lt;= StockMinimo).
    /// </summary>
    public int CantidadStockBajo { get; set; }
}

/// <summary>
/// Ventas vs Compras agrupadas por mes para gráfica comparativa.
/// </summary>
public class VentasVsComprasMesDto
{
    /// <summary>
    /// Mes en formato "yyyy-MM" (ej: "2026-03").
    /// </summary>
    public string Mes { get; set; } = null!;

    /// <summary>
    /// Total de ventas en dinero para el mes.
    /// </summary>
    public decimal TotalVentas { get; set; }

    /// <summary>
    /// Total de compras en dinero para el mes.
    /// </summary>
    public decimal TotalCompras { get; set; }
}

/// <summary>
/// Ventas agrupadas por categoría de producto (nombre del producto).
/// </summary>
public class VentasPorCategoriaDto
{
    /// <summary>
    /// Nombre del producto.
    /// </summary>
    public string Categoria { get; set; } = null!;

    /// <summary>
    /// Total de ventas en dinero para este producto.
    /// </summary>
    public decimal TotalVentas { get; set; }
}

/// <summary>
/// Alerta de inventario con stock bajo.
/// </summary>
public class AlertaInventarioDto
{
    public int IdInventario { get; set; }
    public string NombreProducto { get; set; } = null!;
    public string NumeroParte { get; set; } = null!;
    public string NombreSucursal { get; set; } = null!;
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
}

/// <summary>
/// Movimiento reciente para la lista del dashboard.
/// </summary>
public class MovimientoRecienteDto
{
    public int IdMovimiento { get; set; }
    public string NombreProducto { get; set; } = null!;
    public string TipoMovimiento { get; set; } = null!;
    public int Cantidad { get; set; }
    public string? SucursalOrigen { get; set; }
    public string? SucursalDestino { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public DateTime Fecha { get; set; }
}
