
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CorreosDominios_TiposDestinatarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // CORREOS Y DOMINIOS
            // TIPOS DE DESTINATARIOS CONFIGURABLES
            // =====================================================

            migrationBuilder.AddColumn<string>(
                name: "TipoDestinatario",
                table: "DestinatariosNotificacionesCorreosDominios",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Escalamiento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // REVERSIÓN DE LA MIGRACIÓN
            // =====================================================

            migrationBuilder.DropColumn(
                name: "TipoDestinatario",
                table: "DestinatariosNotificacionesCorreosDominios");
        }
    }
}
