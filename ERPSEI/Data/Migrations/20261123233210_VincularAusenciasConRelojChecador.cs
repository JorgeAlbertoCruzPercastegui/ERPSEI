
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <summary>
    /// Vincula las solicitudes de Home Office del módulo
    /// Ausencias con las autorizaciones del Reloj Checador.
    ///
    /// La columna AusenciaId es nullable para conservar
    /// las autorizaciones manuales existentes.
    ///
    /// El índice único filtrado garantiza una sola
    /// autorización por solicitud de Ausencias.
    /// </summary>
    public partial class VincularAusenciasConRelojChecador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // 1. AGREGAR RELACIÓN CON AUSENCIAS
            // =====================================================

            migrationBuilder.AddColumn<int>(
                name: "AusenciaId",
                table: "RC_AutorizacionesModalidades",
                type: "int",
                nullable: true);

            // =====================================================
            // 2. ÍNDICE ÚNICO FILTRADO
            // =====================================================
            // Permite múltiples autorizaciones manuales
            // con AusenciaId = NULL.
            //
            // Impide dos autorizaciones con el mismo
            // AusenciaId diferente de NULL.

            migrationBuilder.CreateIndex(
                name: "UX_RC_AutorizacionesModalidades_AusenciaId",
                table: "RC_AutorizacionesModalidades",
                column: "AusenciaId",
                unique: true,
                filter: "[AusenciaId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // 1. ELIMINAR ÍNDICE ÚNICO
            // =====================================================

            migrationBuilder.DropIndex(
                name: "UX_RC_AutorizacionesModalidades_AusenciaId",
                table: "RC_AutorizacionesModalidades");

            // =====================================================
            // 2. ELIMINAR COLUMNA
            // =====================================================

            migrationBuilder.DropColumn(
                name: "AusenciaId",
                table: "RC_AutorizacionesModalidades");
        }
    }
}
