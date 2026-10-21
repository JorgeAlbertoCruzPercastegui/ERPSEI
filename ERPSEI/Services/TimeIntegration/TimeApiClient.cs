using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeApiClient : ITimeApiClient
    {
        private static readonly HashSet<string> TablasPermitidas =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "actividades",
                "claves",
                "configuraciones",
                "departamento-actividades",
                "departamentos",
                "empresas",
                "eventos",
                "grupos",
                "horas",
                "ingresos",
                "periodos",
                "roles",
                "sueldos",
                "usuarios"
            };

        private readonly HttpClient _httpClient;
        private readonly TimeApiOptions _options;

        public TimeApiClient(
            HttpClient httpClient,
            IOptions<TimeApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<JsonDocument> ObtenerTablaAsync(
            string tabla,
            long afterId = 0,
            long? upperId = null,
            int limit = 500,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(tabla))
            {
                throw new ArgumentException(
                    "La tabla es obligatoria.",
                    nameof(tabla));
            }

            tabla = tabla
                .Trim()
                .ToLowerInvariant();

            if (!TablasPermitidas.Contains(tabla))
            {
                throw new ArgumentException(
                    $"La tabla '{tabla}' no está permitida.",
                    nameof(tabla));
            }

            if (afterId < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(afterId),
                    "afterId no puede ser negativo.");
            }

            if (upperId.HasValue &&
                upperId.Value < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(upperId),
                    "upperId no puede ser negativo.");
            }

            if (limit < 1 ||
                limit > 500)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(limit),
                    "El límite debe estar entre 1 y 500.");
            }

            if (string.IsNullOrWhiteSpace(
                _options.ApiKey))
            {
                throw new InvalidOperationException(
                    "No se ha configurado la ApiKey de TIME.");
            }

            string url =
                $"api/v1/time/tablas/{Uri.EscapeDataString(tabla)}" +
                $"?after_id={afterId}" +
                $"&limit={limit}";

            if (upperId.HasValue)
            {
                url +=
                    $"&upper_id={upperId.Value}";
            }

            using var request =
                new HttpRequestMessage(
                    HttpMethod.Get,
                    url);

            request.Headers.Accept.Clear();

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/json"));

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _options.ApiKey);

            using var response =
                await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var stream =
                await response.Content.ReadAsStreamAsync(
                    cancellationToken);

            return await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);
        }
    }
}