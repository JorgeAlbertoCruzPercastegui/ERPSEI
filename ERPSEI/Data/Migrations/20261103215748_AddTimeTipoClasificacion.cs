using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeTipoClasificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeTipoClasificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    NombreUsuario = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: false),

                    NombreEmpresa = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: false),

                    Actividad = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: false),

                    Descripcion = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: false),

                    Tipo = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_TimeTipoClasificaciones",
                        x => x.Id);

                    table.CheckConstraint(
                        "CK_TimeTipoClasificaciones_Tipo",
                        "[Tipo] IN ('Administrativa','Back Office','Clientes','Operativa')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeTipoClasificaciones_Relacion",
                table: "TimeTipoClasificaciones",
                columns: new[]
                {
                    "NombreUsuario",
                    "NombreEmpresa",
                    "Actividad",
                    "Descripcion"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeTipoClasificaciones");
        }
    }
}