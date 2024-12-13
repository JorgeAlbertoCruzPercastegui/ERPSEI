using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Managers.AdministradorPolizas;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Polizas
{
	public class PolizasTiposManager(ApplicationDbContext db) : IPolizasTipos
	{
		private async Task<int> GetNextId()
		{
			List<PolizaTipo> polizasTipos = await db.PolizasTipos.ToListAsync();
			PolizaTipo? last = polizasTipos.OrderByDescending(r => r.Id).FirstOrDefault();
			int lastId = last != null ? last.Id : 0;
			lastId += 1;

			return lastId;
		}

		public async Task<int> CreateAsync(PolizaTipo polizasTipos)
		{
			polizasTipos.Id = await GetNextId();
			db.PolizasTipos.Add(polizasTipos);
			await db.SaveChangesAsync();
			return polizasTipos.Id;
		}

		public async Task UpdateAsync(PolizaTipo polizasTipos)
		{
			PolizaTipo? a = db.Find<PolizaTipo>(polizasTipos.Id);
			if (a != null)
			{
				a.Id = polizasTipos.Id;
				a.Descripcion = polizasTipos.Descripcion;
				a.Deshabilitado = polizasTipos.Deshabilitado;
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(PolizaTipo polizasTipos)
		{
			db.PolizasTipos.Remove(polizasTipos);
			await db.SaveChangesAsync();
		}

		public async Task DeleteByIdAsync(int id)
		{
			PolizaTipo? polizasTipos = await GetByIdAsync(id);
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
					PolizaTipo? polizasTipos = await GetByIdAsync(int.Parse(id));
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

		public async Task<List<PolizaTipo>> GetAllAsync()
		{
			return await GetAllAsync(null, null, null);
		}

		public async Task<List<PolizaTipo>> GetAllAsync(
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

		public async Task<PolizaTipo?> GetByIdAsync(int id)
		{
			return await db.PolizasTipos
				.Where(pt => pt.Id == id)
				.FirstOrDefaultAsync();
		}

		public async Task<PolizaTipo?> GetByNameAsync(string desc)
		{
			return await db.PolizasTipos.Where(a => a.Descripcion.ToLower() == desc.ToLower()).FirstOrDefaultAsync();
		}
	}
}
