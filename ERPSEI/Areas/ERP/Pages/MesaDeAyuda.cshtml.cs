using ERPSEI.Data;
using ERPSEI.Data.Entities.ServiceDesk;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tsp;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Areas.ERP.Pages
{
    [Authorize]
    public class MesaDeAyudaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly ILogger<MesaDeAyudaModel> _logger;
        private readonly IWebHostEnvironment _environment;

        public MesaDeAyudaModel(
        ApplicationDbContext context,
        AppUserManager userManager,
        ILogger<MesaDeAyudaModel> logger,
        IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _environment = environment;
        }

        public class ActualizarTicketRequest
        {
            public int TicketId { get; set; }

            public string? UsuarioAsignadoId { get; set; }

            public int PriorityId { get; set; }

            public int StatusId { get; set; }

            public string? Resolucion { get; set; }
        }

        public class AgregarComentarioTicketRequest
        {
            public int TicketId { get; set; }

            public string Comentario { get; set; } =
                string.Empty;

            public bool EsNotaInterna { get; set; }
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
            // USUARIO QUE CERRÓ EL TICKET
            // =====================================================

            AppUser? usuarioCierre =
                null;

            if (
                !string.IsNullOrWhiteSpace(
                    ticket.UsuarioCierreId
                )
            )
            {
                usuarioCierre =
                    await _context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                ticket.UsuarioCierreId
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
                        id =
                            comentario.Id,

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
                            ),

                        direccionIp =
                        esAdmin
                            ? evento.DireccionIp
                            : null
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

                    usuarioCierreId =
                    ticket.UsuarioCierreId,

                    usuarioCierre =
                    usuarioCierre != null
                        ? (
                            usuarioCierre.UserName ??
                            usuarioCierre.Email ??
                            "Usuario"
                        )
                        : string.Empty,

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
        // OBTENER ADJUNTOS DEL TICKET
        // GET ?handler=Adjuntos&ticketId=1
        // =========================================================

        public async Task<IActionResult> OnGetAdjuntosAsync(
            int ticketId)
        {
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
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }

            ServiceTicket? ticket =
                await _context.ServiceTickets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == ticketId
                    );

            if (ticket == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message = "No se encontró el ticket."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }

            bool esAdmin =
                EsAdministradorMesa();

            if (
                !esAdmin &&
                ticket.UsuarioSolicitanteId !=
                usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para consultar los adjuntos de este ticket."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }

            var adjuntos =
                await (
                    from adjunto
                        in _context.ServiceTicketAttachments
                            .AsNoTracking()

                    join usuario
                        in _context.Users.AsNoTracking()
                        on adjunto.UsuarioCargaId
                        equals usuario.Id

                    where
                        adjunto.TicketId == ticketId &&
                        !adjunto.Eliminado

                    orderby adjunto.FechaCarga descending

                    select new
                    {
                        id =
                            adjunto.Id,

                        nombre =
                            adjunto.NombreOriginal,

                        extension =
                            adjunto.Extension ?? string.Empty,

                        mimeType =
                            adjunto.MimeType ?? string.Empty,

                        tamanoBytes =
                            adjunto.TamanoBytes,

                        tamano =
                            FormatearTamanoArchivo(
                                adjunto.TamanoBytes
                            ),

                        usuario =
                            usuario.UserName ??
                            usuario.Email ??
                            "Usuario",

                        fecha =
                            adjunto.FechaCarga
                                .ToString(
                                    "dd/MM/yyyy HH:mm"
                                ),

                        urlDescarga =
                            $"?handler=DescargarAdjunto&id={adjunto.Id}"
                    }
                )
                .ToListAsync();

            return new JsonResult(
                new
                {
                    success = true,
                    adjuntos
                }
            );
        }

        // =========================================================
        // SUBIR ADJUNTO
        // POST ?handler=SubirAdjunto
        // =========================================================

        public async Task<IActionResult> OnPostSubirAdjuntoAsync(
            int ticketId,
            IFormFile? archivo)
        {
            AppUser? usuarioActual =
                await _userManager.GetUserAsync(User);

            if (usuarioActual == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La sesión del usuario no es válida."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }

            if (ticketId <= 0)
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

            ServiceTicket? ticket =
                await _context.ServiceTickets
                    .FirstOrDefaultAsync(
                        x => x.Id == ticketId
                    );

            if (ticket == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró el ticket."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }

            bool esAdmin =
                EsAdministradorMesa();

            if (
                !esAdmin &&
                ticket.UsuarioSolicitanteId !=
                usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para adjuntar archivos a este ticket."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }

            if (
                archivo == null ||
                archivo.Length <= 0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Selecciona un archivo."
                    }
                );
            }

            // =====================================================
            // MÁXIMO 10 MB
            // =====================================================

            const long tamanoMaximo =
                10 * 1024 * 1024;

            if (
                archivo.Length >
                tamanoMaximo
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El archivo no puede superar los 10 MB."
                    }
                );
            }

            // =====================================================
            // EXTENSIONES PERMITIDAS
            // =====================================================

            string extension =
                Path.GetExtension(
                    archivo.FileName
                )
                .ToLowerInvariant();

            string[] extensionesPermitidas =
            {
        ".png",
        ".jpg",
        ".jpeg",
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".txt"
    };

            if (
                !extensionesPermitidas.Contains(
                    extension
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Tipo de archivo no permitido. Se permiten PNG, JPG, PDF, Word, Excel y TXT."
                    }
                );
            }

            try
            {
                // =================================================
                // CARPETA SEGURA
                // FUERA DE WWWROOT
                // =================================================

                string carpetaTicket =
                    Path.Combine(
                        _environment.ContentRootPath,
                        "App_Data",
                        "MesaDeAyuda",
                        "Tickets",
                        ticket.Id.ToString()
                    );

                Directory.CreateDirectory(
                    carpetaTicket
                );

                // =================================================
                // NOMBRE INTERNO ALEATORIO
                // =================================================

                string nombreAlmacenado =
                    $"{Guid.NewGuid():N}{extension}";

                string rutaFisica =
                    Path.Combine(
                        carpetaTicket,
                        nombreAlmacenado
                    );

                // =================================================
                // GUARDAR ARCHIVO
                // =================================================

                await using (
                    FileStream stream =
                        new FileStream(
                            rutaFisica,
                            FileMode.CreateNew,
                            FileAccess.Write,
                            FileShare.None
                        )
                )
                {
                    await archivo.CopyToAsync(
                        stream
                    );
                }

                // =================================================
                // RUTA RELATIVA INTERNA
                // =================================================

                string rutaRelativa =
                    Path.Combine(
                        "App_Data",
                        "MesaDeAyuda",
                        "Tickets",
                        ticket.Id.ToString(),
                        nombreAlmacenado
                    )
                    .Replace(
                        "\\",
                        "/"
                    );

                DateTime ahora =
                    DateTime.Now;

                // =================================================
                // REGISTRO EN BD
                // =================================================

                ServiceTicketAttachment adjunto =
                    new ServiceTicketAttachment
                    {
                        TicketId =
                            ticket.Id,

                        NombreOriginal =
                            Path.GetFileName(
                                archivo.FileName
                            ),

                        NombreAlmacenado =
                            nombreAlmacenado,

                        RutaArchivo =
                            rutaRelativa,

                        Extension =
                            extension,

                        MimeType =
                            archivo.ContentType,

                        TamanoBytes =
                            archivo.Length,

                        UsuarioCargaId =
                            usuarioActual.Id,

                        FechaCarga =
                            ahora,

                        Eliminado =
                            false
                    };

                _context.ServiceTicketAttachments.Add(
                    adjunto
                );

                // =================================================
                // ACTUALIZAR FECHA TICKET
                // =================================================

                ticket.FechaActualizacion =
                    ahora;

                // =================================================
                // HISTORIAL
                // =================================================

                ServiceTicketHistory historial =
                    new ServiceTicketHistory
                    {
                        TicketId =
                            ticket.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        Accion =
                            "Adjunto",

                        Campo =
                            "Archivo",

                        ValorAnterior =
                            null,

                        ValorNuevo =
                            adjunto.NombreOriginal,

                        Detalle =
                            $"Se adjuntó el archivo \"{adjunto.NombreOriginal}\".",

                        FechaHora =
                            ahora,

                        DireccionIp =
                            ObtenerDireccionIp()
                    };

                _context.ServiceTicketHistories.Add(
                    historial
                );

                await _context.SaveChangesAsync();

                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            "Archivo adjuntado correctamente."
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al adjuntar archivo al ticket {TicketId}.",
                    ticketId
                );

                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Ocurrió un error al guardar el archivo."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }

        // =========================================================
        // DESCARGAR ADJUNTO
        // GET ?handler=DescargarAdjunto&id=1
        // =========================================================

        public async Task<IActionResult>
            OnGetDescargarAdjuntoAsync(
                int id)
        {
            AppUser? usuarioActual =
                await _userManager.GetUserAsync(User);

            if (usuarioActual == null)
            {
                return Unauthorized();
            }

            ServiceTicketAttachment? adjunto =
                await _context.ServiceTicketAttachments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == id &&
                            !x.Eliminado
                    );

            if (adjunto == null)
            {
                return NotFound();
            }

            ServiceTicket? ticket =
                await _context.ServiceTickets
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                            adjunto.TicketId
                    );

            if (ticket == null)
            {
                return NotFound();
            }

            bool esAdmin =
                EsAdministradorMesa();

            if (
                !esAdmin &&
                ticket.UsuarioSolicitanteId !=
                usuarioActual.Id
            )
            {
                return Forbid();
            }

            // =====================================================
            // OBTENER RUTA FÍSICA
            // =====================================================

            string raizPermitida =
                Path.GetFullPath(
                    Path.Combine(
                        _environment.ContentRootPath,
                        "App_Data",
                        "MesaDeAyuda"
                    )
                );

            string rutaFisica =
                Path.GetFullPath(
                    Path.Combine(
                        _environment.ContentRootPath,
                        adjunto.RutaArchivo
                            .Replace(
                                "/",
                                Path.DirectorySeparatorChar.ToString()
                            )
                    )
                );

            // =====================================================
            // EVITAR PATH TRAVERSAL
            // =====================================================

            if (
                !rutaFisica.StartsWith(
                    raizPermitida,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return Forbid();
            }

            if (
                !System.IO.File.Exists(
                    rutaFisica
                )
            )
            {
                return NotFound();
            }

            byte[] contenido =
                await System.IO.File.ReadAllBytesAsync(
                    rutaFisica
                );

            string mimeType =
                string.IsNullOrWhiteSpace(
                    adjunto.MimeType
                )
                    ? "application/octet-stream"
                    : adjunto.MimeType;

            return File(
                contenido,
                mimeType,
                adjunto.NombreOriginal
            );
        }

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
                        message =
                            "La sesión del usuario no es válida."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
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
                    StatusCode =
                        StatusCodes.Status403Forbidden
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
                            x =>
                                x.Id ==
                                request.TicketId
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
                // CATÁLOGOS
                // =================================================

                ServiceTicketPriority? nuevaPrioridad =
                    await _context.ServiceTicketPriorities
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                request.PriorityId &&
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


                ServiceTicketStatus? nuevoEstado =
                    await _context.ServiceTicketStatuses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                request.StatusId &&
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
                            x =>
                                x.Id ==
                                ticket.PriorityId
                        );

                ServiceTicketStatus? estadoAnterior =
                    await _context.ServiceTicketStatuses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                ticket.StatusId
                        );


                int estadoAnteriorId =
                    ticket.StatusId;

                string? resolucionAnterior =
                    ticket.Resolucion;

                DateTime? fechaResolucionAnterior =
                    ticket.FechaResolucion;

                DateTime? fechaCierreAnterior =
                    ticket.FechaCierre;

                string? usuarioAsignadoAnteriorId =
                    ticket.UsuarioAsignadoId;


                AppUser? usuarioAsignadoAnterior =
                    null;


                if (
                    !string.IsNullOrWhiteSpace(
                        usuarioAsignadoAnteriorId
                    )
                )
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


                if (
                    !string.IsNullOrWhiteSpace(
                        nuevoUsuarioAsignadoId
                    )
                )
                {
                    nuevoUsuarioAsignado =
                        await _context.Users
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


                    bool esAdministradorTi =
                        await _userManager.IsInRoleAsync(
                            nuevoUsuarioAsignado,
                            "Administrador TI"
                        );


                    if (!esAdministradorTi)
                    {
                        return new JsonResult(
                            new
                            {
                                success = false,
                                message =
                                    "El usuario seleccionado no cuenta con el rol Administrador TI."
                            }
                        )
                        {
                            StatusCode =
                                StatusCodes.Status403Forbidden
                        };
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
                }


                // =================================================
                // ESTADOS
                // =================================================

                const int ESTADO_NUEVO = 1;
                const int ESTADO_ASIGNADO = 2;
                const int ESTADO_EN_PROCESO = 3;
                const int ESTADO_PENDIENTE_USUARIO = 4;
                const int ESTADO_RESUELTO = 5;
                const int ESTADO_CERRADO = 6;
                const int ESTADO_REABIERTO = 7;
                const int ESTADO_CANCELADO = 8;


                string? resolucion =
                    string.IsNullOrWhiteSpace(
                        request.Resolucion
                    )
                        ? null
                        : request.Resolucion.Trim();


                // =================================================
                // VALIDAR RESOLUCIÓN
                // =================================================

                if (
                    request.StatusId ==
                    ESTADO_RESUELTO
                )
                {
                    if (
                        string.IsNullOrWhiteSpace(
                            resolucion
                        )
                    )
                    {
                        return new JsonResult(
                            new
                            {
                                success = false,
                                message =
                                    "Debes registrar la resolución antes de marcar el ticket como Resuelto."
                            }
                        );
                    }


                    if (
                        resolucion.Length >
                        5000
                    )
                    {
                        return new JsonResult(
                            new
                            {
                                success = false,
                                message =
                                    "La resolución no puede superar los 5000 caracteres."
                            }
                        );
                    }
                }


                // =================================================
                // VALIDAR CIERRE
                // Resuelto -> Cerrado
                // =================================================

                if (
                    request.StatusId ==
                        ESTADO_CERRADO &&
                    estadoAnteriorId !=
                        ESTADO_RESUELTO &&
                    estadoAnteriorId !=
                        ESTADO_CERRADO
                )
                {
                    return new JsonResult(
                        new
                        {
                            success = false,
                            message =
                                "El ticket debe estar Resuelto antes de poder cerrarlo."
                        }
                    );
                }


                // =================================================
                // VALIDAR REAPERTURA
                // Resuelto/Cerrado -> Reabierto
                // =================================================

                if (
                    request.StatusId ==
                        ESTADO_REABIERTO &&
                    estadoAnteriorId !=
                        ESTADO_RESUELTO &&
                    estadoAnteriorId !=
                        ESTADO_CERRADO &&
                    estadoAnteriorId !=
                        ESTADO_REABIERTO
                )
                {
                    return new JsonResult(
                        new
                        {
                            success = false,
                            message =
                                "Solamente un ticket Resuelto o Cerrado puede ser reabierto."
                        }
                    );
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


                    _context.ServiceTicketHistories.Add(
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
                                $"El técnico asignado cambió de \"{tecnicoAnteriorNombre}\" a \"{tecnicoNuevoNombre}\".",

                            FechaHora =
                                ahora,

                            DireccionIp =
                                ObtenerDireccionIp()
                        }
                    );


                    ticket.UsuarioAsignadoId =
                        nuevoUsuarioAsignadoId;


                    if (
                        nuevoUsuarioAsignadoId !=
                        null
                    )
                    {
                        ticket.FechaAsignacion =
                            ahora;


                        if (
                            relacionTecnico !=
                            null
                        )
                        {
                            ticket.SupportTeamId =
                                relacionTecnico.SupportTeamId;
                        }
                        else
                        {
                            ticket.SupportTeamId =
                                null;
                        }
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


                    _context.ServiceTicketHistories.Add(
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
                                $"La prioridad cambió de \"{prioridadAnteriorNombre}\" a \"{prioridadNuevaNombre}\".",

                            FechaHora =
                                ahora,

                            DireccionIp =
                                ObtenerDireccionIp()
                        }
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
                    estadoAnteriorId !=
                    request.StatusId
                )
                {
                    string estadoAnteriorNombre =
                        estadoAnterior?.Nombre ??
                        estadoAnteriorId.ToString();


                    string estadoNuevoNombre =
                        nuevoEstado.Nombre;


                    _context.ServiceTicketHistories.Add(
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
                                $"El estado cambió de \"{estadoAnteriorNombre}\" a \"{estadoNuevoNombre}\".",

                            FechaHora =
                                ahora,

                            DireccionIp =
                                ObtenerDireccionIp()
                        }
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
                    // RESUELTO
                    // =============================================

                    if (
                        request.StatusId ==
                        ESTADO_RESUELTO
                    )
                    {
                        ticket.Resolucion =
                            resolucion;

                        ticket.FechaResolucion =
                            ahora;

                        ticket.FechaCierre =
                            null;

                        ticket.UsuarioCierreId =
                            null;


                        _context.ServiceTicketHistories.Add(
                            new ServiceTicketHistory
                            {
                                TicketId =
                                    ticket.Id,

                                UsuarioId =
                                    usuarioActual.Id,

                                Accion =
                                    "Resolución",

                                Campo =
                                    "Resolucion",

                                ValorAnterior =
                                    resolucionAnterior,

                                ValorNuevo =
                                    resolucion,

                                Detalle =
                                    $"Se registró la resolución del ticket:\n{resolucion}",

                                FechaHora =
                                    ahora,

                                DireccionIp =
                                    ObtenerDireccionIp()
                            }
                        );
                    }


                    // =============================================
                    // CERRADO
                    // =============================================

                    else if (
                        request.StatusId ==
                        ESTADO_CERRADO
                    )
                    {
                        ticket.FechaCierre =
                            ahora;

                        ticket.UsuarioCierreId =
                            usuarioActual.Id;


                        _context.ServiceTicketHistories.Add(
                            new ServiceTicketHistory
                            {
                                TicketId =
                                    ticket.Id,

                                UsuarioId =
                                    usuarioActual.Id,

                                Accion =
                                    "Cierre",

                                Campo =
                                    "FechaCierre",

                                ValorAnterior =
                                    fechaCierreAnterior
                                        ?.ToString(
                                            "dd/MM/yyyy HH:mm"
                                        ),

                                ValorNuevo =
                                    ahora.ToString(
                                        "dd/MM/yyyy HH:mm"
                                    ),

                                Detalle =
                                    "El ticket fue cerrado después de haber sido resuelto.",

                                FechaHora =
                                    ahora,

                                DireccionIp =
                                    ObtenerDireccionIp()
                            }
                        );
                    }


                    // =============================================
                    // REABIERTO
                    // =============================================

                    else if (
                        request.StatusId ==
                        ESTADO_REABIERTO
                    )
                    {
                        string detalleReapertura =
                            "El ticket fue reabierto para continuar con su atención.";


                        if (
                            !string.IsNullOrWhiteSpace(
                                resolucionAnterior
                            )
                        )
                        {
                            detalleReapertura +=
                                $"\nResolución anterior: {resolucionAnterior}";
                        }


                        _context.ServiceTicketHistories.Add(
                            new ServiceTicketHistory
                            {
                                TicketId =
                                    ticket.Id,

                                UsuarioId =
                                    usuarioActual.Id,

                                Accion =
                                    "Reapertura",

                                Campo =
                                    "StatusId",

                                ValorAnterior =
                                    estadoAnterior?.Nombre,

                                ValorNuevo =
                                    nuevoEstado.Nombre,

                                Detalle =
                                    detalleReapertura,

                                FechaHora =
                                    ahora,

                                DireccionIp =
                                    ObtenerDireccionIp()
                            }
                        );


                        ticket.FechaResolucion =
                            null;

                        ticket.FechaCierre =
                            null;

                        ticket.UsuarioCierreId =
                            null;

                        ticket.Resolucion =
                            null;


                        // El SLA vuelve a considerarse activo.
                        ticket.SlaResolucionVencido =
                            ticket.FechaLimiteResolucionSla
                                .HasValue &&
                            ahora >
                            ticket.FechaLimiteResolucionSla
                                .Value;
                    }


                    // =============================================
                    // CANCELADO
                    // =============================================

                    else if (
                        request.StatusId ==
                        ESTADO_CANCELADO
                    )
                    {
                        ticket.FechaCierre =
                            ahora;

                        ticket.UsuarioCierreId =
                            usuarioActual.Id;
                    }


                    // =============================================
                    // ESTADOS ACTIVOS
                    // =============================================

                    else if (
                        request.StatusId ==
                            ESTADO_NUEVO ||
                        request.StatusId ==
                            ESTADO_ASIGNADO ||
                        request.StatusId ==
                            ESTADO_EN_PROCESO ||
                        request.StatusId ==
                            ESTADO_PENDIENTE_USUARIO
                    )
                    {
                        ticket.FechaCierre =
                            null;

                        ticket.UsuarioCierreId =
                            null;
                    }


                    huboCambios =
                        true;
                }


                // =================================================
                // EDITAR RESOLUCIÓN SIN CAMBIAR ESTADO
                // =================================================

                if (
                    estadoAnteriorId ==
                        ESTADO_RESUELTO &&
                    request.StatusId ==
                        ESTADO_RESUELTO &&
                    !string.Equals(
                        resolucionAnterior,
                        resolucion,
                        StringComparison.Ordinal
                    )
                )
                {
                    ticket.Resolucion =
                        resolucion;


                    if (
                        ticket.FechaResolucion ==
                        null
                    )
                    {
                        ticket.FechaResolucion =
                            ahora;
                    }


                    _context.ServiceTicketHistories.Add(
                        new ServiceTicketHistory
                        {
                            TicketId =
                                ticket.Id,

                            UsuarioId =
                                usuarioActual.Id,

                            Accion =
                                "Actualización de resolución",

                            Campo =
                                "Resolucion",

                            ValorAnterior =
                                resolucionAnterior,

                            ValorNuevo =
                                resolucion,

                            Detalle =
                                $"La resolución del ticket fue actualizada:\n{resolucion}",

                            FechaHora =
                                ahora,

                            DireccionIp =
                                ObtenerDireccionIp()
                        }
                    );


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
                // ACTUALIZACIÓN
                // =================================================

                ticket.FechaActualizacion =
                    ahora;


                // =================================================
                // SLA RESPUESTA
                // =================================================

                if (
                    ticket.FechaPrimeraRespuesta ==
                        null &&
                    ticket.FechaLimiteRespuestaSla
                        .HasValue
                )
                {
                    ticket.SlaRespuestaVencido =
                        ahora >
                        ticket
                            .FechaLimiteRespuestaSla
                            .Value;
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
                    ticket.FechaLimiteResolucionSla
                        .HasValue
                )
                {
                    ticket.SlaResolucionVencido =
                        ahora >
                        ticket
                            .FechaLimiteResolucionSla
                            .Value;
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

                        ticketId =
                            ticket.Id,

                        statusId =
                            ticket.StatusId,

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
                    StatusCodes
                        .Status500InternalServerError;


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
        // AGREGAR COMENTARIO AL TICKET
        // POST ?handler=AgregarComentario
        // =========================================================

        public async Task<IActionResult>
            OnPostAgregarComentarioAsync(
                [FromBody]
        AgregarComentarioTicketRequest request)
        {
            // =====================================================
            // USUARIO ACTUAL
            // =====================================================

            AppUser? usuarioActual =
                await _userManager.GetUserAsync(User);

            if (usuarioActual == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La sesión del usuario no es válida."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }

            // =====================================================
            // VALIDACIONES
            // =====================================================

            if (
                request == null ||
                request.TicketId <= 0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El ticket especificado no es válido."
                    }
                );
            }

            string comentario =
                request.Comentario
                    ?.Trim()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(
                    comentario))
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Debes escribir un comentario."
                    }
                );
            }

            if (comentario.Length > 5000)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El comentario no puede superar los 5000 caracteres."
                    }
                );
            }

            bool esAdmin =
                EsAdministradorMesa();

            // =====================================================
            // OBTENER TICKET
            // =====================================================

            ServiceTicket? ticket =
                await _context.ServiceTickets
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                            request.TicketId
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
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }

            // =====================================================
            // SEGURIDAD DEL SOLICITANTE
            // =====================================================

            if (
                !esAdmin &&
                ticket.UsuarioSolicitanteId !=
                usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para comentar este ticket."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }

            // =====================================================
            // NOTA INTERNA
            // =====================================================

            bool esNotaInterna =
                request.EsNotaInterna;

            if (
                esNotaInterna &&
                !esAdmin
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para agregar notas internas."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }

            try
            {
                DateTime ahora =
                    DateTime.Now;

                // =================================================
                // CREAR COMENTARIO
                // =================================================

                ServiceTicketComment nuevoComentario =
                    new ServiceTicketComment
                    {
                        TicketId =
                            ticket.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        Comentario =
                            comentario,

                        EsNotaInterna =
                            esNotaInterna,

                        FechaCreacion =
                            ahora,

                        Eliminado =
                            false
                    };

                _context.ServiceTicketComments.Add(
                    nuevoComentario
                );

                // =================================================
                // PRIMERA RESPUESTA DEL ÁREA TI
                // =================================================

                if (
                    esAdmin &&
                    !esNotaInterna &&
                    ticket.FechaPrimeraRespuesta ==
                    null
                )
                {
                    ticket.FechaPrimeraRespuesta =
                        ahora;

                    if (
                        ticket.FechaLimiteRespuestaSla
                            .HasValue
                    )
                    {
                        ticket.SlaRespuestaVencido =
                            ahora >
                            ticket
                                .FechaLimiteRespuestaSla
                                .Value;
                    }
                }

                // =================================================
                // ACTUALIZAR TICKET
                // =================================================

                ticket.FechaActualizacion =
                    ahora;

                // =================================================
                // HISTORIAL
                // =================================================

                ServiceTicketHistory historial =
                    new ServiceTicketHistory
                    {
                        TicketId =
                            ticket.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        Accion =
                            esNotaInterna
                                ? "Nota interna"
                                : "Comentario",

                        Campo =
                            esNotaInterna
                                ? "NotaInterna"
                                : "Comentario",

                        ValorAnterior =
                            null,

                        ValorNuevo =
                            null,

                        Detalle =
                            esNotaInterna
                                ? "Se agregó una nota interna al ticket."
                                : "Se agregó un comentario de seguimiento al ticket.",

                        FechaHora =
                            ahora,

                        DireccionIp =
                            ObtenerDireccionIp()
                    };

                _context.ServiceTicketHistories.Add(
                    historial
                );

                await _context.SaveChangesAsync();

                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            esNotaInterna
                                ? "Nota interna agregada correctamente."
                                : "Comentario agregado correctamente."
                    }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al agregar comentario al ticket {TicketId}.",
                    request.TicketId
                );

                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Ocurrió un error al guardar el comentario."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
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
        // Solamente usuarios con rol "Administrador TI"
        // =========================================================

        // =========================================================
        // OBTENER TÉCNICOS
        // Solamente usuarios con rol "Administrador TI"
        // =========================================================

        private async Task<object[]> ObtenerTecnicosAsync()
        {
            const string rolTecnico =
                "Administrador TI";

            IList<AppUser> usuariosAdministradorTi =
                await _userManager.GetUsersInRoleAsync(
                    rolTecnico
                );

            if (
                usuariosAdministradorTi == null ||
                usuariosAdministradorTi.Count == 0
            )
            {
                return Array.Empty<object>();
            }

            var tecnicos =
                usuariosAdministradorTi
                    .OrderBy(
                        x =>
                            x.UserName ??
                            x.Email
                    )
                    .Select(
                        usuario =>
                            new
                            {
                                id =
                                    usuario.Id,

                                nombre =
                                    usuario.UserName ??
                                    usuario.Email ??
                                    "Usuario",

                                correo =
                                    usuario.Email ??
                                    string.Empty
                            }
                    )
                    .Cast<object>()
                    .ToArray();

            return tecnicos;
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
        // VALIDAR ADMINISTRADOR DE MESA DE AYUDA
        // Administrador y Administrador TI tienen acceso FULL
        // =========================================================

        private bool EsAdministradorMesa()
        {
            return
                User.IsInRole("Administrador") ||
                User.IsInRole("Administrador TI") ||
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

        // =========================================================
        // FORMATEAR TAMAÑO DE ARCHIVO
        // =========================================================

        private static string FormatearTamanoArchivo(
            long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes} B";
            }

            double kb =
                bytes / 1024d;

            if (kb < 1024)
            {
                return $"{kb:N1} KB";
            }

            double mb =
                kb / 1024d;

            if (mb < 1024)
            {
                return $"{mb:N1} MB";
            }

            double gb =
                mb / 1024d;

            return $"{gb:N1} GB";
        }
    }
}