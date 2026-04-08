using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacionEnviadaToComunicados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNotificacion",
                table: "EventosIntranet",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificacionEnviada",
                table: "EventosIntranet",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNotificacion",
                table: "ComunicadosInternos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificacionEnviada",
                table: "ComunicadosInternos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 15, 56, 18, 559, DateTimeKind.Local).AddTicks(5166));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 15, 56, 18, 559, DateTimeKind.Local).AddTicks(5172));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 15, 56, 18, 559, DateTimeKind.Local).AddTicks(5174));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 15, 56, 18, 559, DateTimeKind.Local).AddTicks(5175));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 15, 56, 18, 559, DateTimeKind.Local).AddTicks(5176));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 15, 56, 18, 559, DateTimeKind.Local).AddTicks(5177));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 15, 56, 18, 559, DateTimeKind.Local).AddTicks(5178));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 15, 56, 18, 559, DateTimeKind.Local).AddTicks(5179));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 8, 15, 56, 18, 559, DateTimeKind.Local).AddTicks(5180));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaNotificacion",
                table: "EventosIntranet");

            migrationBuilder.DropColumn(
                name: "NotificacionEnviada",
                table: "EventosIntranet");

            migrationBuilder.DropColumn(
                name: "FechaNotificacion",
                table: "ComunicadosInternos");

            migrationBuilder.DropColumn(
                name: "NotificacionEnviada",
                table: "ComunicadosInternos");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 7, 15, 49, 32, 953, DateTimeKind.Local).AddTicks(7222));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 7, 15, 49, 32, 953, DateTimeKind.Local).AddTicks(7235));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 7, 15, 49, 32, 953, DateTimeKind.Local).AddTicks(7236));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 7, 15, 49, 32, 953, DateTimeKind.Local).AddTicks(7237));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 7, 15, 49, 32, 953, DateTimeKind.Local).AddTicks(7238));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 7, 15, 49, 32, 953, DateTimeKind.Local).AddTicks(7239));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 7, 15, 49, 32, 953, DateTimeKind.Local).AddTicks(7240));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 7, 15, 49, 32, 953, DateTimeKind.Local).AddTicks(7240));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 7, 15, 49, 32, 953, DateTimeKind.Local).AddTicks(7241));
        }
    }
}
