
using ERPSEI.Data;
using ERPSEI.Data.Entities.TimeIntegration;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeBajasDiagnostico
    {
        public long RegistrosLocales { get; set; }
        public long RegistrosOrigenLeidos { get; set; }
        public long UpperId { get; set; }
        public long PosiblesEliminaciones { get; set; }
        public List<long> PrimerosIdsFaltantes { get; set; } = new();
    }

    public interface ITimeDeletionAuditService
    {
        Task<TimeBajasDiagnostico> RevisarHorasAsync(
            CancellationToken cancellationToken = default);
    }

    public sealed class TimeDeletionAuditService
        : ITimeDeletionAuditService
    {
        private readonly ApplicationDbContext _db;
        private readonly ITimeApiClient _api;
        private readonly ILogger<TimeDeletionAuditService> _logger;

        public TimeDeletionAuditService(
            ApplicationDbContext db,
            ITimeApiClient api,
            ILogger<TimeDeletionAuditService> logger)
        {
            _db = db;
            _api = api;
            _logger = logger;
        }

        public async Task<TimeBajasDiagnostico> RevisarHorasAsync(
            CancellationToken cancellationToken = default)
        {
            const string tabla = "horas";
            const int limite = 500;

            long afterId = 0;
            long? upperId = null;
            long leidos = 0;
            bool hayMas = true;

            // Primero conocemos el límite superior del origen.
            // No interpretamos una respuesta vacía como eliminación
            // hasta confirmar que la paginación terminó.
            var idsOrigen = new HashSet<long>();

            while (hayMas)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using JsonDocument respuesta =
                    await _api.ObtenerTablaAsync(
                        tabla: tabla,
                        afterId: afterId,
                        limit: limite,
                        upperId: upperId,
                        cancellationToken: cancellationToken);

                var raiz = respuesta.RootElement;

                if (!raiz.GetProperty("success").GetBoolean())
                    throw new InvalidOperationException(
                        "TIME no confirmó la lectura de Horas.");

                var sync = raiz.GetProperty("sync");
                var datos = raiz.GetProperty("data");

                if (datos.ValueKind != JsonValueKind.Array)
                    throw new InvalidOperationException(
                        "TIME devolvió datos inválidos.");

                long upperRespuesta =
                    sync.GetProperty("upper_id").GetInt64();

                if (!upperId.HasValue)
                    upperId = upperRespuesta;
                else if (upperRespuesta != upperId.Value)
                    throw new InvalidOperationException(
                        "Cambió el límite superior de la consulta.");

                hayMas = sync.GetProperty("has_more").GetBoolean();

                long siguienteId =
                    sync.GetProperty("next_after_id").GetInt64();

                int cantidad = 0;
                long ultimoRegistro = afterId;

                foreach (var registro in datos.EnumerateArray())
                {
                    long id = registro.GetProperty("id").GetInt64();

                    if (id <= ultimoRegistro ||
                        id > upperId.Value ||
                        !idsOrigen.Add(id))
                    {
                        throw new InvalidOperationException(
                            "TIME devolvió IDs fuera de orden, " +
                            "duplicados o fuera del límite.");
                    }

                    ultimoRegistro = id;
                    cantidad++;
                }

                if (cantidad > 0 && siguienteId != ultimoRegistro)
                    throw new InvalidOperationException(
                        "El cursor de TIME no coincide con el lote.");

                if (cantidad == 0)
                {
                    if (hayMas)
                        throw new InvalidOperationException(
                            "TIME devolvió un lote vacío " +
                            "con páginas pendientes.");

                    if (siguienteId != afterId)
                        throw new InvalidOperationException(
                            "TIME devolvió un cursor vacío inesperado.");
                }
                else if (siguienteId <= afterId)
                {
                    throw new InvalidOperationException(
                        "TIME devolvió un cursor sin avance.");
                }

                leidos += cantidad;
                afterId = siguienteId;
            }

            if (!upperId.HasValue)
                throw new InvalidOperationException(
                    "TIME no proporcionó el límite superior.");

            // Comparar solamente registros locales dentro
            // de la ventana que acabamos de revisar.
            var idsLocales = await _db.TimeHoras
                .AsNoTracking()
                .Where(x => x.Id <= upperId.Value)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            var faltantes = idsLocales
                .Where(id => !idsOrigen.Contains(id))
                .OrderBy(id => id)
                .ToList();


            // =====================================================
            // REGISTRAR HALLAZGOS EN SQL SERVER
            // =====================================================

            const string tipoHallazgo = "PosibleEliminacion";
            const string tablaOrigen = "horas";

            DateTime fechaRevision = DateTime.UtcNow;

            var idsFaltantes = faltantes.ToHashSet();

            // Recuperar hallazgos anteriores de esta tabla.
            var hallazgos = await _db.TimeHallazgosConciliacion
                .Where(x =>
                    x.TablaOrigen == tablaOrigen &&
                    x.TipoHallazgo == tipoHallazgo)
                .ToListAsync(cancellationToken);

            var hallazgosPorId = hallazgos
                .ToDictionary(x => x.RegistroOrigenId);

            int nuevos = 0;
            int persistentes = 0;
            int recuperados = 0;

            // Registrar las ausencias detectadas.
            foreach (long id in faltantes)
            {
                if (!hallazgosPorId.TryGetValue(id, out var hallazgo))
                {
                    hallazgo = new TimeHallazgoConciliacion
                    {
                        TablaOrigen = tablaOrigen,
                        RegistroOrigenId = id,
                        TipoHallazgo = tipoHallazgo,
                        Estado = "Pendiente",
                        PrimeraDeteccion = fechaRevision,
                        UltimaRevision = fechaRevision,
                        NumeroDetecciones = 1,
                        Observaciones =
                            "Registro local no encontrado en TIME " +
                            "durante una revisión completa."
                    };

                    _db.TimeHallazgosConciliacion.Add(hallazgo);
                    nuevos++;
                }
                else
                {
                    // Solo incrementar una vez por revisión.
                    hallazgo.UltimaRevision = fechaRevision;

                    if (hallazgo.Estado == "Recuperado")
                    {
                        // Se conserva el historial y se reabre.
                        hallazgo.Estado = "Pendiente";
                        hallazgo.FechaRecuperacion = null;
                        hallazgo.FechaConfirmacion = null;
                        hallazgo.NumeroDetecciones = 1;
                    }
                    else
                    {
                        hallazgo.NumeroDetecciones++;
                    }

                    persistentes++;
                }
            }

            // Detectar registros que reaparecieron en TIME.
            // Solamente se revisan IDs dentro de la ventana
            // completa evaluada.
            foreach (var hallazgo in hallazgos)
            {
                if (hallazgo.Estado == "Recuperado")
                    continue;

                if (hallazgo.RegistroOrigenId > upperId.Value)
                    continue;

                if (idsFaltantes.Contains(hallazgo.RegistroOrigenId))
                    continue;

                if (!idsOrigen.Contains(hallazgo.RegistroOrigenId))
                    continue;

                hallazgo.Estado = "Recuperado";
                hallazgo.FechaRecuperacion = fechaRevision;
                hallazgo.UltimaRevision = fechaRevision;

                recuperados++;
            }

            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "TIME auditoría Horas: nuevos={Nuevos}, " +
                "persistentes={Persistentes}, recuperados={Recuperados}.",
                nuevos,
                persistentes,
                recuperados);


            _logger.LogInformation(
                "TIME diagnóstico Horas: origen={Origen}, " +
                "local={Local}, posibles bajas={Bajas}.",
                leidos,
                idsLocales.Count,
                faltantes.Count);

            return new TimeBajasDiagnostico
            {
                RegistrosLocales = idsLocales.Count,
                RegistrosOrigenLeidos = leidos,
                UpperId = upperId.Value,
                PosiblesEliminaciones = faltantes.Count,
                PrimerosIdsFaltantes = faltantes.Take(50).ToList()
            };
        }
    }
}
