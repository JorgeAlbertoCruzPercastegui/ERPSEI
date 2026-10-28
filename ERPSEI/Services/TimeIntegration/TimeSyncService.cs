
using System.Globalization;
using System.Text.Json;
using ERPSEI.Data;
using ERPSEI.Data.Entities.TimeIntegration;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeSyncService : ITimeSyncService
    {
        private readonly ApplicationDbContext _db;
        private readonly ITimeApiClient _api;
        private readonly ILogger<TimeSyncService> _logger;

        private const int TamanoLote = 500;

        // Evita dos ejecuciones simultáneas en
        // una misma instancia de ERPSEI.
        private static readonly SemaphoreSlim SyncLock = new(1, 1);

        public TimeSyncService(
            ApplicationDbContext db,
            ITimeApiClient api,
            ILogger<TimeSyncService> logger)
        {
            _db = db;
            _api = api;
            _logger = logger;
        }

        // =====================================================
        // 1. DEPARTAMENTOS
        // =====================================================

        public Task<TimeSyncResultadoDto> SincronizarDepartamentosAsync(
            CancellationToken cancellationToken = default)
        {
            return SincronizarTablaAsync(
                tabla: "departamentos",
                obtenerId: x => x.Id,
                crear: id => new TimeDepartamento { Id = id },
                asignar: (entidad, json) =>
                {
                    entidad.Nombre = LeerTexto(json, "nombre");
                    entidad.Clave = LeerTexto(json, "clave");
                    entidad.Estatus = LeerByte(json, "estatus");
                    entidad.Incremental = LeerInt(json, "incremental");
                    entidad.CreatedAt = LeerFecha(json, "created_at");
                    entidad.UpdatedAt = LeerFecha(json, "updated_at");
                },
                cancellationToken: cancellationToken);
        }

        // =====================================================
        // 2. USUARIOS
        // =====================================================

        public Task<TimeSyncResultadoDto> SincronizarUsuariosAsync(
            CancellationToken cancellationToken = default)
        {
            return SincronizarTablaAsync(
                tabla: "usuarios",
                obtenerId: x => x.Id,
                crear: id => new TimeUsuario { Id = id },
                asignar: (entidad, json) =>
                {
                    entidad.Nombre = LeerTexto(json, "nombre");
                    entidad.Email = LeerTexto(json, "email");
                    entidad.DepartamentoId =
                        LeerLong(json, "departamento_id");
                    entidad.Estatus = LeerByte(json, "estatus");
                    entidad.CreatedAt = LeerFecha(json, "created_at");
                    entidad.UpdatedAt = LeerFecha(json, "updated_at");
                },
                cancellationToken: cancellationToken);
        }

        // =====================================================
        // 3. CLAVES / PROYECTOS
        // =====================================================

        public Task<TimeSyncResultadoDto> SincronizarClavesAsync(
            CancellationToken cancellationToken = default)
        {
            return SincronizarTablaAsync(
                tabla: "claves",
                obtenerId: x => x.Id,
                crear: id => new TimeClave { Id = id },
                asignar: (entidad, json) =>
                {
                    entidad.EmpresaId = LeerLong(json, "empresa_id");
                    entidad.DepartamentoId =
                        LeerLong(json, "departamento_id");
                    entidad.Proyecto = LeerTexto(json, "proyecto");
                    entidad.Clave = LeerTexto(json, "clave");
                    entidad.Incremental = LeerInt(json, "incremental");
                    entidad.Tipo = LeerByte(json, "tipo");
                    entidad.Estatus = LeerByte(json, "estatus");
                    entidad.CreatedAt = LeerFecha(json, "created_at");
                    entidad.UpdatedAt = LeerFecha(json, "updated_at");
                },
                cancellationToken: cancellationToken);
        }

        // =====================================================
        // 4. ACTIVIDADES
        // =====================================================

        public Task<TimeSyncResultadoDto> SincronizarActividadesAsync(
            CancellationToken cancellationToken = default)
        {
            return SincronizarTablaAsync(
                tabla: "actividades",
                obtenerId: x => x.Id,
                crear: id => new TimeActividad { Id = id },
                asignar: (entidad, json) =>
                {
                    entidad.PeriodoId = LeerLong(json, "periodo_id");
                    entidad.UsuarioId = LeerLong(json, "usuario_id");
                    entidad.ClaveId = LeerLong(json, "clave_id");
                    entidad.Actividad = LeerTexto(json, "actividad");
                    entidad.DepartamentoActividadId =
                        LeerLong(json, "departamento_actividad_id");
                    entidad.Orden = LeerInt(json, "orden");
                    entidad.CreatedAt = LeerFecha(json, "created_at");
                    entidad.UpdatedAt = LeerFecha(json, "updated_at");
                },
                cancellationToken: cancellationToken);
        }

        // =====================================================
        // 5. HORAS
        // =====================================================

        public Task<TimeSyncResultadoDto> SincronizarHorasAsync(
            CancellationToken cancellationToken = default)
        {
            return SincronizarTablaAsync(
                tabla: "horas",
                obtenerId: x => x.Id,
                crear: id => new TimeHora { Id = id },
                asignar: (entidad, json) =>
                {
                    entidad.Fecha = LeerFechaSolo(json, "fecha");
                    entidad.Horas = LeerDouble(json, "horas");
                    entidad.ActividadId = LeerLong(json, "actividad_id");
                    entidad.CreatedAt = LeerFecha(json, "created_at");
                    entidad.UpdatedAt = LeerFecha(json, "updated_at");
                },
                cancellationToken: cancellationToken);
        }


        // =====================================================
        // 6. EMPRESAS
        // =====================================================

        public Task<TimeSyncResultadoDto> SincronizarEmpresasAsync(
            CancellationToken cancellationToken = default)
        {
            return SincronizarTablaAsync(
                tabla: "empresas",
                obtenerId: x => x.Id,
                crear: id => new TimeEmpresa { Id = id },
                asignar: (entidad, json) =>
                {
                    entidad.Nombre = LeerTexto(json, "nombre");
                    entidad.Tipo = LeerByte(json, "tipo");
                    entidad.Estatus = LeerByte(json, "estatus");
                    entidad.Incremental = LeerInt(json, "incremental");
                    entidad.GrupoId = LeerLong(json, "grupo_id");
                    entidad.Clave = LeerTexto(json, "clave");
                    entidad.CreatedAt = LeerFecha(json, "created_at");
                    entidad.UpdatedAt = LeerFecha(json, "updated_at");
                },
                cancellationToken: cancellationToken);
        }

        // =====================================================
        // 7. GRUPOS
        // =====================================================

        public Task<TimeSyncResultadoDto> SincronizarGruposAsync(
            CancellationToken cancellationToken = default)
        {
            return SincronizarTablaAsync(
                tabla: "grupos",
                obtenerId: x => x.Id,
                crear: id => new TimeGrupo { Id = id },
                asignar: (entidad, json) =>
                {
                    entidad.Nombre = LeerTexto(json, "nombre");
                    entidad.Estatus = LeerByte(json, "estatus");
                    entidad.CreatedAt = LeerFecha(json, "created_at");
                    entidad.UpdatedAt = LeerFecha(json, "updated_at");
                },
                cancellationToken: cancellationToken);
        }


        // =====================================================
        // TIME - CONCILIACIÓN HISTÓRICA DE HORAS
        // =====================================================

        public Task<TimeSyncResultadoDto> ConciliarHorasAsync(
            CancellationToken cancellationToken = default)
        {
            return SincronizarTablaAsync(
                tabla: "horas",
                obtenerId: x => x.Id,
                crear: id => new TimeHora { Id = id },
                asignar: (entidad, json) =>
                {
                    entidad.Fecha = LeerFechaSolo(json, "fecha");
                    entidad.Horas = LeerDouble(json, "horas");
                    entidad.ActividadId = LeerLong(json, "actividad_id");
                    entidad.CreatedAt = LeerFecha(json, "created_at");
                    entidad.UpdatedAt = LeerFecha(json, "updated_at");
                },
                cancellationToken: cancellationToken,
                conciliacion: true);
        }

        // =====================================================
        // TIME - CONCILIACIÓN HISTÓRICA DE ACTIVIDADES
        // =====================================================

        public Task<TimeSyncResultadoDto> ConciliarActividadesAsync(
            CancellationToken cancellationToken = default)
        {
            return SincronizarTablaAsync(
                tabla: "actividades",
                obtenerId: x => x.Id,
                crear: id => new TimeActividad { Id = id },
                asignar: (entidad, json) =>
                {
                    entidad.PeriodoId = LeerLong(json, "periodo_id");
                    entidad.UsuarioId = LeerLong(json, "usuario_id");
                    entidad.ClaveId = LeerLong(json, "clave_id");
                    entidad.Actividad = LeerTexto(json, "actividad");
                    entidad.DepartamentoActividadId =
                        LeerLong(json, "departamento_actividad_id");
                    entidad.Orden = LeerInt(json, "orden");
                    entidad.CreatedAt = LeerFecha(json, "created_at");
                    entidad.UpdatedAt = LeerFecha(json, "updated_at");
                },
                cancellationToken: cancellationToken,
                conciliacion: true);
        }


        // =====================================================
        // MOTOR GENÉRICO DE SINCRONIZACIÓN
        // =====================================================

        private async Task<TimeSyncResultadoDto> SincronizarTablaAsync<T>(
        string tabla,
        Func<T, long> obtenerId,
        Func<long, T> crear,
        Action<T, JsonElement> asignar,
        CancellationToken cancellationToken,
        bool conciliacion = false)
        where T : class
        {
            bool bloqueoAdquirido =
                await SyncLock.WaitAsync(0, cancellationToken);

            if (!bloqueoAdquirido)
            {
                throw new InvalidOperationException(
                    "Ya existe una sincronización TIME en ejecución.");
            }

            try
            {
                return await EjecutarSincronizacionAsync(
                tabla,
                obtenerId,
                crear,
                asignar,
                cancellationToken,
                conciliacion);
            }
            finally
            {
                SyncLock.Release();
            }
        }

        private async Task<TimeSyncResultadoDto> EjecutarSincronizacionAsync<T>(
        string tabla,
        Func<T, long> obtenerId,
        Func<long, T> crear,
        Action<T, JsonElement> asignar,
        CancellationToken cancellationToken,
        bool conciliacion)
        where T : class
        {
            var resultado = new TimeSyncResultadoDto
            {
                Tabla = tabla,
                Estado = "En proceso"
            };

            string claveEstado = conciliacion
            ? $"conciliacion:{tabla}"
            : tabla;

            var estado = await _db.TimeEstadosSincronizacion
            .SingleOrDefaultAsync(
                x => x.Tabla == claveEstado,
                cancellationToken);

            if (estado == null)
            {
                estado = new TimeEstadoSincronizacion
                {
                    Tabla = claveEstado,
                    UltimoId = 0,
                    UltimoUpperId = null,
                    RegistrosProcesados = 0,
                    Estado = "Pendiente"
                };

                _db.TimeEstadosSincronizacion.Add(estado);
            }

            if (estado.Estado == "Completado")
            {
                if (conciliacion)
                {
                    estado.UltimoId = 0;
                    estado.RegistrosProcesados = 0;
                }

                estado.UltimoUpperId = null;
            }


            long afterId = estado.UltimoId;
            long? upperId = estado.UltimoUpperId;

            bool hasMore = true;

            _logger.LogInformation(
                "TIME: Iniciando {Tabla} desde cursor {Cursor}.",
                tabla,
                afterId);

            while (hasMore)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using JsonDocument respuesta =
                await _api.ObtenerTablaAsync(
                    tabla: tabla,
                    afterId: afterId,
                    limit: TamanoLote,
                    upperId: upperId,
                    cancellationToken: cancellationToken);

                JsonElement root = respuesta.RootElement;

                if (!root.GetProperty("success").GetBoolean())
                {
                    throw new InvalidOperationException(
                        $"TIME no completó la lectura de {tabla}.");
                }

                JsonElement datos = root.GetProperty("data");
                JsonElement sync = root.GetProperty("sync");

                if (datos.ValueKind != JsonValueKind.Array)
                {
                    throw new InvalidOperationException(
                        $"TIME devolvió un lote inválido de {tabla}.");
                }

                if (!upperId.HasValue)
                {
                    upperId = sync.GetProperty("upper_id")
                        .GetInt64();
                }

                hasMore = sync.GetProperty("has_more")
                    .GetBoolean();

                long siguienteId = sync
                    .GetProperty("next_after_id")
                    .GetInt64();

                var registros = datos
                    .EnumerateArray()
                    .Select(x => x.Clone())
                    .ToList();

                // Una consulta incremental puede regresar cero registros.
                // Eso no debe reiniciar el cursor.
                if (registros.Count == 0)
                {
                    if (hasMore)
                    {
                        throw new InvalidOperationException(
                            $"TIME indicó más páginas en {tabla}, " +
                            "pero devolvió un lote vacío.");
                    }

                    siguienteId = afterId;
                }
                else if (siguienteId <= afterId)
                {
                    throw new InvalidOperationException(
                        $"TIME devolvió un cursor que no avanzó en {tabla}.");
                }

                if (hasMore &&
                    (registros.Count == 0 ||
                     siguienteId <= afterId))
                {
                    throw new InvalidOperationException(
                        $"El cursor de {tabla} no avanzó correctamente.");
                }

                // Validar todos los IDs antes de escribir.
                var ids = registros
                    .Select(x => LeerLong(x, "id"))
                    .ToList();

                if (ids.Any(id => !id.HasValue || id.Value <= 0))
                {
                    throw new InvalidOperationException(
                        $"TIME devolvió IDs inválidos en {tabla}.");
                }

                var idsValidos = ids
                    .Select(x => x!.Value)
                    .ToList();

                if (idsValidos.Distinct().Count() != idsValidos.Count)
                {
                    throw new InvalidOperationException(
                        $"TIME devolvió IDs duplicados en {tabla}.");
                }

                // Consultar únicamente los IDs de este lote.
                var existentesLista = await _db.Set<T>()
                    .Where(x => idsValidos.Contains(
                        EF.Property<long>(x, "Id")))
                    .ToListAsync(cancellationToken);

                var existentes = existentesLista
                    .ToDictionary(obtenerId);

                long insertadosLote = 0;
                long actualizadosLote = 0;

                foreach (JsonElement registro in registros)
                {
                    long id = LeerLong(registro, "id")!.Value;

                    if (!existentes.TryGetValue(id, out T? entidad))
                    {
                        entidad = crear(id);
                        _db.Set<T>().Add(entidad);
                        insertadosLote++;
                    }
                    else
                    {
                        actualizadosLote++;
                    }

                    asignar(entidad, registro);
                }

                estado.UltimoId = siguienteId;

                // Mientras existan páginas pendientes, mantenemos
                // la misma ventana de lectura.
                // Al completar, permitimos una nueva ventana.
                estado.UltimoUpperId = hasMore
                    ? upperId
                    : null;

                estado.UltimaEjecucion = DateTime.UtcNow;

                estado.Estado = hasMore
                    ? "En proceso"
                    : "Completado";
                estado.Error = null;
                estado.RegistrosProcesados += registros.Count;

                await _db.SaveChangesAsync(cancellationToken);

                resultado.RegistrosLeidos += registros.Count;
                resultado.RegistrosInsertados += insertadosLote;
                resultado.RegistrosActualizados += actualizadosLote;
                resultado.UltimoId = siguienteId;

                _logger.LogInformation(
                    "TIME {Tabla}: lote guardado. " +
                    "Cursor {Cursor}. Leídos en ejecución: {Leidos}.",
                    tabla,
                    siguienteId,
                    resultado.RegistrosLeidos);

                // Limpiar entidades procesadas del lote.
                _db.ChangeTracker.Clear();

                // Recuperar el estado correcto:
                // horas o conciliacion:horas.
                if (hasMore)
                {
                    estado = await _db.TimeEstadosSincronizacion
                        .SingleAsync(
                            x => x.Tabla == claveEstado,
                            cancellationToken);
                }

                afterId = siguienteId;

            }

            resultado.Estado = "Completado";

            _logger.LogInformation(
                "TIME {Tabla} completado: {Leidos} leídos, " +
                "{Insertados} insertados, {Actualizados} existentes.",
                tabla,
                resultado.RegistrosLeidos,
                resultado.RegistrosInsertados,
                resultado.RegistrosActualizados);

            return resultado;
        }

        // =====================================================
        // LECTURA SEGURA DE VALORES JSON
        // =====================================================

        private static long? LeerLong(
            JsonElement registro,
            string campo)
        {
            if (!registro.TryGetProperty(campo, out var valor) ||
                valor.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (valor.ValueKind == JsonValueKind.Number &&
                valor.TryGetInt64(out long numero))
            {
                return numero;
            }

            throw new InvalidOperationException(
                $"TIME devolvió un valor inválido para {campo}.");
        }

        private static int? LeerInt(
            JsonElement registro,
            string campo)
        {
            if (!registro.TryGetProperty(campo, out var valor) ||
                valor.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (valor.ValueKind == JsonValueKind.Number &&
                valor.TryGetInt32(out int numero))
            {
                return numero;
            }

            throw new InvalidOperationException(
                $"TIME devolvió un entero inválido en {campo}.");
        }

        private static byte? LeerByte(
            JsonElement registro,
            string campo)
        {
            if (!registro.TryGetProperty(campo, out var valor) ||
                valor.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (valor.ValueKind == JsonValueKind.Number &&
                valor.TryGetByte(out byte numero))
            {
                return numero;
            }

            throw new InvalidOperationException(
                $"TIME devolvió un byte inválido en {campo}.");
        }

        private static double? LeerDouble(
            JsonElement registro,
            string campo)
        {
            if (!registro.TryGetProperty(campo, out var valor) ||
                valor.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (valor.ValueKind == JsonValueKind.Number &&
                valor.TryGetDouble(out double numero))
            {
                return numero;
            }

            throw new InvalidOperationException(
                $"TIME devolvió un decimal inválido en {campo}.");
        }

        private static string? LeerTexto(
            JsonElement registro,
            string campo)
        {
            if (!registro.TryGetProperty(campo, out var valor) ||
                valor.ValueKind == JsonValueKind.Null)
            {
                return null;
            }

            if (valor.ValueKind == JsonValueKind.String)
            {
                return valor.GetString();
            }

            throw new InvalidOperationException(
                $"TIME devolvió un texto inválido en {campo}.");
        }

        private static DateTime? LeerFecha(
            JsonElement registro,
            string campo)
        {
            string? texto = LeerTexto(registro, campo);

            if (string.IsNullOrWhiteSpace(texto))
                return null;

            if (DateTime.TryParse(
                texto,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime fecha))
            {
                return fecha;
            }

            throw new InvalidOperationException(
                $"TIME devolvió una fecha inválida en {campo}.");
        }

        private static DateTime? LeerFechaSolo(
            JsonElement registro,
            string campo)
        {
            string? texto = LeerTexto(registro, campo);

            if (string.IsNullOrWhiteSpace(texto))
                return null;

            if (DateTime.TryParseExact(
                texto,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime fecha))
            {
                return fecha;
            }

            throw new InvalidOperationException(
                $"TIME devolvió una fecha inválida en {campo}.");
        }
    }
}
