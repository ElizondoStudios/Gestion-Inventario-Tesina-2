using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para el registro de Movimientos de Inventario (Módulo 4).
/// Soporta Entradas, Salidas y Transferencias.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MovimientosController : ControllerBase
{
    private readonly IMovimientoInventarioService _movimientoService;

    public MovimientosController(IMovimientoInventarioService movimientoService)
    {
        _movimientoService = movimientoService;
    }

    /// <summary>
    /// Obtiene todos los movimientos de inventario.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimientoInventarioDto>>> GetAll()
    {
        var movimientos = await _movimientoService.GetAllAsync();
        return Ok(movimientos);
    }

    /// <summary>
    /// Obtiene un movimiento por su Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MovimientoInventarioDto>> GetById(int id)
    {
        var movimiento = await _movimientoService.GetByIdAsync(id);
        if (movimiento is null) return NotFound(new { message = "Movimiento no encontrado." });
        return Ok(movimiento);
    }

    /// <summary>
    /// Obtiene los movimientos de un producto.
    /// </summary>
    [HttpGet("producto/{idProducto}")]
    public async Task<ActionResult<IEnumerable<MovimientoInventarioDto>>> GetByProducto(int idProducto)
    {
        var movimientos = await _movimientoService.GetByProductoAsync(idProducto);
        return Ok(movimientos);
    }

    /// <summary>
    /// Obtiene los movimientos de una sucursal (como origen o destino).
    /// </summary>
    [HttpGet("sucursal/{idSucursal}")]
    public async Task<ActionResult<IEnumerable<MovimientoInventarioDto>>> GetBySucursal(int idSucursal)
    {
        var movimientos = await _movimientoService.GetBySucursalAsync(idSucursal);
        return Ok(movimientos);
    }

    /// <summary>
    /// Obtiene los movimientos en un rango de fechas.
    /// </summary>
    [HttpGet("fecha")]
    public async Task<ActionResult<IEnumerable<MovimientoInventarioDto>>> GetByFechaRango(
        [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        if (fechaInicio > fechaFin)
            return BadRequest(new { message = "La fecha de inicio no puede ser posterior a la fecha fin." });

        var movimientos = await _movimientoService.GetByFechaRangoAsync(fechaInicio, fechaFin);
        return Ok(movimientos);
    }

    /// <summary>
    /// Registra un nuevo movimiento de inventario (Entrada, Salida o Transferencia).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MovimientoInventarioDto>> Registrar([FromBody] CreateMovimientoDto dto)
    {
        try
        {
            var movimiento = await _movimientoService.RegistrarMovimientoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = movimiento.IdMovimiento }, movimiento);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
