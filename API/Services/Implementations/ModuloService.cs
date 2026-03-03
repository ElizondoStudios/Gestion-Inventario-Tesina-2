using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio para la gestión de Módulos y Categorías.
/// </summary>
public class ModuloService : IModuloService
{
    private readonly IModuloRepository _moduloRepo;
    private readonly IModuloCategoriaRepository _categoriaRepo;

    public ModuloService(IModuloRepository moduloRepo, IModuloCategoriaRepository categoriaRepo)
    {
        _moduloRepo = moduloRepo;
        _categoriaRepo = categoriaRepo;
    }

    public async Task<IEnumerable<ModuloDto>> GetAllAsync()
    {
        var modulos = await _moduloRepo.GetAllActivosAsync();
        return modulos.Select(m => new ModuloDto
        {
            IdModulo = m.IdModulo,
            Nombre = m.Nombre,
            Descripcion = m.Descripcion,
            Activo = m.Activo
        });
    }

    public async Task<ModuloDetalleDto?> GetByIdAsync(int idModulo)
    {
        var modulo = await _moduloRepo.GetWithCategoriasAsync(idModulo);
        if (modulo is null) return null;

        return new ModuloDetalleDto
        {
            IdModulo = modulo.IdModulo,
            Nombre = modulo.Nombre,
            Descripcion = modulo.Descripcion,
            Activo = modulo.Activo,
            Categorias = modulo.ModuloCategorias.Select(mc => new ModuloCategoriaDto
            {
                IdModuloCategoria = mc.IdModuloCategoria,
                IdModulo = mc.IdModulo,
                Nombre = mc.Nombre,
                Descripcion = mc.Descripcion,
                NombreModulo = modulo.Nombre
            }).ToList()
        };
    }

    public async Task<ModuloCategoriaDto> CreateCategoriaAsync(int idModulo, CreateModuloCategoriaDto dto)
    {
        var modulo = await _moduloRepo.GetByIdAsync(idModulo);
        if (modulo is null)
            throw new InvalidOperationException("El módulo especificado no existe.");

        var categoria = new ModuloCategoria
        {
            IdModulo = idModulo,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        await _categoriaRepo.AddAsync(categoria);
        await _categoriaRepo.SaveChangesAsync();

        return new ModuloCategoriaDto
        {
            IdModuloCategoria = categoria.IdModuloCategoria,
            IdModulo = categoria.IdModulo,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            NombreModulo = modulo.Nombre
        };
    }

    public async Task<bool> UpdateCategoriaAsync(int idModuloCategoria, UpdateModuloCategoriaDto dto)
    {
        var categoria = await _categoriaRepo.GetByIdAsync(idModuloCategoria);
        if (categoria is null) return false;

        categoria.Nombre = dto.Nombre;
        categoria.Descripcion = dto.Descripcion;

        _categoriaRepo.Update(categoria);
        await _categoriaRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategoriaAsync(int idModuloCategoria)
    {
        var categoria = await _categoriaRepo.GetByIdAsync(idModuloCategoria);
        if (categoria is null) return false;

        _categoriaRepo.Remove(categoria);
        await _categoriaRepo.SaveChangesAsync();
        return true;
    }
}
