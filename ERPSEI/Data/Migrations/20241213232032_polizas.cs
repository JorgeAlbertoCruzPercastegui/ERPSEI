using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class polizas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GruposPolizas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UsuarioCreadorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UsuarioModificadorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaHoraCreacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaHoraModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumeroImpresion = table.Column<int>(type: "int", nullable: false),
                    Deshabilitado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposPolizas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GruposPolizas_AspNetUsers_UsuarioCreadorId",
                        column: x => x.UsuarioCreadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GruposPolizas_AspNetUsers_UsuarioModificadorId",
                        column: x => x.UsuarioModificadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PolizasTipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deshabilitado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolizasTipos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VPolizas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    TipoId = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Concepto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VPolizas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VPolizas_GruposPolizas_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "GruposPolizas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VPolizas_PolizasTipos_TipoId",
                        column: x => x.TipoId,
                        principalTable: "PolizasTipos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PolizasDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    PolizaId = table.Column<int>(type: "int", nullable: false),
                    CuentaId = table.Column<int>(type: "int", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Debe = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: false),
                    Haber = table.Column<decimal>(type: "decimal(24,6)", precision: 24, scale: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolizasDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolizasDetalles_CuentasContables_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "CuentasContables",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PolizasDetalles_VPolizas_PolizaId",
                        column: x => x.PolizaId,
                        principalTable: "VPolizas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GruposPolizas_UsuarioCreadorId",
                table: "GruposPolizas",
                column: "UsuarioCreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_GruposPolizas_UsuarioModificadorId",
                table: "GruposPolizas",
                column: "UsuarioModificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PolizasDetalles_CuentaId",
                table: "PolizasDetalles",
                column: "CuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_PolizasDetalles_PolizaId",
                table: "PolizasDetalles",
                column: "PolizaId");

            migrationBuilder.CreateIndex(
                name: "IX_VPolizas_GrupoId",
                table: "VPolizas",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_VPolizas_TipoId",
                table: "VPolizas",
                column: "TipoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PolizasDetalles");

            migrationBuilder.DropTable(
                name: "VPolizas");

            migrationBuilder.DropTable(
                name: "GruposPolizas");

            migrationBuilder.DropTable(
                name: "PolizasTipos");
        }
    }
}
