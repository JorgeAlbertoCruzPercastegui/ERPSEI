using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class comprobanteComplementoPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PagoId",
                table: "ComprobantesComplementos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesComplementos_PagoId",
                table: "ComprobantesComplementos",
                column: "PagoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComprobantesComplementos_Pagos_PagoId",
                table: "ComprobantesComplementos",
                column: "PagoId",
                principalTable: "Pagos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComprobantesComplementos_Pagos_PagoId",
                table: "ComprobantesComplementos");

            migrationBuilder.DropIndex(
                name: "IX_ComprobantesComplementos_PagoId",
                table: "ComprobantesComplementos");

            migrationBuilder.DropColumn(
                name: "PagoId",
                table: "ComprobantesComplementos");
        }
    }
}
