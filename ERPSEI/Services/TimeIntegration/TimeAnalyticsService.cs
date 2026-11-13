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
    long? departamentoId = null,
    CancellationToken cancellationToken = default)
        {
            var consultaUsuarios =
                _db.TimeUsuarios
                    .AsNoTracking()
                    .Where(x =>
                        x.Estatus == 1 &&
                        x.Nombre != null &&
                        x.Nombre != "");

            if (departamentoId.HasValue &&
                departamentoId.Value > 0)
            {
                consultaUsuarios =
                    consultaUsuarios.Where(x =>
                        x.DepartamentoId ==
                        departamentoId.Value);
            }

            var usuarios =
                await consultaUsuarios
                    .OrderBy(x =>
                        x.Nombre)
                    .Select(x =>
                        new TimeCatalogOptionDto
                        {
                            Id =
                                x.Id,

                            Nombre =
                                x.Nombre!
                        })
                    .ToListAsync(
                        cancellationToken);

            var areas =
                await _db.TimeDepartamentos
                    .AsNoTracking()
                    .Where(x =>
                        x.Nombre != null &&
                        x.Nombre != "")
                    .OrderBy(x =>
                        x.Nombre)
                    .Select(x =>
                        new TimeCatalogOptionDto
                        {
                            Id =
                                x.Id,

                            Nombre =
                                x.Nombre!
                        })
                    .ToListAsync(
                        cancellationToken);

            return new TimeAnalyticsCatalogoDto
            {
                Usuarios =
                    usuarios,

                Areas =
                    areas
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

        public async Task<TimeAnalyticsEvaluacionAreasDto>
    ObtenerEvaluacionAreasAsync(
        TimeAnalyticsFiltroDto filtro,
        CancellationToken cancellationToken = default)
        {
            ValidarFiltro(
                filtro);

            List<TimeReporteHorasFila> registros =
                await ObtenerTodosLosRegistrosAsync(
                    filtro,
                    cancellationToken);

            // =====================================================
            // CATÁLOGOS DISPONIBLES ANTES DE FILTROS LOCALES
            // =====================================================

            var tipos =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Tipo))
                    .Select(x =>
                        x.Tipo!.Trim())
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x =>
                        x)
                    .ToList();

            var puestos =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Puesto))
                    .Select(x =>
                        x.Puesto!.Trim())
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x =>
                        x)
                    .ToList();

            // =====================================================
            // FILTROS LOCALES PBIX
            // =====================================================

            IEnumerable<TimeReporteHorasFila> consulta =
                registros;

            if (!string.IsNullOrWhiteSpace(
                filtro.Tipo))
            {
                consulta =
                    consulta.Where(x =>
                        string.Equals(
                            x.Tipo?.Trim(),
                            filtro.Tipo.Trim(),
                            StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(
                filtro.Puesto))
            {
                consulta =
                    consulta.Where(x =>
                        string.Equals(
                            x.Puesto?.Trim(),
                            filtro.Puesto.Trim(),
                            StringComparison.OrdinalIgnoreCase));
            }

            var filtrados =
                consulta.ToList();

            // =====================================================
            // KPIs
            // =====================================================

            double horas =
                filtrados.Sum(x =>
                    x.Horas ?? 0);

            int personas =
                filtrados
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Nombre))
                    .Select(x =>
                        x.Nombre!.Trim())
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .Count();

            // =====================================================
            // CLAVES CARGADAS POR ÁREA
            //
            // PBIX:
            // Descripción + Tipo + SUM(Horas)
            // =====================================================

            var claves =
                filtrados
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Descripcion))
                    .GroupBy(x =>
                        new
                        {
                            Descripcion =
                                x.Descripcion!.Trim(),

                            Tipo =
                                string.IsNullOrWhiteSpace(
                                    x.Tipo)
                                    ? "Sin clasificación"
                                    : x.Tipo!.Trim()
                        })
                    .Select(g =>
                        new TimeAnalyticsEvaluacionTipoDto
                        {
                            Descripcion =
                                g.Key.Descripcion,

                            Tipo =
                                g.Key.Tipo,

                            Horas =
                                g.Sum(x =>
                                    x.Horas ?? 0)
                        })
                    .OrderByDescending(x =>
                        x.Horas)
                    .Take(20)
                    .ToList();

            // =====================================================
            // DIVISIÓN POR PUESTO
            // =====================================================

            var distribucionPuestos =
                filtrados
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Puesto))
                    .GroupBy(
                        x =>
                            x.Puesto!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                        new TimeAnalyticsPuestoDto
                        {
                            Puesto =
                                g.Key,

                            Horas =
                                g.Sum(x =>
                                    x.Horas ?? 0)
                        })
                    .OrderByDescending(x =>
                        x.Horas)
                    .ToList();

            // =====================================================
            // EMPRESA / COLABORADOR
            // =====================================================

            var empresaColaborador =
                filtrados
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Nombre))
                    .GroupBy(x =>
                        new
                        {
                            GrupoEmpresa =
                                string.IsNullOrWhiteSpace(
                                    x.GrupoEmpresa)
                                    ? "Sin Grupo / Empresa"
                                    : x.GrupoEmpresa!.Trim(),

                            Nombre =
                                x.Nombre!.Trim()
                        })
                    .Select(g =>
                        new TimeAnalyticsEmpresaColaboradorDto
                        {
                            GrupoEmpresa =
                                g.Key.GrupoEmpresa,

                            Nombre =
                                g.Key.Nombre,

                            Horas =
                                g.Sum(x =>
                                    x.Horas ?? 0)
                        })
                    .OrderBy(x =>
                        x.GrupoEmpresa)
                    .ThenByDescending(x =>
                        x.Horas)
                    .ThenBy(x =>
                        x.Nombre)
                    .ToList();

            return new TimeAnalyticsEvaluacionAreasDto
            {
                Horas =
                    horas,

                Personas =
                    personas,

                Tipos =
                    tipos,

                Puestos =
                    puestos,

                Claves =
                    claves,

                DistribucionPuestos =
                    distribucionPuestos,

                EmpresaColaborador =
                    empresaColaborador,

                FechaActualizacion =
                    DateTime.Now
            };
        }


        // =====================================================
        // ANÁLISIS DE PERSONAL
        // =====================================================

        public async Task<TimeAnalyticsAnalisisPersonalDto>
            ObtenerAnalisisPersonalAsync(
                TimeAnalyticsFiltroDto filtro,
                CancellationToken cancellationToken = default)
        {
            TimeAnalyticsEvaluacionAreasDto baseAnalisis =
                await ObtenerEvaluacionAreasAsync(
                    filtro,
                    cancellationToken);

            return new TimeAnalyticsAnalisisPersonalDto
            {
                Horas =
                    baseAnalisis.Horas,

                Personas =
                    baseAnalisis.Personas,

                Tipos =
                    baseAnalisis.Tipos,

                Puestos =
                    baseAnalisis.Puestos,

                Claves =
                    baseAnalisis.Claves,

                DistribucionPuestos =
                    baseAnalisis.DistribucionPuestos,

                ColaboradoresEmpresa =
                    baseAnalisis.EmpresaColaborador
                        .OrderByDescending(x =>
                            x.Horas)
                        .ThenBy(x =>
                            x.Nombre)
                        .ThenBy(x =>
                            x.GrupoEmpresa)
                        .ToList(),

                FechaActualizacion =
                    DateTime.Now
            };
        }


        // =====================================================
        // ALINEACIÓN BACK OFFICE
        // =====================================================

        public async Task<TimeAnalyticsBackOfficeDto>
            ObtenerAlineacionBackOfficeAsync(
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
            // CLASIFICACIÓN AGRUPADA DEL PBIX
            //
            // Administrativa   -> Administrativa
            // Back Office      -> Horas Back Office
            // Operativa/Clientes y resto -> Clientes
            // =================================================

            string ObtenerClasificacionAgrupada(
                TimeReporteHorasFila fila)
            {
                if (EsTipo(
                    fila.Tipo,
                    "Administrativa"))
                {
                    return "Administrativa";
                }

                if (EsTipo(
                    fila.Tipo,
                    "Back Office"))
                {
                    return "Horas Back Office";
                }

                return "Clientes";
            }

            // =================================================
            // KPIs
            // =================================================

            double horasTotales =
                registros.Sum(x =>
                    x.Horas ?? 0);

            double horasBackOffice =
                registros
                    .Where(x =>
                        EsTipo(
                            x.Tipo,
                            "Back Office"))
                    .Sum(x =>
                        x.Horas ?? 0);

            // Evaluación Back Office:
            // proporción de horas clasificadas como Back Office
            // respecto a todas las horas registradas del contexto.
            double evaluacionBackOffice =
                Dividir(
                    horasBackOffice,
                    horasTotales);

            // =================================================
            // DISTRIBUCIÓN DE ACTIVIDADES
            // =================================================

            var distribucion =
                registros
                    .GroupBy(
                        ObtenerClasificacionAgrupada,
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                        new TimeAnalyticsBackOfficeDistribucionDto
                        {
                            Clasificacion =
                                g.Key,

                            Horas =
                                g.Sum(x =>
                                    x.Horas ?? 0)
                        })
                    .OrderByDescending(x =>
                        x.Horas)
                    .ToList();

            // =================================================
            // HORAS CARGADAS POR ÁREA
            // =================================================

            var areas =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Area))
                    .GroupBy(
                        x => x.Area!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                    {
                        double total =
                            g.Sum(x =>
                                x.Horas ?? 0);

                        double backOffice =
                            g.Where(x =>
                                    EsTipo(
                                        x.Tipo,
                                        "Back Office"))
                                .Sum(x =>
                                    x.Horas ?? 0);

                        double administrativas =
                            g.Where(x =>
                                    EsTipo(
                                        x.Tipo,
                                        "Administrativa"))
                                .Sum(x =>
                                    x.Horas ?? 0);

                        double clientes =
                            total -
                            backOffice -
                            administrativas;

                        if (clientes < 0)
                        {
                            clientes = 0;
                        }

                        return new TimeAnalyticsBackOfficeSerieDto
                        {
                            Etiqueta =
                                g.Key,

                            HorasBackOffice =
                                backOffice,

                            HorasAdministrativas =
                                administrativas,

                            HorasClientes =
                                clientes,

                            HorasTotales =
                                total,

                            Evaluacion =
                                Dividir(
                                    backOffice,
                                    total)
                        };
                    })
                    .OrderByDescending(x =>
                        x.HorasTotales)
                    .ThenBy(x =>
                        x.Etiqueta)
                    .ToList();

            // =================================================
            // TENDENCIA DE HORAS POR COLABORADOR
            // =================================================

            var colaboradores =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Nombre))
                    .GroupBy(
                        x => x.Nombre!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                    {
                        double total =
                            g.Sum(x =>
                                x.Horas ?? 0);

                        double backOffice =
                            g.Where(x =>
                                    EsTipo(
                                        x.Tipo,
                                        "Back Office"))
                                .Sum(x =>
                                    x.Horas ?? 0);

                        double administrativas =
                            g.Where(x =>
                                    EsTipo(
                                        x.Tipo,
                                        "Administrativa"))
                                .Sum(x =>
                                    x.Horas ?? 0);

                        double clientes =
                            total -
                            backOffice -
                            administrativas;

                        if (clientes < 0)
                        {
                            clientes = 0;
                        }

                        return new TimeAnalyticsBackOfficeSerieDto
                        {
                            Etiqueta =
                                g.Key,

                            HorasBackOffice =
                                backOffice,

                            HorasAdministrativas =
                                administrativas,

                            HorasClientes =
                                clientes,

                            HorasTotales =
                                total,

                            Evaluacion =
                                Dividir(
                                    backOffice,
                                    total)
                        };
                    })
                    .OrderByDescending(x =>
                        x.HorasTotales)
                    .ThenBy(x =>
                        x.Etiqueta)
                    .ToList();

            // =================================================
            // RESUMEN DE HALLAZGOS
            // =================================================

            var hallazgos =
                areas
                    .Select(x =>
                    {
                        string nivel;
                        string claseCss;

                        double porcentaje =
                            x.Evaluacion * 100d;

                        if (x.HorasTotales <= 0 ||
                            porcentaje <= 0)
                        {
                            nivel =
                                "Sin horas";

                            claseCss =
                                "level-none";
                        }
                        else if (porcentaje < 30)
                        {
                            nivel =
                                "Muy bajo";

                            claseCss =
                                "level-very-low";
                        }
                        else if (porcentaje < 50)
                        {
                            nivel =
                                "Bajo";

                            claseCss =
                                "level-low";
                        }
                        else if (porcentaje <= 80)
                        {
                            nivel =
                                "Medio";

                            claseCss =
                                "level-medium";
                        }
                        else
                        {
                            nivel =
                                "Alto";

                            claseCss =
                                "level-high";
                        }

                        return new TimeAnalyticsBackOfficeHallazgoDto
                        {
                            Area =
                                x.Etiqueta,

                            HorasTotales =
                                x.HorasTotales,

                            HorasBackOffice =
                                x.HorasBackOffice,

                            Evaluacion =
                                x.Evaluacion,

                            Nivel =
                                nivel,

                            ClaseCss =
                                claseCss
                        };
                    })
                    .OrderBy(x =>
                        x.Evaluacion)
                    .ThenBy(x =>
                        x.Area)
                    .ToList();

            return new TimeAnalyticsBackOfficeDto
            {
                HorasTotales =
                    horasTotales,

                HorasBackOffice =
                    horasBackOffice,

                EvaluacionBackOffice =
                    evaluacionBackOffice,

                Distribucion =
                    distribucion,

                Areas =
                    areas,

                Colaboradores =
                    colaboradores,

                Hallazgos =
                    hallazgos,

                FechaActualizacion =
                    DateTime.Now
            };
        }


        // =====================================================
        // ALINEACIÓN OPERATIVAS
        // =====================================================

        public async Task<TimeAnalyticsOperativasDto>
            ObtenerAlineacionOperativasAsync(
                TimeAnalyticsFiltroDto filtro,
                CancellationToken cancellationToken = default)
        {
            ValidarFiltro(
                filtro);

            List<TimeReporteHorasFila> registros =
                await ObtenerTodosLosRegistrosAsync(
                    filtro,
                    cancellationToken);

            double horasTotales =
                registros.Sum(x =>
                    x.Horas ?? 0);

            double horasOperativasClientes =
                registros
                    .Where(x =>
                        EsTipo(
                            x.Tipo,
                            "Operativa") ||
                        EsTipo(
                            x.Tipo,
                            "Clientes"))
                    .Sum(x =>
                        x.Horas ?? 0);

            double evaluacion =
                Dividir(
                    horasOperativasClientes,
                    horasTotales);

            string Clasificacion(
                TimeReporteHorasFila fila)
            {
                if (EsTipo(
                    fila.Tipo,
                    "Operativa"))
                {
                    return "Operativa";
                }

                if (EsTipo(
                    fila.Tipo,
                    "Clientes"))
                {
                    return "Clientes";
                }

                if (EsTipo(
                    fila.Tipo,
                    "Administrativa"))
                {
                    return "Administrativa";
                }

                if (EsTipo(
                    fila.Tipo,
                    "Back Office"))
                {
                    return "Back Office";
                }

                return "Sin clasificación";
            }

            var distribucion =
                registros
                    .GroupBy(
                        Clasificacion,
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                        new TimeAnalyticsBackOfficeDistribucionDto
                        {
                            Clasificacion =
                                g.Key,

                            Horas =
                                g.Sum(x =>
                                    x.Horas ?? 0)
                        })
                    .OrderByDescending(x =>
                        x.Horas)
                    .ToList();

            TimeAnalyticsBackOfficeSerieDto CrearSerie(
                string etiqueta,
                IEnumerable<TimeReporteHorasFila> grupo)
            {
                var lista =
                    grupo.ToList();

                double total =
                    lista.Sum(x =>
                        x.Horas ?? 0);

                double operativa =
                    lista
                        .Where(x =>
                            EsTipo(
                                x.Tipo,
                                "Operativa"))
                        .Sum(x =>
                            x.Horas ?? 0);

                double clientes =
                    lista
                        .Where(x =>
                            EsTipo(
                                x.Tipo,
                                "Clientes"))
                        .Sum(x =>
                            x.Horas ?? 0);

                double administrativas =
                    lista
                        .Where(x =>
                            EsTipo(
                                x.Tipo,
                                "Administrativa"))
                        .Sum(x =>
                            x.Horas ?? 0);

                return new TimeAnalyticsBackOfficeSerieDto
                {
                    Etiqueta =
                        etiqueta,

                    HorasBackOffice =
                        0,

                    HorasAdministrativas =
                        administrativas,

                    HorasClientes =
                        clientes + operativa,

                    HorasTotales =
                        total,

                    Evaluacion =
                        Dividir(
                            operativa + clientes,
                            total)
                };
            }

            var areas =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Area))
                    .GroupBy(
                        x => x.Area!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                        CrearSerie(
                            g.Key,
                            g))
                    .OrderByDescending(x =>
                        x.HorasTotales)
                    .ThenBy(x =>
                        x.Etiqueta)
                    .ToList();

            var colaboradores =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Nombre))
                    .GroupBy(
                        x => x.Nombre!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                        CrearSerie(
                            g.Key,
                            g))
                    .OrderByDescending(x =>
                        x.HorasTotales)
                    .ThenBy(x =>
                        x.Etiqueta)
                    .ToList();

            var totalArea =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Area))
                    .GroupBy(
                        x => x.Area!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(x =>
                            x.Horas ?? 0),
                        StringComparer.OrdinalIgnoreCase);

            var resumen =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Area))
                    .GroupBy(
                        x =>
                            new
                            {
                                Area =
                                    x.Area!.Trim(),

                                Tipo =
                                    string.IsNullOrWhiteSpace(
                                        x.Tipo)
                                        ? "Sin clasificación"
                                        : x.Tipo!.Trim()
                            })
                    .Select(g =>
                    {
                        double horas =
                            g.Sum(x =>
                                x.Horas ?? 0);

                        double totalAreaValor =
                            totalArea.TryGetValue(
                                g.Key.Area,
                                out double total)
                                ? total
                                : 0;

                        return new TimeAnalyticsOperativasResumenDto
                        {
                            Area =
                                g.Key.Area,

                            Tipo =
                                g.Key.Tipo,

                            Horas =
                                horas,

                            Participacion =
                                Dividir(
                                    horas,
                                    totalAreaValor)
                        };
                    })
                    .OrderBy(x =>
                        x.Area)
                    .ThenByDescending(x =>
                        x.Horas)
                    .ToList();

            return new TimeAnalyticsOperativasDto
            {
                HorasTotales =
                    horasTotales,

                HorasOperativasClientes =
                    horasOperativasClientes,

                EvaluacionOperativasClientes =
                    evaluacion,

                Distribucion =
                    distribucion,

                Areas =
                    areas,

                Colaboradores =
                    colaboradores,

                Resumen =
                    resumen,

                FechaActualizacion =
                    DateTime.Now
            };
        }


        // =====================================================
        // HORAS EXTRAS
        // =====================================================

        public async Task<TimeAnalyticsHorasExtrasDto>
            ObtenerHorasExtrasAsync(
                TimeAnalyticsFiltroDto filtro,
                CancellationToken cancellationToken = default)
        {
            ValidarFiltro(
                filtro);

            List<TimeReporteHorasFila> registros =
                await ObtenerTodosLosRegistrosAsync(
                    filtro,
                    cancellationToken);

            var grupos =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Nombre))
                    .GroupBy(
                        x => x.Nombre!.Trim(),
                        StringComparer.OrdinalIgnoreCase)
                    .ToList();

            var colaboradores =
                new List<TimeAnalyticsHorasExtrasColaboradorDto>();

            foreach (var grupo
                     in grupos)
            {
                double horasColaborador =
                    grupo.Sum(x =>
                        x.Horas ?? 0);

                double extras =
                    Math.Max(
                        horasColaborador -
                        MetaHorasPersona,
                        0);

                double faltantes =
                    Math.Max(
                        MetaHorasPersona -
                        horasColaborador,
                        0);

                string clasificacion;
                string claseCss;

                if (horasColaborador >
                    MetaHorasPersona)
                {
                    clasificacion =
                        "Con horas extras";

                    claseCss =
                        "extras-high";
                }
                else if (Math.Abs(
                    horasColaborador -
                    MetaHorasPersona) <
                    0.0001)
                {
                    clasificacion =
                        "Cumplimiento";

                    claseCss =
                        "extras-ok";
                }
                else
                {
                    clasificacion =
                        "Por completar";

                    claseCss =
                        "extras-pending";
                }

                string area =
                    grupo
                        .Select(x =>
                            x.Area)
                        .FirstOrDefault(x =>
                            !string.IsNullOrWhiteSpace(
                                x))
                    ?? "Sin área";

                colaboradores.Add(
                    new TimeAnalyticsHorasExtrasColaboradorDto
                    {
                        Nombre =
                            grupo.Key,

                        Area =
                            area,

                        Horas =
                            horasColaborador,

                        HorasExtras =
                            extras,

                        HorasFaltantes =
                            faltantes,

                        Clasificacion =
                            clasificacion,

                        ClaseCss =
                            claseCss
                    });
            }

            colaboradores =
                colaboradores
                    .OrderBy(x =>
                        x.Nombre,
                        StringComparer.OrdinalIgnoreCase)
                    .ToList();

            int personas =
                colaboradores.Count;

            double horas =
                colaboradores.Sum(x =>
                    x.Horas);

            double horasExtras =
                colaboradores.Sum(x =>
                    x.HorasExtras);

            double horasTotales =
                personas *
                MetaHorasPersona;

            double horasFaltantes =
                colaboradores.Sum(x =>
                    x.HorasFaltantes);

            double promedioPersona =
                personas == 0
                    ? 0
                    : horas /
                      personas;

            var clasificaciones =
                colaboradores
                    .GroupBy(
                        x =>
                            x.Clasificacion,
                        StringComparer.OrdinalIgnoreCase)
                    .Select(g =>
                        new TimeAnalyticsHorasExtrasClasificacionDto
                        {
                            Clasificacion =
                                g.Key,

                            Personas =
                                g.Count()
                        })
                    .OrderByDescending(x =>
                        x.Personas)
                    .ToList();

            var extrasPorUsuario =
                colaboradores
                    .ToDictionary(
                        x =>
                            x.Nombre,
                        x =>
                            x.HorasExtras,
                        StringComparer.OrdinalIgnoreCase);

            var areasTipo =
                registros
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Nombre) &&
                        !string.IsNullOrWhiteSpace(
                            x.Area))
                    .GroupBy(
                        x =>
                            new
                            {
                                Nombre =
                                    x.Nombre!.Trim(),

                                Area =
                                    x.Area!.Trim(),

                                Tipo =
                                    string.IsNullOrWhiteSpace(
                                        x.Tipo)
                                        ? "Sin clasificación"
                                        : x.Tipo!.Trim()
                            })
                    .Select(g =>
                    {
                        double horasTipo =
                            g.Sum(x =>
                                x.Horas ?? 0);

                        double horasUsuario =
                            registros
                                .Where(x =>
                                    string.Equals(
                                        x.Nombre?.Trim(),
                                        g.Key.Nombre,
                                        StringComparison.OrdinalIgnoreCase))
                                .Sum(x =>
                                    x.Horas ?? 0);

                        double extrasUsuario =
                            Math.Max(
                                horasUsuario -
                                MetaHorasPersona,
                                0);

                        double proporcion =
                            horasUsuario <= 0
                                ? 0
                                : horasTipo /
                                  horasUsuario;

                        return new
                        {
                            g.Key.Area,
                            g.Key.Tipo,
                            Extras =
                                extrasUsuario *
                                proporcion
                        };
                    })
                    .GroupBy(
                        x =>
                            new
                            {
                                x.Area,
                                x.Tipo
                            })
                    .Select(g =>
                        new TimeAnalyticsHorasExtrasAreaDto
                        {
                            Area =
                                g.Key.Area,

                            Tipo =
                                g.Key.Tipo,

                            HorasExtras =
                                g.Sum(x =>
                                    x.Extras)
                        })
                    .Where(x =>
                        x.HorasExtras > 0)
                    .OrderByDescending(x =>
                        x.HorasExtras)
                    .ToList();

            return new TimeAnalyticsHorasExtrasDto
            {
                Horas =
                    horas,

                HorasExtras =
                    horasExtras,

                HorasTotales =
                    horasTotales,

                HorasFaltantes =
                    horasFaltantes,

                PromedioPersona =
                    promedioPersona,

                Colaboradores =
                    colaboradores,

                Clasificaciones =
                    clasificaciones,

                AreasTipo =
                    areasTipo,

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