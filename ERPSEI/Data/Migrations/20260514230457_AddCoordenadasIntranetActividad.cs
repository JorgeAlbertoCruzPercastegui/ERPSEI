using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCoordenadasIntranetActividad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitud",
                table: "IntranetActividades",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitud",
                table: "IntranetActividades",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 14, 17, 4, 51, 964, DateTimeKind.Local).AddTicks(7159));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 14, 17, 4, 51, 964, DateTimeKind.Local).AddTicks(7186));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 14, 17, 4, 51, 964, DateTimeKind.Local).AddTicks(7188));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 14, 17, 4, 51, 964, DateTimeKind.Local).AddTicks(7189));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 14, 17, 4, 51, 964, DateTimeKind.Local).AddTicks(7191));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 14, 17, 4, 51, 964, DateTimeKind.Local).AddTicks(7192));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 14, 17, 4, 51, 964, DateTimeKind.Local).AddTicks(7194));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 14, 17, 4, 51, 964, DateTimeKind.Local).AddTicks(7195));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 14, 17, 4, 51, 964, DateTimeKind.Local).AddTicks(7196));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitud",
                table: "IntranetActividades");

            migrationBuilder.DropColumn(
                name: "Longitud",
                table: "IntranetActividades");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 13, 19, 11, 53, 0, DateTimeKind.Local).AddTicks(8184));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 13, 19, 11, 53, 0, DateTimeKind.Local).AddTicks(8195));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 13, 19, 11, 53, 0, DateTimeKind.Local).AddTicks(8196));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 13, 19, 11, 53, 0, DateTimeKind.Local).AddTicks(8197));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 13, 19, 11, 53, 0, DateTimeKind.Local).AddTicks(8198));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 13, 19, 11, 53, 0, DateTimeKind.Local).AddTicks(8199));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 13, 19, 11, 53, 0, DateTimeKind.Local).AddTicks(8200));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 13, 19, 11, 53, 0, DateTimeKind.Local).AddTicks(8200));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 13, 19, 11, 53, 0, DateTimeKind.Local).AddTicks(8201));
        }
    }
}
