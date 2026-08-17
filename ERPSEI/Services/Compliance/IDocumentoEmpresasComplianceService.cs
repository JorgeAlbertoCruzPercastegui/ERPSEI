namespace ERPSEI.Services.Compliance
{
    public interface IDocumentoEmpresasComplianceService
    {
        Task<ResultadoSincronizacionDocumental>
            SincronizarDesdeEmpresaAsync(
                int empresaMaestraId,
                int complianceId,
                string usuarioId,
                CancellationToken cancellationToken =
                    default
            );

        Task SincronizarDesdeComplianceAsync(
                int documentoComplianceId,
                CancellationToken cancellationToken = default
            );
    }

    public sealed class ResultadoSincronizacionDocumental
    {
        public int DocumentosRevisados { get; init; }

        public int DocumentosSincronizados { get; init; }

        public int DocumentosSinCambios { get; init; }

        public int DocumentosIgnorados { get; init; }

        public bool HuboCambios =>
            DocumentosSincronizados > 0;
    }
}