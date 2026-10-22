
namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeHoraReporteDto
    {
        public long Id { get; set; }
        public string? Fecha { get; set; }
        public double Horas { get; set; }
        public long ActividadId { get; set; }
        public string Actividad { get; set; } = "";
        public long UsuarioId { get; set; }
        public string Usuario { get; set; } = "";
        public long ClaveId { get; set; }
        public string Proyecto { get; set; } = "";
        public long? DepartamentoId { get; set; }
    }

    public sealed class TimeReportPreviewDto
    {
        public string Alcance { get; set; } = "Muestra limitada";
        public int RegistrosExaminados { get; set; }
        public int RegistrosCoincidentes { get; set; }
        public List<TimeHoraReporteDto> Registros { get; set; } = new();
    }

    public interface ITimeReportPreviewService
    {
        Task<TimeReportPreviewDto> ObtenerHorasAsync(
            string? fechaInicio,
            string? fechaFin,
            long? usuarioId,
            long? proyectoId,
            long? departamentoId,
            CancellationToken cancellationToken = default);
    }
}
