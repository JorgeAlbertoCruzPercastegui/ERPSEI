using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentoAutorizaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.CreateTable(
                name: "DocumentosAutorizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentoId = table.Column<int>(type: "int", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "PENDIENTE"),
                    AutorizadoPorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosAutorizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosAutorizaciones_AspNetUsers_AutorizadoPorId",
                        column: x => x.AutorizadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosAutorizaciones_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3481));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3497));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3499));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3500));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3501));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3503));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3504));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3505));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 2, 6, 18, 43, 11, 445, DateTimeKind.Local).AddTicks(3507));

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosAutorizaciones_AutorizadoPorId",
                table: "DocumentosAutorizaciones",
                column: "AutorizadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosAutorizaciones_DocumentoId_Rol",
                table: "DocumentosAutorizaciones",
                columns: new[] { "DocumentoId", "Rol" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosAutorizaciones");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8447));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8478));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8482));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8486));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8490));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8493));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8497));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8505));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8508));

            migrationBuilder.InsertData(
                table: "TiposDocumento",
                columns: new[] { "Id", "Activo", "FechaCreacion", "Nombre" },
                values: new object[] { 8, true, new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8501), "Requerimientos" });
        }
    }
}
