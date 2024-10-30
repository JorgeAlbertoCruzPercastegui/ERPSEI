using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class complementosComprobante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NominasDeducciones_NominasDeduccionesDeducciones_DeduccionId",
                table: "NominasDeducciones");

            migrationBuilder.DropForeignKey(
                name: "FK_NominasPercepciones_NominasPercepcionePercepciones_PercepcionId",
                table: "NominasPercepciones");

            migrationBuilder.DropIndex(
                name: "IX_NominasDeducciones_DeduccionId",
                table: "NominasDeducciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NominasPercepcionePercepciones",
                table: "NominasPercepcionePercepciones");

            migrationBuilder.DropColumn(
                name: "DeduccionId",
                table: "NominasDeducciones");

            migrationBuilder.RenameTable(
                name: "NominasPercepcionePercepciones",
                newName: "NominasPercepcionesPercepciones");

            migrationBuilder.RenameColumn(
                name: "PercepcionId",
                table: "NominasPercepciones",
                newName: "SeparacionIndemnizacionId");

            migrationBuilder.RenameIndex(
                name: "IX_NominasPercepciones_PercepcionId",
                table: "NominasPercepciones",
                newName: "IX_NominasPercepciones_SeparacionIndemnizacionId");

            migrationBuilder.AlterColumn<string>(
                name: "TipoRegimen",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "TipoJornada",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "TipoContrato",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<string>(
                name: "PeriodicidadPago",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<string>(
                name: "Antigüedad",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Banco",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BancoSpecified",
                table: "NominasReceptores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CuentaBancaria",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Departamento",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicioRelLaboral",
                table: "NominasReceptores",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FechaInicioRelLaboralSpecified",
                table: "NominasReceptores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NumSeguridadSocial",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Puesto",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiesgoPuesto",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RiesgoPuestoSpecified",
                table: "NominasReceptores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SalarioBaseCotApor",
                table: "NominasReceptores",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "SalarioBaseCotAporSpecified",
                table: "NominasReceptores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "SalarioDiarioIntegrado",
                table: "NominasReceptores",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "SalarioDiarioIntegradoSpecified",
                table: "NominasReceptores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Sindicalizado",
                table: "NominasReceptores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SindicalizadoSpecified",
                table: "NominasReceptores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TipoJornadaSpecified",
                table: "NominasReceptores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "JubilacionPensionRetiroId",
                table: "NominasPercepciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalJubilacionPensionRetiro",
                table: "NominasPercepciones",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "TotalJubilacionPensionRetiroSpecified",
                table: "NominasPercepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalSeparacionIndemnizacion",
                table: "NominasPercepciones",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "TotalSeparacionIndemnizacionSpecified",
                table: "NominasPercepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TotalSueldosSpecified",
                table: "NominasPercepciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "TipoDeduccion",
                table: "NominasDeduccionesDeducciones",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<int>(
                name: "NominaDeduccionesId",
                table: "NominasDeduccionesDeducciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TotalImpuestosRetenidosSpecified",
                table: "NominasDeducciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TotalOtrasDeduccionesSpecified",
                table: "NominasDeducciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EmisorId",
                table: "Nominas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalOtrosPagos",
                table: "Nominas",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "TipoPercepcion",
                table: "NominasPercepcionesPercepciones",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AddColumn<int>(
                name: "AccionesOTitulosId",
                table: "NominasPercepcionesPercepciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NominaPercepcionesId",
                table: "NominasPercepcionesPercepciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NominasPercepcionesPercepciones",
                table: "NominasPercepcionesPercepciones",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "NominasEmisoresEntidadesSNCF",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrigenRecurso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MontoRecursoPropio = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    MontoRecursoPropioSpecified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasEmisoresEntidadesSNCF", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasIncapacidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiasIncapacidad = table.Column<int>(type: "int", nullable: false),
                    TipoIncapacidad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImporteMonetario = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImporteMonetarioSpecified = table.Column<bool>(type: "bit", nullable: false),
                    NominaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasIncapacidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NominasIncapacidades_Nominas_NominaId",
                        column: x => x.NominaId,
                        principalTable: "Nominas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NominasOtrosPagosCompensacionesSaldosAFavor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaldoAFavor = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    Año = table.Column<short>(type: "smallint", nullable: false),
                    RemanenteSalFav = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasOtrosPagosCompensacionesSaldosAFavor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasOtrosPagosSubsidiosAlEmpleo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubsidioCausado = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasOtrosPagosSubsidiosAlEmpleo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasPercepcionesJubilacionesPensionesRetiros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalUnaExhibicion = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalUnaExhibicionSpecified = table.Column<bool>(type: "bit", nullable: false),
                    TotalParcialidad = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalParcialidadSpecified = table.Column<bool>(type: "bit", nullable: false),
                    MontoDiario = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    MontoDiarioSpecified = table.Column<bool>(type: "bit", nullable: false),
                    IngresoAcumulable = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    IngresoNoAcumulable = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasPercepcionesJubilacionesPensionesRetiros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasPercepcionesPercepcionesAccionesOTitulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValorMercado = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    PrecioAlOtorgarse = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasPercepcionesPercepcionesAccionesOTitulos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasPercepcionesPercepcionesHorasExtras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Dias = table.Column<int>(type: "int", nullable: false),
                    TipoHoras = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HorasExtra = table.Column<int>(type: "int", nullable: false),
                    ImportePagado = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    NominaPercepcionesPercepcionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasPercepcionesPercepcionesHorasExtras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NominasPercepcionesPercepcionesHorasExtras_NominasPercepcionesPercepciones_NominaPercepcionesPercepcionId",
                        column: x => x.NominaPercepcionesPercepcionId,
                        principalTable: "NominasPercepcionesPercepciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NominasPercepcionesSeparacionesIndemnizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalPagado = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    NumAñosServicio = table.Column<int>(type: "int", nullable: false),
                    UltimoSueldoMensOrd = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    IngresoAcumulable = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    IngresoNoAcumulable = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasPercepcionesSeparacionesIndemnizaciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasReceptoresSubContrataciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RfcLabora = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PorcentajeTiempo = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    NominaReceptorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasReceptoresSubContrataciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NominasReceptoresSubContrataciones_NominasReceptores_NominaReceptorId",
                        column: x => x.NominaReceptorId,
                        principalTable: "NominasReceptores",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PagosPagosDoctosRelacionadosImpuestosDR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosPagosDoctosRelacionadosImpuestosDR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PagosPagosImpuestosP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosPagosImpuestosP", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PagosTotales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalRetencionesIVA = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalRetencionesIVASpecified = table.Column<bool>(type: "bit", nullable: false),
                    TotalRetencionesISR = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalRetencionesISRSpecified = table.Column<bool>(type: "bit", nullable: false),
                    TotalRetencionesIEPS = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalRetencionesIEPSSpecified = table.Column<bool>(type: "bit", nullable: false),
                    TotalTrasladosBaseIVA16 = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalTrasladosBaseIVA16Specified = table.Column<bool>(type: "bit", nullable: false),
                    TotalTrasladosImpuestoIVA16 = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalTrasladosImpuestoIVA16Specified = table.Column<bool>(type: "bit", nullable: false),
                    TotalTrasladosBaseIVA8 = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalTrasladosBaseIVA8Specified = table.Column<bool>(type: "bit", nullable: false),
                    TotalTrasladosImpuestoIVA8 = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalTrasladosImpuestoIVA8Specified = table.Column<bool>(type: "bit", nullable: false),
                    TotalTrasladosBaseIVA0 = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalTrasladosBaseIVA0Specified = table.Column<bool>(type: "bit", nullable: false),
                    TotalTrasladosImpuestoIVA0 = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalTrasladosImpuestoIVA0Specified = table.Column<bool>(type: "bit", nullable: false),
                    TotalTrasladosBaseIVAExento = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TotalTrasladosBaseIVAExentoSpecified = table.Column<bool>(type: "bit", nullable: false),
                    MontoTotalPagos = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosTotales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NominasEmisores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntidadSNCFId = table.Column<int>(type: "int", nullable: true),
                    Curp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistroPatronal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RfcPatronOrigen = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasEmisores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NominasEmisores_NominasEmisoresEntidadesSNCF_EntidadSNCFId",
                        column: x => x.EntidadSNCFId,
                        principalTable: "NominasEmisoresEntidadesSNCF",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NominasOtrosPagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubsidioAlEmpleoId = table.Column<int>(type: "int", nullable: true),
                    CompensacionSaldosAFavorId = table.Column<int>(type: "int", nullable: true),
                    TipoOtroPago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Clave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Concepto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Importe = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    NominaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NominasOtrosPagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NominasOtrosPagos_NominasOtrosPagosCompensacionesSaldosAFavor_CompensacionSaldosAFavorId",
                        column: x => x.CompensacionSaldosAFavorId,
                        principalTable: "NominasOtrosPagosCompensacionesSaldosAFavor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NominasOtrosPagos_NominasOtrosPagosSubsidiosAlEmpleo_SubsidioAlEmpleoId",
                        column: x => x.SubsidioAlEmpleoId,
                        principalTable: "NominasOtrosPagosSubsidiosAlEmpleo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NominasOtrosPagos_Nominas_NominaId",
                        column: x => x.NominaId,
                        principalTable: "Nominas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PagosPagosDoctosRelacionadosImpuestosDRRetencionesDR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseDR = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImpuestoDR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoFactorDR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TasaOCuotaDR = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImporteDR = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    PagosPagoDoctoRelacionadoImpuestosDRId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosPagosDoctosRelacionadosImpuestosDRRetencionesDR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosPagosDoctosRelacionadosImpuestosDRRetencionesDR_PagosPagosDoctosRelacionadosImpuestosDR_PagosPagoDoctoRelacionadoImpues~",
                        column: x => x.PagosPagoDoctoRelacionadoImpuestosDRId,
                        principalTable: "PagosPagosDoctosRelacionadosImpuestosDR",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PagosPagosDoctosRelacionadosImpuestosDRTrasladosDR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseDR = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImpuestoDR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoFactorDR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TasaOCuotaDR = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TasaOCuotaDRSpecified = table.Column<bool>(type: "bit", nullable: false),
                    ImporteDR = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImporteDRSpecified = table.Column<bool>(type: "bit", nullable: false),
                    PagosPagoDoctoRelacionadoImpuestosDRId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosPagosDoctosRelacionadosImpuestosDRTrasladosDR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosPagosDoctosRelacionadosImpuestosDRTrasladosDR_PagosPagosDoctosRelacionadosImpuestosDR_PagosPagoDoctoRelacionadoImpuesto~",
                        column: x => x.PagosPagoDoctoRelacionadoImpuestosDRId,
                        principalTable: "PagosPagosDoctosRelacionadosImpuestosDR",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PagosPagosImpuestosPRetencionesP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImpuestoP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImporteP = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    PagosPagoImpuestosPId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosPagosImpuestosPRetencionesP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosPagosImpuestosPRetencionesP_PagosPagosImpuestosP_PagosPagoImpuestosPId",
                        column: x => x.PagosPagoImpuestosPId,
                        principalTable: "PagosPagosImpuestosP",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PagosPagosImpuestosPTrasladosP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseP = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImpuestoP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoFactorP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TasaOCuotaP = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TasaOCuotaPSpecified = table.Column<bool>(type: "bit", nullable: false),
                    ImporteP = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImportePSpecified = table.Column<bool>(type: "bit", nullable: false),
                    PagosPagoImpuestosPId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosPagosImpuestosPTrasladosP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosPagosImpuestosPTrasladosP_PagosPagosImpuestosP_PagosPagoImpuestosPId",
                        column: x => x.PagosPagoImpuestosPId,
                        principalTable: "PagosPagosImpuestosP",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalesId = table.Column<int>(type: "int", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pagos_PagosTotales_TotalesId",
                        column: x => x.TotalesId,
                        principalTable: "PagosTotales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PagosPagos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImpuestosPId = table.Column<int>(type: "int", nullable: true),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FormaDePagoP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonedaP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoCambioP = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    TipoCambioPSpecified = table.Column<bool>(type: "bit", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    NumOperacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RfcEmisorCtaOrd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomBancoOrdExt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CtaOrdenante = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RfcEmisorCtaBen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CtaBeneficiario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoCadPago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TipoCadPagoSpecified = table.Column<bool>(type: "bit", nullable: false),
                    CertPago = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CadPago = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelloPago = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PagosId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosPagos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosPagos_PagosPagosImpuestosP_ImpuestosPId",
                        column: x => x.ImpuestosPId,
                        principalTable: "PagosPagosImpuestosP",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PagosPagos_Pagos_PagosId",
                        column: x => x.PagosId,
                        principalTable: "Pagos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PagosPagosDoctosRelacionados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImpuestosDRId = table.Column<int>(type: "int", nullable: true),
                    IdDocumento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Serie = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Folio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonedaDR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquivalenciaDR = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    EquivalenciaDRSpecified = table.Column<bool>(type: "bit", nullable: false),
                    NumParcialidad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImpSaldoAnt = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImpPagado = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ImpSaldoInsoluto = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    ObjetoImpDR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PagosPagoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosPagosDoctosRelacionados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosPagosDoctosRelacionados_PagosPagosDoctosRelacionadosImpuestosDR_ImpuestosDRId",
                        column: x => x.ImpuestosDRId,
                        principalTable: "PagosPagosDoctosRelacionadosImpuestosDR",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PagosPagosDoctosRelacionados_PagosPagos_PagosPagoId",
                        column: x => x.PagosPagoId,
                        principalTable: "PagosPagos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NominasPercepciones_JubilacionPensionRetiroId",
                table: "NominasPercepciones",
                column: "JubilacionPensionRetiroId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasDeduccionesDeducciones_NominaDeduccionesId",
                table: "NominasDeduccionesDeducciones",
                column: "NominaDeduccionesId");

            migrationBuilder.CreateIndex(
                name: "IX_Nominas_EmisorId",
                table: "Nominas",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasPercepcionesPercepciones_AccionesOTitulosId",
                table: "NominasPercepcionesPercepciones",
                column: "AccionesOTitulosId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasPercepcionesPercepciones_NominaPercepcionesId",
                table: "NominasPercepcionesPercepciones",
                column: "NominaPercepcionesId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasEmisores_EntidadSNCFId",
                table: "NominasEmisores",
                column: "EntidadSNCFId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasIncapacidades_NominaId",
                table: "NominasIncapacidades",
                column: "NominaId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasOtrosPagos_CompensacionSaldosAFavorId",
                table: "NominasOtrosPagos",
                column: "CompensacionSaldosAFavorId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasOtrosPagos_NominaId",
                table: "NominasOtrosPagos",
                column: "NominaId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasOtrosPagos_SubsidioAlEmpleoId",
                table: "NominasOtrosPagos",
                column: "SubsidioAlEmpleoId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasPercepcionesPercepcionesHorasExtras_NominaPercepcionesPercepcionId",
                table: "NominasPercepcionesPercepcionesHorasExtras",
                column: "NominaPercepcionesPercepcionId");

            migrationBuilder.CreateIndex(
                name: "IX_NominasReceptoresSubContrataciones_NominaReceptorId",
                table: "NominasReceptoresSubContrataciones",
                column: "NominaReceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_TotalesId",
                table: "Pagos",
                column: "TotalesId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosPagos_ImpuestosPId",
                table: "PagosPagos",
                column: "ImpuestosPId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosPagos_PagosId",
                table: "PagosPagos",
                column: "PagosId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosPagosDoctosRelacionados_ImpuestosDRId",
                table: "PagosPagosDoctosRelacionados",
                column: "ImpuestosDRId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosPagosDoctosRelacionados_PagosPagoId",
                table: "PagosPagosDoctosRelacionados",
                column: "PagosPagoId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosPagosDoctosRelacionadosImpuestosDRRetencionesDR_PagosPagoDoctoRelacionadoImpuestosDRId",
                table: "PagosPagosDoctosRelacionadosImpuestosDRRetencionesDR",
                column: "PagosPagoDoctoRelacionadoImpuestosDRId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosPagosDoctosRelacionadosImpuestosDRTrasladosDR_PagosPagoDoctoRelacionadoImpuestosDRId",
                table: "PagosPagosDoctosRelacionadosImpuestosDRTrasladosDR",
                column: "PagosPagoDoctoRelacionadoImpuestosDRId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosPagosImpuestosPRetencionesP_PagosPagoImpuestosPId",
                table: "PagosPagosImpuestosPRetencionesP",
                column: "PagosPagoImpuestosPId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosPagosImpuestosPTrasladosP_PagosPagoImpuestosPId",
                table: "PagosPagosImpuestosPTrasladosP",
                column: "PagosPagoImpuestosPId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nominas_NominasEmisores_EmisorId",
                table: "Nominas",
                column: "EmisorId",
                principalTable: "NominasEmisores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NominasDeduccionesDeducciones_NominasDeducciones_NominaDeduccionesId",
                table: "NominasDeduccionesDeducciones",
                column: "NominaDeduccionesId",
                principalTable: "NominasDeducciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NominasPercepciones_NominasPercepcionesJubilacionesPensionesRetiros_JubilacionPensionRetiroId",
                table: "NominasPercepciones",
                column: "JubilacionPensionRetiroId",
                principalTable: "NominasPercepcionesJubilacionesPensionesRetiros",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NominasPercepciones_NominasPercepcionesSeparacionesIndemnizaciones_SeparacionIndemnizacionId",
                table: "NominasPercepciones",
                column: "SeparacionIndemnizacionId",
                principalTable: "NominasPercepcionesSeparacionesIndemnizaciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NominasPercepcionesPercepciones_NominasPercepcionesPercepcionesAccionesOTitulos_AccionesOTitulosId",
                table: "NominasPercepcionesPercepciones",
                column: "AccionesOTitulosId",
                principalTable: "NominasPercepcionesPercepcionesAccionesOTitulos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NominasPercepcionesPercepciones_NominasPercepciones_NominaPercepcionesId",
                table: "NominasPercepcionesPercepciones",
                column: "NominaPercepcionesId",
                principalTable: "NominasPercepciones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nominas_NominasEmisores_EmisorId",
                table: "Nominas");

            migrationBuilder.DropForeignKey(
                name: "FK_NominasDeduccionesDeducciones_NominasDeducciones_NominaDeduccionesId",
                table: "NominasDeduccionesDeducciones");

            migrationBuilder.DropForeignKey(
                name: "FK_NominasPercepciones_NominasPercepcionesJubilacionesPensionesRetiros_JubilacionPensionRetiroId",
                table: "NominasPercepciones");

            migrationBuilder.DropForeignKey(
                name: "FK_NominasPercepciones_NominasPercepcionesSeparacionesIndemnizaciones_SeparacionIndemnizacionId",
                table: "NominasPercepciones");

            migrationBuilder.DropForeignKey(
                name: "FK_NominasPercepcionesPercepciones_NominasPercepcionesPercepcionesAccionesOTitulos_AccionesOTitulosId",
                table: "NominasPercepcionesPercepciones");

            migrationBuilder.DropForeignKey(
                name: "FK_NominasPercepcionesPercepciones_NominasPercepciones_NominaPercepcionesId",
                table: "NominasPercepcionesPercepciones");

            migrationBuilder.DropTable(
                name: "NominasEmisores");

            migrationBuilder.DropTable(
                name: "NominasIncapacidades");

            migrationBuilder.DropTable(
                name: "NominasOtrosPagos");

            migrationBuilder.DropTable(
                name: "NominasPercepcionesJubilacionesPensionesRetiros");

            migrationBuilder.DropTable(
                name: "NominasPercepcionesPercepcionesAccionesOTitulos");

            migrationBuilder.DropTable(
                name: "NominasPercepcionesPercepcionesHorasExtras");

            migrationBuilder.DropTable(
                name: "NominasPercepcionesSeparacionesIndemnizaciones");

            migrationBuilder.DropTable(
                name: "NominasReceptoresSubContrataciones");

            migrationBuilder.DropTable(
                name: "PagosPagosDoctosRelacionados");

            migrationBuilder.DropTable(
                name: "PagosPagosDoctosRelacionadosImpuestosDRRetencionesDR");

            migrationBuilder.DropTable(
                name: "PagosPagosDoctosRelacionadosImpuestosDRTrasladosDR");

            migrationBuilder.DropTable(
                name: "PagosPagosImpuestosPRetencionesP");

            migrationBuilder.DropTable(
                name: "PagosPagosImpuestosPTrasladosP");

            migrationBuilder.DropTable(
                name: "NominasEmisoresEntidadesSNCF");

            migrationBuilder.DropTable(
                name: "NominasOtrosPagosCompensacionesSaldosAFavor");

            migrationBuilder.DropTable(
                name: "NominasOtrosPagosSubsidiosAlEmpleo");

            migrationBuilder.DropTable(
                name: "PagosPagos");

            migrationBuilder.DropTable(
                name: "PagosPagosDoctosRelacionadosImpuestosDR");

            migrationBuilder.DropTable(
                name: "PagosPagosImpuestosP");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropTable(
                name: "PagosTotales");

            migrationBuilder.DropIndex(
                name: "IX_NominasPercepciones_JubilacionPensionRetiroId",
                table: "NominasPercepciones");

            migrationBuilder.DropIndex(
                name: "IX_NominasDeduccionesDeducciones_NominaDeduccionesId",
                table: "NominasDeduccionesDeducciones");

            migrationBuilder.DropIndex(
                name: "IX_Nominas_EmisorId",
                table: "Nominas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NominasPercepcionesPercepciones",
                table: "NominasPercepcionesPercepciones");

            migrationBuilder.DropIndex(
                name: "IX_NominasPercepcionesPercepciones_AccionesOTitulosId",
                table: "NominasPercepcionesPercepciones");

            migrationBuilder.DropIndex(
                name: "IX_NominasPercepcionesPercepciones_NominaPercepcionesId",
                table: "NominasPercepcionesPercepciones");

            migrationBuilder.DropColumn(
                name: "Antigüedad",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "Banco",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "BancoSpecified",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "CuentaBancaria",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "Departamento",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "FechaInicioRelLaboral",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "FechaInicioRelLaboralSpecified",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "NumSeguridadSocial",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "Puesto",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "RiesgoPuesto",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "RiesgoPuestoSpecified",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "SalarioBaseCotApor",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "SalarioBaseCotAporSpecified",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "SalarioDiarioIntegrado",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "SalarioDiarioIntegradoSpecified",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "Sindicalizado",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "SindicalizadoSpecified",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "TipoJornadaSpecified",
                table: "NominasReceptores");

            migrationBuilder.DropColumn(
                name: "JubilacionPensionRetiroId",
                table: "NominasPercepciones");

            migrationBuilder.DropColumn(
                name: "TotalJubilacionPensionRetiro",
                table: "NominasPercepciones");

            migrationBuilder.DropColumn(
                name: "TotalJubilacionPensionRetiroSpecified",
                table: "NominasPercepciones");

            migrationBuilder.DropColumn(
                name: "TotalSeparacionIndemnizacion",
                table: "NominasPercepciones");

            migrationBuilder.DropColumn(
                name: "TotalSeparacionIndemnizacionSpecified",
                table: "NominasPercepciones");

            migrationBuilder.DropColumn(
                name: "TotalSueldosSpecified",
                table: "NominasPercepciones");

            migrationBuilder.DropColumn(
                name: "NominaDeduccionesId",
                table: "NominasDeduccionesDeducciones");

            migrationBuilder.DropColumn(
                name: "TotalImpuestosRetenidosSpecified",
                table: "NominasDeducciones");

            migrationBuilder.DropColumn(
                name: "TotalOtrasDeduccionesSpecified",
                table: "NominasDeducciones");

            migrationBuilder.DropColumn(
                name: "EmisorId",
                table: "Nominas");

            migrationBuilder.DropColumn(
                name: "TotalOtrosPagos",
                table: "Nominas");

            migrationBuilder.DropColumn(
                name: "AccionesOTitulosId",
                table: "NominasPercepcionesPercepciones");

            migrationBuilder.DropColumn(
                name: "NominaPercepcionesId",
                table: "NominasPercepcionesPercepciones");

            migrationBuilder.RenameTable(
                name: "NominasPercepcionesPercepciones",
                newName: "NominasPercepcionePercepciones");

            migrationBuilder.RenameColumn(
                name: "SeparacionIndemnizacionId",
                table: "NominasPercepciones",
                newName: "PercepcionId");

            migrationBuilder.RenameIndex(
                name: "IX_NominasPercepciones_SeparacionIndemnizacionId",
                table: "NominasPercepciones",
                newName: "IX_NominasPercepciones_PercepcionId");

            migrationBuilder.AlterColumn<byte>(
                name: "TipoRegimen",
                table: "NominasReceptores",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "TipoJornada",
                table: "NominasReceptores",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "TipoContrato",
                table: "NominasReceptores",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "PeriodicidadPago",
                table: "NominasReceptores",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "TipoDeduccion",
                table: "NominasDeduccionesDeducciones",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeduccionId",
                table: "NominasDeducciones",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "TipoPercepcion",
                table: "NominasPercepcionePercepciones",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_NominasPercepcionePercepciones",
                table: "NominasPercepcionePercepciones",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NominasDeducciones_DeduccionId",
                table: "NominasDeducciones",
                column: "DeduccionId");

            migrationBuilder.AddForeignKey(
                name: "FK_NominasDeducciones_NominasDeduccionesDeducciones_DeduccionId",
                table: "NominasDeducciones",
                column: "DeduccionId",
                principalTable: "NominasDeduccionesDeducciones",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NominasPercepciones_NominasPercepcionePercepciones_PercepcionId",
                table: "NominasPercepciones",
                column: "PercepcionId",
                principalTable: "NominasPercepcionePercepciones",
                principalColumn: "Id");
        }
    }
}
