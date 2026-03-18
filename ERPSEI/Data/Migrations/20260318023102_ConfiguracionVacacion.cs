using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfiguracionVacacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoVisualizacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesVacaciones", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ConfiguracionesVacaciones",
                columns: new[] { "Id", "FechaActualizacion", "TipoVisualizacion" },
                values: new object[] { 1, new DateTime(2026, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "LegalesProporcionales" });

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 20, 30, 58, 319, DateTimeKind.Local).AddTicks(6760));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 20, 30, 58, 319, DateTimeKind.Local).AddTicks(6779));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 20, 30, 58, 319, DateTimeKind.Local).AddTicks(6781));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 20, 30, 58, 319, DateTimeKind.Local).AddTicks(6784));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 20, 30, 58, 319, DateTimeKind.Local).AddTicks(6786));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 20, 30, 58, 319, DateTimeKind.Local).AddTicks(6788));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 20, 30, 58, 319, DateTimeKind.Local).AddTicks(6791));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 20, 30, 58, 319, DateTimeKind.Local).AddTicks(6793));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 20, 30, 58, 319, DateTimeKind.Local).AddTicks(6795));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesVacaciones");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 14, 13, 10, 811, DateTimeKind.Local).AddTicks(8407));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 14, 13, 10, 811, DateTimeKind.Local).AddTicks(8417));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 14, 13, 10, 811, DateTimeKind.Local).AddTicks(8418));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 14, 13, 10, 811, DateTimeKind.Local).AddTicks(8419));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 14, 13, 10, 811, DateTimeKind.Local).AddTicks(8421));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 14, 13, 10, 811, DateTimeKind.Local).AddTicks(8422));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 14, 13, 10, 811, DateTimeKind.Local).AddTicks(8423));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 14, 13, 10, 811, DateTimeKind.Local).AddTicks(8424));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 17, 14, 13, 10, 811, DateTimeKind.Local).AddTicks(8425));
        }
    }
}
