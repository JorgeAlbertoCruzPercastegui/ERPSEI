using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRelojChecadorDigital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RC_Auditoria",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AfectadoUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Resultado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodigoDetalle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JornadaId = table.Column<long>(type: "bigint", nullable: true),
                    ChecadaId = table.Column<long>(type: "bigint", nullable: true),
                    FechaUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RC_Auditoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RC_AutorizacionesModalidades",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Modalidad = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    OficinaId = table.Column<int>(type: "int", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AutorizadoPorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AutorizadoUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RC_AutorizacionesModalidades", x => x.Id);
                    table.CheckConstraint("CK_RC_Autorizacion_Estado", "[Estado] IN ('Pendiente','Aprobada','Rechazada','Revocada')");
                    table.CheckConstraint("CK_RC_Autorizacion_Fechas", "[FechaFin] >= [FechaInicio]");
                    table.CheckConstraint("CK_RC_Autorizacion_Modalidad", "[Modalidad] IN ('Presencial','Remoto','Hibrido','Comision')");
                    table.ForeignKey(
                        name: "FK_RC_AutorizacionesModalidades_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RC_AutorizacionesModalidades_Oficinas_OficinaId",
                        column: x => x.OficinaId,
                        principalTable: "Oficinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RC_Jornadas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Modalidad = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    InicioUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MinutosObjetivo = table.Column<int>(type: "int", nullable: false),
                    MinutosEfectivos = table.Column<int>(type: "int", nullable: true),
                    RequiereRevision = table.Column<bool>(type: "bit", nullable: false),
                    MotivoRevision = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualizadoUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RC_Jornadas", x => x.Id);
                    table.CheckConstraint("CK_RC_Jornada_Estado", "[Estado] IN ('Abierta','Cerrada','EnRevision','Anulada')");
                    table.CheckConstraint("CK_RC_Jornada_Fechas", "[FinUtc] IS NULL OR [FinUtc] >= [InicioUtc]");
                    table.CheckConstraint("CK_RC_Jornada_Objetivo", "[MinutosObjetivo] BETWEEN 1 AND 1440");
                    table.ForeignKey(
                        name: "FK_RC_Jornadas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RC_Jornadas_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RC_OficinasGeocercas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OficinaId = table.Column<int>(type: "int", nullable: false),
                    Latitud = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    Longitud = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: false),
                    RadioMetros = table.Column<int>(type: "int", nullable: false),
                    PrecisionMaximaMetros = table.Column<int>(type: "int", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false),
                    CreadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualizadoUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualizadoPorUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RC_OficinasGeocercas", x => x.Id);
                    table.CheckConstraint("CK_RC_Geocerca_Latitud", "[Latitud] BETWEEN -90 AND 90");
                    table.CheckConstraint("CK_RC_Geocerca_Longitud", "[Longitud] BETWEEN -180 AND 180");
                    table.CheckConstraint("CK_RC_Geocerca_Precision", "[PrecisionMaximaMetros] BETWEEN 5 AND 500");
                    table.CheckConstraint("CK_RC_Geocerca_Radio", "[RadioMetros] BETWEEN 10 AND 1000");
                    table.ForeignKey(
                        name: "FK_RC_OficinasGeocercas_Oficinas_OficinaId",
                        column: x => x.OficinaId,
                        principalTable: "Oficinas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RC_Checadas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JornadaId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    TipoMovimiento = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Origen = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FechaHoraUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Latitud = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    Longitud = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    PrecisionMetros = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: true),
                    OficinaId = table.Column<int>(type: "int", nullable: true),
                    DistanciaOficinaMetros = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    ResultadoUbicacion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EstadoValidacion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IdempotencyKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObservacionCodigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RC_Checadas", x => x.Id);
                    table.CheckConstraint("CK_RC_Checada_Origen", "[Origen] IN ('INTRANET_WEB','INTRANET_PWA','AJUSTE_TH')");
                    table.CheckConstraint("CK_RC_Checada_Tipo", "[TipoMovimiento] IN ('Entrada','SalidaComer','RegresoComer','Salida')");
                    table.ForeignKey(
                        name: "FK_RC_Checadas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RC_Checadas_RC_Jornadas_JornadaId",
                        column: x => x.JornadaId,
                        principalTable: "RC_Jornadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RC_Evidencias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChecadaId = table.Column<long>(type: "bigint", nullable: false),
                    RutaPrivada = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HashSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TipoContenido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    CapturadaUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EliminarDespuesUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RC_Evidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RC_Evidencias_RC_Checadas_ChecadaId",
                        column: x => x.ChecadaId,
                        principalTable: "RC_Checadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RC_Auditoria_AfectadoUserId_FechaUtc",
                table: "RC_Auditoria",
                columns: new[] { "AfectadoUserId", "FechaUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RC_Auditoria_FechaUtc",
                table: "RC_Auditoria",
                column: "FechaUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RC_AutorizacionesModalidades_OficinaId",
                table: "RC_AutorizacionesModalidades",
                column: "OficinaId");

            migrationBuilder.CreateIndex(
                name: "IX_RC_AutorizacionesModalidades_UserId_FechaInicio_FechaFin",
                table: "RC_AutorizacionesModalidades",
                columns: new[] { "UserId", "FechaInicio", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "IX_RC_Checadas_IdempotencyKey",
                table: "RC_Checadas",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RC_Checadas_JornadaId_FechaHoraUtc",
                table: "RC_Checadas",
                columns: new[] { "JornadaId", "FechaHoraUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RC_Checadas_UserId_FechaHoraUtc",
                table: "RC_Checadas",
                columns: new[] { "UserId", "FechaHoraUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RC_Evidencias_ChecadaId",
                table: "RC_Evidencias",
                column: "ChecadaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RC_Jornadas_EmpleadoId_InicioUtc",
                table: "RC_Jornadas",
                columns: new[] { "EmpleadoId", "InicioUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RC_Jornadas_UserId",
                table: "RC_Jornadas",
                column: "UserId",
                unique: true,
                filter: "[Estado] = 'Abierta'");

            migrationBuilder.CreateIndex(
                name: "IX_RC_OficinasGeocercas_OficinaId",
                table: "RC_OficinasGeocercas",
                column: "OficinaId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RC_Auditoria");

            migrationBuilder.DropTable(
                name: "RC_AutorizacionesModalidades");

            migrationBuilder.DropTable(
                name: "RC_Evidencias");

            migrationBuilder.DropTable(
                name: "RC_OficinasGeocercas");

            migrationBuilder.DropTable(
                name: "RC_Checadas");

            migrationBuilder.DropTable(
                name: "RC_Jornadas");
        }
    }
}
