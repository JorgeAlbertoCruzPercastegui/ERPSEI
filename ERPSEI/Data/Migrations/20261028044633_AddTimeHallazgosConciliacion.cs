
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeHallazgosConciliacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // TIME INTEGRATION - HALLAZGOS DE CONCILIACIÓN
            // =====================================================

            migrationBuilder.CreateTable(
                name: "HallazgosConciliacion",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    TablaOrigen = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false),

                    RegistroOrigenId = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    TipoHallazgo = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false),

                    Estado = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false),

                    PrimeraDeteccion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    UltimaRevision = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    NumeroDetecciones = table.Column<int>(
                        type: "int",
                        nullable: false),

                    FechaConfirmacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    FechaRecuperacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    Observaciones = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_HallazgosConciliacion",
                        x => x.Id);
                });

            // =====================================================
            // ÍNDICE PARA CONSULTAS POR ESTADO Y FECHA
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_HallazgosConciliacion_Estado_UltimaRevision",
                schema: "TimeIntegracion",
                table: "HallazgosConciliacion",
                columns: new[]
                {
                    "Estado",
                    "UltimaRevision"
                });

            // =====================================================
            // ÍNDICE ÚNICO PARA EVITAR HALLAZGOS DUPLICADOS
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_HallazgosConciliacion_TablaOrigen_RegistroOrigenId_TipoHallazgo",
                schema: "TimeIntegracion",
                table: "HallazgosConciliacion",
                columns: new[]
                {
                    "TablaOrigen",
                    "RegistroOrigenId",
                    "TipoHallazgo"
                },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // REVERSIÓN EXCLUSIVA DE TIME INTEGRATION
            // =====================================================

            migrationBuilder.DropTable(
                name: "HallazgosConciliacion",
                schema: "TimeIntegracion");
        }
    }
}
