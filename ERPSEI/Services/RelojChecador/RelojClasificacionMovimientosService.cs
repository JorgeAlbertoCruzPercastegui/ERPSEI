
using ERPSEI.Services.RelojChecador.Models;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojClasificacionMovimientosService
{
    private readonly AsistenciaMovimientosService _movimientos;
    private readonly RelojHorariosService _horarios;

    private static readonly TimeZoneInfo ZonaMexico =
        TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

    public RelojClasificacionMovimientosService(
        AsistenciaMovimientosService movimientos,
        RelojHorariosService horarios)
    {
        _movimientos = movimientos;
        _horarios = horarios;
    }

    public sealed class MovimientoClasificado
    {
        public string IdMovimiento { get; init; } = "";
        public string Fuente { get; init; } = "";
        public string Origen { get; init; } = "";
        public string TipoOriginal { get; init; } = "";
        public string TipoPropuesto { get; set; } = "";
        public string EstadoOriginal { get; init; } = "";
        public DateTime FechaHoraMexico { get; init; }
        public DateTime FechaHoraUtc { get; init; }
        public string Observacion { get; set; } = "";
    }

    public sealed class ClasificacionResultado
    {
        public int EmpleadoId { get; init; }
        public DateTime FechaLocal { get; init; }

        public string Estado { get; set; } = "SinEvaluar";
        public string Mensaje { get; set; } = "";

        public int? HorarioId { get; init; }
        public string? HorarioDescripcion { get; init; }

        public TimeSpan? EntradaProgramada { get; init; }
        public TimeSpan? SalidaProgramada { get; init; }

        public int TotalMovimientos { get; set; }
        public int TotalBioTime { get; set; }
        public int TotalIntranet { get; set; }

        public bool PropuestaCompleta { get; set; }
        public bool RequiereRevision { get; set; }

        public List<string> Observaciones { get; set; } = new();

        public List<MovimientoClasificado> Movimientos { get; set; }
            = new();
    }

    // =========================================================
    // CLASIFICACIÓN PRELIMINAR DE UN DÍA
    // =========================================================

    public async Task<ClasificacionResultado>
        ClasificarDiaAsync(
            int empleadoId,
            DateTime fechaLocal,
            CancellationToken cancellationToken = default)
    {
        if (empleadoId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(empleadoId));
        }

        DateTime fecha = fechaLocal.Date;

        var ahoraMexico = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            ZonaMexico);

        var horario = await _horarios.ObtenerHorarioEmpleadoAsync(
            empleadoId,
            fecha,
            cancellationToken);

        var resultado = new ClasificacionResultado
        {
            EmpleadoId = empleadoId,
            FechaLocal = fecha,
            HorarioId = horario.HorarioId,
            HorarioDescripcion = horario.Descripcion,
            EntradaProgramada = horario.Entrada,
            SalidaProgramada = horario.Salida
        };

        if (fecha > ahoraMexico.Date)
        {
            resultado.Estado = "FechaFutura";
            resultado.Mensaje =
                "No se clasifican movimientos de fechas futuras.";

            return resultado;
        }

        // Consultar los proveedores registrados sin escribir
        // ni modificar movimientos originales.
        var movimientos = await _movimientos.ObtenerMovimientosAsync(
            empleadoId,
            fecha,
            fecha,
            cancellationToken);

        resultado.Movimientos = movimientos
            .OrderBy(x => x.FechaHoraUtc)
            .ThenBy(x => x.IdMovimiento)
            .Select(x => new MovimientoClasificado
            {
                IdMovimiento = x.IdMovimiento,
                Fuente = x.Fuente,
                Origen = x.Origen,
                TipoOriginal = x.TipoMovimiento,
                TipoPropuesto = "SinClasificar",
                EstadoOriginal = x.EstadoValidacion,
                FechaHoraMexico = x.FechaHoraMexico,
                FechaHoraUtc = x.FechaHoraUtc,
                Observacion = ""
            })
            .ToList();

        resultado.TotalMovimientos =
            resultado.Movimientos.Count;

        resultado.TotalBioTime =
            resultado.Movimientos.Count(x =>
                EsFuente(x, "BIOTIME"));

        resultado.TotalIntranet =
            resultado.Movimientos.Count(x =>
                EsFuente(x, "INTRANET"));

        if (resultado.TotalMovimientos == 0)
        {
            resultado.Estado = "SinMovimientos";
            resultado.Mensaje =
                "No existen movimientos disponibles para esta fecha.";
            return resultado;
        }

        if (!horario.Configurado ||
            !horario.DiaLaborable ||
            !horario.Entrada.HasValue ||
            !horario.Salida.HasValue)
        {
            MarcarRevision(
                resultado,
                "HorarioNoAplicable",
                "No existe un horario laboral aplicable: " +
                horario.Mensaje);

            return resultado;
        }

        // Si existen movimientos de ambas fuentes,
        // no es seguro inferir el orden de la jornada.
        if (resultado.TotalIntranet > 0 &&
            resultado.TotalBioTime > 0)
        {
            MarcarRevision(
                resultado,
                "FuentesMixtas",
                "Se detectaron movimientos digitales y biométricos. " +
                "Es necesario conciliarlos antes de clasificarlos.");

            return resultado;
        }

        // Los movimientos digitales ya están tipificados.
        // Este servicio NO modifica ni reinterpreta
        // sus clasificaciones originales.
        if (resultado.TotalIntranet > 0)
        {
            resultado.Estado = "MovimientosDigitales";
            resultado.Mensaje =
                "Los movimientos digitales conservan " +
                "su clasificación y validación originales.";

            foreach (var movimiento in resultado.Movimientos)
            {
                movimiento.TipoPropuesto =
                    movimiento.TipoOriginal;

                movimiento.Observacion =
                    "Clasificación original de INTRANET.";
            }

            return resultado;
        }

        // Trabajar exclusivamente con BIOTIME desde aquí.
        var biometrico = resultado.Movimientos
            .Where(x => EsFuente(x, "BIOTIME"))
            .OrderBy(x => x.FechaHoraUtc)
            .ToList();

        // No clasificamos jornadas incompletas mediante
        // la posición de las marcas.
        if (biometrico.Count != 4)
        {
            MarcarRevision(
                resultado,
                "CantidadAmbigua",
                $"Se encontraron {biometrico.Count} checadas " +
                "biométricas. Para una propuesta completa " +
                "se requieren cuatro marcas verificables.");

            return resultado;
        }

        // Checadas demasiado cercanas pueden ser reintentos,
        // registros duplicados o eventos distintos.
        // No descartamos ninguno automáticamente.
        for (int i = 1; i < biometrico.Count; i++)
        {
            var separacion =
                biometrico[i].FechaHoraUtc -
                biometrico[i - 1].FechaHoraUtc;

            if (separacion < TimeSpan.FromMinutes(5))
            {
                MarcarRevision(
                    resultado,
                    "ChecadasCercanas",
                    "Existen dos checadas separadas por menos " +
                    "de cinco minutos; requieren revisión.");

                return resultado;
            }
        }

        var primera = biometrico[0].FechaHoraMexico.TimeOfDay;
        var segunda = biometrico[1].FechaHoraMexico.TimeOfDay;
        var tercera = biometrico[2].FechaHoraMexico.TimeOfDay;
        var cuarta = biometrico[3].FechaHoraMexico.TimeOfDay;

        var entradaProgramada = horario.Entrada.Value;
        var salidaProgramada = horario.Salida.Value;

        // Ventanas preliminares para jornadas diurnas.
        // No equivalen a tolerancias laborales.
        //
        // Entrada: hasta 2 h antes/después de la programada.
        // Salida a comida: 12:30 a 15:00.
        // Regreso: 14:15 a 16:30.
        // Salida final: hasta 90 min antes o 4 h después.
        //
        // Solo se propone si se cumple toda la secuencia.

        bool entradaCompatible =
            EnRango(
                primera,
                entradaProgramada -
                    TimeSpan.FromHours(2),
                entradaProgramada +
                    TimeSpan.FromHours(2));

        bool comidaCompatible =
            EnRango(
                segunda,
                new TimeSpan(12, 30, 0),
                new TimeSpan(15, 0, 0));

        bool regresoCompatible =
            EnRango(
                tercera,
                new TimeSpan(14, 15, 0),
                new TimeSpan(16, 30, 0));

        bool salidaCompatible =
            EnRango(
                cuarta,
                salidaProgramada -
                    TimeSpan.FromMinutes(90),
                salidaProgramada +
                    TimeSpan.FromHours(4));

        bool secuenciaCorrecta =
            primera < segunda &&
            segunda < tercera &&
            tercera < cuarta;

        bool duracionComidaRazonable =
            tercera - segunda >= TimeSpan.FromMinutes(20) &&
            tercera - segunda <= TimeSpan.FromHours(2);

        // Evitar proponer movimientos antes de que
        // haya concluido el horario de la jornada actual.
        if (fecha == ahoraMexico.Date &&
            ahoraMexico.TimeOfDay < salidaProgramada)
        {
            MarcarRevision(
                resultado,
                "JornadaEnCurso",
                "La jornada actual todavía puede estar en curso.");

            return resultado;
        }

        if (!entradaCompatible ||
            !comidaCompatible ||
            !regresoCompatible ||
            !salidaCompatible ||
            !secuenciaCorrecta ||
            !duracionComidaRazonable)
        {
            MarcarRevision(
                resultado,
                "SecuenciaAmbigua",
                "Las cuatro checadas no cumplen las ventanas " +
                "preliminares para una clasificación confiable.");

            return resultado;
        }

        // =====================================================
        // PROPONER CLASIFICACIÓN
        // =====================================================
        //
        // No se actualiza ninguna tabla.
        // No se cambia EstadoValidacion a Validada.
        // No se generan incidencias.
        // =====================================================

        var tipos = new[]
        {
            "Entrada",
            "SalidaComer",
            "RegresoComer",
            "Salida"
        };

        for (int i = 0; i < biometrico.Count; i++)
        {
            biometrico[i].TipoPropuesto = tipos[i];

            biometrico[i].Observacion =
                "Clasificación preliminar sujeta " +
                "a confirmación de Talento Humano.";
        }

        resultado.PropuestaCompleta = true;
        resultado.RequiereRevision = true;

        resultado.Estado = "PropuestaCompleta";
        resultado.Mensaje =
            "Se identificó una secuencia biométrica compatible " +
            "con cuatro movimientos. La propuesta aún no " +
            "está validada para cálculo laboral.";

        resultado.Observaciones.Add(
            "La clasificación es informativa. " +
            "No constituye una determinación de asistencia.");

        return resultado;
    }

    private static bool EsFuente(
        MovimientoClasificado movimiento,
        string fuente)
    {
        return string.Equals(
            movimiento.Fuente,
            fuente,
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool EnRango(
        TimeSpan hora,
        TimeSpan minimo,
        TimeSpan maximo)
    {
        if (minimo < TimeSpan.Zero ||
            maximo >= TimeSpan.FromDays(1))
        {
            return false;
        }

        return hora >= minimo && hora <= maximo;
    }

    private static void MarcarRevision(
        ClasificacionResultado resultado,
        string codigo,
        string mensaje)
    {
        resultado.Estado = codigo;
        resultado.Mensaje = mensaje;
        resultado.RequiereRevision = true;

        resultado.Observaciones.Add(mensaje);

        foreach (var movimiento in resultado.Movimientos)
        {
            movimiento.Observacion =
                "Movimiento pendiente de clasificación.";
        }
    }
}
