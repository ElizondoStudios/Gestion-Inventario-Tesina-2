using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la gestión del catálogo de Productos (Módulo 4).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _productoService;

    public ProductosController(IProductoService productoService)
    {
        _productoService = productoService;
    }

    /// <summary>
    /// Obtiene todos los productos activos.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> GetAll()
    {
        var productos = await _productoService.GetAllAsync();
        return Ok(productos);
    }

    /// <summary>
    /// Obtiene un producto por su Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoDto>> GetById(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto is null) return NotFound(new { message = "Producto no encontrado." });
        return Ok(producto);
    }

    /// <summary>
    /// Crea un nuevo producto.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductoDto>> Create([FromBody] CreateProductoDto dto)
    {
        try
        {
            var producto = await _productoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = producto.IdProducto }, producto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un producto existente.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductoDto dto)
    {
        try
        {
            var result = await _productoService.UpdateAsync(id, dto);
            if (!result) return NotFound(new { message = "Producto no encontrado." });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Desactiva un producto (borrado lógico).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var result = await _productoService.DesactivarAsync(id);
        if (!result) return NotFound(new { message = "Producto no encontrado." });
        return NoContent();
    }
}
