using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio para la gestión del catálogo de Tipos de Movimiento.
/// </summary>
public class TipoMovimientoService : ITipoMovimientoService
{
    private readonly ITipoMovimientoRepository _tipoMovimientoRepo;

    public TipoMovimientoService(ITipoMovimientoRepository tipoMovimientoRepo)
    {
        _tipoMovimientoRepo = tipoMovimientoRepo;
    }

    public async Task<IEnumerable<TipoMovimientoDto>> GetAllAsync()
    {
        var tipos = await _tipoMovimientoRepo.GetAllAsync();
        return tipos.Select(MapToDto);
    }

    public async Task<TipoMovimientoDto?> GetByIdAsync(int idTipoMovimiento)
    {
        var tipo = await _tipoMovimientoRepo.GetByIdAsync(idTipoMovimiento);
        return tipo is null ? null : MapToDto(tipo);
    }

    public async Task<TipoMovimientoDto> CreateAsync(CreateTipoMovimientoDto dto)
    {
        // Verificar unicidad del nombre
        if (await _tipoMovimientoRepo.ExisteNombreAsync(dto.Nombre))
            throw new InvalidOperationException("Ya existe un tipo de movimiento con ese nombre.");

        var tipo = new TipoMovimiento
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            AfectaStock = dto.AfectaStock,
            EsTransferencia = dto.EsTransferencia
        };

        await _tipoMovimientoRepo.AddAsync(tipo);
        await _tipoMovimientoRepo.SaveChangesAsync();
        return MapToDto(tipo);
    }

    public async Task<bool> UpdateAsync(int idTipoMovimiento, UpdateTipoMovimientoDto dto)
    {
        var tipo = await _tipoMovimientoRepo.GetByIdAsync(idTipoMovimiento);
        if (tipo is null) return false;

        // Verificar unicidad del nombre si cambió
        if (tipo.Nombre != dto.Nombre && await _tipoMovimientoRepo.ExisteNombreAsync(dto.Nombre))
            throw new InvalidOperationException("Ya existe otro tipo de movimiento con ese nombre.");

        tipo.Nombre = dto.Nombre;
        tipo.Descripcion = dto.Descripcion;
        tipo.AfectaStock = dto.AfectaStock;
        tipo.EsTransferencia = dto.EsTransferencia;

        _tipoMovimientoRepo.Update(tipo);
        await _tipoMovimientoRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int idTipoMovimiento)
    {
        var tipo = await _tipoMovimientoRepo.GetByIdAsync(idTipoMovimiento);
        if (tipo is null) return false;

        _tipoMovimientoRepo.Remove(tipo);
        await _tipoMovimientoRepo.SaveChangesAsync();
        return true;
    }

    // ─── Mapeo privado ───

    private static TipoMovimientoDto MapToDto(TipoMovimiento t) => new()
    {
        IdTipoMovimiento = t.IdTipoMovimiento,
        Nombre = t.Nombre,
        Descripcion = t.Descripcion,
        AfectaStock = t.AfectaStock,
        EsTransferencia = t.EsTransferencia
    };
}
