using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio para la gestión de Usuarios y Autenticación.
/// Utiliza HMACSHA512 para el hash seguro de contraseñas (RNF1.1).
/// </summary>
public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IRolRepository _rolRepo;
    private readonly IUsuarioSucursalRepository _usuarioSucursalRepo;
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IRolModuloPermisoRepository _permisoRepo;

    public UsuarioService(
        IUsuarioRepository usuarioRepo,
        IRolRepository rolRepo,
        IUsuarioSucursalRepository usuarioSucursalRepo,
        ISucursalRepository sucursalRepo,
        IRolModuloPermisoRepository permisoRepo)
    {
        _usuarioRepo = usuarioRepo;
        _rolRepo = rolRepo;
        _usuarioSucursalRepo = usuarioSucursalRepo;
        _sucursalRepo = sucursalRepo;
        _permisoRepo = permisoRepo;
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllAsync()
    {
        var usuarios = await _usuarioRepo.GetAllActivosAsync();
        return usuarios.Select(MapToDto);
    }

    public async Task<UsuarioDetalleDto?> GetByIdAsync(int idUsuario)
    {
        var usuario = await _usuarioRepo.GetWithSucursalesAsync(idUsuario);
        if (usuario is null) return null;

        // Cargar rol
        var conRol = await _usuarioRepo.GetWithRolAsync(idUsuario);

        return new UsuarioDetalleDto
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Activo = usuario.Activo,
            IdRol = usuario.IdRol,
            NombreRol = conRol?.Rol?.Nombre,
            Sucursales = usuario.UsuarioSucursales.Select(us => new SucursalDto
            {
                IdSucursal = us.Sucursal.IdSucursal,
                Nombre = us.Sucursal.Nombre,
                Direccion = us.Sucursal.Direccion,
                Ciudad = us.Sucursal.Ciudad,
                Estado = us.Sucursal.Estado,
                Activa = us.Sucursal.Activa
            }).ToList()
        };
    }

    public async Task<UsuarioDto> CreateAsync(CreateUsuarioDto dto)
    {
        // Verificar que el correo no exista
        if (await _usuarioRepo.ExisteCorreoAsync(dto.Correo))
            throw new InvalidOperationException("El correo ya está registrado.");

        // Verificar que el rol exista
        var rol = await _rolRepo.GetByIdAsync(dto.IdRol);
        if (rol is null)
            throw new InvalidOperationException("El rol especificado no existe.");

        // Hash de contraseña (RNF1.1)
        using var hmac = new HMACSHA512();
        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Correo = dto.Correo,
            ContraseniaHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Contrasenia)),
            ContraseniaSalt = hmac.Key,
            Activo = true,
            IdRol = dto.IdRol
        };

        await _usuarioRepo.AddAsync(usuario);
        await _usuarioRepo.SaveChangesAsync();

        return new UsuarioDto
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Activo = usuario.Activo,
            IdRol = usuario.IdRol,
            NombreRol = rol.Nombre
        };
    }

    public async Task<bool> UpdateAsync(int idUsuario, UpdateUsuarioDto dto)
    {
        var usuario = await _usuarioRepo.GetByIdAsync(idUsuario);
        if (usuario is null) return false;

        // Verificar unicidad del correo si cambió
        if (usuario.Correo != dto.Correo && await _usuarioRepo.ExisteCorreoAsync(dto.Correo))
            throw new InvalidOperationException("El correo ya está registrado por otro usuario.");

        // Verificar que el rol exista
        var rol = await _rolRepo.GetByIdAsync(dto.IdRol);
        if (rol is null)
            throw new InvalidOperationException("El rol especificado no existe.");

        usuario.Nombre = dto.Nombre;
        usuario.Correo = dto.Correo;
        usuario.IdRol = dto.IdRol;

        _usuarioRepo.Update(usuario);
        await _usuarioRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DesactivarAsync(int idUsuario)
    {
        var usuario = await _usuarioRepo.GetByIdAsync(idUsuario);
        if (usuario is null) return false;

        usuario.Activo = false;
        _usuarioRepo.Update(usuario);
        await _usuarioRepo.SaveChangesAsync();
        return true;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
    {
        var usuario = await _usuarioRepo.GetByCorreoAsync(dto.Correo);
        if (usuario is null || !usuario.Activo) return null;

        // Verificar contraseña
        using var hmac = new HMACSHA512(usuario.ContraseniaSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Contrasenia));

        if (!computedHash.SequenceEqual(usuario.ContraseniaHash))
            return null;

        // Obtener permisos del rol
        var permisos = await _permisoRepo.GetByRolWithDetailsAsync(usuario.IdRol);

        return new LoginResponseDto
        {
            IdUsuario = usuario.IdUsuario,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            NombreRol = usuario.Rol?.Nombre ?? string.Empty,
            Permisos = permisos.Select(p => new RolModuloPermisoDto
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

    public async Task<bool> AsignarSucursalAsync(int idUsuario, AsignarSucursalDto dto)
    {
        var usuario = await _usuarioRepo.GetByIdAsync(idUsuario);
        if (usuario is null) return false;

        var sucursal = await _sucursalRepo.GetByIdAsync(dto.IdSucursal);
        if (sucursal is null) return false;

        // Verificar que no exista ya la asignación
        var existente = await _usuarioSucursalRepo.GetByCompositeKeyAsync(idUsuario, dto.IdSucursal);
        if (existente is not null)
            throw new InvalidOperationException("El usuario ya está asignado a esta sucursal.");

        var asignacion = new UsuarioSucursal
        {
            IdUsuario = idUsuario,
            IdSucursal = dto.IdSucursal
        };

        await _usuarioSucursalRepo.AddAsync(asignacion);
        await _usuarioSucursalRepo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoverSucursalAsync(int idUsuario, int idSucursal)
    {
        var asignacion = await _usuarioSucursalRepo.GetByCompositeKeyAsync(idUsuario, idSucursal);
        if (asignacion is null) return false;

        _usuarioSucursalRepo.Remove(asignacion);
        await _usuarioSucursalRepo.SaveChangesAsync();
        return true;
    }

    // ─── Mapeo privado ───

    private static UsuarioDto MapToDto(Usuario u) => new()
    {
        IdUsuario = u.IdUsuario,
        Nombre = u.Nombre,
        Correo = u.Correo,
        Activo = u.Activo,
        IdRol = u.IdRol,
        NombreRol = u.Rol?.Nombre
    };
}
