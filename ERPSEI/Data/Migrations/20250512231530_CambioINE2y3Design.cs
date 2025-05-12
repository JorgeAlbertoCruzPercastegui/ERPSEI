using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CambioINE2y3Design : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 10,
                column: "Description",
                value: "INE2");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 11,
                column: "Description",
                value: "INE3");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 10,
                column: "Description",
                value: "Logo");

            migrationBuilder.UpdateData(
                table: "TipoArchivoEmpresa",
                keyColumn: "Id",
                keyValue: 11,
                column: "Description",
                value: "HojaMembretada");
        }
    }
}
