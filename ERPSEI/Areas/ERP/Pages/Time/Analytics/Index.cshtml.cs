using ERPSEI.Services.TimeIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPSEI.Areas.ERP.Pages.Time.Analytics
{
    [Authorize(
        Roles =
            "Master,Administrador,Administrador TH")]
    public class IndexModel : PageModel
    {
        private readonly ITimeAnalyticsService
            _analyticsService;

        private readonly ILogger<IndexModel>
            _logger;

        public IndexModel(
            ITimeAnalyticsService analyticsService,
            ILogger<IndexModel> logger)
        {
            _analyticsService =
                analyticsService;

            _logger =
                logger;
        }

        public void OnGet()
        {
        }

        // =====================================================
        // CATÁLOGOS
        // =====================================================

        public async Task<IActionResult>
            OnGetCatalogosAsync()
        {
            try
            {
                TimeAnalyticsCatalogoDto catalogos =
                    await _analyticsService
                        .ObtenerCatalogosAsync(
                            HttpContext
                                .RequestAborted);

                return new JsonResult(
                    new
                    {
                        success = true,
                        usuarios =
                            catalogos.Usuarios,
                        areas =
                            catalogos.Areas
                    });
            }
            catch (OperationCanceledException)
                when (
                    HttpContext
                        .RequestAborted
                        .IsCancellationRequested)
            {
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TIME Analytics: error obteniendo catálogos.");

                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message =
                            "No fue posible obtener los filtros de TIME."
                    });
            }
        }

        // =====================================================
        // DASHBOARD CUMPLIMIENTO
        // =====================================================

        public async Task<IActionResult>
            OnGetDashboardAsync(
                DateTime? fechaInicio,
                DateTime? fechaFin,
                long? usuarioId,
                long? departamentoId)
        {
            try
            {
                var filtro =
                    new TimeAnalyticsFiltroDto
                    {
                        FechaInicio =
                            fechaInicio,

                        FechaFin =
                            fechaFin,

                        UsuarioId =
                            usuarioId,

                        DepartamentoId =
                            departamentoId
                    };

                TimeAnalyticsDashboardDto dashboard =
                    await _analyticsService
                        .ObtenerDashboardAsync(
                            filtro,
                            HttpContext
                                .RequestAborted);

                return new JsonResult(
                    new
                    {
                        success = true,
                        dashboard
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        success = false,
                        message =
                            ex.Message
                    });
            }
            catch (OperationCanceledException)
                when (
                    HttpContext
                        .RequestAborted
                        .IsCancellationRequested)
            {
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TIME Analytics: error obteniendo dashboard.");

                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message =
                            "No fue posible obtener la información de Analítica TIME."
                    });
            }
        }
    }
}