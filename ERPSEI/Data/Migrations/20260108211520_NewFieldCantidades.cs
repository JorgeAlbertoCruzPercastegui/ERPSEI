using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewFieldCantidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cantidades",
                table: "ActivosFijos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClienteContratos_SubTipoContratoId",
                table: "ClienteContratos",
                column: "SubTipoContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClienteContratos_SubTiposContrato_SubTipoContratoId",
                table: "ClienteContratos",
                column: "SubTipoContratoId",
                principalTable: "SubTiposContrato",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClienteContratos_SubTiposContrato_SubTipoContratoId",
                table: "ClienteContratos");

            migrationBuilder.DropIndex(
                name: "IX_ClienteContratos_SubTipoContratoId",
                table: "ClienteContratos");

            migrationBuilder.DropColumn(
                name: "Cantidades",
                table: "ActivosFijos");
        }
    }
}
