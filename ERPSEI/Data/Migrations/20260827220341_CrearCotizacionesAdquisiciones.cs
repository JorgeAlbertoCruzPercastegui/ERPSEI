using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearCotizacionesAdquisiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // ADQ_COTIZACIONES
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_Cotizaciones",
                columns: table => new
                {
                    Id =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        )
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    SolicitudId =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    ProveedorId =
                        table.Column<int>(
                            type: "int",
                            nullable: true
                        ),

                    NombreProveedor =
                        table.Column<string>(
                            type: "nvarchar(250)",
                            maxLength: 250,
                            nullable: false
                        ),

                    RfcProveedor =
                        table.Column<string>(
                            type: "nvarchar(50)",
                            maxLength: 50,
                            nullable: true
                        ),

                    ContactoProveedor =
                        table.Column<string>(
                            type: "nvarchar(250)",
                            maxLength: 250,
                            nullable: true
                        ),

                    EmailProveedor =
                        table.Column<string>(
                            type: "nvarchar(250)",
                            maxLength: 250,
                            nullable: true
                        ),

                    TelefonoProveedor =
                        table.Column<string>(
                            type: "nvarchar(50)",
                            maxLength: 50,
                            nullable: true
                        ),

                    Subtotal =
                        table.Column<decimal>(
                            type: "decimal(18,2)",
                            nullable: false
                        ),

                    AplicaIva =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),

                    PorcentajeIva =
                        table.Column<decimal>(
                            type: "decimal(5,2)",
                            nullable: false
                        ),

                    ImporteIva =
                        table.Column<decimal>(
                            type: "decimal(18,2)",
                            nullable: false
                        ),

                    Total =
                        table.Column<decimal>(
                            type: "decimal(18,2)",
                            nullable: false
                        ),

                    Observaciones =
                        table.Column<string>(
                            type: "nvarchar(3000)",
                            maxLength: 3000,
                            nullable: true
                        ),

                    EsPrincipal =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),

                    Finalizada =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),

                    Eliminado =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),

                    UsuarioCreadorId =
                        table.Column<string>(
                            type: "nvarchar(450)",
                            maxLength: 450,
                            nullable: false
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

                    FechaFinalizacion =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: true
                        )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_Cotizaciones",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Cotizaciones_ADQ_Solicitudes_SolicitudId",

                        column:
                            x => x.SolicitudId,

                        principalTable:
                            "ADQ_Solicitudes",

                        principalColumn:
                            "Id",

                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // ADQ_COTIZACION_ADJUNTOS
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_CotizacionAdjuntos",
                columns: table => new
                {
                    Id =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        )
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    CotizacionId =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    NombreOriginal =
                        table.Column<string>(
                            type: "nvarchar(300)",
                            maxLength: 300,
                            nullable: false
                        ),

                    NombreAlmacenado =
                        table.Column<string>(
                            type: "nvarchar(300)",
                            maxLength: 300,
                            nullable: false
                        ),

                    RutaArchivo =
                        table.Column<string>(
                            type: "nvarchar(500)",
                            maxLength: 500,
                            nullable: false
                        ),

                    Extension =
                        table.Column<string>(
                            type: "nvarchar(20)",
                            maxLength: 20,
                            nullable: false
                        ),

                    MimeType =
                        table.Column<string>(
                            type: "nvarchar(150)",
                            maxLength: 150,
                            nullable: false
                        ),

                    TamanoBytes =
                        table.Column<long>(
                            type: "bigint",
                            nullable: false
                        ),

                    UsuarioCargaId =
                        table.Column<string>(
                            type: "nvarchar(450)",
                            maxLength: 450,
                            nullable: false
                        ),

                    FechaCarga =
                        table.Column<DateTime>(
                            type: "datetime2",
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
                        "PK_ADQ_CotizacionAdjuntos",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_CotizacionAdjuntos_ADQ_Cotizaciones_CotizacionId",

                        column:
                            x => x.CotizacionId,

                        principalTable:
                            "ADQ_Cotizaciones",

                        principalColumn:
                            "Id",

                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // ADQ_COTIZACION_DETALLES
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_CotizacionDetalles",
                columns: table => new
                {
                    Id =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        )
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    CotizacionId =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    ProductoServicio =
                        table.Column<string>(
                            type: "nvarchar(500)",
                            maxLength: 500,
                            nullable: false
                        ),

                    Descripcion =
                        table.Column<string>(
                            type: "nvarchar(2000)",
                            maxLength: 2000,
                            nullable: true
                        ),

                    Cantidad =
                        table.Column<decimal>(
                            type: "decimal(18,4)",
                            nullable: false
                        ),

                    Unidad =
                        table.Column<string>(
                            type: "nvarchar(100)",
                            maxLength: 100,
                            nullable: false
                        ),

                    PrecioUnitario =
                        table.Column<decimal>(
                            type: "decimal(18,2)",
                            nullable: false
                        ),

                    Importe =
                        table.Column<decimal>(
                            type: "decimal(18,2)",
                            nullable: false
                        ),

                    Orden =
                        table.Column<int>(
                            type: "int",
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
                        "PK_ADQ_CotizacionDetalles",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_CotizacionDetalles_ADQ_Cotizaciones_CotizacionId",

                        column:
                            x => x.CotizacionId,

                        principalTable:
                            "ADQ_Cotizaciones",

                        principalColumn:
                            "Id",

                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // ÍNDICES
            // =========================================================

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_CotizacionAdjuntos_CotizacionId",

                table:
                    "ADQ_CotizacionAdjuntos",

                column:
                    "CotizacionId"
            );


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_CotizacionDetalles_CotizacionId",

                table:
                    "ADQ_CotizacionDetalles",

                column:
                    "CotizacionId"
            );


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Cotizaciones_SolicitudId",

                table:
                    "ADQ_Cotizaciones",

                column:
                    "SolicitudId"
            );
        }


        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // ELIMINAR TABLAS DE COTIZACIONES
            // =========================================================

            migrationBuilder.DropTable(
                name:
                    "ADQ_CotizacionAdjuntos"
            );


            migrationBuilder.DropTable(
                name:
                    "ADQ_CotizacionDetalles"
            );


            migrationBuilder.DropTable(
                name:
                    "ADQ_Cotizaciones"
            );
        }
    }
}