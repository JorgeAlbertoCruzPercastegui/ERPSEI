
namespace ERPSEI.Services.TimeIntegration
{
    public sealed class TimeSyncResultadoDto
    {
        public string Tabla { get; set; } = "";
        public long RegistrosLeidos { get; set; }
        public long RegistrosInsertados { get; set; }
        public long RegistrosActualizados { get; set; }
        public long UltimoId { get; set; }
        public string Estado { get; set; } = "";
    }

    public interface ITimeSyncService
    {
        Task<TimeSyncResultadoDto> SincronizarDepartamentosAsync(
            CancellationToken cancellationToken = default);

        Task<TimeSyncResultadoDto> SincronizarUsuariosAsync(
            CancellationToken cancellationToken = default);

        Task<TimeSyncResultadoDto> SincronizarClavesAsync(
            CancellationToken cancellationToken = default);

        Task<TimeSyncResultadoDto> SincronizarActividadesAsync(
            CancellationToken cancellationToken = default);

        Task<TimeSyncResultadoDto> SincronizarHorasAsync(
            CancellationToken cancellationToken = default);
    }
}
