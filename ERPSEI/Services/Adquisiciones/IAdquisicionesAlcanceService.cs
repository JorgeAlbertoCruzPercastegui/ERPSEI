using System.Security.Claims;

namespace ERPSEI.Services.Adquisiciones
{
    public interface IAdquisicionesAlcanceService
    {
        Task<AdquisicionesAlcance> ObtenerAlcanceAsync(
            ClaimsPrincipal principal,
            CancellationToken cancellationToken = default
        );

        Task<bool> TieneAlcanceAreaAsync(
            ClaimsPrincipal principal,
            int areaId,
            CancellationToken cancellationToken = default
        );

        Task<AdquisicionesAccesoSolicitud>
            ObtenerAccesoSolicitudAsync(
                ClaimsPrincipal principal,
                int solicitudId,
                CancellationToken cancellationToken = default
            );

        Task<bool> PuedeConsultarSolicitudAsync(
            ClaimsPrincipal principal,
            int solicitudId,
            CancellationToken cancellationToken = default
        );

        Task<bool> PuedeGestionarSolicitudAsync(
            ClaimsPrincipal principal,
            int solicitudId,
            CancellationToken cancellationToken = default
        );
    }
}