using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCodePermisosUsuariosAdquisiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(
            MigrationBuilder migrationBuilder
        )
        {
            migrationBuilder.AddColumn<bool>(
                name: "PuedeCrear",
                table: "ADQ_PermisosUsuarios",
                type: "bit",
                nullable: false,
                defaultValue: false
            );


            migrationBuilder.AddColumn<bool>(
                name: "PuedeDescargar",
                table: "ADQ_PermisosUsuarios",
                type: "bit",
                nullable: false,
                defaultValue: false
            );


            migrationBuilder.AddColumn<bool>(
                name: "PuedeEditar",
                table: "ADQ_PermisosUsuarios",
                type: "bit",
                nullable: false,
                defaultValue: false
            );


            migrationBuilder.AddColumn<bool>(
                name: "PuedeEliminar",
                table: "ADQ_PermisosUsuarios",
                type: "bit",
                nullable: false,
                defaultValue: false
            );


            migrationBuilder.AddColumn<bool>(
                name: "PuedeTodo",
                table: "ADQ_PermisosUsuarios",
                type: "bit",
                nullable: false,
                defaultValue: false
            );
        }


        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder
        )
        {
            migrationBuilder.DropColumn(
                name: "PuedeCrear",
                table: "ADQ_PermisosUsuarios"
            );


            migrationBuilder.DropColumn(
                name: "PuedeDescargar",
                table: "ADQ_PermisosUsuarios"
            );


            migrationBuilder.DropColumn(
                name: "PuedeEditar",
                table: "ADQ_PermisosUsuarios"
            );


            migrationBuilder.DropColumn(
                name: "PuedeEliminar",
                table: "ADQ_PermisosUsuarios"
            );


            migrationBuilder.DropColumn(
                name: "PuedeTodo",
                table: "ADQ_PermisosUsuarios"
            );
        }
    }
}