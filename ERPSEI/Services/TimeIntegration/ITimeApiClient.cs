using System.Text.Json;

namespace ERPSEI.Services.TimeIntegration
{
    public interface ITimeApiClient
    {
        Task<JsonDocument> ObtenerTablaAsync(
            string tabla,
            long afterId = 0,
            long? upperId = null,
            int limit = 500,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<JsonElement> LeerTodosAsync(
        string tabla,
        int tamanoLote = 500,
        CancellationToken cancellationToken = default);
    }
}