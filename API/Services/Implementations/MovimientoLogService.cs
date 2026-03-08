using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio para la consulta de la Bitácora de Auditoría (MovimientoLog).
/// Los logs se crean automáticamente desde el servicio de MovimientoInventario.
/// </summary>
public class MovimientoLogService : IMovimientoLogService
{
    private readonly IMovimientoLogRepository _logRepo;

    public MovimientoLogService(IMovimientoLogRepository logRepo)
    {
        _logRepo = logRepo;
    }

    public async Task<IEnumerable<MovimientoLogDto>> GetAllAsync()
    {
        var logs = await _logRepo.GetAllAsync();
        var result = new List<MovimientoLogDto>();
        foreach (var l in logs)
        {
            var detalle = await _logRepo.GetWithDetailsAsync(l.IdLog);
            if (detalle is not null) result.Add(MapToDto(detalle));
        }
        return result;
    }

    public async Task<MovimientoLogDto?> GetByIdAsync(int idLog)
    {
        var log = await _logRepo.GetWithDetailsAsync(idLog);
        return log is null ? null : MapToDto(log);
    }

    public async Task<IEnumerable<MovimientoLogDto>> GetByMovimientoAsync(int idMovimiento)
    {
        var logs = await _logRepo.GetByMovimientoAsync(idMovimiento);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<MovimientoLogDto>> GetByUsuarioAsync(int idUsuario)
    {
        var logs = await _logRepo.GetByUsuarioAsync(idUsuario);
        return logs.Select(MapToDto);
    }

    public async Task<IEnumerable<MovimientoLogDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var logs = await _logRepo.GetByFechaRangoAsync(fechaInicio, fechaFin);
        return logs.Select(MapToDto);
    }

    // ─── Mapeo privado ───

    private static MovimientoLogDto MapToDto(MovimientoLog l) => new()
    {
        IdLog = l.IdLog,
        IdMovimiento = l.IdMovimiento,
        IdUsuario = l.IdUsuario,
        NombreUsuario = l.Usuario?.Nombre,
        Accion = l.Accion,
        Fecha = l.Fecha,
        ValorAnterior = l.ValorAnterior,
        ValorNuevo = l.ValorNuevo
    };
}
