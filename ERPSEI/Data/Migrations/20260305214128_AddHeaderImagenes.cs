using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHeaderImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeaderImagenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Temporada = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NombreArchivo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    VigenciaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VigenciaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EsPermanente = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Orden = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioCreadorId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeaderImagenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeaderImagenes_AspNetUsers_UsuarioCreadorId",
                        column: x => x.UsuarioCreadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_HeaderImagenes_Activo_EsPermanente_Orden_Temporada",
                table: "HeaderImagenes",
                columns: new[] { "Activo", "EsPermanente", "Orden", "Temporada" });

            migrationBuilder.CreateIndex(
                name: "IX_HeaderImagenes_UsuarioCreadorId",
                table: "HeaderImagenes",
                column: "UsuarioCreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_HeaderImagenes_VigenciaFin",
                table: "HeaderImagenes",
                column: "VigenciaFin");

            migrationBuilder.CreateIndex(
                name: "IX_HeaderImagenes_VigenciaInicio",
                table: "HeaderImagenes",
                column: "VigenciaInicio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeaderImagenes");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 12, 43, 9, 823, DateTimeKind.Local).AddTicks(2164));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 12, 43, 9, 823, DateTimeKind.Local).AddTicks(2178));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 12, 43, 9, 823, DateTimeKind.Local).AddTicks(2179));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 12, 43, 9, 823, DateTimeKind.Local).AddTicks(2180));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 12, 43, 9, 823, DateTimeKind.Local).AddTicks(2181));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 12, 43, 9, 823, DateTimeKind.Local).AddTicks(2182));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 12, 43, 9, 823, DateTimeKind.Local).AddTicks(2183));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 12, 43, 9, 823, DateTimeKind.Local).AddTicks(2184));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 12, 43, 9, 823, DateTimeKind.Local).AddTicks(2185));
        }
    }
}
