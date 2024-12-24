using ERPSEI.Data.Entities.Cuentas;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Cuentas
{
	public class CuentaContableTipoManager(ApplicationDbContext db) : ICuentaContableTipoManager
	{

		public async Task<List<CuentaContableTipo>> GetAllAsync()
		{
			return await db.CuentaContableTipos.ToListAsync();
		}

		public async Task<CuentaContableTipo?> GetByIdAsync(int id)
		{
			return await db.CuentaContableTipos.Where(c => c.Id == id).FirstOrDefaultAsync();
		}

		public async Task<CuentaContableTipo?> GetByNameAsync(string name)
		{
			return await db.CuentaContableTipos.Where(c => c.Descripcion == name.ToLower()).FirstOrDefaultAsync();
		}
    }
}
