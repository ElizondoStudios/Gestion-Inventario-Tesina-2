using API.DTOs;

namespace API.Services.Interfaces;

/// <summary>
/// Servicio para la gestión del catálogo de Productos.
/// </summary>
public interface IProductoService
{
    Task<IEnumerable<ProductoDto>> GetAllAsync();
    Task<ProductoDto?> GetByIdAsync(int idProducto);
    Task<ProductoDto> CreateAsync(CreateProductoDto dto);
    Task<bool> UpdateAsync(int idProducto, UpdateProductoDto dto);
    Task<bool> DesactivarAsync(int idProducto);
}
