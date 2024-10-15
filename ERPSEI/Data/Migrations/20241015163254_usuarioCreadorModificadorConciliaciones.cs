using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class usuarioCreadorModificadorConciliaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conciliaciones_AspNetUsers_AppUserCId",
                table: "Conciliaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Conciliaciones_AspNetUsers_AppUserMId",
                table: "Conciliaciones");

            migrationBuilder.DropIndex(
                name: "IX_Conciliaciones_AppUserCId",
                table: "Conciliaciones");

            migrationBuilder.DropIndex(
                name: "IX_Conciliaciones_AppUserMId",
                table: "Conciliaciones");

            migrationBuilder.DropColumn(
                name: "AppUserCId",
                table: "Conciliaciones");

            migrationBuilder.DropColumn(
                name: "AppUserMId",
                table: "Conciliaciones");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioModificadorId",
                table: "Conciliaciones",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreadorId",
                table: "Conciliaciones",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conciliaciones_UsuarioCreadorId",
                table: "Conciliaciones",
                column: "UsuarioCreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Conciliaciones_UsuarioModificadorId",
                table: "Conciliaciones",
                column: "UsuarioModificadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conciliaciones_AspNetUsers_UsuarioCreadorId",
                table: "Conciliaciones",
                column: "UsuarioCreadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conciliaciones_AspNetUsers_UsuarioModificadorId",
                table: "Conciliaciones",
                column: "UsuarioModificadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conciliaciones_AspNetUsers_UsuarioCreadorId",
                table: "Conciliaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Conciliaciones_AspNetUsers_UsuarioModificadorId",
                table: "Conciliaciones");

            migrationBuilder.DropIndex(
                name: "IX_Conciliaciones_UsuarioCreadorId",
                table: "Conciliaciones");

            migrationBuilder.DropIndex(
                name: "IX_Conciliaciones_UsuarioModificadorId",
                table: "Conciliaciones");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioModificadorId",
                table: "Conciliaciones",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioCreadorId",
                table: "Conciliaciones",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserCId",
                table: "Conciliaciones",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserMId",
                table: "Conciliaciones",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conciliaciones_AppUserCId",
                table: "Conciliaciones",
                column: "AppUserCId");

            migrationBuilder.CreateIndex(
                name: "IX_Conciliaciones_AppUserMId",
                table: "Conciliaciones",
                column: "AppUserMId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conciliaciones_AspNetUsers_AppUserCId",
                table: "Conciliaciones",
                column: "AppUserCId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conciliaciones_AspNetUsers_AppUserMId",
                table: "Conciliaciones",
                column: "AppUserMId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
