namespace ERPSEI.Services.TimeIntegration
{
    public interface ITimeAnalyticsService
    {
        Task<TimeAnalyticsCatalogoDto> ObtenerCatalogosAsync(
            long? departamentoId = null,
            CancellationToken cancellationToken = default);

        Task<TimeAnalyticsDashboardDto> ObtenerDashboardAsync(
            TimeAnalyticsFiltroDto filtro,
            CancellationToken cancellationToken = default);

        Task<TimeAnalyticsEvaluacionAreasDto> ObtenerEvaluacionAreasAsync(
            TimeAnalyticsFiltroDto filtro,
            CancellationToken cancellationToken = default);

        Task<TimeAnalyticsAnalisisPersonalDto> ObtenerAnalisisPersonalAsync(
            TimeAnalyticsFiltroDto filtro,
            CancellationToken cancellationToken = default);

        Task<TimeAnalyticsBackOfficeDto> ObtenerAlineacionBackOfficeAsync(
            TimeAnalyticsFiltroDto filtro,
            CancellationToken cancellationToken = default);

        Task<TimeAnalyticsOperativasDto> ObtenerAlineacionOperativasAsync(
            TimeAnalyticsFiltroDto filtro,
            CancellationToken cancellationToken = default);

        Task<TimeAnalyticsHorasExtrasDto> ObtenerHorasExtrasAsync(
            TimeAnalyticsFiltroDto filtro,
            CancellationToken cancellationToken = default);
    }
}