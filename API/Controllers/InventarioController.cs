using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para el control de Inventario y alertas de stock (Módulo 4).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventarioController : ControllerBase
{
    private readonly IInventarioService _inventarioService;

    public InventarioController(IInventarioService inventarioService)
    {
        _inventarioService = inventarioService;
    }

    /// <summary>
    /// Obtiene todo el inventario.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventarioDto>>> GetAll()
    {
        var inventario = await _inventarioService.GetAllAsync();
        return Ok(inventario);
    }

    /// <summary>
    /// Obtiene el inventario de una sucursal específica.
    /// </summary>
    [HttpGet("sucursal/{idSucursal}")]
    public async Task<ActionResult<IEnumerable<InventarioDto>>> GetBySucursal(int idSucursal)
    {
        var inventario = await _inventarioService.GetBySucursalAsync(idSucursal);
        return Ok(inventario);
    }

    /// <summary>
    /// Obtiene el inventario de un producto en todas las sucursales.
    /// </summary>
    [HttpGet("producto/{idProducto}")]
    public async Task<ActionResult<IEnumerable<InventarioDto>>> GetByProducto(int idProducto)
    {
        var inventario = await _inventarioService.GetByProductoAsync(idProducto);
        return Ok(inventario);
    }

    /// <summary>
    /// Obtiene todas las alertas de stock bajo (RF4.3).
    /// </summary>
    [HttpGet("alertas")]
    public async Task<ActionResult<IEnumerable<InventarioDto>>> GetAlertasStockBajo()
    {
        var alertas = await _inventarioService.GetAlertasStockBajoAsync();
        return Ok(alertas);
    }

    /// <summary>
    /// Obtiene alertas de stock bajo para una sucursal específica (RF4.3).
    /// </summary>
    [HttpGet("alertas/sucursal/{idSucursal}")]
    public async Task<ActionResult<IEnumerable<InventarioDto>>> GetAlertasBySucursal(int idSucursal)
    {
        var alertas = await _inventarioService.GetAlertasStockBajoPorSucursalAsync(idSucursal);
        return Ok(alertas);
    }

    /// <summary>
    /// Actualiza el stock mínimo de un registro de inventario.
    /// </summary>
    [HttpPatch("{idInventario}/stock-minimo")]
    public async Task<IActionResult> UpdateStockMinimo(int idInventario, [FromBody] UpdateStockMinimoDto dto)
    {
        var result = await _inventarioService.UpdateStockMinimoAsync(idInventario, dto);
        if (!result) return NotFound(new { message = "Registro de inventario no encontrado." });
        return NoContent();
    }
}
