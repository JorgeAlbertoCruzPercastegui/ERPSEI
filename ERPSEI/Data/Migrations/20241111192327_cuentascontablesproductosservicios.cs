using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class cuentascontablesproductosservicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInicioRelLaboral",
                table: "NominasReceptores",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CuentaContableProductosServicios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuentaContableId = table.Column<int>(type: "int", nullable: true),
                    ProductoServicioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaContableProductosServicios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentaContableProductosServicios_CuentasContables_CuentaContableId",
                        column: x => x.CuentaContableId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CuentaContableProductosServicios_ProductosServicios_ProductoServicioId",
                        column: x => x.ProductoServicioId,
                        principalTable: "ProductosServicios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuentaContableProductosServicios_CuentaContableId",
                table: "CuentaContableProductosServicios",
                column: "CuentaContableId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaContableProductosServicios_ProductoServicioId",
                table: "CuentaContableProductosServicios",
                column: "ProductoServicioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuentaContableProductosServicios");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaInicioRelLaboral",
                table: "NominasReceptores",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
