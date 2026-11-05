
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class HU2_DocumentosObligatoriosEmpresas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================
            // HU2 - DOCUMENTOS OBLIGATORIOS POR EMPRESA
            // ============================================

            migrationBuilder.CreateTable(
                name: "EmpresasDocumentosObligatorios",
                columns: table => new
                {
                    EmpresaId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    TipoArchivoId = table.Column<int>(
                        type: "int",
                        nullable: false)
                },
                constraints: table =>
                {
                    // Clave primaria compuesta:
                    // Impide duplicar el mismo documento
                    // obligatorio para una empresa.

                    table.PrimaryKey(
                        "PK_EmpresasDocumentosObligatorios",
                        x => new
                        {
                            x.EmpresaId,
                            x.TipoArchivoId
                        });

                    // Relación con Empresas.

                    table.ForeignKey(
                        name: "FK_EmpresasDocumentosObligatorios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);

                    // Relación con los tipos documentales.

                    table.ForeignKey(
                        name: "FK_EmpresasDocumentosObligatorios_TipoArchivoEmpresa_TipoArchivoId",
                        column: x => x.TipoArchivoId,
                        principalTable: "TipoArchivoEmpresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            // Índice para optimizar consultas por tipo documental.

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasDocumentosObligatorios_TipoArchivoId",
                table: "EmpresasDocumentosObligatorios",
                column: "TipoArchivoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revierte únicamente la tabla creada por esta HU.
            // No altera empresas ni documentos existentes.

            migrationBuilder.DropTable(
                name: "EmpresasDocumentosObligatorios");
        }
    }
}