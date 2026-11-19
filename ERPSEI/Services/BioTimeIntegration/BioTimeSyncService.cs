using ERPSEI.Data;
using ERPSEI.Data.Entities.BioTime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ERPSEI.Services.BioTimeIntegration;

public sealed class BioTimeSyncService
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private const int MaxDiasPeriodo = 365;
    private readonly ApplicationDbContext _db;
    private readonly BioTimeApiClient _api;
    private readonly BioTimeOptions _options;
    private readonly ILogger<BioTimeSyncService> _logger;

    public BioTimeSyncService(ApplicationDbContext db, BioTimeApiClient api,
        IOptions<BioTimeOptions> options, ILogger<BioTimeSyncService> logger)
    {
        _db = db;
        _api = api;
        _options = options.Value;
        _logger = logger;
    }

    // Servicio programado: no bloquea un ciclo si ya hay otra sincronización.
    public async Task SincronizarAsync(CancellationToken ct)
    {
        if (!await Gate.WaitAsync(0, ct)) return;
        try
        {
            var zona = TimeZoneInfo.FindSystemTimeZoneById(_options.TimeZoneId);
            var hoy = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona).Date;
            var dias = Math.Clamp(_options.LookbackDays, 1, 31);
            for (var offset = dias - 1; offset >= 0; offset--)
            {
                ct.ThrowIfCancellationRequested();
                var dia = hoy.AddDays(-offset);
                await SincronizarDiaAsync(dia, dia.AddDays(1).AddTicks(-1), ct);
            }
        }
        finally { Gate.Release(); }
    }

    // Permite que otras sincronizaciones utilicen el servidor entre días.
    // El worker histórico crea un scope diferente por día para limitar el ChangeTracker.
    public async Task<(bool Consultado, int Insertadas)> SincronizarDiaHistoricoAsync(
        DateTime dia, bool actualizarEmpleados, CancellationToken ct)
    {
        dia = dia.Date;
        var zona = TimeZoneInfo.FindSystemTimeZoneById(_options.TimeZoneId);
        var hoy = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zona).Date;
        if (dia > hoy) throw new ArgumentException("No se permiten fechas futuras.");

        await Gate.WaitAsync(ct);
        try
        {
            var finDia = dia.AddDays(1).AddTicks(-1);
            var ultimoExito = await _db.BioTimeSincronizaciones.AsNoTracking()
                .Where(x => x.FechaInicioConsulta == dia && x.FechaFinConsulta == finDia && x.Estado == "Correcta")
                .OrderByDescending(x => x.FinUtc)
                .Select(x => x.FinUtc)
                .FirstOrDefaultAsync(ct);
            var vigencia = dia >= hoy.AddDays(-1) ? TimeSpan.FromMinutes(5) : TimeSpan.FromHours(24);
            if (ultimoExito.HasValue && DateTime.UtcNow - ultimoExito.Value < vigencia)
                return (false, 0);
            var insertadas = await SincronizarDiaAsync(dia, finDia, ct, actualizarEmpleados);
            return (true, insertadas);
        }
        finally { Gate.Release(); }
    }

    // Se conserva para quienes ya lo utilicen desde otro código.
    public async Task<(int DiasConsultados, int DiasOmitidos, int ChecadasInsertadas)> SincronizarPeriodoAsync(
        DateTime fechaInicio, DateTime fechaFin, CancellationToken ct)
    {
        var inicio = fechaInicio.Date;
        var fin = fechaFin.Date;
        if (fin < inicio || (fin - inicio).Days + 1 > MaxDiasPeriodo)
            throw new ArgumentException($"Selecciona un periodo de 1 a {MaxDiasPeriodo} días.");
        var consultados = 0;
        var omitidos = 0;
        var insertadas = 0;
        var empleadosActualizados = false;
        for (var dia = inicio; dia <= fin; dia = dia.AddDays(1))
        {
            var resultado = await SincronizarDiaHistoricoAsync(dia, !empleadosActualizados, ct);
            if (resultado.Consultado) { consultados++; empleadosActualizados = true; }
            else omitidos++;
            insertadas += resultado.Insertadas;
        }
        return (consultados, omitidos, insertadas);
    }

    private async Task<int> SincronizarDiaAsync(DateTime inicio, DateTime fin, CancellationToken ct, bool actualizarEmpleados = true)
    {
        var auditoria = new BioTimeSincronizacion
        {
            InicioUtc = DateTime.UtcNow,
            FechaInicioConsulta = inicio,
            FechaFinConsulta = fin,
            Estado = "Iniciada"
        };
        _db.BioTimeSincronizaciones.Add(auditoria);
        await _db.SaveChangesAsync(ct);
        try
        {
            if (actualizarEmpleados)
            {
                var empleados = (await _api.ObtenerEmpleadosAsync(ct))
                    .Where(x => !string.IsNullOrWhiteSpace(x.Codigo))
                    .DistinctBy(x => x.Codigo)
                    .ToList();
                auditoria.EmpleadosProcesados = empleados.Count;
                foreach (var lote in empleados.Chunk(500))
                {
                    var codigos = lote.Select(x => x.Codigo).ToArray();
                    var existentes = await _db.BioTimeEmpleados
                        .Where(x => codigos.Contains(x.EmpCode))
                        .ToDictionaryAsync(x => x.EmpCode, ct);
                    foreach (var e in lote)
                    {
                        if (!existentes.TryGetValue(e.Codigo, out var entidad))
                        {
                            entidad = new BioTimeEmpleado { EmpCode = e.Codigo };
                            _db.BioTimeEmpleados.Add(entidad);
                        }
                        entidad.Nombre = e.Nombre;
                        entidad.Apellidos = e.Apellidos;
                        entidad.Departamento = e.Departamento;
                        entidad.Puesto = e.Puesto;
                        entidad.ActualizadoUtc = DateTime.UtcNow;
                    }
                    await _db.SaveChangesAsync(ct);
                }

            }

            var checadas = (await _api.ObtenerChecadasAsync(inicio, fin, ct))
                .DistinctBy(x => x.Id).ToList();
            foreach (var lote in checadas.Chunk(500))
            {
                var ids = lote.Select(x => x.Id).ToArray();
                var existentes = (await _db.BioTimeChecadas.AsNoTracking()
                    .Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToListAsync(ct)).ToHashSet();
                var nuevas = lote.Where(x => !existentes.Contains(x.Id)).ToList();
                foreach (var c in nuevas)
                {
                    _db.BioTimeChecadas.Add(new BioTimeChecada
                    {
                        Id = c.Id,
                        EmpCode = c.Codigo,
                        FechaHora = c.FechaHora,
                        PunchState = c.Estado,
                        VerifyType = c.Verificacion,
                        TerminalSn = c.Dispositivo,
                        SincronizadoUtc = DateTime.UtcNow
                    });
                }
                await _db.SaveChangesAsync(ct);
                auditoria.ChecadasInsertadas += nuevas.Count;
                // Mantener el ChangeTracker acotado en importaciones históricas.
                foreach (var entrada in _db.ChangeTracker.Entries<BioTimeChecada>()
                    .Where(e => e.State == EntityState.Unchanged).ToList())
                    entrada.State = EntityState.Detached;
            }
            auditoria.Estado = "Correcta";
            return auditoria.ChecadasInsertadas;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            auditoria.Estado = "Cancelada";
            DescartarCambiosPendientesYReanexarAuditoria(auditoria);
            throw;
        }
        catch (Exception ex)
        {
            auditoria.Estado = "Error";
            auditoria.Error = ex.GetType().Name;
            _logger.LogError("Error en sincronización BioTime ({Tipo})", ex.GetType().Name);
            DescartarCambiosPendientesYReanexarAuditoria(auditoria);
            throw;
        }
        finally
        {
            auditoria.FinUtc = DateTime.UtcNow;
            await _db.SaveChangesAsync(CancellationToken.None);
        }
    }

    private void DescartarCambiosPendientesYReanexarAuditoria(BioTimeSincronizacion auditoria)
    {
        _db.ChangeTracker.Clear();
        _db.BioTimeSincronizaciones.Attach(auditoria);
        _db.Entry(auditoria).State = EntityState.Modified;
    }
}
