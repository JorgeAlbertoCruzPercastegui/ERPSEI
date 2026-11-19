
using System.Security.Claims;
using ERPSEI.Data.Entities.TalentoHumano;

namespace ERPSEI.Services.TalentoHumano
{
    public interface ISolicitudesTHService
    {
        Task<THSolicitud> CrearSolicitudAsync(
            TipoSolicitudTH tipo,
            ClaimsPrincipal usuario,
            CancellationToken cancellationToken = default);
    }
}