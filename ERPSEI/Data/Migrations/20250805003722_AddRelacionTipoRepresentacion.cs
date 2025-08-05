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
            migrationBuilder.DropColumn(
                name: "TipoRepresentacion",
                table: "EmpresaContratos");

            migrationBuilder.DropColumn(
                name: "TipoRepresentacion",
                table: "ClienteContratos");

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
                name: "TipoRepresentacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoRepresentacion", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TipoRepresentacion",
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
                name: "FK_ClienteContratos_TipoRepresentacion_TipoRepresentacionId",
                table: "ClienteContratos",
                column: "TipoRepresentacionId",
                principalTable: "TipoRepresentacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmpresaContratos_TipoRepresentacion_TipoRepresentacionId",
                table: "EmpresaContratos",
                column: "TipoRepresentacionId",
                principalTable: "TipoRepresentacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClienteContratos_TipoRepresentacion_TipoRepresentacionId",
                table: "ClienteContratos");

            migrationBuilder.DropForeignKey(
                name: "FK_EmpresaContratos_TipoRepresentacion_TipoRepresentacionId",
                table: "EmpresaContratos");

            migrationBuilder.DropTable(
                name: "TipoRepresentacion");

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

            migrationBuilder.AddColumn<string>(
                name: "TipoRepresentacion",
                table: "EmpresaContratos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoRepresentacion",
                table: "ClienteContratos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
