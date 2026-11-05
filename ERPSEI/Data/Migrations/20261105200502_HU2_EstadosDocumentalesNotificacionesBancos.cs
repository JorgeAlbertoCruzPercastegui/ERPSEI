
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class HU2_EstadosDocumentalesNotificacionesBancos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ==========================================================
            // HU2 - CA05: NOTIFICACIONES AUTOMÁTICAS A BANCOS
            // ==========================================================
            //
            // Se crea una tabla para registrar el estado documental
            // de cada empresa conforme a la regla global.
            //
            // Permite identificar:
            // - La empresa evaluada.
            // - La firma de la regla documental utilizada.
            // - Si el expediente está completo.
            // - La fecha de la última evaluación.
            // - La fecha de la última notificación enviada.
            //
            // No modifica ni elimina documentos o empresas.
            // ==========================================================

            migrationBuilder.CreateTable(
                name: "EmpresasEstadosDocumentalesHU2",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    FirmaRegla = table.Column<string>(
                        type: "nvarchar(64)",
                        maxLength: 64,
                        nullable: false),

                    Completo = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    FechaEvaluacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    FechaUltimaNotificacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EmpresasEstadosDocumentalesHU2",
                        x => x.EmpresaId);

                    table.ForeignKey(
                        name: "FK_EmpresasEstadosDocumentalesHU2_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id");
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ==========================================================
            // REVERSIÓN DE HU2 - CA05
            // ==========================================================
            //
            // Elimina únicamente la tabla creada por esta migración.
            // No elimina documentos ni empresas existentes.
            // ==========================================================

            migrationBuilder.DropTable(
                name: "EmpresasEstadosDocumentalesHU2");
        }
    }
}