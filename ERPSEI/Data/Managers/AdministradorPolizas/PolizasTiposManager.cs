using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Managers.AdministradorPolizas;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Polizas
{
	public class PolizasTiposManager(ApplicationDbContext db) : IPolizasTipos
	{
		private async Task<int> GetNextId()
		{
			List<PolizasTipos> polizasTipos = await db.PolizasTipos.ToListAsync();
			PolizasTipos? last = polizasTipos.OrderByDescending(r => r.Id).FirstOrDefault();
			int lastId = last != null ? last.Id : 0;
			lastId += 1;

			return lastId;
		}

		public async Task<int> CreateAsync(PolizasTipos polizasTipos)
		{
			polizasTipos.Id = await GetNextId();
			db.PolizasTipos.Add(polizasTipos);
			await db.SaveChangesAsync();
			return polizasTipos.Id;
		}

		public async Task UpdateAsync(PolizasTipos polizasTipos)
		{
			PolizasTipos? a = db.Find<PolizasTipos>(polizasTipos.Id);
			if (a != null)
			{
				a.Id = polizasTipos.Id;
				a.Descripcion = polizasTipos.Descripcion;
				a.Deshabilitado = polizasTipos.Deshabilitado;
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(PolizasTipos polizasTipos)
		{
			db.PolizasTipos.Remove(polizasTipos);
			await db.SaveChangesAsync();
		}

		public async Task DeleteByIdAsync(int id)
		{
			PolizasTipos? polizasTipos = await GetByIdAsync(id);
			if (polizasTipos != null)
			{
				db.Remove(polizasTipos);
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteMultipleByIdAsync(string[] ids)
		{
			//Inicia una transacción.
			await db.Database.BeginTransactionAsync();
			try
			{
				foreach (string id in ids)
				{
					PolizasTipos? polizasTipos = await GetByIdAsync(int.Parse(id));
					if (polizasTipos != null)
					{
						db.Remove(polizasTipos);
						await db.SaveChangesAsync();
					}
				}

				await db.Database.CommitTransactionAsync();
			}
			catch (Exception)
			{
				await db.Database.RollbackTransactionAsync();
				throw;

			}
		}

		public async Task<List<PolizasTipos>> GetAllAsync()
		{
			return await GetAllAsync(null, null, null);
		}

		public async Task<List<PolizasTipos>> GetAllAsync(
		int? id = null,
		string? descripcion = null,
		bool? deshabilitado = null)
		{
			return await db.PolizasTipos
				.Where(pt => id == null || pt.Id == id)
				.Where(pt => string.IsNullOrEmpty(descripcion) || pt.Descripcion.Contains(descripcion))
				.Where(pt => deshabilitado == null || pt.Deshabilitado == deshabilitado)
				.ToListAsync();
		}

		public async Task<PolizasTipos?> GetByIdAsync(int id)
		{
			return await db.PolizasTipos
				.Where(pt => pt.Id == id)
				.FirstOrDefaultAsync();
		}

		public async Task<PolizasTipos?> GetByNameAsync(string desc)
		{
			return await db.PolizasTipos.Where(a => a.Descripcion.ToLower() == desc.ToLower()).FirstOrDefaultAsync();
		}
	}
}
