using ERPSEI.Data;
using ERPSEI.Data.Entities.ServiceDesk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERPSEI.Areas.ERP.Pages
{
    [Authorize]
    public class MesaDeAyudaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MesaDeAyudaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // CONTROL DE INTERFAZ
        // =====================================================

        public bool EsAdmin { get; set; }

        public string UsuarioActualId { get; set; } = string.Empty;


        // =====================================================
        // DASHBOARD
        // =====================================================

        public int TotalAbiertos { get; set; }

        public int TotalNuevos { get; set; }

        public int TotalEnProceso { get; set; }

        public int TotalResueltos { get; set; }

        public int TotalSinAsignar { get; set; }


        // =====================================================
        // TICKETS
        // =====================================================

        public List<ServiceTicket> Tickets { get; set; } = new();


        // =====================================================
        // CATÁLOGOS
        // =====================================================

        public List<SelectListItem> TiposTicket { get; set; } = new();

        public List<SelectListItem> Categorias { get; set; } = new();

        public List<SelectListItem> Subcategorias { get; set; } = new();


        // =====================================================
        // NUEVO TICKET
        // =====================================================

        [BindProperty]
        public int TicketTypeId { get; set; }

        [BindProperty]
        public int CategoryId { get; set; }

        [BindProperty]
        public int? SubcategoryId { get; set; }

        [BindProperty]
        public string Titulo { get; set; } = string.Empty;

        [BindProperty]
        public string Descripcion { get; set; } = string.Empty;


        // =====================================================
        // ON GET
        // =====================================================

        public async Task<IActionResult> OnGetAsync()
        {
            UsuarioActualId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(UsuarioActualId))
                return Challenge();

            DeterminarRol();

            await CargarCatalogosAsync();

            await CargarDashboardAsync();

            await CargarTicketsAsync();

            return Page();
        }


        // =====================================================
        // CREAR TICKET
        // =====================================================

        public async Task<IActionResult> OnPostCrearTicketAsync()
        {
            UsuarioActualId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(UsuarioActualId))
                return Challenge();

            DeterminarRol();

            // -------------------------------------------------
            // VALIDACIONES
            // -------------------------------------------------

            if (TicketTypeId <= 0)
            {
                ModelState.AddModelError(
                    nameof(TicketTypeId),
                    "Selecciona el tipo de solicitud."
                );
            }

            if (CategoryId <= 0)
            {
                ModelState.AddModelError(
                    nameof(CategoryId),
                    "Selecciona una categoría."
                );
            }

            if (string.IsNullOrWhiteSpace(Titulo))
            {
                ModelState.AddModelError(
                    nameof(Titulo),
                    "Ingresa el asunto del ticket."
                );
            }

            if (string.IsNullOrWhiteSpace(Descripcion))
            {
                ModelState.AddModelError(
                    nameof(Descripcion),
                    "Describe el problema o solicitud."
                );
            }

            bool tipoExiste =
                await _context.ServiceTicketTypes
                    .AnyAsync(x =>
                        x.Id == TicketTypeId &&
                        x.Activo
                    );

            if (!tipoExiste)
            {
                ModelState.AddModelError(
                    nameof(TicketTypeId),
                    "El tipo seleccionado no es válido."
                );
            }

            bool categoriaExiste =
                await _context.ServiceCategories
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

            if (SubcategoryId.HasValue)
            {
                bool subcategoriaExiste =
                    await _context.ServiceSubcategories
                        .AnyAsync(x =>
                            x.Id == SubcategoryId.Value &&
                            x.CategoryId == CategoryId &&
                            x.Activo
                        );

                if (!subcategoriaExiste)
                {
                    ModelState.AddModelError(
                        nameof(SubcategoryId),
                        "La subcategoría no corresponde a la categoría seleccionada."
                    );
                }
            }

            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync();
                await CargarDashboardAsync();
                await CargarTicketsAsync();

                return Page();
            }


            // =================================================
            // CONFIGURACIÓN INICIAL
            // =================================================

            const int ESTADO_NUEVO = 1;
            const int PRIORIDAD_MEDIA = 3;
            const int EQUIPO_MESA_SERVICIO = 1;

            var prioridad =
                await _context.ServiceTicketPriorities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x =>
                        x.Id == PRIORIDAD_MEDIA &&
                        x.Activo
                    );

            if (prioridad == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se encontró la prioridad predeterminada."
                );

                await CargarCatalogosAsync();
                await CargarDashboardAsync();
                await CargarTicketsAsync();

                return Page();
            }

            var tipo =
                await _context.ServiceTicketTypes
                    .AsNoTracking()
                    .FirstAsync(x =>
                        x.Id == TicketTypeId
                    );


            DateTime ahora = DateTime.Now;


            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // =================================================
                // CREAR TICKET
                // =================================================

                var ticket = new ServiceTicket
                {
                    // Folio temporal para cumplir índice UNIQUE
                    Folio = $"TMP-{Guid.NewGuid():N}",

                    TicketTypeId = TicketTypeId,

                    Titulo = Titulo.Trim(),

                    Descripcion = Descripcion.Trim(),

                    UsuarioSolicitanteId = UsuarioActualId,

                    UsuarioAsignadoId = null,

                    SupportTeamId = EQUIPO_MESA_SERVICIO,

                    CategoryId = CategoryId,

                    SubcategoryId = SubcategoryId,

                    PriorityId = PRIORIDAD_MEDIA,

                    StatusId = ESTADO_NUEVO,

                    Origen = "Intranet",

                    FechaCreacion = ahora,

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

                    Eliminado = false
                };


                _context.ServiceTickets.Add(ticket);

                await _context.SaveChangesAsync();


                // =================================================
                // FOLIO DEFINITIVO
                // =================================================

                ticket.Folio =
                    $"{tipo.Codigo}-{ahora.Year}-{ticket.Id:D6}";

                ticket.FechaActualizacion = DateTime.Now;


                // =================================================
                // HISTORIAL FUNCIONAL
                // =================================================

                _context.ServiceTicketHistories.Add(
                    new ServiceTicketHistory
                    {
                        TicketId = ticket.Id,

                        UsuarioId = UsuarioActualId,

                        Accion = "CREACION",

                        Campo = "Ticket",

                        ValorAnterior = null,

                        ValorNuevo = ticket.Folio,

                        Detalle =
                            "Ticket creado desde la Mesa de Ayuda de la Intranet.",

                        FechaHora = DateTime.Now,

                        DireccionIp =
                            HttpContext.Connection
                                .RemoteIpAddress?
                                .ToString()
                    }
                );


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();


                TempData["MensajeExito"] =
                    $"El ticket {ticket.Folio} fue creado correctamente.";

                return RedirectToPage();
            }
            catch
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Ocurrió un error al crear el ticket."
                );

                await CargarCatalogosAsync();
                await CargarDashboardAsync();
                await CargarTicketsAsync();

                return Page();
            }
        }


        // =====================================================
        // AJAX - SUBCATEGORÍAS
        // =====================================================

        public async Task<JsonResult> OnGetSubcategoriasAsync(
            int categoryId)
        {
            var datos =
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

            return new JsonResult(datos);
        }


        // =====================================================
        // DETERMINAR ROL
        // =====================================================

        private void DeterminarRol()
        {
            EsAdmin =
                User.IsInRole("Admin") ||
                User.IsInRole("Administrador");
        }


        // =====================================================
        // CARGAR TICKETS
        // =====================================================

        private async Task CargarTicketsAsync()
        {
            var query =
                _context.ServiceTickets
                    .AsNoTracking()
                    .Include(x => x.TicketType)
                    .Include(x => x.Category)
                    .Include(x => x.Subcategory)
                    .Include(x => x.Priority)
                    .Include(x => x.Status)
                    .AsQueryable();


            // ADMIN → TODOS
            if (EsAdmin)
            {
                Tickets =
                    await query
                        .OrderByDescending(x =>
                            x.FechaCreacion
                        )
                        .Take(100)
                        .ToListAsync();
            }
            else
            {
                // USUARIO → SÓLO LOS SUYOS
                Tickets =
                    await query
                        .Where(x =>
                            x.UsuarioSolicitanteId ==
                            UsuarioActualId
                        )
                        .OrderByDescending(x =>
                            x.FechaCreacion
                        )
                        .Take(100)
                        .ToListAsync();
            }
        }


        // =====================================================
        // DASHBOARD
        // =====================================================

        private async Task CargarDashboardAsync()
        {
            var query =
                _context.ServiceTickets
                    .AsNoTracking()
                    .AsQueryable();


            if (!EsAdmin)
            {
                query =
                    query.Where(x =>
                        x.UsuarioSolicitanteId ==
                        UsuarioActualId
                    );
            }


            TotalAbiertos =
                await query.CountAsync(x =>
                    x.StatusId != 6 &&
                    x.StatusId != 8
                );


            TotalNuevos =
                await query.CountAsync(x =>
                    x.StatusId == 1
                );


            TotalEnProceso =
                await query.CountAsync(x =>
                    x.StatusId == 3
                );


            TotalResueltos =
                await query.CountAsync(x =>
                    x.StatusId == 5 ||
                    x.StatusId == 6
                );


            if (EsAdmin)
            {
                TotalSinAsignar =
                    await query.CountAsync(x =>
                        x.UsuarioAsignadoId == null &&
                        x.StatusId != 6 &&
                        x.StatusId != 8
                    );
            }
        }


        // =====================================================
        // CATÁLOGOS
        // =====================================================

        private async Task CargarCatalogosAsync()
        {
            TiposTicket =
                await _context.ServiceTicketTypes
                    .AsNoTracking()
                    .Where(x =>
                        x.Activo &&
                        (
                            x.Codigo == "INC" ||
                            x.Codigo == "SR"
                        )
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


            Categorias =
                await _context.ServiceCategories
                    .AsNoTracking()
                    .Where(x =>
                        x.Activo
                    )
                    .OrderBy(x =>
                        x.Orden
                    )
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


            if (CategoryId > 0)
            {
                Subcategorias =
                    await _context.ServiceSubcategories
                        .AsNoTracking()
                        .Where(x =>
                            x.CategoryId ==
                            CategoryId &&
                            x.Activo
                        )
                        .OrderBy(x =>
                            x.Orden
                        )
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
        }
    }
}