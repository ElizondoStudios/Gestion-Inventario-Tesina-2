using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    IdModulo = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.IdModulo);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    IdProducto = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    NumeroParte = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.IdProducto);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Sucursales",
                columns: table => new
                {
                    IdSucursal = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Ciudad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursales", x => x.IdSucursal);
                });

            migrationBuilder.CreateTable(
                name: "ModuloCategorias",
                columns: table => new
                {
                    IdModuloCategoria = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdModulo = table.Column<int>(type: "INTEGER", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuloCategorias", x => x.IdModuloCategoria);
                    table.ForeignKey(
                        name: "FK_ModuloCategorias_Modulos_IdModulo",
                        column: x => x.IdModulo,
                        principalTable: "Modulos",
                        principalColumn: "IdModulo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Correo = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ContraseniaHash = table.Column<byte[]>(type: "BLOB", nullable: false),
                    ContraseniaSalt = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    IdRol = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventarios",
                columns: table => new
                {
                    IdInventario = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdProducto = table.Column<int>(type: "INTEGER", nullable: false),
                    IdSucursal = table.Column<int>(type: "INTEGER", nullable: false),
                    StockActual = table.Column<int>(type: "INTEGER", nullable: false),
                    StockMinimo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventarios", x => x.IdInventario);
                    table.ForeignKey(
                        name: "FK_Inventarios_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventarios_Sucursales_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursales",
                        principalColumn: "IdSucursal",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolModuloPermisos",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "INTEGER", nullable: false),
                    IdModuloCategoria = table.Column<int>(type: "INTEGER", nullable: false),
                    PuedeLeer = table.Column<bool>(type: "INTEGER", nullable: false),
                    PuedeEscribir = table.Column<bool>(type: "INTEGER", nullable: false),
                    PuedeEliminar = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolModuloPermisos", x => new { x.IdRol, x.IdModuloCategoria });
                    table.ForeignKey(
                        name: "FK_RolModuloPermisos_ModuloCategorias_IdModuloCategoria",
                        column: x => x.IdModuloCategoria,
                        principalTable: "ModuloCategorias",
                        principalColumn: "IdModuloCategoria",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RolModuloPermisos_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosInventario",
                columns: table => new
                {
                    IdMovimiento = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdProducto = table.Column<int>(type: "INTEGER", nullable: false),
                    IdSucursalOrigen = table.Column<int>(type: "INTEGER", nullable: true),
                    IdSucursalDestino = table.Column<int>(type: "INTEGER", nullable: true),
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosInventario", x => x.IdMovimiento);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Productos_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Productos",
                        principalColumn: "IdProducto",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Sucursales_IdSucursalDestino",
                        column: x => x.IdSucursalDestino,
                        principalTable: "Sucursales",
                        principalColumn: "IdSucursal",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Sucursales_IdSucursalOrigen",
                        column: x => x.IdSucursalOrigen,
                        principalTable: "Sucursales",
                        principalColumn: "IdSucursal",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosInventario_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioSucursales",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false),
                    IdSucursal = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioSucursales", x => new { x.IdUsuario, x.IdSucursal });
                    table.ForeignKey(
                        name: "FK_UsuarioSucursales_Sucursales_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursales",
                        principalColumn: "IdSucursal",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioSucursales_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_IdProducto_IdSucursal",
                table: "Inventarios",
                columns: new[] { "IdProducto", "IdSucursal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_IdSucursal",
                table: "Inventarios",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_ModuloCategorias_IdModulo",
                table: "ModuloCategorias",
                column: "IdModulo");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdProducto",
                table: "MovimientosInventario",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdSucursalDestino",
                table: "MovimientosInventario",
                column: "IdSucursalDestino");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdSucursalOrigen",
                table: "MovimientosInventario",
                column: "IdSucursalOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdUsuario",
                table: "MovimientosInventario",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Productos_NumeroParte",
                table: "Productos",
                column: "NumeroParte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RolModuloPermisos_IdModuloCategoria",
                table: "RolModuloPermisos",
                column: "IdModuloCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSucursales_IdSucursal",
                table: "UsuarioSucursales",
                column: "IdSucursal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventarios");

            migrationBuilder.DropTable(
                name: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "RolModuloPermisos");

            migrationBuilder.DropTable(
                name: "UsuarioSucursales");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "ModuloCategorias");

            migrationBuilder.DropTable(
                name: "Sucursales");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
