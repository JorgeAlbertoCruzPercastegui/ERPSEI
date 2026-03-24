using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixSolicitudesVacaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstadoJefeDirecto",
                table: "SolicitudesVacaciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EstadoTH",
                table: "SolicitudesVacaciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRevisionJefeDirecto",
                table: "SolicitudesVacaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRevisionTH",
                table: "SolicitudesVacaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JefeDirectoEmpleadoId",
                table: "SolicitudesVacaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioJefeDirectoId",
                table: "SolicitudesVacaciones",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioTHId",
                table: "SolicitudesVacaciones",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8541));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8554));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8554));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8555));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8556));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8557));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8558));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8559));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 24, 13, 18, 21, 416, DateTimeKind.Local).AddTicks(8560));

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_JefeDirectoEmpleadoId",
                table: "SolicitudesVacaciones",
                column: "JefeDirectoEmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_UsuarioJefeDirectoId",
                table: "SolicitudesVacaciones",
                column: "UsuarioJefeDirectoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesVacaciones_UsuarioTHId",
                table: "SolicitudesVacaciones",
                column: "UsuarioTHId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesVacaciones_AspNetUsers_UsuarioJefeDirectoId",
                table: "SolicitudesVacaciones",
                column: "UsuarioJefeDirectoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesVacaciones_AspNetUsers_UsuarioTHId",
                table: "SolicitudesVacaciones",
                column: "UsuarioTHId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesVacaciones_Empleados_JefeDirectoEmpleadoId",
                table: "SolicitudesVacaciones",
                column: "JefeDirectoEmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesVacaciones_AspNetUsers_UsuarioJefeDirectoId",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesVacaciones_AspNetUsers_UsuarioTHId",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesVacaciones_Empleados_JefeDirectoEmpleadoId",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesVacaciones_JefeDirectoEmpleadoId",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesVacaciones_UsuarioJefeDirectoId",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesVacaciones_UsuarioTHId",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "EstadoJefeDirecto",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "EstadoTH",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "FechaRevisionJefeDirecto",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "FechaRevisionTH",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "JefeDirectoEmpleadoId",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioJefeDirectoId",
                table: "SolicitudesVacaciones");

            migrationBuilder.DropColumn(
                name: "UsuarioTHId",
                table: "SolicitudesVacaciones");

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
        }
    }
}
