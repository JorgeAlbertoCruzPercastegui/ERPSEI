using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearAprobacionesPresupuestalesAdquisiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // NUEVO PERMISO PARA APROBACIÓN PRESUPUESTAL
            // =========================================================

            migrationBuilder.AddColumn<bool>(
                name: "PuedeAprobarPresupuesto",
                table: "ADQ_PermisosUsuarios",
                type: "bit",
                nullable: false,
                defaultValue: false);


            // =========================================================
            // APROBACIONES PRESUPUESTALES
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_AprobacionesPresupuestales",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    SolicitudId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    CotizacionId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    MontoSolicitado = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false),

                    UsuarioSolicitaId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false),

                    FechaSolicitud = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    UsuarioAprobadorId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true),

                    FechaRespuesta = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    Estatus = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false),

                    ComentarioSolicitud = table.Column<string>(
                        type: "nvarchar(3000)",
                        maxLength: 3000,
                        nullable: true),

                    ComentarioRespuesta = table.Column<string>(
                        type: "nvarchar(3000)",
                        maxLength: 3000,
                        nullable: true),

                    Eliminado = table.Column<bool>(
                        type: "bit",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_AprobacionesPresupuestales",
                        x => x.Id);


                    table.ForeignKey(
                        name: "FK_ADQ_AprobacionesPresupuestales_ADQ_Cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "ADQ_Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);


                    table.ForeignKey(
                        name: "FK_ADQ_AprobacionesPresupuestales_ADQ_Solicitudes_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "ADQ_Solicitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });


            // =========================================================
            // ÍNDICES
            // =========================================================

            migrationBuilder.CreateIndex(
                name: "IX_ADQ_AprobacionesPresupuestales_CotizacionId",
                table: "ADQ_AprobacionesPresupuestales",
                column: "CotizacionId");


            migrationBuilder.CreateIndex(
                name: "IX_ADQ_AprobacionesPresupuestales_SolicitudId",
                table: "ADQ_AprobacionesPresupuestales",
                column: "SolicitudId");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // ELIMINAR APROBACIONES PRESUPUESTALES
            // =========================================================

            migrationBuilder.DropTable(
                name: "ADQ_AprobacionesPresupuestales");


            // =========================================================
            // ELIMINAR PERMISO
            // =========================================================

            migrationBuilder.DropColumn(
                name: "PuedeAprobarPresupuesto",
                table: "ADQ_PermisosUsuarios");
        }
    }
}