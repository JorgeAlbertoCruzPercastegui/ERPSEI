
namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeCatalogOptionDto
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = "";
    }

    public sealed class TimeReportCatalogosDto
    {
        public List<TimeCatalogOptionDto> Usuarios { get; set; } = new();
        public List<TimeCatalogOptionDto> Proyectos { get; set; } = new();
        public List<TimeCatalogOptionDto> Departamentos { get; set; } = new();
    }

    public interface ITimeReportCatalogService
    {
        Task<TimeReportCatalogosDto> ObtenerCatalogosAsync(
            CancellationToken cancellationToken = default);
    }
}
