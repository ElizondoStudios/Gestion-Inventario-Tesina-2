using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para la gestión de Roles y Permisos.
/// </summary>
public interface IRolService
{
    Task<IEnumerable<RolDto>> GetAllAsync();
    Task<RolDetalleDto?> GetByIdAsync(int idRol);
    Task<RolDto> CreateAsync(CreateRolDto dto);
    Task<bool> UpdateAsync(int idRol, UpdateRolDto dto);
    Task<bool> DeleteAsync(int idRol);
    Task<IEnumerable<RolModuloPermisoDto>> GetPermisosAsync(int idRol);
    Task<bool> AsignarPermisoAsync(int idRol, AsignarPermisoDto dto);
    Task<bool> RemoverPermisoAsync(int idRol, int idModuloCategoria);
}
