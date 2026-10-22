
namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeReporteHorasFiltro
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public long? UsuarioId { get; set; }
        public long? ProyectoId { get; set; }
        public long? DepartamentoId { get; set; }
        public int Pagina { get; set; } = 1;
        public int TamanoPagina { get; set; } = 25;
    }


    public sealed class TimeReporteHorasFila
    {
        public long Id { get; set; }

        // Columnas del reporte detallado TIME
        public string? Nombre { get; set; }
        public string? Area { get; set; }
        public string? Puesto { get; set; }
        public string? NivelCliente { get; set; }
        public string? GrupoEmpresa { get; set; }
        public string? InternaExterna { get; set; }
        public string? Tipo { get; set; }

        public DateTime? Fecha { get; set; }
        public string? Actividad { get; set; }
        public string? Descripcion { get; set; }
        public double? Horas { get; set; }

        // Propiedades de compatibilidad
        public string? Usuario { get; set; }
        public string? Proyecto { get; set; }
        public string? Departamento { get; set; }
    }


    public sealed class TimeReporteHorasResultado
    {
        public long TotalRegistros { get; set; }
        public int Pagina { get; set; }
        public int TamanoPagina { get; set; }
        public int TotalPaginas { get; set; }

        public List<TimeReporteHorasFila> Registros { get; set; }
            = new();
    }

    public interface ITimeSqlReportService
    {
        Task<TimeReporteHorasResultado> ConsultarHorasAsync(
            TimeReporteHorasFiltro filtro,
            CancellationToken cancellationToken = default);
    }
}
