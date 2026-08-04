using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearPermisosComplianceUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name:
                    "EB_PermisosComplianceUsuarios",

                columns:
                    table => new
                    {
                        Id =
                            table.Column<int>(
                                type: "int",
                                nullable: false
                            )
                            .Annotation(
                                "SqlServer:Identity",
                                "1, 1"
                            ),

                        UsuarioId =
                            table.Column<string>(
                                type: "nvarchar(450)",
                                maxLength: 450,
                                nullable: false
                            ),

                        PuedeVisualizar =
                            table.Column<bool>(
                                type: "bit",
                                nullable: false,
                                defaultValue: false
                            ),

                        PuedeCrearCargar =
                            table.Column<bool>(
                                type: "bit",
                                nullable: false,
                                defaultValue: false
                            ),

                        PuedeModificar =
                            table.Column<bool>(
                                type: "bit",
                                nullable: false,
                                defaultValue: false
                            ),

                        PuedeEliminar =
                            table.Column<bool>(
                                type: "bit",
                                nullable: false,
                                defaultValue: false
                            ),

                        PuedeDescargar =
                            table.Column<bool>(
                                type: "bit",
                                nullable: false,
                                defaultValue: false
                            ),

                        FechaCreacion =
                            table.Column<DateTime>(
                                type: "datetime2",
                                nullable: false,
                                defaultValueSql:
                                    "GETDATE()"
                            ),

                        FechaModificacion =
                            table.Column<DateTime>(
                                type: "datetime2",
                                nullable: true
                            ),

                        UsuarioModificacionId =
                            table.Column<string>(
                                type: "nvarchar(450)",
                                maxLength: 450,
                                nullable: true
                            )
                    },

                constraints:
                    table =>
                    {
                        table.PrimaryKey(
                            "PK_EB_PermisosComplianceUsuarios",
                            x => x.Id
                        );
                    }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_PermisosComplianceUsuarios_FechaModificacion",

                table:
                    "EB_PermisosComplianceUsuarios",

                column:
                    "FechaModificacion"
            );

            migrationBuilder.CreateIndex(
                name:
                    "UX_EB_PermisosComplianceUsuarios_UsuarioId",

                table:
                    "EB_PermisosComplianceUsuarios",

                column:
                    "UsuarioId",

                unique:
                    true
            );
        }

        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name:
                    "EB_PermisosComplianceUsuarios"
            );
        }
    }
}