using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class BuildManualesPoliticasIntranet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManualesPoliticasIntranet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModoVisualizacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodigoHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlExterna = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NombreArchivoPdf = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RutaArchivoPdf = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NombrePortada = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RutaPortada = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Publicado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Orden = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioCreadorId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualesPoliticasIntranet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualesPoliticasIntranet_AspNetUsers_UsuarioCreadorId",
                        column: x => x.UsuarioCreadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_ManualesPoliticasIntranet_UsuarioCreadorId",
                table: "ManualesPoliticasIntranet",
                column: "UsuarioCreadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManualesPoliticasIntranet");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 15, 41, 26, 182, DateTimeKind.Local).AddTicks(4050));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 15, 41, 26, 182, DateTimeKind.Local).AddTicks(4062));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 15, 41, 26, 182, DateTimeKind.Local).AddTicks(4063));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 15, 41, 26, 182, DateTimeKind.Local).AddTicks(4064));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 15, 41, 26, 182, DateTimeKind.Local).AddTicks(4065));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 15, 41, 26, 182, DateTimeKind.Local).AddTicks(4066));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 15, 41, 26, 182, DateTimeKind.Local).AddTicks(4067));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 15, 41, 26, 182, DateTimeKind.Local).AddTicks(4068));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 15, 41, 26, 182, DateTimeKind.Local).AddTicks(4070));
        }
    }
}
