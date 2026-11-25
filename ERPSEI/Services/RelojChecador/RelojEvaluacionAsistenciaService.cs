
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojEvaluacionAsistenciaService
{
    private readonly ApplicationDbContext _db;
    private readonly RelojHorariosService _horarios;

    private static readonly TimeZoneInfo ZonaMexico =
        TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

    public RelojEvaluacionAsistenciaService(
        ApplicationDbContext db,
        RelojHorariosService horarios)
    {
        _db = db;
        _horarios = horarios;
    }

    // =========================================================
    // MODELOS DE RESULTADO
    // =========================================================

    public sealed class IncidenciaResultado
    {
        public string Codigo { get; init; } = "";
        public string Descripcion { get; init; } = "";
        public string Severidad { get; init; } = "Revision";
    }

    public sealed class MovimientoResultado
    {
        public long ChecadaId { get; init; }
        public long JornadaId { get; init; }

        public string TipoMovimiento { get; init; } = "";
        public string Origen { get; init; } = "";
        public string EstadoValidacion { get; init; } = "";

        public DateTime FechaHoraUtc { get; init; }
        public DateTime FechaHoraLocal { get; init; }
    }

    public sealed class EvaluacionResultado
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

        public bool? EntradaPuntual { get; set; }
        public bool? RegresoComidaPuntual { get; set; }
        public bool? SalidaEnHorario { get; set; }

        public int ChecadasValidas { get; set; }
        public int ChecadasEsperadas { get; set; } = 4;

        public int? MinutosEfectivos { get; set; }

        public bool RequiereRevision { get; set; }

        public List<MovimientoResultado> Movimientos { get; set; }
            = new();

        public List<IncidenciaResultado> Incidencias { get; set; }
            = new();
    }

    // =========================================================
    // EVALUAR ASISTENCIA DE UN EMPLEADO EN UNA FECHA
    // =========================================================

    public async Task<EvaluacionResultado> EvaluarDiaAsync(
        int empleadoId,
        DateTime fechaLocal,
        CancellationToken cancellationToken = default)
    {
        if (empleadoId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(empleadoId));
        }

        var fecha = fechaLocal.Date;

        // No evaluar fechas futuras.
        var ahoraMexico = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            ZonaMexico);

        if (fecha > ahoraMexico.Date)
        {
            return new EvaluacionResultado
            {
                EmpleadoId = empleadoId,
                FechaLocal = fecha,
                Estado = "FechaFutura",
                Mensaje = "La fecha todavía no ha transcurrido."
            };
        }

        // =====================================================
        // 1. OBTENER HORARIO DE GESTIÓN DE TALENTO
        // =====================================================

        var horario = await _horarios.ObtenerHorarioEmpleadoAsync(
            empleadoId,
            fecha,
            cancellationToken);

        var resultado = new EvaluacionResultado
        {
            EmpleadoId = empleadoId,
            FechaLocal = fecha,
            HorarioId = horario.HorarioId,
            HorarioDescripcion = horario.Descripcion,
            EntradaProgramada = horario.Entrada,
            LimiteRetardo = horario.ToleranciaEntrada,
            SalidaProgramada = horario.Salida,
            Estado = "SinEvaluar"
        };

        if (!horario.Configurado)
        {
            resultado.Estado = "SinConfiguracion";
            resultado.Mensaje = horario.Mensaje;
            resultado.RequiereRevision = true;

            AgregarIncidencia(
                resultado,
                horario.Codigo,
                horario.Mensaje);

            return resultado;
        }

        if (!horario.DiaLaborable)
        {
            resultado.Estado = "NoLaborable";
            resultado.Mensaje =
                "El horario del colaborador no contempla labores en esta fecha.";

            return resultado;
        }

        if (!horario.Entrada.HasValue ||
            !horario.ToleranciaEntrada.HasValue ||
            !horario.Salida.HasValue)
        {
            resultado.Estado = "SinConfiguracion";
            resultado.RequiereRevision = true;
            resultado.Mensaje =
                "Faltan límites de entrada o salida en el horario.";

            AgregarIncidencia(
                resultado,
                "HORARIO_INCOMPLETO",
                resultado.Mensaje);

            return resultado;
        }

        // =====================================================
        // 2. CALCULAR PERIODO UTC DEL DÍA EN MÉXICO
        // =====================================================

        var inicioLocal = DateTime.SpecifyKind(
            fecha,
            DateTimeKind.Unspecified);

        var finLocal = inicioLocal.AddDays(1);

        var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(
            inicioLocal,
            ZonaMexico);

        var finUtc = TimeZoneInfo.ConvertTimeToUtc(
            finLocal,
            ZonaMexico);

        // =====================================================
        // 3. CONSULTAR JORNADAS Y MOVIMIENTOS ORIGINALES
        // =====================================================
        // Incluye las jornadas que iniciaron en el día.
        // Los movimientos se buscan por fecha local,
        // para preservar la separación por días.
        // =====================================================

        var jornadas = await _db.RC_Jornadas
            .AsNoTracking()
            .Where(x =>
                x.EmpleadoId == empleadoId &&
                x.InicioUtc >= inicioUtc &&
                x.InicioUtc < finUtc)
            .OrderBy(x => x.InicioUtc)
            .Select(x => new
            {
                x.Id,
                x.Estado,
                x.MinutosEfectivos,
                x.RequiereRevision,
                x.MotivoRevision
            })
            .ToListAsync(cancellationToken);

        if (jornadas.Count > 1)
        {
            AgregarIncidencia(
                resultado,
                "MULTIPLES_JORNADAS",
                "Existen varias jornadas del colaborador en el mismo día.");
        }

        var jornadaIds = jornadas
            .Select(x => x.Id)
            .ToArray();

        var checadas = await _db.RC_Checadas
            .AsNoTracking()
            .Where(x =>
                jornadaIds.Contains(x.JornadaId) &&
                x.FechaHoraUtc >= inicioUtc &&
                x.FechaHoraUtc < finUtc)
            .OrderBy(x => x.FechaHoraUtc)
            .Select(x => new
            {
                x.Id,
                x.JornadaId,
                x.TipoMovimiento,
                x.Origen,
                x.EstadoValidacion,
                x.FechaHoraUtc
            })
            .ToListAsync(cancellationToken);

        resultado.Movimientos = checadas
            .Select(x => new MovimientoResultado
            {
                ChecadaId = x.Id,
                JornadaId = x.JornadaId,
                TipoMovimiento = x.TipoMovimiento,
                Origen = x.Origen,
                EstadoValidacion = x.EstadoValidacion,
                FechaHoraUtc = x.FechaHoraUtc,
                FechaHoraLocal = ConvertirAMexico(x.FechaHoraUtc)
            })
            .ToList();

        // Solo los registros validados participan
        // en la evaluación definitiva.
        var movimientosValidos = resultado.Movimientos
            .Where(x =>
                string.Equals(
                    x.EstadoValidacion,
                    "Validada",
                    StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.FechaHoraUtc)
            .ToList();

        resultado.ChecadasValidas = movimientosValidos.Count;

        if (checadas.Count > movimientosValidos.Count)
        {
            AgregarIncidencia(
                resultado,
                "CHECADAS_NO_VALIDADAS",
                "Existen movimientos pendientes o no validados que requieren revisión.");
        }

        // =====================================================
        // 4. IDENTIFICAR LOS CUATRO MOVIMIENTOS
        // =====================================================

        resultado.EntradaReal = ObtenerMovimiento(
            movimientosValidos,
            "Entrada");

        resultado.SalidaComidaReal = ObtenerMovimiento(
            movimientosValidos,
            "SalidaComer");

        resultado.RegresoComidaReal = ObtenerMovimiento(
            movimientosValidos,
            "RegresoComer");

        resultado.SalidaReal = ObtenerMovimiento(
            movimientosValidos,
            "Salida");

        ValidarMovimiento(
            resultado,
            movimientosValidos,
            "Entrada",
            "ENTRADA_OMITIDA",
            "No existe una entrada validada.");

        ValidarMovimiento(
            resultado,
            movimientosValidos,
            "SalidaComer",
            "SALIDA_COMIDA_OMITIDA",
            "No existe una salida a comida validada.");

        ValidarMovimiento(
            resultado,
            movimientosValidos,
            "RegresoComer",
            "REGRESO_COMIDA_OMITIDO",
            "No existe un regreso de comida validado.");

        ValidarMovimiento(
            resultado,
            movimientosValidos,
            "Salida",
            "SALIDA_OMITIDA",
            "No existe una salida de labores validada.");

        if (movimientosValidos.Count > 4)
        {
            AgregarIncidencia(
                resultado,
                "CHECADAS_EXCEDENTES",
                "Se registraron más de cuatro movimientos validados.");
        }

        // =====================================================
        // 5. EVALUAR ENTRADA
        // =====================================================
        // ToleranciaEntrada representa el primer
        // minuto de retardo: 09:16, 08:16, 09:01.
        // =====================================================

        if (resultado.EntradaReal.HasValue)
        {
            var horaEntrada = resultado.EntradaReal.Value.TimeOfDay;

            resultado.EntradaPuntual =
                horaEntrada < horario.ToleranciaEntrada.Value;

            if (!resultado.EntradaPuntual.Value)
            {
                AgregarIncidencia(
                    resultado,
                    "RETARDO_ENTRADA",
                    $"Entrada registrada a las " +
                    $"{horaEntrada:hh\\:mm}; " +
                    $"el retardo inicia a las " +
                    $"{horario.ToleranciaEntrada.Value:hh\\:mm}.");
            }
        }

        // =====================================================
        // 6. EVALUAR COMIDA
        // =====================================================
        // Política general:
        // Comida 14:00 a 15:00.
        // Tolerancia hasta 15:05.
        // Retardo a partir de las 15:06.
        //
        // En esta primera versión usamos el horario
        // general de comida para jornadas de cuatro
        // movimientos. Las excepciones se configurarán
        // posteriormente.
        // =====================================================

        var limiteRegresoComida = new TimeSpan(15, 6, 0);

        if (resultado.RegresoComidaReal.HasValue)
        {
            var horaRegreso =
                resultado.RegresoComidaReal.Value.TimeOfDay;

            resultado.RegresoComidaPuntual =
                horaRegreso < limiteRegresoComida;

            if (!resultado.RegresoComidaPuntual.Value)
            {
                AgregarIncidencia(
                    resultado,
                    "REGRESO_COMIDA_TARDIO",
                    $"El regreso de comida fue a las " +
                    $"{horaRegreso:hh\\:mm}; " +
                    "la tolerancia termina a las 15:05.");
            }
        }

        if (resultado.SalidaComidaReal.HasValue &&
            resultado.RegresoComidaReal.HasValue)
        {
            if (resultado.RegresoComidaReal.Value <=
                resultado.SalidaComidaReal.Value)
            {
                AgregarIncidencia(
                    resultado,
                    "SECUENCIA_COMIDA_INVALIDA",
                    "El regreso de comida no es posterior a la salida.");
            }
        }

        // =====================================================
        // 7. EVALUAR SALIDA
        // =====================================================
        // Se compara con la salida programada.
        // ToleranciaSalida se conserva como dato
        // del horario, pero no se interpreta todavía
        // como permiso de salida anticipada.
        // =====================================================

        if (resultado.SalidaReal.HasValue)
        {
            var horaSalida = resultado.SalidaReal.Value.TimeOfDay;

            resultado.SalidaEnHorario =
                horaSalida >= horario.Salida.Value;

            if (!resultado.SalidaEnHorario.Value)
            {
                AgregarIncidencia(
                    resultado,
                    "SALIDA_ANTICIPADA",
                    $"Salida registrada a las " +
                    $"{horaSalida:hh\\:mm}; " +
                    $"horario programado: " +
                    $"{horario.Salida.Value:hh\\:mm}.");
            }
        }

        // =====================================================
        // 8. VALIDAR SECUENCIA COMPLETA
        // =====================================================

        var secuencia = new DateTime?[]
        {
            resultado.EntradaReal,
            resultado.SalidaComidaReal,
            resultado.RegresoComidaReal,
            resultado.SalidaReal
        };

        if (secuencia.All(x => x.HasValue))
        {
            for (int i = 1; i < secuencia.Length; i++)
            {
                if (secuencia[i]!.Value <=
                    secuencia[i - 1]!.Value)
                {
                    AgregarIncidencia(
                        resultado,
                        "SECUENCIA_INVALIDA",
                        "Las checadas no siguen el orden esperado.");

                    break;
                }
            }
        }

        // =====================================================
        // 9. RECUPERAR HORAS EFECTIVAS
        // =====================================================
        // No recalculamos ni sobrescribimos
        // RC_Jornada.MinutosEfectivos.
        // =====================================================

        if (jornadas.Count == 1)
        {
            var jornada = jornadas[0];

            resultado.MinutosEfectivos =
                jornada.MinutosEfectivos;

            if (jornada.RequiereRevision)
            {
                AgregarIncidencia(
                    resultado,
                    "REVISION_JORNADA",
                    jornada.MotivoRevision ??
                    "La jornada fue marcada para revisión.");
            }

            if (jornada.Estado == "Abierta")
            {
                AgregarIncidencia(
                    resultado,
                    "JORNADA_ABIERTA",
                    "La jornada todavía no se ha cerrado.");
            }
        }

        // =====================================================
        // 10. RESULTADO FINAL
        // =====================================================

        resultado.RequiereRevision =
            resultado.Incidencias.Count > 0;

        // La jornada actual puede estar en curso.
        // No clasificarla como incumplimiento definitivo.
        if (fecha == ahoraMexico.Date &&
            ahoraMexico.TimeOfDay < horario.Salida.Value)
        {
            resultado.Estado = "EnCurso";
            resultado.Mensaje =
                "Evaluación provisional: la jornada todavía puede estar en curso.";

            return resultado;
        }

        resultado.Estado = resultado.RequiereRevision
            ? "RequiereRevision"
            : "Conforme";

        resultado.Mensaje = resultado.RequiereRevision
            ? "La asistencia presenta observaciones para revisión."
            : "La asistencia cumple las validaciones configuradas.";

        return resultado;
    }

    // =========================================================
    // FUNCIONES AUXILIARES
    // =========================================================

    private static DateTime ConvertirAMexico(DateTime valorUtc)
    {
        // SQL Server datetime2 suele devolver
        // DateTimeKind.Unspecified.
        var utc = DateTime.SpecifyKind(
            valorUtc,
            DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(
            utc,
            ZonaMexico);
    }

    private static DateTime? ObtenerMovimiento(
        List<MovimientoResultado> movimientos,
        string tipo)
    {
        return movimientos
            .Where(x => string.Equals(
                x.TipoMovimiento,
                tipo,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.FechaHoraUtc)
            .Select(x => (DateTime?)x.FechaHoraLocal)
            .FirstOrDefault();
    }

    private static void ValidarMovimiento(
        EvaluacionResultado resultado,
        List<MovimientoResultado> movimientos,
        string tipo,
        string codigoOmision,
        string mensajeOmision)
    {
        int cantidad = movimientos.Count(x =>
            string.Equals(
                x.TipoMovimiento,
                tipo,
                StringComparison.OrdinalIgnoreCase));

        if (cantidad == 0)
        {
            AgregarIncidencia(
                resultado,
                codigoOmision,
                mensajeOmision);
        }
        else if (cantidad > 1)
        {
            AgregarIncidencia(
                resultado,
                "MOVIMIENTO_DUPLICADO",
                $"Existen {cantidad} movimientos del tipo {tipo}.");
        }
    }

    private static void AgregarIncidencia(
        EvaluacionResultado resultado,
        string codigo,
        string descripcion)
    {
        if (resultado.Incidencias.Any(x =>
            x.Codigo == codigo &&
            x.Descripcion == descripcion))
        {
            return;
        }

        resultado.Incidencias.Add(new IncidenciaResultado
        {
            Codigo = codigo,
            Descripcion = descripcion,
            Severidad = "Revision"
        });
    }
}
