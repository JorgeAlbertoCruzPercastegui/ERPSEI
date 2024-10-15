using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ComprobantesFiscalesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UUID",
                table: "Comprobantes");

            migrationBuilder.AddColumn<int>(
                name: "ComplementoId",
                table: "Comprobantes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComprobantesAddendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesAddendas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComprobantesConceptosComplementosConceptos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesConceptosComplementosConceptos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasDeduccionesDeducciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Concepto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Importe = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TipoDeduccion = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasDeduccionesDeducciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasPercepcionePercepciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Concepto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImporteExento = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImporteGravado = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TipoPercepcion = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasPercepcionePercepciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasReceptores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaveEntFed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Curp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeriodicidadPago = table.Column<byte>(type: "tinyint", nullable: false),
                    TipoContrato = table.Column<byte>(type: "tinyint", nullable: false),
                    TipoJornada = table.Column<byte>(type: "tinyint", nullable: false),
                    TipoRegimen = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasReceptores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimbresFiscalesDigitales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaTimbrado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoCertificadoSAT = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    RfcProvCertif = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelloCFD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelloSAT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimbresFiscalesDigitales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasDeducciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeduccionId = table.Column<int>(type: "int", nullable: true),
                    TotalImpuestosRetenidos = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalOtrasDeducciones = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasDeducciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NominasDeducciones_NominasDeduccionesDeducciones_DeduccionId",
                        column: x => x.DeduccionId,
                        principalTable: "NominasDeduccionesDeducciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NominasPercepciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PercepcionId = table.Column<int>(type: "int", nullable: true),
                    TotalExento = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalGravado = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalSueldos = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasPercepciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NominasPercepciones_NominasPercepcionePercepciones_PercepcionId",
                        column: x => x.PercepcionId,
                        principalTable: "NominasPercepcionePercepciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Nominas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceptorId = table.Column<int>(type: "int", nullable: true),
                    PercepcionesId = table.Column<int>(type: "int", nullable: true),
                    DeduccionesId = table.Column<int>(type: "int", nullable: true),
                    Version = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TipoNomina = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicialPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinalPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumDiasPagados = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalPercepciones = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalDeducciones = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nominas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nominas_NominasDeducciones_DeduccionesId",
                        column: x => x.DeduccionesId,
                        principalTable: "NominasDeducciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Nominas_NominasPercepciones_PercepcionesId",
                        column: x => x.PercepcionesId,
                        principalTable: "NominasPercepciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Nominas_NominasReceptores_ReceptorId",
                        column: x => x.ReceptorId,
                        principalTable: "NominasReceptores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ComprobantesComplementos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NominaId = table.Column<int>(type: "int", nullable: true),
                    TimbreFiscalDigitalId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprobantesComplementos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprobantesComplementos_Nominas_NominaId",
                        column: x => x.NominaId,
                        principalTable: "Nominas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ComprobantesComplementos_TimbresFiscalesDigitales_TimbreFiscalDigitalId",
                        column: x => x.TimbreFiscalDigitalId,
                        principalTable: "TimbresFiscalesDigitales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comprobantes_ComplementoId",
                table: "Comprobantes",
                column: "ComplementoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesComplementos_NominaId",
                table: "ComprobantesComplementos",
                column: "NominaId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprobantesComplementos_TimbreFiscalDigitalId",
                table: "ComprobantesComplementos",
                column: "TimbreFiscalDigitalId");

            migrationBuilder.CreateIndex(
                name: "IX_Nominas_DeduccionesId",
                table: "Nominas",
                column: "DeduccionesId");

            migrationBuilder.CreateIndex(
                name: "IX_Nominas_PercepcionesId",
                table: "Nominas",
                column: "PercepcionesId");

            migrationBuilder.CreateIndex(
                name: "IX_Nominas_ReceptorId",
                table: "Nominas",
                column: "ReceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasDeducciones_DeduccionId",
                table: "NominasDeducciones",
                column: "DeduccionId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasPercepciones_PercepcionId",
                table: "NominasPercepciones",
                column: "PercepcionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comprobantes_ComprobantesComplementos_ComplementoId",
                table: "Comprobantes",
                column: "ComplementoId",
                principalTable: "ComprobantesComplementos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comprobantes_ComprobantesComplementos_ComplementoId",
                table: "Comprobantes");

            migrationBuilder.DropTable(
                name: "ComprobantesAddendas");

            migrationBuilder.DropTable(
                name: "ComprobantesComplementos");

            migrationBuilder.DropTable(
                name: "ComprobantesConceptosComplementosConceptos");

            migrationBuilder.DropTable(
                name: "Nominas");

            migrationBuilder.DropTable(
                name: "TimbresFiscalesDigitales");

            migrationBuilder.DropTable(
                name: "NominasDeducciones");

            migrationBuilder.DropTable(
                name: "NominasPercepciones");

            migrationBuilder.DropTable(
                name: "NominasReceptores");

            migrationBuilder.DropTable(
                name: "NominasDeduccionesDeducciones");

            migrationBuilder.DropTable(
                name: "NominasPercepcionePercepciones");

            migrationBuilder.DropIndex(
                name: "IX_Comprobantes_ComplementoId",
                table: "Comprobantes");

            migrationBuilder.DropColumn(
                name: "ComplementoId",
                table: "Comprobantes");

            migrationBuilder.AddColumn<string>(
                name: "UUID",
                table: "Comprobantes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
