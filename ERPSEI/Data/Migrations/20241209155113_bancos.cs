using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class bancos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Autofin");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nombre",
                value: "Azteca");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nombre",
                value: "American Express");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 6,
                column: "Nombre",
                value: "Bancomer");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 7,
                column: "Nombre",
                value: "Bancoppel");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 8,
                column: "Nombre",
                value: "Banamex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 9,
                column: "Nombre",
                value: "Bankaool");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 10,
                column: "Nombre",
                value: "Banorte");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 11,
                column: "Nombre",
                value: "Banregio");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 12,
                column: "Nombre",
                value: "Bajio");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 13,
                column: "Nombre",
                value: "Banbajio");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 14,
                column: "Nombre",
                value: "Base");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 15,
                column: "Nombre",
                value: "Bx");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 16,
                column: "Nombre",
                value: "Cibanco");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 17,
                column: "Nombre",
                value: "Citibanamex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 18,
                column: "Nombre",
                value: "Eplata");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 19,
                column: "Nombre",
                value: "Fortuna");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 20,
                column: "Nombre",
                value: "HSBC");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 21,
                column: "Nombre",
                value: "Inbursa");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 22,
                column: "Nombre",
                value: "Intercam");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 23,
                column: "Nombre",
                value: "Invex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 24,
                column: "Nombre",
                value: "Jeeves");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 25,
                column: "Nombre",
                value: "KLU");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 26,
                column: "Nombre",
                value: "Konfio");

            migrationBuilder.InsertData(
                table: "Bancos",
                columns: new[] { "Id", "Deshabilitado", "Nombre" },
                values: new object[,]
                {
                    { 27, false, "Mercado Pago" },
                    { 28, false, "Mifel" },
                    { 29, false, "Monex" },
                    { 30, false, "Multiva" },
                    { 31, false, "PayMax" },
                    { 32, false, "Santander" },
                    { 33, false, "Scotiabank" },
                    { 34, false, "SantanderDig" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Azteca");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nombre",
                value: "American Express");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 5,
                column: "Nombre",
                value: "Bancomer");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 6,
                column: "Nombre",
                value: "Bancoppel");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 7,
                column: "Nombre",
                value: "Banorte");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 8,
                column: "Nombre",
                value: "Banregio");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 9,
                column: "Nombre",
                value: "Bajio");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 10,
                column: "Nombre",
                value: "Base");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 11,
                column: "Nombre",
                value: "Bx");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 12,
                column: "Nombre",
                value: "Cibanco");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 13,
                column: "Nombre",
                value: "Citibanamex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 14,
                column: "Nombre",
                value: "Fortuna");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 15,
                column: "Nombre",
                value: "HSBC");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 16,
                column: "Nombre",
                value: "Inbursa");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 17,
                column: "Nombre",
                value: "Intercam");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 18,
                column: "Nombre",
                value: "Invex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 19,
                column: "Nombre",
                value: "Jeeves");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 20,
                column: "Nombre",
                value: "Konfio");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 21,
                column: "Nombre",
                value: "Mercado Pago");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 22,
                column: "Nombre",
                value: "Mifel");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 23,
                column: "Nombre",
                value: "Monex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 24,
                column: "Nombre",
                value: "Multiva");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 25,
                column: "Nombre",
                value: "Santander");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 26,
                column: "Nombre",
                value: "Scotiabank");
        }
    }
}
