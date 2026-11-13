using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeReportCatalogService
        : ITimeReportCatalogService
    {
        private readonly ITimeTableReader _reader;
        private readonly IMemoryCache _cache;

        private const string CacheKey = "TIME_REPORT_CATALOGOS_V2";
        private const int MaxRegistrosPorCatalogo = 20000;

        public TimeReportCatalogService(
            ITimeTableReader reader,
            IMemoryCache cache)
        {
            _reader = reader;
            _cache = cache;
        }

        public async Task<TimeReportCatalogosDto> ObtenerCatalogosAsync(
            CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(
                CacheKey,
                out TimeReportCatalogosDto? existente) &&
                existente != null)
            {
                return existente;
            }

            var catalogos = new TimeReportCatalogosDto
            {
                Usuarios = await LeerOpcionesAsync(
                    tabla: "usuarios",
                    campoNombre: "nombre",
                    soloActivos: true,
                    cancellationToken: cancellationToken),

                Proyectos = await LeerOpcionesAsync(
                    tabla: "claves",
                    campoNombre: "proyecto",
                    soloActivos: false,
                    cancellationToken: cancellationToken),

                Departamentos = await LeerOpcionesAsync(
                    tabla: "departamentos",
                    campoNombre: "nombre",
                    soloActivos: false,
                    cancellationToken: cancellationToken)
            };

            _cache.Set(
                CacheKey,
                catalogos,
                TimeSpan.FromMinutes(10));

            return catalogos;
        }

        private async Task<List<TimeCatalogOptionDto>>
            LeerOpcionesAsync(
                string tabla,
                string campoNombre,
                bool soloActivos,
                CancellationToken cancellationToken)
        {
            var opciones =
                new Dictionary<long, TimeCatalogOptionDto>();

            int totalProcesado = 0;

            await _reader.ProcesarTablaPorLotesAsync(
                tabla: tabla,
                limit: 500,
                cancellationToken: cancellationToken,
                procesarLote: lote =>
                {
                    totalProcesado += lote.Count;

                    if (totalProcesado > MaxRegistrosPorCatalogo)
                    {
                        throw new InvalidOperationException(
                            $"El catálogo {tabla} excede el límite " +
                            $"de {MaxRegistrosPorCatalogo} registros.");
                    }

                    foreach (JsonElement registro in lote)
                    {
                        // =========================================
                        // FILTRO DE REGISTROS ACTIVOS
                        // =========================================

                        if (soloActivos)
                        {
                            if (!registro.TryGetProperty(
                                "estatus",
                                out JsonElement estatusJson))
                            {
                                continue;
                            }

                            if (estatusJson.ValueKind !=
                                JsonValueKind.Number)
                            {
                                continue;
                            }

                            if (!estatusJson.TryGetByte(
                                out byte estatus))
                            {
                                continue;
                            }

                            if (estatus != 1)
                            {
                                continue;
                            }
                        }

                        // =========================================
                        // ID
                        // =========================================

                        if (!registro.TryGetProperty(
                            "id",
                            out JsonElement idJson))
                        {
                            continue;
                        }

                        if (!idJson.TryGetInt64(
                            out long id))
                        {
                            continue;
                        }

                        // =========================================
                        // NOMBRE
                        // =========================================

                        if (!registro.TryGetProperty(
                            campoNombre,
                            out JsonElement nombreJson))
                        {
                            continue;
                        }

                        string? nombre =
                            nombreJson.ValueKind ==
                            JsonValueKind.String
                                ? nombreJson.GetString()
                                : null;

                        if (string.IsNullOrWhiteSpace(
                            nombre))
                        {
                            continue;
                        }

                        opciones[id] =
                            new TimeCatalogOptionDto
                            {
                                Id = id,
                                Nombre = nombre.Trim()
                            };
                    }

                    return Task.CompletedTask;
                });

            return opciones
                .Values
                .OrderBy(
                    x => x.Nombre,
                    StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x.Id)
                .ToList();
        }
    }
}