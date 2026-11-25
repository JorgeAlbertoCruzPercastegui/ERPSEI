
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using ERPSEI.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojRevisionBioTimeService
{
    private readonly ApplicationDbContext _db;

    private static readonly TimeZoneInfo ZonaMexico =
        TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

    private static readonly Dictionary<string, int> Orden =
        new(StringComparer.Ordinal)
        {
            ["Entrada"] = 1,
            ["SalidaComer"] = 2,
            ["RegresoComer"] = 3,
            ["Salida"] = 4
        };

    public RelojRevisionBioTimeService(
        ApplicationDbContext db)
    {
        _db = db;
    }

    public sealed class ChecadaRevision
    {
        public long Id { get; init; }
        public DateTime FechaHoraMexico { get; init; }
        public string Origen { get; init; } = "";
        public string Estado { get; init; } = "";
        public string? TipoMovimiento { get; init; }
        public DateTime? FechaConfirmacionUtc { get; init; }
    }

    public sealed class OperacionResultado
    {
        public bool Correcto { get; init; }
        public string Codigo { get; init; } = "";
        public string Mensaje { get; init; } = "";
    }

    private sealed class ClasificacionExistente
    {
        public long Id { get; init; }
        public string Tipo { get; init; } = "";
        public string Estado { get; init; } = "";
    }

    private sealed class MovimientoConfirmado
    {
        public long ChecadaId { get; init; }
        public string Tipo { get; init; } = "";
        public DateTime FechaHoraLocal { get; init; }
    }

    // =========================================================
    // 1. CONSULTAR CHECADAS DEL DÍA
    // =========================================================

    public async Task<IReadOnlyList<ChecadaRevision>>
        ObtenerDiaAsync(
            int empleadoId,
            DateTime fechaLocal,
            CancellationToken ct = default)
    {
        if (empleadoId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(empleadoId));

        DateTime inicio = fechaLocal.Date;

        if (inicio == DateTime.MaxValue.Date)
            throw new ArgumentOutOfRangeException(
                nameof(fechaLocal));

        DateTime fin = inicio.AddDays(1);

        const string sql = """
            SELECT
                c.Id,
                c.FechaHora,
                c.TerminalSn,
                COALESCE(cl.Estado, 'Pendiente'),
                cl.TipoMovimiento,
                cl.FechaConfirmacionUtc
            FROM dbo.BioTimeChecadas c
            LEFT JOIN dbo.RC_ClasificacionesBioTime cl
                ON cl.BioTimeChecadaId = c.Id
            WHERE c.FechaHora >= @Inicio
              AND c.FechaHora < @Fin
              AND EXISTS
              (
                  SELECT 1
                  FROM dbo.RC_EmpleadosBioTime v
                  WHERE v.EmpCode = c.EmpCode
                    AND v.EmpleadoId = @EmpleadoId
                    AND v.Estado = 'Validada'
                    AND v.FechaInicio <= CONVERT(date, c.FechaHora)
                    AND (
                        v.FechaFin IS NULL
                        OR v.FechaFin >= CONVERT(date, c.FechaHora)
                    )
              )
            ORDER BY c.FechaHora, c.Id;
            """;

        var lista = new List<ChecadaRevision>();

        var conexion = _db.Database.GetDbConnection();
        bool cerrar = conexion.State != ConnectionState.Open;

        try
        {
            if (cerrar)
                await conexion.OpenAsync(ct);

            await using var cmd = CrearComando(conexion, sql);

            Agregar(cmd, "@EmpleadoId", DbType.Int32, empleadoId);
            Agregar(cmd, "@Inicio", DbType.DateTime2, inicio);
            Agregar(cmd, "@Fin", DbType.DateTime2, fin);

            await using var lector =
                await cmd.ExecuteReaderAsync(ct);

            while (await lector.ReadAsync(ct))
            {
                lista.Add(new ChecadaRevision
                {
                    Id = lector.GetInt64(0),

                    FechaHoraMexico = lector.GetDateTime(1),

                    Origen = lector.IsDBNull(2)
                        ? "BIOTIME"
                        : lector.GetString(2),

                    Estado = lector.GetString(3),

                    TipoMovimiento = lector.IsDBNull(4)
                        ? null
                        : lector.GetString(4),

                    FechaConfirmacionUtc = lector.IsDBNull(5)
                        ? null
                        : lector.GetDateTime(5)
                });
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

        return lista;
    }

    // =========================================================
    // 2. CONFIRMAR O CORREGIR CLASIFICACIÓN
    // =========================================================

    public async Task<OperacionResultado> ConfirmarAsync(
        long checadaId,
        int empleadoId,
        string tipoMovimiento,
        string motivo,
        string actorUserId,
        CancellationToken ct = default)
    {
        if (checadaId <= 0 || empleadoId <= 0)
        {
            return Error(
                "PARAMETROS_INVALIDOS",
                "La checada o el empleado no son válidos.");
        }

        if (string.IsNullOrWhiteSpace(tipoMovimiento) ||
            !Orden.ContainsKey(tipoMovimiento))
        {
            return Error(
                "TIPO_INVALIDO",
                "Selecciona un tipo de movimiento válido.");
        }

        if (!DatosAuditoriaValidos(motivo, actorUserId))
        {
            return Error(
                "AUDITORIA_INVALIDA",
                "Se requiere un usuario y una justificación de 10 a 1000 caracteres.");
        }

        motivo = motivo.Trim();
        actorUserId = actorUserId.Trim();

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
                // ---------------------------------------------
                // 2.1. VERIFICAR CHECADA Y VINCULACIÓN
                // ---------------------------------------------

                const string sqlOrigen = """
                    SELECT c.FechaHora
                    FROM dbo.BioTimeChecadas c
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE c.Id = @ChecadaId
                      AND EXISTS
                      (
                          SELECT 1
                          FROM dbo.RC_EmpleadosBioTime v
                          WHERE v.EmpCode = c.EmpCode
                            AND v.EmpleadoId = @EmpleadoId
                            AND v.Estado = 'Validada'
                            AND v.FechaInicio <= CONVERT(date, c.FechaHora)
                            AND (
                                v.FechaFin IS NULL
                                OR v.FechaFin >= CONVERT(date, c.FechaHora)
                            )
                      );
                    """;

                DateTime? fechaHora = null;

                await using (var cmd = CrearComando(
                    conexion, sqlOrigen, tx))
                {
                    Agregar(cmd, "@ChecadaId",
                        DbType.Int64, checadaId);

                    Agregar(cmd, "@EmpleadoId",
                        DbType.Int32, empleadoId);

                    object? valor =
                        await cmd.ExecuteScalarAsync(ct);

                    if (valor is DateTime fecha)
                        fechaHora = fecha;
                }

                if (!fechaHora.HasValue)
                {
                    return Error(
                        "SIN_VINCULACION",
                        "La checada no tiene una vinculación validada y vigente.");
                }

                DateTime fechaLocal = fechaHora.Value.Date;

                DateTime hoyMexico =
                    TimeZoneInfo.ConvertTimeFromUtc(
                        DateTime.UtcNow,
                        ZonaMexico).Date;

                if (fechaLocal > hoyMexico)
                {
                    return Error(
                        "FECHA_FUTURA",
                        "No se pueden confirmar movimientos futuros.");
                }

                // ---------------------------------------------
                // 2.2. CONSULTAR CLASIFICACIÓN EXISTENTE
                // ---------------------------------------------

                const string sqlExistente = """
                    SELECT
                        Id,
                        TipoMovimiento,
                        Estado
                    FROM dbo.RC_ClasificacionesBioTime
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE BioTimeChecadaId = @ChecadaId;
                    """;

                ClasificacionExistente? actual = null;

                await using (var cmd = CrearComando(
                    conexion, sqlExistente, tx))
                {
                    Agregar(cmd, "@ChecadaId",
                        DbType.Int64, checadaId);

                    await using var lector =
                        await cmd.ExecuteReaderAsync(ct);

                    if (await lector.ReadAsync(ct))
                    {
                        actual = new ClasificacionExistente
                        {
                            Id = lector.GetInt64(0),
                            Tipo = lector.GetString(1),
                            Estado = lector.GetString(2)
                        };
                    }
                }

                if (actual?.Estado == "Confirmada" &&
                    actual.Tipo == tipoMovimiento)
                {
                    return Error(
                        "YA_CONFIRMADA",
                        "La checada ya está confirmada con ese tipo de movimiento.");
                }

                // ---------------------------------------------
                // 2.3. CONSULTAR LOS MOVIMIENTOS CONFIRMADOS
                //      DEL MISMO EMPLEADO Y DÍA
                // ---------------------------------------------

                const string sqlConfirmados = """
                    SELECT
                        cl.BioTimeChecadaId,
                        cl.TipoMovimiento,
                        c.FechaHora
                    FROM dbo.RC_ClasificacionesBioTime cl
                        WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN dbo.BioTimeChecadas c
                        ON c.Id = cl.BioTimeChecadaId
                    WHERE cl.EmpleadoId = @EmpleadoId
                      AND cl.FechaLocal = @FechaLocal
                      AND cl.Estado = 'Confirmada'
                    ORDER BY c.FechaHora, c.Id;
                    """;

                var confirmados =
                    new List<MovimientoConfirmado>();

                await using (var cmd = CrearComando(
                    conexion, sqlConfirmados, tx))
                {
                    Agregar(cmd, "@EmpleadoId",
                        DbType.Int32, empleadoId);

                    Agregar(cmd, "@FechaLocal",
                        DbType.Date, fechaLocal);

                    await using var lector =
                        await cmd.ExecuteReaderAsync(ct);

                    while (await lector.ReadAsync(ct))
                    {
                        confirmados.Add(
                            new MovimientoConfirmado
                            {
                                ChecadaId = lector.GetInt64(0),
                                Tipo = lector.GetString(1),
                                FechaHoraLocal = lector.GetDateTime(2)
                            });
                    }
                }

                // ---------------------------------------------
                // 2.4. IMPEDIR TIPOS DUPLICADOS
                // ---------------------------------------------

                bool tipoDuplicado = confirmados.Any(x =>
                    x.ChecadaId != checadaId &&
                    x.Tipo == tipoMovimiento);

                if (tipoDuplicado)
                {
                    return Error(
                        "TIPO_DUPLICADO",
                        "Ya existe otra checada confirmada con ese tipo de movimiento.");
                }

                // ---------------------------------------------
                // 2.5. VALIDACIÓN CRONOLÓGICA
                // ---------------------------------------------
                //
                // Entrada      = 1
                // SalidaComer  = 2
                // RegresoComer = 3
                // Salida       = 4
                //
                // Permite movimientos faltantes, pero no
                // movimientos confirmados en orden inverso.
                // ---------------------------------------------

                int ordenNuevo = Orden[tipoMovimiento];

                foreach (var confirmado in confirmados)
                {
                    if (confirmado.ChecadaId == checadaId)
                        continue;

                    if (!Orden.TryGetValue(
                        confirmado.Tipo,
                        out int ordenExistente))
                    {
                        return Error(
                            "TIPO_EXISTENTE_INVALIDO",
                            "Existe una clasificación confirmada desconocida.");
                    }

                    bool contradiccion =
                        (ordenNuevo < ordenExistente &&
                         fechaHora.Value >= confirmado.FechaHoraLocal)
                        ||
                        (ordenNuevo > ordenExistente &&
                         fechaHora.Value <= confirmado.FechaHoraLocal);

                    if (contradiccion)
                    {
                        return Error(
                            "SECUENCIA_INVALIDA",
                            "El movimiento contradice el orden cronológico de otra checada confirmada.");
                    }
                }

                // ---------------------------------------------
                // 2.6. GUARDAR CONFIRMACIÓN O CORRECCIÓN
                // ---------------------------------------------

                long clasificacionId;
                string accion;

                if (actual == null)
                {
                    accion = "CONFIRMACION";

                    const string sqlInsert = """
                        INSERT INTO dbo.RC_ClasificacionesBioTime
                        (
                            BioTimeChecadaId,
                            EmpleadoId,
                            FechaLocal,
                            TipoMovimiento,
                            Estado,
                            ConfirmadoPorUserId,
                            FechaConfirmacionUtc
                        )
                        OUTPUT INSERTED.Id
                        VALUES
                        (
                            @ChecadaId,
                            @EmpleadoId,
                            @FechaLocal,
                            @Tipo,
                            'Confirmada',
                            @Actor,
                            SYSUTCDATETIME()
                        );
                        """;

                    await using var cmd = CrearComando(
                        conexion, sqlInsert, tx);

                    Agregar(cmd, "@ChecadaId",
                        DbType.Int64, checadaId);

                    Agregar(cmd, "@EmpleadoId",
                        DbType.Int32, empleadoId);

                    Agregar(cmd, "@FechaLocal",
                        DbType.Date, fechaLocal);

                    Agregar(cmd, "@Tipo",
                        DbType.String, tipoMovimiento);

                    Agregar(cmd, "@Actor",
                        DbType.String, actorUserId);

                    clasificacionId = Convert.ToInt64(
                        await cmd.ExecuteScalarAsync(ct));
                }
                else
                {
                    accion = actual.Estado == "Revocada"
                        ? "RECONFIRMACION"
                        : "CORRECCION";

                    clasificacionId = actual.Id;

                    const string sqlUpdate = """
                        UPDATE dbo.RC_ClasificacionesBioTime
                        SET
                            EmpleadoId = @EmpleadoId,
                            FechaLocal = @FechaLocal,
                            TipoMovimiento = @Tipo,
                            Estado = 'Confirmada',
                            ConfirmadoPorUserId = @Actor,
                            FechaConfirmacionUtc = SYSUTCDATETIME(),
                            FechaActualizacionUtc = SYSUTCDATETIME()
                        WHERE Id = @Id;
                        """;

                    await using var cmd = CrearComando(
                        conexion, sqlUpdate, tx);

                    Agregar(cmd, "@Id",
                        DbType.Int64, clasificacionId);

                    Agregar(cmd, "@EmpleadoId",
                        DbType.Int32, empleadoId);

                    Agregar(cmd, "@FechaLocal",
                        DbType.Date, fechaLocal);

                    Agregar(cmd, "@Tipo",
                        DbType.String, tipoMovimiento);

                    Agregar(cmd, "@Actor",
                        DbType.String, actorUserId);

                    int afectadas =
                        await cmd.ExecuteNonQueryAsync(ct);

                    if (afectadas != 1)
                    {
                        return Error(
                            "ACTUALIZACION_FALLIDA",
                            "No fue posible actualizar la clasificación.");
                    }
                }

                // ---------------------------------------------
                // 2.7. AUDITORÍA EN LA MISMA TRANSACCIÓN
                // ---------------------------------------------

                await RegistrarHistorialAsync(
                    conexion,
                    tx,
                    clasificacionId,
                    checadaId,
                    empleadoId,
                    accion,
                    actual?.Tipo,
                    tipoMovimiento,
                    actual?.Estado,
                    "Confirmada",
                    motivo,
                    actorUserId,
                    ct);

                // ---------------------------------------------
                // 2.8. CONFIRMAR TRANSACCIÓN
                // ---------------------------------------------

                await tx.CommitAsync(ct);

                return new OperacionResultado
                {
                    Correcto = true,
                    Codigo = "CONFIRMADA",
                    Mensaje =
                        "Movimiento confirmado y auditado correctamente."
                };
            }
            catch (SqlException ex) when (
                ex.Number == 2601 ||
                ex.Number == 2627 ||
                ex.Number == 1205)
            {
                // Al salir del using, una transacción
                // no confirmada se revierte.
                return Error(
                    "CONFLICTO_CONCURRENCIA",
                    "La clasificación entró en conflicto con otra operación. Actualiza la consulta y vuelve a revisar.");
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
    // 3. REVOCAR CLASIFICACIÓN
    // =========================================================

    public async Task<OperacionResultado> RevocarAsync(
        long checadaId,
        int empleadoId,
        string motivo,
        string actorUserId,
        CancellationToken ct = default)
    {
        if (checadaId <= 0 ||
            empleadoId <= 0 ||
            !DatosAuditoriaValidos(motivo, actorUserId))
        {
            return Error(
                "PARAMETROS_INVALIDOS",
                "La checada, el usuario o la justificación son inválidos.");
        }

        motivo = motivo.Trim();
        actorUserId = actorUserId.Trim();

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
                // ---------------------------------------------
                // 3.1. OBTENER CLASIFICACIÓN CONFIRMADA
                // ---------------------------------------------

                const string sqlActual = """
                    SELECT Id, TipoMovimiento
                    FROM dbo.RC_ClasificacionesBioTime
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE BioTimeChecadaId = @ChecadaId
                      AND EmpleadoId = @EmpleadoId
                      AND Estado = 'Confirmada';
                    """;

                long? clasificacionId = null;
                string? tipoAnterior = null;

                await using (var cmd = CrearComando(
                    conexion, sqlActual, tx))
                {
                    Agregar(cmd, "@ChecadaId",
                        DbType.Int64, checadaId);

                    Agregar(cmd, "@EmpleadoId",
                        DbType.Int32, empleadoId);

                    await using var lector =
                        await cmd.ExecuteReaderAsync(ct);

                    if (await lector.ReadAsync(ct))
                    {
                        clasificacionId = lector.GetInt64(0);
                        tipoAnterior = lector.GetString(1);
                    }
                }

                if (!clasificacionId.HasValue)
                {
                    return Error(
                        "NO_CONFIRMADA",
                        "No existe una clasificación confirmada para revocar.");
                }

                // ---------------------------------------------
                // 3.2. ACTUALIZAR ESTADO
                // ---------------------------------------------

                const string sqlRevocar = """
                    UPDATE dbo.RC_ClasificacionesBioTime
                    SET
                        Estado = 'Revocada',
                        FechaActualizacionUtc = SYSUTCDATETIME()
                    WHERE Id = @Id
                      AND Estado = 'Confirmada';
                    """;

                await using (var cmd = CrearComando(
                    conexion, sqlRevocar, tx))
                {
                    Agregar(cmd, "@Id",
                        DbType.Int64, clasificacionId.Value);

                    int afectadas =
                        await cmd.ExecuteNonQueryAsync(ct);

                    if (afectadas != 1)
                    {
                        return Error(
                            "REVOCACION_FALLIDA",
                            "La clasificación cambió de estado durante la operación.");
                    }
                }

                // ---------------------------------------------
                // 3.3. AUDITORÍA
                // ---------------------------------------------

                await RegistrarHistorialAsync(
                    conexion,
                    tx,
                    clasificacionId.Value,
                    checadaId,
                    empleadoId,
                    "REVOCACION",
                    tipoAnterior,
                    tipoAnterior,
                    "Confirmada",
                    "Revocada",
                    motivo,
                    actorUserId,
                    ct);

                await tx.CommitAsync(ct);

                return new OperacionResultado
                {
                    Correcto = true,
                    Codigo = "REVOCADA",
                    Mensaje =
                        "Clasificación revocada y auditada correctamente."
                };
            }
            catch (SqlException ex) when (
                ex.Number == 1205)
            {
                return Error(
                    "CONFLICTO_CONCURRENCIA",
                    "La operación encontró un conflicto de concurrencia. Actualiza e inténtalo nuevamente.");
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
    // 4. INSERTAR HISTORIAL DE AUDITORÍA
    // =========================================================

    private static async Task RegistrarHistorialAsync(
        DbConnection conexion,
        DbTransaction tx,
        long clasificacionId,
        long checadaId,
        int empleadoId,
        string accion,
        string? tipoAnterior,
        string? tipoNuevo,
        string? estadoAnterior,
        string estadoNuevo,
        string motivo,
        string actor,
        CancellationToken ct)
    {
        const string sql = """
            INSERT INTO dbo.RC_HistorialClasificacionesBioTime
            (
                ClasificacionId,
                BioTimeChecadaId,
                EmpleadoId,
                Accion,
                TipoAnterior,
                TipoNuevo,
                EstadoAnterior,
                EstadoNuevo,
                Motivo,
                ActorUserId
            )
            VALUES
            (
                @ClasificacionId,
                @ChecadaId,
                @EmpleadoId,
                @Accion,
                @TipoAnterior,
                @TipoNuevo,
                @EstadoAnterior,
                @EstadoNuevo,
                @Motivo,
                @Actor
            );
            """;

        await using var cmd =
            CrearComando(conexion, sql, tx);

        Agregar(cmd, "@ClasificacionId",
            DbType.Int64, clasificacionId);

        Agregar(cmd, "@ChecadaId",
            DbType.Int64, checadaId);

        Agregar(cmd, "@EmpleadoId",
            DbType.Int32, empleadoId);

        Agregar(cmd, "@Accion",
            DbType.String, accion);

        Agregar(cmd, "@TipoAnterior",
            DbType.String,
            (object?)tipoAnterior ?? DBNull.Value);

        Agregar(cmd, "@TipoNuevo",
            DbType.String,
            (object?)tipoNuevo ?? DBNull.Value);

        Agregar(cmd, "@EstadoAnterior",
            DbType.String,
            (object?)estadoAnterior ?? DBNull.Value);

        Agregar(cmd, "@EstadoNuevo",
            DbType.String, estadoNuevo);

        Agregar(cmd, "@Motivo",
            DbType.String, motivo);

        Agregar(cmd, "@Actor",
            DbType.String, actor);

        int afectadas =
            await cmd.ExecuteNonQueryAsync(ct);

        if (afectadas != 1)
        {
            throw new InvalidOperationException(
                "No fue posible registrar la auditoría.");
        }
    }

    // =========================================================
    // 5. UTILIDADES
    // =========================================================

    private static bool DatosAuditoriaValidos(
        string? motivo,
        string? actorUserId)
    {
        return
            !string.IsNullOrWhiteSpace(motivo) &&
            motivo.Trim().Length >= 10 &&
            motivo.Trim().Length <= 1000 &&
            !string.IsNullOrWhiteSpace(actorUserId) &&
            actorUserId.Trim().Length <= 450;
    }

    private static DbCommand CrearComando(
        DbConnection conexion,
        string sql,
        DbTransaction? tx = null)
    {
        var cmd = conexion.CreateCommand();

        cmd.CommandText = sql;
        cmd.CommandType = CommandType.Text;
        cmd.CommandTimeout = 30;

        if (tx != null)
            cmd.Transaction = tx;

        return cmd;
    }

    private static void Agregar(
        DbCommand cmd,
        string nombre,
        DbType tipo,
        object valor)
    {
        var parametro = cmd.CreateParameter();

        parametro.ParameterName = nombre;
        parametro.DbType = tipo;
        parametro.Value = valor;

        cmd.Parameters.Add(parametro);
    }

    private static OperacionResultado Error(
        string codigo,
        string mensaje)
    {
        return new OperacionResultado
        {
            Correcto = false,
            Codigo = codigo,
            Mensaje = mensaje
        };
    }
}
