namespace ERPSEI.Services.TimeIntegration
{
    public interface ITimeAnalyticsService
    {
        Task<TimeAnalyticsCatalogoDto> ObtenerCatalogosAsync(
            CancellationToken cancellationToken = default);

        Task<TimeAnalyticsDashboardDto> ObtenerDashboardAsync(
            TimeAnalyticsFiltroDto filtro,
            CancellationToken cancellationToken = default);
    }
}