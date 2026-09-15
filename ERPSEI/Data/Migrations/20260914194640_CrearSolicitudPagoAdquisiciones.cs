using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearSolicitudPagoAdquisiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ADQ_SolicitudesPago",
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

                    SolicitudId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    AprobacionPresupuestalId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    CotizacionId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    Compania = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: false
                    ),

                    AreaSolicitante = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: false
                    ),

                    Moneda = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),

                    FormaPago = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),

                    ConceptoPago = table.Column<string>(
                        type: "nvarchar(max)",
                        maxLength: 5000,
                        nullable: false
                    ),

                    NombreProveedor = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: false
                    ),

                    Banco = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: true
                    ),

                    Cuenta = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),

                    ClabeInterbancaria = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),

                    ComprobanteAdjunto = table.Column<bool>(
                        type: "bit",
                        nullable: false
                    ),

                    Subtotal = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false
                    ),

                    Iva = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false
                    ),

                    RetencionIva = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false
                    ),

                    RetencionIsr = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false
                    ),

                    OtrosImpuestos = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false
                    ),

                    OtrosServicios = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false
                    ),

                    Total = table.Column<decimal>(
                        type: "decimal(18,2)",
                        nullable: false
                    ),

                    TipoDocumentoSolicitud = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),

                    FechaSolicitud = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false
                    ),

                    FechaGeneracion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false
                    ),

                    UsuarioGeneracionId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true
                    ),

                    NombreArchivo = table.Column<string>(
                        type: "nvarchar(260)",
                        maxLength: 260,
                        nullable: true
                    ),

                    RutaArchivo = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),

                    HashArchivo = table.Column<string>(
                        type: "nvarchar(128)",
                        maxLength: 128,
                        nullable: true
                    ),

                    PdfGenerado = table.Column<bool>(
                        type: "bit",
                        nullable: false
                    ),

                    Eliminado = table.Column<bool>(
                        type: "bit",
                        nullable: false
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_SolicitudesPago",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_SolicitudesPago_ADQ_AprobacionesPresupuestales_AprobacionPresupuestalId",

                        column:
                            x => x.AprobacionPresupuestalId,

                        principalTable:
                            "ADQ_AprobacionesPresupuestales",

                        principalColumn:
                            "Id",

                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_SolicitudesPago_ADQ_Cotizaciones_CotizacionId",

                        column:
                            x => x.CotizacionId,

                        principalTable:
                            "ADQ_Cotizaciones",

                        principalColumn:
                            "Id",

                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_SolicitudesPago_ADQ_Solicitudes_SolicitudId",

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


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_SolicitudesPago_AprobacionPresupuestalId",

                table:
                    "ADQ_SolicitudesPago",

                column:
                    "AprobacionPresupuestalId"
            );


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_SolicitudesPago_CotizacionId",

                table:
                    "ADQ_SolicitudesPago",

                column:
                    "CotizacionId"
            );


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_SolicitudesPago_SolicitudId",

                table:
                    "ADQ_SolicitudesPago",

                column:
                    "SolicitudId",

                unique:
                    true
            );
        }


        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder
        )
        {
            migrationBuilder.DropTable(
                name:
                    "ADQ_SolicitudesPago"
            );
        }
    }
}