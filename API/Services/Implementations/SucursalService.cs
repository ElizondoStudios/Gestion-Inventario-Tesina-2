using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio para la gestión de Sucursales.
/// </summary>
public class SucursalService : ISucursalService
{
    private readonly ISucursalRepository _sucursalRepo;

    public SucursalService(ISucursalRepository sucursalRepo)
    {
        _sucursalRepo = sucursalRepo;
    }

    public async Task<IEnumerable<SucursalDto>> GetAllAsync()
    {
        var sucursales = await _sucursalRepo.GetAllActivasAsync();
        return sucursales.Select(MapToDto);
    }

    public async Task<SucursalDto?> GetByIdAsync(int idSucursal)
    {
        var sucursal = await _sucursalRepo.GetByIdAsync(idSucursal);
        return sucursal is null ? null : MapToDto(sucursal);
    }

    public async Task<SucursalDto> CreateAsync(CreateSucursalDto dto)
    {
        var sucursal = new Sucursal
        {
            Nombre = dto.Nombre,
            Direccion = dto.Direccion,
            Ciudad = dto.Ciudad,
            Estado = dto.Estado,
            Activa = true
        };

        await _sucursalRepo.AddAsync(sucursal);
        await _sucursalRepo.SaveChangesAsync();
        return MapToDto(sucursal);
    }

    public async Task<bool> UpdateAsync(int idSucursal, UpdateSucursalDto dto)
    {
        var sucursal = await _sucursalRepo.GetByIdAsync(idSucursal);
        if (sucursal is null) return false;

        sucursal.Nombre = dto.Nombre;
        sucursal.Direccion = dto.Direccion;
        sucursal.Ciudad = dto.Ciudad;
        sucursal.Estado = dto.Estado;

        _sucursalRepo.Update(sucursal);
        await _sucursalRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DesactivarAsync(int idSucursal)
    {
        var sucursal = await _sucursalRepo.GetByIdAsync(idSucursal);
        if (sucursal is null) return false;

        sucursal.Activa = false;
        _sucursalRepo.Update(sucursal);
        await _sucursalRepo.SaveChangesAsync();
        return true;
    }

    // ─── Mapeo privado ───

    private static SucursalDto MapToDto(Sucursal s) => new()
    {
        IdSucursal = s.IdSucursal,
        Nombre = s.Nombre,
        Direccion = s.Direccion,
        Ciudad = s.Ciudad,
        Estado = s.Estado,
        Activa = s.Activa
    };
}
