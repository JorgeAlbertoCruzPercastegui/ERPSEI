using ERPSEI.Email;

namespace ERPSEI.Services.Adquisiciones
{
    public interface IAdquisicionesEmailService
    {
        Task NotificarAprobacionPendienteAsync(
            int solicitudId,
            string usuarioAprobadorId,
            int ordenEtapa,
            string nombreEtapa
        );


        Task NotificarEtapaAprobadaAsync(
            int solicitudId,
            string nombreAprobador,
            int ordenEtapa,
            string nombreEtapa,
            string? siguienteEtapa
        );


        Task NotificarFlujoCompletadoAsync(
            int solicitudId,
            string nombreAprobador,
            IEnumerable<EmailAttachment>? adjuntos = null
        );
    }
}