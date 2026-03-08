using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para la gestión del catálogo de Tipos de Movimiento.
/// </summary>
public interface ITipoMovimientoService
{
    Task<IEnumerable<TipoMovimientoDto>> GetAllAsync();
    Task<TipoMovimientoDto?> GetByIdAsync(int idTipoMovimiento);
    Task<TipoMovimientoDto> CreateAsync(CreateTipoMovimientoDto dto);
    Task<bool> UpdateAsync(int idTipoMovimiento, UpdateTipoMovimientoDto dto);
    Task<bool> DeleteAsync(int idTipoMovimiento);
}
