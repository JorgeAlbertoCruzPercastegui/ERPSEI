using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class VacacionesAnticipadasSolicitud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DescuentoAnticipadoAplicado",
                table: "SolicitudesVacaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DiasAnticipadosPendientesDescuento",
                table: "SolicitudesVacaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "EsVacacionAnticipada",
                table: "SolicitudesVacaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAplicacionDescuentoAnticipado",
                table: "SolicitudesVacaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 27, 13, 22, 14, 155, DateTimeKind.Local).AddTicks(5133));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 27, 13, 22, 14, 155, DateTimeKind.Local).AddTicks(5151));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 27, 13, 22, 14, 155, DateTimeKind.Local).AddTicks(5153));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 27, 13, 22, 14, 155, DateTimeKind.Local).AddTicks(5155));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 27, 13, 22, 14, 155, DateTimeKind.Local).AddTicks(5156));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 27, 13, 22, 14, 155, DateTimeKind.Local).AddTicks(5158));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 27, 13, 22, 14, 155, DateTimeKind.Local).AddTicks(5159));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 27, 13, 22, 14, 155, DateTimeKind.Local).AddTicks(5161));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 27, 13, 22, 14, 155, DateTimeKind.Local).AddTicks(5162));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescuentoAnticipadoAplicado",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "DiasAnticipadosPendientesDescuento",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "EsVacacionAnticipada",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "FechaAplicacionDescuentoAnticipado",
                table: "SolicitudesVacaciones");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8541));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8554));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8554));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8555));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8556));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8557));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8558));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8559));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8560));
        }
    }
}
