using API.DTOs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador para la consulta de la Bitácora de Auditoría (MovimientoLog).
/// Los logs se generan automáticamente al registrar movimientos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MovimientoLogsController : ControllerBase
{
    private readonly IMovimientoLogService _logService;

    public MovimientoLogsController(IMovimientoLogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// Obtiene todos los logs de auditoría.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimientoLogDto>>> GetAll()
    {
        var logs = await _logService.GetAllAsync();
        return Ok(logs);
    }

    /// <summary>
    /// Obtiene un log por su Id.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MovimientoLogDto>> GetById(int id)
    {
        var log = await _logService.GetByIdAsync(id);
        if (log is null) return NotFound(new { message = "Log no encontrado." });
        return Ok(log);
    }

    /// <summary>
    /// Obtiene los logs de un movimiento específico.
    /// </summary>
    [HttpGet("movimiento/{idMovimiento}")]
    public async Task<ActionResult<IEnumerable<MovimientoLogDto>>> GetByMovimiento(int idMovimiento)
    {
        var logs = await _logService.GetByMovimientoAsync(idMovimiento);
        return Ok(logs);
    }

    /// <summary>
    /// Obtiene los logs realizados por un usuario específico.
    /// </summary>
    [HttpGet("usuario/{idUsuario}")]
    public async Task<ActionResult<IEnumerable<MovimientoLogDto>>> GetByUsuario(int idUsuario)
    {
        var logs = await _logService.GetByUsuarioAsync(idUsuario);
        return Ok(logs);
    }

    /// <summary>
    /// Obtiene los logs en un rango de fechas.
    /// </summary>
    [HttpGet("fecha")]
    public async Task<ActionResult<IEnumerable<MovimientoLogDto>>> GetByFechaRango(
        [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
    {
        if (fechaInicio > fechaFin)
            return BadRequest(new { message = "La fecha de inicio no puede ser posterior a la fecha fin." });

        var logs = await _logService.GetByFechaRangoAsync(fechaInicio, fechaFin);
        return Ok(logs);
    }
}
