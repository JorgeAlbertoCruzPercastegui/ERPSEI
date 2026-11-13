namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeAnalyticsFiltroDto
    {
        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public long? UsuarioId { get; set; }

        public long? DepartamentoId { get; set; }
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