using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloBanners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Id", "Categoria", "Deshabilitado", "Nombre", "NombreNormalizado" },
                values: new object[] { 25, "erp", 0, "Banners", "banners" });

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8264));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8278));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8279));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8280));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8281));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8282));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8284));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8285));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8286));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Modulos",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3481));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3497));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3499));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3500));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3501));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3503));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3504));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3505));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3507));
        }
    }
}
