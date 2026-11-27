using ERPSEI.Data.Entities.Adquisiciones;
using System.Security.Claims;

namespace ERPSEI.Services.Adquisiciones
{
    public interface IAdquisicionesFlujoAreaService
    {
        Task<AdquisicionesConfiguracionAreaDto>
            ObtenerConfiguracionAsync(
                ClaimsPrincipal principal,
                int areaId,
                CancellationToken cancellationToken = default
            );

        Task<List<AdqConfiguracionAprobacionPresupuestal>>
            ObtenerFlujoVigenteAsync(
                ClaimsPrincipal principal,
                int solicitudId,
                CancellationToken cancellationToken = default
            );

        Task<bool> ConfiguracionCompletaAsync(
            ClaimsPrincipal principal,
            int solicitudId,
            CancellationToken cancellationToken = default
        );

        Task GuardarConfiguracionAsync(
            ClaimsPrincipal principal,
            int areaId,
            IReadOnlyCollection<AdquisicionesNivelAprobacionDto> etapas,
            CancellationToken cancellationToken = default
        );
    }
}