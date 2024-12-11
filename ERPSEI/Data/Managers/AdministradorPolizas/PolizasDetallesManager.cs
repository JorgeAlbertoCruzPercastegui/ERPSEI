using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Managers.AdministradorPolizas;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Polizas
{
	public class PolizasDetallesManager(ApplicationDbContext db) : IPolizasDetalles
	{
		private async Task<int> GetNextId()
		{
			List<PolizasDetalles> polizasDetalles = await db.PolizasDetalles.ToListAsync();
			PolizasDetalles? last = polizasDetalles.OrderByDescending(r => r.Id).FirstOrDefault();
			int lastId = last != null ? last.Id : 0;
			lastId += 1;

			return lastId;
		}

		public async Task<int> CreateAsync(PolizasDetalles polizasDetalles)
		{
			polizasDetalles.Id = await GetNextId();
			db.PolizasDetalles.Add(polizasDetalles);
			await db.SaveChangesAsync();
			return polizasDetalles.Id;
		}

		public async Task UpdateAsync(PolizasDetalles polizasDetalles)
		{
			PolizasDetalles? a = db.Find<PolizasDetalles>(polizasDetalles.Id);
			if (a != null)
			{
				a.PolizaId = polizasDetalles.PolizaId;
				a.CuentaId = polizasDetalles.CuentaId;
				a.Concepto = polizasDetalles.Concepto;
				a.Debe = polizasDetalles.Debe;
				a.Haber = polizasDetalles.Haber;
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(PolizasDetalles polizasDetalles)
		{
			db.PolizasDetalles.Remove(polizasDetalles);
			await db.SaveChangesAsync();
		}

		public async Task DeleteByIdAsync(int id)
		{
			PolizasDetalles? polizasDetalles = await GetByIdAsync(id);
			if (polizasDetalles != null)
			{
				db.Remove(polizasDetalles);
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
					PolizasDetalles? polizasDetalles = await GetByIdAsync(int.Parse(id));
					if (polizasDetalles != null)
					{
						db.Remove(polizasDetalles);
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

		public async Task<List<PolizasDetalles>> GetAllAsync()
		{
			return await GetAllAsync(null, null, null, null, null, null);
		}

		public async Task<List<PolizasDetalles>> GetAllAsync(
		int? id = null,
		int? polizaId = null,
		int? cuentaId = null,
		string? concepto = null,
		decimal? debe = null,
		decimal? haber = null)
		{
			return await db.PolizasDetalles
				.Where(pd => id == null || pd.Id == id)
				.Where(pd => polizaId == null || pd.PolizaId == polizaId)
				.Where(pd => cuentaId == null || pd.CuentaId == cuentaId)
				.Where(pd => string.IsNullOrEmpty(concepto) || pd.Concepto.Contains(concepto))
				.Where(pd => debe == null || pd.Debe == debe)
				.Where(pd => haber == null || pd.Haber == haber)
				.Include(pd => pd.Poliza).ThenInclude(p => p.Grupo)
				.Include(pd => pd.Poliza).ThenInclude(p => p.Tipo)
				.Include(pd => pd.Cuenta)
				.ToListAsync();
		}

		public async Task<PolizasDetalles?> GetByIdAsync(int id)
		{
			return await db.PolizasDetalles
				.Where(pd => pd.Id == id)
				.Include(pd => pd.Poliza).ThenInclude(p => p.Grupo)
				.Include(pd => pd.Poliza).ThenInclude(p => p.Tipo)
				.Include(pd => pd.Cuenta)
				.FirstOrDefaultAsync();
		}

		public async Task<PolizasDetalles?> GetByNameAsync(string desc)
		{
			return await db.PolizasDetalles.Where(a => a.Concepto.ToLower() == desc.ToLower()).FirstOrDefaultAsync();
		}
	}
}
