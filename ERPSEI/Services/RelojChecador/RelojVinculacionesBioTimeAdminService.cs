
using ERPSEI.Data;
using ERPSEI.Data.Entities.RelojChecador;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojVinculacionesBioTimeAdminService
{
    private readonly ApplicationDbContext _db;

    public RelojVinculacionesBioTimeAdminService(
        ApplicationDbContext db)
    {
        _db = db;
    }

    public sealed class VinculacionItem
    {
        public long Id { get; init; }
        public int EmpleadoId { get; init; }
        public string EmpCode { get; init; } = "";

        public string NombreIntranet { get; init; } = "";
        public string NombreBioTime { get; init; } = "";
        public string DepartamentoBioTime { get; init; } = "";

        public int? HorarioId { get; init; }
        public string Horario { get; init; } = "";
        public bool CalcularAsistencia { get; init; }

        public string Estado { get; init; } = "";
        public DateTime FechaInicio { get; init; }
        public DateTime? FechaFin { get; init; }

        public string? ValidadoPorUserId { get; init; }
        public DateTime? FechaValidacionUtc { get; init; }
    }

    public sealed class ResultadoOperacion
    {
        public bool Correcto { get; init; }
        public string Codigo { get; init; } = "";
        public string Mensaje { get; init; } = "";
    }

    // ---------------------------------------------------------
    // CONSULTA DE VINCULACIONES
    // ---------------------------------------------------------

    public async Task<IReadOnlyList<VinculacionItem>>
        ObtenerTodasAsync(CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                r.Id,
                r.EmpleadoId,
                r.EmpCode,
                e.NombreCompleto,
                LTRIM(RTRIM(CONCAT(
                    COALESCE(b.Nombre, ''),
                    ' ',
                    COALESCE(b.Apellidos, '')
                ))) AS NombreBioTime,
                COALESCE(b.Departamento, ''),
                e.HorarioId,
                COALESCE(h.Descripcion, ''),
                e.CalcularAsistencia,
                r.Estado,
                r.FechaInicio,
                r.FechaFin,
                r.ValidadoPorUserId,
                r.FechaValidacionUtc
            FROM dbo.RC_EmpleadosBioTime r
            INNER JOIN dbo.Empleados e
                ON e.Id = r.EmpleadoId
            LEFT JOIN dbo.BioTimeEmpleados b
                ON b.EmpCode = r.EmpCode
            LEFT JOIN dbo.Horarios h
                ON h.Id = e.HorarioId
            ORDER BY
                CASE r.Estado
                    WHEN 'Pendiente' THEN 0
                    WHEN 'Validada' THEN 1
                    ELSE 2
                END,
                e.NombreCompleto,
                r.Id;
            """;

        var items = new List<VinculacionItem>();
        var conexion = _db.Database.GetDbConnection();
        bool cerrar = conexion.State !=
            System.Data.ConnectionState.Open;

        try
        {
            if (cerrar)
                await conexion.OpenAsync(ct);

            await using var cmd = conexion.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 30;

            await using var reader =
                await cmd.ExecuteReaderAsync(ct);

            while (await reader.ReadAsync(ct))
            {
                items.Add(new VinculacionItem
                {
                    Id = reader.GetInt64(0),
                    EmpleadoId = reader.GetInt32(1),
                    EmpCode = reader.GetString(2),
                    NombreIntranet = reader.GetString(3),
                    NombreBioTime = reader.GetString(4),
                    DepartamentoBioTime = reader.GetString(5),

                    HorarioId = reader.IsDBNull(6)
                        ? null : reader.GetInt32(6),

                    Horario = reader.GetString(7),

                    CalcularAsistencia =
                        !reader.IsDBNull(8) &&
                        reader.GetBoolean(8),

                    Estado = reader.GetString(9),
                    FechaInicio = reader.GetDateTime(10),

                    FechaFin = reader.IsDBNull(11)
                        ? null : reader.GetDateTime(11),

                    ValidadoPorUserId = reader.IsDBNull(12)
                        ? null : reader.GetString(12),

                    FechaValidacionUtc = reader.IsDBNull(13)
                        ? null : reader.GetDateTime(13)
                });
            }
        }
        finally
        {
            if (cerrar &&
                conexion.State !=
                System.Data.ConnectionState.Closed)
            {
                await conexion.CloseAsync();
            }
        }

        return items;
    }

    // ---------------------------------------------------------
    // CERRAR VIGENCIA
    //
    // Mantiene Estado='Validada' para conservar
    // la consulta de asistencia histórica.
    // ---------------------------------------------------------

    public async Task<ResultadoOperacion> CerrarVigenciaAsync(
        long id,
        DateTime fechaFin,
        string actorUserId,
        CancellationToken ct = default)
    {
        if (id <= 0 ||
            string.IsNullOrWhiteSpace(actorUserId))
        {
            return Error(
                "PARAMETROS_INVALIDOS",
                "No se identificó la vinculación o el usuario.");
        }

        var fin = fechaFin.Date;

        var hoyMexico = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById(
                "America/Mexico_City")).Date;

        if (fin >= hoyMexico)
        {
            return Error(
                "FECHA_INVALIDA",
                "El cierre debe ser anterior al día actual.");
        }

        await using var transaccion =
            await _db.Database.BeginTransactionAsync(ct);

        try
        {
            int filas = await _db.Database
                .ExecuteSqlInterpolatedAsync($"""
                    UPDATE dbo.RC_EmpleadosBioTime
                    SET FechaFin = {fin}
                    WHERE Id = {id}
                      AND Estado = 'Validada'
                      AND FechaInicio <= {fin}
                      AND (FechaFin IS NULL OR FechaFin > {fin})
                    """, ct);

            if (filas != 1)
            {
                await transaccion.RollbackAsync(ct);
                return Error(
                    "NO_ACTUALIZADA",
                    "La vinculación no está validada, ya está cerrada o la fecha es inválida.");
            }

            _db.Set<RC_Auditoria>().Add(new RC_Auditoria
            {
                ActorUserId = actorUserId,
                Accion = "BIOTIME_VINCULACION_CIERRE",
                Resultado = "Correcto",
                CodigoDetalle = $"VINCULACION_ID:{id}",
                FechaUtc = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(ct);
            await transaccion.CommitAsync(ct);

            return new ResultadoOperacion
            {
                Correcto = true,
                Codigo = "VIGENCIA_CERRADA",
                Mensaje = "Vigencia cerrada. Se conserva el historial autorizado."
            };
        }
        catch
        {
            await transaccion.RollbackAsync(
                CancellationToken.None);
            throw;
        }
    }

    // ---------------------------------------------------------
    // REVOCAR VINCULACIÓN
    //
    // Revocada se excluye totalmente del proveedor
    // biométrico, incluso para fechas históricas.
    // Usar cuando la correspondencia fue incorrecta
    // o debe invalidarse integralmente.
    // ---------------------------------------------------------

    public async Task<ResultadoOperacion> RevocarAsync(
        long id,
        string actorUserId,
        CancellationToken ct = default)
    {
        if (id <= 0 ||
            string.IsNullOrWhiteSpace(actorUserId))
        {
            return Error(
                "PARAMETROS_INVALIDOS",
                "No se identificó la vinculación o el usuario.");
        }

        await using var transaccion =
            await _db.Database.BeginTransactionAsync(ct);

        try
        {
            int filas = await _db.Database
                .ExecuteSqlInterpolatedAsync($"""
                    UPDATE dbo.RC_EmpleadosBioTime
                    SET Estado = {"Revocada"}
                    WHERE Id = {id}
                      AND Estado IN ('Pendiente', 'Validada')
                    """, ct);

            if (filas != 1)
            {
                await transaccion.RollbackAsync(ct);
                return Error(
                    "NO_ACTUALIZADA",
                    "La vinculación ya fue revocada o no existe.");
            }

            _db.Set<RC_Auditoria>().Add(new RC_Auditoria
            {
                ActorUserId = actorUserId,
                Accion = "BIOTIME_VINCULACION_REVOCADA",
                Resultado = "Correcto",
                CodigoDetalle = $"VINCULACION_ID:{id}",
                FechaUtc = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(ct);
            await transaccion.CommitAsync(ct);

            return new ResultadoOperacion
            {
                Correcto = true,
                Codigo = "REVOCADA",
                Mensaje = "Vinculación revocada correctamente."
            };
        }
        catch
        {
            await transaccion.RollbackAsync(
                CancellationToken.None);
            throw;
        }
    }

    private static ResultadoOperacion Error(
        string codigo,
        string mensaje)
    {
        return new ResultadoOperacion
        {
            Correcto = false,
            Codigo = codigo,
            Mensaje = mensaje
        };
    }
}
