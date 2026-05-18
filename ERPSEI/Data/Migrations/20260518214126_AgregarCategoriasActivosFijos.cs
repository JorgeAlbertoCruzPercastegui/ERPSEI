using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCategoriasActivosFijos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CategoriasActivosFijos",
                columns: new[] { "Id", "Descripcion", "Deshabilitado" },
                values: new object[,]
                {
                    { 4, "Archiveros", 0 },
                    { 5, "Escritorios y mesas", 0 },
                    { 6, "Estaciones", 0 },
                    { 7, "Extintores", 0 },
                    { 8, "Línea blanca", 0 },
                    { 9, "Pingüinos y ventiladores", 0 },
                    { 10, "Sillas", 0 },
                    { 11, "Decoración", 0 },
                    { 12, "Otros ", 0 }
                });

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 15, 41, 24, 18, DateTimeKind.Local).AddTicks(8081));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 15, 41, 24, 18, DateTimeKind.Local).AddTicks(8093));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 15, 41, 24, 18, DateTimeKind.Local).AddTicks(8094));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 15, 41, 24, 18, DateTimeKind.Local).AddTicks(8095));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 15, 41, 24, 18, DateTimeKind.Local).AddTicks(8096));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 15, 41, 24, 18, DateTimeKind.Local).AddTicks(8097));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 15, 41, 24, 18, DateTimeKind.Local).AddTicks(8098));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 15, 41, 24, 18, DateTimeKind.Local).AddTicks(8098));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 15, 41, 24, 18, DateTimeKind.Local).AddTicks(8099));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CategoriasActivosFijos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CategoriasActivosFijos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CategoriasActivosFijos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CategoriasActivosFijos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CategoriasActivosFijos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CategoriasActivosFijos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CategoriasActivosFijos",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CategoriasActivosFijos",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CategoriasActivosFijos",
                keyColumn: "Id",
                keyValue: 12);

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
    }
}
