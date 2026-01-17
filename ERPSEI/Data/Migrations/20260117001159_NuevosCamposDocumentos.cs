using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class NuevosCamposDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstatusDocumentoId",
                table: "Documentos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "NombreArchivo",
                table: "Documentos",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Documentos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsable",
                table: "Documentos",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RutaArchivo",
                table: "Documentos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ubicacion",
                table: "Documentos",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8447));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8478));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8482));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8486));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8490));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8493));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8497));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8501));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8505));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 16, 18, 11, 49, 421, DateTimeKind.Local).AddTicks(8508));

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_AreaId_TipoDocumentoId_EstatusDocumentoId_Activo",
                table: "Documentos",
                columns: new[] { "AreaId", "TipoDocumentoId", "EstatusDocumentoId", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_EstatusDocumentoId",
                table: "Documentos",
                column: "EstatusDocumentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documentos_EstatusDocumento_EstatusDocumentoId",
                table: "Documentos",
                column: "EstatusDocumentoId",
                principalTable: "EstatusDocumento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documentos_EstatusDocumento_EstatusDocumentoId",
                table: "Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_AreaId_TipoDocumentoId_EstatusDocumentoId_Activo",
                table: "Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Documentos_EstatusDocumentoId",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "EstatusDocumentoId",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "NombreArchivo",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "RutaArchivo",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Ubicacion",
                table: "Documentos");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(704));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(722));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(723));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(724));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(726));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(727));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(728));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 8,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(729));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(730));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 1, 12, 14, 12, 48, 879, DateTimeKind.Local).AddTicks(732));
        }
    }
}
