
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AjustarIndiceTimeTipoClasificacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeTipoClasificaciones_Relacion",
                table: "TimeTipoClasificaciones");

            migrationBuilder.CreateIndex(
                name: "IX_TimeTipoClasificaciones_Relacion",
                table: "TimeTipoClasificaciones",
                column: "NombreUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TimeTipoClasificaciones_Relacion",
                table: "TimeTipoClasificaciones");

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
    }
}
