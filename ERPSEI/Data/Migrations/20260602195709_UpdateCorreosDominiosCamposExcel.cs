using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCorreosDominiosCamposExcel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Responsable",
                table: "CorreosDominios",
                newName: "Proveedor");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "CorreosDominios",
                newName: "PagWeb");

            migrationBuilder.RenameColumn(
                name: "Correo",
                table: "CorreosDominios",
                newName: "Estado");

            migrationBuilder.AddColumn<string>(
                name: "ContrasenaFiscal",
                table: "CorreosDominios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContrasenaOperaciones",
                table: "CorreosDominios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorreoFiscal",
                table: "CorreosDominios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CorreoOperaciones",
                table: "CorreosDominios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Costos",
                table: "CorreosDominios",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Empresa",
                table: "CorreosDominios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCaducacion",
                table: "CorreosDominios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 57, 7, 784, DateTimeKind.Local).AddTicks(3502));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 57, 7, 784, DateTimeKind.Local).AddTicks(3513));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 57, 7, 784, DateTimeKind.Local).AddTicks(3514));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 57, 7, 784, DateTimeKind.Local).AddTicks(3516));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 57, 7, 784, DateTimeKind.Local).AddTicks(3516));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 57, 7, 784, DateTimeKind.Local).AddTicks(3517));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 57, 7, 784, DateTimeKind.Local).AddTicks(3518));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 57, 7, 784, DateTimeKind.Local).AddTicks(3519));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 57, 7, 784, DateTimeKind.Local).AddTicks(3520));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContrasenaFiscal",
                table: "CorreosDominios");

            migrationBuilder.DropColumn(
                name: "ContrasenaOperaciones",
                table: "CorreosDominios");

            migrationBuilder.DropColumn(
                name: "CorreoFiscal",
                table: "CorreosDominios");

            migrationBuilder.DropColumn(
                name: "CorreoOperaciones",
                table: "CorreosDominios");

            migrationBuilder.DropColumn(
                name: "Costos",
                table: "CorreosDominios");

            migrationBuilder.DropColumn(
                name: "Empresa",
                table: "CorreosDominios");

            migrationBuilder.DropColumn(
                name: "FechaCaducacion",
                table: "CorreosDominios");

            migrationBuilder.RenameColumn(
                name: "Proveedor",
                table: "CorreosDominios",
                newName: "Responsable");

            migrationBuilder.RenameColumn(
                name: "PagWeb",
                table: "CorreosDominios",
                newName: "Descripcion");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "CorreosDominios",
                newName: "Correo");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 6, 4, 886, DateTimeKind.Local).AddTicks(4558));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 6, 4, 886, DateTimeKind.Local).AddTicks(4565));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 6, 4, 886, DateTimeKind.Local).AddTicks(4566));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 6, 4, 886, DateTimeKind.Local).AddTicks(4568));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 6, 4, 886, DateTimeKind.Local).AddTicks(4568));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 6, 4, 886, DateTimeKind.Local).AddTicks(4569));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 6, 4, 886, DateTimeKind.Local).AddTicks(4570));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 6, 4, 886, DateTimeKind.Local).AddTicks(4571));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 6, 2, 13, 6, 4, 886, DateTimeKind.Local).AddTicks(4572));
        }
    }
}
