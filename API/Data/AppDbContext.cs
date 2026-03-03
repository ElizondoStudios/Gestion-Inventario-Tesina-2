using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<ModuloCategoria> ModuloCategorias => Set<ModuloCategoria>();
    public DbSet<RolModuloPermiso> RolModuloPermisos => Set<RolModuloPermiso>();
    public DbSet<Sucursal> Sucursales => Set<Sucursal>();
    public DbSet<UsuarioSucursal> UsuarioSucursales => Set<UsuarioSucursal>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Inventario> Inventarios => Set<Inventario>();
    public DbSet<MovimientoInventario> MovimientosInventario => Set<MovimientoInventario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── Rol ───
        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(255);
        });

        // ─── Usuario ───
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Correo).IsRequired().HasMaxLength(150);
            entity.Property(e => e.ContraseniaHash).IsRequired();
            entity.Property(e => e.ContraseniaSalt).IsRequired();
            entity.Property(e => e.Activo).IsRequired();

            entity.HasIndex(e => e.Correo).IsUnique();

            entity.HasOne(e => e.Rol)
                  .WithMany(r => r.Usuarios)
                  .HasForeignKey(e => e.IdRol)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── Modulo ───
        modelBuilder.Entity<Modulo>(entity =>
        {
            entity.HasKey(e => e.IdModulo);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Activo).IsRequired();
        });

        // ─── ModuloCategoria ───
        modelBuilder.Entity<ModuloCategoria>(entity =>
        {
            entity.HasKey(e => e.IdModuloCategoria);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(255);

            entity.HasOne(e => e.Modulo)
                  .WithMany(m => m.ModuloCategorias)
                  .HasForeignKey(e => e.IdModulo)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── RolModuloPermiso (clave compuesta) ───
        modelBuilder.Entity<RolModuloPermiso>(entity =>
        {
            entity.HasKey(e => new { e.IdRol, e.IdModuloCategoria });

            entity.Property(e => e.PuedeLeer).IsRequired();
            entity.Property(e => e.PuedeEscribir).IsRequired();
            entity.Property(e => e.PuedeEliminar).IsRequired();

            entity.HasOne(e => e.Rol)
                  .WithMany(r => r.RolModuloPermisos)
                  .HasForeignKey(e => e.IdRol)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ModuloCategoria)
                  .WithMany(mc => mc.RolModuloPermisos)
                  .HasForeignKey(e => e.IdModuloCategoria)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── Sucursal ───
        modelBuilder.Entity<Sucursal>(entity =>
        {
            entity.HasKey(e => e.IdSucursal);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Direccion).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Ciudad).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Activa).IsRequired();
        });

        // ─── UsuarioSucursal (clave compuesta) ───
        modelBuilder.Entity<UsuarioSucursal>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.IdSucursal });

            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.UsuarioSucursales)
                  .HasForeignKey(e => e.IdUsuario)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Sucursal)
                  .WithMany(s => s.UsuarioSucursales)
                  .HasForeignKey(e => e.IdSucursal)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── Producto ───
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.NumeroParte).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Activo).IsRequired();

            entity.HasIndex(e => e.NumeroParte).IsUnique();
        });

        // ─── Inventario ───
        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.IdInventario);
            entity.Property(e => e.StockActual).IsRequired();
            entity.Property(e => e.StockMinimo).IsRequired();

            entity.HasIndex(e => new { e.IdProducto, e.IdSucursal }).IsUnique();

            entity.HasOne(e => e.Producto)
                  .WithMany(p => p.Inventarios)
                  .HasForeignKey(e => e.IdProducto)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Sucursal)
                  .WithMany(s => s.Inventarios)
                  .HasForeignKey(e => e.IdSucursal)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ─── MovimientoInventario ───
        modelBuilder.Entity<MovimientoInventario>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento);
            entity.Property(e => e.TipoMovimiento).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Cantidad).IsRequired();
            entity.Property(e => e.Fecha).IsRequired();
            entity.Property(e => e.Observaciones).HasMaxLength(255);

            entity.HasOne(e => e.Producto)
                  .WithMany(p => p.MovimientosInventario)
                  .HasForeignKey(e => e.IdProducto)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.MovimientosInventario)
                  .HasForeignKey(e => e.IdUsuario)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SucursalOrigen)
                  .WithMany(s => s.MovimientosOrigen)
                  .HasForeignKey(e => e.IdSucursalOrigen)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SucursalDestino)
                  .WithMany(s => s.MovimientosDestino)
                  .HasForeignKey(e => e.IdSucursalDestino)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
