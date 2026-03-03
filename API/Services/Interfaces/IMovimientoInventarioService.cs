using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para el registro de Movimientos de Inventario.
/// </summary>
public interface IMovimientoInventarioService
{
    Task<IEnumerable<MovimientoInventarioDto>> GetAllAsync();
    Task<MovimientoInventarioDto?> GetByIdAsync(int idMovimiento);
    Task<IEnumerable<MovimientoInventarioDto>> GetByProductoAsync(int idProducto);
    Task<IEnumerable<MovimientoInventarioDto>> GetBySucursalAsync(int idSucursal);
    Task<IEnumerable<MovimientoInventarioDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<MovimientoInventarioDto> RegistrarMovimientoAsync(CreateMovimientoDto dto);
}
