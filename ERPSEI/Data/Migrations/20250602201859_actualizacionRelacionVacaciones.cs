using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class actualizacionRelacionVacaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialesVacaciones_Empleados_EmpleadoId",
                table: "HistorialesVacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PeriodosVacacionales_Empleados_EmpleadoId",
                table: "PeriodosVacacionales");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesVacaciones_Empleados_EmpleadoId",
                table: "SolicitudesVacaciones");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialesVacaciones_Empleados_EmpleadoId",
                table: "HistorialesVacaciones",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodosVacacionales_Empleados_EmpleadoId",
                table: "PeriodosVacacionales",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesVacaciones_Empleados_EmpleadoId",
                table: "SolicitudesVacaciones",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistorialesVacaciones_Empleados_EmpleadoId",
                table: "HistorialesVacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PeriodosVacacionales_Empleados_EmpleadoId",
                table: "PeriodosVacacionales");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesVacaciones_Empleados_EmpleadoId",
                table: "SolicitudesVacaciones");

            migrationBuilder.AddForeignKey(
                name: "FK_HistorialesVacaciones_Empleados_EmpleadoId",
                table: "HistorialesVacaciones",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodosVacacionales_Empleados_EmpleadoId",
                table: "PeriodosVacacionales",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesVacaciones_Empleados_EmpleadoId",
                table: "SolicitudesVacaciones",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
