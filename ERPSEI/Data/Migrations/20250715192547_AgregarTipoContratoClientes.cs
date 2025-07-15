using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTipoContratoClientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoContratoId",
                table: "ClienteContratos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClienteContratos_TipoContratoId",
                table: "ClienteContratos",
                column: "TipoContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClienteContratos_TipoContratos_TipoContratoId",
                table: "ClienteContratos",
                column: "TipoContratoId",
                principalTable: "TipoContratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClienteContratos_TipoContratos_TipoContratoId",
                table: "ClienteContratos");

            migrationBuilder.DropIndex(
                name: "IX_ClienteContratos_TipoContratoId",
                table: "ClienteContratos");

            migrationBuilder.DropColumn(
                name: "TipoContratoId",
                table: "ClienteContratos");
        }
    }
}
