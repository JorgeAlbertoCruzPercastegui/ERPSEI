using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAusenciasModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoAusencia",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "TipoIncapacidad",
                table: "Ausencias");

            migrationBuilder.RenameColumn(
                name: "TipoRegistro",
                table: "Ausencias",
                newName: "TipoCaptura");

            migrationBuilder.RenameColumn(
                name: "DiasAporte",
                table: "Ausencias",
                newName: "Dias");

            migrationBuilder.AddColumn<int>(
                name: "TipoAusenciaId",
                table: "Ausencias",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoIncapacidadId",
                table: "Ausencias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TiposAusencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    ManejaHoras = table.Column<bool>(type: "bit", nullable: false),
                    ManejaDias = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAusencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposIncapacidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposIncapacidades", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TiposAusencias",
                columns: new[] { "Id", "Activo", "ManejaDias", "ManejaHoras", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, true, false, true, "Checada fuera de tiempo por instalación cerrada", 1 },
                    { 2, true, false, true, "Permiso llegada tardía", 2 },
                    { 3, true, false, true, "Permiso salida temprana", 3 },
                    { 4, true, true, false, "Permiso de ausencia", 4 },
                    { 5, true, false, true, "Permiso salida diligencia con regreso", 5 },
                    { 6, true, false, true, "Omisión de checada (no aplica para casos en donde se incumplan horarios laborales)", 6 },
                    { 7, true, true, false, "Permiso de paternidad", 7 },
                    { 8, true, false, true, "Cambio de hora de comida (especificar razón y horario tomado)", 8 },
                    { 9, true, true, false, "Permiso de ausencia por Fallecimiento de familiar", 9 },
                    { 10, true, true, false, "Permiso de ausencia médica justificada", 10 },
                    { 11, true, false, true, "Permiso diligencia sin regreso", 11 },
                    { 12, true, true, false, "Permiso de ausencia personal justificada", 12 },
                    { 13, true, true, false, "Permiso de ausencia por accidente justificado", 13 },
                    { 14, true, true, false, "Permiso para trabajar desde casa (HO)", 14 },
                    { 15, true, false, true, "Sin registro por falla de biométrico (sin luz y/o descompuesto)", 15 },
                    { 16, true, true, false, "Permiso sin goce de sueldo", 16 }
                });

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

            migrationBuilder.InsertData(
                table: "TiposIncapacidades",
                columns: new[] { "Id", "Activo", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, true, "Riesgo de trabajo", 1 },
                    { 2, true, "Enfermedad en general", 2 },
                    { 3, true, "Maternidad", 3 },
                    { 4, true, "Licencia por cuidados médicos de hijos diagnosticados con cáncer", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ausencias_TipoAusenciaId",
                table: "Ausencias",
                column: "TipoAusenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Ausencias_TipoIncapacidadId",
                table: "Ausencias",
                column: "TipoIncapacidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ausencias_TiposAusencias_TipoAusenciaId",
                table: "Ausencias",
                column: "TipoAusenciaId",
                principalTable: "TiposAusencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ausencias_TiposIncapacidades_TipoIncapacidadId",
                table: "Ausencias",
                column: "TipoIncapacidadId",
                principalTable: "TiposIncapacidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ausencias_TiposAusencias_TipoAusenciaId",
                table: "Ausencias");

            migrationBuilder.DropForeignKey(
                name: "FK_Ausencias_TiposIncapacidades_TipoIncapacidadId",
                table: "Ausencias");

            migrationBuilder.DropTable(
                name: "TiposAusencias");

            migrationBuilder.DropTable(
                name: "TiposIncapacidades");

            migrationBuilder.DropIndex(
                name: "IX_Ausencias_TipoAusenciaId",
                table: "Ausencias");

            migrationBuilder.DropIndex(
                name: "IX_Ausencias_TipoIncapacidadId",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "TipoAusenciaId",
                table: "Ausencias");

            migrationBuilder.DropColumn(
                name: "TipoIncapacidadId",
                table: "Ausencias");

            migrationBuilder.RenameColumn(
                name: "TipoCaptura",
                table: "Ausencias",
                newName: "TipoRegistro");

            migrationBuilder.RenameColumn(
                name: "Dias",
                table: "Ausencias",
                newName: "DiasAporte");

            migrationBuilder.AddColumn<string>(
                name: "TipoAusencia",
                table: "Ausencias",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoIncapacidad",
                table: "Ausencias",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 15, 38, 42, 707, DateTimeKind.Local).AddTicks(9039));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 15, 38, 42, 707, DateTimeKind.Local).AddTicks(9053));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 15, 38, 42, 707, DateTimeKind.Local).AddTicks(9055));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 4,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 15, 38, 42, 707, DateTimeKind.Local).AddTicks(9056));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 5,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 15, 38, 42, 707, DateTimeKind.Local).AddTicks(9058));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 6,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 15, 38, 42, 707, DateTimeKind.Local).AddTicks(9059));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 7,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 15, 38, 42, 707, DateTimeKind.Local).AddTicks(9060));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 9,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 15, 38, 42, 707, DateTimeKind.Local).AddTicks(9062));

            migrationBuilder.UpdateData(
                table: "TiposDocumento",
                keyColumn: "Id",
                keyValue: 10,
                column: "FechaCreacion",
                value: new DateTime(2026, 3, 19, 15, 38, 42, 707, DateTimeKind.Local).AddTicks(9063));
        }
    }
}
