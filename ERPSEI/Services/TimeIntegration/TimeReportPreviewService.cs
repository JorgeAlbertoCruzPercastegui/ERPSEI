
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;

namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeReportPreviewService
        : ITimeReportPreviewService
    {
        private readonly ITimeTableReader _reader;
        private readonly ITimeReportCatalogService _catalogos;
        private readonly IMemoryCache _cache;

        private const int TamanoLote = 500;

        private sealed record ActividadInfo(
            long UsuarioId,
            long ClaveId,
            string Nombre);

        public TimeReportPreviewService(
            ITimeTableReader reader,
            ITimeReportCatalogService catalogos,
            IMemoryCache cache)
        {
            _reader = reader;
            _catalogos = catalogos;
            _cache = cache;
        }

        public async Task<TimeReportPreviewDto> ObtenerHorasAsync(
            string? fechaInicio,
            string? fechaFin,
            long? usuarioId,
            long? proyectoId,
            long? departamentoId,
            CancellationToken cancellationToken = default)
        {
            DateOnly? desde = ParseFecha(fechaInicio);
            DateOnly? hasta = ParseFecha(fechaFin);

            if (desde.HasValue && hasta.HasValue &&
                desde.Value > hasta.Value)
            {
                throw new ArgumentException(
                    "La fecha inicial no puede ser mayor que la final.");
            }

            // Obtener todas las actividades y sus relaciones.
            var actividades = await ObtenerActividadesAsync(
                cancellationToken);

            // Recuperar los nombres originales.
            var catalogos = await _catalogos.ObtenerCatalogosAsync(
                cancellationToken);

            var usuarios = catalogos.Usuarios
                .ToDictionary(x => x.Id, x => x.Nombre);

            var proyectos = catalogos.Proyectos
                .ToDictionary(x => x.Id, x => x.Nombre);

            // El departamento corresponde al usuario de TIME.
            var departamentosUsuario = departamentoId.HasValue
                ? await ObtenerDepartamentosUsuarioAsync(
                    cancellationToken)
                : new Dictionary<long, long>();

            var reporte = new TimeReportPreviewDto
            {
                Alcance = "Histórico completo de TIME"
            };

            // Recorrer TODAS las horas, lote por lote.
            await _reader.ProcesarTablaPorLotesAsync(
                tabla: "horas",
                limit: TamanoLote,
                cancellationToken: cancellationToken,
                procesarLote: lote =>
                {
                    foreach (JsonElement hora in lote)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        reporte.RegistrosExaminados++;

                        // -------------------------------
                        // FILTRO DE FECHAS
                        // -------------------------------

                        string? fechaTexto =
                            LeerTexto(hora, "fecha");

                        if (!DateOnly.TryParseExact(
                            fechaTexto,
                            "yyyy-MM-dd",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateOnly fecha))
                        {
                            if (desde.HasValue || hasta.HasValue)
                                continue;
                        }
                        else
                        {
                            if (desde.HasValue && fecha < desde.Value)
                                continue;

                            if (hasta.HasValue && fecha > hasta.Value)
                                continue;
                        }

                        // -------------------------------
                        // RELACIÓN CON ACTIVIDADES
                        // -------------------------------

                        long actividadId =
                            LeerLong(hora, "actividad_id");

                        actividades.TryGetValue(
                            actividadId,
                            out ActividadInfo? actividad);

                        long idUsuario = actividad?.UsuarioId ?? 0;
                        long idClave = actividad?.ClaveId ?? 0;

                        // -------------------------------
                        // FILTRO DE USUARIO
                        // -------------------------------

                        if (usuarioId.HasValue &&
                            idUsuario != usuarioId.Value)
                        {
                            continue;
                        }

                        // -------------------------------
                        // FILTRO DE PROYECTO
                        // -------------------------------

                        if (proyectoId.HasValue &&
                            idClave != proyectoId.Value)
                        {
                            continue;
                        }

                        // -------------------------------
                        // FILTRO DE DEPARTAMENTO
                        // -------------------------------

                        departamentosUsuario.TryGetValue(
                            idUsuario,
                            out long idDepartamento);

                        if (departamentoId.HasValue &&
                            idDepartamento != departamentoId.Value)
                        {
                            continue;
                        }

                        // -------------------------------
                        // NOMBRES RELACIONADOS
                        // -------------------------------

                        usuarios.TryGetValue(
                            idUsuario,
                            out string? nombreUsuario);

                        proyectos.TryGetValue(
                            idClave,
                            out string? nombreProyecto);

                        // -------------------------------
                        // RESULTADO DEL REPORTE
                        // -------------------------------

                        reporte.Registros.Add(
                            new TimeHoraReporteDto
                            {
                                Id = LeerLong(hora, "id"),
                                Fecha = fechaTexto,
                                Horas = LeerDouble(hora, "horas"),
                                ActividadId = actividadId,
                                Actividad = actividad?.Nombre
                                    ?? "Actividad no encontrada",
                                UsuarioId = idUsuario,
                                Usuario = nombreUsuario
                                    ?? "Usuario no encontrado",
                                ClaveId = idClave,
                                Proyecto = nombreProyecto
                                    ?? "Proyecto no encontrado",
                                DepartamentoId = idDepartamento > 0
                                    ? idDepartamento
                                    : null
                            });
                    }

                    return Task.CompletedTask;
                });

            reporte.RegistrosCoincidentes =
                reporte.Registros.Count;

            return reporte;
        }

        // =====================================================
        // CATÁLOGO COMPLETO DE ACTIVIDADES
        // =====================================================

        private Task<Dictionary<long, ActividadInfo>>
            ObtenerActividadesAsync(CancellationToken ct)
        {
            return ObtenerDiccionarioAsync(
                "TIME_REPORT_ACTIVIDADES_COMPLETO_V1",
                "actividades",
                registro => new ActividadInfo(
                    LeerLong(registro, "usuario_id"),
                    LeerLong(registro, "clave_id"),
                    LeerTexto(registro, "actividad")
                        ?? "Sin descripción"),
                ct);
        }

        // =====================================================
        // RELACIÓN COMPLETA USUARIO -> DEPARTAMENTO
        // =====================================================

        private Task<Dictionary<long, long>>
            ObtenerDepartamentosUsuarioAsync(
                CancellationToken ct)
        {
            return ObtenerDiccionarioAsync(
                "TIME_REPORT_USUARIOS_DEPTO_COMPLETO_V1",
                "usuarios",
                registro => LeerLong(
                    registro, "departamento_id"),
                ct);
        }

        // =====================================================
        // RECORRER UN CATÁLOGO SIN LÍMITE DE REGISTROS
        // =====================================================

        private async Task<Dictionary<long, T>>
            ObtenerDiccionarioAsync<T>(
                string cacheKey,
                string tabla,
                Func<JsonElement, T> convertir,
                CancellationToken ct)
        {
            if (_cache.TryGetValue(
                cacheKey,
                out Dictionary<long, T>? existente)
                && existente != null)
            {
                return existente;
            }

            var diccionario = new Dictionary<long, T>();

            await _reader.ProcesarTablaPorLotesAsync(
                tabla: tabla,
                limit: TamanoLote,
                cancellationToken: ct,
                procesarLote: lote =>
                {
                    foreach (JsonElement registro in lote)
                    {
                        ct.ThrowIfCancellationRequested();

                        long id = LeerLong(registro, "id");

                        if (id > 0)
                        {
                            diccionario[id] = convertir(registro);
                        }
                    }

                    return Task.CompletedTask;
                });

            _cache.Set(
                cacheKey,
                diccionario,
                TimeSpan.FromMinutes(10));

            return diccionario;
        }

        // =====================================================
        // LECTURA DE VALORES JSON
        // =====================================================

        private static long LeerLong(
            JsonElement registro,
            string campo)
        {
            return registro.TryGetProperty(
                       campo, out JsonElement valor)
                   && valor.ValueKind == JsonValueKind.Number
                   && valor.TryGetInt64(out long numero)
                ? numero
                : 0;
        }

        private static double LeerDouble(
            JsonElement registro,
            string campo)
        {
            return registro.TryGetProperty(
                       campo, out JsonElement valor)
                   && valor.ValueKind == JsonValueKind.Number
                   && valor.TryGetDouble(out double numero)
                ? numero
                : 0;
        }

        private static string? LeerTexto(
            JsonElement registro,
            string campo)
        {
            return registro.TryGetProperty(
                       campo, out JsonElement valor)
                   && valor.ValueKind == JsonValueKind.String
                ? valor.GetString()
                : null;
        }

        private static DateOnly? ParseFecha(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return null;

            if (!DateOnly.TryParseExact(
                texto,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateOnly fecha))
            {
                throw new ArgumentException(
                    "El formato de fecha es inválido.");
            }

            return fecha;
        }
    }
}
