using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPublicacionPorAreasManuales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublicacionGeneral",
                table: "ManualesPoliticasIntranet",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ManualPoliticaAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ManualPoliticaIntranetId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualPoliticaAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualPoliticaAreas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ManualPoliticaAreas_ManualesPoliticasIntranet_ManualPoliticaIntranetId",
                        column: x => x.ManualPoliticaIntranetId,
                        principalTable: "ManualesPoliticasIntranet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 5, 17, 6, 53, 287, DateTimeKind.Local).AddTicks(6551));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 5, 17, 6, 53, 287, DateTimeKind.Local).AddTicks(6567));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 5, 17, 6, 53, 287, DateTimeKind.Local).AddTicks(6568));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 5, 17, 6, 53, 287, DateTimeKind.Local).AddTicks(6569));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 5, 17, 6, 53, 287, DateTimeKind.Local).AddTicks(6570));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 5, 17, 6, 53, 287, DateTimeKind.Local).AddTicks(6571));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 5, 17, 6, 53, 287, DateTimeKind.Local).AddTicks(6572));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 5, 17, 6, 53, 287, DateTimeKind.Local).AddTicks(6573));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 5, 17, 6, 53, 287, DateTimeKind.Local).AddTicks(6574));

            migrationBuilder.CreateIndex(
                name: "IX_ManualPoliticaAreas_AreaId",
                table: "ManualPoliticaAreas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualPoliticaAreas_ManualPoliticaIntranetId",
                table: "ManualPoliticaAreas",
                column: "ManualPoliticaIntranetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManualPoliticaAreas");

            migrationBuilder.DropColumn(
                name: "PublicacionGeneral",
                table: "ManualesPoliticasIntranet");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 29, 17, 10, 41, 492, DateTimeKind.Local).AddTicks(1716));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 29, 17, 10, 41, 492, DateTimeKind.Local).AddTicks(1729));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 29, 17, 10, 41, 492, DateTimeKind.Local).AddTicks(1730));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 29, 17, 10, 41, 492, DateTimeKind.Local).AddTicks(1731));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 29, 17, 10, 41, 492, DateTimeKind.Local).AddTicks(1733));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 29, 17, 10, 41, 492, DateTimeKind.Local).AddTicks(1734));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 29, 17, 10, 41, 492, DateTimeKind.Local).AddTicks(1735));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 29, 17, 10, 41, 492, DateTimeKind.Local).AddTicks(1736));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 29, 17, 10, 41, 492, DateTimeKind.Local).AddTicks(1737));
        }
    }
}
