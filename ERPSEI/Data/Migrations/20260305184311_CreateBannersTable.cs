using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateBannersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    table.PrimaryKey("PK_Banners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banners_AspNetUsers_UsuarioCreadorId",
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

            migrationBuilder.CreateIndex(
                name: "IX_Banners_Activo_EsPermanente_Orden",
                table: "Banners",
                columns: new[] { "Activo", "EsPermanente", "Orden" });

            migrationBuilder.CreateIndex(
                name: "IX_Banners_UsuarioCreadorId",
                table: "Banners",
                column: "UsuarioCreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_VigenciaFin",
                table: "Banners",
                column: "VigenciaFin");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_VigenciaInicio",
                table: "Banners",
                column: "VigenciaInicio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8264));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8278));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8279));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8280));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8281));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8282));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8284));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8285));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 5, 11, 53, 40, 479, DateTimeKind.Local).AddTicks(8286));
        }
    }
}
