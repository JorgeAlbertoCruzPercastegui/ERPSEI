using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class RelacionEmpleadoJefeAusencias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JefeDirectoEmpleadoId",
                table: "Ausencias",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 14, 10, 13, 63, DateTimeKind.Local).AddTicks(116));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 14, 10, 13, 63, DateTimeKind.Local).AddTicks(134));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 14, 10, 13, 63, DateTimeKind.Local).AddTicks(136));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 14, 10, 13, 63, DateTimeKind.Local).AddTicks(137));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 14, 10, 13, 63, DateTimeKind.Local).AddTicks(139));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 14, 10, 13, 63, DateTimeKind.Local).AddTicks(141));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 14, 10, 13, 63, DateTimeKind.Local).AddTicks(143));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 14, 10, 13, 63, DateTimeKind.Local).AddTicks(145));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 14, 10, 13, 63, DateTimeKind.Local).AddTicks(146));

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_JefeId",
                table: "Empleados",
                column: "JefeId");

            migrationBuilder.CreateIndex(
                name: "IX_Ausencias_JefeDirectoEmpleadoId",
                table: "Ausencias",
                column: "JefeDirectoEmpleadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ausencias_Empleados_JefeDirectoEmpleadoId",
                table: "Ausencias",
                column: "JefeDirectoEmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_Empleados_JefeId",
                table: "Empleados",
                column: "JefeId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ausencias_Empleados_JefeDirectoEmpleadoId",
                table: "Ausencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_Empleados_JefeId",
                table: "Empleados");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_JefeId",
                table: "Empleados");

            migrationBuilder.DropIndex(
                name: "IX_Ausencias_JefeDirectoEmpleadoId",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "JefeDirectoEmpleadoId",
                table: "Ausencias");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 36, 5, 231, DateTimeKind.Local).AddTicks(792));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 36, 5, 231, DateTimeKind.Local).AddTicks(814));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 36, 5, 231, DateTimeKind.Local).AddTicks(817));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 36, 5, 231, DateTimeKind.Local).AddTicks(819));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 36, 5, 231, DateTimeKind.Local).AddTicks(822));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 36, 5, 231, DateTimeKind.Local).AddTicks(825));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 36, 5, 231, DateTimeKind.Local).AddTicks(827));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 36, 5, 231, DateTimeKind.Local).AddTicks(830));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 20, 12, 36, 5, 231, DateTimeKind.Local).AddTicks(833));
        }
    }
}
