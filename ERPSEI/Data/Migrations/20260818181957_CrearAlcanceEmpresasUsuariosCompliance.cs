using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearAlcanceEmpresasUsuariosCompliance : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder)
        {
            /*
             * ==========================================================
             * ALCANCE DE EMPRESAS POR USUARIO
             * ==========================================================
             *
             * Esta tabla indica si un usuario tiene activada
             * la restricción por empresas.
             *
             * Si RestringirEmpresas = false:
             * conserva el comportamiento actual.
             *
             * Si RestringirEmpresas = true:
             * únicamente podrá acceder a las empresas asignadas
             * en EB_PermisosComplianceEmpresasUsuario.
             * ==========================================================
             */
            migrationBuilder.CreateTable(
                name: "EB_AlcanceComplianceUsuarios",
                columns: table => new
                {
                    UsuarioId =
                        table.Column<string>(
                            type: "nvarchar(450)",
                            maxLength: 450,
                            nullable: false
                        ),

                    RestringirEmpresas =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),

                    FechaCreacion =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: false
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
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EB_AlcanceComplianceUsuarios",
                        x => x.UsuarioId
                    );
                }
            );

            /*
             * ==========================================================
             * EMPRESAS PERMITIDAS POR USUARIO
             * ==========================================================
             *
             * EmpresaId corresponde a Empresa.Id
             * del catálogo maestro Empresas.
             *
             * NO corresponde a EbEmpresa.Id.
             * ==========================================================
             */
            migrationBuilder.CreateTable(
                name:
                    "EB_PermisosComplianceEmpresasUsuario",

                columns: table => new
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

                    EmpresaId =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    FechaCreacion =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: false
                        ),

                    UsuarioCreacionId =
                        table.Column<string>(
                            type: "nvarchar(450)",
                            maxLength: 450,
                            nullable: true
                        )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_EB_PermisosComplianceEmpresasUsuario",
                        x => x.Id
                    );
                }
            );

            /*
             * ==========================================================
             * EVITAR EMPRESA DUPLICADA PARA EL MISMO USUARIO
             * ==========================================================
             *
             * Ejemplo inválido:
             *
             * Usuario A - Empresa 1
             * Usuario A - Empresa 1
             *
             * El índice único impide duplicados.
             * ==========================================================
             */
            migrationBuilder.CreateIndex(
                name:
                    "IX_EB_PermisosComplianceEmpresasUsuario_UsuarioId_EmpresaId",

                table:
                    "EB_PermisosComplianceEmpresasUsuario",

                columns: new[]
                {
                    "UsuarioId",
                    "EmpresaId"
                },

                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            /*
             * ==========================================================
             * ELIMINAR TABLAS DE ALCANCE
             * ==========================================================
             *
             * Primero eliminamos la tabla dependiente y después
             * la configuración general.
             * ==========================================================
             */
            migrationBuilder.DropTable(
                name:
                    "EB_PermisosComplianceEmpresasUsuario"
            );

            migrationBuilder.DropTable(
                name:
                    "EB_AlcanceComplianceUsuarios"
            );
        }
    }
}