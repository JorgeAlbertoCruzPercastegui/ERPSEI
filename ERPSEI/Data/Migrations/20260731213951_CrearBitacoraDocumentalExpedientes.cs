using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearBitacoraDocumentalExpedientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EB_BitacoraDocumentos",
                columns: table => new
                {
                    Id = table.Column<long>(
                            type: "bigint",
                            nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    EmpresaId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    DocumentoId = table.Column<int>(
                        type: "int",
                        nullable: true),

                    TipoDocumentoId = table.Column<int>(
                        type: "int",
                        nullable: true),

                    Accion = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false),

                    UsuarioId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true),

                    NombreUsuario = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true),

                    NombreDocumento = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true),

                    Banco = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true),

                    FechaEvento = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    DireccionIp = table.Column<string>(
                        type: "nvarchar(64)",
                        maxLength: 64,
                        nullable: true),

                    Navegador = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true),

                    Exitoso = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    Detalle = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true),

                    VersionDocumento = table.Column<int>(
                        type: "int",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EB_BitacoraDocumentos",
                        x => x.Id);

                    table.ForeignKey(
                        name:
                            "FK_EB_BitacoraDocumentos_EB_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "EB_Documentos",
                        principalColumn: "Id",
                        onDelete:
                            ReferentialAction.SetNull);

                    table.ForeignKey(
                        name:
                            "FK_EB_BitacoraDocumentos_EB_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "EB_Empresas",
                        principalColumn: "Id",
                        onDelete:
                            ReferentialAction.Restrict);

                    table.ForeignKey(
                        name:
                            "FK_EB_BitacoraDocumentos_EB_TiposDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable:
                            "EB_TiposDocumento",
                        principalColumn: "Id",
                        onDelete:
                            ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraDocumentos_Accion_Fecha",
                table: "EB_BitacoraDocumentos",
                columns: new[]
                {
                    "Accion",
                    "FechaEvento"
                });

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraDocumentos_Banco_Fecha",
                table: "EB_BitacoraDocumentos",
                columns: new[]
                {
                    "Banco",
                    "FechaEvento"
                });

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraDocumentos_DocumentoId",
                table: "EB_BitacoraDocumentos",
                column: "DocumentoId");

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraDocumentos_Empresa_Fecha",
                table: "EB_BitacoraDocumentos",
                columns: new[]
                {
                    "EmpresaId",
                    "FechaEvento"
                });

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraDocumentos_FechaEvento",
                table: "EB_BitacoraDocumentos",
                column: "FechaEvento");

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraDocumentos_TipoDocumentoId",
                table: "EB_BitacoraDocumentos",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraDocumentos_Usuario_Fecha",
                table: "EB_BitacoraDocumentos",
                columns: new[]
                {
                    "UsuarioId",
                    "FechaEvento"
                });
        }

        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EB_BitacoraDocumentos");
        }
    }
}