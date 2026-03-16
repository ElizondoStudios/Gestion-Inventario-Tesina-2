using API.DTOs;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;

namespace API.Services.Implementations;

/// <summary>
/// Implementación del servicio para el registro de Movimientos de Inventario.
/// Aplica las reglas de negocio: Entrada, Salida y Transferencia (RF4.2).
/// No permite stock negativo (RNF4.1).
/// </summary>
public class MovimientoInventarioService : IMovimientoInventarioService
{
    private readonly IMovimientoInventarioRepository _movimientoRepo;
    private readonly IInventarioRepository _inventarioRepo;
    private readonly IProductoRepository _productoRepo;
    private readonly ISucursalRepository _sucursalRepo;
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IUsuarioSucursalRepository _usuarioSucursalRepo;
    private readonly ITipoMovimientoRepository _tipoMovimientoRepo;
    private readonly IMovimientoLogRepository _movimientoLogRepo;

    public MovimientoInventarioService(
        IMovimientoInventarioRepository movimientoRepo,
        IInventarioRepository inventarioRepo,
        IProductoRepository productoRepo,
        ISucursalRepository sucursalRepo,
        IUsuarioRepository usuarioRepo,
        IUsuarioSucursalRepository usuarioSucursalRepo,
        ITipoMovimientoRepository tipoMovimientoRepo,
        IMovimientoLogRepository movimientoLogRepo)
    {
        _movimientoRepo = movimientoRepo;
        _inventarioRepo = inventarioRepo;
        _productoRepo = productoRepo;
        _sucursalRepo = sucursalRepo;
        _usuarioRepo = usuarioRepo;
        _usuarioSucursalRepo = usuarioSucursalRepo;
        _tipoMovimientoRepo = tipoMovimientoRepo;
        _movimientoLogRepo = movimientoLogRepo;
    }

    public async Task<IEnumerable<MovimientoInventarioDto>> GetAllAsync()
    {
        var movimientos = await _movimientoRepo.GetAllAsync();
        var result = new List<MovimientoInventarioDto>();
        foreach (var m in movimientos)
        {
            var detalle = await _movimientoRepo.GetWithDetailsAsync(m.IdMovimiento);
            if (detalle is not null) result.Add(MapToDto(detalle));
        }
        return result;
    }

    public async Task<MovimientoInventarioDto?> GetByIdAsync(int idMovimiento)
    {
        var movimiento = await _movimientoRepo.GetWithDetailsAsync(idMovimiento);
        return movimiento is null ? null : MapToDto(movimiento);
    }

    public async Task<IEnumerable<MovimientoInventarioDto>> GetByProductoAsync(int idProducto)
    {
        var movimientos = await _movimientoRepo.GetByProductoAsync(idProducto);
        return movimientos.Select(MapToDto);
    }

    public async Task<IEnumerable<MovimientoInventarioDto>> GetBySucursalAsync(int idSucursal)
    {
        var movimientos = await _movimientoRepo.GetBySucursalAsync(idSucursal);
        return movimientos.Select(MapToDto);
    }

