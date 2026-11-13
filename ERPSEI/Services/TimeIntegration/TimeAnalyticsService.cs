using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeAnalyticsService
        : ITimeAnalyticsService
    {
        private readonly ITimeSqlReportService _reportService;
        private readonly ApplicationDbContext _db;

        private const int TamanoPagina = 50000;

        private const double MetaHorasPersona = 176d;

        public TimeAnalyticsService(
            ITimeSqlReportService reportService,
            ApplicationDbContext db)
        {
            _reportService = reportService;
            _db = db;
        }

        // =====================================================
        // CATÁLOGOS
        // =====================================================

        public async Task<TimeAnalyticsCatalogoDto> ObtenerCatalogosAsync(
            CancellationToken cancellationToken = default)
        {
            var usuarios =
                await _db.TimeUsuarios
                    .AsNoTracking()
                    .Where(x =>
                        x.Estatus == 1 &&
                        x.Nombre != null &&
                        x.Nombre != "")
                    .OrderBy(x => x.Nombre)
                    .Select(x =>
                        new TimeCatalogOptionDto
                        {
                            Id = x.Id,
                            Nombre = x.Nombre!
                        })
                    .ToListAsync(
                        cancellationToken);

            var areas =
                await _db.TimeDepartamentos
                    .AsNoTracking()
                    .Where(x =>
                        x.Nombre != null &&
                        x.Nombre != "")
                    .OrderBy(x => x.Nombre)
                    .Select(x =>
                        new TimeCatalogOptionDto
                        {
                            Id = x.Id,
                            Nombre = x.Nombre!
                        })
                    .ToListAsync(
                        cancellationToken);

            return new TimeAnalyticsCatalogoDto
            {
                Usuarios = usuarios,
                Areas = areas
            };
        }

        // =====================================================
        // DASHBOARD
        // =====================================================

        public async Task<TimeAnalyticsDashboardDto> ObtenerDashboardAsync(
            TimeAnalyticsFiltroDto filtro,
            CancellationToken cancellationToken = default)
        {
            ValidarFiltro(
                filtro);

            List<TimeReporteHorasFila> registros =
                await ObtenerTodosLosRegistrosAsync(
                    filtro,
                    cancellationToken);

            // =================================================
            // COLABORADORES
            // =================================================

            var gruposColaborador =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Nombre))
                    .GroupBy(
                        x => x.Nombre!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .ToList();

            var colaboradores =
                new List<TimeAnalyticsColaboradorDto>();

            foreach (var grupo in gruposColaborador)
            {
                double horas =
                    grupo.Sum(x =>
                        x.Horas ?? 0);

                double horasAdministrativas =
                    grupo
                        .Where(x =>
                            EsTipo(
                                x.Tipo,
                                "Administrativa"))
                        .Sum(x =>
                            x.Horas ?? 0);

                double horasBackOffice =
                    grupo
                        .Where(x =>
                            EsTipo(
                                x.Tipo,
                                "Back Office"))
                        .Sum(x =>
                            x.Horas ?? 0);

                double horasOperativasClientes =
                    grupo
                        .Where(x =>
                            EsTipo(
                                x.Tipo,
                                "Operativa") ||
                            EsTipo(
                                x.Tipo,
                                "Clientes"))
                        .Sum(x =>
                            x.Horas ?? 0);

                // DAX:
                // Horas Totales = Personas * 176
                //
                // Para una persona:
                // 1 * 176.
                double horasTotales =
                    MetaHorasPersona;

                // DAX:
                // Horas no Registradas =
                // Horas Totales - SUM(Horas)
                double horasNoRegistradas =
                    horasTotales -
                    horas;

                // DAX:
                // Extras =
                // IF(Horas no registradas < 0,
                //    ABS(Horas no registradas),
                //    0)
                double extras =
                    horasNoRegistradas < 0
                        ? Math.Abs(
                            horasNoRegistradas)
                        : 0;

                // DAX:
                // Horas Ajustadas =
                // Horas Totales + Extras
                double horasAjustadas =
                    horasTotales +
                    extras;

                // DAX:
                // Horas Admin + Back Office
                double horasAdminBackOffice =
                    horasAdministrativas +
                    horasBackOffice;

                double porcentajeAdministrativas =
                    Dividir(
                        horasAdminBackOffice,
                        horasAjustadas);

                double porcentajeOperativasClientes =
                    Dividir(
                        horasOperativasClientes,
                        horasAjustadas);

                // DAX:
                // Tendencia =
                // % Administrativas +
                // % Operativas + Clientes
                double tendencia =
                    porcentajeAdministrativas +
                    porcentajeOperativasClientes;

                string area =
                    grupo
                        .Select(x =>
                            x.Area)
                        .FirstOrDefault(x =>
                            !string.IsNullOrWhiteSpace(
                                x))
                    ?? "Sin área";

                colaboradores.Add(
                    new TimeAnalyticsColaboradorDto
                    {
                        Nombre =
                            grupo.Key,

                        Area =
                            area,

                        Horas =
                            horas,

                        // DAX representa decimal:
                        // 1 = 100%.
                        Cumplimiento =
                            tendencia,

                        ColorHoras =
                            ObtenerColorHoras(
                                horas)
                    });
            }

            // =================================================
            // KPI HORAS
            // =================================================

            double horasRegistradas =
                registros.Sum(x =>
                    x.Horas ?? 0);

            int totalColaboradores =
                colaboradores.Count;

            double horasTotalesGlobal =
                totalColaboradores *
                MetaHorasPersona;

            // DAX Horas Faltantes:
            //
            // IF(
            //   HorasArea >= HorasTotales,
            //   0,
            //   HorasTotales - HorasArea
            // )
            double horasFaltantesGlobal =
                horasRegistradas >=
                horasTotalesGlobal
                    ? 0
                    : horasTotalesGlobal -
                      horasRegistradas;

            // DAX:
            // Cumplimiento Global =
            // SUMX(
            //   VALUES(Nombre),
            //   Tendencia
            // )
            // / COUNTROWS(VALUES(Nombre))
            double cumplimientoGlobal =
                colaboradores.Count == 0
                    ? 0
                    : colaboradores.Average(
                        x =>
                            x.Cumplimiento);

            int totalAreas =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Area))
                    .Select(x =>
                        x.Area!.Trim())
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .Count();

            // =================================================
            // DISTRIBUCIÓN POR TIPO
            // =================================================

            var distribucionTipo =
                registros
                    .GroupBy(
                        x =>
                            string.IsNullOrWhiteSpace(
                                x.Tipo)
                                ? "Sin clasificación"
                                : x.Tipo!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                        new TimeAnalyticsTipoDto
                        {
                            Tipo =
                                g.Key,

                            Horas =
                                g.Sum(x =>
                                    x.Horas ?? 0)
                        })
                    .OrderByDescending(x =>
                        x.Horas)
                    .ToList();

            // =================================================
            // TENDENCIA DE HORAS POR FECHA
            // =================================================

            var tendenciaHoras =
                registros
                    .Where(x =>
                        x.Fecha.HasValue)
                    .GroupBy(x =>
                        x.Fecha!.Value.Date)
                    .Select(g =>
                        new TimeAnalyticsTendenciaDto
                        {
                            Fecha =
                                g.Key,

                            Horas =
                                g.Sum(x =>
                                    x.Horas ?? 0)
                        })
                    .OrderBy(x =>
                        x.Fecha)
                    .ToList();

            // =================================================
            // CUMPLIMIENTO POR ÁREA
            // =================================================

            var areas =
                new List<TimeAnalyticsAreaDto>();

            foreach (var grupoArea in
                     registros
                         .Where(x =>
                             !string.IsNullOrWhiteSpace(
                                 x.Area))
                         .GroupBy(
                             x =>
                                 x.Area!.Trim(),
                             StringComparer.OrdinalIgnoreCase))
            {
                string nombreArea =
                    grupoArea.Key;

                var nombresArea =
                    grupoArea
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(
                                x.Nombre))
                        .Select(x =>
                            x.Nombre!.Trim())
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase)
                        .ToList();

                int personas =
                    nombresArea.Count;

                double horas =
                    grupoArea.Sum(x =>
                        x.Horas ?? 0);

                double horasTotales =
                    personas *
                    MetaHorasPersona;

                double horasFaltantes =
                    horas >= horasTotales
                        ? 0
                        : horasTotales -
                          horas;

                double horasBackOffice =
                    grupoArea
                        .Where(x =>
                            EsTipo(
                                x.Tipo,
                                "Back Office"))
                        .Sum(x =>
                            x.Horas ?? 0);

                double horasOperativasClientes =
                    grupoArea
                        .Where(x =>
                            EsTipo(
                                x.Tipo,
                                "Operativa") ||
                            EsTipo(
                                x.Tipo,
                                "Clientes"))
                        .Sum(x =>
                            x.Horas ?? 0);

                var colaboradoresArea =
                    colaboradores
                        .Where(x =>
                            string.Equals(
                                x.Area,
                                nombreArea,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                double cumplimientoArea =
                    colaboradoresArea.Count == 0
                        ? 0
                        : colaboradoresArea
                            .Average(x =>
                                x.Cumplimiento);

                areas.Add(
                    new TimeAnalyticsAreaDto
                    {
                        Area =
                            nombreArea,

                        Personas =
                            personas,

                        Horas =
                            horas,

                        HorasBackOffice =
                            horasBackOffice,

                        HorasOperativasClientes =
                            horasOperativasClientes,

                        HorasFaltantes =
                            horasFaltantes,

                        Cumplimiento =
                            cumplimientoArea,

                        ColorTendencia =
                            ObtenerColorTendencia(
                                cumplimientoArea)
                    });
            }

            areas =
                areas
                    .OrderByDescending(x =>
                        x.Horas)
                    .ThenBy(x =>
                        x.Area)
                    .ToList();

            colaboradores =
                colaboradores
                    .OrderByDescending(x =>
                        x.Horas)
                    .ThenBy(x =>
                        x.Nombre)
                    .ToList();

            // =================================================
            // RESULTADO
            // =================================================

            return new TimeAnalyticsDashboardDto
            {
                Kpis =
                    new TimeAnalyticsKpisDto
                    {
                        HorasRegistradas =
                            horasRegistradas,

                        HorasFaltantes =
                            horasFaltantesGlobal,

                        Cumplimiento =
                            cumplimientoGlobal,

                        Colaboradores =
                            totalColaboradores,

                        Areas =
                            totalAreas
                    },

                DistribucionTipo =
                    distribucionTipo,

                Tendencia =
                    tendenciaHoras,

                Areas =
                    areas,

                Colaboradores =
                    colaboradores,

                FechaActualizacion =
                    DateTime.Now
            };
        }

        // =====================================================
        // LEER TODOS LOS REGISTROS
        // =====================================================

        private async Task<List<TimeReporteHorasFila>>
            ObtenerTodosLosRegistrosAsync(
                TimeAnalyticsFiltroDto filtro,
                CancellationToken cancellationToken)
        {
            var registros =
                new List<TimeReporteHorasFila>();

            var primerFiltro =
                CrearFiltroReporte(
                    filtro,
                    pagina: 1);

            TimeReporteHorasResultado primera =
                await _reportService
                    .ConsultarHorasAsync(
                        primerFiltro,
                        cancellationToken);

            registros.AddRange(
                primera.Registros);

            if (primera.TotalPaginas <= 1)
            {
                return registros;
            }

            for (int pagina = 2;
                 pagina <= primera.TotalPaginas;
                 pagina++)
            {
                cancellationToken
                    .ThrowIfCancellationRequested();

                TimeReporteHorasResultado resultado =
                    await _reportService
                        .ConsultarHorasAsync(
                            CrearFiltroReporte(
                                filtro,
                                pagina),
                            cancellationToken);

                registros.AddRange(
                    resultado.Registros);
            }

            return registros;
        }

        private static TimeReporteHorasFiltro
            CrearFiltroReporte(
                TimeAnalyticsFiltroDto filtro,
                int pagina)
        {
            return new TimeReporteHorasFiltro
            {
                FechaInicio =
                    filtro.FechaInicio,

                FechaFin =
                    filtro.FechaFin,

                UsuarioId =
                    filtro.UsuarioId,

                DepartamentoId =
                    filtro.DepartamentoId,

                ProyectoId =
                    null,

                Pagina =
                    pagina,

                TamanoPagina =
                    TamanoPagina
            };
        }

        // =====================================================
        // HELPERS DAX
        // =====================================================

        private static bool EsTipo(
            string? valor,
            string tipo)
        {
            return string.Equals(
                valor?.Trim(),
                tipo,
                StringComparison.OrdinalIgnoreCase);
        }

        private static double Dividir(
            double numerador,
            double denominador)
        {
            if (denominador == 0)
            {
                return 0;
            }

            return numerador /
                   denominador;
        }

        private static string ObtenerColorHoras(
            double horas)
        {
            // DAX Color Horas TIME

            if (horas > MetaHorasPersona)
            {
                return "#DB5375";
            }

            if (Math.Abs(
                    horas -
                    MetaHorasPersona) < 0.0001)
            {
                return "#76F3EF";
            }

            if (horas < MetaHorasPersona)
            {
                return "#7D8CA3";
            }

            return "#BDBDBD";
        }

        private static string ObtenerColorTendencia(
            double cumplimiento)
        {
            // DAX Color Tendencia:
            //
            // < 1 = #F2F2F2
            // >= 1 = #00DDC2

            return cumplimiento < 1
                ? "#F2F2F2"
                : "#00DDC2";
        }

        // =====================================================
        // VALIDACIÓN
        // =====================================================

        private static void ValidarFiltro(
            TimeAnalyticsFiltroDto filtro)
        {
            if (filtro.FechaInicio.HasValue &&
                filtro.FechaFin.HasValue &&
                filtro.FechaInicio.Value.Date >
                filtro.FechaFin.Value.Date)
            {
                throw new ArgumentException(
                    "La fecha inicial no puede superar la fecha final.");
            }

            if (filtro.UsuarioId.HasValue &&
                filtro.UsuarioId.Value <= 0)
            {
                throw new ArgumentException(
                    "El colaborador seleccionado es inválido.");
            }

            if (filtro.DepartamentoId.HasValue &&
                filtro.DepartamentoId.Value <= 0)
            {
                throw new ArgumentException(
                    "El área seleccionada es inválida.");
            }
        }
    }
}