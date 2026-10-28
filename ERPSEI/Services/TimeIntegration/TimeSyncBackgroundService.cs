
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeSyncBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TimeSyncBackgroundService> _logger;

        public TimeSyncBackgroundService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<TimeSyncBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            bool habilitado = _configuration.GetValue<bool>(
                "TimeAutoSync:Enabled", false);

            if (!habilitado)
            {
                _logger.LogInformation(
                    "TIME: Sincronización automática deshabilitada.");
                return;
            }

            int minutos = _configuration.GetValue<int>(
                "TimeAutoSync:IntervalMinutes", 60);

            if (minutos < 15)
                minutos = 15;

            bool ejecutarAlIniciar = _configuration.GetValue<bool>(
                "TimeAutoSync:RunOnStartup", true);

            _logger.LogInformation(
                "TIME: Sincronización automática habilitada. " +
                "Intervalo: {Minutos} minutos.", minutos);

            if (ejecutarAlIniciar)
                await EjecutarCicloSeguroAsync(stoppingToken);

            using var timer = new PeriodicTimer(
                TimeSpan.FromMinutes(minutos));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await EjecutarCicloSeguroAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation(
                    "TIME: Servicio automático detenido.");
            }
        }

        private async Task EjecutarCicloSeguroAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                await EjecutarCicloAsync(cancellationToken);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                // La aplicación está cerrando.
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TIME: Error general en la sincronización automática.");
            }
        }

        private async Task EjecutarCicloAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "TIME: Iniciando ciclo automático.");

            // Crear un scope independiente para utilizar
            // servicios Scoped desde BackgroundService.
            using var scope = _scopeFactory.CreateScope();

            var servicio = scope.ServiceProvider
                .GetRequiredService<ITimeSyncService>();

            // Orden de sincronización:
            // primero catálogos y después datos relacionados.
            var operaciones = new (
                string Nombre,
                Func<CancellationToken, Task<TimeSyncResultadoDto>> Ejecutar
            )[]
            {
                ("departamentos",
                    ct => servicio.SincronizarDepartamentosAsync(ct)),

                ("usuarios",
                    ct => servicio.SincronizarUsuariosAsync(ct)),

                ("grupos",
                    ct => servicio.SincronizarGruposAsync(ct)),

                ("empresas",
                    ct => servicio.SincronizarEmpresasAsync(ct)),

                ("claves",
                    ct => servicio.SincronizarClavesAsync(ct)),

                ("actividades",
                    ct => servicio.SincronizarActividadesAsync(ct)),

                ("horas",
                    ct => servicio.SincronizarHorasAsync(ct))
            };

            foreach (var operacion in operaciones)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var resultado = await operacion.Ejecutar(
                        cancellationToken);

                    _logger.LogInformation(
                        "TIME automático: {Tabla} - " +
                        "Leídos: {Leidos}, Insertados: {Insertados}, " +
                        "Existentes: {Existentes}, Estado: {Estado}",
                        operacion.Nombre,
                        resultado.RegistrosLeidos,
                        resultado.RegistrosInsertados,
                        resultado.RegistrosActualizados,
                        resultado.Estado);
                }
                catch (OperationCanceledException)
                    when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "TIME: Error sincronizando {Tabla}.",
                        operacion.Nombre);

                    // Si una tabla falla, no se sincronizan
                    // las dependientes con una carga incompleta.
                    // Se reintentará en el siguiente ciclo.
                    return;
                }
            }

            _logger.LogInformation(
                "TIME: Ciclo automático finalizado correctamente.");
        }
    }
}
