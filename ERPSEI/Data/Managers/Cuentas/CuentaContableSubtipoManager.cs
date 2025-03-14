using ERPSEI.Data.Entities.Cuentas;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Cuentas
{
	public class CuentaContableSubtipoManager(ApplicationDbContext db) : ICuentaContableSubtipoManager
	{

		public async Task<List<CuentaContableSubtipo>> GetAllAsync()
		{
			return await db.CuentaContableSubtipos.ToListAsync();
		}

		public async Task<CuentaContableSubtipo?> GetByIdAsync(int id)
		{
			return await db.CuentaContableSubtipos.Where(c => c.Id == id).FirstOrDefaultAsync();
		}

		public async Task<CuentaContableSubtipo?> GetByNameAsync(string name)
		{
			return await db.CuentaContableSubtipos.Where(c => c.Descripcion == name.ToLower()).FirstOrDefaultAsync();
		}
    }
}
