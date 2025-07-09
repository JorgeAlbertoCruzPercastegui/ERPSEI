using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class extension_roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deshabilitado = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccesoModulo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PuedeConsultar = table.Column<int>(type: "int", nullable: false),
                    PuedeEditar = table.Column<int>(type: "int", nullable: false),
                    PuedeEliminar = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModuloId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccesoModulo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccesoModulo_AspNetRoles_RolId",
                        column: x => x.RolId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AccesoModulo_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Id", "Deshabilitado", "Nombre" },
                values: new object[,]
                {
                    { 1, 0, "Gestión de Talento" },
                    { 2, 0, "Usuarios" },
                    { 3, 0, "Puestos" },
                    { 4, 0, "Áreas" },
                    { 5, 0, "Subareas" },
                    { 6, 0, "Oficinas" },
                    { 7, 0, "Empresas" },
                    { 8, 0, "Orígenes" },
                    { 9, 0, "Niveles" },
                    { 10, 0, "Perfiles" },
                    { 11, 0, "Vacaciones" },
                    { 12, 0, "Incapacidades" },
                    { 13, 0, "Permisos" },
                    { 14, 0, "Prefacturas" },
                    { 15, 0, "Organigrama" },
                    { 16, 0, "Asistencia" },
                    { 17, 0, "GeneradorContrato" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccesoModulo_ModuloId",
                table: "AccesoModulo",
                column: "ModuloId");

            migrationBuilder.CreateIndex(
                name: "IX_AccesoModulo_RolId",
                table: "AccesoModulo",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccesoModulo");

            migrationBuilder.DropTable(
                name: "Modulos");
        }
    }
}
