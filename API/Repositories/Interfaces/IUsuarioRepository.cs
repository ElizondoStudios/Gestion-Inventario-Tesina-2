using API.Models;

namespace API.Repositories.Interfaces;

/// <summary>
/// Repositorio para la entidad Usuario.
/// </summary>
public interface IUsuarioRepository : IRepository<Usuario>
{
    /// <summary>
    /// Obtiene un usuario por su correo electrónico.
    /// </summary>
    Task<Usuario?> GetByCorreoAsync(string correo);

    /// <summary>
    /// Obtiene un usuario con su rol cargado.
    /// </summary>
    Task<Usuario?> GetWithRolAsync(int idUsuario);

    /// <summary>
    /// Obtiene un usuario con sus sucursales asignadas.
    /// </summary>
    Task<Usuario?> GetWithSucursalesAsync(int idUsuario);

    /// <summary>
    /// Obtiene todos los usuarios activos.
    /// </summary>
    Task<IEnumerable<Usuario>> GetAllActivosAsync();

    /// <summary>
    /// Verifica si un correo ya está registrado.
    /// </summary>
    Task<bool> ExisteCorreoAsync(string correo);
}
