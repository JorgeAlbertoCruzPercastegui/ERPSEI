using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class SubTipoContratoGeneradorContratosEmpresaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubTipoContratoId",
                table: "EmpresaContratos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubTipoContratoId",
                table: "ClienteContratos",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTipoContratoId",
                table: "EmpresaContratos");

            migrationBuilder.DropColumn(
                name: "SubTipoContratoId",
                table: "ClienteContratos");
        }
    }
}
