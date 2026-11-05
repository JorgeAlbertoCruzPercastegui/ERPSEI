
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class HU2_ObligatoriedadGlobalEmpresas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =====================================================
            // HU2 - OBLIGATORIEDAD DOCUMENTAL GLOBAL
            // =====================================================
            //
            // Agrega un indicador de obligatoriedad a cada tipo
            // documental del catálogo general de Empresas.
            //
            // La configuración aplicará a todas las empresas,
            // existentes y futuras.
            //
            // false = Documento opcional
            // true  = Documento obligatorio
            //
            // Inicialmente todos quedan como opcionales hasta
            // que el administrador configure cuáles son
            // obligatorios desde el módulo Empresas.
            // =====================================================

            migrationBuilder.AddColumn<bool>(
                name: "EsObligatorio",
                table: "TipoArchivoEmpresa",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revierte únicamente la columna incorporada.
            // No elimina empresas ni archivos documentales.

            migrationBuilder.DropColumn(
                name: "EsObligatorio",
                table: "TipoArchivoEmpresa");
        }
    }
}