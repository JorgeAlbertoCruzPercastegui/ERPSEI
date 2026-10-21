
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
            int limit = 2,
            CancellationToken cancellationToken = default)
        {
            // Validar que la tabla esté autorizada.
            if (!TablasPermitidas.Contains(tabla))
            {
                throw new ArgumentException(
                    "La tabla solicitada no está autorizada.",
                    nameof(tabla));
            }

            // Validar parámetros de paginación.
            if (afterId < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(afterId),
                    "El cursor no puede ser negativo.");
            }

            if (limit < 1 || limit > 500)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(limit),
                    "El límite debe estar entre 1 y 500.");
            }

            // Validar la configuración del Bearer Token.
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new InvalidOperationException(
                    "No se ha configurado TimeApi:ApiKey.");
            }

            // Construir la ruta solicitada.
            string ruta =
                $"api/v1/time/tablas/{tabla}" +
                $"?after_id={afterId}&limit={limit}";

            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                ruta);

            // Autenticación Bearer Token.
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    _options.ApiKey);

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/json"));

            // Enviar solicitud HTTPS.
            using HttpResponseMessage response =
                await _httpClient.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

            // Si TIME devuelve 401, 403, 500, etc.,
            // se genera una excepción HTTP.
            response.EnsureSuccessStatusCode();

            // Leer el JSON original de TIME.
            await using Stream stream =
                await response.Content.ReadAsStreamAsync(
                    cancellationToken);

            return await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);
        }
    }
}