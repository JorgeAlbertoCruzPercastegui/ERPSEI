using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearDetalleAprobacionPresupuestalAdquisiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ADQ_AprobacionesPresupuestalesDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false
                    )
                    .Annotation(
                        "SqlServer:Identity",
                        "1, 1"
                    ),

                    AprobacionPresupuestalId =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    Orden =
                        table.Column<int>(
                            type: "int",
                            nullable: false
                        ),

                    TipoAprobador =
                        table.Column<string>(
                            type: "nvarchar(100)",
                            maxLength: 100,
                            nullable: false
                        ),

                    NombreEtapa =
                        table.Column<string>(
                            type: "nvarchar(150)",
                            maxLength: 150,
                            nullable: false
                        ),

                    UsuarioAprobadorId =
                        table.Column<string>(
                            type: "nvarchar(450)",
                            maxLength: 450,
                            nullable: true
                        ),

                    Estatus =
                        table.Column<string>(
                            type: "nvarchar(30)",
                            maxLength: 30,
                            nullable: false
                        ),

                    EsActual =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        ),

                    Comentario =
                        table.Column<string>(
                            type: "nvarchar(3000)",
                            maxLength: 3000,
                            nullable: true
                        ),

                    FechaDecision =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: true
                        ),

                    FechaCreacion =
                        table.Column<DateTime>(
                            type: "datetime2",
                            nullable: false
                        ),

                    Eliminado =
                        table.Column<bool>(
                            type: "bit",
                            nullable: false
                        )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_AprobacionesPresupuestalesDetalle",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_AprobacionesPresupuestalesDetalle_ADQ_AprobacionesPresupuestales_AprobacionPresupuestalId",

                        column:
                            x => x.AprobacionPresupuestalId,

                        principalTable:
                            "ADQ_AprobacionesPresupuestales",

                        principalColumn:
                            "Id",

                        onDelete:
                            ReferentialAction.Restrict
                    );
                });


            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_AprobacionesPresupuestalesDetalle_AprobacionPresupuestalId_Orden",

                table:
                    "ADQ_AprobacionesPresupuestalesDetalle",

                columns:
                    new[]
                    {
                        "AprobacionPresupuestalId",
                        "Orden"
                    },

                unique:
                    true
            );
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name:
                    "ADQ_AprobacionesPresupuestalesDetalle"
            );
        }
    }
}