using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearModuloExpedientesBancarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EB_Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    RazonSocial = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: false),

                    NombreCorto = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false),

                    Rfc = table.Column<string>(
                        type: "nvarchar(13)",
                        maxLength: 13,
                        nullable: false),

                    Nivel = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true),

                    ActividadComercial = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true),

                    TelefonoBancos = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: true),

                    CorreoBancos = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true),

                    FechaConstitucion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    NumeroEscritura = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true),

                    DomicilioFiscal = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true),

                    Observaciones = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true),

                    Deshabilitado = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    Eliminado = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"),

                    FechaActualizacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    UsuarioCreacionId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false),

                    UsuarioActualizacionId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EB_Empresas",
                        x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EB_TiposDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    Nombre = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false),

                    Categoria = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    Descripcion = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true),

                    EsObligatorio = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true),

                    RequiereFechaVencimiento = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    PermiteMultiplesArchivos = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    Orden = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Deshabilitado = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    Eliminado = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"),

                    FechaActualizacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    UsuarioCreacionId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EB_TiposDocumento",
                        x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EB_Accionistas",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    EmpresaId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    NombreCompleto = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: false),

                    Rfc = table.Column<string>(
                        type: "nvarchar(13)",
                        maxLength: 13,
                        nullable: true),

                    PorcentajeParticipacion = table.Column<decimal>(
                        type: "decimal(7,4)",
                        precision: 7,
                        scale: 4,
                        nullable: false),

                    Nacionalidad = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true),

                    EsRepresentanteLegal = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    Deshabilitado = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    Eliminado = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"),

                    FechaActualizacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    UsuarioCreacionId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false),

                    UsuarioActualizacionId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EB_Accionistas",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_EB_Accionistas_EB_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "EB_Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EB_Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    EmpresaId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    TipoDocumentoId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    NombreOriginal = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: false),

                    NombreAlmacenado = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: false),

                    RutaArchivo = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: false),

                    Extension = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: false),

                    MimeType = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false),

                    TamanoBytes = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    Version = table.Column<int>(
                        type: "int",
                        nullable: false,
                        defaultValue: 1),

                    FechaCarga = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"),

                    FechaVencimiento = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    Estado = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false,
                        defaultValue: "Vigente"),

                    Observaciones = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true),

                    EsVersionActual = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true),

                    Eliminado = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false),

                    FechaEliminacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    UsuarioCargaId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false),

                    UsuarioEliminacionId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EB_Documentos",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_EB_Documentos_EB_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "EB_Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_EB_Documentos_EB_TiposDocumento_TipoDocumentoId",
                        column: x => x.TipoDocumentoId,
                        principalTable: "EB_TiposDocumento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EB_Accionistas_EmpresaId",
                table: "EB_Accionistas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EB_Documentos_EmpresaId",
                table: "EB_Documentos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_EB_Documentos_Expediente",
                table: "EB_Documentos",
                columns: new[]
                {
                    "EmpresaId",
                    "TipoDocumentoId",
                    "EsVersionActual"
                });

            migrationBuilder.CreateIndex(
                name: "IX_EB_Documentos_TipoDocumentoId",
                table: "EB_Documentos",
                column: "TipoDocumentoId");

            migrationBuilder.CreateIndex(
                name: "IX_EB_Empresas_NombreCorto",
                table: "EB_Empresas",
                column: "NombreCorto");

            migrationBuilder.CreateIndex(
                name: "IX_EB_Empresas_RazonSocial",
                table: "EB_Empresas",
                column: "RazonSocial");

            migrationBuilder.CreateIndex(
                name: "UX_EB_Empresas_Rfc",
                table: "EB_Empresas",
                column: "Rfc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_EB_TiposDocumento_Nombre_Categoria",
                table: "EB_TiposDocumento",
                columns: new[]
                {
                    "Nombre",
                    "Categoria"
                },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EB_Accionistas");

            migrationBuilder.DropTable(
                name: "EB_Documentos");

            migrationBuilder.DropTable(
                name: "EB_Empresas");

            migrationBuilder.DropTable(
                name: "EB_TiposDocumento");
        }
    }
}