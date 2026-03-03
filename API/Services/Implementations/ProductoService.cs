using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio para la gestión del catálogo de Productos.
/// </summary>
public class ProductoService : IProductoService
{
    private readonly IProductoRepository _productoRepo;

    public ProductoService(IProductoRepository productoRepo)
    {
        _productoRepo = productoRepo;
    }

    public async Task<IEnumerable<ProductoDto>> GetAllAsync()
    {
        var productos = await _productoRepo.GetAllActivosAsync();
        return productos.Select(MapToDto);
    }

    public async Task<ProductoDto?> GetByIdAsync(int idProducto)
    {
        var producto = await _productoRepo.GetByIdAsync(idProducto);
        return producto is null ? null : MapToDto(producto);
    }

    public async Task<ProductoDto> CreateAsync(CreateProductoDto dto)
    {
        // Verificar unicidad del número de parte
        if (await _productoRepo.ExisteNumeroParteAsync(dto.NumeroParte))
            throw new InvalidOperationException("El número de parte ya está registrado.");

        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            NumeroParte = dto.NumeroParte,
            Precio = dto.Precio,
            Activo = true
        };

        await _productoRepo.AddAsync(producto);
        await _productoRepo.SaveChangesAsync();
        return MapToDto(producto);
    }

    public async Task<bool> UpdateAsync(int idProducto, UpdateProductoDto dto)
    {
        var producto = await _productoRepo.GetByIdAsync(idProducto);
        if (producto is null) return false;

        // Verificar unicidad del número de parte si cambió
        if (producto.NumeroParte != dto.NumeroParte && await _productoRepo.ExisteNumeroParteAsync(dto.NumeroParte))
            throw new InvalidOperationException("El número de parte ya está registrado por otro producto.");

        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.NumeroParte = dto.NumeroParte;
        producto.Precio = dto.Precio;

        _productoRepo.Update(producto);
        await _productoRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DesactivarAsync(int idProducto)
    {
        var producto = await _productoRepo.GetByIdAsync(idProducto);
        if (producto is null) return false;

        producto.Activo = false;
        _productoRepo.Update(producto);
        await _productoRepo.SaveChangesAsync();
        return true;
    }

    // ─── Mapeo privado ───

    private static ProductoDto MapToDto(Producto p) => new()
    {
        IdProducto = p.IdProducto,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        NumeroParte = p.NumeroParte,
        Precio = p.Precio,
        Activo = p.Activo
    };
}
