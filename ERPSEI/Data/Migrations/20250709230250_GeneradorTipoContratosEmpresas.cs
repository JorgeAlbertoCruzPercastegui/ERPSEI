using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class GeneradorTipoContratosEmpresas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipoContratos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deshabilitado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoContratos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TipoContratos",
                columns: new[] { "Id", "Descripcion", "Deshabilitado", "Nombre" },
                values: new object[,]
                {
                    { 1, "Contratos de tipo asimilados a salarios", true, "Asimilados" },
                    { 2, "Prestación de servicios profesionales o técnicos", true, "Servicios" },
                    { 3, "Contrato por uso de marca registrada", true, "Uso de Marca" },
                    { 4, "Arrendamiento de activos generales", true, "Arrendamiento Act." },
                    { 5, "Arrendamiento de tecnología e infraestructura", true, "Arrendamiento TI" },
                    { 6, "Arrendamiento de oficinas físicas", true, "Arrendamiento Ofi." }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TipoContratos");
        }
    }
}
