using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTiposDocumentalesEmpresasCompliance
        : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            /*
             * ==========================================================
             * NUEVOS TIPOS DOCUMENTALES PARA EMPRESAS
             * ==========================================================
             *
             * IMPORTANTE:
             *
             * No modificamos ninguno de los tipos existentes 1 - 13.
             *
             * Únicamente agregamos los nuevos tipos que tendrán
             * equivalencia documental con Compliance.
             * ==========================================================
             */

            migrationBuilder.InsertData(
                table: "TipoArchivoEmpresa",
                columns: new[]
                {
                    "Id",
                    "Description"
                },
                values: new object[,]
                {
                    {
                        14,
                        "ActasAdicionales"
                    },
                    {
                        15,
                        "PoderNotarial"
                    },
                    {
                        16,
                        "INEAccionistas"
                    },
                    {
                        17,
                        "CSFAccionistas"
                    },
                    {
                        18,
                        "ComprobanteDomicilioAccionistas"
                    },
                    {
                        19,
                        "DeclaracionAnualMensual"
                    },
                    {
                        20,
                        "OpinionCumplimientoSAT"
                    },
                    {
                        21,
                        "PruebaVida"
                    }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            /*
             * ==========================================================
             * REVERTIR ÚNICAMENTE LOS TIPOS NUEVOS
             * ==========================================================
             *
             * No se modifica ningún registro histórico 1 - 13.
             * ==========================================================
             */

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 14
            );

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 15
            );

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 16
            );

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 17
            );

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 18
            );

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 19
            );

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 20
            );

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 21
            );
        }
    }
}