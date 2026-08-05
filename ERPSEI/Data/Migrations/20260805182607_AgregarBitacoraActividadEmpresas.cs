using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarBitacoraActividadEmpresas : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EB_BitacoraEmpresas",
                columns: table => new
                {
                    Id = table.Column<long>(
                        type: "bigint",
                        nullable: false
                    )
                    .Annotation(
                        "SqlServer:Identity",
                        "1, 1"
                    ),

                    EmpresaId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    Accion = table.Column<string>(
                        type: "nvarchar(80)",
                        maxLength: 80,
                        nullable: false
                    ),

                    UsuarioId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    NombreUsuario = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: false
                    ),

                    FechaEvento = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"
                    ),

                    DireccionIp = table.Column<string>(
                        type: "nvarchar(64)",
                        maxLength: 64,
                        nullable: true
                    ),

                    Navegador = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: true
                    ),

                    Exitoso = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true
                    ),

                    Detalle = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: true
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EB_BitacoraEmpresas",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_EB_BitacoraEmpresas_EB_Empresas_EmpresaId",
                        column:
                            x => x.EmpresaId,
                        principalTable:
                            "EB_Empresas",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraEmpresas_Accion_FechaEvento",
                table:
                    "EB_BitacoraEmpresas",
                columns:
                    new[]
                    {
                        "Accion",
                        "FechaEvento"
                    }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraEmpresas_EmpresaId",
                table:
                    "EB_BitacoraEmpresas",
                column:
                    "EmpresaId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraEmpresas_FechaEvento",
                table:
                    "EB_BitacoraEmpresas",
                column:
                    "FechaEvento"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_BitacoraEmpresas_UsuarioId",
                table:
                    "EB_BitacoraEmpresas",
                column:
                    "UsuarioId"
            );
        }

        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name:
                    "EB_BitacoraEmpresas"
            );
        }
    }
}