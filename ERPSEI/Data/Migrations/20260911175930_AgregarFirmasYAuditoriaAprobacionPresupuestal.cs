using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFirmasYAuditoriaAprobacionPresupuestal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // EVENTOS / AUDITORÍA DE APROBACIÓN PRESUPUESTAL
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_AprobacionesPresupuestalesEventos",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false
                    )
                    .Annotation(
                        "SqlServer:Identity",
                        "1, 1"
                    ),

                    AprobacionPresupuestalId =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    AprobacionPresupuestalDetalleId =
                        table.Column<int>(
                            type: "int",
                            nullable: true
                        ),

                    TipoEvento =
                        table.Column<string>(
                            type: "nvarchar(100)",
                            maxLength: 100,
                            nullable: false
                        ),

                    Descripcion =
                        table.Column<string>(
                            type: "nvarchar(500)",
                            maxLength: 500,
                            nullable: false
                        ),

                    UsuarioId =
                        table.Column<string>(
                            type: "nvarchar(450)",
                            maxLength: 450,
                            nullable: true
                        ),

                    OrdenEtapa =
                        table.Column<int>(
                            type: "int",
                            nullable: true
                        ),

                    NombreEtapa =
                        table.Column<string>(
                            type: "nvarchar(150)",
                            maxLength: 150,
                            nullable: true
                        ),

                    EstatusAnterior =
                        table.Column<string>(
                            type: "nvarchar(50)",
                            maxLength: 50,
                            nullable: true
                        ),

                    EstatusNuevo =
                        table.Column<string>(
                            type: "nvarchar(50)",
                            maxLength: 50,
                            nullable: true
                        ),

                    FechaEvento =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: false
                        ),

                    DireccionIp =
                        table.Column<string>(
                            type: "nvarchar(64)",
                            maxLength: 64,
                            nullable: true
                        ),

                    Eliminado =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_AprobacionesPresupuestalesEventos",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_AprobacionesPresupuestalesEventos_ADQ_AprobacionesPresupuestalesDetalle_AprobacionPresupuestalDetalleId",
                        column:
                            x => x.AprobacionPresupuestalDetalleId,
                        principalTable:
                            "ADQ_AprobacionesPresupuestalesDetalle",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_AprobacionesPresupuestalesEventos_ADQ_AprobacionesPresupuestales_AprobacionPresupuestalId",
                        column:
                            x => x.AprobacionPresupuestalId,
                        principalTable:
                            "ADQ_AprobacionesPresupuestales",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // FIRMAS REUTILIZABLES DEL USUARIO
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_FirmasUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false
                    )
                    .Annotation(
                        "SqlServer:Identity",
                        "1, 1"
                    ),

                    UsuarioId =
                        table.Column<string>(
                            type: "nvarchar(450)",
                            maxLength: 450,
                            nullable: false
                        ),

                    NombreFirma =
                        table.Column<string>(
                            type: "nvarchar(150)",
                            maxLength: 150,
                            nullable: false
                        ),

                    TipoFirma =
                        table.Column<string>(
                            type: "nvarchar(30)",
                            maxLength: 30,
                            nullable: false
                        ),

                    RutaArchivo =
                        table.Column<string>(
                            type: "nvarchar(1000)",
                            maxLength: 1000,
                            nullable: false
                        ),

                    HashArchivo =
                        table.Column<string>(
                            type: "nvarchar(128)",
                            maxLength: 128,
                            nullable: false
                        ),

                    EsPredeterminada =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),

                    TotalUsos =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    FechaUltimoUso =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: true
                        ),

                    FechaCreacion =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: false
                        ),

                    FechaModificacion =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: true
                        ),

                    Activa =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),

                    Eliminado =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_FirmasUsuario",
                        x => x.Id
                    );
                }
            );


            // =========================================================
            // SEGURIDAD / PIN DE FIRMA
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_SeguridadFirmaUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false
                    )
                    .Annotation(
                        "SqlServer:Identity",
                        "1, 1"
                    ),

                    UsuarioId =
                        table.Column<string>(
                            type: "nvarchar(450)",
                            maxLength: 450,
                            nullable: false
                        ),

                    PinHash =
                        table.Column<string>(
                            type: "nvarchar(1000)",
                            maxLength: 1000,
                            nullable: false
                        ),

                    IntentosFallidos =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    BloqueadoHasta =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: true
                        ),

                    FechaConfiguracion =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: false
                        ),

                    FechaModificacion =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: true
                        ),

                    Activo =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),

                    Eliminado =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_SeguridadFirmaUsuario",
                        x => x.Id
                    );
                }
            );


            // =========================================================
            // SNAPSHOT DE FIRMA UTILIZADA EN CADA APROBACIÓN
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_FirmasAprobacionPresupuestal",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false
                    )
                    .Annotation(
                        "SqlServer:Identity",
                        "1, 1"
                    ),

                    AprobacionPresupuestalDetalleId =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    FirmaUsuarioId =
                        table.Column<int>(
                            type: "int",
                            nullable: true
                        ),

                    UsuarioFirmanteId =
                        table.Column<string>(
                            type: "nvarchar(450)",
                            maxLength: 450,
                            nullable: false
                        ),

                    NombreFirmante =
                        table.Column<string>(
                            type: "nvarchar(250)",
                            maxLength: 250,
                            nullable: false
                        ),

                    EmailFirmante =
                        table.Column<string>(
                            type: "nvarchar(256)",
                            maxLength: 256,
                            nullable: true
                        ),

                    OrdenEtapa =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    NombreEtapa =
                        table.Column<string>(
                            type: "nvarchar(150)",
                            maxLength: 150,
                            nullable: false
                        ),

                    TipoFirma =
                        table.Column<string>(
                            type: "nvarchar(30)",
                            maxLength: 30,
                            nullable: false
                        ),

                    RutaFirmaSnapshot =
                        table.Column<string>(
                            type: "nvarchar(1000)",
                            maxLength: 1000,
                            nullable: false
                        ),

                    HashFirma =
                        table.Column<string>(
                            type: "nvarchar(128)",
                            maxLength: 128,
                            nullable: false
                        ),

                    HashContextoFirmado =
                        table.Column<string>(
                            type: "nvarchar(128)",
                            maxLength: 128,
                            nullable: true
                        ),

                    Decision =
                        table.Column<string>(
                            type: "nvarchar(30)",
                            maxLength: 30,
                            nullable: false
                        ),

                    FechaFirma =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: false
                        ),

                    DireccionIp =
                        table.Column<string>(
                            type: "nvarchar(64)",
                            maxLength: 64,
                            nullable: true
                        ),

                    UserAgent =
                        table.Column<string>(
                            type: "nvarchar(500)",
                            maxLength: 500,
                            nullable: true
                        ),

                    Eliminado =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_FirmasAprobacionPresupuestal",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_FirmasAprobacionPresupuestal_ADQ_AprobacionesPresupuestalesDetalle_AprobacionPresupuestalDetalleId",
                        column:
                            x => x.AprobacionPresupuestalDetalleId,
                        principalTable:
                            "ADQ_AprobacionesPresupuestalesDetalle",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_FirmasAprobacionPresupuestal_ADQ_FirmasUsuario_FirmaUsuarioId",
                        column:
                            x => x.FirmaUsuarioId,
                        principalTable:
                            "ADQ_FirmasUsuario",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // ÍNDICES - EVENTOS
            // =========================================================

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_AprobacionesPresupuestalesEventos_AprobacionPresupuestalDetalleId",
                table:
                    "ADQ_AprobacionesPresupuestalesEventos",
                column:
                    "AprobacionPresupuestalDetalleId"
            );


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_AprobacionesPresupuestalesEventos_AprobacionPresupuestalId_FechaEvento",
                table:
                    "ADQ_AprobacionesPresupuestalesEventos",
                columns:
                    new[]
                    {
                        "AprobacionPresupuestalId",
                        "FechaEvento"
                    }
            );


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_AprobacionesPresupuestalesEventos_UsuarioId_FechaEvento",
                table:
                    "ADQ_AprobacionesPresupuestalesEventos",
                columns:
                    new[]
                    {
                        "UsuarioId",
                        "FechaEvento"
                    }
            );


            // =========================================================
            // ÍNDICES - SNAPSHOT FIRMA
            // =========================================================

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_FirmasAprobacionPresupuestal_AprobacionPresupuestalDetalleId_UsuarioFirmanteId",
                table:
                    "ADQ_FirmasAprobacionPresupuestal",
                columns:
                    new[]
                    {
                        "AprobacionPresupuestalDetalleId",
                        "UsuarioFirmanteId"
                    }
            );


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_FirmasAprobacionPresupuestal_FirmaUsuarioId",
                table:
                    "ADQ_FirmasAprobacionPresupuestal",
                column:
                    "FirmaUsuarioId"
            );


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_FirmasAprobacionPresupuestal_UsuarioFirmanteId_FechaFirma",
                table:
                    "ADQ_FirmasAprobacionPresupuestal",
                columns:
                    new[]
                    {
                        "UsuarioFirmanteId",
                        "FechaFirma"
                    }
            );


            // =========================================================
            // ÍNDICES - FIRMAS DEL USUARIO
            // =========================================================

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_FirmasUsuario_UsuarioId_Activa_Eliminado",
                table:
                    "ADQ_FirmasUsuario",
                columns:
                    new[]
                    {
                        "UsuarioId",
                        "Activa",
                        "Eliminado"
                    }
            );


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_FirmasUsuario_UsuarioId_FechaUltimoUso",
                table:
                    "ADQ_FirmasUsuario",
                columns:
                    new[]
                    {
                        "UsuarioId",
                        "FechaUltimoUso"
                    }
            );


            // =========================================================
            // ÍNDICE ÚNICO - SEGURIDAD DE FIRMA
            // UN USUARIO = UNA CONFIGURACIÓN DE PIN
            // =========================================================

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_SeguridadFirmaUsuario_UsuarioId",
                table:
                    "ADQ_SeguridadFirmaUsuario",
                column:
                    "UsuarioId",
                unique:
                    true
            );
        }


        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder
        )
        {
            // =========================================================
            // ELIMINAR EN ORDEN INVERSO POR LAS RELACIONES
            // =========================================================

            migrationBuilder.DropTable(
                name:
                    "ADQ_AprobacionesPresupuestalesEventos"
            );


            migrationBuilder.DropTable(
                name:
                    "ADQ_FirmasAprobacionPresupuestal"
            );


            migrationBuilder.DropTable(
                name:
                    "ADQ_SeguridadFirmaUsuario"
            );


            migrationBuilder.DropTable(
                name:
                    "ADQ_FirmasUsuario"
            );
        }
    }
}