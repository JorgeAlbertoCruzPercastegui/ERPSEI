
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdquisicionesPagoParcialidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // HU#1 - REGISTRO Y CONTROL DE PAGOS EN PARCIALIDADES
            // =========================================================
            // Crea exclusivamente la tabla de parcialidades.
            // No modifica registros de otras tablas existentes.
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_SolicitudesPagoParcialidades",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    SolicitudPagoId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    NumeroParcialidad = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Porcentaje = table.Column<decimal>(
                        type: "decimal(9,4)",
                        precision: 9,
                        scale: 4,
                        nullable: false),

                    Importe = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false),

                    Estatus = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false),

                    FechaPago = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    UsuarioPagoId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true),

                    ReferenciaPago = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    FechaActualizacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    Eliminado = table.Column<bool>(
                        type: "bit",
                        nullable: false)
                },
                constraints: table =>
                {
                    // Llave primaria
                    table.PrimaryKey(
                        "PK_ADQ_SolicitudesPagoParcialidades",
                        x => x.Id);

                    // Estatus permitidos
                    table.CheckConstraint(
                        "CK_ADQ_Parcialidad_Estatus",
                        "[Estatus] IN ('Pendiente', 'Pagada')");

                    // El importe debe ser mayor que cero
                    table.CheckConstraint(
                        "CK_ADQ_Parcialidad_Importe",
                        "[Importe] > 0");

                    // El número de parcialidad debe ser positivo
                    table.CheckConstraint(
                        "CK_ADQ_Parcialidad_Numero",
                        "[NumeroParcialidad] > 0");

                    // Porcentaje válido
                    table.CheckConstraint(
                        "CK_ADQ_Parcialidad_Porcentaje",
                        "[Porcentaje] > 0 AND [Porcentaje] <= 100");

                    // Relación con la solicitud de pago
                    table.ForeignKey(
                        name: "FK_ADQ_SolicitudesPagoParcialidades_ADQ_SolicitudesPago_SolicitudPagoId",
                        column: x => x.SolicitudPagoId,
                        principalTable: "ADQ_SolicitudesPago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // =========================================================
            // ÍNDICE ÚNICO
            // =========================================================
            // Impide números de parcialidad repetidos
            // dentro de la misma solicitud de pago.
            // =========================================================

            migrationBuilder.CreateIndex(
                name: "IX_ADQ_SolicitudesPagoParcialidades_SolicitudPagoId_NumeroParcialidad",
                table: "ADQ_SolicitudesPagoParcialidades",
                columns: new[]
                {
                    "SolicitudPagoId",
                    "NumeroParcialidad"
                },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // REVERSIÓN HU#1
            // =========================================================
            // Advertencia: revertir esta migración elimina
            // la tabla y todos sus registros de parcialidades.
            // =========================================================

            migrationBuilder.DropTable(
                name: "ADQ_SolicitudesPagoParcialidades");
        }
    }
}
