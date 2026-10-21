using System.Text.Json;

namespace ERPSEI.Services.TimeIntegration
{
    public class TimeTableReader : ITimeTableReader
    {
        private readonly ITimeApiClient _timeApiClient;

        public TimeTableReader(
            ITimeApiClient timeApiClient)
        {
            _timeApiClient = timeApiClient;
        }

        public async Task<List<JsonElement>> ObtenerTablaCompletaAsync(
            string tabla,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            var registros =
                new List<JsonElement>();

            await ProcesarTablaPorLotesAsync(
                tabla,
                lote =>
                {
                    registros.AddRange(lote);

                    return Task.CompletedTask;
                },
                limit,
                cancellationToken);

            return registros;
        }

        public async Task ProcesarTablaPorLotesAsync(
            string tabla,
            Func<IReadOnlyList<JsonElement>, Task> procesarLote,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tabla))
            {
                throw new ArgumentException(
                    "La tabla es obligatoria.",
                    nameof(tabla));
            }

            if (procesarLote == null)
            {
                throw new ArgumentNullException(
                    nameof(procesarLote));
            }

            if (limit < 1 ||
                limit > 500)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(limit),
                    "El límite debe estar entre 1 y 500.");
            }

            long afterId = 0;
            long? upperId = null;

            bool hasMore = true;

            while (hasMore)
            {
                cancellationToken
                    .ThrowIfCancellationRequested();

                using var documento =
                    await _timeApiClient.ObtenerTablaAsync(
                        tabla: tabla,
                        afterId: afterId,
                        upperId: upperId,
                        limit: limit,
                        cancellationToken: cancellationToken);

                var root =
                    documento.RootElement;

                if (!root.TryGetProperty(
                        "success",
                        out var successElement) ||
                    !successElement.GetBoolean())
                {
                    throw new InvalidOperationException(
                        $"TIME API indicó error al consultar la tabla '{tabla}'.");
                }

                if (!root.TryGetProperty(
                        "sync",
                        out var syncElement))
                {
                    throw new InvalidOperationException(
                        "TIME API no devolvió información de sincronización.");
                }

                if (!upperId.HasValue)
                {
                    upperId =
                        syncElement
                            .GetProperty("upper_id")
                            .GetInt64();
                }

                var lote =
                    new List<JsonElement>();

                if (root.TryGetProperty(
                        "data",
                        out var dataElement) &&
                    dataElement.ValueKind ==
                    JsonValueKind.Array)
                {
                    foreach (
                        var registro
                        in dataElement.EnumerateArray())
                    {
                        lote.Add(
                            registro.Clone());
                    }
                }

                if (lote.Count > 0)
                {
                    await procesarLote(lote);
                }

                hasMore =
                    syncElement
                        .GetProperty("has_more")
                        .GetBoolean();

                if (!hasMore)
                    break;

                afterId =
                    syncElement
                        .GetProperty("next_after_id")
                        .GetInt64();
            }
        }
    }
}