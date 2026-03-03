using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio para la gestión de Roles y Permisos.
/// </summary>
public class RolService : IRolService
{
    private readonly IRolRepository _rolRepo;
    private readonly IRolModuloPermisoRepository _permisoRepo;
    private readonly IModuloCategoriaRepository _categoriaRepo;

    public RolService(
        IRolRepository rolRepo,
        IRolModuloPermisoRepository permisoRepo,
        IModuloCategoriaRepository categoriaRepo)
    {
        _rolRepo = rolRepo;
        _permisoRepo = permisoRepo;
        _categoriaRepo = categoriaRepo;
    }

    public async Task<IEnumerable<RolDto>> GetAllAsync()
    {
        var roles = await _rolRepo.GetAllAsync();
        return roles.Select(MapToDto);
    }

    public async Task<RolDetalleDto?> GetByIdAsync(int idRol)
    {
        var rol = await _rolRepo.GetWithPermisosAsync(idRol);
        if (rol is null) return null;

        return new RolDetalleDto
        {
            IdRol = rol.IdRol,
            Nombre = rol.Nombre,
            Descripcion = rol.Descripcion,
            Permisos = rol.RolModuloPermisos.Select(p => new RolModuloPermisoDto
            {
                IdRol = p.IdRol,
                IdModuloCategoria = p.IdModuloCategoria,
                NombreModulo = p.ModuloCategoria?.Modulo?.Nombre,
                NombreCategoria = p.ModuloCategoria?.Nombre,
                PuedeLeer = p.PuedeLeer,
                PuedeEscribir = p.PuedeEscribir,
                PuedeEliminar = p.PuedeEliminar
            }).ToList()
        };
    }

    public async Task<RolDto> CreateAsync(CreateRolDto dto)
    {
        var rol = new Rol
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        await _rolRepo.AddAsync(rol);
        await _rolRepo.SaveChangesAsync();
        return MapToDto(rol);
    }

    public async Task<bool> UpdateAsync(int idRol, UpdateRolDto dto)
    {
        var rol = await _rolRepo.GetByIdAsync(idRol);
        if (rol is null) return false;

        rol.Nombre = dto.Nombre;
        rol.Descripcion = dto.Descripcion;

        _rolRepo.Update(rol);
        await _rolRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int idRol)
    {
        var rol = await _rolRepo.GetByIdAsync(idRol);
        if (rol is null) return false;

        _rolRepo.Remove(rol);
        await _rolRepo.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<RolModuloPermisoDto>> GetPermisosAsync(int idRol)
    {
        var permisos = await _permisoRepo.GetByRolWithDetailsAsync(idRol);
        return permisos.Select(p => new RolModuloPermisoDto
        {
            IdRol = p.IdRol,
            IdModuloCategoria = p.IdModuloCategoria,
            NombreModulo = p.ModuloCategoria?.Modulo?.Nombre,
            NombreCategoria = p.ModuloCategoria?.Nombre,
            PuedeLeer = p.PuedeLeer,
            PuedeEscribir = p.PuedeEscribir,
            PuedeEliminar = p.PuedeEliminar
        });
    }

    public async Task<bool> AsignarPermisoAsync(int idRol, AsignarPermisoDto dto)
    {
        // Verificar que existan el rol y la categoría
        var rol = await _rolRepo.GetByIdAsync(idRol);
        if (rol is null) return false;

        var categoria = await _categoriaRepo.GetByIdAsync(dto.IdModuloCategoria);
        if (categoria is null) return false;

        // Verificar si ya existe el permiso; si existe, actualizarlo
        var existente = await _permisoRepo.GetByCompositeKeyAsync(idRol, dto.IdModuloCategoria);
        if (existente is not null)
        {
            existente.PuedeLeer = dto.PuedeLeer;
            existente.PuedeEscribir = dto.PuedeEscribir;
            existente.PuedeEliminar = dto.PuedeEliminar;
            _permisoRepo.Update(existente);
        }
        else
        {
            var permiso = new RolModuloPermiso
            {
                IdRol = idRol,
                IdModuloCategoria = dto.IdModuloCategoria,
                PuedeLeer = dto.PuedeLeer,
                PuedeEscribir = dto.PuedeEscribir,
                PuedeEliminar = dto.PuedeEliminar
            };
            await _permisoRepo.AddAsync(permiso);
        }

        await _permisoRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoverPermisoAsync(int idRol, int idModuloCategoria)
    {
        var permiso = await _permisoRepo.GetByCompositeKeyAsync(idRol, idModuloCategoria);
        if (permiso is null) return false;

        _permisoRepo.Remove(permiso);
        await _permisoRepo.SaveChangesAsync();
        return true;
    }

    // ─── Mapeo privado ───

    private static RolDto MapToDto(Rol rol) => new()
    {
        IdRol = rol.IdRol,
        Nombre = rol.Nombre,
        Descripcion = rol.Descripcion
    };
}
