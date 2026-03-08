using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTipoMovimientoYLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoMovimiento",
                table: "MovimientosInventario");

            migrationBuilder.AddColumn<int>(
                name: "IdTipoMovimiento",
                table: "MovimientosInventario",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MovimientoLogs",
                columns: table => new
                {
                    IdLog = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdMovimiento = table.Column<int>(type: "INTEGER", nullable: false),
                    IdUsuario = table.Column<int>(type: "INTEGER", nullable: false),
                    Accion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValorAnterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorNuevo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoLogs", x => x.IdLog);
                    table.ForeignKey(
                        name: "FK_MovimientoLogs_MovimientosInventario_IdMovimiento",
                        column: x => x.IdMovimiento,
                        principalTable: "MovimientosInventario",
                        principalColumn: "IdMovimiento",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientoLogs_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposMovimiento",
                columns: table => new
                {
                    IdTipoMovimiento = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    AfectaStock = table.Column<bool>(type: "INTEGER", nullable: false),
                    EsTransferencia = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposMovimiento", x => x.IdTipoMovimiento);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosInventario_IdTipoMovimiento",
                table: "MovimientosInventario",
                column: "IdTipoMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoLogs_IdMovimiento",
                table: "MovimientoLogs",
                column: "IdMovimiento");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoLogs_IdUsuario",
                table: "MovimientoLogs",
                column: "IdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_MovimientosInventario_TiposMovimiento_IdTipoMovimiento",
                table: "MovimientosInventario",
                column: "IdTipoMovimiento",
                principalTable: "TiposMovimiento",
                principalColumn: "IdTipoMovimiento",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovimientosInventario_TiposMovimiento_IdTipoMovimiento",
                table: "MovimientosInventario");

            migrationBuilder.DropTable(
                name: "MovimientoLogs");

            migrationBuilder.DropTable(
                name: "TiposMovimiento");

            migrationBuilder.DropIndex(
                name: "IX_MovimientosInventario_IdTipoMovimiento",
                table: "MovimientosInventario");

            migrationBuilder.DropColumn(
                name: "IdTipoMovimiento",
                table: "MovimientosInventario");

            migrationBuilder.AddColumn<string>(
                name: "TipoMovimiento",
                table: "MovimientosInventario",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
