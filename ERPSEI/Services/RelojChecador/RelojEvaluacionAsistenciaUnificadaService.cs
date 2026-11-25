
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojEvaluacionAsistenciaUnificadaService
{
    private readonly ApplicationDbContext _db;
    private readonly AsistenciaMovimientosService _movimientos;
    private readonly RelojHorariosService _horarios;
    private readonly RelojResolucionConflictosService _resoluciones;
    private readonly RelojAsistenciaContextoTHService _contextoTH;

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
        RelojResolucionConflictosService resoluciones,
        RelojAsistenciaContextoTHService contextoTH)
    {
        _db = db;
        _movimientos = movimientos;
        _horarios = horarios;
        _resoluciones = resoluciones;
        _contextoTH = contextoTH;
    }


    private async Task<ResultadoUnificado> CompletarEvaluacionTHAsync(
        ResultadoUnificado resultado,
        CancellationToken ct,
        bool conflictosSinResolver = false)
    {
        // El contexto se consulta después de consolidar
        // las checadas y aplicar las resoluciones vigentes.
        // Ambos servicios utilizan el mismo DbContext,
        // por lo que las consultas son secuenciales.

        var contexto = await _contextoTH.ObtenerAsync(
            resultado.EmpleadoId,
            resultado.FechaLocal,
            ct);

        resultado.ContextoTH = contexto;

        // No se concede una justificación automática.
        // Cualquier solicitud o autorización que coincida
        // con la fecha deberá conservar su trazabilidad.

        bool contextoRequiereRevision =
            contexto.RequiereRevisionTH ||
            contexto.TieneSolicitudesAusencia ||
            contexto.HomeOfficeAutorizado ||
            contexto.Autorizaciones.Any(x =>
                !string.Equals(
                    x.Estado,
                    "Aprobada",
                    StringComparison.OrdinalIgnoreCase));

        // Primero se mantiene la evaluación técnica
        // de los movimientos confirmados.

        CompletarEvaluacionTH(
            resultado,
            conflictosSinResolver);

        // La evaluación preliminar TH permanece bloqueada
        // para cierres automáticos.

        if (contextoRequiereRevision)
        {
            resultado.RequiereRevision = true;

            resultado.Observaciones.Add(
                "Se identificó contexto de Talento Humano " +
                "que debe considerarse durante la revisión " +
                "de la asistencia.");
        }

        foreach (var observacion in contexto.Observaciones)
        {
            if (!resultado.Observaciones.Contains(observacion))
            {
                resultado.Observaciones.Add(observacion);
            }
        }

        return resultado;
    }



    // =========================================================
    // MODELO DE MOVIMIENTOS UNIFICADOS
    // =========================================================

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

        // Puede modificarse exclusivamente en memoria
        // mediante la resolución administrativa.
        public bool ParticipaEnEvaluacion { get; set; }

        public string Observacion { get; set; } = "";
    }

    // =========================================================
    // RESULTADO UNIFICADO
    // =========================================================

    public sealed class ResultadoUnificado
    {
        public int EmpleadoId { get; init; }
        public DateTime FechaLocal { get; init; }

        // Se conservan los estados técnicos anteriores
        // para no romper el dashboard y la conciliación.
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

        // =====================================================
        // NUEVO: RESULTADO PRELIMINAR DE TALENTO HUMANO
        // =====================================================
        //
        // Null en fecha futura, día no laborable o cuando
        // el horario no está configurado.
        //
        // No sustituye el Estado técnico existente.
        // No autoriza incidencias ni descuentos de nómina.

        public RelojReglasEvaluacionTH.Resultado? EvaluacionTH
        {
            get;
            set;
        }

        public RelojAsistenciaContextoTHService.Resultado? ContextoTH
        {
            get;
            set;
        }

    }

    // =========================================================
    // EVALUACIÓN PRINCIPAL POR EMPLEADO Y FECHA
    // =========================================================

    public async Task<ResultadoUnificado> EvaluarDiaAsync(
        int empleadoId,
        DateTime fechaLocal,
        CancellationToken ct = default)
    {
        if (empleadoId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(empleadoId));
        }

        var fecha = fechaLocal.Date;

        var hoyMexico = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            ZonaMexico).Date;

        // =====================================================
        // 1. VALIDAR FECHA
        // =====================================================

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

        // =====================================================
        // 2. CONSULTAR HORARIO ASIGNADO
        // =====================================================

        var horario =
            await _horarios.ObtenerHorarioEmpleadoAsync(
                empleadoId,
                fecha,
                ct);

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

        // No se ejecuta una evaluación laboral ordinaria
        // si no existe horario aplicable o no es laborable.

        if (!horario.Configurado || !horario.DiaLaborable)
        {
            resultado.Estado = horario.Configurado
                ? "NoLaborable"
                : "SinConfiguracion";

            resultado.Mensaje = horario.Mensaje;

            resultado.RequiereRevision =
                !horario.Configurado;

            return resultado;
        }

        // =====================================================
        // 3. OBTENER MOVIMIENTOS NORMALIZADOS
        // =====================================================

        var normalizados =
            await _movimientos.ObtenerMovimientosAsync(
                empleadoId,
                fecha,
                fecha,
                ct);

        // Confirmaciones existentes de BioTime.
        // Solo se aceptan si siguen vinculadas
        // válidamente al empleado correspondiente.

        var confirmaciones =
            await ObtenerConfirmacionesAsync(
                empleadoId,
                fecha,
                ct);

        // =====================================================
        // 4. CONSTRUIR MOVIMIENTOS UNIFICADOS
        // =====================================================

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

            // ===============================================
            // MOVIMIENTOS DIGITALES INTRANET
            // ===============================================

            if (intranet)
            {
                confirmado = string.Equals(
                    movimiento.EstadoValidacion,
                    "Validada",
                    StringComparison.OrdinalIgnoreCase)
                    && EsTipoValido(
                        movimiento.TipoMovimiento);

                if (confirmado)
                {
                    tipo = movimiento.TipoMovimiento;
                }
            }

            // ===============================================
            // MOVIMIENTOS BIOMÉTRICOS BIOTIME
            // ===============================================

            else if (biotime)
            {
                const string prefijo = "BIOTIME:";

                if (movimiento.IdMovimiento.StartsWith(
                        prefijo,
                        StringComparison.OrdinalIgnoreCase)
                    && long.TryParse(
                        movimiento.IdMovimiento[
                            prefijo.Length..],
                        out long id)
                    && confirmaciones.TryGetValue(
                        id,
                        out var tipoConfirmado)
                    && EsTipoValido(tipoConfirmado))
                {
                    tipo = tipoConfirmado;
                    confirmado = true;
                }
            }

            resultado.Movimientos.Add(
                new MovimientoUnificado
                {
                    IdMovimiento = movimiento.IdMovimiento,
                    Fuente = movimiento.Fuente,
                    Origen = movimiento.Origen,

                    FechaHoraMexico =
                        movimiento.FechaHoraMexico,

                    FechaHoraUtc =
                        movimiento.FechaHoraUtc,

                    TipoOriginal =
                        movimiento.TipoMovimiento,

                    TipoEvaluacion = tipo,

                    EstadoValidacion =
                        movimiento.EstadoValidacion,

                    Confirmado = confirmado,

                    ParticipaEnEvaluacion =
                        confirmado,

                    Observacion = confirmado
                        ? "Movimiento confirmado para conciliación."
                        : "Movimiento pendiente de revisión o clasificación."
                });
        }

        // =====================================================
        // 5. ORDENAR MOVIMIENTOS Y CALCULAR TOTALES
        // =====================================================

        resultado.Movimientos = resultado.Movimientos
            .OrderBy(x => x.FechaHoraUtc)
            .ThenBy(x => x.IdMovimiento)
            .ToList();

        resultado.TotalMovimientos =
            resultado.Movimientos.Count;

        resultado.MovimientosIntranet =
            resultado.Movimientos.Count(x =>
                x.Fuente.Equals(
                    "INTRANET",
                    StringComparison.OrdinalIgnoreCase));

        resultado.MovimientosBioTime =
            resultado.Movimientos.Count(x =>
                x.Fuente.Equals(
                    "BIOTIME",
                    StringComparison.OrdinalIgnoreCase));

        // =====================================================
        // 6. VALIDAR AUSENCIA DE MOVIMIENTOS
        // =====================================================

        if (resultado.TotalMovimientos == 0)
        {
            resultado.Estado = "SinMovimientos";

            resultado.Mensaje =
                "No se encontraron movimientos de asistencia.";

            resultado.RequiereRevision = true;

            // NUEVO:
            // El resultado TH indicará SinMovimientos,
            // sin declarar automáticamente una falta.

            return CompletarEvaluacionTH(resultado);
        }

        // =====================================================
        // 7. APLICAR RESOLUCIONES ADMINISTRATIVAS VIGENTES
        // =====================================================

        var aplicacion =
            await _resoluciones.AplicarAsync(
                empleadoId,
                fecha,
                resultado.Movimientos,
                ct);

        resultado.ResolucionesAplicadas =
            aplicacion.ResolucionesAplicadas;

        resultado.Observaciones.AddRange(
            aplicacion.Observaciones);

        // Solo los movimientos que participan
        // deben utilizarse para construir la jornada.

        var validos = resultado.Movimientos
            .Where(x => x.ParticipaEnEvaluacion)
            .ToList();

        resultado.MovimientosConfirmados =
            validos.Count;

        resultado.MovimientosPendientes =
            resultado.Movimientos.Count(x =>
                !x.Confirmado);

        resultado.MovimientosExcluidosPorResolucion =
            resultado.Movimientos.Count(x =>
                x.Confirmado &&
                !x.ParticipaEnEvaluacion);

        // =====================================================
        // 8. DETECTAR DUPLICADOS SIN RESOLVER
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
            resultado.Observaciones.AddRange(
                conflictos);

            resultado.Estado =
                "ConflictoMovimientos";

            resultado.Mensaje =
                "Existen movimientos duplicados pendientes de conciliación.";

            resultado.RequiereRevision = true;

            // NUEVO:
            // La evaluación TH reconoce que existe
            // un conflicto y no autoriza cerrar
            // automáticamente la jornada.

            return CompletarEvaluacionTH(
                resultado,
                conflictosSinResolver: true);
        }

        // =====================================================
        // 9. EXTRAER HORAS REALES DE MOVIMIENTOS VÁLIDOS
        // =====================================================

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

        // =====================================================
        // 10. VALIDAR JORNADA INCOMPLETA
        // =====================================================

        if (!resultado.SecuenciaCompleta)
        {
            resultado.Estado =
                "JornadaIncompleta";

            resultado.Mensaje =
                "No existen cuatro movimientos confirmados " +
                "para una jornada completa.";

            resultado.RequiereRevision = true;

            resultado.Observaciones.Add(
                "Las marcas faltantes no se consideran " +
                "automáticamente ausencias o incumplimientos.");

            // NUEVO:
            // Aunque falten movimientos, podremos
            // evaluar la puntualidad si existe
            // una entrada confirmada.

            return CompletarEvaluacionTH(resultado);
        }

        // =====================================================
        // 11. VALIDAR ORDEN CRONOLÓGICO
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
                resultado.Estado =
                    "SecuenciaInvalida";

                resultado.Mensaje =
                    "Los movimientos confirmados no siguen " +
                    "el orden cronológico esperado.";

                resultado.RequiereRevision = true;

                // NUEVO:
                // La regla TH devuelve
                // SecuenciaInconsistente, manteniendo
                // el estado técnico anterior.

                return CompletarEvaluacionTH(resultado);
            }
        }

        // =====================================================
        // 12. JORNADA COMPLETA PARA REVISIÓN
        // =====================================================

        resultado.Estado =
            "CompletaParaRevision";

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

        // NUEVO:
        // Calcula puntualidad, jornada y salida.
        // No modifica los movimientos ni el estado
        // técnico que esperan los módulos existentes.

        return CompletarEvaluacionTH(resultado);
    }

    // =========================================================
    // NUEVO: EVALUACIÓN PRELIMINAR DE TALENTO HUMANO
    // =========================================================

    private static ResultadoUnificado CompletarEvaluacionTH(
        ResultadoUnificado resultado,
        bool conflictosSinResolver = false)
    {
        var datos = new RelojReglasEvaluacionTH.DatosJornada
        {
            EntradaProgramada =
                resultado.EntradaProgramada,

            LimiteRetardo =
                resultado.LimiteRetardo,

            SalidaProgramada =
                resultado.SalidaProgramada,

            EntradaReal =
                resultado.EntradaReal,

            SalidaComerReal =
                resultado.SalidaComidaReal,

            RegresoComerReal =
                resultado.RegresoComidaReal,

            SalidaReal =
                resultado.SalidaReal,

            ConflictosSinResolver =
                conflictosSinResolver,

            // El contexto de ausencias, vacaciones
            // y Home Office no se ha integrado aún
            // en este evaluador.
            //
            // No declarar que está revisado ni
            // generar conclusiones sobre esos datos.


            ContextoTHRequiereRevision =
            resultado.ContextoTH?.RequiereRevisionTH == true ||
            resultado.ContextoTH?.TieneSolicitudesAusencia == true ||
            resultado.ContextoTH?.HomeOfficeAutorizado == true ||
            resultado.ContextoTH?.Autorizaciones.Any(x =>
        !string.Equals(
            x.Estado,
            "Aprobada",
            StringComparison.OrdinalIgnoreCase)) == true

        };

        var evaluacion =
            RelojReglasEvaluacionTH.Evaluar(datos);

        resultado.EvaluacionTH = evaluacion;

        // La regla TH todavía no permite
        // cierre automático de jornadas.

        if (evaluacion.RequiereRevisionTH)
        {
            resultado.RequiereRevision = true;
        }

        // Se conserva resultado.Estado para
        // compatibilidad con dashboard y conciliación.

        return resultado;
    }

    // =========================================================
    // OBTENER CLASIFICACIONES BIOTIME CONFIRMADAS
    // =========================================================

    private async Task<Dictionary<long, string>>
        ObtenerConfirmacionesAsync(
            int empleadoId,
            DateTime fecha,
            CancellationToken ct)
    {
        const string sql = """
            SELECT
                cl.BioTimeChecadaId,
                cl.TipoMovimiento
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
                        v.FechaFin IS NULL
                        OR v.FechaFin >= @Fecha
                    )
              );
            """;

        var resultado =
            new Dictionary<long, string>();

        var conexion =
            _db.Database.GetDbConnection();

        bool cerrar =
            conexion.State != ConnectionState.Open;

        try
        {
            if (cerrar)
            {
                await conexion.OpenAsync(ct);
            }

            await using var cmd =
                conexion.CreateCommand();

            cmd.CommandText = sql;
            cmd.CommandTimeout = 30;

            var pEmpleado =
                cmd.CreateParameter();

            pEmpleado.ParameterName =
                "@EmpleadoId";

            pEmpleado.DbType =
                DbType.Int32;

            pEmpleado.Value =
                empleadoId;

            cmd.Parameters.Add(pEmpleado);

            var pFecha =
                cmd.CreateParameter();

            pFecha.ParameterName =
                "@Fecha";

            pFecha.DbType =
                DbType.Date;

            pFecha.Value =
                fecha.Date;

            cmd.Parameters.Add(pFecha);

            await using var lector =
                await cmd.ExecuteReaderAsync(ct);

            while (await lector.ReadAsync(ct))
            {
                var id =
                    lector.GetInt64(0);

                var tipo =
                    lector.GetString(1);

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
            {
                await conexion.CloseAsync();
            }
        }

        return resultado;
    }

    // =========================================================
    // VALIDAR TIPOS DE MOVIMIENTO
    // =========================================================

    private static bool EsTipoValido(string? tipo)
    {
        return RelojReglasConciliacion.EsTipoValido(
            tipo);
    }

    // =========================================================
    // OBTENER HORA LOCAL DEL MOVIMIENTO CONFIRMADO
    // =========================================================

    private static DateTime? HoraMovimiento(
        IEnumerable<MovimientoUnificado> movimientos,
        string tipo)
    {
        return movimientos
            .Where(x =>
                x.TipoEvaluacion == tipo)
            .OrderBy(x => x.FechaHoraUtc)
            .Select(x =>
                (DateTime?)x.FechaHoraMexico)
            .FirstOrDefault();
    }
}
