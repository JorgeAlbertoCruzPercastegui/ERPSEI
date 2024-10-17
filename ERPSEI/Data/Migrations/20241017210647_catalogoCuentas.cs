using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class catalogoCuentas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CuentaContableSubtipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaContableSubtipos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CuentaContableTipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaContableTipos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CuentasContables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cuenta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RFC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmpresaId = table.Column<int>(type: "int", nullable: true),
                    TipoId = table.Column<int>(type: "int", nullable: true),
                    SubtipoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasContables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasContables_CuentaContableSubtipos_SubtipoId",
                        column: x => x.SubtipoId,
                        principalTable: "CuentaContableSubtipos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CuentasContables_CuentaContableTipos_TipoId",
                        column: x => x.TipoId,
                        principalTable: "CuentaContableTipos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CuentasContables_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "CuentaContableSubtipos",
                columns: new[] { "Id", "Clave", "Descripcion" },
                values: new object[,]
                {
                    { 1, "C", "Cliente" },
                    { 2, "G", "Gasto" },
                    { 3, "I", "IVA" },
                    { 4, "P", "Proveedor" }
                });

            migrationBuilder.InsertData(
                table: "CuentaContableTipos",
                columns: new[] { "Id", "Clave", "Descripcion" },
                values: new object[,]
                {
                    { 1, "E", "Egreso" },
                    { 2, "I", "Ingreso" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_EmpresaId",
                table: "CuentasContables",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_SubtipoId",
                table: "CuentasContables",
                column: "SubtipoId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasContables_TipoId",
                table: "CuentasContables",
                column: "TipoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuentasContables");

            migrationBuilder.DropTable(
                name: "CuentaContableSubtipos");

            migrationBuilder.DropTable(
                name: "CuentaContableTipos");
        }
    }
}
