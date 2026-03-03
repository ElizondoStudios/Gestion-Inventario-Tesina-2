using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para la gestión de Módulos y Categorías del sistema.
/// </summary>
public interface IModuloService
{
    Task<IEnumerable<ModuloDto>> GetAllAsync();
    Task<ModuloDetalleDto?> GetByIdAsync(int idModulo);
    Task<ModuloCategoriaDto> CreateCategoriaAsync(int idModulo, CreateModuloCategoriaDto dto);
    Task<bool> UpdateCategoriaAsync(int idModuloCategoria, UpdateModuloCategoriaDto dto);
    Task<bool> DeleteCategoriaAsync(int idModuloCategoria);
}
