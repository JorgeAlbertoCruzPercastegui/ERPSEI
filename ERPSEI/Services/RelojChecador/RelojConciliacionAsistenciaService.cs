
using System.Data;
using System.Data.Common;
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojConciliacionAsistenciaService
{
    private readonly ApplicationDbContext _db;
    private readonly RelojEvaluacionAsistenciaUnificadaService _evaluador;

    private static readonly Dictionary<string, int> Orden =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Entrada"] = 1,
            ["SalidaComer"] = 2,
            ["RegresoComer"] = 3,
            ["Salida"] = 4
        };

    public RelojConciliacionAsistenciaService(
        ApplicationDbContext db,
        RelojEvaluacionAsistenciaUnificadaService evaluador)
    {
        _db = db;
        _evaluador = evaluador;
    }

    public sealed class MovimientoConciliado
    {
        public string IdMovimiento { get; set; } = "";
        public string Fuente { get; set; } = "";
        public string Origen { get; set; } = "";
        public DateTime FechaHoraMexico { get; set; }
        public string TipoMovimiento { get; set; } = "";
        public string Estado { get; set; } = "";
        public bool ParticipaEnEvaluacion { get; set; }
        public bool RequiereRevision { get; set; }
        public string Observacion { get; set; } = "";
    }

    public sealed class Conflicto
    {
        public string Codigo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public List<string> Movimientos { get; set; } = new();
    }

    public sealed class ResultadoConciliacion
    {
        public int EmpleadoId { get; set; }
        public DateTime FechaLocal { get; set; }
        public string Horario { get; set; } = "";
        public string EstadoEvaluacion { get; set; } = "";
        public string EstadoConciliacion { get; set; } = "";

        public int Total { get; set; }
        public int Confirmados { get; set; }
        public int Pendientes { get; set; }
        public int Revocados { get; set; }
        public int NoValidados { get; set; }
        public int Conflictos { get; set; }

        public List<MovimientoConciliado> Movimientos { get; set; }
            = new();

        public List<Conflicto> DetalleConflictos { get; set; }
            = new();
    }

    public async Task<ResultadoConciliacion> ConciliarDiaAsync(
        int empleadoId,
        DateTime fechaLocal,
        CancellationToken ct = default)
    {
        if (empleadoId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(empleadoId));

        var fecha = fechaLocal.Date;

        // No ejecutamos dos consultas con el mismo
        // DbContext simultáneamente.
        var evaluacion = await _evaluador.EvaluarDiaAsync(
            empleadoId, fecha, ct);

        var clasificaciones = await
            ObtenerEstadosBioTimeAsync(empleadoId, fecha, ct);

        var resultado = new ResultadoConciliacion
        {
            EmpleadoId = empleadoId,
            FechaLocal = fecha,
            Horario = evaluacion.HorarioDescripcion ?? "Sin horario",
            EstadoEvaluacion = evaluacion.Estado
        };

        foreach (var original in evaluacion.Movimientos)
        {
            bool esBioTime = string.Equals(
                original.Fuente, "BIOTIME",
                StringComparison.OrdinalIgnoreCase);

            string estado;
            string tipo = original.TipoEvaluacion;
            bool confirmado = original.ParticipaEnEvaluacion;
            string observacion = original.Observacion;

            if (esBioTime)
            {
                // Consultamos el estado persistido en SQL;
                // el proveedor normalizado no distingue
                // todavía Pendiente de Revocada.
                estado = "Pendiente";

                const string prefijo = "BIOTIME:";

                if (original.IdMovimiento.StartsWith(
                        prefijo,
                        StringComparison.OrdinalIgnoreCase)
                    && long.TryParse(
                        original.IdMovimiento[prefijo.Length..],
                        out var id)
                    && clasificaciones.TryGetValue(id, out var c))
                {
                    estado = c.Estado;

                    if (c.Estado == "Confirmada")
                    {
                        tipo = c.TipoMovimiento;
                        confirmado = original.ParticipaEnEvaluacion;

                        observacion = confirmado
                            ? "Clasificación confirmada."
                            : "Clasificación existente no habilitada para evaluación.";
                    }
                    else if (c.Estado == "Revocada")
                    {
                        tipo = "SinClasificar";
                        confirmado = false;
                        observacion =
                            "Clasificación revocada. La marca original se conserva.";
                    }
                }
                else
                {
                    tipo = "SinClasificar";
                    confirmado = false;
                    observacion =
                        "La marca requiere clasificación de TH.";
                }
            }
            else
            {
                estado = confirmado
                    ? "Confirmada"
                    : "NoValidada";
            }

            resultado.Movimientos.Add(new MovimientoConciliado
            {
                IdMovimiento = original.IdMovimiento,
                Fuente = original.Fuente,
                Origen = original.Origen,
                FechaHoraMexico = original.FechaHoraMexico,
                TipoMovimiento = tipo,
                Estado = estado,
                ParticipaEnEvaluacion = confirmado,
                RequiereRevision = !confirmado,
                Observacion = observacion
            });
        }

        resultado.Movimientos = resultado.Movimientos
            .OrderBy(x => x.FechaHoraMexico)
            .ThenBy(x => x.IdMovimiento)
            .ToList();

        // Solo las marcas confirmadas pueden producir
        // una contradicción en la secuencia laboral.
        var confirmados = resultado.Movimientos
            .Where(x => x.ParticipaEnEvaluacion)
            .ToList();

        // A. Duplicidad del mismo tipo, incluso
        // cuando las fuentes son distintas.
        foreach (var grupo in confirmados
            .Where(x => Orden.ContainsKey(x.TipoMovimiento))
            .GroupBy(x => x.TipoMovimiento,
                StringComparer.OrdinalIgnoreCase))
        {
            if (grupo.Count() <= 1)
                continue;

            AgregarConflicto(
                resultado,
                "TIPO_DUPLICADO",
                $"Existen {grupo.Count()} movimientos " +
                $"confirmados como {grupo.Key}.",
                grupo.Select(x => x.IdMovimiento).ToList());
        }

        // B. Secuencia contradictoria de tipos.
        for (int i = 1; i < confirmados.Count; i++)
        {
            var anterior = confirmados[i - 1];
            var actual = confirmados[i];

            if (!Orden.TryGetValue(
                    anterior.TipoMovimiento,
                    out var ordenAnterior) ||
                !Orden.TryGetValue(
                    actual.TipoMovimiento,
                    out var ordenActual))
                continue;

            if (ordenActual <= ordenAnterior)
            {
                AgregarConflicto(
                    resultado,
                    "ORDEN_INCONSISTENTE",
                    "Los movimientos confirmados no siguen " +
                    "la secuencia Entrada, SalidaComer, " +
                    "RegresoComer y Salida.",
                    new List<string>
                    {
                        anterior.IdMovimiento,
                        actual.IdMovimiento
                    });
            }
        }

        // C. Dos tipos confirmados a la misma hora.
        // No asumimos automáticamente cuál es correcto.
        for (int i = 1; i < confirmados.Count; i++)
        {
            var anterior = confirmados[i - 1];
            var actual = confirmados[i];

            if (anterior.FechaHoraMexico ==
                actual.FechaHoraMexico &&
                anterior.TipoMovimiento != actual.TipoMovimiento)
            {
                AgregarConflicto(
                    resultado,
                    "MISMA_HORA_TIPOS_DISTINTOS",
                    "Dos movimientos confirmados presentan " +
                    "la misma fecha y hora con tipos diferentes.",
                    new List<string>
                    {
                        anterior.IdMovimiento,
                        actual.IdMovimiento
                    });
            }
        }

        resultado.Total = resultado.Movimientos.Count;

        resultado.Confirmados = resultado.Movimientos.Count(x =>
            x.ParticipaEnEvaluacion);

        resultado.Revocados = resultado.Movimientos.Count(x =>
            x.Estado == "Revocada");

        resultado.Pendientes = resultado.Movimientos.Count(x =>
            x.Estado == "Pendiente");

        resultado.NoValidados = resultado.Movimientos.Count(x =>
            x.Estado == "NoValidada" ||
            (x.Estado == "Confirmada" &&
             !x.ParticipaEnEvaluacion));

        resultado.Conflictos = resultado.DetalleConflictos.Count;

        resultado.EstadoConciliacion =
            resultado.Conflictos > 0
                ? "Conflicto"
                : resultado.Pendientes > 0 ||
                  resultado.Revocados > 0 ||
                  resultado.NoValidados > 0
                    ? "PendienteRevision"
                    : resultado.Total == 0
                        ? "SinMovimientos"
                        : "SinConflictosDetectados";

        return resultado;
    }

    private static void AgregarConflicto(
        ResultadoConciliacion resultado,
        string codigo,
        string descripcion,
        List<string> ids)
    {
        resultado.DetalleConflictos.Add(new Conflicto
        {
            Codigo = codigo,
            Descripcion = descripcion,
            Movimientos = ids
        });

        foreach (var movimiento in resultado.Movimientos
            .Where(x => ids.Contains(x.IdMovimiento)))
        {
            movimiento.RequiereRevision = true;
        }
    }

    private sealed class EstadoClasificacion
    {
        public string Estado { get; init; } = "";
        public string TipoMovimiento { get; init; } = "";
    }

    private async Task<Dictionary<long, EstadoClasificacion>>
        ObtenerEstadosBioTimeAsync(
            int empleadoId,
            DateTime fecha,
            CancellationToken ct)
    {
        const string sql = """
            SELECT
                cl.BioTimeChecadaId,
                cl.Estado,
                cl.TipoMovimiento
            FROM dbo.RC_ClasificacionesBioTime cl
            INNER JOIN dbo.BioTimeChecadas c
                ON c.Id = cl.BioTimeChecadaId
            WHERE cl.EmpleadoId = @EmpleadoId
              AND cl.FechaLocal = @Fecha
              AND CONVERT(date, c.FechaHora) = @Fecha
              AND EXISTS
              (
                  SELECT 1
                  FROM dbo.RC_EmpleadosBioTime v
                  WHERE v.EmpleadoId = cl.EmpleadoId
                    AND v.EmpCode = c.EmpCode
                    AND v.Estado = 'Validada'
                    AND v.FechaInicio <= @Fecha
                    AND (
                        v.FechaFin IS NULL OR
                        v.FechaFin >= @Fecha
                    )
              );
            """;

        var resultado =
            new Dictionary<long, EstadoClasificacion>();

        var conexion = _db.Database.GetDbConnection();
        bool cerrar = conexion.State != ConnectionState.Open;

        try
        {
            if (cerrar)
                await conexion.OpenAsync(ct);

            await using var comando = conexion.CreateCommand();
            comando.CommandText = sql;
            comando.CommandTimeout = 30;

            AgregarParametro(
                comando, "@EmpleadoId",
                DbType.Int32, empleadoId);

            AgregarParametro(
                comando, "@Fecha",
                DbType.Date, fecha);

            await using var lector =
                await comando.ExecuteReaderAsync(ct);

            while (await lector.ReadAsync(ct))
            {
                var id = lector.GetInt64(0);

                if (!resultado.TryAdd(
                    id,
                    new EstadoClasificacion
                    {
                        Estado = lector.GetString(1),
                        TipoMovimiento = lector.GetString(2)
                    }))
                {
                    throw new InvalidOperationException(
                        $"Clasificaciones duplicadas para BioTime {id}.");
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

    private static void AgregarParametro(
        DbCommand comando,
        string nombre,
        DbType tipo,
        object valor)
    {
        var parametro = comando.CreateParameter();
        parametro.ParameterName = nombre;
        parametro.DbType = tipo;
        parametro.Value = valor;
        comando.Parameters.Add(parametro);
    }
}
