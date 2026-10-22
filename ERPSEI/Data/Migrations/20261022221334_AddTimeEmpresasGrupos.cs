
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeEmpresasGrupos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // TIME INTEGRATION - EMPRESAS
            // =====================================================

            migrationBuilder.CreateTable(
                name: "Empresas",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    Nombre = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true),

                    Tipo = table.Column<byte>(
                        type: "tinyint",
                        nullable: true),

                    Estatus = table.Column<byte>(
                        type: "tinyint",
                        nullable: true),

                    Incremental = table.Column<int>(
                        type: "int",
                        nullable: true),

                    GrupoId = table.Column<long>(
                        type: "bigint",
                        nullable: true),

                    Clave = table.Column<string>(
                        type: "nvarchar(7)",
                        maxLength: 7,
                        nullable: true),

                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Empresas",
                        x => x.Id);
                });

            // =====================================================
            // TIME INTEGRATION - GRUPOS
            // =====================================================

            migrationBuilder.CreateTable(
                name: "Grupos",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    Nombre = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: true),

                    Estatus = table.Column<byte>(
                        type: "tinyint",
                        nullable: true),

                    CreatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true),

                    UpdatedAt = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Grupos",
                        x => x.Id);
                });

            // =====================================================
            // ÍNDICES - EMPRESAS
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_GrupoId",
                schema: "TimeIntegracion",
                table: "Empresas",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Nombre",
                schema: "TimeIntegracion",
                table: "Empresas",
                column: "Nombre");

            // =====================================================
            // ÍNDICES - GRUPOS
            // =====================================================

            migrationBuilder.CreateIndex(
                name: "IX_Grupos_Nombre",
                schema: "TimeIntegracion",
                table: "Grupos",
                column: "Nombre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // REVERSIÓN EXCLUSIVA DE TIME INTEGRATION
            // =====================================================

            migrationBuilder.DropTable(
                name: "Empresas",
                schema: "TimeIntegracion");

            migrationBuilder.DropTable(
                name: "Grupos",
                schema: "TimeIntegracion");
        }
    }
}
