using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNivelPresupuestalPermisosAdquisiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NivelPresupuestal",
                table: "ADQ_PermisosUsuarios",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NivelPresupuestal",
                table: "ADQ_PermisosUsuarios");
        }
    }
}