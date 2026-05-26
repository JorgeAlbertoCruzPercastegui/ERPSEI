using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSaldoVacacionesImportadoEmpleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaImportacionSaldoVacaciones",
                table: "Empleados",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoVacacionesImportado",
                table: "Empleados",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 26, 15, 42, 24, 909, DateTimeKind.Local).AddTicks(8242));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 26, 15, 42, 24, 909, DateTimeKind.Local).AddTicks(8251));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 26, 15, 42, 24, 909, DateTimeKind.Local).AddTicks(8252));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 26, 15, 42, 24, 909, DateTimeKind.Local).AddTicks(8253));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 26, 15, 42, 24, 909, DateTimeKind.Local).AddTicks(8253));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 26, 15, 42, 24, 909, DateTimeKind.Local).AddTicks(8254));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 26, 15, 42, 24, 909, DateTimeKind.Local).AddTicks(8255));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 26, 15, 42, 24, 909, DateTimeKind.Local).AddTicks(8256));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 26, 15, 42, 24, 909, DateTimeKind.Local).AddTicks(8256));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaImportacionSaldoVacaciones",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "SaldoVacacionesImportado",
                table: "Empleados");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6336));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6346));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6347));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6348));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6348));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6354));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6355));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6356));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6356));
        }
    }
}
