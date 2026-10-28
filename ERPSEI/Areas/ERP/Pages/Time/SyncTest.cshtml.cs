
using ERPSEI.Services.TimeIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPSEI.Areas.ERP.Pages.Time
{
    [Authorize(Roles = "Administrador,Master")]
    public class SyncTestModel : PageModel
    {
        private readonly ITimeSyncService _syncService;
        private readonly IWebHostEnvironment _environment;
        private readonly ITimeDeletionAuditService _deletionAuditService;
        private readonly ILogger<SyncTestModel> _logger;

        public TimeSyncResultadoDto? Resultado { get; private set; }
        public TimeBajasDiagnostico? DiagnosticoBajas { get; private set; }
        public string? Error { get; private set; }

        public SyncTestModel(
            ITimeSyncService syncService,
            IWebHostEnvironment environment,
            ITimeDeletionAuditService deletionAuditService,
            ILogger<SyncTestModel> logger)
        {
            _syncService = syncService;
            _environment = environment;
            _deletionAuditService = deletionAuditService;
            _logger = logger;
        }


        public async Task<IActionResult> OnPostDiagnosticarBajasHorasAsync()
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            try
            {
                DiagnosticoBajas = await _deletionAuditService
                    .RevisarHorasAsync(HttpContext.RequestAborted);
            }
            catch (OperationCanceledException)
                when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TIME: Falló el diagnóstico de bajas de Horas.");

                Error = "No se completó la revisión de bajas. " +
                        "Consulta la salida de Visual Studio.";
            }

            return Page();
        }

        public IActionResult OnGet()
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            return Page();
        }

        private async Task<IActionResult> EjecutarAsync(
            Func<CancellationToken, Task<TimeSyncResultadoDto>> operacion)
        {
            if (!_environment.IsDevelopment())
                return NotFound();

            try
            {
                Resultado = await operacion(
                    HttpContext.RequestAborted);
            }
            catch (OperationCanceledException)
                when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TIME: Error durante sincronización manual.");

                Error = "No fue posible completar la sincronización. " +
                        "Consulta la salida de Visual Studio. " +
                        "Los lotes confirmados permanecen guardados.";
            }

            return Page();
        }

        public Task<IActionResult> OnPostAsync()
            => EjecutarAsync(
                ct => _syncService.SincronizarDepartamentosAsync(ct));

        public Task<IActionResult> OnPostUsuariosAsync()
            => EjecutarAsync(
                ct => _syncService.SincronizarUsuariosAsync(ct));

        public Task<IActionResult> OnPostClavesAsync()
            => EjecutarAsync(
                ct => _syncService.SincronizarClavesAsync(ct));

        public Task<IActionResult> OnPostActividadesAsync()
            => EjecutarAsync(
                ct => _syncService.SincronizarActividadesAsync(ct));

        public Task<IActionResult> OnPostHorasAsync()
            => EjecutarAsync(
                ct => _syncService.SincronizarHorasAsync(ct));

        public Task<IActionResult> OnPostEmpresasAsync()
            => EjecutarAsync(
                ct => _syncService.SincronizarEmpresasAsync(ct));

        public Task<IActionResult> OnPostGruposAsync()
            => EjecutarAsync(
                ct => _syncService.SincronizarGruposAsync(ct));

        public Task<IActionResult> OnPostConciliarHorasAsync()
    => EjecutarAsync(
        ct => _syncService.ConciliarHorasAsync(ct));

        public Task<IActionResult> OnPostConciliarActividadesAsync()
            => EjecutarAsync(
                ct => _syncService.ConciliarActividadesAsync(ct));

    }

}
