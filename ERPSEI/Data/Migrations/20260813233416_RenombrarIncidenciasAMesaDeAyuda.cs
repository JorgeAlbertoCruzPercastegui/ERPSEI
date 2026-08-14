using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarIncidenciasAMesaDeAyuda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Modulos",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Nombre", "NombreNormalizado" },
                values: new object[] { "Mesa de Ayuda", "mesadeayuda" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Modulos",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "Nombre", "NombreNormalizado" },
                values: new object[] { "Incidencias", "incidencias" });
        }
    }
}