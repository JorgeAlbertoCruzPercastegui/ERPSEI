
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class HU1_ComprobantePagoParcialidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // HU#1 - COMPROBANTE OPCIONAL DE PAGO EN PARCIALIDADES
            // =========================================================
            // Agrega únicamente tres columnas a la tabla existente.
            // No modifica registros históricos.
            // No altera restricciones ni índices existentes.
            // =========================================================

            migrationBuilder.AddColumn<string>(
                name: "ComprobanteHashSha256",
                table: "ADQ_SolicitudesPagoParcialidades",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComprobanteNombreAlmacenado",
                table: "ADQ_SolicitudesPagoParcialidades",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComprobanteNombreOriginal",
                table: "ADQ_SolicitudesPagoParcialidades",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // REVERSIÓN HU#1 - COMPROBANTES
            // =========================================================
            // ATENCIÓN:
            // La reversión elimina las tres columnas y sus datos.
            // No elimina parcialidades ni modifica sus estados.
            // =========================================================

            migrationBuilder.DropColumn(
                name: "ComprobanteHashSha256",
                table: "ADQ_SolicitudesPagoParcialidades");

            migrationBuilder.DropColumn(
                name: "ComprobanteNombreAlmacenado",
                table: "ADQ_SolicitudesPagoParcialidades");

            migrationBuilder.DropColumn(
                name: "ComprobanteNombreOriginal",
                table: "ADQ_SolicitudesPagoParcialidades");
        }
    }
}
