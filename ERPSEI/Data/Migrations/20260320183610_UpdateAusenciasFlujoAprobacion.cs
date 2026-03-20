using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAusenciasFlujoAprobacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Ausencias",
                newName: "EstadoTH");

            migrationBuilder.AddColumn<string>(
                name: "EstadoJefeDirecto",
                table: "Ausencias",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRevisionJefeDirecto",
                table: "Ausencias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRevisionTH",
                table: "Ausencias",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioJefeDirectoId",
                table: "Ausencias",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioTHId",
                table: "Ausencias",
                type: "nvarchar(450)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Ausencias_UsuarioJefeDirectoId",
                table: "Ausencias",
                column: "UsuarioJefeDirectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ausencias_UsuarioTHId",
                table: "Ausencias",
                column: "UsuarioTHId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ausencias_AspNetUsers_UsuarioJefeDirectoId",
                table: "Ausencias",
                column: "UsuarioJefeDirectoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ausencias_AspNetUsers_UsuarioTHId",
                table: "Ausencias",
                column: "UsuarioTHId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ausencias_AspNetUsers_UsuarioJefeDirectoId",
                table: "Ausencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Ausencias_AspNetUsers_UsuarioTHId",
                table: "Ausencias");

            migrationBuilder.DropIndex(
                name: "IX_Ausencias_UsuarioJefeDirectoId",
                table: "Ausencias");

            migrationBuilder.DropIndex(
                name: "IX_Ausencias_UsuarioTHId",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "EstadoJefeDirecto",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "FechaRevisionJefeDirecto",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "FechaRevisionTH",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "UsuarioJefeDirectoId",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "UsuarioTHId",
                table: "Ausencias");

            migrationBuilder.RenameColumn(
                name: "EstadoTH",
                table: "Ausencias",
                newName: "Estado");

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 16, 39, 45, 404, DateTimeKind.Local).AddTicks(8568));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 16, 39, 45, 404, DateTimeKind.Local).AddTicks(8583));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 16, 39, 45, 404, DateTimeKind.Local).AddTicks(8584));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 16, 39, 45, 404, DateTimeKind.Local).AddTicks(8585));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 16, 39, 45, 404, DateTimeKind.Local).AddTicks(8587));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 16, 39, 45, 404, DateTimeKind.Local).AddTicks(8588));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 16, 39, 45, 404, DateTimeKind.Local).AddTicks(8589));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 16, 39, 45, 404, DateTimeKind.Local).AddTicks(8591));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 16, 39, 45, 404, DateTimeKind.Local).AddTicks(8592));
        }
    }
}
