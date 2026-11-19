
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.TalentoHumano;
using ERPSEI.Services.TalentoHumano;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ERPSEI.Areas.ERP.Pages.TalentoHumano
{
    [Authorize(Roles = "Administrador")]
    public class PruebasMotorModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ISolicitudesTHService _solicitudesService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<PruebasMotorModel> _logger;

        public PruebasMotorModel(
            ApplicationDbContext context,
            ISolicitudesTHService solicitudesService,
            IServiceScopeFactory scopeFactory,
            IWebHostEnvironment environment,
            ILogger<PruebasMotorModel> logger)
        {
            _context = context;
            _solicitudesService = solicitudesService;
            _scopeFactory = scopeFactory;
            _environment = environment;
            _logger = logger;
        }

        [BindProperty]
        public TipoSolicitudTH TipoSeleccionado { get; set; }
            = TipoSolicitudTH.ConstanciaLaboral;

        public List<THSolicitud> UltimasSolicitudes { get; set; }
            = new();

        public string? Resultado { get; set; }

        public string? Error { get; set; }

        public async Task<IActionResult> OnGetAsync(
            CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            await CargarSolicitudesAsync(cancellationToken);

            return Page();
        }

        // =========================================================
        // PRUEBA 1: CREACIÓN INDIVIDUAL
        // =========================================================

        public async Task<IActionResult> OnPostCrearAsync(
            CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            try
            {
                var solicitud =
                    await _solicitudesService.CrearSolicitudAsync(
                        TipoSeleccionado,
                        User,
                        cancellationToken);

                Resultado =
                    $"Solicitud creada: {solicitud.Folio}";
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is UnauthorizedAccessException ||
                ex is InvalidOperationException ||
                ex is DbUpdateException)
            {
                _logger.LogWarning(
                    "Prueba individual TH rechazada: {TipoError}",
                    ex.GetType().Name);

                Error = ex.Message;
            }

            await CargarSolicitudesAsync(cancellationToken);
            return Page();
        }

        // =========================================================
        // PRUEBA 2: CINCO SOLICITUDES CONCURRENTES
        // =========================================================

        public async Task<IActionResult> OnPostConcurrenciaAsync(
            CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            try
            {
                const int cantidad = 5;

                var tareas = Enumerable.Range(0, cantidad)
                    .Select(_ => CrearEnScopeAsync(
                        TipoSeleccionado,
                        User,
                        cancellationToken))
                    .ToArray();

                var folios = await Task.WhenAll(tareas);

                bool unicos =
                    folios.Distinct(
                        StringComparer.OrdinalIgnoreCase)
                        .Count() == cantidad;

                Resultado = unicos
                    ? "PRUEBA CORRECTA: " +
                      string.Join(", ", folios)
                    : "PRUEBA FALLIDA: existen folios duplicados.";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error en concurrencia TH.");

                Error =
                    "La concurrencia presentó un error. " +
                    "Verifica las solicitudes en SQL antes de repetir.";
            }

            await CargarSolicitudesAsync(cancellationToken);
            return Page();
        }

        // =========================================================
        // PRUEBA 3: TIPO INVÁLIDO
        // =========================================================

        public async Task<IActionResult> OnPostTipoInvalidoAsync(
            CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            try
            {
                long antes = await _context.THSolicitudes
                    .LongCountAsync(cancellationToken);

                bool rechazado = false;

                try
                {
                    await _solicitudesService.CrearSolicitudAsync(
                        (TipoSolicitudTH)999,
                        User,
                        cancellationToken);
                }
                catch (ArgumentException)
                {
                    rechazado = true;
                }

                long despues = await _context.THSolicitudes
                    .LongCountAsync(cancellationToken);

                Resultado = rechazado && antes == despues
                    ? "PRUEBA CORRECTA: tipo inválido rechazado."
                    : "PRUEBA FALLIDA: verificar los datos.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error probando tipo inválido TH.");
                Error = "No se pudo completar la prueba.";
            }

            await CargarSolicitudesAsync(cancellationToken);
            return Page();
        }

        // =========================================================
        // PRUEBA 4: ROLLBACK DEL SERVICIO REAL
        // =========================================================

        public async Task<IActionResult> OnPostRollbackAsync(
            CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            if (!Enum.IsDefined(
                typeof(TipoSolicitudTH),
                TipoSeleccionado))
            {
                Error = "Selecciona un tipo válido.";
                await CargarSolicitudesAsync(cancellationToken);
                return Page();
            }

            try
            {
                // Tomamos los valores ANTES de la prueba.

                int anio = ObtenerAnioMexico();

                long solicitudesAntes =
                    await _context.THSolicitudes
                        .LongCountAsync(cancellationToken);

                long historialAntes =
                    await _context.THHistorialSolicitudes
                        .LongCountAsync(cancellationToken);

                int? consecutivoAntes =
                    await _context.THFolioConsecutivos
                        .AsNoTracking()
                        .Where(x =>
                            x.Tipo == TipoSeleccionado &&
                            x.Anio == anio)
                        .Select(x => (int?)x.UltimoConsecutivo)
                        .SingleOrDefaultAsync(cancellationToken);

                bool falloControladoDetectado = false;

                // El servicio se ejecuta en OTRO scope.
                // Así evitamos reutilizar su DbContext después
                // de la transacción revertida.

                using (var scope = _scopeFactory.CreateScope())
                {
                    var servicio = scope.ServiceProvider
                        .GetRequiredService<ISolicitudesTHService>();

                    if (servicio is not SolicitudesTHService motor)
                    {
                        throw new InvalidOperationException(
                            "No se pudo obtener el motor de pruebas.");
                    }

                    try
                    {
                        await motor
                            .CrearSolicitudParaPruebaRollbackAsync(
                                TipoSeleccionado,
                                User,
                                cancellationToken);
                    }
                    catch (PruebaRollbackTHException)
                    {
                        falloControladoDetectado = true;
                    }
                }

                // Revisamos la base DESPUÉS de finalizar el scope
                // y de disponer la transacción fallida.

                long solicitudesDespues =
                    await _context.THSolicitudes
                        .LongCountAsync(cancellationToken);

                long historialDespues =
                    await _context.THHistorialSolicitudes
                        .LongCountAsync(cancellationToken);

                int? consecutivoDespues =
                    await _context.THFolioConsecutivos
                        .AsNoTracking()
                        .Where(x =>
                            x.Tipo == TipoSeleccionado &&
                            x.Anio == anio)
                        .Select(x => (int?)x.UltimoConsecutivo)
                        .SingleOrDefaultAsync(cancellationToken);

                bool sinCambios =
                    solicitudesAntes == solicitudesDespues &&
                    historialAntes == historialDespues &&
                    consecutivoAntes == consecutivoDespues;

                Resultado =
                    falloControladoDetectado && sinCambios
                        ? "PRUEBA CORRECTA: el servicio ejecutó " +
                          "SaveChangesAsync, provocó el error " +
                          "controlado y SQL Server revirtió la " +
                          "solicitud, el historial y el consecutivo."
                        : "PRUEBA FALLIDA: revisar rollback. " +
                          $"Antes: S={solicitudesAntes}, " +
                          $"H={historialAntes}, " +
                          $"C={consecutivoAntes?.ToString() ?? "NULL"}. " +
                          $"Después: S={solicitudesDespues}, " +
                          $"H={historialDespues}, " +
                          $"C={consecutivoDespues?.ToString() ?? "NULL"}.";
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Fallo inesperado en prueba rollback TH.");

                Error =
                    "No se pudo completar la prueba de rollback. " +
                    "Revisa los registros de la aplicación.";
            }

            await CargarSolicitudesAsync(cancellationToken);
            return Page();
        }

        // =========================================================
        // PRUEBA 5: REGLAS DE EMPLEADO Y CUENTA INACTIVOS
        // Solo consulta datos, no crea solicitudes ni inicia sesión.
        // =========================================================

        public async Task<IActionResult> OnPostEmpleadoInactivoAsync(
            CancellationToken cancellationToken)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            try
            {
                var empleado = await _context.Empleados
                    .AsNoTracking()
                    .Where(x => x.Deshabilitado != 0)
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (empleado == null)
                {
                    Error =
                        "No existen empleados inactivos para esta prueba.";
                }
                else
                {
                    bool rechazoEmpleado = false;
                    bool rechazoCuenta = false;

                    // Se prueba exactamente el validador que utiliza
                    // CrearSolicitudAsync antes de la transacción.

                    try
                    {
                        SolicitudesTHService.ValidarEmpleadoActivo(
                            empleado);
                    }
                    catch (InvalidOperationException)
                    {
                        rechazoEmpleado = true;
                    }

                    // Se busca una cuenta bloqueada de forma
                    // independiente, sin alterar sus permisos.

                    var cuentaBloqueada = await _context.Users
                        .AsNoTracking()
                        .Where(x => x.IsBanned)
                        .Select(x => new { x.IsBanned })
                        .FirstOrDefaultAsync(cancellationToken);

                    if (cuentaBloqueada != null)
                    {
                        try
                        {
                            SolicitudesTHService.ValidarCuentaHabilitada(
                                cuentaBloqueada.IsBanned);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            rechazoCuenta = true;
                        }
                    }

                    Resultado =
                        rechazoEmpleado && rechazoCuenta
                            ? "PRUEBA CORRECTA: las reglas del motor " +
                              "rechazan empleados inactivos y cuentas " +
                              "bloqueadas sin modificar datos."
                            : "PRUEBA INCOMPLETA: revisar las reglas " +
                              "o la disponibilidad de cuentas bloqueadas.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error validando empleados inactivos TH.");

                Error =
                    "No se pudo completar la prueba de empleado inactivo.";
            }

            await CargarSolicitudesAsync(cancellationToken);
            return Page();
        }

        // =========================================================
        // MÉTODOS AUXILIARES
        // =========================================================

        private async Task<string> CrearEnScopeAsync(
            TipoSolicitudTH tipo,
            ClaimsPrincipal usuario,
            CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var servicio = scope.ServiceProvider
                .GetRequiredService<ISolicitudesTHService>();

            var solicitud = await servicio.CrearSolicitudAsync(
                tipo,
                usuario,
                cancellationToken);

            return solicitud.Folio;
        }

        private async Task CargarSolicitudesAsync(
            CancellationToken cancellationToken)
        {
            string? usuarioId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(usuarioId))
                return;

            UltimasSolicitudes = await _context.THSolicitudes
                .AsNoTracking()
                .Where(x => x.SolicitanteUserId == usuarioId)
                .OrderByDescending(x => x.Id)
                .Take(15)
                .ToListAsync(cancellationToken);
        }

        private static int ObtenerAnioMexico()
        {
            TimeZoneInfo zona;

            try
            {
                zona = TimeZoneInfo.FindSystemTimeZoneById(
                    "America/Mexico_City");
            }
            catch (TimeZoneNotFoundException)
            {
                zona = TimeZoneInfo.FindSystemTimeZoneById(
                    "Central Standard Time (Mexico)");
            }

            return TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                zona).Year;
        }
    }
}
