using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InsertEmpresaClienteContrato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpresaContratos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaConstitucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RazonSocial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DomicilioFiscal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RFC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoNotario = table.Column<int>(type: "int", nullable: true),
                    Notario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepresentanteLegal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaginaWeb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deshabilitado = table.Column<bool>(type: "bit", nullable: false),
                    TipoContratoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaContratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresaContratos_TipoContratos_TipoContratoId",
                        column: x => x.TipoContratoId,
                        principalTable: "TipoContratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClienteContratos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaConstitucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RazonSocial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DomicilioFiscal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RFC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoNotario = table.Column<int>(type: "int", nullable: true),
                    Notario = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepresentanteLegal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaginaWeb = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deshabilitado = table.Column<bool>(type: "bit", nullable: false),
                    EmpresaContratoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClienteContratos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClienteContratos_EmpresaContratos_EmpresaContratoId",
                        column: x => x.EmpresaContratoId,
                        principalTable: "EmpresaContratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClienteContratos_EmpresaContratoId",
                table: "ClienteContratos",
                column: "EmpresaContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaContratos_TipoContratoId",
                table: "EmpresaContratos",
                column: "TipoContratoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClienteContratos");

            migrationBuilder.DropTable(
                name: "EmpresaContratos");
        }
    }
}
