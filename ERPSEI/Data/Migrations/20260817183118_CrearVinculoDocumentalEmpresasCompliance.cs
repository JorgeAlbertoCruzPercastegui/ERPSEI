using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearVinculoDocumentalEmpresasCompliance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EB_DocumentosVinculosEmpresa",
                columns: table => new
                {
                    Id = table.Column<int>(
                            type: "int",
                            nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    EmpresaMaestraId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    EmpresaComplianceId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    TipoArchivoEmpresaId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    TipoDocumentoComplianceId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    ArchivoEmpresaId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true),

                    DocumentoComplianceId = table.Column<int>(
                        type: "int",
                        nullable: true),

                    HashContenido = table.Column<string>(
                        type: "nvarchar(64)",
                        maxLength: 64,
                        nullable: false),

                    Origen = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false),

                    Activo = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"),

                    FechaActualizacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EB_DocumentosVinculosEmpresa",
                        x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EB_DocumentosVinculosEmpresa_ArchivoEmpresa",
                table: "EB_DocumentosVinculosEmpresa",
                column: "ArchivoEmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EB_DocumentosVinculosEmpresa_DocumentoCompliance",
                table: "EB_DocumentosVinculosEmpresa",
                column: "DocumentoComplianceId");

            migrationBuilder.CreateIndex(
                name: "IX_EB_DocumentosVinculosEmpresa_Hash",
                table: "EB_DocumentosVinculosEmpresa",
                column: "HashContenido");

            migrationBuilder.CreateIndex(
                name: "IX_EB_DocumentosVinculosEmpresa_Relacion",
                table: "EB_DocumentosVinculosEmpresa",
                columns: new[]
                {
                    "EmpresaMaestraId",
                    "EmpresaComplianceId",
                    "TipoArchivoEmpresaId",
                    "TipoDocumentoComplianceId",
                    "Activo"
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EB_DocumentosVinculosEmpresa");
        }
    }
}