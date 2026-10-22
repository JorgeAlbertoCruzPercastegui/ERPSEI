using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeIntegrationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TimeIntegracion");

            migrationBuilder.CreateTable(
                name: "Actividades",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    PeriodoId = table.Column<long>(type: "bigint", nullable: true),
                    UsuarioId = table.Column<long>(type: "bigint", nullable: true),
                    ClaveId = table.Column<long>(type: "bigint", nullable: true),
                    Actividad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DepartamentoActividadId = table.Column<long>(type: "bigint", nullable: true),
                    Orden = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actividades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Claves",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    EmpresaId = table.Column<long>(type: "bigint", nullable: true),
                    DepartamentoId = table.Column<long>(type: "bigint", nullable: true),
                    Proyecto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Clave = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Incremental = table.Column<int>(type: "int", nullable: true),
                    Tipo = table.Column<byte>(type: "tinyint", nullable: true),
                    Estatus = table.Column<byte>(type: "tinyint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claves", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departamentos",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Clave = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Estatus = table.Column<byte>(type: "tinyint", nullable: true),
                    Incremental = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departamentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadoSincronizacion",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Tabla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UltimoId = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    UltimoUpperId = table.Column<long>(type: "bigint", nullable: true),
                    UltimaEjecucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    RegistrosProcesados = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    Error = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadoSincronizacion", x => x.Tabla);
                });

            migrationBuilder.CreateTable(
                name: "Horas",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Fecha = table.Column<DateTime>(type: "date", nullable: true),
                    Horas = table.Column<double>(type: "float", nullable: true),
                    ActividadId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(191)", maxLength: 191, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(191)", maxLength: 191, nullable: true),
                    DepartamentoId = table.Column<long>(type: "bigint", nullable: true),
                    Estatus = table.Column<byte>(type: "tinyint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_UsuarioId_ClaveId",
                schema: "TimeIntegracion",
                table: "Actividades",
                columns: new[] { "UsuarioId", "ClaveId" });

            migrationBuilder.CreateIndex(
                name: "IX_Claves_DepartamentoId",
                schema: "TimeIntegracion",
                table: "Claves",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Horas_ActividadId",
                schema: "TimeIntegracion",
                table: "Horas",
                column: "ActividadId");

            migrationBuilder.CreateIndex(
                name: "IX_Horas_Fecha",
                schema: "TimeIntegracion",
                table: "Horas",
                column: "Fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_DepartamentoId",
                schema: "TimeIntegracion",
                table: "Usuarios",
                column: "DepartamentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actividades",
                schema: "TimeIntegracion");

            migrationBuilder.DropTable(
                name: "Claves",
                schema: "TimeIntegracion");

            migrationBuilder.DropTable(
                name: "Departamentos",
                schema: "TimeIntegracion");

            migrationBuilder.DropTable(
                name: "EstadoSincronizacion",
                schema: "TimeIntegracion");

            migrationBuilder.DropTable(
                name: "Horas",
                schema: "TimeIntegracion");

            migrationBuilder.DropTable(
                name: "Usuarios",
                schema: "TimeIntegracion");
        }
    }
}
