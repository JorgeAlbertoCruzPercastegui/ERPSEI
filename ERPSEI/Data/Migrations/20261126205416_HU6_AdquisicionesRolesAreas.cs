using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    public partial class HU6_AdquisicionesRolesAreas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ADQ_RolesAreas",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false
                    )
                    .Annotation("SqlServer:Identity", "1, 1"),

                    RolId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    AreaId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    Activo = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true
                    ),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "SYSUTCDATETIME()"
                    ),

                    FechaModificacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true
                    ),

                    UsuarioModificacionId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true
                    ),

                    Eliminado = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    )
                },

                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_RolesAreas",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name: "FK_ADQ_RolesAreas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name: "FK_ADQ_RolesAreas_AspNetRoles_RolId",
                        column: x => x.RolId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_ADQ_RolesAreas_Alcance",
                table: "ADQ_RolesAreas",
                columns: new[]
                {
                    "AreaId",
                    "Activo",
                    "Eliminado"
                }
            );

            migrationBuilder.CreateIndex(
                name: "UX_ADQ_RolesAreas_Rol_Area",
                table: "ADQ_RolesAreas",
                columns: new[]
                {
                    "RolId",
                    "AreaId"
                },
                unique: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADQ_RolesAreas"
            );
        }
    }
}