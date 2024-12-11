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
			List<PolizaDetalle> polizasDetalles = await db.PolizasDetalles.ToListAsync();
			PolizaDetalle? last = polizasDetalles.OrderByDescending(r => r.Id).FirstOrDefault();
			int lastId = last != null ? last.Id : 0;
			lastId += 1;

			return lastId;
		}

		public async Task<int> CreateAsync(PolizaDetalle polizasDetalles)
		{
			polizasDetalles.Id = await GetNextId();
			db.PolizasDetalles.Add(polizasDetalles);
			await db.SaveChangesAsync();
			return polizasDetalles.Id;
		}

		public async Task UpdateAsync(PolizaDetalle polizasDetalles)
		{
			PolizaDetalle? a = db.Find<PolizaDetalle>(polizasDetalles.Id);
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

		public async Task DeleteAsync(PolizaDetalle polizasDetalles)
		{
			db.PolizasDetalles.Remove(polizasDetalles);
			await db.SaveChangesAsync();
		}

		public async Task DeleteByIdAsync(int id)
		{
			PolizaDetalle? polizasDetalles = await GetByIdAsync(id);
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
					PolizaDetalle? polizasDetalles = await GetByIdAsync(int.Parse(id));
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

		public async Task<List<PolizaDetalle>> GetAllAsync()
		{
			return await GetAllAsync(null, null, null, null, null, null);
		}

		public async Task<List<PolizaDetalle>> GetAllAsync(
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

		public async Task<PolizaDetalle?> GetByIdAsync(int id)
		{
			return await db.PolizasDetalles
				.Where(pd => pd.Id == id)
				.Include(pd => pd.Poliza).ThenInclude(p => p.Grupo)
				.Include(pd => pd.Poliza).ThenInclude(p => p.Tipo)
				.Include(pd => pd.Cuenta)
				.FirstOrDefaultAsync();
		}

		public async Task<PolizaDetalle?> GetByNameAsync(string desc)
		{
			return await db.PolizasDetalles.Where(a => a.Concepto.ToLower() == desc.ToLower()).FirstOrDefaultAsync();
		}
	}
}
