using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeEmpresaClasificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeEmpresaClasificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    NombreEmpresa = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: false),

                    Tipo = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_TimeEmpresaClasificaciones",
                        x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeEmpresaClasificaciones_NombreEmpresa",
                table: "TimeEmpresaClasificaciones",
                column: "NombreEmpresa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeEmpresaClasificaciones");
        }
    }
}