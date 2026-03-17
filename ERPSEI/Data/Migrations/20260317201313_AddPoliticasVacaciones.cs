using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPoliticasVacaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoliticasVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TipoVacacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliticasVacaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PoliticasVacacionesDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PoliticaVacacionId = table.Column<int>(type: "int", nullable: false),
                    AniosAntiguedad = table.Column<decimal>(type: "decimal(5,1)", nullable: false),
                    DiasVacaciones = table.Column<decimal>(type: "decimal(5,1)", nullable: false),
                    PrimaVacacional = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    DiasAguinaldo = table.Column<decimal>(type: "decimal(5,1)", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliticasVacacionesDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoliticasVacacionesDetalles_PoliticasVacaciones_PoliticaVacacionId",
                        column: x => x.PoliticaVacacionId,
                        principalTable: "PoliticasVacaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PoliticasVacaciones",
                columns: new[] { "Id", "Activo", "Descripcion", "FechaCreacion", "Nombre", "TipoVacacion" },
                values: new object[,]
                {
                    { 1, true, "Política legal vigente 2023", new DateTime(2026, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Legal 2023", "Legales" },
                    { 2, true, "Política anual interna 2023", new DateTime(2026, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Anual 2023", "Anuales" }
                });

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

            migrationBuilder.InsertData(
                table: "PoliticasVacacionesDetalles",
                columns: new[] { "Id", "AniosAntiguedad", "DiasAguinaldo", "DiasVacaciones", "Orden", "PoliticaVacacionId", "PrimaVacacional" },
                values: new object[,]
                {
                    { 1, 1.0m, 15.0m, 12.0m, 1, 1, 0.25m },
                    { 2, 2.0m, 15.0m, 14.0m, 2, 1, 0.25m },
                    { 3, 3.0m, 15.0m, 16.0m, 3, 1, 0.25m },
                    { 4, 4.0m, 15.0m, 18.0m, 4, 1, 0.25m },
                    { 5, 5.0m, 15.0m, 20.0m, 5, 1, 0.25m },
                    { 6, 6.0m, 15.0m, 22.0m, 6, 1, 0.25m },
                    { 7, 11.0m, 15.0m, 24.0m, 7, 1, 0.25m },
                    { 8, 16.0m, 15.0m, 26.0m, 8, 1, 0.25m },
                    { 9, 21.0m, 15.0m, 28.0m, 9, 1, 0.25m },
                    { 10, 26.0m, 15.0m, 30.0m, 10, 1, 0.25m },
                    { 11, 31.0m, 15.0m, 32.0m, 11, 1, 0.25m },
                    { 12, 36.0m, 15.0m, 34.0m, 12, 1, 0.25m },
                    { 13, 1.0m, 15.0m, 12.0m, 1, 2, 0.25m },
                    { 14, 2.0m, 15.0m, 12.0m, 2, 2, 0.25m },
                    { 15, 3.0m, 15.0m, 12.0m, 3, 2, 0.25m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PoliticasVacacionesDetalles_PoliticaVacacionId",
                table: "PoliticasVacacionesDetalles",
                column: "PoliticaVacacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoliticasVacacionesDetalles");

            migrationBuilder.DropTable(
                name: "PoliticasVacaciones");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 14, 8, 35, 309, DateTimeKind.Local).AddTicks(9419));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 14, 8, 35, 309, DateTimeKind.Local).AddTicks(9536));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 14, 8, 35, 309, DateTimeKind.Local).AddTicks(9537));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 14, 8, 35, 309, DateTimeKind.Local).AddTicks(9538));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 14, 8, 35, 309, DateTimeKind.Local).AddTicks(9539));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 14, 8, 35, 309, DateTimeKind.Local).AddTicks(9540));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 14, 8, 35, 309, DateTimeKind.Local).AddTicks(9542));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 14, 8, 35, 309, DateTimeKind.Local).AddTicks(9543));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 6, 14, 8, 35, 309, DateTimeKind.Local).AddTicks(9544));
        }
    }
}
