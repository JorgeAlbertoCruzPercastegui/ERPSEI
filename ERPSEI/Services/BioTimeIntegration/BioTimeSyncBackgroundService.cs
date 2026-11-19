using ERPSEI.Services.TimeIntegration;
using Microsoft.Extensions.Options;

namespace ERPSEI.Services.BioTimeIntegration;

public sealed class BioTimeSyncBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<BioTimeOptions> _settings;
    private readonly ILogger<BioTimeSyncBackgroundService> _logger;
    public BioTimeSyncBackgroundService(IServiceScopeFactory scopeFactory, IOptions<BioTimeOptions> settings, ILogger<BioTimeSyncBackgroundService> logger)
    { _scopeFactory = scopeFactory; _settings = settings; _logger = logger; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = _settings.Value;
        if (!settings.Enabled) return;
        if (settings.RunOnStartup) await RunOnce(stoppingToken);
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(Math.Clamp(settings.IntervalMinutes, 5, 1440)));
        while (await timer.WaitForNextTickAsync(stoppingToken)) await RunOnce(stoppingToken);
    }

    private async Task RunOnce(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            await scope.ServiceProvider.GetRequiredService<BioTimeSyncService>().SincronizarAsync(ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
        catch (Exception ex) { _logger.LogError("Sincronización BioTime falló ({Tipo}).", ex.GetType().Name); }
    }
}
