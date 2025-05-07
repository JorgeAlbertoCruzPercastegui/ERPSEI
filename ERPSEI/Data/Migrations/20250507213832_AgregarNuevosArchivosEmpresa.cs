using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarNuevosArchivosEmpresa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "GrupoId",
                table: "VPolizas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "INE2");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "INE3");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: "RFC");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: "ComprobanteDomicilio");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: "Otro");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 8,
                column: "Description",
                value: "CER");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 9,
                column: "Description",
                value: "KEY");

            migrationBuilder.InsertData(
                table: "TipoArchivoEmpresa",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 10, "Logo" },
                    { 11, "HojaMembretada" },
                    { 12, "ActaConstitutiva" },
                    { 13, "Organigrama" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.AlterColumn<int>(
                name: "GrupoId",
                table: "VPolizas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "RFC");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "ComprobanteDomicilio");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 5,
                column: "Description",
                value: "Otro");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 6,
                column: "Description",
                value: "CER");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: "KEY");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 8,
                column: "Description",
                value: "Logo");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 9,
                column: "Description",
                value: "HojaMembretada");
        }
    }
}
