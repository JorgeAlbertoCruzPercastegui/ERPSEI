
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    public partial class AddBioTimeIntegration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // BIOTIME CLOUD 2.0 - CHECADAS
            // =====================================================

            migrationBuilder.CreateTable(
                name: "BioTimeChecadas",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    EmpCode = table.Column<string>(
                        type: "nvarchar(40)",
                        maxLength: 40,
                        nullable: false),

                    FechaHora = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    PunchState = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: true),

                    VerifyType = table.Column<int>(
                        type: "int",
                        nullable: true),

                    TerminalSn = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true),

                    SincronizadoUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_BioTimeChecadas",
                        x => x.Id);
                });

            // =====================================================
            // BIOTIME CLOUD 2.0 - EMPLEADOS
            // =====================================================

            migrationBuilder.CreateTable(
                name: "BioTimeEmpleados",
                columns: table => new
                {
                    EmpCode = table.Column<string>(
                        type: "nvarchar(40)",
                        maxLength: 40,
                        nullable: false),

                    Nombre = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true),

                    Apellidos = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true),

                    Departamento = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true),

                    Puesto = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true),

                    ActualizadoUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_BioTimeEmpleados",
                        x => x.EmpCode);
                });

            // =====================================================
            // BIOTIME CLOUD 2.0 - SINCRONIZACIONES
            // =====================================================

            migrationBuilder.CreateTable(
                name: "BioTimeSincronizaciones",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    InicioUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    FinUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    FechaInicioConsulta = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    FechaFinConsulta = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    Estado = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false),

                    EmpleadosProcesados = table.Column<int>(
                        type: "int",
                        nullable: false),

                    ChecadasInsertadas = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Error = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_BioTimeSincronizaciones",
                        x => x.Id);
                });

            // =====================================================
            // ÍNDICES - CHECADAS
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_BioTimeChecadas_EmpCode",
                table: "BioTimeChecadas",
                column: "EmpCode");

            migrationBuilder.CreateIndex(
                name: "IX_BioTimeChecadas_FechaHora_EmpCode",
                table: "BioTimeChecadas",
                columns: new[]
                {
                    "FechaHora",
                    "EmpCode"
                });

            // =====================================================
            // ÍNDICES - SINCRONIZACIONES
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_BioTimeSincronizaciones_InicioUtc",
                table: "BioTimeSincronizaciones",
                column: "InicioUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // REVERSIÓN - ÚNICAMENTE BIOTIME
            // =====================================================

            migrationBuilder.DropTable(
                name: "BioTimeChecadas");

            migrationBuilder.DropTable(
                name: "BioTimeEmpleados");

            migrationBuilder.DropTable(
                name: "BioTimeSincronizaciones");
        }
    }
}