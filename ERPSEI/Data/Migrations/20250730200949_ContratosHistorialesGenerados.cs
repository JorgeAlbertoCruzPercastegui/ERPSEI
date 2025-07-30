using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ContratosHistorialesGenerados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorialContratoGenerados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioGenerador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpresaContratoId = table.Column<int>(type: "int", nullable: false),
                    ClienteContratoId = table.Column<int>(type: "int", nullable: false),
                    NumeroContrato = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArchivoGenerado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialContratoGenerados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialContratoGenerados_ClienteContratos_ClienteContratoId",
                        column: x => x.ClienteContratoId,
                        principalTable: "ClienteContratos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialContratoGenerados_EmpresaContratos_EmpresaContratoId",
                        column: x => x.EmpresaContratoId,
                        principalTable: "EmpresaContratos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistorialContratoGenerados_ClienteContratoId",
                table: "HistorialContratoGenerados",
                column: "ClienteContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialContratoGenerados_EmpresaContratoId",
                table: "HistorialContratoGenerados",
                column: "EmpresaContratoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialContratoGenerados");
        }
    }
}
