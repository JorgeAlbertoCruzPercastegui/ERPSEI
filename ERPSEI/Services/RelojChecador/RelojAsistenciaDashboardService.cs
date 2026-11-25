
using System.Data;
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojAsistenciaDashboardService
{
    private readonly ApplicationDbContext _db;
    private readonly RelojEvaluacionAsistenciaUnificadaService _evaluador;
    private readonly RelojAsistenciaContextoTHService _contextoTH;

    private static readonly TimeZoneInfo ZonaMexico =
        TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

    public RelojAsistenciaDashboardService(
        ApplicationDbContext db,
        RelojEvaluacionAsistenciaUnificadaService evaluador,
        RelojAsistenciaContextoTHService contextoTH)
    {
        _db = db;
        _evaluador = evaluador;
        _contextoTH = contextoTH;
    }

    public sealed class Filtros
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int? EmpleadoId { get; set; }
        public string? Buscar { get; set; }
        public int Pagina { get; set; } = 1;
        public int Tamano { get; set; } = 10;
    }

    public sealed class JornadaResumen
    {
        public int EmpleadoId { get; init; }
        public string Colaborador { get; init; } = "";
        public DateTime Fecha { get; init; }

        public string Horario { get; init; } = "";
        public string Estado { get; init; } = "";

        public string? Entrada { get; init; }
        public string? SalidaComer { get; init; }
        public string? RegresoComer { get; init; }
        public string? Salida { get; init; }

        public bool? EntradaPuntual { get; init; }
        public bool? RegresoComidaPuntual { get; init; }
        public bool? SalidaEnHorario { get; init; }

        public int? MinutosEfectivos { get; init; }

        public int MovimientosIntranet { get; init; }
        public int MovimientosBioTime { get; init; }
        public int Confirmados { get; init; }
        public int Pendientes { get; init; }

        public bool HomeOfficeAutorizado { get; init; }
        public bool TieneAusencias { get; init; }
        public bool RequiereRevision { get; init; }

        public string Observaciones { get; init; } = "";

        public string IntegridadChecadas { get; init; } = "NoEvaluable";
        public string CumplimientoHorario { get; init; } = "NoEvaluable";

        public int? MinutosRetardoPotencial { get; init; }
        public int? MinutosSalidaAnticipadaPotencial { get; init; }

    }

    public sealed class ResumenDashboard
    {
        public int TotalJornadas { get; init; }
        public int Pagina { get; init; }
        public int Tamano { get; init; }
        public int TotalPaginas { get; init; }

        public int JornadasPagina { get; init; }
        public int JornadasCompletasPagina { get; init; }
        public int JornadasRevisionPagina { get; init; }
        public int MovimientosPendientesPagina { get; init; }

        public List<JornadaResumen> Registros { get; init; }
            = new();
    }

    private sealed record DiaEmpleado(
        int EmpleadoId,
        DateTime Fecha);

    public async Task<ResumenDashboard> ConsultarAsync(
        Filtros filtros,
        CancellationToken ct = default)
    {
        ValidarFiltros(filtros);

        DateTime inicio = filtros.FechaInicio.Date;
        DateTime fin = filtros.FechaFin.Date;

        // 1. Leer días con movimientos digitales.
        var dias = new HashSet<DiaEmpleado>();

        var inicioUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                inicio,
                DateTimeKind.Unspecified),
            ZonaMexico);

        var finUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                fin.AddDays(1),
                DateTimeKind.Unspecified),
            ZonaMexico);

        var movimientosDigitales = await _db.RC_Checadas
            .AsNoTracking()
            .Where(c =>
                c.FechaHoraUtc >= inicioUtc &&
                c.FechaHoraUtc < finUtc &&
                (!filtros.EmpleadoId.HasValue ||
                 c.Jornada.EmpleadoId ==
                    filtros.EmpleadoId.Value))
            .Select(c => new
            {
                c.Jornada.EmpleadoId,
                c.FechaHoraUtc
            })
            .ToListAsync(ct);

        foreach (var movimiento in movimientosDigitales)
        {
            var utc = DateTime.SpecifyKind(
                movimiento.FechaHoraUtc,
                DateTimeKind.Utc);

            var local = TimeZoneInfo.ConvertTimeFromUtc(
                utc,
                ZonaMexico);

            dias.Add(new DiaEmpleado(
                movimiento.EmpleadoId,
                local.Date));
        }

        // 2. Leer días con checadas de BioTime
        // vinculadas y vigentes.
        await AgregarDiasBioTimeAsync(
            dias,
            inicio,
            fin,
            filtros.EmpleadoId,
            ct);

        if (dias.Count == 0)
        {
            return new ResumenDashboard
            {
                Pagina = filtros.Pagina,
                Tamano = filtros.Tamano,
                TotalPaginas = 0
            };
        }

        // 3. Resolver colaboradores desde INTRANET.
        var ids = dias
            .Select(x => x.EmpleadoId)
            .Distinct()
            .ToArray();

        var empleados = await _db.Empleados
            .AsNoTracking()
            .Where(e => ids.Contains(e.Id))
            .Select(e => new
            {
                e.Id,
                e.NombreCompleto
            })
            .ToDictionaryAsync(x => x.Id, ct);

        var buscar = filtros.Buscar?.Trim() ?? "";

        var candidatos = dias
            .Where(x => empleados.ContainsKey(x.EmpleadoId))
            .Where(x =>
                string.IsNullOrWhiteSpace(buscar) ||
                empleados[x.EmpleadoId].NombreCompleto.Contains(
                    buscar,
                    StringComparison.OrdinalIgnoreCase) ||
                x.EmpleadoId.ToString().Contains(buscar))
            .OrderByDescending(x => x.Fecha)
            .ThenBy(x =>
                empleados[x.EmpleadoId].NombreCompleto)
            .ThenBy(x => x.EmpleadoId)
            .ToList();

        var total = candidatos.Count;

        var pagina = candidatos
            .Skip((filtros.Pagina - 1) * filtros.Tamano)
            .Take(filtros.Tamano)
            .ToList();

        var registros = new List<JornadaResumen>();

        // Los servicios comparten DbContext.
        // Consultar secuencialmente.
        foreach (var dia in pagina)
        {
            ct.ThrowIfCancellationRequested();

            var evaluacion = await _evaluador.EvaluarDiaAsync(
                dia.EmpleadoId,
                dia.Fecha,
                ct);

            var contexto = await _contextoTH.ObtenerAsync(
                dia.EmpleadoId,
                dia.Fecha,
                ct);


            bool revision =
                evaluacion.RequiereRevision ||
                evaluacion.EvaluacionTH?.RequiereRevisionTH == true ||
                contexto.RequiereRevisionTH ||
                evaluacion.MovimientosPendientes > 0;



            var observaciones = evaluacion.Observaciones
                .Concat(
                    evaluacion.EvaluacionTH?.Observaciones
                    ?? Enumerable.Empty<string>())
                .Concat(contexto.Observaciones)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();


            if (observaciones.Count == 0 &&
                !string.IsNullOrWhiteSpace(evaluacion.Mensaje))
            {
                observaciones.Add(evaluacion.Mensaje);
            }


            // =====================================================
            // INDICADORES OFICIALES DEL EVALUADOR PRELIMINAR TH
            // =====================================================
            // No duplicamos las reglas de puntualidad,
            // salida ni cálculo de horas.

            var evaluacionTH = evaluacion.EvaluacionTH;

            int? minutos = evaluacionTH?
                .TiempoEfectivoRegistrado is TimeSpan efectivo
                    ? (int)Math.Floor(efectivo.TotalMinutes)
                    : null;

            bool? puntual = evaluacionTH?.Puntualidad switch
            {
                "Puntual" => true,
                "RetardoPotencial" => false,
                _ => null
            };

            bool? salidaEnHorario =
                evaluacionTH?.EstadoSalida switch
                {
                    "SalidaEnHorario" => true,
                    "SalidaAnticipadaPotencial" => false,
                    _ => null
                };

            // No existe una tolerancia aprobada de
            // regreso de comida en el horario consultado.
            // No utilizamos una hora fija para todos.
            bool? comidaPuntual = null;


            registros.Add(new JornadaResumen
            {
                EmpleadoId = dia.EmpleadoId,
                Colaborador =
                    empleados[dia.EmpleadoId].NombreCompleto,
                Fecha = dia.Fecha,

                Horario = evaluacion.HorarioDescripcion ??
                          "Sin horario",

                Estado = evaluacion.Estado,

                Entrada = FormatearHora(
                    evaluacion.EntradaReal),

                SalidaComer = FormatearHora(
                    evaluacion.SalidaComidaReal),

                RegresoComer = FormatearHora(
                    evaluacion.RegresoComidaReal),

                Salida = FormatearHora(
                    evaluacion.SalidaReal),

                EntradaPuntual = puntual,
                RegresoComidaPuntual = comidaPuntual,
                SalidaEnHorario = salidaEnHorario,

                MinutosEfectivos = minutos,

                MovimientosIntranet =
                    evaluacion.MovimientosIntranet,

                MovimientosBioTime =
                    evaluacion.MovimientosBioTime,

                Confirmados =
                    evaluacion.MovimientosConfirmados,

                Pendientes =
                    evaluacion.MovimientosPendientes,

                HomeOfficeAutorizado =
                    contexto.HomeOfficeAutorizado,

                TieneAusencias =
                    contexto.TieneSolicitudesAusencia,

                RequiereRevision = revision,

                Observaciones = string.Join(
                    " | ",
                    observaciones),

                IntegridadChecadas =
                    evaluacionTH?.IntegridadChecadas ?? "NoEvaluable",

                                CumplimientoHorario =
                    evaluacionTH?.CumplimientoHorario ?? "NoEvaluable",

                                MinutosRetardoPotencial =
                    evaluacionTH?.MinutosRetardoPotencial,

                                MinutosSalidaAnticipadaPotencial =
                    evaluacionTH?.MinutosSalidaAnticipadaPotencial,

            });
        }

        return new ResumenDashboard
        {
            TotalJornadas = total,
            Pagina = filtros.Pagina,
            Tamano = filtros.Tamano,

            TotalPaginas = (int)Math.Ceiling(
                total / (double)filtros.Tamano),

            JornadasPagina = registros.Count,

            JornadasCompletasPagina =
                registros.Count(x =>
                    x.Estado == "CompletaParaRevision"),

            JornadasRevisionPagina =
                registros.Count(x => x.RequiereRevision),

            MovimientosPendientesPagina =
                registros.Sum(x => x.Pendientes),

            Registros = registros
        };
    }

    private async Task AgregarDiasBioTimeAsync(
        HashSet<DiaEmpleado> dias,
        DateTime inicio,
        DateTime fin,
        int? empleadoId,
        CancellationToken ct)
    {
        const string sql = """
            SELECT DISTINCT
                v.EmpleadoId,
                CONVERT(date, c.FechaHora)
            FROM dbo.BioTimeChecadas c
            INNER JOIN dbo.RC_EmpleadosBioTime v
                ON v.EmpCode = c.EmpCode
            WHERE v.Estado = 'Validada'
              AND c.FechaHora >= @Inicio
              AND c.FechaHora < @Fin
              AND v.FechaInicio <= CONVERT(date, c.FechaHora)
              AND (
                  v.FechaFin IS NULL
                  OR v.FechaFin >= CONVERT(date, c.FechaHora)
              )
              AND (
                  @EmpleadoId IS NULL
                  OR v.EmpleadoId = @EmpleadoId
              );
            """;

        var conexion = _db.Database.GetDbConnection();
        bool cerrar = conexion.State != ConnectionState.Open;

        try
        {
            if (cerrar)
                await conexion.OpenAsync(ct);

            await using var cmd = conexion.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 60;

            Parametro(
                cmd, "@Inicio",
                DbType.DateTime2, inicio);

            Parametro(
                cmd, "@Fin",
                DbType.DateTime2, fin.AddDays(1));

            Parametro(
                cmd, "@EmpleadoId",
                DbType.Int32,
                (object?)empleadoId ?? DBNull.Value);

            await using var lector =
                await cmd.ExecuteReaderAsync(ct);

            while (await lector.ReadAsync(ct))
            {
                dias.Add(new DiaEmpleado(
                    lector.GetInt32(0),
                    lector.GetDateTime(1).Date));
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

    private static void Parametro(
        System.Data.Common.DbCommand cmd,
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

    private static string? FormatearHora(DateTime? valor)
    {
        return valor?.ToString("HH:mm:ss");
    }

    private static void ValidarFiltros(Filtros f)
    {
        if (f.FechaFin.Date < f.FechaInicio.Date ||
            (f.FechaFin.Date - f.FechaInicio.Date).TotalDays > 6)
        {
            throw new ArgumentException(
                "El periodo debe ser de 1 a 7 días.");
        }

        if (f.Pagina < 1 ||
            f.Pagina > 100000 ||
            f.Tamano != 10)
        {
            throw new ArgumentException(
                "Los parámetros de paginación no son válidos.");
        }

        if (f.EmpleadoId.HasValue &&
            f.EmpleadoId.Value <= 0)
        {
            throw new ArgumentException(
                "El ID de empleado no es válido.");
        }

        if ((f.Buscar?.Length ?? 0) > 100)
        {
            throw new ArgumentException(
                "La búsqueda permite máximo 100 caracteres.");
        }

        var hoyMexico = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            ZonaMexico).Date;

        if (f.FechaFin.Date > hoyMexico)
        {
            throw new ArgumentException(
                "No se permiten fechas futuras.");
        }
    }
}
