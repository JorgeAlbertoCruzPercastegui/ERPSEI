using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeDepartamentoActividades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepartamentoActividades",
                schema: "TimeIntegracion",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false),

                    Nombre = table.Column<string>(
                        type: "nvarchar(191)",
                        maxLength: 191,
                        nullable: true),

                    DepartamentoId = table.Column<long>(
                        type: "bigint",
                        nullable: true),

                    Descripcion = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
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
                        "PK_DepartamentoActividades",
                        x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartamentoActividades_DepartamentoId",
                schema: "TimeIntegracion",
                table: "DepartamentoActividades",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Actividades_DepartamentoActividadId",
                schema: "TimeIntegracion",
                table: "Actividades",
                column: "DepartamentoActividadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Actividades_DepartamentoActividadId",
                schema: "TimeIntegracion",
                table: "Actividades");

            migrationBuilder.DropTable(
                name: "DepartamentoActividades",
                schema: "TimeIntegracion");
        }
    }
}