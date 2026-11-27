using System.Security.Claims;

namespace ERPSEI.Services.Adquisiciones
{
    public interface IAdquisicionesControlCentralService
    {
        Task<List<AdquisicionesSolicitudPagoCentralDto>>
            ObtenerSolicitudesPagoAsync(
                ClaimsPrincipal principal,
                int? areaId = null,
                bool soloAutorizadas = false,
                CancellationToken cancellationToken = default
            );

        Task<bool> PuedeProcesarPagoAsync(
            ClaimsPrincipal principal,
            int solicitudId,
            CancellationToken cancellationToken = default
        );
    }
}