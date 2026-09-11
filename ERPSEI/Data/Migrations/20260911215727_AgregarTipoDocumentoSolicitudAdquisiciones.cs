using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTipoDocumentoSolicitudAdquisiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoDocumentoSolicitud",
                table: "ADQ_Solicitudes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Cotizaciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoDocumentoSolicitud",
                table: "ADQ_Solicitudes");
        }
    }
}