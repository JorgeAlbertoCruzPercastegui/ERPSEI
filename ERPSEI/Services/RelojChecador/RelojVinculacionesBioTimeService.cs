
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.RelojChecador;

public sealed class RelojVinculacionesBioTimeService
{
    private readonly ApplicationDbContext _db;

    public RelojVinculacionesBioTimeService(
        ApplicationDbContext db)
    {
        _db = db;
    }

    // =========================================================
    // MODELOS DE RESULTADO
    // =========================================================

    public sealed class VinculacionResultado
    {
        public bool Correcto { get; init; }

        public string Codigo { get; init; } = "";

        public string Mensaje { get; init; } = "";

        public long Id { get; init; }
    }

    public sealed class VinculacionDetalle
    {
        public long Id { get; init; }

        public int EmpleadoId { get; init; }

        public string NombreCompleto { get; init; } = "";

        public string EmpCode { get; init; } = "";

        public string Estado { get; init; } = "";

        public int? HorarioId { get; init; }

        public bool CalcularAsistencia { get; init; }

        public DateTime FechaInicio { get; init; }

        public DateTime? FechaFin { get; init; }
    }

    // =========================================================
    // 1. CONSULTAR VINCULACIONES PENDIENTES
    // =========================================================

    public async Task<IReadOnlyList<VinculacionDetalle>>
        ObtenerPendientesAsync(
            CancellationToken ct = default)
    {
        const string sql = """
            SELECT
                r.Id,
                r.EmpleadoId,
                e.NombreCompleto,
                r.EmpCode,
                r.Estado,
                e.HorarioId,
                e.CalcularAsistencia,
                r.FechaInicio,
                r.FechaFin
            FROM dbo.RC_EmpleadosBioTime r
            INNER JOIN dbo.Empleados e
                ON e.Id = r.EmpleadoId
            WHERE r.Estado = 'Pendiente'
            ORDER BY e.NombreCompleto, r.Id;
            """;

        var resultado = new List<VinculacionDetalle>();

        var conexion = _db.Database.GetDbConnection();

        bool cerrarConexion =
            conexion.State != ConnectionState.Open;

        try
        {
            if (cerrarConexion)
            {
                await conexion.OpenAsync(ct);
            }

            await using var comando =
                conexion.CreateCommand();

            comando.CommandText = sql;
            comando.CommandType = CommandType.Text;
            comando.CommandTimeout = 30;

            await using var lector =
                await comando.ExecuteReaderAsync(ct);

            while (await lector.ReadAsync(ct))
            {
                resultado.Add(new VinculacionDetalle
                {
                    Id = lector.GetInt64(0),

                    EmpleadoId = lector.GetInt32(1),

                    NombreCompleto = lector.IsDBNull(2)
                        ? ""
                        : lector.GetString(2),

                    EmpCode = lector.GetString(3),

                    Estado = lector.GetString(4),

                    HorarioId = lector.IsDBNull(5)
                        ? null
                        : lector.GetInt32(5),

                    CalcularAsistencia =
                        !lector.IsDBNull(6) &&
                        lector.GetBoolean(6),

                    FechaInicio = lector.GetDateTime(7),

                    FechaFin = lector.IsDBNull(8)
                        ? null
                        : lector.GetDateTime(8)
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

        return resultado;
    }

    // =========================================================
    // 2. VALIDAR CORRESPONDENCIA BIOTIME - INTRANET
    // =========================================================
    //
    // Este método debe llamarse únicamente desde una
    // operación autenticada y autorizada por TH / TI.
    //
    // No autoriza por sí mismo al usuario de ASP.NET.
    //
    // Reglas:
    // - La vinculación debe existir.
    // - Debe encontrarse en estado Pendiente.
    // - El empleado debe estar habilitado.
    // - El código debe existir en BioTimeEmpleados.
    // - No debe existir una vigencia superpuesta.
    // - Debe registrarse quién valida.
    // - Toda actualización debe ser transaccional.
    //
    // =========================================================

    public async Task<VinculacionResultado> ValidarAsync(
        long vinculacionId,
        string validadoPorUserId,
        DateTime fechaInicio,
        DateTime? fechaFin = null,
        CancellationToken ct = default)
    {
        // =====================================================
        // 2.1. VALIDACIONES INICIALES
        // =====================================================

        if (vinculacionId <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(vinculacionId));
        }

        if (string.IsNullOrWhiteSpace(validadoPorUserId))
        {
            throw new ArgumentException(
                "Se requiere identificar al usuario que autoriza.",
                nameof(validadoPorUserId));
        }

        validadoPorUserId = validadoPorUserId.Trim();

        if (validadoPorUserId.Length > 450)
        {
            throw new ArgumentException(
                "El identificador del usuario excede 450 caracteres.",
                nameof(validadoPorUserId));
        }

        fechaInicio = fechaInicio.Date;
        fechaFin = fechaFin?.Date;

        if (fechaFin.HasValue &&
            fechaFin.Value < fechaInicio)
        {
            throw new ArgumentException(
                "La fecha final no puede ser anterior a la inicial.");
        }

        // =====================================================
        // 2.2. PREPARAR CONEXIÓN
        // =====================================================

        var conexion = _db.Database.GetDbConnection();

        bool cerrarConexion =
            conexion.State != ConnectionState.Open;

        try
        {
            if (cerrarConexion)
            {
                await conexion.OpenAsync(ct);
            }

            // Serializable permite comprobar conflictos
            // y actualizar la vinculación en una sola
            // operación protegida contra concurrencia.

            await using var transaccion =
                await conexion.BeginTransactionAsync(
                    IsolationLevel.Serializable,
                    ct);

            bool transaccionFinalizada = false;

            try
            {
                // =================================================
                // 2.3. CONSULTAR VINCULACIÓN
                // =================================================

                const string consulta = """
                    SELECT
                        r.EmpleadoId,
                        r.EmpCode,
                        r.Estado,
                        e.Deshabilitado,
                        CASE
                            WHEN b.EmpCode IS NULL THEN 0
                            ELSE 1
                        END AS ExisteBioTime
                    FROM dbo.RC_EmpleadosBioTime r
                        WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN dbo.Empleados e
                        ON e.Id = r.EmpleadoId
                    LEFT JOIN dbo.BioTimeEmpleados b
                        ON b.EmpCode = r.EmpCode
                    WHERE r.Id = @Id;
                    """;

                int empleadoId = 0;
                string empCode = "";

                string? errorCodigo = null;
                string? errorMensaje = null;

                await using (var comando = CrearComando(
                    conexion,
                    transaccion,
                    consulta))
                {
                    AgregarParametro(
                        comando,
                        "@Id",
                        DbType.Int64,
                        vinculacionId);

                    // El lector se cierra antes de
                    // realizar cualquier rollback.
                    await using (var lector =
                        await comando.ExecuteReaderAsync(ct))
                    {
                        if (!await lector.ReadAsync(ct))
                        {
                            errorCodigo = "NO_ENCONTRADA";

                            errorMensaje =
                                "La vinculación no existe o el empleado relacionado no fue encontrado.";
                        }
                        else
                        {
                            empleadoId = lector.GetInt32(0);
                            empCode = lector.GetString(1);

                            string estado =
                                lector.GetString(2);

                            int deshabilitado =
                                lector.GetInt32(3);

                            int existeBioTime =
                                lector.GetInt32(4);

                            if (estado != "Pendiente")
                            {
                                errorCodigo =
                                    "ESTADO_INVALIDO";

                                errorMensaje =
                                    "Solo pueden validarse vinculaciones pendientes.";
                            }
                            else if (deshabilitado != 0)
                            {
                                errorCodigo =
                                    "EMPLEADO_DESHABILITADO";

                                errorMensaje =
                                    "El empleado de la INTRANET está deshabilitado.";
                            }
                            else if (existeBioTime == 0)
                            {
                                errorCodigo =
                                    "CODIGO_BIOTIME_NO_EXISTE";

                                errorMensaje =
                                    "El código del empleado no existe en BioTimeEmpleados.";
                            }
                        }
                    }
                }

                // =================================================
                // 2.4. RECHAZAR DESPUÉS DE CERRAR EL LECTOR
                // =================================================

                if (errorCodigo != null)
                {
                    return CrearError(
                        vinculacionId,
                        errorCodigo,
                        errorMensaje ??
                        "No fue posible validar la vinculación.");

                    // Al salir del método, el using
                    // desecha la transacción no confirmada.
                    // No se ejecuta un segundo rollback.
                }

                // =================================================
                // 2.5. COMPROBAR VIGENCIAS SUPERPUESTAS
                // =================================================

                const string consultaConflictos = """
                    SELECT COUNT_BIG(*)
                    FROM dbo.RC_EmpleadosBioTime
                        WITH (UPDLOCK, HOLDLOCK)
                    WHERE Id <> @Id
                      AND Estado = 'Validada'
                      AND
                      (
                          EmpleadoId = @EmpleadoId
                          OR EmpCode = @EmpCode
                      )
                      AND FechaInicio <= @FechaFin
                      AND
                      (
                          FechaFin IS NULL
                          OR FechaFin >= @FechaInicio
                      );
                    """;

                long totalConflictos;

                await using (var comando = CrearComando(
                    conexion,
                    transaccion,
                    consultaConflictos))
                {
                    AgregarParametro(
                        comando,
                        "@Id",
                        DbType.Int64,
                        vinculacionId);

                    AgregarParametro(
                        comando,
                        "@EmpleadoId",
                        DbType.Int32,
                        empleadoId);

                    AgregarParametro(
                        comando,
                        "@EmpCode",
                        DbType.String,
                        empCode);

                    AgregarParametro(
                        comando,
                        "@FechaInicio",
                        DbType.Date,
                        fechaInicio);

                    AgregarParametro(
                        comando,
                        "@FechaFin",
                        DbType.Date,
                        fechaFin ??
                        new DateTime(9999, 12, 31));

                    var valor =
                        await comando.ExecuteScalarAsync(ct);

                    totalConflictos =
                        Convert.ToInt64(valor);
                }

                if (totalConflictos > 0)
                {
                    return CrearError(
                        vinculacionId,
                        "VIGENCIA_SUPERPUESTA",
                        "Ya existe otra vinculación validada para el empleado o código BioTime dentro del periodo seleccionado.");
                }

                // =================================================
                // 2.6. ACTUALIZAR LA VINCULACIÓN
                // =================================================

                const string actualizar = """
                    UPDATE dbo.RC_EmpleadosBioTime
                    SET
                        Estado = 'Validada',
                        FechaInicio = @FechaInicio,
                        FechaFin = @FechaFin,
                        ValidadoPorUserId = @Usuario,
                        FechaValidacionUtc = SYSUTCDATETIME()
                    WHERE Id = @Id
                      AND Estado = 'Pendiente';
                    """;

                int filasActualizadas;

                await using (var comando = CrearComando(
                    conexion,
                    transaccion,
                    actualizar))
                {
                    AgregarParametro(
                        comando,
                        "@Id",
                        DbType.Int64,
                        vinculacionId);

                    AgregarParametro(
                        comando,
                        "@FechaInicio",
                        DbType.Date,
                        fechaInicio);

                    AgregarParametro(
                        comando,
                        "@FechaFin",
                        DbType.Date,
                        fechaFin.HasValue
                            ? fechaFin.Value
                            : DBNull.Value);

                    AgregarParametro(
                        comando,
                        "@Usuario",
                        DbType.String,
                        validadoPorUserId);

                    filasActualizadas =
                        await comando.ExecuteNonQueryAsync(ct);
                }

                if (filasActualizadas != 1)
                {
                    return CrearError(
                        vinculacionId,
                        "NO_ACTUALIZADA",
                        "La vinculación no pudo actualizarse. Verifica su estado actual.");
                }

                // =================================================
                // 2.7. CONFIRMAR TRANSACCIÓN
                // =================================================

                await transaccion.CommitAsync(ct);

                transaccionFinalizada = true;

                return new VinculacionResultado
                {
                    Correcto = true,
                    Codigo = "VALIDADA",
                    Mensaje =
                        "Correspondencia BioTime-INTRANET validada correctamente.",
                    Id = vinculacionId
                };
            }
            catch
            {
                // Revertir únicamente cuando no se ha
                // confirmado la transacción.
                if (!transaccionFinalizada)
                {
                    try
                    {
                        await transaccion.RollbackAsync(
                            CancellationToken.None);
                    }
                    catch
                    {
                        // Conservamos la excepción original.
                        // El dispose también liberará
                        // los recursos de la transacción.
                    }
                }

                throw;
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
    }

    // =========================================================
    // 3. CREAR RESPUESTA DE ERROR
    // =========================================================
    //
    // No realiza rollback aquí.
    // La transacción se desecha sin commit cuando
    // se devuelve una validación fallida.
    //
    // =========================================================

    private static VinculacionResultado CrearError(
        long vinculacionId,
        string codigo,
        string mensaje)
    {
        return new VinculacionResultado
        {
            Correcto = false,
            Codigo = codigo,
            Mensaje = mensaje,
            Id = vinculacionId
        };
    }

    // =========================================================
    // 4. CREAR COMANDO SQL
    // =========================================================

    private static DbCommand CrearComando(
        DbConnection conexion,
        DbTransaction transaccion,
        string sql)
    {
        var comando = conexion.CreateCommand();

        comando.Transaction = transaccion;
        comando.CommandText = sql;
        comando.CommandType = CommandType.Text;
        comando.CommandTimeout = 30;

        return comando;
    }

    // =========================================================
    // 5. AGREGAR PARÁMETRO SQL
    // =========================================================

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