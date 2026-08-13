using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERPSEI.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearModuloMesaServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SD_Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SD_EquiposSoporte",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_EquiposSoporte", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SD_EstadosTicket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EsFinal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PausaSla = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_EstadosTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SD_PrioridadesTicket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    MinutosRespuesta = table.Column<int>(type: "int", nullable: false),
                    MinutosResolucion = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_PrioridadesTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SD_TiposTicket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_TiposTicket", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SD_Subcategorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_Subcategorias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SD_Subcategorias_SD_Categorias_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SD_Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SD_EquiposSoporteUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupportTeamId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EsResponsable = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_EquiposSoporteUsuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SD_EquiposSoporteUsuarios_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_EquiposSoporteUsuarios_SD_EquiposSoporte_SupportTeamId",
                        column: x => x.SupportTeamId,
                        principalTable: "SD_EquiposSoporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SD_Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Folio = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TicketTypeId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    UsuarioSolicitanteId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UsuarioAsignadoId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SupportTeamId = table.Column<int>(type: "int", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    SubcategoryId = table.Column<int>(type: "int", nullable: true),
                    PriorityId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Origen = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Intranet"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaPrimeraRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaLimiteRespuestaSla = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaLimiteResolucionSla = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SlaRespuestaVencido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SlaResolucionVencido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Resolucion = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    UsuarioCierreId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SD_Tickets_AspNetUsers_UsuarioAsignadoId",
                        column: x => x.UsuarioAsignadoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_Tickets_AspNetUsers_UsuarioCierreId",
                        column: x => x.UsuarioCierreId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_Tickets_AspNetUsers_UsuarioSolicitanteId",
                        column: x => x.UsuarioSolicitanteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_Tickets_SD_Categorias_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "SD_Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_Tickets_SD_EquiposSoporte_SupportTeamId",
                        column: x => x.SupportTeamId,
                        principalTable: "SD_EquiposSoporte",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_Tickets_SD_EstadosTicket_StatusId",
                        column: x => x.StatusId,
                        principalTable: "SD_EstadosTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_Tickets_SD_PrioridadesTicket_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "SD_PrioridadesTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_Tickets_SD_Subcategorias_SubcategoryId",
                        column: x => x.SubcategoryId,
                        principalTable: "SD_Subcategorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_Tickets_SD_TiposTicket_TicketTypeId",
                        column: x => x.TicketTypeId,
                        principalTable: "SD_TiposTicket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SD_TicketAdjuntos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    NombreOriginal = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NombreAlmacenado = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    UsuarioCargaId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_TicketAdjuntos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SD_TicketAdjuntos_AspNetUsers_UsuarioCargaId",
                        column: x => x.UsuarioCargaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_TicketAdjuntos_SD_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "SD_Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SD_TicketComentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    EsNotaInterna = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Eliminado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_TicketComentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SD_TicketComentarios_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_TicketComentarios_SD_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "SD_Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SD_TicketHistorial",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Accion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Campo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ValorAnterior = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValorNuevo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Detalle = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    DireccionIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SD_TicketHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SD_TicketHistorial_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SD_TicketHistorial_SD_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "SD_Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "Id", "Categoria", "Deshabilitado", "Nombre", "NombreNormalizado" },
                values: new object[] { 26, "erp", 0, "Incidencias", "incidencias" });

            migrationBuilder.InsertData(
                table: "SD_Categorias",
                columns: new[] { "Id", "Activo", "Descripcion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, true, "Redes, servidores, VPN, conectividad e infraestructura tecnológica.", "Infraestructura", 1 },
                    { 2, true, "Aplicaciones y programas utilizados por los usuarios.", "Software", 2 },
                    { 3, true, "Solicitudes o problemas relacionados con cuentas y permisos.", "Accesos", 3 },
                    { 4, true, "Equipos de cómputo y periféricos.", "Hardware", 4 },
                    { 5, true, "Outlook, Teams, OneDrive y servicios Microsoft.", "Microsoft 365", 5 },
                    { 6, true, "Incidencias y solicitudes relacionadas con la Intranet.", "Intranet", 6 },
                    { 7, true, "Incidentes relacionados con ciberseguridad.", "Seguridad", 7 },
                    { 8, true, "Solicitudes que no pertenecen a otra categoría.", "Otros", 8 }
                });

            migrationBuilder.InsertData(
                table: "SD_EquiposSoporte",
                columns: new[] { "Id", "Activo", "Descripcion", "Nombre" },
                values: new object[] { 1, true, "Equipo principal responsable de la atención de tickets.", "Mesa de Servicio TI" });

            migrationBuilder.InsertData(
                table: "SD_EstadosTicket",
                columns: new[] { "Id", "Activo", "Codigo", "Descripcion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, true, "NUEVO", "Ticket registrado y pendiente de atención.", "Nuevo", 1 },
                    { 2, true, "ASIGNADO", "Ticket asignado a un administrador o equipo.", "Asignado", 2 },
                    { 3, true, "EN_PROCESO", "Ticket actualmente en atención.", "En proceso", 3 }
                });

            migrationBuilder.InsertData(
                table: "SD_EstadosTicket",
                columns: new[] { "Id", "Activo", "Codigo", "Descripcion", "Nombre", "Orden", "PausaSla" },
                values: new object[] { 4, true, "PENDIENTE_USUARIO", "Se requiere información o respuesta del solicitante.", "Pendiente del usuario", 4, true });

            migrationBuilder.InsertData(
                table: "SD_EstadosTicket",
                columns: new[] { "Id", "Activo", "Codigo", "Descripcion", "Nombre", "Orden" },
                values: new object[] { 5, true, "RESUELTO", "El administrador ha registrado una solución.", "Resuelto", 5 });

            migrationBuilder.InsertData(
                table: "SD_EstadosTicket",
                columns: new[] { "Id", "Activo", "Codigo", "Descripcion", "EsFinal", "Nombre", "Orden" },
                values: new object[] { 6, true, "CERRADO", "Ticket finalizado.", true, "Cerrado", 6 });

            migrationBuilder.InsertData(
                table: "SD_EstadosTicket",
                columns: new[] { "Id", "Activo", "Codigo", "Descripcion", "Nombre", "Orden" },
                values: new object[] { 7, true, "REABIERTO", "Ticket reabierto después de su resolución.", "Reabierto", 7 });

            migrationBuilder.InsertData(
                table: "SD_EstadosTicket",
                columns: new[] { "Id", "Activo", "Codigo", "Descripcion", "EsFinal", "Nombre", "Orden" },
                values: new object[] { 8, true, "CANCELADO", "Ticket cancelado.", true, "Cancelado", 8 });

            migrationBuilder.InsertData(
                table: "SD_PrioridadesTicket",
                columns: new[] { "Id", "Activo", "Codigo", "MinutosResolucion", "MinutosRespuesta", "Nivel", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "CRITICA", 120, 15, 1, "Crítica" },
                    { 2, true, "ALTA", 240, 30, 2, "Alta" },
                    { 3, true, "MEDIA", 480, 120, 3, "Media" },
                    { 4, true, "BAJA", 1440, 240, 4, "Baja" }
                });

            migrationBuilder.InsertData(
                table: "SD_TiposTicket",
                columns: new[] { "Id", "Activo", "Codigo", "Descripcion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, true, "INC", "Falla, interrupción o afectación de un servicio.", "Incidente", 1 },
                    { 2, true, "SR", "Solicitud de acceso, equipo, software o servicio.", "Solicitud de Servicio", 2 },
                    { 3, true, "PRB", "Análisis de causa raíz de incidentes recurrentes.", "Problema", 3 },
                    { 4, true, "CHG", "Solicitud de cambio controlado en infraestructura o sistemas.", "Cambio", 4 }
                });









            migrationBuilder.InsertData(
                table: "SD_Subcategorias",
                columns: new[] { "Id", "Activo", "CategoryId", "Descripcion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { 1, true, 1, null, "VPN", 1 },
                    { 2, true, 1, null, "Internet", 2 },
                    { 3, true, 1, null, "Servidor", 3 },
                    { 4, true, 2, null, "Instalación de software", 1 },
                    { 5, true, 2, null, "Error de aplicación", 2 },
                    { 6, true, 3, null, "Alta de usuario", 1 },
                    { 7, true, 3, null, "Permisos", 2 },
                    { 8, true, 3, null, "Contraseña", 3 },
                    { 9, true, 4, null, "Laptop / PC", 1 },
                    { 10, true, 4, null, "Monitor", 2 },
                    { 11, true, 4, null, "Impresora", 3 },
                    { 12, true, 5, null, "Outlook", 1 },
                    { 13, true, 5, null, "Teams", 2 },
                    { 14, true, 5, null, "OneDrive", 3 },
                    { 15, true, 6, null, "Acceso", 1 },
                    { 16, true, 6, null, "Error funcional", 2 },
                    { 17, true, 7, null, "Correo sospechoso", 1 },
                    { 18, true, 7, null, "Malware", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "UX_SD_Categorias_Nombre",
                table: "SD_Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SD_EquiposSoporte_Nombre",
                table: "SD_EquiposSoporte",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SD_EquiposSoporteUsuarios_UserId",
                table: "SD_EquiposSoporteUsuarios",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UX_SD_EquiposSoporteUsuarios_Equipo_Usuario",
                table: "SD_EquiposSoporteUsuarios",
                columns: new[] { "SupportTeamId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SD_EstadosTicket_Codigo",
                table: "SD_EstadosTicket",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SD_PrioridadesTicket_Codigo",
                table: "SD_PrioridadesTicket",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_SD_Subcategorias_Categoria_Nombre",
                table: "SD_Subcategorias",
                columns: new[] { "CategoryId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SD_TicketAdjuntos_Ticket",
                table: "SD_TicketAdjuntos",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_TicketAdjuntos_UsuarioCargaId",
                table: "SD_TicketAdjuntos",
                column: "UsuarioCargaId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_TicketComentarios_Ticket_Fecha",
                table: "SD_TicketComentarios",
                columns: new[] { "TicketId", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_SD_TicketComentarios_UsuarioId",
                table: "SD_TicketComentarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_TicketHistorial_Ticket_Fecha",
                table: "SD_TicketHistorial",
                columns: new[] { "TicketId", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_SD_TicketHistorial_UsuarioId",
                table: "SD_TicketHistorial",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_CategoryId",
                table: "SD_Tickets",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_FechaCreacion",
                table: "SD_Tickets",
                column: "FechaCreacion");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_Priority",
                table: "SD_Tickets",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_Status",
                table: "SD_Tickets",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_SubcategoryId",
                table: "SD_Tickets",
                column: "SubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_SupportTeamId",
                table: "SD_Tickets",
                column: "SupportTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_TicketTypeId",
                table: "SD_Tickets",
                column: "TicketTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_UsuarioAsignado",
                table: "SD_Tickets",
                column: "UsuarioAsignadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_UsuarioCierreId",
                table: "SD_Tickets",
                column: "UsuarioCierreId");

            migrationBuilder.CreateIndex(
                name: "IX_SD_Tickets_UsuarioSolicitante",
                table: "SD_Tickets",
                column: "UsuarioSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "UX_SD_Tickets_Folio",
                table: "SD_Tickets",
                column: "Folio",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SD_TiposTicket_Nombre",
                table: "SD_TiposTicket",
                column: "Nombre");

            migrationBuilder.CreateIndex(
                name: "UX_SD_TiposTicket_Codigo",
                table: "SD_TiposTicket",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SD_EquiposSoporteUsuarios");

            migrationBuilder.DropTable(
                name: "SD_TicketAdjuntos");

            migrationBuilder.DropTable(
                name: "SD_TicketComentarios");

            migrationBuilder.DropTable(
                name: "SD_TicketHistorial");

            migrationBuilder.DropTable(
                name: "SD_Tickets");

            migrationBuilder.DropTable(
                name: "SD_EquiposSoporte");

            migrationBuilder.DropTable(
                name: "SD_EstadosTicket");

            migrationBuilder.DropTable(
                name: "SD_PrioridadesTicket");

            migrationBuilder.DropTable(
                name: "SD_Subcategorias");

            migrationBuilder.DropTable(
                name: "SD_TiposTicket");

            migrationBuilder.DropTable(
                name: "SD_Categorias");

            migrationBuilder.DeleteData(
                table: "Modulos",
                keyColumn: "Id",
                keyValue: 26);
        }
    }
}