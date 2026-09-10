using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearConfiguracionYObservadoresAprobacionPresupuestal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ADQ_ConfiguracionAprobacionPresupuestal",
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

                    Orden = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    TipoEtapa = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),

                    NombreEtapa = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false
                    ),

                    UsuarioResponsableId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    UsuarioAsistenteId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true
                    ),

                    AsistenteRecibeCopia = table.Column<bool>(
                        type: "bit",
                        nullable: false
                    ),

                    RecibirCopiaDesdeOrden = table.Column<int>(
                        type: "int",
                        nullable: true
                    ),

                    Activo = table.Column<bool>(
                        type: "bit",
                        nullable: false
                    ),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false
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
                        nullable: false
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_ConfiguracionAprobacionPresupuestal",
                        x => x.Id
                    );
                }
            );


            migrationBuilder.CreateTable(
                name: "ADQ_AprobacionesPresupuestalesObservadores",
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

                    AprobacionPresupuestalId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    UsuarioId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    TipoObservador = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),

                    NombreOrigen = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false
                    ),

                    OrdenActivacion = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    Activo = table.Column<bool>(
                        type: "bit",
                        nullable: false
                    ),

                    FechaActivacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true
                    ),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false
                    ),

                    Eliminado = table.Column<bool>(
                        type: "bit",
                        nullable: false
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_AprobacionesPresupuestalesObservadores",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name: "FK_ADQ_AprobacionesPresupuestalesObservadores_ADQ_AprobacionesPresupuestales_AprobacionPresupuestalId",
                        column: x => x.AprobacionPresupuestalId,
                        principalTable: "ADQ_AprobacionesPresupuestales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );


            migrationBuilder.CreateIndex(
                name: "IX_ADQ_ConfiguracionAprobacionPresupuestal_Orden_Activo_Eliminado",
                table: "ADQ_ConfiguracionAprobacionPresupuestal",
                columns: new[]
                {
                    "Orden",
                    "Activo",
                    "Eliminado"
                }
            );


            migrationBuilder.CreateIndex(
                name: "IX_ADQ_AprobacionesPresupuestalesObservadores_AprobacionPresupuestalId_UsuarioId",
                table: "ADQ_AprobacionesPresupuestalesObservadores",
                columns: new[]
                {
                    "AprobacionPresupuestalId",
                    "UsuarioId"
                }
            );
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ADQ_AprobacionesPresupuestalesObservadores"
            );


            migrationBuilder.DropTable(
                name: "ADQ_ConfiguracionAprobacionPresupuestal"
            );
        }
    }
}