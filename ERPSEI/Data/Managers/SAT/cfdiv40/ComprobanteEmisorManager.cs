using ERPSEI.Data.Entities.SAT.cfdiv40;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.SAT.cfdiv40
{
	public class ComprobanteEmisorManager(ApplicationDbContext _db) : IComprobanteEmisorManager
	{
		public async Task<List<ComprobanteEmisor>> GetAllAsync()
		{
			return await _db.ComprobantesEmisores.ToListAsync();
		}

		public async Task<ComprobanteEmisor?> GetByIdAsync(int id)
        {
            return await _db.ComprobantesEmisores.Where(e => e.Id == id).FirstOrDefaultAsync();
        }

		public async Task<ComprobanteEmisor?> GetByNameAsync(string name)
		{
			return await _db.ComprobantesEmisores
				.Where(ce => (ce.Nombre ?? string.Empty).Equals(name, StringComparison.CurrentCultureIgnoreCase) || (ce.Rfc ?? string.Empty).Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
		}

		public async Task<List<ComprobanteEmisor>> SearchEmisor(string text)
		{
			string sql = $"SELECT DISTINCT 1 AS Id, Rfc, Nombre, '' AS RegimenFiscal, '' AS FacAtrAdquirente " +
						 $"FROM ComprobantesEmisores " +
						 $"WHERE Nombre LIKE '%{text}%' OR Rfc LIKE '%{text}%'";
			List<ComprobanteEmisor> emisores = await _db.Database.SqlQueryRaw<ComprobanteEmisor>(sql).ToListAsync();

			return emisores;
		}
	}
}
