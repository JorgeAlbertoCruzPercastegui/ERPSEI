namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeAnalyticsFiltroDto
    {
        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public long? UsuarioId { get; set; }

        public long? DepartamentoId { get; set; }

        public string? Tipo { get; set; }

        public string? Puesto { get; set; }
    }

    public sealed class TimeAnalyticsCatalogoDto
    {
        public List<TimeCatalogOptionDto> Usuarios { get; set; } = new();

        public List<TimeCatalogOptionDto> Areas { get; set; } = new();
    }

    public sealed class TimeAnalyticsKpisDto
    {
        public double HorasRegistradas { get; set; }

        public double HorasFaltantes { get; set; }

        public double Cumplimiento { get; set; }

        public int Colaboradores { get; set; }

        public int Areas { get; set; }
    }

    public sealed class TimeAnalyticsTipoDto
    {
        public string Tipo { get; set; } = "";

        public double Horas { get; set; }
    }

    public sealed class TimeAnalyticsTendenciaDto
    {
        public DateTime Fecha { get; set; }

        public double Horas { get; set; }
    }

    public sealed class TimeAnalyticsAreaDto
    {
        public string Area { get; set; } = "";

        public int Personas { get; set; }

        public double Horas { get; set; }

        public double HorasBackOffice { get; set; }

        public double HorasOperativasClientes { get; set; }

        public double HorasFaltantes { get; set; }

        public double Cumplimiento { get; set; }

        public string ColorTendencia { get; set; } = "";
    }

    public sealed class TimeAnalyticsColaboradorDto
    {
        public string Nombre { get; set; } = "";

        public string Area { get; set; } = "";

        public double Horas { get; set; }

        public double Cumplimiento { get; set; }

        public string ColorHoras { get; set; } = "";
    }

    public sealed class TimeAnalyticsEvaluacionTipoDto
    {
        public string Descripcion { get; set; } = "";

        public string Tipo { get; set; } = "";

        public double Horas { get; set; }
    }

    public sealed class TimeAnalyticsPuestoDto
    {
        public string Puesto { get; set; } = "";

        public double Horas { get; set; }
    }

    public sealed class TimeAnalyticsEmpresaColaboradorDto
    {
        public string GrupoEmpresa { get; set; } = "";

        public string Nombre { get; set; } = "";

        public double Horas { get; set; }
    }

    public sealed class TimeAnalyticsEvaluacionAreasDto
    {
        public double Horas { get; set; }

        public int Personas { get; set; }

        public List<string> Tipos { get; set; } = new();

        public List<string> Puestos { get; set; } = new();

        public List<TimeAnalyticsEvaluacionTipoDto> Claves { get; set; } = new();

        public List<TimeAnalyticsPuestoDto> DistribucionPuestos { get; set; } = new();

        public List<TimeAnalyticsEmpresaColaboradorDto> EmpresaColaborador { get; set; } = new();

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }

    public sealed class TimeAnalyticsAnalisisPersonalDto
    {
        public double Horas { get; set; }

        public int Personas { get; set; }

        public List<string> Tipos { get; set; } = new();

        public List<string> Puestos { get; set; } = new();

        public List<TimeAnalyticsEvaluacionTipoDto> Claves { get; set; } = new();

        public List<TimeAnalyticsPuestoDto> DistribucionPuestos { get; set; } = new();

        public List<TimeAnalyticsEmpresaColaboradorDto> ColaboradoresEmpresa { get; set; } = new();

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }

    public sealed class TimeAnalyticsBackOfficeDistribucionDto
    {
        public string Clasificacion { get; set; } = "";

        public double Horas { get; set; }
    }

    public sealed class TimeAnalyticsBackOfficeSerieDto
    {
        public string Etiqueta { get; set; } = "";

        public double HorasBackOffice { get; set; }

        public double HorasAdministrativas { get; set; }

        public double HorasClientes { get; set; }

        public double HorasTotales { get; set; }

        public double Evaluacion { get; set; }
    }

    public sealed class TimeAnalyticsBackOfficeHallazgoDto
    {
        public string Area { get; set; } = "";

        public double HorasTotales { get; set; }

        public double HorasBackOffice { get; set; }

        public double Evaluacion { get; set; }

        public string Nivel { get; set; } = "";

        public string ClaseCss { get; set; } = "";
    }

    public sealed class TimeAnalyticsBackOfficeDto
    {
        public double HorasTotales { get; set; }

        public double HorasBackOffice { get; set; }

        public double EvaluacionBackOffice { get; set; }

        public List<TimeAnalyticsBackOfficeDistribucionDto> Distribucion { get; set; } = new();

        public List<TimeAnalyticsBackOfficeSerieDto> Areas { get; set; } = new();

        public List<TimeAnalyticsBackOfficeSerieDto> Colaboradores { get; set; } = new();

        public List<TimeAnalyticsBackOfficeHallazgoDto> Hallazgos { get; set; } = new();

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }


    public sealed class TimeAnalyticsOperativasResumenDto
    {
        public string Area { get; set; } = "";

        public string Tipo { get; set; } = "";

        public double Horas { get; set; }

        public double Participacion { get; set; }
    }

    public sealed class TimeAnalyticsOperativasDto
    {
        public double HorasTotales { get; set; }

        public double HorasOperativasClientes { get; set; }

        public double EvaluacionOperativasClientes { get; set; }

        public List<TimeAnalyticsBackOfficeDistribucionDto> Distribucion { get; set; } = new();

        public List<TimeAnalyticsBackOfficeSerieDto> Areas { get; set; } = new();

        public List<TimeAnalyticsBackOfficeSerieDto> Colaboradores { get; set; } = new();

        public List<TimeAnalyticsOperativasResumenDto> Resumen { get; set; } = new();

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }

    public sealed class TimeAnalyticsHorasExtrasColaboradorDto
    {
        public string Nombre { get; set; } = "";

        public string Area { get; set; } = "";

        public double Horas { get; set; }

        public double HorasExtras { get; set; }

        public double HorasFaltantes { get; set; }

        public string Clasificacion { get; set; } = "";

        public string ClaseCss { get; set; } = "";
    }

    public sealed class TimeAnalyticsHorasExtrasClasificacionDto
    {
        public string Clasificacion { get; set; } = "";

        public int Personas { get; set; }
    }

    public sealed class TimeAnalyticsHorasExtrasAreaDto
    {
        public string Area { get; set; } = "";

        public string Tipo { get; set; } = "";

        public double HorasExtras { get; set; }
    }

    public sealed class TimeAnalyticsHorasExtrasDto
    {
        public double Horas { get; set; }

        public double HorasExtras { get; set; }

        public double HorasTotales { get; set; }

        public double HorasFaltantes { get; set; }

        public double PromedioPersona { get; set; }

        public List<TimeAnalyticsHorasExtrasColaboradorDto> Colaboradores { get; set; } = new();

        public List<TimeAnalyticsHorasExtrasClasificacionDto> Clasificaciones { get; set; } = new();

        public List<TimeAnalyticsHorasExtrasAreaDto> AreasTipo { get; set; } = new();

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }

    public sealed class TimeAnalyticsDashboardDto
    {
        public TimeAnalyticsKpisDto Kpis { get; set; } = new();

        public List<TimeAnalyticsTipoDto> DistribucionTipo { get; set; } = new();

        public List<TimeAnalyticsTendenciaDto> Tendencia { get; set; } = new();

        public List<TimeAnalyticsAreaDto> Areas { get; set; } = new();

        public List<TimeAnalyticsColaboradorDto> Colaboradores { get; set; } = new();

        public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    }
}
