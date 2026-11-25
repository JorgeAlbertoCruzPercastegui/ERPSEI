
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

using Movimiento =
    ERPSEI.Services.RelojChecador
        .RelojEvaluacionAsistenciaUnificadaService
        .MovimientoUnificado;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojResolucionConflictosService
{
    private readonly ApplicationDbContext _db;

    private static readonly TimeZoneInfo ZonaMexico =
        TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

    public RelojResolucionConflictosService(
        ApplicationDbContext db)
    {
        _db = db;
    }

    // =========================================================
    // MODELOS PÚBLICOS
    // =========================================================

    public sealed class Decision
    {
        public long Id { get; set; }
        public string TipoMovimiento { get; set; } = "";
        public string MovimientoSeleccionado { get; set; } = "";
        public string FirmaCandidatos { get; set; } = "";
        public string Estado { get; set; } = "";
        public string Motivo { get; set; } = "";
        public DateTime FechaResolucionUtc { get; set; }
    }

    public sealed class GrupoConflicto
    {
        public string TipoMovimiento { get; set; } = "";
        public string Firma { get; set; } = "";
        public List<string> Candidatos { get; set; } = new();
        public string? Seleccionado { get; set; }
        public string EstadoResolucion { get; set; } = "SinResolver";
        public long? ResolucionId { get; set; }
    }

    public sealed class ResultadoOperacion
    {
        public bool Success { get; set; }
        public string Codigo { get; set; } = "";
        public string Message { get; set; } = "";
    }

    public sealed class AplicacionResultado
    {
        public int ResolucionesAplicadas { get; set; }
        public List<string> Observaciones { get; set; } = new();
    }

    private sealed class ResolucionActual
    {
        public long Id { get; set; }
        public string MovimientoSeleccionado { get; set; } = "";
        public string Estado { get; set; } = "";
    }

    // =========================================================
    // REGLAS COMPARTIDAS
    // =========================================================

    private static bool TipoValido(string? tipo)
    {
        return RelojReglasConciliacion.EsTipoValido(tipo);
    }

    public static string CalcularFirma(
        string tipo,
        IEnumerable<string> ids)
    {
        return RelojReglasConciliacion.CalcularFirma(
            tipo,
            ids);
    }

    // =========================================================
    // CONSULTAR DECISIONES
    // =========================================================

    public async Task<Dictionary<string, Decision>>
        ObtenerDecisionesAsync(
            int empleadoId,
            DateTime fecha,
            CancellationToken ct = default)
    {
        if (empleadoId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(empleadoId));
        }

        const string sql = """
            SELECT
                Id,
                TipoMovimiento,
                MovimientoSeleccionado,
                FirmaCandidatos,
                Estado,
                Motivo,
                FechaResolucionUtc
            FROM dbo.RC_ResolucionesConflictos
            WHERE EmpleadoId = @EmpleadoId
              AND FechaLocal = @Fecha;
            """;

        var resultado = new Dictionary<string, Decision>(
            StringComparer.Ordinal);

        var conexion = _db.Database.GetDbConnection();
        bool cerrar = conexion.State != ConnectionState.Open;

        try
        {
            if (cerrar)
                await conexion.OpenAsync(ct);

            await using var cmd = NuevoComando(
                conexion,
                sql);

            Parametro(
                cmd,
                "@EmpleadoId",
                DbType.Int32,
                empleadoId);

            Parametro(
                cmd,
                "@Fecha",
                DbType.Date,
                fecha.Date);

            await using var lector =
                await cmd.ExecuteReaderAsync(ct);

            while (await lector.ReadAsync(ct))
            {
                var decision = new Decision
                {
                    Id = lector.GetInt64(0),
                    TipoMovimiento = lector.GetString(1),
                    MovimientoSeleccionado = lector.GetString(2),
                    FirmaCandidatos = lector.GetString(3),
                    Estado = lector.GetString(4),
                    Motivo = lector.GetString(5),
                    FechaResolucionUtc = lector.GetDateTime(6)
                };

                resultado.Add(
                    decision.TipoMovimiento,
                    decision);
            }
        }
        finally
        {
            if (cerrar &&
                conexion.State != ConnectionState.Closed)
            {
                await conexion.CloseAsync();
            }
        }

        return resultado;
    }

    // =========================================================
    // APLICAR RESOLUCIONES AL EVALUADOR UNIFICADO
    // =========================================================

    public async Task<AplicacionResultado> AplicarAsync(
        int empleadoId,
        DateTime fecha,
        List<Movimiento> movimientos,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(movimientos);

        var resultado = new AplicacionResultado();

        var decisiones = await ObtenerDecisionesAsync(
            empleadoId,
            fecha,
            ct);

        var grupos = movimientos
            .Where(x =>
                x.Confirmado &&
                TipoValido(x.TipoEvaluacion))
            .GroupBy(
                x => x.TipoEvaluacion,
                StringComparer.Ordinal)
            .ToList();

        foreach (var grupo in grupos)
        {
            if (!decisiones.TryGetValue(
                    grupo.Key,
                    out var decision))
            {
                continue;
            }

            var candidatos = grupo.ToList();

            // REGLA CENTRALIZADA:
            // No se aplica una decisión revocada,
            // desactualizada o con candidato inválido.

            bool aplicable =
                RelojReglasConciliacion.ResolucionAplicable(
                    grupo.Key,
                    candidatos.Select(x => x.IdMovimiento),
                    decision.MovimientoSeleccionado,
                    decision.FirmaCandidatos,
                    decision.Estado);

            if (!aplicable)
            {
                if (decision.Estado == "Activa")
                {
                    resultado.Observaciones.Add(
                        $"La resolución de {grupo.Key} " +
                        "ya no coincide con las marcas confirmadas. " +
                        "Se requiere nueva revisión administrativa.");
                }

                continue;
            }

            foreach (var movimiento in candidatos)
            {
                bool seleccionado =
                    movimiento.IdMovimiento ==
                    decision.MovimientoSeleccionado;

                movimiento.ParticipaEnEvaluacion =
                    seleccionado;

                movimiento.Observacion = seleccionado
                    ? "Seleccionado mediante resolución administrativa auditada."
                    : "Excluido del cálculo preliminar por resolución administrativa. Se conserva la checada original.";
            }

            resultado.ResolucionesAplicadas++;
        }

        foreach (var decision in decisiones.Values
            .Where(x => x.Estado == "Activa"))
        {
            bool existeGrupo = grupos.Any(x =>
                x.Key == decision.TipoMovimiento);

            if (!existeGrupo)
            {
                resultado.Observaciones.Add(
                    $"La resolución activa de " +
                    $"{decision.TipoMovimiento} no tiene " +
                    "movimientos confirmados aplicables.");
            }
        }

        return resultado;
    }

    // =========================================================
    // GRUPOS DE CONFLICTOS PARA LA INTERFAZ
    // =========================================================

    public async Task<List<GrupoConflicto>>
        ObtenerGruposAsync(
            int empleadoId,
            DateTime fecha,
            IReadOnlyList<Movimiento> movimientos,
            CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(movimientos);

        var decisiones = await ObtenerDecisionesAsync(
            empleadoId,
            fecha,
            ct);

        var resultado = new List<GrupoConflicto>();

        var grupos = movimientos
            .Where(x =>
                x.Confirmado &&
                TipoValido(x.TipoEvaluacion))
            .GroupBy(
                x => x.TipoEvaluacion,
                StringComparer.Ordinal)
            .ToList();

        foreach (var grupo in grupos)
        {
            var candidatos = grupo
                .Select(x => x.IdMovimiento)
                .OrderBy(
                    x => x,
                    StringComparer.Ordinal)
                .ToList();

            if (candidatos.Count < 2 &&
                !decisiones.ContainsKey(grupo.Key))
            {
                continue;
            }

            string firma = CalcularFirma(
                grupo.Key,
                candidatos);

            decisiones.TryGetValue(
                grupo.Key,
                out var decision);

            string estado = "SinResolver";

            if (decision != null)
            {
                bool aplicable =
                    RelojReglasConciliacion
                        .ResolucionAplicable(
                            grupo.Key,
                            candidatos,
                            decision.MovimientoSeleccionado,
                            decision.FirmaCandidatos,
                            decision.Estado);

                estado = decision.Estado == "Revocada"
                    ? "Revocada"
                    : aplicable
                        ? "Aplicada"
                        : "Desactualizada";
            }

            resultado.Add(new GrupoConflicto
            {
                TipoMovimiento = grupo.Key,
                Firma = firma,
                Candidatos = candidatos,
                Seleccionado =
                    decision?.MovimientoSeleccionado,
                EstadoResolucion = estado,
                ResolucionId = decision?.Id
            });
        }

        foreach (var decision in decisiones.Values)
        {
            if (resultado.Any(x =>
                x.TipoMovimiento ==
                decision.TipoMovimiento))
            {
                continue;
            }

            resultado.Add(new GrupoConflicto
            {
                TipoMovimiento =
                    decision.TipoMovimiento,
                Seleccionado =
                    decision.MovimientoSeleccionado,
                ResolucionId = decision.Id,
                EstadoResolucion =
                    decision.Estado == "Revocada"
                        ? "Revocada"
                        : "Desactualizada"
            });
        }

        return resultado
            .OrderBy(x => x.TipoMovimiento)
            .ToList();
    }

    // =========================================================
    // RESOLVER CONFLICTO
    // =========================================================

    public async Task<ResultadoOperacion> ResolverAsync(
        int empleadoId,
        DateTime fecha,
        string tipo,
        string seleccionado,
        string firmaEsperada,
        string motivo,
        string actor,
        IReadOnlyList<Movimiento> movimientos,
        CancellationToken ct = default)
    {
        if (empleadoId <= 0 ||
            !TipoValido(tipo) ||
            string.IsNullOrWhiteSpace(seleccionado) ||
            seleccionado.Length > 100 ||
            string.IsNullOrWhiteSpace(actor) ||
            actor.Length > 450 ||
            string.IsNullOrWhiteSpace(motivo) ||
            motivo.Trim().Length < 10 ||
            motivo.Trim().Length > 1000)
        {
            return Error(
                "DATOS_INVALIDOS",
                "Verifica el empleado, tipo, usuario y justificación.");
        }

        if (!FechaPermitida(fecha))
        {
            return Error(
                "FECHA_INVALIDA",
                "La fecha está fuera del periodo permitido.");
        }

        ArgumentNullException.ThrowIfNull(movimientos);

        motivo = motivo.Trim();

        // =====================================================
        // VALIDACIÓN PRELIMINAR
        // =====================================================

        var candidatosPantalla = movimientos
            .Where(x =>
                x.Confirmado &&
                x.TipoEvaluacion == tipo)
            .Select(x => x.IdMovimiento)
            .OrderBy(
                x => x,
                StringComparer.Ordinal)
            .ToList();

        var validacionPantalla =
            RelojReglasConciliacion.ValidarSeleccion(
                tipo,
                seleccionado,
                firmaEsperada,
                candidatosPantalla);

        if (!validacionPantalla.Correcto)
        {
            return Error(
                validacionPantalla.Codigo,
                validacionPantalla.Mensaje);
        }

        // =====================================================
        // TRANSACCIÓN SQL
        // =====================================================

        var conexion = _db.Database.GetDbConnection();

        bool cerrar =
            conexion.State != ConnectionState.Open;

        try
        {
            if (cerrar)
                await conexion.OpenAsync(ct);

            await using var tx =
                await conexion.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    ct);

            try
            {
                // =============================================
                // VALIDACIÓN DEFINITIVA
                // =============================================

                var candidatosSQL =
                    await ObtenerCandidatosBloqueadosAsync(
                        conexion,
                        tx,
                        empleadoId,
                        fecha.Date,
                        tipo,
                        ct);

                var validacionSQL =
                    RelojReglasConciliacion.ValidarSeleccion(
                        tipo,
                        seleccionado,
                        firmaEsperada,
                        candidatosSQL);

                if (!validacionSQL.Correcto)
                {
                    // No se hizo ninguna escritura.
                    // La transacción se revierte al salir.
                    await tx.RollbackAsync(ct);

                    return Error(
                        validacionSQL.Codigo,
                        validacionSQL.Mensaje);
                }

                string firmaSQL =
                    RelojReglasConciliacion.CalcularFirma(
                        tipo,
                        candidatosSQL);

                // =============================================
                // CONSULTAR Y BLOQUEAR RESOLUCIÓN ACTUAL
                // =============================================

                var actual =
                    await ObtenerResolucionBloqueadaAsync(
                        conexion,
                        tx,
                        empleadoId,
                        fecha.Date,
                        tipo,
                        ct);

                long resolucionId;
                string accion;

                if (actual == null)
                {
                    accion = "RESOLUCION";

                    const string sqlInsert = """
                        INSERT INTO dbo.RC_ResolucionesConflictos
                        (
                            EmpleadoId,
                            FechaLocal,
                            TipoMovimiento,
                            MovimientoSeleccionado,
                            FirmaCandidatos,
                            Estado,
                            Motivo,
                            ResueltoPorUserId
                        )
                        OUTPUT INSERTED.Id
                        VALUES
                        (
                            @EmpleadoId,
                            @Fecha,
                            @Tipo,
                            @Seleccionado,
                            @Firma,
                            'Activa',
                            @Motivo,
                            @Actor
                        );
                        """;

                    await using var cmd = NuevoComando(
                        conexion,
                        sqlInsert,
                        tx);

                    DatosComunes(
                        cmd,
                        empleadoId,
                        fecha,
                        tipo);

                    Parametro(
                        cmd,
                        "@Seleccionado",
                        DbType.String,
                        seleccionado);

                    Parametro(
                        cmd,
                        "@Firma",
                        DbType.AnsiStringFixedLength,
                        firmaSQL);

                    Parametro(
                        cmd,
                        "@Motivo",
                        DbType.String,
                        motivo);

                    Parametro(
                        cmd,
                        "@Actor",
                        DbType.String,
                        actor);

                    resolucionId = Convert.ToInt64(
                        await cmd.ExecuteScalarAsync(ct));
                }
                else
                {
                    accion = actual.Estado == "Revocada"
                        ? "REACTIVACION"
                        : "CAMBIO";

                    resolucionId = actual.Id;

                    const string sqlUpdate = """
                        UPDATE dbo.RC_ResolucionesConflictos
                        SET
                            MovimientoSeleccionado = @Seleccionado,
                            FirmaCandidatos = @Firma,
                            Estado = 'Activa',
                            Motivo = @Motivo,
                            ResueltoPorUserId = @Actor,
                            FechaResolucionUtc = SYSUTCDATETIME(),
                            FechaActualizacionUtc = SYSUTCDATETIME()
                        WHERE Id = @Id;
                        """;

                    await using var cmd = NuevoComando(
                        conexion,
                        sqlUpdate,
                        tx);

                    Parametro(
                        cmd,
                        "@Id",
                        DbType.Int64,
                        resolucionId);

                    Parametro(
                        cmd,
                        "@Seleccionado",
                        DbType.String,
                        seleccionado);

                    Parametro(
                        cmd,
                        "@Firma",
                        DbType.AnsiStringFixedLength,
                        firmaSQL);

                    Parametro(
                        cmd,
                        "@Motivo",
                        DbType.String,
                        motivo);

                    Parametro(
                        cmd,
                        "@Actor",
                        DbType.String,
                        actor);

                    int afectados =
                        await cmd.ExecuteNonQueryAsync(ct);

                    if (afectados != 1)
                    {
                        throw new InvalidOperationException(
                            "No fue posible actualizar la resolución.");
                    }
                }

                // =============================================
                // AUDITORÍA TRANSACCIONAL
                // =============================================

                await GuardarHistorialAsync(
                    conexion,
                    tx,
                    resolucionId,
                    empleadoId,
                    fecha,
                    tipo,
                    accion,
                    actual?.MovimientoSeleccionado,
                    seleccionado,
                    actual?.Estado,
                    "Activa",
                    firmaSQL,
                    motivo,
                    actor,
                    ct);

                // La resolución y su historial se confirman
                // juntos. Si falla el historial, se revierte
                // también la resolución.

                await tx.CommitAsync(ct);

                return Ok(
                    "RESUELTO",
                    "Resolución registrada y auditada correctamente.");
            }
            catch
            {
                await tx.RollbackAsync(
                    CancellationToken.None);

                throw;
            }
        }
        finally
        {
            if (cerrar &&
                conexion.State != ConnectionState.Closed)
            {
                await conexion.CloseAsync();
            }
        }
    }

    // =========================================================
    // REVOCAR RESOLUCIÓN
    // =========================================================

    public async Task<ResultadoOperacion> RevocarAsync(
        int empleadoId,
        DateTime fecha,
        string tipo,
        string motivo,
        string actor,
        CancellationToken ct = default)
    {
        if (empleadoId <= 0 ||
            !TipoValido(tipo) ||
            string.IsNullOrWhiteSpace(actor) ||
            actor.Length > 450 ||
            string.IsNullOrWhiteSpace(motivo) ||
            motivo.Trim().Length < 10 ||
            motivo.Trim().Length > 1000)
        {
            return Error(
                "DATOS_INVALIDOS",
                "Indica una justificación válida.");
        }

        if (!FechaPermitida(fecha))
        {
            return Error(
                "FECHA_INVALIDA",
                "La fecha está fuera del periodo permitido.");
        }

        motivo = motivo.Trim();

        var conexion = _db.Database.GetDbConnection();
        bool cerrar = conexion.State != ConnectionState.Open;

        try
        {
            if (cerrar)
                await conexion.OpenAsync(ct);

            await using var tx =
                await conexion.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    ct);

            try
            {
                var actual =
                    await ObtenerResolucionBloqueadaAsync(
                        conexion,
                        tx,
                        empleadoId,
                        fecha.Date,
                        tipo,
                        ct);

                if (actual == null ||
                    actual.Estado != "Activa")
                {
                    await tx.RollbackAsync(ct);

                    return Error(
                        "NO_ACTIVA",
                        "No existe una resolución activa para revocar.");
                }

                const string sql = """
                    UPDATE dbo.RC_ResolucionesConflictos
                    SET
                        Estado = 'Revocada',
                        FechaActualizacionUtc = SYSUTCDATETIME()
                    WHERE Id = @Id
                      AND Estado = 'Activa';
                    """;

                await using (var cmd = NuevoComando(
                    conexion,
                    sql,
                    tx))
                {
                    Parametro(
                        cmd,
                        "@Id",
                        DbType.Int64,
                        actual.Id);

                    int afectados =
                        await cmd.ExecuteNonQueryAsync(ct);

                    if (afectados != 1)
                    {
                        await tx.RollbackAsync(ct);

                        return Error(
                            "ESTADO_CAMBIO",
                            "La resolución cambió de estado.");
                    }
                }

                await GuardarHistorialAsync(
                    conexion,
                    tx,
                    actual.Id,
                    empleadoId,
                    fecha,
                    tipo,
                    "REVOCACION",
                    actual.MovimientoSeleccionado,
                    null,
                    "Activa",
                    "Revocada",
                    null,
                    motivo,
                    actor,
                    ct);

                await tx.CommitAsync(ct);

                return Ok(
                    "REVOCADA",
                    "Resolución revocada y auditada. Las checadas originales permanecen intactas.");
            }
            catch
            {
                await tx.RollbackAsync(
                    CancellationToken.None);

                throw;
            }
        }
        finally
        {
            if (cerrar &&
                conexion.State != ConnectionState.Closed)
            {
                await conexion.CloseAsync();
            }
        }
    }

    // =========================================================
    // RECONSTRUIR CANDIDATOS DIRECTAMENTE DESDE SQL
    // =========================================================

    private static async Task<List<string>>
        ObtenerCandidatosBloqueadosAsync(
            DbConnection conexion,
            DbTransaction tx,
            int empleadoId,
            DateTime fecha,
            string tipo,
            CancellationToken ct)
    {
        var inicioLocal = DateTime.SpecifyKind(
            fecha.Date,
            DateTimeKind.Unspecified);

        var finLocal = inicioLocal.AddDays(1);

        var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(
            inicioLocal,
            ZonaMexico);

        var finUtc = TimeZoneInfo.ConvertTimeToUtc(
            finLocal,
            ZonaMexico);

        // La validación consulta ambas fuentes.
        // Se mantienen bloqueos durante la transacción
        // para reducir el riesgo de decisiones obsoletas.

        const string sql = """
            SELECT
                CONCAT(
                    'INTRANET:',
                    CONVERT(varchar(30), c.Id)
                ) AS IdMovimiento
            FROM dbo.RC_Checadas c WITH (UPDLOCK, HOLDLOCK)
            INNER JOIN dbo.RC_Jornadas j WITH (HOLDLOCK)
                ON j.Id = c.JornadaId
            WHERE j.EmpleadoId = @EmpleadoId
              AND c.TipoMovimiento = @Tipo
              AND c.EstadoValidacion = 'Validada'
              AND c.FechaHoraUtc >= @InicioUtc
              AND c.FechaHoraUtc < @FinUtc

            UNION ALL

            SELECT
                CONCAT(
                    'BIOTIME:',
                    CONVERT(varchar(30), b.Id)
                ) AS IdMovimiento
            FROM dbo.RC_ClasificacionesBioTime cl
                WITH (UPDLOCK, HOLDLOCK)
            INNER JOIN dbo.BioTimeChecadas b WITH (HOLDLOCK)
                ON b.Id = cl.BioTimeChecadaId
            WHERE cl.EmpleadoId = @EmpleadoId
              AND cl.FechaLocal = @Fecha
              AND cl.TipoMovimiento = @Tipo
              AND cl.Estado = 'Confirmada'
              AND b.FechaHora >= @Fecha
              AND b.FechaHora < DATEADD(day, 1, @Fecha)
              AND EXISTS
              (
                  SELECT 1
                  FROM dbo.RC_EmpleadosBioTime v WITH (HOLDLOCK)
                  WHERE v.EmpleadoId = cl.EmpleadoId
                    AND v.EmpCode = b.EmpCode
                    AND v.Estado = 'Validada'
                    AND v.FechaInicio <= @Fecha
                    AND (
                        v.FechaFin IS NULL
                        OR v.FechaFin >= @Fecha
                    )
              );
            """;

        var candidatos = new List<string>();

        await using var cmd = NuevoComando(
            conexion,
            sql,
            tx);

        DatosComunes(
            cmd,
            empleadoId,
            fecha,
            tipo);

        Parametro(
            cmd,
            "@InicioUtc",
            DbType.DateTime2,
            inicioUtc);

        Parametro(
            cmd,
            "@FinUtc",
            DbType.DateTime2,
            finUtc);

        await using var lector =
            await cmd.ExecuteReaderAsync(ct);

        while (await lector.ReadAsync(ct))
        {
            candidatos.Add(lector.GetString(0));
        }

        if (candidatos.Count !=
            candidatos.Distinct(
                StringComparer.Ordinal).Count())
        {
            throw new InvalidOperationException(
                "Se detectaron identificadores de movimiento duplicados.");
        }

        return candidatos
            .OrderBy(
                x => x,
                StringComparer.Ordinal)
            .ToList();
    }

    // =========================================================
    // OBTENER RESOLUCIÓN EXISTENTE CON BLOQUEO
    // =========================================================

    private static async Task<ResolucionActual?>
        ObtenerResolucionBloqueadaAsync(
            DbConnection conexion,
            DbTransaction tx,
            int empleadoId,
            DateTime fecha,
            string tipo,
            CancellationToken ct)
    {
        const string sql = """
            SELECT
                Id,
                MovimientoSeleccionado,
                Estado
            FROM dbo.RC_ResolucionesConflictos
                WITH (UPDLOCK, HOLDLOCK)
            WHERE EmpleadoId = @EmpleadoId
              AND FechaLocal = @Fecha
              AND TipoMovimiento = @Tipo;
            """;

        await using var cmd = NuevoComando(
            conexion,
            sql,
            tx);

        DatosComunes(
            cmd,
            empleadoId,
            fecha,
            tipo);

        await using var lector =
            await cmd.ExecuteReaderAsync(ct);

        if (!await lector.ReadAsync(ct))
            return null;

        return new ResolucionActual
        {
            Id = lector.GetInt64(0),
            MovimientoSeleccionado =
                lector.GetString(1),
            Estado = lector.GetString(2)
        };
    }

    // =========================================================
    // GUARDAR HISTORIAL DE AUDITORÍA
    // =========================================================

    private static async Task GuardarHistorialAsync(
        DbConnection conexion,
        DbTransaction tx,
        long resolucionId,
        int empleadoId,
        DateTime fecha,
        string tipo,
        string accion,
        string? anterior,
        string? nuevo,
        string? estadoAnterior,
        string estadoNuevo,
        string? firma,
        string motivo,
        string actor,
        CancellationToken ct)
    {
        const string sql = """
            INSERT INTO dbo.RC_HistorialResolucionesConflictos
            (
                ResolucionId,
                EmpleadoId,
                FechaLocal,
                TipoMovimiento,
                Accion,
                MovimientoAnterior,
                MovimientoNuevo,
                EstadoAnterior,
                EstadoNuevo,
                FirmaCandidatos,
                Motivo,
                ActorUserId
            )
            VALUES
            (
                @Id,
                @EmpleadoId,
                @Fecha,
                @Tipo,
                @Accion,
                @Anterior,
                @Nuevo,
                @EstadoAnterior,
                @EstadoNuevo,
                @Firma,
                @Motivo,
                @Actor
            );
            """;

        await using var cmd = NuevoComando(
            conexion,
            sql,
            tx);

        DatosComunes(
            cmd,
            empleadoId,
            fecha,
            tipo);

        Parametro(
            cmd,
            "@Id",
            DbType.Int64,
            resolucionId);

        Parametro(
            cmd,
            "@Accion",
            DbType.String,
            accion);

        Parametro(
            cmd,
            "@Anterior",
            DbType.String,
            (object?)anterior ?? DBNull.Value);

        Parametro(
            cmd,
            "@Nuevo",
            DbType.String,
            (object?)nuevo ?? DBNull.Value);

        Parametro(
            cmd,
            "@EstadoAnterior",
            DbType.String,
            (object?)estadoAnterior ?? DBNull.Value);

        Parametro(
            cmd,
            "@EstadoNuevo",
            DbType.String,
            estadoNuevo);

        Parametro(
            cmd,
            "@Firma",
            DbType.AnsiStringFixedLength,
            (object?)firma ?? DBNull.Value);

        Parametro(
            cmd,
            "@Motivo",
            DbType.String,
            motivo);

        Parametro(
            cmd,
            "@Actor",
            DbType.String,
            actor);

        int afectados =
            await cmd.ExecuteNonQueryAsync(ct);

        if (afectados != 1)
        {
            throw new InvalidOperationException(
                "No fue posible registrar el historial de auditoría.");
        }
    }

    // =========================================================
    // VALIDACIONES Y UTILIDADES
    // =========================================================

    private static bool FechaPermitida(DateTime fecha)
    {
        var hoy = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            ZonaMexico).Date;

        return fecha.Date >= hoy.AddDays(-365) &&
               fecha.Date <= hoy;
    }

    private static DbCommand NuevoComando(
        DbConnection conexion,
        string sql,
        DbTransaction? tx = null)
    {
        var cmd = conexion.CreateCommand();

        cmd.CommandText = sql;
        cmd.CommandType = CommandType.Text;
        cmd.CommandTimeout = 30;
        cmd.Transaction = tx;

        return cmd;
    }

    private static void DatosComunes(
        DbCommand cmd,
        int empleadoId,
        DateTime fecha,
        string tipo)
    {
        Parametro(
            cmd,
            "@EmpleadoId",
            DbType.Int32,
            empleadoId);

        Parametro(
            cmd,
            "@Fecha",
            DbType.Date,
            fecha.Date);

        Parametro(
            cmd,
            "@Tipo",
            DbType.String,
            tipo);
    }

    private static void Parametro(
        DbCommand cmd,
        string nombre,
        DbType tipo,
        object valor)
    {
        var p = cmd.CreateParameter();

        p.ParameterName = nombre;
        p.DbType = tipo;
        p.Value = valor;

        cmd.Parameters.Add(p);
    }

    private static ResultadoOperacion Ok(
        string codigo,
        string mensaje)
    {
        return new ResultadoOperacion
        {
            Success = true,
            Codigo = codigo,
            Message = mensaje
        };
    }

    private static ResultadoOperacion Error(
        string codigo,
        string mensaje)
    {
        return new ResultadoOperacion
        {
            Success = false,
            Codigo = codigo,
            Message = mensaje
        };
    }
}
