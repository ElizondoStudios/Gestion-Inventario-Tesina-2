using System.Security.Cryptography;
using System.Text;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        // Si ya hay roles, asumimos que la base ya está poblada.
        if (await context.Roles.AnyAsync()) return;

        // ─── Roles ───
        var rolAdmin = new Rol
        {
            Nombre = "Administrador",
            Descripcion = "Acceso completo al sistema"
        };
        var rolEncargado = new Rol
        {
            Nombre = "Encargado",
            Descripcion = "Gestión de inventario y movimientos"
        };
        var rolCajero = new Rol
        {
            Nombre = "Cajero",
            Descripcion = "Registro de ventas"
        };

        await context.Roles.AddRangeAsync(rolAdmin, rolEncargado, rolCajero);
        await context.SaveChangesAsync();

        // ─── Módulos y categorías ───
        var moduloDashboard = new Modulo
        {
            Nombre = "Dashboard",
            Descripcion = "Resumen general de indicadores",
            Activo = true
        };
        var moduloInventario = new Modulo
        {
            Nombre = "Inventario",
            Descripcion = "Gestión de productos, inventarios y movimientos",
            Activo = true
        };
        var moduloSeguridad = new Modulo
        {
            Nombre = "Seguridad",
            Descripcion = "Gestión de usuarios, roles y permisos",
            Activo = true
        };

        await context.Modulos.AddRangeAsync(moduloDashboard, moduloInventario, moduloSeguridad);
        await context.SaveChangesAsync();

        var catDashboard = new ModuloCategoria
        {
            IdModulo = moduloDashboard.IdModulo,
            Nombre = "Vista General",
            Descripcion = "Tarjetas y reportes"
        };
        var catProductos = new ModuloCategoria
        {
            IdModulo = moduloInventario.IdModulo,
            Nombre = "Productos",
            Descripcion = "ABM de productos"
        };
        var catInventarios = new ModuloCategoria
        {
            IdModulo = moduloInventario.IdModulo,
            Nombre = "Inventarios",
            Descripcion = "Control de stock por sucursal"
        };
        var catMovimientos = new ModuloCategoria
        {
            IdModulo = moduloInventario.IdModulo,
            Nombre = "Movimientos",
            Descripcion = "Entradas, salidas, mermas y transferencias"
        };
        var catUsuarios = new ModuloCategoria
        {
            IdModulo = moduloSeguridad.IdModulo,
            Nombre = "Usuarios",
            Descripcion = "Gestión de usuarios"
        };
        var catRoles = new ModuloCategoria
        {
            IdModulo = moduloSeguridad.IdModulo,
            Nombre = "Roles y Permisos",
            Descripcion = "Gestión de perfiles y permisos"
        };

        await context.ModuloCategorias.AddRangeAsync(
            catDashboard,
            catProductos,
            catInventarios,
            catMovimientos,
            catUsuarios,
            catRoles);
        await context.SaveChangesAsync();

        // ─── Permisos ───
        var categorias = new[] { catDashboard, catProductos, catInventarios, catMovimientos, catUsuarios, catRoles };

        var permisosAdmin = categorias.Select(c => new RolModuloPermiso
        {
            IdRol = rolAdmin.IdRol,
            IdModuloCategoria = c.IdModuloCategoria,
            PuedeLeer = true,
            PuedeEscribir = true,
            PuedeEliminar = true
        });

        var permisosEncargado = new[]
        {
            new RolModuloPermiso
            {
                IdRol = rolEncargado.IdRol,
                IdModuloCategoria = catDashboard.IdModuloCategoria,
                PuedeLeer = true,
                PuedeEscribir = false,
                PuedeEliminar = false
            },
            new RolModuloPermiso
            {
                IdRol = rolEncargado.IdRol,
                IdModuloCategoria = catProductos.IdModuloCategoria,
                PuedeLeer = true,
                PuedeEscribir = true,
                PuedeEliminar = false
            },
            new RolModuloPermiso
            {
                IdRol = rolEncargado.IdRol,
                IdModuloCategoria = catInventarios.IdModuloCategoria,
                PuedeLeer = true,
                PuedeEscribir = true,
                PuedeEliminar = false
            },
            new RolModuloPermiso
            {
                IdRol = rolEncargado.IdRol,
                IdModuloCategoria = catMovimientos.IdModuloCategoria,
                PuedeLeer = true,
                PuedeEscribir = true,
                PuedeEliminar = false
            }
        };

        var permisosCajero = new[]
        {
            new RolModuloPermiso
            {
                IdRol = rolCajero.IdRol,
                IdModuloCategoria = catDashboard.IdModuloCategoria,
                PuedeLeer = true,
                PuedeEscribir = false,
                PuedeEliminar = false
            },
            new RolModuloPermiso
            {
                IdRol = rolCajero.IdRol,
                IdModuloCategoria = catMovimientos.IdModuloCategoria,
                PuedeLeer = true,
                PuedeEscribir = true,
                PuedeEliminar = false
            }
        };

        await context.RolModuloPermisos.AddRangeAsync(permisosAdmin);
        await context.RolModuloPermisos.AddRangeAsync(permisosEncargado);
        await context.RolModuloPermisos.AddRangeAsync(permisosCajero);
        await context.SaveChangesAsync();

        // ─── Sucursales ───
        var sucursalCentral = new Sucursal
        {
            Nombre = "Casa Central",
            Direccion = "Av. Principal 100",
            Ciudad = "Formosa",
            Estado = "Formosa",
            Activa = true
        };
        var sucursalNorte = new Sucursal
        {
            Nombre = "Sucursal Norte",
            Direccion = "Calle Norte 250",
            Ciudad = "Formosa",
            Estado = "Formosa",
            Activa = true
        };
        var sucursalSur = new Sucursal
        {
            Nombre = "Sucursal Sur",
            Direccion = "Ruta Sur Km 5",
            Ciudad = "Formosa",
            Estado = "Formosa",
            Activa = true
        };

        await context.Sucursales.AddRangeAsync(sucursalCentral, sucursalNorte, sucursalSur);
        await context.SaveChangesAsync();

        // ─── Productos ───
        var productos = new[]
        {
            new Producto { Nombre = "Notebook Lenovo V15", Descripcion = "Core i5 / 16GB / 512GB SSD", NumeroParte = "NB-LNV-V15", Precio = 950000m, Activo = true },
            new Producto { Nombre = "Mouse Logitech M90", Descripcion = "Mouse óptico USB", NumeroParte = "MS-LOG-M90", Precio = 12000m, Activo = true },
            new Producto { Nombre = "Teclado Redragon K552", Descripcion = "Teclado mecánico", NumeroParte = "KB-RDG-K552", Precio = 68000m, Activo = true },
            new Producto { Nombre = "Monitor Samsung 24", Descripcion = "Monitor Full HD 75Hz", NumeroParte = "MN-SMS-24FHD", Precio = 210000m, Activo = true },
            new Producto { Nombre = "Cable HDMI 2m", Descripcion = "Cable HDMI alta velocidad", NumeroParte = "CB-HDMI-2M", Precio = 8000m, Activo = true },
            new Producto { Nombre = "Disco SSD 1TB", Descripcion = "SSD NVMe Gen4", NumeroParte = "SSD-1TB-NVME", Precio = 130000m, Activo = true }
        };

        await context.Productos.AddRangeAsync(productos);
        await context.SaveChangesAsync();

        // ─── Usuarios (password: 123456) ───
        var usuarioAdmin = CrearUsuario("Admin General", "admin@inventario.local", rolAdmin.IdRol);
        var usuarioEncargado = CrearUsuario("Encargado Central", "encargado@inventario.local", rolEncargado.IdRol);
        var usuarioCajero = CrearUsuario("Cajero Norte", "cajero@inventario.local", rolCajero.IdRol);

        await context.Usuarios.AddRangeAsync(usuarioAdmin, usuarioEncargado, usuarioCajero);
        await context.SaveChangesAsync();

        // ─── Usuario-Sucursal ───
        await context.UsuarioSucursales.AddRangeAsync(
            new UsuarioSucursal { IdUsuario = usuarioAdmin.IdUsuario, IdSucursal = sucursalCentral.IdSucursal },
            new UsuarioSucursal { IdUsuario = usuarioAdmin.IdUsuario, IdSucursal = sucursalNorte.IdSucursal },
            new UsuarioSucursal { IdUsuario = usuarioAdmin.IdUsuario, IdSucursal = sucursalSur.IdSucursal },
            new UsuarioSucursal { IdUsuario = usuarioEncargado.IdUsuario, IdSucursal = sucursalCentral.IdSucursal },
            new UsuarioSucursal { IdUsuario = usuarioCajero.IdUsuario, IdSucursal = sucursalNorte.IdSucursal });
        await context.SaveChangesAsync();

        // ─── Tipos de movimiento ───
        var tipoCompra = new TipoMovimiento
        {
            Nombre = "Compra",
            Descripcion = "Ingreso por compra",
            AfectaStock = true,
            EsTransferencia = false
        };
        var tipoVenta = new TipoMovimiento
        {
            Nombre = "Venta",
            Descripcion = "Salida por venta",
            AfectaStock = true,
            EsTransferencia = false
        };
        var tipoMerma = new TipoMovimiento
        {
            Nombre = "Merma",
            Descripcion = "Pérdida, rotura o vencimiento",
            AfectaStock = true,
            EsTransferencia = false
        };
        var tipoTransferencia = new TipoMovimiento
        {
            Nombre = "Transferencia",
            Descripcion = "Movimiento entre sucursales",
            AfectaStock = true,
            EsTransferencia = true
        };

        await context.TiposMovimiento.AddRangeAsync(tipoCompra, tipoVenta, tipoMerma, tipoTransferencia);
        await context.SaveChangesAsync();

        // ─── Inventario inicial ───
        var notebook = productos[0];
        var mouse = productos[1];
        var teclado = productos[2];
        var monitor = productos[3];
        var cable = productos[4];
        var ssd = productos[5];

        await context.Inventarios.AddRangeAsync(
            new Inventario { IdProducto = notebook.IdProducto, IdSucursal = sucursalCentral.IdSucursal, StockActual = 18, StockMinimo = 5 },
            new Inventario { IdProducto = mouse.IdProducto, IdSucursal = sucursalCentral.IdSucursal, StockActual = 55, StockMinimo = 20 },
            new Inventario { IdProducto = teclado.IdProducto, IdSucursal = sucursalCentral.IdSucursal, StockActual = 25, StockMinimo = 10 },
            new Inventario { IdProducto = monitor.IdProducto, IdSucursal = sucursalNorte.IdSucursal, StockActual = 9, StockMinimo = 4 },
            new Inventario { IdProducto = cable.IdProducto, IdSucursal = sucursalNorte.IdSucursal, StockActual = 10, StockMinimo = 15 }, // stock bajo para alertas
            new Inventario { IdProducto = ssd.IdProducto, IdSucursal = sucursalSur.IdSucursal, StockActual = 12, StockMinimo = 6 });
        await context.SaveChangesAsync();

        // ─── Movimientos de ejemplo (con fechas variadas para dashboard) ───
        var ahora = DateTime.Now;
        var movimientos = new[]
        {
            new MovimientoInventario
            {
                IdProducto = notebook.IdProducto,
                IdSucursalOrigen = null,
                IdSucursalDestino = sucursalCentral.IdSucursal,
                IdUsuario = usuarioEncargado.IdUsuario,
                IdTipoMovimiento = tipoCompra.IdTipoMovimiento,
                Cantidad = 10,
                Fecha = ahora.AddDays(-5),
                Observaciones = "Ingreso por compra a proveedor"
            },
            new MovimientoInventario
            {
                IdProducto = notebook.IdProducto,
                IdSucursalOrigen = sucursalCentral.IdSucursal,
                IdSucursalDestino = null,
                IdUsuario = usuarioCajero.IdUsuario,
                IdTipoMovimiento = tipoVenta.IdTipoMovimiento,
                Cantidad = 3,
                Fecha = ahora.AddDays(-3),
                Observaciones = "Venta mostrador"
            },
            new MovimientoInventario
            {
                IdProducto = cable.IdProducto,
                IdSucursalOrigen = sucursalNorte.IdSucursal,
                IdSucursalDestino = null,
                IdUsuario = usuarioEncargado.IdUsuario,
                IdTipoMovimiento = tipoMerma.IdTipoMovimiento,
                Cantidad = 2,
                Fecha = ahora.AddDays(-2),
                Observaciones = "Cables dañados"
            },
            new MovimientoInventario
            {
                IdProducto = mouse.IdProducto,
                IdSucursalOrigen = sucursalCentral.IdSucursal,
                IdSucursalDestino = sucursalNorte.IdSucursal,
                IdUsuario = usuarioAdmin.IdUsuario,
                IdTipoMovimiento = tipoTransferencia.IdTipoMovimiento,
                Cantidad = 8,
                Fecha = ahora.AddDays(-1),
                Observaciones = "Reposición de stock sucursal norte"
            },
            new MovimientoInventario
            {
                IdProducto = monitor.IdProducto,
                IdSucursalOrigen = null,
                IdSucursalDestino = sucursalNorte.IdSucursal,
                IdUsuario = usuarioEncargado.IdUsuario,
                IdTipoMovimiento = tipoCompra.IdTipoMovimiento,
                Cantidad = 5,
                Fecha = ahora.AddMonths(-1).AddDays(2),
                Observaciones = "Compra mes anterior"
            },
            new MovimientoInventario
            {
                IdProducto = ssd.IdProducto,
                IdSucursalOrigen = sucursalSur.IdSucursal,
                IdSucursalDestino = null,
                IdUsuario = usuarioCajero.IdUsuario,
                IdTipoMovimiento = tipoVenta.IdTipoMovimiento,
                Cantidad = 2,
                Fecha = ahora.AddMonths(-1).AddDays(5),
                Observaciones = "Venta mes anterior"
            }
        };

        await context.MovimientosInventario.AddRangeAsync(movimientos);
        await context.SaveChangesAsync();

        // ─── Logs de movimientos ───
        var logs = movimientos.Select(m => new MovimientoLog
        {
            IdMovimiento = m.IdMovimiento,
            IdUsuario = m.IdUsuario,
            Accion = "Creación",
            Fecha = m.Fecha,
            ValorAnterior = null,
            ValorNuevo = $"Producto:{m.IdProducto}, Cantidad:{m.Cantidad}, TipoMovimiento:{m.IdTipoMovimiento}"
        });

        await context.MovimientoLogs.AddRangeAsync(logs);
        await context.SaveChangesAsync();
    }

    private static Usuario CrearUsuario(string nombre, string correo, int idRol)
    {
        CrearHashPassword("123456", out var hash, out var salt);
        return new Usuario
        {
            Nombre = nombre,
            Correo = correo,
            ContraseniaHash = hash,
            ContraseniaSalt = salt,
            Activo = true,
            IdRol = idRol
        };
    }

    private static void CrearHashPassword(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
}