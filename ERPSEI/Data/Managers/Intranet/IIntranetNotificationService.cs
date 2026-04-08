namespace ERPSEI.Data.Managers.Intranet
{
    public interface IIntranetNotificationService
    {
        Task EnviarNotificacionComunicadoPruebaAsync(string titulo, string? descripcion, string urlDestino);
        Task EnviarNotificacionEventoPruebaAsync(string titulo, string? descripcion, string urlDestino);
    }
}