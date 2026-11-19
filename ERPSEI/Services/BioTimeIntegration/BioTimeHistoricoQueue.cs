using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ERPSEI.Services.BioTimeIntegration;

// Cola en memoria: no requiere migraciones, pero los trabajos no sobreviven a un reinicio.
public sealed class BioTimeHistoricoQueue
{
    private readonly Channel<BioTimeHistoricoJob> _channel = Channel.CreateBounded<BioTimeHistoricoJob>(
        new BoundedChannelOptions(4) { SingleReader = true, SingleWriter = false, FullMode = BoundedChannelFullMode.Wait });
    private readonly ConcurrentDictionary<Guid, BioTimeHistoricoJob> _jobs = new();
    private readonly object _gate = new();

    public (bool Accepted, Guid Id, string? Error) Enqueue(DateTime inicio, DateTime fin)
    {
        lock (_gate)
        {
            // Evita solicitudes duplicadas del mismo periodo.
            var existing = _jobs.Values.FirstOrDefault(x =>
                x.Inicio == inicio && x.Fin == fin && (x.Estado == "En cola" || x.Estado == "En proceso"));
            if (existing != null) return (true, existing.Id, null);
            var job = new BioTimeHistoricoJob(inicio, fin);
            if (!_channel.Writer.TryWrite(job)) return (false, Guid.Empty, "Hay demasiados periodos pendientes. Intenta más tarde.");
            _jobs[job.Id] = job;
            // Conserva sólo resultados recientes y siempre conserva los trabajos activos.
            foreach (var old in _jobs.Values.Where(x => x.FinalizadoUtc.HasValue && x.FinalizadoUtc < DateTime.UtcNow.AddHours(-6)))
                _jobs.TryRemove(old.Id, out _);
            return (true, job.Id, null);
        }
    }

    public object? GetStatus(Guid id)
    {
        if (!_jobs.TryGetValue(id, out var job)) return null;
        lock (job)
            return new
            {
                job.Id,
                job.Estado,
                job.DiasTotales,
                job.DiasCompletados,
                job.DiasConsultados,
                job.DiasOmitidos,
                job.ChecadasInsertadas,
                job.Inicio,
                job.Fin,
                job.Error,
                job.FinalizadoUtc
            };
    }

    public IAsyncEnumerable<BioTimeHistoricoJob> ReadAllAsync(CancellationToken ct) => _channel.Reader.ReadAllAsync(ct);
}

public sealed class BioTimeHistoricoJob
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime Inicio { get; }
    public DateTime Fin { get; }
    public string Estado { get; set; } = "En cola";
    public int DiasTotales => (Fin - Inicio).Days + 1;
    public int DiasCompletados { get; set; }
    public int DiasConsultados { get; set; }
    public int DiasOmitidos { get; set; }
    public int ChecadasInsertadas { get; set; }
    public string? Error { get; set; }
    public DateTime? FinalizadoUtc { get; set; }
    public BioTimeHistoricoJob(DateTime inicio, DateTime fin) { Inicio = inicio; Fin = fin; }
}

public sealed class BioTimeHistoricoWorker : BackgroundService
{
    private readonly BioTimeHistoricoQueue _queue;
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<BioTimeHistoricoWorker> _logger;

    public BioTimeHistoricoWorker(BioTimeHistoricoQueue queue, IServiceScopeFactory scopes, ILogger<BioTimeHistoricoWorker> logger)
    { _queue = queue; _scopes = scopes; _logger = logger; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _queue.ReadAllAsync(stoppingToken))
        {
            lock (job) job.Estado = "En proceso";
            try
            {
                var empleadosActualizados = false;
                for (var dia = job.Inicio; dia <= job.Fin; dia = dia.AddDays(1))
                {
                    stoppingToken.ThrowIfCancellationRequested();
                    using var scope = _scopes.CreateScope();
                    var sync = scope.ServiceProvider.GetRequiredService<BioTimeSyncService>();
                    var resultado = await sync.SincronizarDiaHistoricoAsync(dia, !empleadosActualizados, stoppingToken);
                    lock (job)
                    {
                        job.DiasCompletados++;
                        if (resultado.Consultado) { job.DiasConsultados++; empleadosActualizados = true; }
                        else job.DiasOmitidos++;
                        job.ChecadasInsertadas += resultado.Insertadas;
                    }
                }
                lock (job) { job.Estado = "Correcta"; job.FinalizadoUtc = DateTime.UtcNow; }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                lock (job) { job.Estado = "Interrumpida"; job.FinalizadoUtc = DateTime.UtcNow; }
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falló la importación histórica BioTime (job {Id})", job.Id);
                lock (job) { job.Estado = "Error"; job.Error = ex.GetType().Name; job.FinalizadoUtc = DateTime.UtcNow; }
            }
        }
    }
}
