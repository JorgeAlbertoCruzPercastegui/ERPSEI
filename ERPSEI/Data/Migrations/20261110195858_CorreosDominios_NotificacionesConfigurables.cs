using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorreosDominios_NotificacionesConfigurables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionesNotificacionesCorreosDominios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DiasAnticipacion = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    DiasEscalamiento = table.Column<int>(type: "int", nullable: false, defaultValue: 2),
                    RecordatoriosDiarios = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NotificarVencidos = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificador = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesNotificacionesCorreosDominios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DestinatariosNotificacionesCorreosDominios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioCreador = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioModificador = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DestinatariosNotificacionesCorreosDominios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DestinatariosNotificacionesCorreosDominios_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialNotificacionesCorreosDominios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorreoDominioId = table.Column<int>(type: "int", nullable: false),
                    FechaCaducacion = table.Column<DateTime>(type: "date", nullable: false),
                    FechaAviso = table.Column<DateTime>(type: "date", nullable: false),
                    DestinatarioClave = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CorreoElectronico = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    TipoAviso = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Canal = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Intentos = table.Column<int>(type: "int", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialNotificacionesCorreosDominios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialNotificacionesCorreosDominios_CorreosDominios_CorreoDominioId",
                        column: x => x.CorreoDominioId,
                        principalTable: "CorreosDominios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ConfiguracionesNotificacionesCorreosDominios",
                columns: new[] { "Id", "DiasAnticipacion", "DiasEscalamiento", "FechaModificacion", "Habilitado", "NotificarVencidos", "RecordatoriosDiarios", "UsuarioModificador" },
                values: new object[] { 1, 10, 2, null, true, true, true, null });

            migrationBuilder.CreateIndex(
                name: "IX_DestinatariosNotificacionesCorreosDominios_CorreoElectronico",
                table: "DestinatariosNotificacionesCorreosDominios",
                column: "CorreoElectronico");

            migrationBuilder.CreateIndex(
                name: "IX_DestinatariosNotificacionesCorreosDominios_UserId",
                table: "DestinatariosNotificacionesCorreosDominios",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_CorreosDominios_Historial_NotificacionDiaria",
                table: "HistorialNotificacionesCorreosDominios",
                columns: new[] { "CorreoDominioId", "FechaCaducacion", "FechaAviso", "DestinatarioClave", "Canal" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionesNotificacionesCorreosDominios");

            migrationBuilder.DropTable(
                name: "DestinatariosNotificacionesCorreosDominios");

            migrationBuilder.DropTable(
                name: "HistorialNotificacionesCorreosDominios");
        }
    }
}