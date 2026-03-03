using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para la gestión de Sucursales.
/// </summary>
public interface ISucursalService
{
    Task<IEnumerable<SucursalDto>> GetAllAsync();
    Task<SucursalDto?> GetByIdAsync(int idSucursal);
    Task<SucursalDto> CreateAsync(CreateSucursalDto dto);
    Task<bool> UpdateAsync(int idSucursal, UpdateSucursalDto dto);
    Task<bool> DesactivarAsync(int idSucursal);
}
