using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================
            // EstatusDocumento (catálogo)
            // =========================
            migrationBuilder.CreateTable(
                name: "EstatusDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false), // NO IDENTITY: catálogo fijo
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EsPublicable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstatusDocumento", x => x.Id);
                });

            // =========================
            // TiposDocumento (catálogo)
            // =========================
            migrationBuilder.CreateTable(
                name: "TiposDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false), // ✅ NO IDENTITY (para insertar Id 1..10)
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumento", x => x.Id);
                });

            // =========================
            // Documentos (cabecera)
            // =========================
            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    AreaId = table.Column<int>(type: "int", nullable: false),
                    TipoDocumentoId = table.Column<int>(type: "int", nullable: false),

                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),

                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),

                    CreadoPorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),

                    ModificadoPorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);

                    table.ForeignKey(
        name: "FK_Documentos_Areas_AreaId",
        column: x => x.AreaId,
        principalTable: "Areas",
        principalColumn: "Id",
        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Documentos_TiposDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "TiposDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // =========================
            // DocumentoPalabrasClave
            // =========================
            migrationBuilder.CreateTable(
                name: "DocumentoPalabrasClave",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentoId = table.Column<int>(type: "int", nullable: false),
                    Palabra = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoPalabrasClave", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentoPalabrasClave_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // =========================
            // DocumentoVersiones
            // =========================
            migrationBuilder.CreateTable(
                name: "DocumentoVersiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    DocumentoId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),

                    EstatusDocumentoId = table.Column<int>(type: "int", nullable: false),

                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comentarios = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),

                    NombreArchivo = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    RutaArchivo = table.Column<string>(type: "nvarchar(800)", maxLength: 800, nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: true),

                    EsActual = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),

                    CreadoPorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentoVersiones", x => x.Id);

                    table.ForeignKey(
                        name: "FK_DocumentoVersiones_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);

                    table.ForeignKey(
                        name: "FK_DocumentoVersiones_EstatusDocumento_EstatusDocumentoId",
                        column: x => x.EstatusDocumentoId,
                        principalTable: "EstatusDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // =========================
            // Índices
            // =========================
            migrationBuilder.CreateIndex(name: "IX_Documentos_AreaId", table: "Documentos", column: "AreaId");
            migrationBuilder.CreateIndex(name: "IX_Documentos_TipoDocumentoId", table: "Documentos", column: "TipoDocumentoId");
            migrationBuilder.CreateIndex(name: "IX_Documentos_Titulo", table: "Documentos", column: "Titulo");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoPalabrasClave_DocumentoId_Palabra",
                table: "DocumentoPalabrasClave",
                columns: new[] { "DocumentoId", "Palabra" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoPalabrasClave_Palabra",
                table: "DocumentoPalabrasClave",
                column: "Palabra");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoVersiones_DocumentoId_Version",
                table: "DocumentoVersiones",
                columns: new[] { "DocumentoId", "Version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoVersiones_DocumentoId_EsActual",
                table: "DocumentoVersiones",
                columns: new[] { "DocumentoId", "EsActual" },
                unique: true,
                filter: "[EsActual] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentoVersiones_EstatusDocumentoId",
                table: "DocumentoVersiones",
                column: "EstatusDocumentoId");

            // =========================
            // Seed catálogos
            // =========================
            migrationBuilder.InsertData(
                table: "EstatusDocumento",
                columns: new[] { "Id", "Nombre", "EsPublicable", "Activo" },
                values: new object[,]
                {
            { 1, "Vigente", true, true },
            { 2, "Obsoleto", false, true },
            { 3, "En Revisión", false, true }
                });

            migrationBuilder.InsertData(
                table: "TiposDocumento",
                columns: new[] { "Id", "Nombre", "Activo" },
                values: new object[,]
                {
            { 1, "Manuales", true },
            { 2, "Procedimientos", true },
            { 3, "Políticas", true },
            { 4, "Reglamentos", true },
            { 5, "Formatos", true },
            { 6, "Diagramas", true },
            { 7, "Referencias Normativas", true },
            { 8, "Requerimientos", true },
            { 9, "Manuales de Capacitación", true },
            { 10, "Otros", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DocumentoPalabrasClave");
            migrationBuilder.DropTable(name: "DocumentoVersiones");
            migrationBuilder.DropTable(name: "Documentos");
            migrationBuilder.DropTable(name: "TiposDocumento");
            migrationBuilder.DropTable(name: "EstatusDocumento");
        }
    }
}
