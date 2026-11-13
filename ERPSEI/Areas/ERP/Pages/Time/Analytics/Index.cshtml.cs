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


        // =====================================================
        // PÁGINA
        // =====================================================

        public void OnGet()
        {
        }


        // =====================================================
        // CATÁLOGOS
        // =====================================================

        public async Task<IActionResult>
            OnGetCatalogosAsync(
                long? departamentoId)
        {
            try
            {
                if (departamentoId.HasValue &&
                    departamentoId.Value <= 0)
                {
                    return BadRequest(
                        new
                        {
                            success =
                                false,

                            message =
                                "El área seleccionada es inválida."
                        });
                }

                TimeAnalyticsCatalogoDto catalogos =
                    await _analyticsService
                        .ObtenerCatalogosAsync(
                            departamentoId,
                            HttpContext
                                .RequestAborted);

                return new JsonResult(
                    new
                    {
                        success =
                            true,

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
                        success =
                            false,

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
                        success =
                            true,

                        dashboard
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        success =
                            false,

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
                    "TIME Analytics: error obteniendo Dashboard Cumplimiento.");

                return StatusCode(
                    500,
                    new
                    {
                        success =
                            false,

                        message =
                            "No fue posible obtener la información de Analítica TIME."
                    });
            }
        }


        // =====================================================
        // EVALUACIÓN DE ÁREAS
        // =====================================================

        public async Task<IActionResult>
            OnGetEvaluacionAreasAsync(
                DateTime? fechaInicio,
                DateTime? fechaFin,
                long? usuarioId,
                long? departamentoId,
                string? tipo,
                string? puesto)
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
                            departamentoId,

                        Tipo =
                            tipo,

                        Puesto =
                            puesto
                    };

                TimeAnalyticsEvaluacionAreasDto evaluacion =
                    await _analyticsService
                        .ObtenerEvaluacionAreasAsync(
                            filtro,
                            HttpContext
                                .RequestAborted);

                return new JsonResult(
                    new
                    {
                        success =
                            true,

                        evaluacion
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        success =
                            false,

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
                    "TIME Analytics: error obteniendo Evaluación de Áreas.");

                return StatusCode(
                    500,
                    new
                    {
                        success =
                            false,

                        message =
                            "No fue posible obtener la Evaluación de Áreas."
                    });
            }
        }


        // =====================================================
        // ANÁLISIS DE PERSONAL
        // =====================================================

        public async Task<IActionResult>
            OnGetAnalisisPersonalAsync(
                DateTime? fechaInicio,
                DateTime? fechaFin,
                long? usuarioId,
                long? departamentoId,
                string? tipo,
                string? puesto)
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
                            departamentoId,

                        Tipo =
                            tipo,

                        Puesto =
                            puesto
                    };

                TimeAnalyticsAnalisisPersonalDto personal =
                    await _analyticsService
                        .ObtenerAnalisisPersonalAsync(
                            filtro,
                            HttpContext
                                .RequestAborted);

                return new JsonResult(
                    new
                    {
                        success =
                            true,

                        personal
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        success =
                            false,

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
                    "TIME Analytics: error obteniendo Análisis de Personal.");

                return StatusCode(
                    500,
                    new
                    {
                        success =
                            false,

                        message =
                            "No fue posible obtener el Análisis de Personal."
                    });
            }
        }

        // =====================================================
        // ALINEACIÓN BACK OFFICE
        // =====================================================

        public async Task<IActionResult>
            OnGetAlineacionBackOfficeAsync(
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

                TimeAnalyticsBackOfficeDto backOffice =
                    await _analyticsService
                        .ObtenerAlineacionBackOfficeAsync(
                            filtro,
                            HttpContext
                                .RequestAborted);

                return new JsonResult(
                    new
                    {
                        success =
                            true,

                        backOffice
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        success =
                            false,

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
                    "TIME Analytics: error obteniendo Alineación Back Office.");

                return StatusCode(
                    500,
                    new
                    {
                        success =
                            false,

                        message =
                            "No fue posible obtener la Alineación Back Office."
                    });
            }
        }


        // =====================================================
        // ALINEACIÓN OPERATIVAS
        // =====================================================

        public async Task<IActionResult>
            OnGetAlineacionOperativasAsync(
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

                TimeAnalyticsOperativasDto operativas =
                    await _analyticsService
                        .ObtenerAlineacionOperativasAsync(
                            filtro,
                            HttpContext
                                .RequestAborted);

                return new JsonResult(
                    new
                    {
                        success =
                            true,

                        operativas
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        success =
                            false,

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
                    "TIME Analytics: error obteniendo Alineación Operativas.");

                return StatusCode(
                    500,
                    new
                    {
                        success =
                            false,

                        message =
                            "No fue posible obtener la Alineación Operativas."
                    });
            }
        }


        // =====================================================
        // HORAS EXTRAS
        // =====================================================

        public async Task<IActionResult>
            OnGetHorasExtrasAsync(
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

                TimeAnalyticsHorasExtrasDto extras =
                    await _analyticsService
                        .ObtenerHorasExtrasAsync(
                            filtro,
                            HttpContext
                                .RequestAborted);

                return new JsonResult(
                    new
                    {
                        success =
                            true,

                        extras
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        success =
                            false,

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
                    "TIME Analytics: error obteniendo Horas Extras.");

                return StatusCode(
                    500,
                    new
                    {
                        success =
                            false,

                        message =
                            "No fue posible obtener Horas Extras."
                    });
            }
        }

    }
}