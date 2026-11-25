
using System.Data;
using ERPSEI.Data;
using ERPSEI.Services.RelojChecador.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ERPSEI.Services.RelojChecador;

public sealed class ProveedorMovimientosBioTime
    : IProveedorMovimientosAsistencia
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProveedorMovimientosBioTime> _logger;

    private static readonly TimeZoneInfo ZonaMexico =
        TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

    public string Fuente => "BIOTIME";

    public ProveedorMovimientosBioTime(
        ApplicationDbContext db,
        IConfiguration configuration,
        ILogger<ProveedorMovimientosBioTime> logger)
    {
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IReadOnlyList<MovimientoAsistenciaNormalizado>>
        ObtenerMovimientosAsync(
            int empleadoId,
            DateTime fechaInicioLocal,
            DateTime fechaFinLocal,
            CancellationToken cancellationToken = default)
    {
        if (empleadoId <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(empleadoId));

        var inicio = fechaInicioLocal.Date;
        var fin = fechaFinLocal.Date;

        if (fin < inicio || (fin - inicio).TotalDays > 365)
        {
            throw new ArgumentException(
                "El periodo debe estar entre 1 y 366 días.");
        }

        // Seguridad: no se procesan datos biométricos
        // hasta confirmar la zona horaria del sincronizador.
        var habilitado = _configuration.GetValue<bool>(
            "BioTimeAttendance:Enabled");

        var fechaLocalConfirmada = _configuration.GetValue<bool>(
            "BioTimeAttendance:FechaHoraEsHoraLocalMexico");

        if (!habilitado || !fechaLocalConfirmada)
        {
            _logger.LogDebug(
                "Proveedor BioTime deshabilitado o zona horaria sin confirmar.");

            return Array.Empty<MovimientoAsistenciaNormalizado>();
        }

        const string sql = """
            SELECT
                c.Id,
                c.FechaHora,
                c.PunchState,
                c.TerminalSn
            FROM dbo.BioTimeChecadas c
            WHERE EXISTS
            (
                SELECT 1
                FROM dbo.RC_EmpleadosBioTime m
                WHERE m.EmpCode = c.EmpCode
                  AND m.EmpleadoId = @EmpleadoId
                  AND m.Estado = 'Validada'
                  AND CONVERT(date, c.FechaHora)
                      >= m.FechaInicio
                  AND (
                      m.FechaFin IS NULL
                      OR CONVERT(date, c.FechaHora)
                         <= m.FechaFin
                  )
            )
              AND c.FechaHora >= @FechaInicio
              AND c.FechaHora < @FechaFinExclusiva
            ORDER BY c.FechaHora, c.Id;
            """;

        var resultados =
            new List<MovimientoAsistenciaNormalizado>();

        var conexion = _db.Database.GetDbConnection();
        var cerrarConexion = conexion.State != ConnectionState.Open;

        try
        {
            if (cerrarConexion)
            {
                await conexion.OpenAsync(cancellationToken);
            }

            await using var comando = conexion.CreateCommand();
            comando.CommandText = sql;
            comando.CommandType = CommandType.Text;
            comando.CommandTimeout = 60;

            AgregarParametro(
                comando,
                "@EmpleadoId",
                DbType.Int32,
                empleadoId);

            AgregarParametro(
                comando,
                "@FechaInicio",
                DbType.DateTime2,
                inicio);

            AgregarParametro(
                comando,
                "@FechaFinExclusiva",
                DbType.DateTime2,
                fin.AddDays(1));

            await using var lector =
                await comando.ExecuteReaderAsync(cancellationToken);

            while (await lector.ReadAsync(cancellationToken))
            {
                var id = lector.GetInt64(0);
                var fechaHora = lector.GetDateTime(1);

                var punchState = lector.IsDBNull(2)
                    ? null
                    : lector.GetString(2);

                var terminal = lector.IsDBNull(3)
                    ? null
                    : lector.GetString(3);

                // Solo después de confirmar el origen
                // de las fechas, interpretamos FechaHora
                // como hora local de Ciudad de México.
                var horaLocal = DateTime.SpecifyKind(
                    fechaHora,
                    DateTimeKind.Unspecified);

                var horaUtc = TimeZoneInfo.ConvertTimeToUtc(
                    horaLocal,
                    ZonaMexico);

                // PunchState=0 no garantiza que sea Entrada.
                // Por eso no clasificamos el movimiento.
                var tipoMovimiento = "SinClasificar";

                resultados.Add(
                    new MovimientoAsistenciaNormalizado
                    {
                        IdMovimiento = $"BIOTIME:{id}",
                        EmpleadoId = empleadoId,
                        Fuente = Fuente,
                        Origen = string.IsNullOrWhiteSpace(terminal)
                            ? "BIOTIME"
                            : terminal,
                        TipoMovimiento = tipoMovimiento,
                        FechaHoraUtc = horaUtc,
                        FechaHoraMexico = horaLocal,
                        EstadoValidacion =
                            "PendienteClasificacion",
                        JornadaId = null,
                        ChecadaId = null,
                        TieneUbicacion = false
                    });
            }
        }
        finally
        {
            if (cerrarConexion &&
                conexion.State != ConnectionState.Closed)
            {
                await conexion.CloseAsync();
            }
        }

        return resultados;
    }

    private static void AgregarParametro(
        System.Data.Common.DbCommand comando,
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