    public async Task<IEnumerable<MovimientoInventarioDto>> GetByFechaRangoAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var movimientos = await _movimientoRepo.GetByFechaRangoAsync(fechaInicio, fechaFin);
        return movimientos.Select(MapToDto);
    }

    public async Task<MovimientoInventarioDto> RegistrarMovimientoAsync(CreateMovimientoDto dto)
    {
        // Validar que el producto exista
        var producto = await _productoRepo.GetByIdAsync(dto.IdProducto)
            ?? throw new InvalidOperationException("El producto especificado no existe.");

        // Validar que el usuario exista
        var usuario = await _usuarioRepo.GetByIdAsync(dto.IdUsuario)
            ?? throw new InvalidOperationException("El usuario especificado no existe.");

        // Validar que el tipo de movimiento exista
        var tipoMovimiento = await _tipoMovimientoRepo.GetByIdAsync(dto.IdTipoMovimiento)
            ?? throw new InvalidOperationException("El tipo de movimiento especificado no existe.");

        // Validar que el usuario tenga asignadas las sucursales involucradas
        if (tipoMovimiento.EsTransferencia)
        {
            if (dto.IdSucursalOrigen is not null)
                await ValidarUsuarioAsignadoASucursalAsync(dto.IdUsuario, dto.IdSucursalOrigen.Value, "origen");

            if (dto.IdSucursalDestino is not null)
                await ValidarUsuarioAsignadoASucursalAsync(dto.IdUsuario, dto.IdSucursalDestino.Value, "destino");
        }
        else if (tipoMovimiento.AfectaStock)
        {
            if (dto.IdSucursalOrigen is not null)
                await ValidarUsuarioAsignadoASucursalAsync(dto.IdUsuario, dto.IdSucursalOrigen.Value, "origen");

            if (dto.IdSucursalDestino is not null)
                await ValidarUsuarioAsignadoASucursalAsync(dto.IdUsuario, dto.IdSucursalDestino.Value, "destino");
        }

        // Validar reglas por tipo de movimiento
        if (tipoMovimiento.EsTransferencia)
        {
            await ProcesarTransferenciaAsync(dto);
        }
        else if (tipoMovimiento.AfectaStock && dto.IdSucursalDestino is not null)
        {
            // Entrada: suma stock en destino
            await ProcesarEntradaAsync(dto);
        }
        else if (tipoMovimiento.AfectaStock && dto.IdSucursalOrigen is not null)
        {
            // Salida: resta stock en origen
            await ProcesarSalidaAsync(dto);
        }

        // Crear el registro del movimiento
        var movimiento = new MovimientoInventario
        {
            IdProducto = dto.IdProducto,
            IdSucursalOrigen = dto.IdSucursalOrigen,
            IdSucursalDestino = dto.IdSucursalDestino,
            IdUsuario = dto.IdUsuario,
            IdTipoMovimiento = dto.IdTipoMovimiento,
            Cantidad = dto.Cantidad,
            Fecha = DateTime.Now,
            Observaciones = dto.Observaciones
        };

        await _movimientoRepo.AddAsync(movimiento);
        await _movimientoRepo.SaveChangesAsync();

        // Registrar log de auditoría (Creación)
        var log = new MovimientoLog
        {
            IdMovimiento = movimiento.IdMovimiento,
            IdUsuario = dto.IdUsuario,
            Accion = "Creación",
            Fecha = DateTime.Now,
            ValorAnterior = null,
            ValorNuevo = $"Producto:{dto.IdProducto}, Cantidad:{dto.Cantidad}, TipoMovimiento:{tipoMovimiento.Nombre}"
        };
        await _movimientoLogRepo.AddAsync(log);
        await _movimientoLogRepo.SaveChangesAsync();

        // Retornar con detalles
        var result = await _movimientoRepo.GetWithDetailsAsync(movimiento.IdMovimiento);
        return MapToDto(result!);
    }

    // ─── Reglas de negocio por tipo de movimiento ───

    /// <summary>
    /// Entrada: Suma stock en la sucursal destino. IdSucursalOrigen debe ser NULL.
    /// </summary>
    private async Task ProcesarEntradaAsync(CreateMovimientoDto dto)
    {
        if (dto.IdSucursalOrigen is not null)
            throw new InvalidOperationException("Una entrada no debe tener sucursal de origen.");
        if (dto.IdSucursalDestino is null)
            throw new InvalidOperationException("Una entrada requiere sucursal de destino.");

        var sucursal = await _sucursalRepo.GetByIdAsync(dto.IdSucursalDestino.Value)
            ?? throw new InvalidOperationException("La sucursal de destino no existe.");

        var inventario = await _inventarioRepo.GetByProductoYSucursalAsync(dto.IdProducto, dto.IdSucursalDestino.Value);

        if (inventario is null)
        {
            // Crear registro de inventario si no existe
            inventario = new Inventario
            {
                IdProducto = dto.IdProducto,
                IdSucursal = dto.IdSucursalDestino.Value,
                StockActual = dto.Cantidad,
                StockMinimo = 0
            };
            await _inventarioRepo.AddAsync(inventario);
        }
        else
        {
            inventario.StockActual += dto.Cantidad;
            _inventarioRepo.Update(inventario);
        }

        await _inventarioRepo.SaveChangesAsync();
    }

    /// <summary>
    /// Salida: Resta stock de la sucursal origen. IdSucursalDestino debe ser NULL.
    /// No permite stock negativo (RNF4.1).
    /// </summary>
    private async Task ProcesarSalidaAsync(CreateMovimientoDto dto)
    {
        if (dto.IdSucursalDestino is not null)
            throw new InvalidOperationException("Una salida no debe tener sucursal de destino.");
        if (dto.IdSucursalOrigen is null)
            throw new InvalidOperationException("Una salida requiere sucursal de origen.");

        var sucursal = await _sucursalRepo.GetByIdAsync(dto.IdSucursalOrigen.Value)
            ?? throw new InvalidOperationException("La sucursal de origen no existe.");

        var inventario = await _inventarioRepo.GetByProductoYSucursalAsync(dto.IdProducto, dto.IdSucursalOrigen.Value)
            ?? throw new InvalidOperationException("No existe inventario de este producto en la sucursal de origen.");

        if (inventario.StockActual < dto.Cantidad)
            throw new InvalidOperationException(
                $"Stock insuficiente. Disponible: {inventario.StockActual}, solicitado: {dto.Cantidad}.");

        inventario.StockActual -= dto.Cantidad;
        _inventarioRepo.Update(inventario);
        await _inventarioRepo.SaveChangesAsync();
    }

    /// <summary>
    /// Transferencia: Resta del origen y suma al destino. Ambas sucursales son requeridas.
    /// No permite stock negativo (RNF4.1).
    /// </summary>
    private async Task ProcesarTransferenciaAsync(CreateMovimientoDto dto)
    {
        if (dto.IdSucursalOrigen is null)
            throw new InvalidOperationException("Una transferencia requiere sucursal de origen.");
        if (dto.IdSucursalDestino is null)
            throw new InvalidOperationException("Una transferencia requiere sucursal de destino.");
        if (dto.IdSucursalOrigen == dto.IdSucursalDestino)
            throw new InvalidOperationException("La sucursal de origen y destino no pueden ser la misma.");

        var sucOrigen = await _sucursalRepo.GetByIdAsync(dto.IdSucursalOrigen.Value)
            ?? throw new InvalidOperationException("La sucursal de origen no existe.");
        var sucDestino = await _sucursalRepo.GetByIdAsync(dto.IdSucursalDestino.Value)
            ?? throw new InvalidOperationException("La sucursal de destino no existe.");

        // Restar del origen
        var invOrigen = await _inventarioRepo.GetByProductoYSucursalAsync(dto.IdProducto, dto.IdSucursalOrigen.Value)
            ?? throw new InvalidOperationException("No existe inventario de este producto en la sucursal de origen.");

        if (invOrigen.StockActual < dto.Cantidad)
            throw new InvalidOperationException(
                $"Stock insuficiente en origen. Disponible: {invOrigen.StockActual}, solicitado: {dto.Cantidad}.");

        invOrigen.StockActual -= dto.Cantidad;
        _inventarioRepo.Update(invOrigen);

        // Sumar al destino
        var invDestino = await _inventarioRepo.GetByProductoYSucursalAsync(dto.IdProducto, dto.IdSucursalDestino.Value);
        if (invDestino is null)
        {
            invDestino = new Inventario
            {
                IdProducto = dto.IdProducto,
                IdSucursal = dto.IdSucursalDestino.Value,
                StockActual = dto.Cantidad,
                StockMinimo = 0
            };
            await _inventarioRepo.AddAsync(invDestino);
        }
        else
        {
            invDestino.StockActual += dto.Cantidad;
            _inventarioRepo.Update(invDestino);
        }

        await _inventarioRepo.SaveChangesAsync();
    }

    /// <summary>
    /// Verifica que el usuario esté asignado a la sucursal indicada.
    /// </summary>
    private async Task ValidarUsuarioAsignadoASucursalAsync(int idUsuario, int idSucursal, string tipoSucursal)
    {
        var asignacion = await _usuarioSucursalRepo.GetByCompositeKeyAsync(idUsuario, idSucursal);
        if (asignacion is null)
        {
            throw new InvalidOperationException(
                $"El usuario no tiene asignada la sucursal de {tipoSucursal} (IdSucursal: {idSucursal}).");
        }
    }

    // ─── Mapeo privado ───

    private static MovimientoInventarioDto MapToDto(MovimientoInventario m) => new()
    {
        IdMovimiento = m.IdMovimiento,
        IdProducto = m.IdProducto,
        NombreProducto = m.Producto?.Nombre,
        IdSucursalOrigen = m.IdSucursalOrigen,
        NombreSucursalOrigen = m.SucursalOrigen?.Nombre,
        IdSucursalDestino = m.IdSucursalDestino,
        NombreSucursalDestino = m.SucursalDestino?.Nombre,
        IdUsuario = m.IdUsuario,
        NombreUsuario = m.Usuario?.Nombre,
        IdTipoMovimiento = m.IdTipoMovimiento,
        NombreTipoMovimiento = m.TipoMovimiento?.Nombre,
        Cantidad = m.Cantidad,
        Fecha = m.Fecha,
        Observaciones = m.Observaciones
    };
}
