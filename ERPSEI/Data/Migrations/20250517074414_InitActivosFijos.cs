using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitActivosFijos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasActivosFijos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Deshabilitado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasActivosFijos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposActivosFijos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PermiteMultiplesAsignaciones = table.Column<int>(type: "int", nullable: false),
                    Deshabilitado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposActivosFijos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivosFijos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    TipoId = table.Column<int>(type: "int", nullable: false),
                    Folio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroSerie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comentarios = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRenovacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LinkFacturaCompra = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deshabilitado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivosFijos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivosFijos_CategoriasActivosFijos_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriasActivosFijos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivosFijos_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivosFijos_TiposActivosFijos_TipoId",
                        column: x => x.TipoId,
                        principalTable: "TiposActivosFijos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CategoriasActivosFijos",
                columns: new[] { "Id", "Descripcion", "Deshabilitado" },
                values: new object[,]
                {
                    { 1, "Software", 0 },
                    { 2, "Hardware", 0 },
                    { 3, "Inmobiliario", 0 }
                });

            migrationBuilder.InsertData(
                table: "TiposActivosFijos",
                columns: new[] { "Id", "Descripcion", "Deshabilitado", "PermiteMultiplesAsignaciones" },
                values: new object[,]
                {
                    { 1, "Laptop", 0, 0 },
                    { 2, "Monitor", 0, 0 },
                    { 3, "Licencia", 0, 1 },
                    { 4, "Programa", 0, 1 },
                    { 5, "Mesa", 0, 0 },
                    { 6, "Silla", 0, 0 },
                    { 7, "Escritorio", 0, 0 },
                    { 8, "Unidad de Almacenamiento", 0, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivosFijos_CategoriaId",
                table: "ActivosFijos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivosFijos_EmpleadoId",
                table: "ActivosFijos",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivosFijos_TipoId",
                table: "ActivosFijos",
                column: "TipoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivosFijos");

            migrationBuilder.DropTable(
                name: "CategoriasActivosFijos");

            migrationBuilder.DropTable(
                name: "TiposActivosFijos");
        }
    }
}
