using System.Text.Json;

namespace ERPSEI.Services.TimeIntegration
{
    public interface ITimeTableReader
    {
        Task<List<JsonElement>> ObtenerTablaCompletaAsync(
            string tabla,
            int limit = 500,
            CancellationToken cancellationToken = default);

        Task ProcesarTablaPorLotesAsync(
            string tabla,
            Func<IReadOnlyList<JsonElement>, Task> procesarLote,
            int limit = 500,
            CancellationToken cancellationToken = default);
    }
}