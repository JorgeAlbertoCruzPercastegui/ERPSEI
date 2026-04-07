using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicadoYHoraComunicados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComunicadosInternos_Activo_FechaPublicacion",
                table: "ComunicadosInternos");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "HoraPublicacion",
                table: "ComunicadosInternos",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Publicado",
                table: "ComunicadosInternos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 17, 21, 23, 136, DateTimeKind.Local).AddTicks(166));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 17, 21, 23, 136, DateTimeKind.Local).AddTicks(180));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 17, 21, 23, 136, DateTimeKind.Local).AddTicks(181));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 17, 21, 23, 136, DateTimeKind.Local).AddTicks(182));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 17, 21, 23, 136, DateTimeKind.Local).AddTicks(183));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 17, 21, 23, 136, DateTimeKind.Local).AddTicks(183));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 17, 21, 23, 136, DateTimeKind.Local).AddTicks(184));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 17, 21, 23, 136, DateTimeKind.Local).AddTicks(185));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 17, 21, 23, 136, DateTimeKind.Local).AddTicks(186));

            migrationBuilder.CreateIndex(
                name: "IX_ComunicadosInternos_Activo_Publicado_FechaPublicacion",
                table: "ComunicadosInternos",
                columns: new[] { "Activo", "Publicado", "FechaPublicacion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ComunicadosInternos_Activo_Publicado_FechaPublicacion",
                table: "ComunicadosInternos");

            migrationBuilder.DropColumn(
                name: "HoraPublicacion",
                table: "ComunicadosInternos");

            migrationBuilder.DropColumn(
                name: "Publicado",
                table: "ComunicadosInternos");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 16, 27, 9, 937, DateTimeKind.Local).AddTicks(1785));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 16, 27, 9, 937, DateTimeKind.Local).AddTicks(1795));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 16, 27, 9, 937, DateTimeKind.Local).AddTicks(1796));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 16, 27, 9, 937, DateTimeKind.Local).AddTicks(1797));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 16, 27, 9, 937, DateTimeKind.Local).AddTicks(1798));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 16, 27, 9, 937, DateTimeKind.Local).AddTicks(1799));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 16, 27, 9, 937, DateTimeKind.Local).AddTicks(1800));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 16, 27, 9, 937, DateTimeKind.Local).AddTicks(1801));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 16, 27, 9, 937, DateTimeKind.Local).AddTicks(1802));

            migrationBuilder.CreateIndex(
                name: "IX_ComunicadosInternos_Activo_FechaPublicacion",
                table: "ComunicadosInternos",
                columns: new[] { "Activo", "FechaPublicacion" });
        }
    }
}
