
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERPSEI.Data;
using ERPSEI.Data.Entities.TalentoHumano;
using ERPSEI.Services.TalentoHumano;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERPSEI.Areas.ERP.Pages.TalentoHumano
{
    [Authorize]
    public class MisSolicitudesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ISolicitudesTHService _solicitudesService;
        private readonly ILogger<MisSolicitudesModel> _logger;

        public MisSolicitudesModel(
            ApplicationDbContext context,
            ISolicitudesTHService solicitudesService,
            ILogger<MisSolicitudesModel> logger)
        {
            _context = context;
            _solicitudesService = solicitudesService;
            _logger = logger;
        }

        // =====================================================
        // CATÁLOGO VISIBLE DE TRÁMITES
        // =====================================================

        public sealed record OpcionTramite(
            TipoSolicitudTH Tipo,
            string Nombre,
            string Descripcion,
            string Icono);

        public static readonly IReadOnlyList<OpcionTramite>
            Tramites = new List<OpcionTramite>
            {
                new(
                    TipoSolicitudTH.ConstanciaLaboral,
                    "Constancia laboral",
                    "Solicita una constancia de tu relación laboral.",
                    "bi-file-earmark-text"),

                new(
                    TipoSolicitudTH.DesgloseNomina,
                    "Desglose de nómina",
                    "Solicita información sobre el desglose de tu nómina.",
                    "bi-receipt"),

                new(
                    TipoSolicitudTH.ComprobanteNominaCFDI,
                    "Comprobante de nómina CFDI",
                    "Solicita el comprobante fiscal de tu nómina.",
                    "bi-file-earmark-check"),

                new(
                    TipoSolicitudTH.VacacionesSaldo,
                    "Saldo de vacaciones",
                    "Consulta o solicita la aclaración de tu saldo.",
                    "bi-calendar-check"),

                new(
                    TipoSolicitudTH.Prestamo,
                    "Préstamo",
                    "Envía una solicitud de préstamo para revisión.",
                    "bi-cash-coin"),

                new(
                    TipoSolicitudTH.AnticipoFondoAhorro,
                    "Anticipo de fondo de ahorro",
                    "Solicita la revisión de un anticipo.",
                    "bi-wallet2"),

                new(
                    TipoSolicitudTH.CreditoInfonavit,
                    "Crédito Infonavit",
                    "Solicita apoyo con información de tu crédito.",
                    "bi-house-door")
            };

        // =====================================================
        // FORMULARIO DE CREACIÓN
        // =====================================================

        [BindProperty]
        public TipoSolicitudTH TipoSeleccionado { get; set; }

        // =====================================================
        // FILTROS DE CONSULTA
        // =====================================================

        [BindProperty(SupportsGet = true)]
        public string? BuscarFolio { get; set; }

        [BindProperty(SupportsGet = true)]
        public TipoSolicitudTH? TipoFiltro { get; set; }

        [BindProperty(SupportsGet = true)]
        public EstatusSolicitudTH? EstatusFiltro { get; set; }

        // =====================================================
        // DATOS PARA LA VISTA
        // =====================================================

        public List<THSolicitud> Solicitudes { get; private set; }
            = new();

        public int TotalMostradas => Solicitudes.Count;

        // =====================================================
        // GET: MIS SOLICITUDES
        // =====================================================

        public async Task<IActionResult> OnGetAsync(
            CancellationToken cancellationToken)
        {
            string? usuarioId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                return Challenge();
            }

            await CargarSolicitudesAsync(
                usuarioId,
                cancellationToken);

            return Page();
        }

        // =====================================================
        // POST: CREAR SOLICITUD
        // =====================================================

        public async Task<IActionResult> OnPostCrearAsync(
            CancellationToken cancellationToken)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            if (!Enum.IsDefined(
                typeof(TipoSolicitudTH),
                TipoSeleccionado))
            {
                TempData["TH_Error"] =
                    "Selecciona un trámite válido.";

                return RedirectToPage();
            }

            try
            {
                var solicitud =
                    await _solicitudesService.CrearSolicitudAsync(
                        TipoSeleccionado,
                        User,
                        cancellationToken);

                TempData["TH_Exito"] =
                    $"Tu solicitud {solicitud.Folio} " +
                    "se registró correctamente.";

                // Evita repetir la creación al actualizar
                // el navegador después de enviar el formulario.
                return RedirectToPage();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(
                    "Intento rechazado de crear solicitud TH: {Motivo}",
                    ex.Message);

                TempData["TH_Error"] =
                    "Tu cuenta no está habilitada para " +
                    "registrar solicitudes.";

                return RedirectToPage();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(
                    "Tipo de solicitud TH inválido: {Motivo}",
                    ex.Message);

                TempData["TH_Error"] =
                    "El tipo de solicitud no es válido.";

                return RedirectToPage();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                    "Solicitud TH no registrada: {Motivo}",
                    ex.Message);

                TempData["TH_Error"] =
                    "No fue posible registrar la solicitud. " +
                    "Verifica que tu cuenta esté vinculada " +
                    "a un empleado activo.";

                return RedirectToPage();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Error de base de datos al crear solicitud TH.");

                TempData["TH_Error"] =
                    "Ocurrió un error al guardar la solicitud. " +
                    "Intenta nuevamente más tarde.";

                return RedirectToPage();
            }
        }

        // =====================================================
        // CONSULTA PROTEGIDA POR IDENTIDAD
        // =====================================================

        private async Task CargarSolicitudesAsync(
            string usuarioId,
            CancellationToken cancellationToken)
        {
            // Esta condición SIEMPRE se aplica.
            // Los filtros del navegador no pueden sustituir
            // el SolicitanteUserId de la sesión.

            IQueryable<THSolicitud> consulta =
                _context.THSolicitudes
                    .AsNoTracking()
                    .Where(x =>
                        x.SolicitanteUserId == usuarioId);

            if (!string.IsNullOrWhiteSpace(BuscarFolio))
            {
                string folio = BuscarFolio.Trim();

                // El folio tiene un máximo de 40 caracteres.
                if (folio.Length > 40)
                {
                    Solicitudes = new List<THSolicitud>();
                    return;
                }

                consulta = consulta.Where(x =>
                    x.Folio.Contains(folio));
            }

            if (TipoFiltro.HasValue &&
                Enum.IsDefined(
                    typeof(TipoSolicitudTH),
                    TipoFiltro.Value))
            {
                consulta = consulta.Where(x =>
                    x.Tipo == TipoFiltro.Value);
            }

            if (EstatusFiltro.HasValue &&
                Enum.IsDefined(
                    typeof(EstatusSolicitudTH),
                    EstatusFiltro.Value))
            {
                consulta = consulta.Where(x =>
                    x.Estatus == EstatusFiltro.Value);
            }

            // Límite inicial para no cargar un volumen
            // ilimitado de información.
            // Incorporaremos paginación en la siguiente etapa.

            Solicitudes = await consulta
                .OrderByDescending(x => x.FechaEnvioUtc)
                .ThenByDescending(x => x.Id)
                .Take(100)
                .ToListAsync(cancellationToken);
        }
    }
}
