using System.Security.Claims;

namespace ERPSEI.Services.Compliance
{
    public interface IPermisosComplianceService
    {
        Task<PermisosComplianceResultado>
            ObtenerPermisosAsync(
                ClaimsPrincipal usuario
            );

        Task<bool> TieneAccesoModuloAsync(
            ClaimsPrincipal usuario
        );

        Task<bool> EsAdministradorAsync(
            ClaimsPrincipal usuario
        );

        Task<bool> PuedeAdministrarPermisosAsync(
            ClaimsPrincipal usuario
        );

        Task<bool> PuedeVisualizarAsync(
            ClaimsPrincipal usuario
        );

        Task<bool> PuedeCrearCargarAsync(
            ClaimsPrincipal usuario
        );

        Task<bool> PuedeModificarAsync(
            ClaimsPrincipal usuario
        );

        Task<bool> PuedeEliminarAsync(
            ClaimsPrincipal usuario
        );

        Task<bool> PuedeDescargarAsync(
            ClaimsPrincipal usuario
        );
    }
}