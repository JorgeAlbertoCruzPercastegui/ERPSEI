using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Managers.AdministradorPolizas;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Polizas
{
	public class PolizasManager(ApplicationDbContext db) : IPolizasManager
	{
		private async Task<int> GetNextId()
		{
			List<VPoliza> polizas = await db.VPolizas.ToListAsync();
			VPoliza? last = polizas.OrderByDescending(r => r.Id).FirstOrDefault();
			int lastId = last != null ? last.Id : 0;
			lastId += 1;

			return lastId;
		}

		public async Task<int> CreateAsync(VPoliza polizas)
		{
			polizas.Id = await GetNextId();
			db.VPolizas.Add(polizas);
			await db.SaveChangesAsync();
			return polizas.Id;
		}

		public async Task UpdateAsync(VPoliza polizas)
		{
			VPoliza? a = db.Find<VPoliza>(polizas.Id);
			if (a != null)
			{
				a.GrupoId = polizas.GrupoId;
				a.TipoId = polizas.TipoId;
				a.FechaHora = polizas.FechaHora;
				a.Concepto = polizas.Concepto;
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(VPoliza polizas)
		{
			db.VPolizas.Remove(polizas);
			await db.SaveChangesAsync();
		}

		public async Task DeleteByIdAsync(int id)
		{
			VPoliza? polizas = await GetByIdAsync(id);
			if (polizas != null)
			{
				db.Remove(polizas);
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
					VPoliza? polizas = await GetByIdAsync(int.Parse(id));
					if (polizas != null)
					{
						db.Remove(polizas);
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

		public async Task<List<VPoliza>> GetAllAsync()
		{
			return await GetAllAsync(null, null, null, null, null);
		}

		public async Task<List<VPoliza>> GetAllAsync(
		int? id = null,
		int? grupoId = null,
		int? tipoId = null,
		DateTime? fechaHora = null,
		string? concepto = null)
		{
			return await db.VPolizas
				.Where(p => id == null || p.Id == id)
				.Where(p => grupoId == null || p.GrupoId == grupoId)
				.Where(p => tipoId == null || p.TipoId == tipoId)
				.Where(p => fechaHora == null || p.FechaHora == fechaHora)
				.Where(p => string.IsNullOrEmpty(concepto) || p.Concepto.Contains(concepto))
				.Include(p => p.Grupo)
				.Include(p => p.Tipo)
				.Include(p => p.PolizasDetalles).ThenInclude(pd => pd.Cuenta)
				.ToListAsync();
		}

		public async Task<VPoliza?> GetByIdAsync(int id)
		{
			return await db.VPolizas
				.Where(p => p.Id == id)
				.Include(p => p.Grupo)
				.Include(p => p.Tipo)
				.Include(p => p.PolizasDetalles).ThenInclude(pd => pd.Cuenta)
				.FirstOrDefaultAsync();
		}

		public async Task<VPoliza?> GetByNameAsync(string desc)
		{
			return await db.VPolizas.Where(a => a.Concepto.ToLower() == desc.ToLower()).FirstOrDefaultAsync();
		}
	}
}
