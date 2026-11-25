
using System.Data;
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojEvaluacionAsistenciaUnificadaService
{
    private readonly ApplicationDbContext _db;
    private readonly AsistenciaMovimientosService _movimientos;
    private readonly RelojHorariosService _horarios;
    private readonly RelojResolucionConflictosService _resoluciones;

    private static readonly string[] Tipos =
    {
        "Entrada",
        "SalidaComer",
        "RegresoComer",
        "Salida"
    };

    private static readonly TimeZoneInfo ZonaMexico =
        TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

    public RelojEvaluacionAsistenciaUnificadaService(
        ApplicationDbContext db,
        AsistenciaMovimientosService movimientos,
        RelojHorariosService horarios,
        RelojResolucionConflictosService resoluciones)
    {
        _db = db;
        _movimientos = movimientos;
        _horarios = horarios;
        _resoluciones = resoluciones;
    }

    public sealed class MovimientoUnificado
    {
        public string IdMovimiento { get; init; } = "";
        public string Fuente { get; init; } = "";
        public string Origen { get; init; } = "";

        public DateTime FechaHoraMexico { get; init; }
        public DateTime FechaHoraUtc { get; init; }

        public string TipoOriginal { get; init; } = "";
        public string TipoEvaluacion { get; init; } = "";
        public string EstadoValidacion { get; init; } = "";

        public bool Confirmado { get; init; }

        // Ahora puede modificarse mediante una decisión
        // administrativa de conciliación.
        public bool ParticipaEnEvaluacion { get; set; }

        public string Observacion { get; set; } = "";
    }

    public sealed class ResultadoUnificado
    {
        public int EmpleadoId { get; init; }
        public DateTime FechaLocal { get; init; }

        public string Estado { get; set; } = "SinEvaluar";
        public string Mensaje { get; set; } = "";

        public int? HorarioId { get; init; }
        public string? HorarioDescripcion { get; init; }

        public TimeSpan? EntradaProgramada { get; init; }
        public TimeSpan? LimiteRetardo { get; init; }
        public TimeSpan? SalidaProgramada { get; init; }

        public DateTime? EntradaReal { get; set; }
        public DateTime? SalidaComidaReal { get; set; }
        public DateTime? RegresoComidaReal { get; set; }
        public DateTime? SalidaReal { get; set; }

        public int TotalMovimientos { get; set; }
        public int MovimientosIntranet { get; set; }
        public int MovimientosBioTime { get; set; }

        public int MovimientosConfirmados { get; set; }
        public int MovimientosPendientes { get; set; }

        public int MovimientosExcluidosPorResolucion { get; set; }
        public int ResolucionesAplicadas { get; set; }

        public bool SecuenciaCompleta { get; set; }
        public bool RequiereRevision { get; set; }

        public List<string> Observaciones { get; set; } = new();
        public List<MovimientoUnificado> Movimientos { get; set; }
            = new();
    }

    public async Task<ResultadoUnificado> EvaluarDiaAsync(
        int empleadoId,
        DateTime fechaLocal,
        CancellationToken ct = default)
    {
        if (empleadoId <= 0)
            throw new ArgumentOutOfRangeException(nameof(empleadoId));

        var fecha = fechaLocal.Date;

        var hoyMexico = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow, ZonaMexico).Date;

        if (fecha > hoyMexico)
        {
            return new ResultadoUnificado
            {
                EmpleadoId = empleadoId,
                FechaLocal = fecha,
                Estado = "FechaFutura",
                Mensaje = "No se evalúan fechas futuras."
            };
        }

        var horario = await _horarios.ObtenerHorarioEmpleadoAsync(
            empleadoId, fecha, ct);

        var resultado = new ResultadoUnificado
        {
            EmpleadoId = empleadoId,
            FechaLocal = fecha,
            HorarioId = horario.HorarioId,
            HorarioDescripcion = horario.Descripcion,
            EntradaProgramada = horario.Entrada,
            LimiteRetardo = horario.ToleranciaEntrada,
            SalidaProgramada = horario.Salida
        };

        if (!horario.Configurado || !horario.DiaLaborable)
        {
            resultado.Estado = horario.Configurado
                ? "NoLaborable"
                : "SinConfiguracion";

            resultado.Mensaje = horario.Mensaje;
            resultado.RequiereRevision = !horario.Configurado;
            return resultado;
        }

        var normalizados = await _movimientos.ObtenerMovimientosAsync(
            empleadoId, fecha, fecha, ct);

        var confirmaciones = await ObtenerConfirmacionesAsync(
            empleadoId, fecha, ct);

        foreach (var movimiento in normalizados)
        {
            bool intranet = string.Equals(
                movimiento.Fuente,
                "INTRANET",
                StringComparison.OrdinalIgnoreCase);

            bool biotime = string.Equals(
                movimiento.Fuente,
                "BIOTIME",
                StringComparison.OrdinalIgnoreCase);

            string tipo = "SinClasificar";
            bool confirmado = false;

            if (intranet)
            {
                confirmado =
                    string.Equals(
                        movimiento.EstadoValidacion,
                        "Validada",
                        StringComparison.OrdinalIgnoreCase)
                    && EsTipoValido(movimiento.TipoMovimiento);

                if (confirmado)
                    tipo = movimiento.TipoMovimiento;
            }
            else if (biotime)
            {
                const string prefijo = "BIOTIME:";

                if (movimiento.IdMovimiento.StartsWith(
                        prefijo,
                        StringComparison.OrdinalIgnoreCase)
                    && long.TryParse(
                        movimiento.IdMovimiento[prefijo.Length..],
                        out long id)
                    && confirmaciones.TryGetValue(id, out var tipoConfirmado)
                    && EsTipoValido(tipoConfirmado))
                {
                    tipo = tipoConfirmado;
                    confirmado = true;
                }
            }

            resultado.Movimientos.Add(new MovimientoUnificado
            {
                IdMovimiento = movimiento.IdMovimiento,
                Fuente = movimiento.Fuente,
                Origen = movimiento.Origen,
                FechaHoraMexico = movimiento.FechaHoraMexico,
                FechaHoraUtc = movimiento.FechaHoraUtc,
                TipoOriginal = movimiento.TipoMovimiento,
                TipoEvaluacion = tipo,
                EstadoValidacion = movimiento.EstadoValidacion,
                Confirmado = confirmado,
                ParticipaEnEvaluacion = confirmado,
                Observacion = confirmado
                    ? "Movimiento confirmado para conciliación."
                    : "Movimiento pendiente de revisión o clasificación."
            });
        }

        resultado.Movimientos = resultado.Movimientos
            .OrderBy(x => x.FechaHoraUtc)
            .ThenBy(x => x.IdMovimiento)
            .ToList();

        resultado.TotalMovimientos = resultado.Movimientos.Count;

        resultado.MovimientosIntranet = resultado.Movimientos.Count(x =>
            x.Fuente.Equals("INTRANET",
                StringComparison.OrdinalIgnoreCase));

        resultado.MovimientosBioTime = resultado.Movimientos.Count(x =>
            x.Fuente.Equals("BIOTIME",
                StringComparison.OrdinalIgnoreCase));

        if (resultado.TotalMovimientos == 0)
        {
            resultado.Estado = "SinMovimientos";
            resultado.Mensaje =
                "No se encontraron movimientos de asistencia.";
            resultado.RequiereRevision = true;
            return resultado;
        }

        // =====================================================
        // APLICAR RESOLUCIONES ADMINISTRATIVAS VIGENTES
        // =====================================================

        var aplicacion = await _resoluciones.AplicarAsync(
            empleadoId,
            fecha,
            resultado.Movimientos,
            ct);

        resultado.ResolucionesAplicadas =
            aplicacion.ResolucionesAplicadas;

        resultado.Observaciones.AddRange(
            aplicacion.Observaciones);

        var validos = resultado.Movimientos
            .Where(x => x.ParticipaEnEvaluacion)
            .ToList();

        resultado.MovimientosConfirmados = validos.Count;

        resultado.MovimientosPendientes =
            resultado.Movimientos.Count(x => !x.Confirmado);

        resultado.MovimientosExcluidosPorResolucion =
            resultado.Movimientos.Count(x =>
                x.Confirmado && !x.ParticipaEnEvaluacion);

        // =====================================================
        // DETECTAR DUPLICADOS NO RESUELTOS
        // =====================================================

        var conflictos = new List<string>();

        foreach (var tipo in Tipos)
        {
            int cantidad = validos.Count(x =>
                x.TipoEvaluacion == tipo);

            if (cantidad > 1)
            {
                conflictos.Add(
                    $"Existen {cantidad} marcas confirmadas " +
                    $"de tipo {tipo} sin resolución vigente.");
            }
        }

        if (conflictos.Count > 0)
        {
            resultado.Observaciones.AddRange(conflictos);
            resultado.Estado = "ConflictoMovimientos";
            resultado.Mensaje =
                "Existen movimientos duplicados pendientes de conciliación.";
            resultado.RequiereRevision = true;
            return resultado;
        }

        resultado.EntradaReal =
            HoraMovimiento(validos, "Entrada");

        resultado.SalidaComidaReal =
            HoraMovimiento(validos, "SalidaComer");

        resultado.RegresoComidaReal =
            HoraMovimiento(validos, "RegresoComer");

        resultado.SalidaReal =
            HoraMovimiento(validos, "Salida");

        resultado.SecuenciaCompleta =
            resultado.EntradaReal.HasValue &&
            resultado.SalidaComidaReal.HasValue &&
            resultado.RegresoComidaReal.HasValue &&
            resultado.SalidaReal.HasValue;

        if (!resultado.SecuenciaCompleta)
        {
            resultado.Estado = "JornadaIncompleta";
            resultado.Mensaje =
                "No existen cuatro movimientos confirmados " +
                "para una jornada completa.";
            resultado.RequiereRevision = true;

            resultado.Observaciones.Add(
                "Las marcas faltantes no se consideran " +
                "automáticamente ausencias o incumplimientos.");

            return resultado;
        }

        // =====================================================
        // VALIDAR ORDEN CRONOLÓGICO
        // =====================================================

        var secuencia = new[]
        {
            resultado.EntradaReal!.Value,
            resultado.SalidaComidaReal!.Value,
            resultado.RegresoComidaReal!.Value,
            resultado.SalidaReal!.Value
        };

        for (int i = 1; i < secuencia.Length; i++)
        {
            if (secuencia[i] <= secuencia[i - 1])
            {
                resultado.Estado = "SecuenciaInvalida";
                resultado.Mensaje =
                    "Los movimientos confirmados no siguen " +
                    "el orden cronológico esperado.";
                resultado.RequiereRevision = true;
                return resultado;
            }
        }

        resultado.Estado = "CompletaParaRevision";
        resultado.Mensaje =
            "Se encontraron cuatro movimientos confirmados " +
            "en orden cronológico. Pendiente de validación TH.";

        resultado.RequiereRevision = true;

        if (resultado.MovimientosPendientes > 0)
        {
            resultado.Observaciones.Add(
                $"Existen {resultado.MovimientosPendientes} " +
                "movimientos pendientes de revisión.");
        }

        if (resultado.ResolucionesAplicadas > 0)
        {
            resultado.Observaciones.Add(
                $"Se aplicaron {resultado.ResolucionesAplicadas} " +
                "resoluciones administrativas auditadas.");
        }

        return resultado;
    }

    private async Task<Dictionary<long, string>>
        ObtenerConfirmacionesAsync(
            int empleadoId,
            DateTime fecha,
            CancellationToken ct)
    {
        const string sql = """
            SELECT cl.BioTimeChecadaId, cl.TipoMovimiento
            FROM dbo.RC_ClasificacionesBioTime cl
            INNER JOIN dbo.BioTimeChecadas c
                ON c.Id = cl.BioTimeChecadaId
            WHERE cl.EmpleadoId = @EmpleadoId
              AND cl.FechaLocal = @Fecha
              AND cl.Estado = 'Confirmada'
              AND CONVERT(date, c.FechaHora) = @Fecha
              AND EXISTS
              (
                  SELECT 1
                  FROM dbo.RC_EmpleadosBioTime v
                  WHERE v.EmpCode = c.EmpCode
                    AND v.EmpleadoId = cl.EmpleadoId
                    AND v.Estado = 'Validada'
                    AND v.FechaInicio <= @Fecha
                    AND (
                        v.FechaFin IS NULL OR
                        v.FechaFin >= @Fecha
                    )
              );
            """;

        var resultado = new Dictionary<long, string>();

        var conexion = _db.Database.GetDbConnection();
        bool cerrar = conexion.State != ConnectionState.Open;

        try
        {
            if (cerrar)
                await conexion.OpenAsync(ct);

            await using var cmd = conexion.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 30;

            var pEmpleado = cmd.CreateParameter();
            pEmpleado.ParameterName = "@EmpleadoId";
            pEmpleado.DbType = DbType.Int32;
            pEmpleado.Value = empleadoId;
            cmd.Parameters.Add(pEmpleado);

            var pFecha = cmd.CreateParameter();
            pFecha.ParameterName = "@Fecha";
            pFecha.DbType = DbType.Date;
            pFecha.Value = fecha.Date;
            cmd.Parameters.Add(pFecha);

            await using var lector =
                await cmd.ExecuteReaderAsync(ct);

            while (await lector.ReadAsync(ct))
            {
                var id = lector.GetInt64(0);
                var tipo = lector.GetString(1);

                if (!resultado.TryAdd(id, tipo))
                {
                    throw new InvalidOperationException(
                        $"Clasificación duplicada de BioTime {id}.");
                }
            }
        }
        finally
        {
            if (cerrar &&
                conexion.State != ConnectionState.Closed)
                await conexion.CloseAsync();
        }

        return resultado;
    }

    private static bool EsTipoValido(string? tipo)
    {
        return tipo is "Entrada" or "SalidaComer"
            or "RegresoComer" or "Salida";
    }

    private static DateTime? HoraMovimiento(
        IEnumerable<MovimientoUnificado> movimientos,
        string tipo)
    {
        return movimientos
            .Where(x => x.TipoEvaluacion == tipo)
            .OrderBy(x => x.FechaHoraUtc)
            .Select(x => (DateTime?)x.FechaHoraMexico)
            .FirstOrDefault();
    }
}
