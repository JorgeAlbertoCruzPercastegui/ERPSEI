using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ERPSEI.Services.BioTimeIntegration;

public sealed record BioTimeEmpleadoDto(string Codigo, string? Nombre, string? Apellidos, string? Departamento, string? Puesto);
public sealed record BioTimeChecadaDto(long Id, string Codigo, DateTime FechaHora, string? Estado, int? Verificacion, string? Dispositivo);

public sealed class BioTimeApiClient
{
    private readonly HttpClient _http;
    private readonly BioTimeOptions _options;
    public BioTimeApiClient(HttpClient http, IOptions<BioTimeOptions> options)
    {
        _http = http;
        _options = options.Value;
        if (!Uri.TryCreate(_options.BaseUrl, UriKind.Absolute, out var url) || url.Scheme != Uri.UriSchemeHttps)
            throw new InvalidOperationException("BioTime: BaseUrl debe ser HTTPS.");
        _http.BaseAddress = new Uri(url.ToString().TrimEnd('/') + "/");
        _http.Timeout = TimeSpan.FromSeconds(60);
    }


    private async Task<string> ObtenerTokenAsync(
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.Email) ||
            string.IsNullOrWhiteSpace(_options.Password) ||
            string.IsNullOrWhiteSpace(_options.Company))
        {
            throw new InvalidOperationException(
                "BioTime: credenciales incompletas.");
        }

        using var response = await _http.PostAsJsonAsync(
            "api-token-auth/",
            new
            {
                email = _options.Email,
                password = _options.Password,
                company = _options.Company
            },
            ct);

        if (!response.IsSuccessStatusCode)
        {
            // No se registran credenciales, tokens ni
            // contenido sensible de la respuesta.
            throw new HttpRequestException(
                $"BioTime AUTH /api-token-auth/: " +
                $"HTTP {(int)response.StatusCode} " +
                $"({response.ReasonPhrase}). " +
                "Revisar credenciales, compañía y formato de autenticación.",
                null,
                response.StatusCode);
        }

        using var json = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(ct),
            cancellationToken: ct);

        if (!json.RootElement.TryGetProperty(
                "token", out var token) ||
            string.IsNullOrWhiteSpace(token.GetString()))
        {
            throw new InvalidOperationException(
                "BioTime AUTH: la respuesta no contiene un token válido.");
        }

        return token.GetString()!;
    }


    private async Task<JsonDocument> GetAsync(
        string path,
        string token,
        CancellationToken ct)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            path);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Token", token);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json"));

        using var response = await _http.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            ct);

        if (!response.IsSuccessStatusCode)
        {
            // Solo se registra el nombre del endpoint,
            // sin parámetros ni datos de autenticación.
            string endpoint =
                path.StartsWith(
                    "personnel/api/employees/",
                    StringComparison.OrdinalIgnoreCase)
                    ? "EMPLOYEES"
                    : path.StartsWith(
                        "iclock/api/transactions/",
                        StringComparison.OrdinalIgnoreCase)
                        ? "TRANSACTIONS"
                        : "OTHER";

            throw new HttpRequestException(
                $"BioTime GET {endpoint}: " +
                $"HTTP {(int)response.StatusCode} " +
                $"({response.ReasonPhrase}).",
                null,
                response.StatusCode);
        }

        var doc = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(ct),
            cancellationToken: ct);

        if (doc.RootElement.TryGetProperty(
                "code", out var code) &&
            code.ValueKind == JsonValueKind.Number &&
            code.GetInt32() != 0)
        {
            doc.Dispose();

            throw new InvalidOperationException(
                "BioTime devolvió un código de error en el cuerpo JSON.");
        }

        return doc;
    }

    private static string? String(JsonElement obj, string key)
    {
        if (!obj.TryGetProperty(key, out var value) || value.ValueKind == JsonValueKind.Null) return null;
        return value.ToString();
    }

    public async Task<List<BioTimeEmpleadoDto>> ObtenerEmpleadosAsync(CancellationToken ct)
    {
        var token = await ObtenerTokenAsync(ct);
        var datos = new List<BioTimeEmpleadoDto>();
        for (int page = 1; page <= 1000; page++)
        {
            using var doc = await GetAsync($"personnel/api/employees/?page_size=100&page={page}", token, ct);
            foreach (var e in doc.RootElement.GetProperty("data").EnumerateArray())
            {
                var codigo = String(e, "emp_code");
                if (string.IsNullOrWhiteSpace(codigo)) continue;
                string? dept = null; string? puesto = null;
                if (e.TryGetProperty("department", out var d) && d.ValueKind == JsonValueKind.Object) dept = String(d, "dept_name");
                if (e.TryGetProperty("position", out var p) && p.ValueKind == JsonValueKind.Object) puesto = String(p, "position_name");
                datos.Add(new BioTimeEmpleadoDto(codigo, String(e, "first_name"), String(e, "last_name"), dept, puesto));
            }
            if (!doc.RootElement.TryGetProperty("next", out var next) || next.ValueKind == JsonValueKind.Null || string.IsNullOrEmpty(next.ToString())) return datos;
        }
        throw new InvalidOperationException("BioTime: límite de paginación de empleados alcanzado.");
    }

    public async Task<List<BioTimeChecadaDto>> ObtenerChecadasAsync(DateTime inicio, DateTime fin, CancellationToken ct)
    {
        var token = await ObtenerTokenAsync(ct);
        var datos = new List<BioTimeChecadaDto>();
        var desde = Uri.EscapeDataString(inicio.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        var hasta = Uri.EscapeDataString(fin.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        for (int page = 1; page <= 10000; page++)
        {
            using var doc = await GetAsync($"iclock/api/transactions/?start_time={desde}&end_time={hasta}&page_size=100&page={page}", token, ct);
            foreach (var e in doc.RootElement.GetProperty("data").EnumerateArray())
            {
                var codigo = String(e, "emp_code");
                if (!long.TryParse(String(e, "id"), out var id) || string.IsNullOrWhiteSpace(codigo)) continue;
                if (!DateTime.TryParseExact(String(e, "punch_time"), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha)) continue;
                // Validación local porque algunos servidores ignoran ciertos filtros.
                if (fecha < inicio || fecha > fin) continue;
                int? verif = int.TryParse(String(e, "verify_type"), out var num) ? num : null;
                datos.Add(new BioTimeChecadaDto(id, codigo, fecha, String(e, "punch_state"), verif, String(e, "terminal_sn")));
            }
            if (!doc.RootElement.TryGetProperty("next", out var next) || next.ValueKind == JsonValueKind.Null || string.IsNullOrEmpty(next.ToString())) return datos;
        }
        throw new InvalidOperationException("BioTime: límite de paginación de checadas alcanzado.");
    }
}
