using ERPSEI.Data.Entities.SAT.cfdiv40;

namespace ERPSEI.Data.Managers.SAT.cfdiv40
{
    public interface IComprobanteManager : IRWCatalogoManager<Comprobante>
	{
		public Task<List<Comprobante>> GetAllAsync(
			string? empresaRFC = null,
			string? anio = null,
			string? mes = null,
			int? estatusId = null,
			int? tipoId = null,
			int? estatusContableId = null,
			string? tipoComprobanteClave = null,
			string? formaPagoClave = null,
			string? metodoPagoClave = null,
			string? usoCFDIClave = null,
			string? emisorRFC = null,
			string? receptorRFC = null
		);

		public Task<List<Comprobante>> GetByDateRangeAsync(
			DateTime? fechaInicio, 
			DateTime? fechaFin
            //int? clienteId = null
            );

        Task<List<Comprobante>> GetByRFCAsync(string rfc);

		public Task UpdateMultipleAsync(List<Comprobante> comprobantes);

		public Task<Comprobante?> GetValidatableComprobanteByIdAsync(int id);

		public Task<Comprobante?> GetWithConceptosByIdAsync(int id);

		public Task<Comprobante?> GetWithReceptorByIdAsync(int id);

		public Task<Comprobante?> GetByIdWithDescripcionesAsync(int id);

		public Task<List<Comprobante>> GetComprobantesGraficas(
			string? anio = null,
			string? mes = null
		);

	}
}