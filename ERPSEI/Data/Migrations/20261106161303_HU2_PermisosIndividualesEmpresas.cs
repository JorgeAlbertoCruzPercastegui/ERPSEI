
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class HU2_PermisosIndividualesEmpresas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ==========================================================
            // HU2 - PERMISOS INDIVIDUALES DE USUARIOS EN EMPRESAS
            // ==========================================================
            //
            // Permite configurar permisos específicos para usuarios
            // con rol "Administrador Empresas".
            //
            // Cada usuario podrá tener permisos diferenciados para:
            // - Visualizar empresas
            // - Crear empresas
            // - Editar empresas
            // - Cargar documentos
            // - Descargar documentos
            // - Desactivar empresas
            // - Editar información bancaria
            //
            // No modifica roles existentes ni otras tablas.
            // ==========================================================

            migrationBuilder.CreateTable(
                name: "EmpresasPermisosUsuariosHU2",
                columns: table => new
                {
                    UserId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false),

                    PuedeVisualizar = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    PuedeCrear = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    PuedeEditar = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    PuedeCargarDocumentos = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    PuedeDescargar = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    PuedeDesactivar = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    PuedeEditarBancos = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    FechaActualizacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    ActualizadoPorUserId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true)
                },
                constraints: table =>
                {
                    // Una única configuración de permisos por usuario.
                    table.PrimaryKey(
                        "PK_EmpresasPermisosUsuariosHU2",
                        x => x.UserId);

                    // Relación con los usuarios de ASP.NET Identity.
                    table.ForeignKey(
                        name: "FK_EmpresasPermisosUsuariosHU2_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ==========================================================
            // REVERSIÓN HU2 - PERMISOS INDIVIDUALES
            // ==========================================================
            //
            // Elimina únicamente la tabla de permisos individuales.
            // No modifica usuarios, roles ni otros módulos.
            // ==========================================================

            migrationBuilder.DropTable(
                name: "EmpresasPermisosUsuariosHU2");
        }
    }
}
