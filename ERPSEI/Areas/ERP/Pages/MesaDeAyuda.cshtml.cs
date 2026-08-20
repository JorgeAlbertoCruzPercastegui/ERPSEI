using ERPSEI.Data;
using ERPSEI.Data.Entities.ServiceDesk;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Areas.ERP.Pages
{
    [Authorize]
    public class MesaDeAyudaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly ILogger<MesaDeAyudaModel> _logger;

        public MesaDeAyudaModel(
            ApplicationDbContext context,
            AppUserManager userManager,
            ILogger<MesaDeAyudaModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public class ActualizarTicketRequest
        {
            public int TicketId { get; set; }

            public string? UsuarioAsignadoId { get; set; }

            public int PriorityId { get; set; }

            public int StatusId { get; set; }
        }

        // =========================================================
        // PERMISOS
        // =========================================================

        public bool EsAdmin { get; private set; }

        // =========================================================
        // LISTADO PRINCIPAL
        // =========================================================

        public List<ServiceTicket> Tickets { get; private set; } = new();

        public List<SelectListItem> TiposTicket { get; private set; } = new();

        public List<SelectListItem> Categorias { get; private set; } = new();

        public List<SelectListItem> Subcategorias { get; private set; } = new();

        // =========================================================
        // KPIs
        // =========================================================

        public int TotalAbiertos { get; private set; }

        public int TotalNuevos { get; private set; }

        public int TotalEnProceso { get; private set; }

        public int TotalSinAsignar { get; private set; }

        public int TotalResueltos { get; private set; }

        // =========================================================
        // CREAR TICKET
        // =========================================================

        [BindProperty]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debes seleccionar un tipo de solicitud.")]
        public int TicketTypeId { get; set; }

        [BindProperty]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debes seleccionar una categoría.")]
        public int CategoryId { get; set; }

        [BindProperty]
        public int? SubcategoryId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [StringLength(
            250,
            ErrorMessage = "El asunto no puede superar los 250 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(
            5000,
            ErrorMessage = "La descripción no puede superar los 5000 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        // =========================================================
        // GET PRINCIPAL
        // =========================================================

        public async Task<IActionResult> OnGetAsync()
        {
            AppUser? usuarioActual =
                await _userManager.GetUserAsync(User);

            if (usuarioActual == null)
            {
                return Challenge();
            }

            EsAdmin = EsAdministradorMesa();

            await CargarCatalogosAsync();

            IQueryable<ServiceTicket> query =
                _context.ServiceTickets
                    .AsNoTracking()
                    .Include(x => x.TicketType)
                    .Include(x => x.Status)
                    .Include(x => x.Priority)
                    .Include(x => x.Category)
                    .Include(x => x.Subcategory)
                    .Include(x => x.SupportTeam);

            /*
             * El administrador puede consultar todos los tickets.
             * El usuario normal solamente puede consultar los suyos.
             */
            if (!EsAdmin)
            {
                query = query.Where(
                    x => x.UsuarioSolicitanteId == usuarioActual.Id
                );
            }

            Tickets =
                await query
                    .OrderByDescending(x => x.FechaCreacion)
                    .ToListAsync();

            CalcularKpis();

            return Page();
        }

        // =========================================================
        // CREAR TICKET
        // POST ?handler=CrearTicket
        // =========================================================

        public async Task<IActionResult> OnPostCrearTicketAsync()
        {
            AppUser? usuarioActual =
                await _userManager.GetUserAsync(User);

            if (usuarioActual == null)
            {
                return Challenge();
            }

            EsAdmin = EsAdministradorMesa();

            // =====================================================
            // NORMALIZAR DATOS
            // =====================================================

            Titulo = Titulo?.Trim() ?? string.Empty;
            Descripcion = Descripcion?.Trim() ?? string.Empty;

            // =====================================================
            // VALIDAR TIPO
            // =====================================================

            bool tipoExiste =
                await _context.ServiceTicketTypes
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == TicketTypeId &&
                        x.Activo
                    );

            if (!tipoExiste)
            {
                ModelState.AddModelError(
                    nameof(TicketTypeId),
                    "El tipo de solicitud seleccionado no es válido."
                );
            }

            // =====================================================
            // VALIDAR CATEGORÍA
            // =====================================================

            bool categoriaExiste =
                await _context.ServiceCategories
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == CategoryId &&
                        x.Activo
                    );

            if (!categoriaExiste)
            {
                ModelState.AddModelError(
                    nameof(CategoryId),
                    "La categoría seleccionada no es válida."
                );
            }

            // =====================================================
            // VALIDAR SUBCATEGORÍA
            // =====================================================

            if (SubcategoryId.HasValue)
            {
                bool subcategoriaExiste =
                    await _context.ServiceSubcategories
                        .AsNoTracking()
                        .AnyAsync(x =>
                            x.Id == SubcategoryId.Value &&
                            x.CategoryId == CategoryId &&
                            x.Activo
                        );

                if (!subcategoriaExiste)
                {
                    ModelState.AddModelError(
                        nameof(SubcategoryId),
                        "La subcategoría seleccionada no corresponde a la categoría."
                    );
                }
            }

            // =====================================================
            // VALIDACIÓN GENERAL
            // =====================================================

            if (!ModelState.IsValid)
            {
                await CargarPantallaAsync(usuarioActual);

                return Page();
            }

            try
            {
                DateTime ahora = DateTime.Now;

                /*
                 * Por el momento:
                 *
                 * PRIORIDAD:
                 * 3 = Media
                 *
                 * ESTADO:
                 * 1 = Nuevo
                 *
                 * Esto coincide con el catálogo ya creado
                 * para Mesa de Ayuda.
                 */
                const int prioridadInicialId = 3;
                const int estadoInicialId = 1;

                ServiceTicketPriority? prioridad =
                    await _context.ServiceTicketPriorities
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x => x.Id == prioridadInicialId
                        );

                if (prioridad == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "No fue posible determinar la prioridad inicial."
                    );

                    await CargarPantallaAsync(usuarioActual);

                    return Page();
                }

                // =================================================
                // GENERAR FOLIO
                // =================================================

                string folio =
                    await GenerarFolioAsync();

                // =================================================
                // CREAR TICKET
                // =================================================

                ServiceTicket ticket =
                    new ServiceTicket
                    {
                        Folio = folio,

                        TicketTypeId = TicketTypeId,

                        Titulo = Titulo,

                        Descripcion = Descripcion,

                        UsuarioSolicitanteId =
                            usuarioActual.Id,

                        UsuarioAsignadoId = null,

                        SupportTeamId = null,

                        CategoryId = CategoryId,

                        SubcategoryId = SubcategoryId,

                        PriorityId = prioridadInicialId,

                        StatusId = estadoInicialId,

                        Origen = "Intranet",

                        FechaCreacion = ahora,

                        FechaActualizacion = ahora,

                        FechaAsignacion = null,

                        FechaPrimeraRespuesta = null,

                        FechaResolucion = null,

                        FechaCierre = null,

                        FechaLimiteRespuestaSla =
                            ahora.AddMinutes(
                                prioridad.MinutosRespuesta
                            ),

                        FechaLimiteResolucionSla =
                            ahora.AddMinutes(
                                prioridad.MinutosResolucion
                            ),

                        SlaRespuestaVencido = false,

                        SlaResolucionVencido = false,

                        Resolucion = null,

                        UsuarioCierreId = null,

                        Eliminado = false,

                        FechaEliminacion = null
                    };

                _context.ServiceTickets.Add(ticket);

                await _context.SaveChangesAsync();

                // =================================================
                // HISTORIAL INICIAL
                // =================================================

                ServiceTicketHistory historial =
                    new ServiceTicketHistory
                    {
                        TicketId = ticket.Id,

                        UsuarioId = usuarioActual.Id,

                        Accion = "Creación",

                        Campo = "Ticket",

                        ValorAnterior = null,

                        ValorNuevo = ticket.Folio,

                        Detalle =
                            $"Ticket {ticket.Folio} creado desde la Intranet.",

                        FechaHora = ahora,

                        DireccionIp =
                            ObtenerDireccionIp()
                    };

                _context.ServiceTicketHistories.Add(historial);

                await _context.SaveChangesAsync();

                TempData["MensajeExito"] =
                    $"El ticket {ticket.Folio} fue creado correctamente.";

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al crear un ticket en Mesa de Ayuda."
                );

                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error al crear el ticket."
                );

                await CargarPantallaAsync(usuarioActual);

                return Page();
            }
        }

        // =========================================================
        // OBTENER SUBCATEGORÍAS
        // GET ?handler=Subcategorias&categoryId=1
        // =========================================================

        public async Task<JsonResult> OnGetSubcategoriasAsync(
            int categoryId)
        {
            if (categoryId <= 0)
            {
                return new JsonResult(
                    Array.Empty<object>()
                );
            }

            var subcategorias =
                await _context.ServiceSubcategories
                    .AsNoTracking()
                    .Where(x =>
                        x.CategoryId == categoryId &&
                        x.Activo
                    )
                    .OrderBy(x => x.Orden)
                    .ThenBy(x => x.Nombre)
                    .Select(x => new
                    {
                        id = x.Id,
                        nombre = x.Nombre
                    })
                    .ToListAsync();

            return new JsonResult(subcategorias);
        }

        // =========================================================
        // DETALLE DEL TICKET
        // GET ?handler=Ticket&id=1
        // =========================================================

        public async Task<IActionResult> OnGetTicketAsync(
            int id)
        {
            if (id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "El identificador del ticket no es válido."
                });
            }

            AppUser? usuarioActual =
                await _userManager.GetUserAsync(User);

            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            bool esAdmin =
                EsAdministradorMesa();

            // =====================================================
            // CONSULTAR TICKET
            // =====================================================

            ServiceTicket? ticket =
                await _context.ServiceTickets
                    .AsNoTracking()
                    .Include(x => x.TicketType)
                    .Include(x => x.Status)
                    .Include(x => x.Priority)
                    .Include(x => x.Category)
                    .Include(x => x.Subcategory)
                    .Include(x => x.SupportTeam)
                    .FirstOrDefaultAsync(
                        x => x.Id == id
                    );

            if (ticket == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "No se encontró el ticket solicitado."
                });
            }

            // =====================================================
            // SEGURIDAD
            // =====================================================

            if (
                !esAdmin &&
                ticket.UsuarioSolicitanteId != usuarioActual.Id
            )
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para consultar este ticket."
                    }
                );
            }

            // =====================================================
            // SOLICITANTE
            // =====================================================

            AppUser? solicitante =
                await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                            ticket.UsuarioSolicitanteId
                    );

            // =====================================================
            // USUARIO ASIGNADO
            // =====================================================

            AppUser? usuarioAsignado = null;

            if (!string.IsNullOrWhiteSpace(
                    ticket.UsuarioAsignadoId))
            {
                usuarioAsignado =
                    await _context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                ticket.UsuarioAsignadoId
                        );
            }

            // =====================================================
            // COMENTARIOS
            // =====================================================

            var comentariosQuery =
                _context.ServiceTicketComments
                    .AsNoTracking()
                    .Where(x =>
                        x.TicketId == ticket.Id &&
                        !x.Eliminado
                    );

            /*
             * Las notas internas solamente deben ser visibles
             * para los administradores.
             */
            if (!esAdmin)
            {
                comentariosQuery =
                    comentariosQuery.Where(
                        x => !x.EsNotaInterna
                    );
            }

            var comentarios =
                await (
                    from comentario
                        in comentariosQuery

                    join usuario
                        in _context.Users.AsNoTracking()
                        on comentario.UsuarioId
                        equals usuario.Id
                        into usuarioJoin

                    from usuario
                        in usuarioJoin.DefaultIfEmpty()

                    orderby comentario.FechaCreacion

                    select new
                    {
                        id = comentario.Id,

                        usuarioId =
                            comentario.UsuarioId,

                        usuario =
                            usuario != null
                                ? (
                                    usuario.UserName ??
                                    usuario.Email ??
                                    "Usuario"
                                  )
                                : "Usuario",

                        comentario =
                            comentario.Comentario,

                        esNotaInterna =
                            comentario.EsNotaInterna,

                        fecha =
                            comentario.FechaCreacion
                                .ToString(
                                    "dd/MM/yyyy HH:mm"
                                ),

                        esPropio =
                            comentario.UsuarioId ==
                            usuarioActual.Id
                    }
                )
                .ToListAsync();

            // =====================================================
            // HISTORIAL
            // =====================================================

            var historial =
                await (
                    from evento
                        in _context.ServiceTicketHistories
                            .AsNoTracking()

                    join usuario
                        in _context.Users.AsNoTracking()
                        on evento.UsuarioId
                        equals usuario.Id
                        into usuarioJoin

                    from usuario
                        in usuarioJoin.DefaultIfEmpty()

                    where evento.TicketId == ticket.Id

                    orderby evento.FechaHora descending

                    select new
                    {
                        id = evento.Id,

                        accion =
                            evento.Accion,

                        campo =
                            evento.Campo,

                        valorAnterior =
                            evento.ValorAnterior,

                        valorNuevo =
                            evento.ValorNuevo,

                        detalle =
                            evento.Detalle,

                        usuario =
                            usuario != null
                                ? (
                                    usuario.UserName ??
                                    usuario.Email ??
                                    "Usuario"
                                  )
                                : "Sistema",

                        fecha =
                            evento.FechaHora
                                .ToString(
                                    "dd/MM/yyyy HH:mm"
                                )
                    }
                )
                .ToListAsync();

            // =====================================================
            // CATÁLOGOS ADMINISTRATIVOS
            // =====================================================

            object[] tecnicos =
                Array.Empty<object>();

            object[] estados =
                Array.Empty<object>();

            object[] prioridades =
                Array.Empty<object>();

            if (esAdmin)
            {
                tecnicos =
                    await ObtenerTecnicosAsync();

                estados =
                    (
                        await _context.ServiceTicketStatuses
                            .AsNoTracking()
                            .Where(x => x.Activo)
                            .OrderBy(x => x.Orden)
                            .Select(x => new
                            {
                                id = x.Id,
                                nombre = x.Nombre,
                                codigo = x.Codigo
                            })
                            .ToListAsync()
                    )
                    .Cast<object>()
                    .ToArray();

                prioridades =
                    (
                        await _context.ServiceTicketPriorities
                            .AsNoTracking()
                            .Where(x => x.Activo)
                            .OrderBy(x => x.Nivel)
                            .Select(x => new
                            {
                                id = x.Id,
                                nombre = x.Nombre,
                                codigo = x.Codigo
                            })
                            .ToListAsync()
                    )
                    .Cast<object>()
                    .ToArray();
            }

            // =====================================================
            // RESPUESTA
            // =====================================================

            return new JsonResult(new
            {
                success = true,

                esAdmin,

                ticket = new
                {
                    id = ticket.Id,

                    folio =
                        ticket.Folio,

                    titulo =
                        ticket.Titulo,

                    descripcion =
                        ticket.Descripcion,

                    tipoId =
                        ticket.TicketTypeId,

                    tipo =
                        ticket.TicketType?.Nombre ??
                        "-",

                    categoriaId =
                        ticket.CategoryId,

                    categoria =
                        ticket.Category?.Nombre ??
                        "-",

                    subcategoriaId =
                        ticket.SubcategoryId,

                    subcategoria =
                        ticket.Subcategory?.Nombre ??
                        "-",

                    prioridadId =
                        ticket.PriorityId,

                    prioridad =
                        ticket.Priority?.Nombre ??
                        "-",

                    prioridadCodigo =
                        ticket.Priority?.Codigo ??
                        string.Empty,

                    estadoId =
                        ticket.StatusId,

                    estado =
                        ticket.Status?.Nombre ??
                        "-",

                    estadoCodigo =
                        ticket.Status?.Codigo ??
                        string.Empty,

                    origen =
                        ticket.Origen,

                    solicitanteId =
                        ticket.UsuarioSolicitanteId,

                    solicitante =
                        solicitante != null
                            ? (
                                solicitante.UserName ??
                                solicitante.Email ??
                                "Usuario"
                              )
                            : "Usuario",

                    solicitanteCorreo =
                        solicitante?.Email ??
                        string.Empty,

                    usuarioAsignadoId =
                        ticket.UsuarioAsignadoId,

                    usuarioAsignado =
                        usuarioAsignado != null
                            ? (
                                usuarioAsignado.UserName ??
                                usuarioAsignado.Email ??
                                "Usuario"
                              )
                            : string.Empty,

                    equipoSoporte =
                        ticket.SupportTeam?.Nombre ??
                        string.Empty,

                    fechaCreacion =
                        ticket.FechaCreacion
                            .ToString(
                                "dd/MM/yyyy HH:mm"
                            ),

                    fechaActualizacion =
                        ticket.FechaActualizacion
                            ?.ToString(
                                "dd/MM/yyyy HH:mm"
                            ),

                    fechaAsignacion =
                        ticket.FechaAsignacion
                            ?.ToString(
                                "dd/MM/yyyy HH:mm"
                            ),

                    fechaPrimeraRespuesta =
                        ticket.FechaPrimeraRespuesta
                            ?.ToString(
                                "dd/MM/yyyy HH:mm"
                            ),

                    fechaResolucion =
                        ticket.FechaResolucion
                            ?.ToString(
                                "dd/MM/yyyy HH:mm"
                            ),

                    fechaCierre =
                        ticket.FechaCierre
                            ?.ToString(
                                "dd/MM/yyyy HH:mm"
                            ),

                    fechaLimiteRespuestaSla =
                        ticket.FechaLimiteRespuestaSla
                            ?.ToString(
                                "dd/MM/yyyy HH:mm"
                            ),

                    fechaLimiteResolucionSla =
                        ticket.FechaLimiteResolucionSla
                            ?.ToString(
                                "dd/MM/yyyy HH:mm"
                            ),

                    slaRespuestaVencido =
                        ticket.SlaRespuestaVencido,

                    slaResolucionVencido =
                        ticket.SlaResolucionVencido,

                    resolucion =
                        ticket.Resolucion ??
                        string.Empty
                },

                comentarios,

                historial,

                tecnicos,

                estados,

                prioridades
            });
        }

        // =========================================================
        // ACTUALIZAR TICKET
        // POST ?handler=ActualizarTicket
        // =========================================================

        public async Task<IActionResult> OnPostActualizarTicketAsync(
            [FromBody] ActualizarTicketRequest request)
        {
            // =====================================================
            // VALIDAR USUARIO
            // =====================================================

            AppUser? usuarioActual =
                await _userManager.GetUserAsync(User);

            if (usuarioActual == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message = "La sesión del usuario no es válida."
                    }
                )
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }

            // =====================================================
            // VALIDAR ADMINISTRADOR
            // =====================================================

            if (!EsAdministradorMesa())
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para modificar tickets."
                    }
                )
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }

            // =====================================================
            // VALIDAR REQUEST
            // =====================================================

            if (request == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se recibió información para actualizar."
                    }
                );
            }

            if (request.TicketId <= 0)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El identificador del ticket no es válido."
                    }
                );
            }

            if (request.PriorityId <= 0)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Debes seleccionar una prioridad."
                    }
                );
            }

            if (request.StatusId <= 0)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Debes seleccionar un estado."
                    }
                );
            }

            try
            {
                // =================================================
                // OBTENER TICKET
                // =================================================

                ServiceTicket? ticket =
                    await _context.ServiceTickets
                        .FirstOrDefaultAsync(
                            x => x.Id == request.TicketId
                        );

                if (ticket == null)
                {
                    return new JsonResult(
                        new
                        {
                            success = false,
                            message =
                                "No se encontró el ticket solicitado."
                        }
                    );
                }

                // =================================================
                // VALIDAR PRIORIDAD
                // =================================================

                ServiceTicketPriority? nuevaPrioridad =
                    await _context.ServiceTicketPriorities
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id == request.PriorityId &&
                                x.Activo
                        );

                if (nuevaPrioridad == null)
                {
                    return new JsonResult(
                        new
                        {
                            success = false,
                            message =
                                "La prioridad seleccionada no es válida."
                        }
                    );
                }

                // =================================================
                // VALIDAR ESTADO
                // =================================================

                ServiceTicketStatus? nuevoEstado =
                    await _context.ServiceTicketStatuses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id == request.StatusId &&
                                x.Activo
                        );

                if (nuevoEstado == null)
                {
                    return new JsonResult(
                        new
                        {
                            success = false,
                            message =
                                "El estado seleccionado no es válido."
                        }
                    );
                }

                // =================================================
                // INFORMACIÓN ANTERIOR
                // =================================================

                ServiceTicketPriority? prioridadAnterior =
                    await _context.ServiceTicketPriorities
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x => x.Id == ticket.PriorityId
                        );

                ServiceTicketStatus? estadoAnterior =
                    await _context.ServiceTicketStatuses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x => x.Id == ticket.StatusId
                        );

                string? usuarioAsignadoAnteriorId =
                    ticket.UsuarioAsignadoId;

                AppUser? usuarioAsignadoAnterior =
                    null;

                if (!string.IsNullOrWhiteSpace(
                        usuarioAsignadoAnteriorId))
                {
                    usuarioAsignadoAnterior =
                        await _context.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                x =>
                                    x.Id ==
                                    usuarioAsignadoAnteriorId
                            );
                }

                // =================================================
                // VALIDAR TÉCNICO
                // =================================================

                string? nuevoUsuarioAsignadoId =
                    string.IsNullOrWhiteSpace(
                        request.UsuarioAsignadoId
                    )
                        ? null
                        : request.UsuarioAsignadoId.Trim();

                AppUser? nuevoUsuarioAsignado =
                    null;

                ServiceSupportTeamUser? relacionTecnico =
                    null;

                if (!string.IsNullOrWhiteSpace(
                        nuevoUsuarioAsignadoId))
                {
                    nuevoUsuarioAsignado =
                        await _context.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                x =>
                                    x.Id ==
                                    nuevoUsuarioAsignadoId
                            );

                    if (nuevoUsuarioAsignado == null)
                    {
                        return new JsonResult(
                            new
                            {
                                success = false,
                                message =
                                    "El técnico seleccionado no existe."
                            }
                        );
                    }

                    relacionTecnico =
                        await _context.ServiceSupportTeamUsers
                            .AsNoTracking()
                            .FirstOrDefaultAsync(
                                x =>
                                    x.UserId ==
                                    nuevoUsuarioAsignadoId &&
                                    x.Activo
                            );

                    if (relacionTecnico == null)
                    {
                        return new JsonResult(
                            new
                            {
                                success = false,
                                message =
                                    "El usuario seleccionado no pertenece a un equipo de soporte activo."
                            }
                        );
                    }
                }

                DateTime ahora =
                    DateTime.Now;

                bool huboCambios =
                    false;

                // =================================================
                // CAMBIO DE TÉCNICO
                // =================================================

                if (
                    ticket.UsuarioAsignadoId !=
                    nuevoUsuarioAsignadoId
                )
                {
                    string tecnicoAnteriorNombre =
                        usuarioAsignadoAnterior != null
                            ? (
                                usuarioAsignadoAnterior.UserName ??
                                usuarioAsignadoAnterior.Email ??
                                "Usuario"
                            )
                            : "Sin asignar";

                    string tecnicoNuevoNombre =
                        nuevoUsuarioAsignado != null
                            ? (
                                nuevoUsuarioAsignado.UserName ??
                                nuevoUsuarioAsignado.Email ??
                                "Usuario"
                            )
                            : "Sin asignar";

                    ServiceTicketHistory historialAsignacion =
                        new ServiceTicketHistory
                        {
                            TicketId =
                                ticket.Id,

                            UsuarioId =
                                usuarioActual.Id,

                            Accion =
                                "Asignación",

                            Campo =
                                "UsuarioAsignadoId",

                            ValorAnterior =
                                tecnicoAnteriorNombre,

                            ValorNuevo =
                                tecnicoNuevoNombre,

                            Detalle =
                                $"Asignación modificada de \"{tecnicoAnteriorNombre}\" a \"{tecnicoNuevoNombre}\".",

                            FechaHora =
                                ahora,

                            DireccionIp =
                                ObtenerDireccionIp()
                        };

                    _context.ServiceTicketHistories.Add(
                        historialAsignacion
                    );

                    ticket.UsuarioAsignadoId =
                        nuevoUsuarioAsignadoId;

                    if (
                        nuevoUsuarioAsignadoId != null &&
                        relacionTecnico != null
                    )
                    {
                        ticket.SupportTeamId =
                            relacionTecnico.SupportTeamId;

                        ticket.FechaAsignacion =
                            ahora;
                    }
                    else
                    {
                        ticket.SupportTeamId =
                            null;

                        ticket.FechaAsignacion =
                            null;
                    }

                    huboCambios =
                        true;
                }

                // =================================================
                // CAMBIO DE PRIORIDAD
                // =================================================

                if (
                    ticket.PriorityId !=
                    request.PriorityId
                )
                {
                    string prioridadAnteriorNombre =
                        prioridadAnterior?.Nombre ??
                        ticket.PriorityId.ToString();

                    string prioridadNuevaNombre =
                        nuevaPrioridad.Nombre;

                    ServiceTicketHistory historialPrioridad =
                        new ServiceTicketHistory
                        {
                            TicketId =
                                ticket.Id,

                            UsuarioId =
                                usuarioActual.Id,

                            Accion =
                                "Cambio de prioridad",

                            Campo =
                                "PriorityId",

                            ValorAnterior =
                                prioridadAnteriorNombre,

                            ValorNuevo =
                                prioridadNuevaNombre,

                            Detalle =
                                $"Prioridad modificada de \"{prioridadAnteriorNombre}\" a \"{prioridadNuevaNombre}\".",

                            FechaHora =
                                ahora,

                            DireccionIp =
                                ObtenerDireccionIp()
                        };

                    _context.ServiceTicketHistories.Add(
                        historialPrioridad
                    );

                    ticket.PriorityId =
                        request.PriorityId;

                    // =============================================
                    // RECALCULAR SLA
                    // =============================================

                    if (
                        ticket.FechaPrimeraRespuesta ==
                        null
                    )
                    {
                        ticket.FechaLimiteRespuestaSla =
                            ticket.FechaCreacion.AddMinutes(
                                nuevaPrioridad.MinutosRespuesta
                            );
                    }

                    if (
                        ticket.FechaResolucion ==
                        null
                    )
                    {
                        ticket.FechaLimiteResolucionSla =
                            ticket.FechaCreacion.AddMinutes(
                                nuevaPrioridad.MinutosResolucion
                            );
                    }

                    huboCambios =
                        true;
                }

                // =================================================
                // CAMBIO DE ESTADO
                // =================================================

                if (
                    ticket.StatusId !=
                    request.StatusId
                )
                {
                    string estadoAnteriorNombre =
                        estadoAnterior?.Nombre ??
                        ticket.StatusId.ToString();

                    string estadoNuevoNombre =
                        nuevoEstado.Nombre;

                    ServiceTicketHistory historialEstado =
                        new ServiceTicketHistory
                        {
                            TicketId =
                                ticket.Id,

                            UsuarioId =
                                usuarioActual.Id,

                            Accion =
                                "Cambio de estado",

                            Campo =
                                "StatusId",

                            ValorAnterior =
                                estadoAnteriorNombre,

                            ValorNuevo =
                                estadoNuevoNombre,

                            Detalle =
                                $"Estado modificado de \"{estadoAnteriorNombre}\" a \"{estadoNuevoNombre}\".",

                            FechaHora =
                                ahora,

                            DireccionIp =
                                ObtenerDireccionIp()
                        };

                    _context.ServiceTicketHistories.Add(
                        historialEstado
                    );

                    ticket.StatusId =
                        request.StatusId;

                    // =============================================
                    // PRIMERA RESPUESTA
                    // =============================================

                    if (
                        ticket.FechaPrimeraRespuesta ==
                        null
                    )
                    {
                        ticket.FechaPrimeraRespuesta =
                            ahora;
                    }

                    // =============================================
                    // ESTADO RESUELTO
                    // Id 5
                    // =============================================

                    if (
                        request.StatusId == 5
                    )
                    {
                        ticket.FechaResolucion =
                            ahora;

                        ticket.FechaCierre =
                            null;
                    }

                    // =============================================
                    // ESTADO CERRADO
                    // Id 6
                    // =============================================

                    if (
                        request.StatusId == 6
                    )
                    {
                        if (
                            ticket.FechaResolucion ==
                            null
                        )
                        {
                            ticket.FechaResolucion =
                                ahora;
                        }

                        ticket.FechaCierre =
                            ahora;
                    }

                    // =============================================
                    // ESTADO REABIERTO
                    // Id 7
                    // =============================================

                    if (
                        request.StatusId == 7
                    )
                    {
                        ticket.FechaCierre =
                            null;
                    }

                    // =============================================
                    // ESTADO CANCELADO
                    // Id 8
                    // =============================================

                    if (
                        request.StatusId == 8
                    )
                    {
                        ticket.FechaCierre =
                            ahora;
                    }

                    // =============================================
                    // ESTADOS ACTIVOS
                    // =============================================

                    if (
                        request.StatusId == 1 ||
                        request.StatusId == 2 ||
                        request.StatusId == 3 ||
                        request.StatusId == 4
                    )
                    {
                        ticket.FechaCierre =
                            null;
                    }

                    huboCambios =
                        true;
                }

                // =================================================
                // SIN CAMBIOS
                // =================================================

                if (!huboCambios)
                {
                    return new JsonResult(
                        new
                        {
                            success = true,
                            changed = false,
                            message =
                                "No se detectaron cambios en el ticket."
                        }
                    );
                }

                // =================================================
                // FECHA ACTUALIZACIÓN
                // =================================================

                ticket.FechaActualizacion =
                    ahora;

                // =================================================
                // SLA RESPUESTA
                // =================================================

                if (
                    ticket.FechaPrimeraRespuesta ==
                    null &&
                    ticket.FechaLimiteRespuestaSla.HasValue
                )
                {
                    ticket.SlaRespuestaVencido =
                        ahora >
                        ticket.FechaLimiteRespuestaSla.Value;
                }
                else
                {
                    ticket.SlaRespuestaVencido =
                        false;
                }

                // =================================================
                // SLA RESOLUCIÓN
                // =================================================

                if (
                    ticket.FechaResolucion ==
                    null &&
                    ticket.FechaLimiteResolucionSla.HasValue
                )
                {
                    ticket.SlaResolucionVencido =
                        ahora >
                        ticket.FechaLimiteResolucionSla.Value;
                }
                else
                {
                    ticket.SlaResolucionVencido =
                        false;
                }

                // =================================================
                // GUARDAR
                // =================================================

                await _context.SaveChangesAsync();

                return new JsonResult(
                    new
                    {
                        success = true,
                        changed = true,
                        message =
                            $"El ticket {ticket.Folio} fue actualizado correctamente."
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al actualizar el ticket {TicketId}.",
                    request.TicketId
                );

                Response.StatusCode =
                    StatusCodes.Status500InternalServerError;

                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Ocurrió un error al actualizar el ticket."
                    }
                );
            }
        }

        // =========================================================
        // CARGAR PANTALLA
        // =========================================================

        private async Task CargarPantallaAsync(
            AppUser usuarioActual)
        {
            EsAdmin =
                EsAdministradorMesa();

            await CargarCatalogosAsync();

            IQueryable<ServiceTicket> query =
                _context.ServiceTickets
                    .AsNoTracking()
                    .Include(x => x.TicketType)
                    .Include(x => x.Status)
                    .Include(x => x.Priority)
                    .Include(x => x.Category)
                    .Include(x => x.Subcategory)
                    .Include(x => x.SupportTeam);

            if (!EsAdmin)
            {
                query =
                    query.Where(
                        x =>
                            x.UsuarioSolicitanteId ==
                            usuarioActual.Id
                    );
            }

            Tickets =
                await query
                    .OrderByDescending(
                        x => x.FechaCreacion
                    )
                    .ToListAsync();

            CalcularKpis();
        }

        // =========================================================
        // CATÁLOGOS
        // =========================================================

        private async Task CargarCatalogosAsync()
        {
            TiposTicket =
                await _context.ServiceTicketTypes
                    .AsNoTracking()
                    .Where(x => x.Activo)
                    .OrderBy(x => x.Orden)
                    .Select(x =>
                        new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Nombre
                        }
                    )
                    .ToListAsync();

            Categorias =
                await _context.ServiceCategories
                    .AsNoTracking()
                    .Where(x => x.Activo)
                    .OrderBy(x => x.Orden)
                    .Select(x =>
                        new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.Nombre
                        }
                    )
                    .ToListAsync();

            if (
                CategoryId > 0
            )
            {
                Subcategorias =
                    await _context.ServiceSubcategories
                        .AsNoTracking()
                        .Where(x =>
                            x.Activo &&
                            x.CategoryId ==
                            CategoryId
                        )
                        .OrderBy(x => x.Orden)
                        .Select(x =>
                            new SelectListItem
                            {
                                Value =
                                    x.Id.ToString(),

                                Text =
                                    x.Nombre
                            }
                        )
                        .ToListAsync();
            }
            else
            {
                Subcategorias =
                    new List<SelectListItem>();
            }
        }

        // =========================================================
        // KPIs
        // =========================================================

        private void CalcularKpis()
        {
            /*
             * Estados:
             *
             * 1 Nuevo
             * 2 Asignado
             * 3 En proceso
             * 4 Pendiente usuario
             * 5 Resuelto
             * 6 Cerrado
             * 7 Reabierto
             * 8 Cancelado
             */

            TotalAbiertos =
                Tickets.Count(
                    x =>
                        x.StatusId != 5 &&
                        x.StatusId != 6 &&
                        x.StatusId != 8
                );

            TotalNuevos =
                Tickets.Count(
                    x => x.StatusId == 1
                );

            TotalEnProceso =
                Tickets.Count(
                    x => x.StatusId == 3
                );

            TotalSinAsignar =
                Tickets.Count(
                    x =>
                        string.IsNullOrWhiteSpace(
                            x.UsuarioAsignadoId
                        ) &&
                        x.StatusId != 5 &&
                        x.StatusId != 6 &&
                        x.StatusId != 8
                );

            TotalResueltos =
                Tickets.Count(
                    x =>
                        x.StatusId == 5 ||
                        x.StatusId == 6
                );
        }

        // =========================================================
        // OBTENER TÉCNICOS
        // =========================================================

        private async Task<object[]> ObtenerTecnicosAsync()
        {
            var tecnicos =
                await (
                    from relacion
                        in _context.ServiceSupportTeamUsers
                            .AsNoTracking()

                    join usuario
                        in _context.Users.AsNoTracking()
                        on relacion.UserId
                        equals usuario.Id

                    where relacion.Activo

                    orderby usuario.UserName

                    select new
                    {
                        id =
                            usuario.Id,

                        nombre =
                            usuario.UserName ??
                            usuario.Email ??
                            "Usuario",

                        correo =
                            usuario.Email ??
                            string.Empty,

                        equipoId =
                            relacion.SupportTeamId,

                        esResponsable =
                            relacion.EsResponsable
                    }
                )
                .Distinct()
                .ToListAsync();

            return tecnicos
                .Cast<object>()
                .ToArray();
        }

        // =========================================================
        // GENERAR FOLIO
        // =========================================================

        private async Task<string> GenerarFolioAsync()
        {
            /*
             * Ejemplo:
             *
             * SD-20260820-00001
             */

            DateTime hoy =
                DateTime.Now.Date;

            DateTime manana =
                hoy.AddDays(1);

            int consecutivo =
                await _context.ServiceTickets
                    .IgnoreQueryFilters()
                    .CountAsync(
                        x =>
                            x.FechaCreacion >= hoy &&
                            x.FechaCreacion < manana
                    )
                + 1;

            string folio;

            do
            {
                folio =
                    $"SD-{hoy:yyyyMMdd}-{consecutivo:D5}";

                bool existe =
                    await _context.ServiceTickets
                        .IgnoreQueryFilters()
                        .AnyAsync(
                            x => x.Folio == folio
                        );

                if (!existe)
                {
                    return folio;
                }

                consecutivo++;
            }
            while (true);
        }

        // =========================================================
        // ADMINISTRADOR MESA DE AYUDA
        // =========================================================

        private bool EsAdministradorMesa()
        {
            return
                User.IsInRole("Administrador") ||
                User.IsInRole("Master");
        }

        // =========================================================
        // IP
        // =========================================================

        private string ObtenerDireccionIp()
        {
            string ip =
                HttpContext
                    .Connection
                    .RemoteIpAddress
                    ?.ToString()
                ?? string.Empty;

            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }

            return ip;
        }
    }
}