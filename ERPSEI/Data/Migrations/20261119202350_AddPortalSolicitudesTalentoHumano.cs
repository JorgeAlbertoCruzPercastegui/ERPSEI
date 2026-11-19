
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    public partial class AddPortalSolicitudesTalentoHumano : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // 1. CONSECUTIVOS DE FOLIOS
            // =====================================================

            migrationBuilder.CreateTable(
                name: "TH_FolioConsecutivos",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    Tipo = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Anio = table.Column<int>(
                        type: "int",
                        nullable: false),

                    UltimoConsecutivo = table.Column<int>(
                        type: "int",
                        nullable: false),

                    FechaActualizacionUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    RowVersion = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_TH_FolioConsecutivos",
                        x => x.Id);

                    table.CheckConstraint(
                        "CK_TH_Folios_Anio",
                        "[Anio] >= 2000");

                    table.CheckConstraint(
                        "CK_TH_Folios_Consecutivo",
                        "[UltimoConsecutivo] >= 0");
                });

            // =====================================================
            // 2. SOLICITUDES DE TALENTO HUMANO
            // =====================================================

            migrationBuilder.CreateTable(
                name: "TH_Solicitudes",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    Folio = table.Column<string>(
                        type: "nvarchar(40)",
                        maxLength: 40,
                        nullable: false),

                    Tipo = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Estatus = table.Column<int>(
                        type: "int",
                        nullable: false),

                    EmpleadoId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    SolicitanteUserId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false),

                    NumeroEmpleado = table.Column<string>(
                        type: "nvarchar(40)",
                        maxLength: 40,
                        nullable: false),

                    NombreSolicitante = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: false),

                    AreaSolicitante = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true),

                    OficinaSolicitante = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true),

                    AtendidoPorUserId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true),

                    FechaEnvioUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    FechaInicioAtencionUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    FechaRespuestaUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    FechaCierreUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    FechaActualizacionUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    DiasObjetivoSla = table.Column<int>(
                        type: "int",
                        nullable: true),

                    FechaLimiteSlaUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    RespondidaAutomaticamente = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    RowVersion = table.Column<byte[]>(
                        type: "rowversion",
                        rowVersion: true,
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_TH_Solicitudes",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_TH_Solicitudes_AspNetUsers_AtendidoPorUserId",
                        column: x => x.AtendidoPorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_TH_Solicitudes_AspNetUsers_SolicitanteUserId",
                        column: x => x.SolicitanteUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_TH_Solicitudes_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // =====================================================
            // 3. HISTORIAL DE SOLICITUDES
            // =====================================================

            migrationBuilder.CreateTable(
                name: "TH_HistorialSolicitudes",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    SolicitudId = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    UsuarioId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true),

                    RolEjecutor = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    Accion = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    EstatusAnterior = table.Column<int>(
                        type: "int",
                        nullable: true),

                    EstatusNuevo = table.Column<int>(
                        type: "int",
                        nullable: true),

                    Comentario = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: true),

                    FechaHoraUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    EjecutadoPorSistema = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    IpOrigen = table.Column<string>(
                        type: "nvarchar(64)",
                        maxLength: 64,
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_TH_HistorialSolicitudes",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_TH_HistorialSolicitudes_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_TH_HistorialSolicitudes_TH_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "TH_Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // =====================================================
            // 4. RESPUESTAS DE SOLICITUDES
            // =====================================================

            migrationBuilder.CreateTable(
                name: "TH_Respuestas",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    SolicitudId = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    TipoRespuesta = table.Column<string>(
                        type: "nvarchar(40)",
                        maxLength: 40,
                        nullable: false),

                    Contenido = table.Column<string>(
                        type: "nvarchar(4000)",
                        maxLength: 4000,
                        nullable: true),

                    UsuarioRespuestaId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true),

                    EsAutomatica = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    EsRespuestaFinal = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    FechaRespuestaUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_TH_Respuestas",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_TH_Respuestas_AspNetUsers_UsuarioRespuestaId",
                        column: x => x.UsuarioRespuestaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_TH_Respuestas_TH_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "TH_Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // =====================================================
            // 5. DOCUMENTOS DE SOLICITUDES
            // =====================================================

            migrationBuilder.CreateTable(
                name: "TH_Documentos",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    SolicitudId = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    RespuestaId = table.Column<long>(
                        type: "bigint",
                        nullable: true),

                    NombreOriginal = table.Column<string>(
                        type: "nvarchar(260)",
                        maxLength: 260,
                        nullable: false),

                    NombreAlmacenado = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true),

                    TipoContenido = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true),

                    TamanoBytes = table.Column<long>(
                        type: "bigint",
                        nullable: true),

                    Origen = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false),

                    DriveId = table.Column<string>(
                        type: "nvarchar(300)",
                        maxLength: 300,
                        nullable: true),

                    DriveItemId = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true),

                    RutaAlmacenamiento = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true),

                    HashSha256 = table.Column<string>(
                        type: "nvarchar(64)",
                        maxLength: 64,
                        nullable: true),

                    Anio = table.Column<int>(
                        type: "int",
                        nullable: true),

                    Periodo = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true),

                    Formato = table.Column<string>(
                        type: "nvarchar(15)",
                        maxLength: 15,
                        nullable: true),

                    UsuarioCargaId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true),

                    FechaRegistroUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    EntregadoAlSolicitante = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    FechaEntregaUtc = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_TH_Documentos",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_TH_Documentos_AspNetUsers_UsuarioCargaId",
                        column: x => x.UsuarioCargaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_TH_Documentos_TH_Respuestas_RespuestaId",
                        column: x => x.RespuestaId,
                        principalTable: "TH_Respuestas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_TH_Documentos_TH_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "TH_Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // =====================================================
            // 6. ÍNDICES - DOCUMENTOS
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_TH_Documentos_DriveId_DriveItemId",
                table: "TH_Documentos",
                columns: new[] { "DriveId", "DriveItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_TH_Documentos_RespuestaId",
                table: "TH_Documentos",
                column: "RespuestaId");

            migrationBuilder.CreateIndex(
                name: "IX_TH_Documentos_SolicitudId_FechaRegistroUtc",
                table: "TH_Documentos",
                columns: new[] { "SolicitudId", "FechaRegistroUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TH_Documentos_UsuarioCargaId",
                table: "TH_Documentos",
                column: "UsuarioCargaId");

            // =====================================================
            // 7. ÍNDICES - FOLIOS
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_TH_FolioConsecutivos_Tipo_Anio",
                table: "TH_FolioConsecutivos",
                columns: new[] { "Tipo", "Anio" },
                unique: true);

            // =====================================================
            // 8. ÍNDICES - HISTORIAL
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_TH_HistorialSolicitudes_SolicitudId_FechaHoraUtc",
                table: "TH_HistorialSolicitudes",
                columns: new[] { "SolicitudId", "FechaHoraUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TH_HistorialSolicitudes_UsuarioId",
                table: "TH_HistorialSolicitudes",
                column: "UsuarioId");

            // =====================================================
            // 9. ÍNDICES - RESPUESTAS
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_TH_Respuestas_SolicitudId_FechaRespuestaUtc",
                table: "TH_Respuestas",
                columns: new[] { "SolicitudId", "FechaRespuestaUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TH_Respuestas_UsuarioRespuestaId",
                table: "TH_Respuestas",
                column: "UsuarioRespuestaId");

            // =====================================================
            // 10. ÍNDICES - SOLICITUDES
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_TH_Solicitudes_AtendidoPorUserId",
                table: "TH_Solicitudes",
                column: "AtendidoPorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TH_Solicitudes_EmpleadoId_FechaEnvioUtc",
                table: "TH_Solicitudes",
                columns: new[] { "EmpleadoId", "FechaEnvioUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_TH_Solicitudes_Folio",
                table: "TH_Solicitudes",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TH_Solicitudes_SolicitanteUserId",
                table: "TH_Solicitudes",
                column: "SolicitanteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TH_Solicitudes_Tipo_Estatus_FechaEnvioUtc",
                table: "TH_Solicitudes",
                columns: new[]
                {
                    "Tipo",
                    "Estatus",
                    "FechaEnvioUtc"
                });
        }

        // =====================================================
        // REVERSIÓN EXCLUSIVA DE TABLAS TH
        // =====================================================

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TH_Documentos");

            migrationBuilder.DropTable(
                name: "TH_FolioConsecutivos");

            migrationBuilder.DropTable(
                name: "TH_HistorialSolicitudes");

            migrationBuilder.DropTable(
                name: "TH_Respuestas");

            migrationBuilder.DropTable(
                name: "TH_Solicitudes");
        }
    }
}
