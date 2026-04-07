using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventosIntranet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventosIntranet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TipoEvento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraEvento = table.Column<TimeSpan>(type: "time", nullable: true),
                    FechaPublicacionProgramada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Publicado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EsProgramado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RequiereGeolocalizacion = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Region = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UrlFormulario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TextoBoton = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RutaPortada = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NombrePortada = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    CreadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosIntranet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventosIntranet_AspNetUsers_CreadoPorId",
                        column: x => x.CreadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventosIntranet_AspNetUsers_ModificadoPorId",
                        column: x => x.ModificadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_EventosIntranet_Activo_Publicado_FechaEvento",
                table: "EventosIntranet",
                columns: new[] { "Activo", "Publicado", "FechaEvento" });

            migrationBuilder.CreateIndex(
                name: "IX_EventosIntranet_CreadoPorId",
                table: "EventosIntranet",
                column: "CreadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_EventosIntranet_ModificadoPorId",
                table: "EventosIntranet",
                column: "ModificadoPorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventosIntranet");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 18, 19, 53, 399, DateTimeKind.Local).AddTicks(4847));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 18, 19, 53, 399, DateTimeKind.Local).AddTicks(4860));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 18, 19, 53, 399, DateTimeKind.Local).AddTicks(4861));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 18, 19, 53, 399, DateTimeKind.Local).AddTicks(4862));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 18, 19, 53, 399, DateTimeKind.Local).AddTicks(4862));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 18, 19, 53, 399, DateTimeKind.Local).AddTicks(4863));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 18, 19, 53, 399, DateTimeKind.Local).AddTicks(4864));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 18, 19, 53, 399, DateTimeKind.Local).AddTicks(4865));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 4, 6, 18, 19, 53, 399, DateTimeKind.Local).AddTicks(4866));
        }
    }
}
