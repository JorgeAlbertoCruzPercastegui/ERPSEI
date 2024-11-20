using ERPSEI.Data.Entities.Cuentas;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using MathNet.Numerics.RootFinding;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Cuentas
{
	public class CuentaContableManager(ApplicationDbContext db) : ICuentaContableManager
	{
		public async Task<int> CreateAsync(CuentaContable element)
		{
			db.CuentasContables.Add(element);
			await db.SaveChangesAsync();
			return element.Id;
		}

		public async Task DeleteAsync(CuentaContable element)
		{
			db.CuentasContables.Remove(element);
			await db.SaveChangesAsync();
		}

		public async Task DeleteByIdAsync(int id)
		{
			CuentaContable? cuenta = await GetByIdAsync(id);
			if (cuenta != null) { await DeleteAsync(cuenta); }
		}

		public async Task DeleteMultipleByIdAsync(string[] ids)
		{
			//Inicia una transacción.
			await db.Database.BeginTransactionAsync();
			try
			{
				foreach (string id in ids) { await DeleteByIdAsync(int.Parse(id)); }

				await db.Database.CommitTransactionAsync();
			}
			catch (Exception)
			{
				await db.Database.RollbackTransactionAsync();
				throw;
			}
		}

		public async Task<List<CuentaContable>> GetAllAsync()
		{
			return await db.CuentasContables.Include(c => c.Empresa).ToListAsync();
		}

		public async Task<CuentaContable?> GetByIdAsync(int id)
		{
			return await db.CuentasContables.Include(c => c.Empresa).Where(c => c.Id == id).FirstOrDefaultAsync();
		}

		public async Task<CuentaContable?> GetByNameAsync(string name)
		{
			return await db.CuentasContables.Include(c => c.Empresa).Where(c => c.Nombre == name.ToLower()).FirstOrDefaultAsync();
		}

		public async Task<List<CuentaContable>> GetByIdEmpresaAsync(int id)
		{
			return await db.CuentasContables.Include(c => c.Empresa).Where(c => c.EmpresaId == id).ToListAsync();
		}

		public async Task UpdateAsync(CuentaContable element)
		{
			CuentaContable? c = db.Find<CuentaContable>(element.Id);
			if (c != null)
			{
				c.Cuenta = element.Cuenta;
				c.Nombre = element.Nombre;
				c.RFC = element.RFC;
				c.EmpresaId = element.EmpresaId;
				c.TipoId = element.TipoId;
				c.SubtipoId = element.SubtipoId;

				await db.SaveChangesAsync();
			}
		}

		public Task<List<CuentaContable>> SearchCuentas(string text, string receptorRFC, int tipoCuentaId, int subtipoCuentaId)
		{
			List<CuentaContable> cuentas = [..db.CuentasContables.Include(cc => cc.Empresa).ToList().Where(cc => cc.Empresa.RFC == receptorRFC).Where(cc => cc.TipoId == tipoCuentaId).Where(cc => cc.SubtipoId == subtipoCuentaId).Where(c => c.Cuenta.Contains(text, StringComparison.InvariantCultureIgnoreCase) || c.Nombre.Contains(text, StringComparison.InvariantCultureIgnoreCase)).Take(20)];

			return Task.FromResult(cuentas);
        }

        public async Task<List<string>> GetFilteredAsync(int empresaId, int subtipoId, int tipoId, string rfc)
        {
            return await db.CuentasContables
                .Where(c => c.EmpresaId == empresaId &&
                            c.SubtipoId == subtipoId &&
                            c.TipoId == tipoId &&
                            c.RFC == rfc)
                .Select(c => c.Cuenta) // Selecciona únicamente la propiedad 'Cuenta'
                .ToListAsync();
        }

    }
}
