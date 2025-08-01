using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class SubTipoContrato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubTiposContrato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deshabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TipoContratoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTiposContrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubTiposContrato_TipoContratos_TipoContratoId",
                        column: x => x.TipoContratoId,
                        principalTable: "TipoContratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SubTiposContrato",
                columns: new[] { "Id", "Descripcion", "Deshabilitado", "Nombre", "TipoContratoId" },
                values: new object[,]
                {
                    { 1, "", false, "Servicios de diseño de presentaciones", 3 },
                    { 2, "", false, "Servicios de evaluación de clientes y proveedores", 3 },
                    { 3, "", false, "Servicios profesionales", 3 },
                    { 4, "", false, "Servicios profesionales independientes", 3 },
                    { 5, "", false, "Servicios (“El Contrato”)", 3 },
                    { 6, "", false, "Servicios profesionales de asesoria en inversiones", 3 },
                    { 7, "", false, "Servicios profesionales de mantenimiento de software", 3 },
                    { 8, "", false, "Servicios profesionales de integración de expedientes para licitaciones", 3 },
                    { 9, "", false, "Servicios profesionales de asesoria legal", 3 },
                    { 10, "", false, "Asesoria Financiera", 2 },
                    { 11, "", false, "Asesoria en Recursos Humanos", 2 },
                    { 12, "", false, "Asesoría financiera y revisión fiscal", 2 },
                    { 13, "", false, "Capacitación y asesoria por la venta a clientes", 2 }
                });

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "", "Asesoría" });

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Prestación de servicios profesionales o técnicos", "Servicios" });

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Contrato por uso de marca registrada", "Uso de Marca" });

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Arrendamiento de activos generales", "Arrendamiento Act." });

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Arrendamiento de tecnología e infraestructura", "Arrendamiento TI" });

            migrationBuilder.InsertData(
                table: "TipoContratos",
                columns: new[] { "Id", "Descripcion", "Deshabilitado", "Nombre" },
                values: new object[] { 7, "Arrendamiento de oficinas físicas", true, "Arrendamiento Ofi." });

            migrationBuilder.CreateIndex(
                name: "IX_SubTiposContrato_TipoContratoId",
                table: "SubTiposContrato",
                column: "TipoContratoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubTiposContrato");

            migrationBuilder.DeleteData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Prestación de servicios profesionales o técnicos", "Servicios" });

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Contrato por uso de marca registrada", "Uso de Marca" });

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Arrendamiento de activos generales", "Arrendamiento Act." });

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Arrendamiento de tecnología e infraestructura", "Arrendamiento TI" });

            migrationBuilder.UpdateData(
                table: "TipoContratos",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Arrendamiento de oficinas físicas", "Arrendamiento Ofi." });
        }
    }
}
