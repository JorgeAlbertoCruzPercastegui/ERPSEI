using ERPSEI.Data.Entities.SAT.cfdiv40;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.SAT.cfdiv40
{
	public class ComprobanteReceptorManager(ApplicationDbContext _db) : IComprobanteReceptorManager
	{
		public async Task<List<ComprobanteReceptor>> GetAllAsync()
		{
			return await _db.ComprobantesReceptores.ToListAsync();
		}

		public async Task<ComprobanteReceptor?> GetByIdAsync(int id)
        {
            return await _db.ComprobantesReceptores.Where(e => e.Id == id).FirstOrDefaultAsync();
        }

		public async Task<ComprobanteReceptor?> GetByNameAsync(string name)
		{
			return await _db.ComprobantesReceptores
				.Where(ce => (ce.Nombre ?? string.Empty).Equals(name, StringComparison.CurrentCultureIgnoreCase) || (ce.Rfc ?? string.Empty).Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
		}

		public async Task<List<ComprobanteReceptor>> SearchReceptor(string text)
		{
			string sql = $"SELECT DISTINCT 1 As Id, Rfc, Nombre, '' AS DomicilioFiscalReceptor, '' AS ResidenciaFiscal, CAST(0 AS BIT) AS ResidenciaFiscalSpecified, '' AS NumRegIdTrib, '' AS RegimenFiscalReceptor, '' AS UsoCFDI " +
						 $"FROM ComprobantesReceptores " +
						 $"WHERE Nombre LIKE '%{text}%' OR Rfc LIKE '%{text}%'";
			List<ComprobanteReceptor> receptores = await _db.Database.SqlQueryRaw<ComprobanteReceptor>(sql).ToListAsync();

			return receptores;
		}
	}
}
