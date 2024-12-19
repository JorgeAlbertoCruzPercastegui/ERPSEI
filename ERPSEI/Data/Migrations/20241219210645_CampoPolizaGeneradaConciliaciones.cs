using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CampoPolizaGeneradaConciliaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PolizaGenerada",
                table: "Conciliaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 15,
                column: "Nombre",
                value: "BBVA");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 16,
                column: "Nombre",
                value: "Bx");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 17,
                column: "Nombre",
                value: "Cibanco");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 18,
                column: "Nombre",
                value: "Citibanamex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 19,
                column: "Nombre",
                value: "Eplata");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 20,
                column: "Nombre",
                value: "Fortuna");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 21,
                column: "Nombre",
                value: "HSBC");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 22,
                column: "Nombre",
                value: "Inbursa");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 23,
                column: "Nombre",
                value: "Intercam");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 24,
                column: "Nombre",
                value: "Invex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 25,
                column: "Nombre",
                value: "Jeeves");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 26,
                column: "Nombre",
                value: "KLU");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 27,
                column: "Nombre",
                value: "Konfio");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 28,
                column: "Nombre",
                value: "Mercado Pago");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 29,
                column: "Nombre",
                value: "Mifel");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 30,
                column: "Nombre",
                value: "Monex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 31,
                column: "Nombre",
                value: "Multiva");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 32,
                column: "Nombre",
                value: "PayMax");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 33,
                column: "Nombre",
                value: "Santander");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 34,
                column: "Nombre",
                value: "Scotiabank");

            migrationBuilder.InsertData(
                table: "Bancos",
                columns: new[] { "Id", "Deshabilitado", "Nombre" },
                values: new object[] { 35, false, "SantanderDig" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DropColumn(
                name: "PolizaGenerada",
                table: "Conciliaciones");

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

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 27,
                column: "Nombre",
                value: "Mercado Pago");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 28,
                column: "Nombre",
                value: "Mifel");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 29,
                column: "Nombre",
                value: "Monex");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 30,
                column: "Nombre",
                value: "Multiva");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 31,
                column: "Nombre",
                value: "PayMax");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 32,
                column: "Nombre",
                value: "Santander");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 33,
                column: "Nombre",
                value: "Scotiabank");

            migrationBuilder.UpdateData(
                table: "Bancos",
                keyColumn: "Id",
                keyValue: 34,
                column: "Nombre",
                value: "SantanderDig");
        }
    }
}
