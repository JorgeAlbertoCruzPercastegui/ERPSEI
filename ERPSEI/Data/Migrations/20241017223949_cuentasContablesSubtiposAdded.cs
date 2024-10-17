using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class cuentasContablesSubtiposAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Clave",
                value: "CL");

            migrationBuilder.UpdateData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 2,
                column: "Clave",
                value: "GA");

            migrationBuilder.UpdateData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Clave", "Descripcion" },
                values: new object[] { "VA", "Ventas al 16" });

            migrationBuilder.UpdateData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 4,
                column: "Clave",
                value: "PR");

            migrationBuilder.InsertData(
                table: "CuentaContableSubtipos",
                columns: new[] { "Id", "Clave", "Descripcion" },
                values: new object[,]
                {
                    { 5, "VB", "Ventas al 0" },
                    { 6, "VC", "Ventas Exentas" },
                    { 7, "IN", "I.V.A. No Cobrado" },
                    { 8, "IC", "I.V.A. Cobrado" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Clave",
                value: "C");

            migrationBuilder.UpdateData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 2,
                column: "Clave",
                value: "G");

            migrationBuilder.UpdateData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Clave", "Descripcion" },
                values: new object[] { "I", "IVA" });

            migrationBuilder.UpdateData(
                table: "CuentaContableSubtipos",
                keyColumn: "Id",
                keyValue: 4,
                column: "Clave",
                value: "P");
        }
    }
}
