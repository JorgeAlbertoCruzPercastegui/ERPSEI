using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloPathVacaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiasFestivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiasFestivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PeriodosVacacionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FechaInicioPeriodo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinPeriodo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiasDisponibles = table.Column<int>(type: "int", nullable: false),
                    DiasTomados = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosVacacionales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodosVacacionales_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiasSolicitados = table.Column<int>(type: "int", nullable: false),
                    ComentarioEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ComentarioAutorizador = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    AutorizadorId = table.Column<int>(type: "int", nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesVacaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesVacaciones_Empleados_AutorizadorId",
                        column: x => x.AutorizadorId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesVacaciones_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesVacaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiasTomados = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SolicitudVacacionesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesVacaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesVacaciones_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialesVacaciones_SolicitudesVacaciones_SolicitudVacacionesId",
                        column: x => x.SolicitudVacacionesId,
                        principalTable: "SolicitudesVacaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesVacaciones_EmpleadoId",
                table: "HistorialesVacaciones",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesVacaciones_SolicitudVacacionesId",
                table: "HistorialesVacaciones",
                column: "SolicitudVacacionesId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosVacacionales_EmpleadoId",
                table: "PeriodosVacacionales",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_AutorizadorId",
                table: "SolicitudesVacaciones",
                column: "AutorizadorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_EmpleadoId",
                table: "SolicitudesVacaciones",
                column: "EmpleadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiasFestivos");

            migrationBuilder.DropTable(
                name: "HistorialesVacaciones");

            migrationBuilder.DropTable(
                name: "PeriodosVacacionales");

            migrationBuilder.DropTable(
                name: "SolicitudesVacaciones");
        }
    }
}
