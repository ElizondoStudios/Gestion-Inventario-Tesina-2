using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para la gestión de Usuarios y Autenticación.
/// </summary>
public interface IUsuarioService
{
    Task<IEnumerable<UsuarioDto>> GetAllAsync();
    Task<UsuarioDetalleDto?> GetByIdAsync(int idUsuario);
    Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto);
    Task<bool> UpdateAsync(int idUsuario, UpdateUsuarioDto dto);
    Task<bool> DesactivarAsync(int idUsuario);
    Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    Task<bool> AsignarSucursalAsync(int idUsuario, AsignarSucursalDto dto);
    Task<bool> RemoverSucursalAsync(int idUsuario, int idSucursal);
}
