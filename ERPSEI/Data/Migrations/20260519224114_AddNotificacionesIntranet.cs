using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacionesIntranet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificacionesIntranet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Url = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Icono = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    UserIdCreador = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionesIntranet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacionesIntranet_AspNetUsers_UserIdCreador",
                        column: x => x.UserIdCreador,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificacionesIntranetUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificacionIntranetId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Leida = table.Column<bool>(type: "bit", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionesIntranetUsuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacionesIntranetUsuarios_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificacionesIntranetUsuarios_NotificacionesIntranet_NotificacionIntranetId",
                        column: x => x.NotificacionIntranetId,
                        principalTable: "NotificacionesIntranet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6336));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6346));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6347));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6348));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6348));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6354));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6355));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6356));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 19, 16, 41, 12, 561, DateTimeKind.Local).AddTicks(6356));

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionesIntranet_UserIdCreador",
                table: "NotificacionesIntranet",
                column: "UserIdCreador");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionesIntranetUsuarios_NotificacionIntranetId",
                table: "NotificacionesIntranetUsuarios",
                column: "NotificacionIntranetId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionesIntranetUsuarios_UserId",
                table: "NotificacionesIntranetUsuarios",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificacionesIntranetUsuarios");

            migrationBuilder.DropTable(
                name: "NotificacionesIntranet");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 16, 10, 47, 392, DateTimeKind.Local).AddTicks(1073));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 16, 10, 47, 392, DateTimeKind.Local).AddTicks(1089));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 16, 10, 47, 392, DateTimeKind.Local).AddTicks(1091));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 16, 10, 47, 392, DateTimeKind.Local).AddTicks(1092));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 16, 10, 47, 392, DateTimeKind.Local).AddTicks(1093));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 16, 10, 47, 392, DateTimeKind.Local).AddTicks(1093));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 16, 10, 47, 392, DateTimeKind.Local).AddTicks(1094));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 16, 10, 47, 392, DateTimeKind.Local).AddTicks(1095));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 5, 18, 16, 10, 47, 392, DateTimeKind.Local).AddTicks(1096));
        }
    }
}
