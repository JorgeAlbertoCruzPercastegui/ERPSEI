using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearModuloAdquisiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // =========================================================
            // ESTATUS
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_Estatus",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    Nombre = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),

                    Codigo = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),

                    Descripcion = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),

                    Orden = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    Activo = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_Estatus",
                        x => x.Id
                    );
                }
            );


            // =========================================================
            // PERMISOS POR USUARIO
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_PermisosUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    UsuarioId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    PuedeVisualizar = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    PuedeCrearSolicitud = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    PuedeGestionarSolicitudes = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    PuedeAprobar = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    PuedeAsignar = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    PuedeCotizar = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    PuedeGestionarProveedores = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    PuedeGenerarSolicitudPago = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    PuedeVerReportes = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    PuedeAdministrar = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"
                    ),

                    FechaModificacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true
                    ),

                    UsuarioModificacionId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_PermisosUsuarios",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_PermisosUsuarios_AspNetUsers_UsuarioId",
                        column:
                            x => x.UsuarioId,
                        principalTable:
                            "AspNetUsers",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // SOLICITUDES
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_Solicitudes",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    Folio = table.Column<string>(
                        type: "nvarchar(30)",
                        maxLength: 30,
                        nullable: false
                    ),

                    Titulo = table.Column<string>(
                        type: "nvarchar(250)",
                        maxLength: 250,
                        nullable: false
                    ),

                    FechaSolicitud = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false
                    ),

                    UsuarioSolicitanteId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    EmpleadoSolicitanteId = table.Column<int>(
                        type: "int",
                        nullable: true
                    ),

                    AreaId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    Descripcion = table.Column<string>(
                        type: "nvarchar(max)",
                        maxLength: 5000,
                        nullable: false
                    ),

                    Justificacion = table.Column<string>(
                        type: "nvarchar(max)",
                        maxLength: 5000,
                        nullable: false
                    ),

                    EstatusId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    UsuarioAsignadoId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: true
                    ),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"
                    ),

                    FechaModificacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true
                    ),

                    FechaEnvio = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true
                    ),

                    FechaFinalizacion = table.Column<DateTime>(
                        type: "datetime2",
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
                        "PK_ADQ_Solicitudes",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Solicitudes_ADQ_Estatus_EstatusId",
                        column:
                            x => x.EstatusId,
                        principalTable:
                            "ADQ_Estatus",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Solicitudes_Areas_AreaId",
                        column:
                            x => x.AreaId,
                        principalTable:
                            "Areas",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Solicitudes_AspNetUsers_UsuarioAsignadoId",
                        column:
                            x => x.UsuarioAsignadoId,
                        principalTable:
                            "AspNetUsers",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Solicitudes_AspNetUsers_UsuarioSolicitanteId",
                        column:
                            x => x.UsuarioSolicitanteId,
                        principalTable:
                            "AspNetUsers",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Solicitudes_Empleados_EmpleadoSolicitanteId",
                        column:
                            x => x.EmpleadoSolicitanteId,
                        principalTable:
                            "Empleados",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // ADJUNTOS
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_Adjuntos",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    SolicitudId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    NombreOriginal = table.Column<string>(
                        type: "nvarchar(260)",
                        maxLength: 260,
                        nullable: false
                    ),

                    NombreGuardado = table.Column<string>(
                        type: "nvarchar(260)",
                        maxLength: 260,
                        nullable: false
                    ),

                    RutaArchivo = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: false
                    ),

                    Extension = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: true
                    ),

                    MimeType = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true
                    ),

                    TamanoBytes = table.Column<long>(
                        type: "bigint",
                        nullable: false
                    ),

                    UsuarioCargaId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    FechaCarga = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"
                    ),

                    TipoDocumento = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false,
                        defaultValue: "General"
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
                        "PK_ADQ_Adjuntos",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Adjuntos_ADQ_Solicitudes_SolicitudId",
                        column:
                            x => x.SolicitudId,
                        principalTable:
                            "ADQ_Solicitudes",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Adjuntos_AspNetUsers_UsuarioCargaId",
                        column:
                            x => x.UsuarioCargaId,
                        principalTable:
                            "AspNetUsers",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // APROBACIONES
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_Aprobaciones",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    SolicitudId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    TipoAprobacion = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),

                    Orden = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    UsuarioAprobadorId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    Estatus = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false,
                        defaultValue: "Pendiente"
                    ),

                    Comentario = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"
                    ),

                    FechaRespuesta = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_Aprobaciones",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Aprobaciones_ADQ_Solicitudes_SolicitudId",
                        column:
                            x => x.SolicitudId,
                        principalTable:
                            "ADQ_Solicitudes",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Aprobaciones_AspNetUsers_UsuarioAprobadorId",
                        column:
                            x => x.UsuarioAprobadorId,
                        principalTable:
                            "AspNetUsers",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // ASIGNACIONES
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_Asignaciones",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    SolicitudId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    UsuarioAsignadoId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    UsuarioAsignadorId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    FechaAsignacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"
                    ),

                    FechaFin = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: true
                    ),

                    Activa = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: true
                    ),

                    Observaciones = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: true
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_Asignaciones",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Asignaciones_ADQ_Solicitudes_SolicitudId",
                        column:
                            x => x.SolicitudId,
                        principalTable:
                            "ADQ_Solicitudes",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Asignaciones_AspNetUsers_UsuarioAsignadoId",
                        column:
                            x => x.UsuarioAsignadoId,
                        principalTable:
                            "AspNetUsers",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Asignaciones_AspNetUsers_UsuarioAsignadorId",
                        column:
                            x => x.UsuarioAsignadorId,
                        principalTable:
                            "AspNetUsers",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // COMENTARIOS
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    SolicitudId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    UsuarioId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    Comentario = table.Column<string>(
                        type: "nvarchar(max)",
                        maxLength: 5000,
                        nullable: false
                    ),

                    EsNotaInterna = table.Column<bool>(
                        type: "bit",
                        nullable: false,
                        defaultValue: false
                    ),

                    FechaCreacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"
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
                        "PK_ADQ_Comentarios",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Comentarios_ADQ_Solicitudes_SolicitudId",
                        column:
                            x => x.SolicitudId,
                        principalTable:
                            "ADQ_Solicitudes",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Comentarios_AspNetUsers_UsuarioId",
                        column:
                            x => x.UsuarioId,
                        principalTable:
                            "AspNetUsers",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // HISTORIAL
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_Historial",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    SolicitudId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    UsuarioId = table.Column<string>(
                        type: "nvarchar(450)",
                        maxLength: 450,
                        nullable: false
                    ),

                    TipoEvento = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),

                    Descripcion = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: false
                    ),

                    EstatusAnteriorId = table.Column<int>(
                        type: "int",
                        nullable: true
                    ),

                    EstatusNuevoId = table.Column<int>(
                        type: "int",
                        nullable: true
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
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_ADQ_Historial",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Historial_ADQ_Estatus_EstatusAnteriorId",
                        column:
                            x => x.EstatusAnteriorId,
                        principalTable:
                            "ADQ_Estatus",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Historial_ADQ_Estatus_EstatusNuevoId",
                        column:
                            x => x.EstatusNuevoId,
                        principalTable:
                            "ADQ_Estatus",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Historial_ADQ_Solicitudes_SolicitudId",
                        column:
                            x => x.SolicitudId,
                        principalTable:
                            "ADQ_Solicitudes",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_Historial_AspNetUsers_UsuarioId",
                        column:
                            x => x.UsuarioId,
                        principalTable:
                            "AspNetUsers",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // DETALLE DE SOLICITUD
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_SolicitudesDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    SolicitudId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    ProductoServicio = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: false
                    ),

                    Cantidad = table.Column<decimal>(
                        type: "decimal(18,4)",
                        precision: 18,
                        scale: 4,
                        nullable: false
                    ),

                    Unidad = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),

                    Descripcion = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),

                    Orden = table.Column<int>(
                        type: "int",
                        nullable: false
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
                        "PK_ADQ_SolicitudesDetalle",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_SolicitudesDetalle_ADQ_Solicitudes_SolicitudId",
                        column:
                            x => x.SolicitudId,
                        principalTable:
                            "ADQ_Solicitudes",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // ADJUNTOS DE COMENTARIOS
            // =========================================================

            migrationBuilder.CreateTable(
                name: "ADQ_ComentariosAdjuntos",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"
                        ),

                    ComentarioId = table.Column<int>(
                        type: "int",
                        nullable: false
                    ),

                    NombreOriginal = table.Column<string>(
                        type: "nvarchar(260)",
                        maxLength: 260,
                        nullable: false
                    ),

                    NombreGuardado = table.Column<string>(
                        type: "nvarchar(260)",
                        maxLength: 260,
                        nullable: false
                    ),

                    RutaArchivo = table.Column<string>(
                        type: "nvarchar(1000)",
                        maxLength: 1000,
                        nullable: false
                    ),

                    Extension = table.Column<string>(
                        type: "nvarchar(20)",
                        maxLength: 20,
                        nullable: true
                    ),

                    MimeType = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: true
                    ),

                    TamanoBytes = table.Column<long>(
                        type: "bigint",
                        nullable: false
                    ),

                    FechaCarga = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false,
                        defaultValueSql: "GETDATE()"
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
                        "PK_ADQ_ComentariosAdjuntos",
                        x => x.Id
                    );

                    table.ForeignKey(
                        name:
                            "FK_ADQ_ComentariosAdjuntos_ADQ_Comentarios_ComentarioId",
                        column:
                            x => x.ComentarioId,
                        principalTable:
                            "ADQ_Comentarios",
                        principalColumn:
                            "Id",
                        onDelete:
                            ReferentialAction.Restrict
                    );
                }
            );


            // =========================================================
            // ESTADOS INICIALES
            // =========================================================

            migrationBuilder.InsertData(
                table: "ADQ_Estatus",
                columns: new[]
                {
                    "Id",
                    "Activo",
                    "Codigo",
                    "Descripcion",
                    "Nombre",
                    "Orden"
                },
                values: new object[,]
                {
                    {
                        1,
                        true,
                        "BORRADOR",
                        "Solicitud en proceso de captura.",
                        "Borrador",
                        1
                    },
                    {
                        2,
                        true,
                        "PENDIENTE_GERENTE",
                        "Solicitud pendiente de aprobación por el gerente del área.",
                        "Pendiente aprobación Gerente",
                        2
                    },
                    {
                        3,
                        true,
                        "SOLICITUD_ENVIADA",
                        "Solicitud enviada al área de Adquisiciones.",
                        "Solicitud enviada",
                        3
                    },
                    {
                        4,
                        true,
                        "EN_REVISION",
                        "Solicitud siendo revisada por Adquisiciones.",
                        "En revisión por Adquisiciones",
                        4
                    },
                    {
                        5,
                        true,
                        "APROBADA",
                        "Solicitud aprobada por Adquisiciones.",
                        "Aprobada",
                        5
                    },
                    {
                        6,
                        true,
                        "RECHAZADA",
                        "Solicitud rechazada.",
                        "Rechazada",
                        6
                    },
                    {
                        7,
                        true,
                        "CANCELADA",
                        "Solicitud cancelada.",
                        "Cancelada",
                        7
                    },
                    {
                        8,
                        true,
                        "ASIGNADA",
                        "Solicitud asignada a un agente de compras.",
                        "Asignada",
                        8
                    },
                    {
                        9,
                        true,
                        "EN_COTIZACION",
                        "Solicitud en proceso de cotización.",
                        "En proceso de cotización",
                        9
                    },
                    {
                        10,
                        true,
                        "COTIZACION_FINALIZADA",
                        "Proceso de cotización finalizado.",
                        "Cotización finalizada",
                        10
                    },
                    {
                        11,
                        true,
                        "PENDIENTE_PRESUPUESTO",
                        "Solicitud pendiente de iniciar el flujo presupuestal.",
                        "Pendiente aprobación presupuestal",
                        11
                    },
                    {
                        12,
                        true,
                        "EN_APROBACION_PRESUPUESTAL",
                        "Solicitud dentro del flujo de aprobación presupuestal.",
                        "En aprobación presupuestal",
                        12
                    },
                    {
                        13,
                        true,
                        "PRESUPUESTO_APROBADO",
                        "Flujo presupuestal completado.",
                        "Aprobación presupuestal completada",
                        13
                    },
                    {
                        14,
                        true,
                        "EN_PAGO",
                        "Solicitud en proceso de pago.",
                        "En proceso de pago",
                        14
                    },
                    {
                        15,
                        true,
                        "EN_COMPRA",
                        "Compra en proceso.",
                        "En proceso de compra",
                        15
                    },
                    {
                        16,
                        true,
                        "EN_ENTREGA",
                        "Compra en proceso de entrega.",
                        "En proceso de entrega",
                        16
                    },
                    {
                        17,
                        true,
                        "FINALIZADA",
                        "Proceso de adquisición finalizado.",
                        "Finalizada",
                        17
                    }
                }
            );


            // =========================================================
            // ÍNDICES
            // =========================================================

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Adjuntos_Solicitud",
                table:
                    "ADQ_Adjuntos",
                column:
                    "SolicitudId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Adjuntos_Solicitud_Tipo",
                table:
                    "ADQ_Adjuntos",
                columns:
                    new[]
                    {
                        "SolicitudId",
                        "TipoDocumento"
                    }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Adjuntos_UsuarioCargaId",
                table:
                    "ADQ_Adjuntos",
                column:
                    "UsuarioCargaId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Aprobaciones_Solicitud",
                table:
                    "ADQ_Aprobaciones",
                column:
                    "SolicitudId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Aprobaciones_Solicitud_Tipo_Orden",
                table:
                    "ADQ_Aprobaciones",
                columns:
                    new[]
                    {
                        "SolicitudId",
                        "TipoAprobacion",
                        "Orden"
                    }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Aprobaciones_Usuario_Estatus",
                table:
                    "ADQ_Aprobaciones",
                columns:
                    new[]
                    {
                        "UsuarioAprobadorId",
                        "Estatus"
                    }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Asignaciones_Solicitud",
                table:
                    "ADQ_Asignaciones",
                column:
                    "SolicitudId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Asignaciones_Usuario_Activa",
                table:
                    "ADQ_Asignaciones",
                columns:
                    new[]
                    {
                        "UsuarioAsignadoId",
                        "Activa"
                    }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Asignaciones_UsuarioAsignadorId",
                table:
                    "ADQ_Asignaciones",
                column:
                    "UsuarioAsignadorId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Comentarios_Solicitud_Fecha",
                table:
                    "ADQ_Comentarios",
                columns:
                    new[]
                    {
                        "SolicitudId",
                        "FechaCreacion"
                    }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Comentarios_UsuarioId",
                table:
                    "ADQ_Comentarios",
                column:
                    "UsuarioId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_ComentariosAdjuntos_Comentario",
                table:
                    "ADQ_ComentariosAdjuntos",
                column:
                    "ComentarioId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Estatus_Orden",
                table:
                    "ADQ_Estatus",
                column:
                    "Orden"
            );

            migrationBuilder.CreateIndex(
                name:
                    "UX_ADQ_Estatus_Codigo",
                table:
                    "ADQ_Estatus",
                column:
                    "Codigo",
                unique:
                    true
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Historial_EstatusAnteriorId",
                table:
                    "ADQ_Historial",
                column:
                    "EstatusAnteriorId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Historial_EstatusNuevoId",
                table:
                    "ADQ_Historial",
                column:
                    "EstatusNuevoId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Historial_Solicitud",
                table:
                    "ADQ_Historial",
                column:
                    "SolicitudId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Historial_Solicitud_Fecha",
                table:
                    "ADQ_Historial",
                columns:
                    new[]
                    {
                        "SolicitudId",
                        "FechaEvento"
                    }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Historial_TipoEvento",
                table:
                    "ADQ_Historial",
                column:
                    "TipoEvento"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Historial_UsuarioId",
                table:
                    "ADQ_Historial",
                column:
                    "UsuarioId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_PermisosUsuarios_FechaModificacion",
                table:
                    "ADQ_PermisosUsuarios",
                column:
                    "FechaModificacion"
            );

            migrationBuilder.CreateIndex(
                name:
                    "UX_ADQ_PermisosUsuarios_UsuarioId",
                table:
                    "ADQ_PermisosUsuarios",
                column:
                    "UsuarioId",
                unique:
                    true
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Solicitudes_Area",
                table:
                    "ADQ_Solicitudes",
                column:
                    "AreaId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Solicitudes_EmpleadoSolicitanteId",
                table:
                    "ADQ_Solicitudes",
                column:
                    "EmpleadoSolicitanteId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Solicitudes_Estatus",
                table:
                    "ADQ_Solicitudes",
                column:
                    "EstatusId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Solicitudes_Estatus_Fecha",
                table:
                    "ADQ_Solicitudes",
                columns:
                    new[]
                    {
                        "EstatusId",
                        "FechaSolicitud"
                    }
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Solicitudes_FechaSolicitud",
                table:
                    "ADQ_Solicitudes",
                column:
                    "FechaSolicitud"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Solicitudes_UsuarioAsignado",
                table:
                    "ADQ_Solicitudes",
                column:
                    "UsuarioAsignadoId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_Solicitudes_UsuarioSolicitante",
                table:
                    "ADQ_Solicitudes",
                column:
                    "UsuarioSolicitanteId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "UX_ADQ_Solicitudes_Folio",
                table:
                    "ADQ_Solicitudes",
                column:
                    "Folio",
                unique:
                    true
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_SolicitudesDetalle_Solicitud",
                table:
                    "ADQ_SolicitudesDetalle",
                column:
                    "SolicitudId"
            );

            migrationBuilder.CreateIndex(
                name:
                    "IX_ADQ_SolicitudesDetalle_Solicitud_Orden",
                table:
                    "ADQ_SolicitudesDetalle",
                columns:
                    new[]
                    {
                        "SolicitudId",
                        "Orden"
                    }
            );
        }


        /// <inheritdoc />
        protected override void Down(
            MigrationBuilder migrationBuilder)
        {
            /*
             * IMPORTANTE:
             *
             * Esta reversión solamente elimina estructuras
             * pertenecientes al módulo de Adquisiciones.
             *
             * No modifica datos ni tablas de otros módulos.
             */

            migrationBuilder.DropTable(
                name:
                    "ADQ_Adjuntos"
            );

            migrationBuilder.DropTable(
                name:
                    "ADQ_Aprobaciones"
            );

            migrationBuilder.DropTable(
                name:
                    "ADQ_Asignaciones"
            );

            migrationBuilder.DropTable(
                name:
                    "ADQ_ComentariosAdjuntos"
            );

            migrationBuilder.DropTable(
                name:
                    "ADQ_Historial"
            );

            migrationBuilder.DropTable(
                name:
                    "ADQ_PermisosUsuarios"
            );

            migrationBuilder.DropTable(
                name:
                    "ADQ_SolicitudesDetalle"
            );

            migrationBuilder.DropTable(
                name:
                    "ADQ_Comentarios"
            );

            migrationBuilder.DropTable(
                name:
                    "ADQ_Solicitudes"
            );

            migrationBuilder.DropTable(
                name:
                    "ADQ_Estatus"
            );
        }
    }
}