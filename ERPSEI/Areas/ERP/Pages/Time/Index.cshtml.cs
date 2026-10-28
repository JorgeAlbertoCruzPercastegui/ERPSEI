
using ERPSEI.Services.TimeIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace ERPSEI.Areas.ERP.Pages.Time
{
    [Authorize(Roles = "Administrador,Master")]
    public class IndexModel : PageModel
    {
        private readonly ITimeApiClient _timeApiClient;
        private readonly ITimeReportCatalogService _catalogService;
        private readonly ITimeReportPreviewService _previewService;
        private readonly ITimeSqlReportService _sqlReportService;
        private readonly ITimeReportExportService _exportService;
        private readonly ILogger<IndexModel> _logger;

        private static readonly HashSet<string> Tablas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "actividades",
                "claves",
                "configuraciones",
                "departamento-actividades",
                "departamentos",
                "empresas",
                "eventos",
                "grupos",
                "horas",
                "ingresos",
                "periodos",
                "roles",
                "sueldos",
                "usuarios"
            };

        public IndexModel(
            ITimeApiClient timeApiClient,
            ITimeReportCatalogService catalogService,
            ITimeReportPreviewService previewService,
            ITimeSqlReportService sqlReportService,
            ITimeReportExportService exportService,
            ILogger<IndexModel> logger)
        {
            _timeApiClient = timeApiClient;
            _catalogService = catalogService;
            _previewService = previewService;
            _sqlReportService = sqlReportService;
            _exportService = exportService;
            _logger = logger;
        }


        // =====================================================
        // TIME - FILTROS COMPARTIDOS DE EXPORTACIÓN
        // =====================================================

        private static TimeReporteHorasFiltro CrearFiltroExportacion(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            long? usuarioId,
            long? proyectoId,
            long? departamentoId)
        {
            if (fechaInicio.HasValue && fechaFin.HasValue &&
                fechaInicio.Value.Date > fechaFin.Value.Date)
            {
                throw new ArgumentException(
                    "La fecha inicial no puede superar la fecha final.");
            }

            if ((usuarioId.HasValue && usuarioId <= 0) ||
                (proyectoId.HasValue && proyectoId <= 0) ||
                (departamentoId.HasValue && departamentoId <= 0))
            {
                throw new ArgumentException(
                    "Los identificadores de los filtros son inválidos.");
            }

            return new TimeReporteHorasFiltro
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                UsuarioId = usuarioId,
                ProyectoId = proyectoId,
                DepartamentoId = departamentoId,
                Pagina = 1,
                TamanoPagina = 100
            };
        }

        // =====================================================
        // TIME - EXPORTAR EXCEL
        // =====================================================

        public async Task<IActionResult> OnGetExportarExcelTimeAsync(
            string tipo,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            long? usuarioId,
            long? proyectoId,
            long? departamentoId)
        {
            if (tipo != "horas")
                return BadRequest("Tipo de reporte no disponible.");

            try
            {
                var filtro = CrearFiltroExportacion(
                    fechaInicio,
                    fechaFin,
                    usuarioId,
                    proyectoId,
                    departamentoId);

                byte[] archivo = await _exportService
                    .GenerarExcelAsync(
                        filtro,
                        HttpContext.RequestAborted);

                return File(
                    archivo,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"TIME_Horas_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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
                    "TIME: Error al generar Excel.");

                return StatusCode(
                    500,
                    "No se pudo generar el archivo Excel.");
            }
        }

        // =====================================================
        // TIME - EXPORTAR PDF
        // =====================================================

        public async Task<IActionResult> OnGetExportarPdfTimeAsync(
            string tipo,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            long? usuarioId,
            long? proyectoId,
            long? departamentoId)
        {
            if (tipo != "horas")
                return BadRequest("Tipo de reporte no disponible.");

            try
            {
                var filtro = CrearFiltroExportacion(
                    fechaInicio,
                    fechaFin,
                    usuarioId,
                    proyectoId,
                    departamentoId);

                byte[] archivo = await _exportService
                    .GenerarPdfAsync(
                        filtro,
                        HttpContext.RequestAborted);

                return File(
                    archivo,
                    "application/pdf",
                    $"TIME_Horas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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
                    "TIME: Error al generar PDF.");

                return StatusCode(
                    500,
                    "No se pudo generar el archivo PDF.");
            }
        }


        public async Task<IActionResult> OnGetReporteHorasSqlAsync(
            string tipo,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            long? usuarioId,
            long? proyectoId,
            long? departamentoId,
            int pagina = 1,
            int tamanoPagina = 25)
        {
            if (tipo != "horas")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Este reporte todavía no está disponible."
                });
            }

            if ((usuarioId.HasValue && usuarioId.Value <= 0) ||
                (proyectoId.HasValue && proyectoId.Value <= 0) ||
                (departamentoId.HasValue && departamentoId.Value <= 0))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Los filtros contienen IDs inválidos."
                });
            }

            try
            {
                var filtros = new TimeReporteHorasFiltro
                {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    UsuarioId = usuarioId,
                    ProyectoId = proyectoId,
                    DepartamentoId = departamentoId,
                    Pagina = pagina,
                    TamanoPagina = tamanoPagina
                };

                var resultado = await _sqlReportService
                    .ConsultarHorasAsync(
                        filtros,
                        HttpContext.RequestAborted);

                return new JsonResult(new
                {
                    success = true,
                    reporte = resultado
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
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
                    "TIME: Error consultando reporte desde SQL Server.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al consultar el reporte."
                });
            }
        }


        public void OnGet()
        {
        }


        public async Task<IActionResult> OnGetCatalogosReportesAsync()
        {
            try
            {
                var catalogos =
                    await _catalogService.ObtenerCatalogosAsync(
                        HttpContext.RequestAborted);

                return new JsonResult(new
                {
                    success = true,
                    usuarios = catalogos.Usuarios,
                    proyectos = catalogos.Proyectos,
                    departamentos = catalogos.Departamentos
                });
            }
            catch (OperationCanceledException)
                when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                return new EmptyResult();
            }
            catch (Exception)
            {
                return StatusCode(502, new
                {
                    success = false,
                    message = "No fue posible obtener los catálogos de TIME."
                });
            }
        }


        public async Task<IActionResult> OnGetVistaPreviaReporteAsync(
            string tipo,
            string? fechaInicio,
            string? fechaFin,
            long? usuarioId,
            long? proyectoId,
            long? departamentoId)
        {
            if (tipo != "horas")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Este tipo de reporte todavía no está disponible."
                });
            }

            if (usuarioId < 1 && usuarioId.HasValue ||
                proyectoId < 1 && proyectoId.HasValue ||
                departamentoId < 1 && departamentoId.HasValue)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Los identificadores de filtro son inválidos."
                });
            }

            try
            {
                var resultado = await _previewService.ObtenerHorasAsync(
                    fechaInicio,
                    fechaFin,
                    usuarioId,
                    proyectoId,
                    departamentoId,
                    HttpContext.RequestAborted);

                return new JsonResult(new
                {
                    success = true,
                    reporte = resultado
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
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
                    "TIME: Error al construir la vista previa del reporte. Tipo: {Tipo}",
                    tipo);

                return StatusCode(500, new
                {
                    success = false,
                    message = "Error interno al construir la vista previa de TIME. Revisa los registros del servidor."
                });
            }
        }



        public async Task<IActionResult> OnGetRegistrosAsync(
            string tabla,
            long afterId = 0,
            long? upperId = null,
            int limit = 25)
        {
            if (string.IsNullOrWhiteSpace(tabla) ||
                !Tablas.Contains(tabla))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Recurso no autorizado."
                });
            }

            if (afterId < 0 ||
                upperId < 0 ||
                limit < 1 ||
                limit > 100)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Parámetros de paginación inválidos."
                });
            }

            try
            {
                using JsonDocument respuesta =
                    await _timeApiClient.ObtenerTablaAsync(
                        tabla: tabla,
                        afterId: afterId,
                        upperId: upperId,
                        limit: limit,
                        cancellationToken: HttpContext.RequestAborted);

                JsonElement root = respuesta.RootElement;

                if (!root.GetProperty("success").GetBoolean())
                {
                    return StatusCode(502, new
                    {
                        success = false,
                        message = "TIME no completó la consulta."
                    });
                }

                return new JsonResult(new
                {
                    success = true,
                    tabla,
                    data = root.GetProperty("data").Clone(),
                    sync = root.GetProperty("sync").Clone()
                });
            }
            catch (OperationCanceledException)
                when (HttpContext.RequestAborted.IsCancellationRequested)
            {
                return new EmptyResult();
            }
            catch (HttpRequestException)
            {
                return StatusCode(502, new
                {
                    success = false,
                    message = "No fue posible conectar con TIME API."
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al consultar TIME."
                });
            }
        }
    }
}
