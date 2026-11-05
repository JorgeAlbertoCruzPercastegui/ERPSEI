namespace ERPSEI.Services.Empresas
{
    public interface INotificacionExpedienteBancosService
    {
        Task EvaluarDespuesDeGuardarAsync(int empresaId, CancellationToken ct = default);
    }
}
