using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    public partial class HU6_AdquisicionesAprobadoresPorArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // HU #6 - Configuración independiente por área.

            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "ADQ_ConfiguracionAprobacionPresupuestal",
                type: "int",
                nullable: true);

            // Evitar niveles duplicados dentro de una misma área.

            migrationBuilder.CreateIndex(
                name: "UX_ADQ_Configuracion_Area_Orden",
                table: "ADQ_ConfiguracionAprobacionPresupuestal",
                columns: new[] { "AreaId", "Orden" },
                unique: true,
                filter: "[AreaId] IS NOT NULL AND [Eliminado] = 0");

            // Relación con el catálogo de áreas.

            migrationBuilder.AddForeignKey(
                name: "FK_ADQ_ConfiguracionAprobacionPresupuestal_Areas_AreaId",
                table: "ADQ_ConfiguracionAprobacionPresupuestal",
                column: "AreaId",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ADQ_ConfiguracionAprobacionPresupuestal_Areas_AreaId",
                table: "ADQ_ConfiguracionAprobacionPresupuestal");

            migrationBuilder.DropIndex(
                name: "UX_ADQ_Configuracion_Area_Orden",
                table: "ADQ_ConfiguracionAprobacionPresupuestal");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "ADQ_ConfiguracionAprobacionPresupuestal");
        }
    }
}