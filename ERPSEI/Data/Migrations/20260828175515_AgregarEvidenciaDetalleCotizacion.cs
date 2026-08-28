using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEvidenciaDetalleCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CotizacionDetalleId",
                table: "ADQ_CotizacionAdjuntos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ADQ_CotizacionAdjuntos_CotizacionDetalleId",
                table: "ADQ_CotizacionAdjuntos",
                column: "CotizacionDetalleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ADQ_CotizacionAdjuntos_ADQ_CotizacionDetalles_CotizacionDetalleId",
                table: "ADQ_CotizacionAdjuntos",
                column: "CotizacionDetalleId",
                principalTable: "ADQ_CotizacionDetalles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ADQ_CotizacionAdjuntos_ADQ_CotizacionDetalles_CotizacionDetalleId",
                table: "ADQ_CotizacionAdjuntos");

            migrationBuilder.DropIndex(
                name: "IX_ADQ_CotizacionAdjuntos_CotizacionDetalleId",
                table: "ADQ_CotizacionAdjuntos");

            migrationBuilder.DropColumn(
                name: "CotizacionDetalleId",
                table: "ADQ_CotizacionAdjuntos");
        }
    }
}