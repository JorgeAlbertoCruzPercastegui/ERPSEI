using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVacacionesSiguientePeriodo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DescuentoSiguientePeriodoAplicado",
                table: "SolicitudesVacaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DiasPendientesSiguientePeriodo",
                table: "SolicitudesVacaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "EsVacacionSiguientePeriodo",
                table: "SolicitudesVacaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAplicacionDescuentoSiguientePeriodo",
                table: "SolicitudesVacaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaProgramadaDescuentoSiguientePeriodo",
                table: "SolicitudesVacaciones",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescuentoSiguientePeriodoAplicado",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "DiasPendientesSiguientePeriodo",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "EsVacacionSiguientePeriodo",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "FechaAplicacionDescuentoSiguientePeriodo",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "FechaProgramadaDescuentoSiguientePeriodo",
                table: "SolicitudesVacaciones");
        }
    }
}