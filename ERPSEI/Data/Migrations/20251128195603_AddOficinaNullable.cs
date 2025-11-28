using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOficinaNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RazonSocial",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "OficinaId",
                table: "ActivosFijos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivosFijos_OficinaId",
                table: "ActivosFijos",
                column: "OficinaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivosFijos_Oficinas_OficinaId",
                table: "ActivosFijos",
                column: "OficinaId",
                principalTable: "Oficinas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivosFijos_Oficinas_OficinaId",
                table: "ActivosFijos");

            migrationBuilder.DropIndex(
                name: "IX_ActivosFijos_OficinaId",
                table: "ActivosFijos");

            migrationBuilder.DropColumn(
                name: "OficinaId",
                table: "ActivosFijos");

            migrationBuilder.AlterColumn<string>(
                name: "RazonSocial",
                table: "Empresas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
