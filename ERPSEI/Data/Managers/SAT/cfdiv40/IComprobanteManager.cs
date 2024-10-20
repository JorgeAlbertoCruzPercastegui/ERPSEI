using ERPSEI.Data.Entities.SAT.cfdiv40;

namespace ERPSEI.Data.Managers.SAT.cfdiv40
{
    public interface IComprobanteManager : IRWCatalogoManager<Comprobante>
    {
		public Task<List<Comprobante>> GetAllAsync(
			string? periodo = null,
			int? estatusId = null,
			int? tipoId = null,
			int? formaPagoId = null,
			int? metodoPagoId = null,
			int? usoCFDIId = null,
			int? emisorId = null,
			int? receptorId = null
		);
        public Task<List<Comprobante>> GetByDateRangeAsync(
			DateTime? fechaInicio, 
			DateTime? fechaFin
			);
    }
}