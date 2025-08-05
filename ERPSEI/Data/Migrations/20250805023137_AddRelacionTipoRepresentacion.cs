using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelacionTipoRepresentacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoRepresentacionId",
                table: "EmpresaContratos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoRepresentacionId",
                table: "ClienteContratos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TipoRepresentaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoRepresentaciones", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TipoRepresentaciones",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Representante Legal" },
                    { 2, "Apoderado Legal" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaContratos_TipoRepresentacionId",
                table: "EmpresaContratos",
                column: "TipoRepresentacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteContratos_TipoRepresentacionId",
                table: "ClienteContratos",
                column: "TipoRepresentacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClienteContratos_TipoRepresentaciones_TipoRepresentacionId",
                table: "ClienteContratos",
                column: "TipoRepresentacionId",
                principalTable: "TipoRepresentaciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmpresaContratos_TipoRepresentaciones_TipoRepresentacionId",
                table: "EmpresaContratos",
                column: "TipoRepresentacionId",
                principalTable: "TipoRepresentaciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClienteContratos_TipoRepresentaciones_TipoRepresentacionId",
                table: "ClienteContratos");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpresaContratos_TipoRepresentaciones_TipoRepresentacionId",
                table: "EmpresaContratos");

            migrationBuilder.DropTable(
                name: "TipoRepresentaciones");

            migrationBuilder.DropIndex(
                name: "IX_EmpresaContratos_TipoRepresentacionId",
                table: "EmpresaContratos");

            migrationBuilder.DropIndex(
                name: "IX_ClienteContratos_TipoRepresentacionId",
                table: "ClienteContratos");

            migrationBuilder.DropColumn(
                name: "TipoRepresentacionId",
                table: "EmpresaContratos");

            migrationBuilder.DropColumn(
                name: "TipoRepresentacionId",
                table: "ClienteContratos");
        }
    }
}
