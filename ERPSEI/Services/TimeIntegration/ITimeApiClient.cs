using System.Text.Json;

namespace ERPSEI.Services.TimeIntegration
{
    public interface ITimeApiClient
    {
        Task<JsonDocument> ObtenerTablaAsync(
            string tabla,
            long afterId = 0,
            int limit = 2,
            CancellationToken cancellationToken = default
        );
    }
}