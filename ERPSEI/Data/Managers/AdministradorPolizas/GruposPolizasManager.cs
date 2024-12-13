using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Managers.AdministradorPolizas;
using ERPSEI.Data.Managers.Conciliaciones;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Polizas
{
	public class GruposPolizasManager(ApplicationDbContext db) : IGruposPolizasManager
	{
		private async Task<int> GetNextId()
		{
			List<GrupoPoliza> gruposPolizas = await db.GruposPolizas.ToListAsync();
			GrupoPoliza? last = gruposPolizas.OrderByDescending(r => r.Id).FirstOrDefault();
			int lastId = last != null ? last.Id : 0;
			lastId += 1;

			return lastId;
		}
		public async Task<int> CreateAsync(GrupoPoliza gruposPolizas)
		{
			gruposPolizas.Id = await GetNextId();
			db.GruposPolizas.Add(gruposPolizas);
			await db.SaveChangesAsync();
			return gruposPolizas.Id;
		}

		public async Task UpdateAsync(GrupoPoliza gruposPolizas)
		{
			GrupoPoliza? a = db.Find<GrupoPoliza>(gruposPolizas.Id);
			if (a != null)
			{
				a.UsuarioCreador = gruposPolizas.UsuarioCreador;
				a.UsuarioModificador = gruposPolizas.UsuarioModificador;
				a.FechaHoraCreacion = gruposPolizas.FechaHoraCreacion;
				a.FechaHoraModificacion = gruposPolizas.FechaHoraModificacion;
				a.NumeroImpresion = gruposPolizas.NumeroImpresion;
				a.Deshabilitado = gruposPolizas.Deshabilitado;
				await db.SaveChangesAsync();
			}
		}

		public async Task DeleteAsync(GrupoPoliza gruposPolizas)
		{
			db.GruposPolizas.Remove(gruposPolizas);
			await db.SaveChangesAsync();
		}

		public async Task DeleteByIdAsync(int id)
		{
			GrupoPoliza? gruposPolizas = await GetByIdAsync(id);
			if (gruposPolizas != null)
			{
				db.Remove(gruposPolizas);
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
					GrupoPoliza? gruposPolizas = await GetByIdAsync(int.Parse(id));
					if (gruposPolizas != null)
					{
						db.Remove(gruposPolizas);
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
		public async Task<List<GrupoPoliza>> GetAllAsync()
		{
			return await GetAllAsync(null, null, null, null, null, null, false);
		}

		public async Task<List<GrupoPoliza>> GetAllAsync(
			int? id = null,
			string? usuarioCreador = null,
			string? usuarioModificador = null,
			DateTime? fechaHoraCreacion = null,
			DateTime? fechaHoraModificacion = null,
			int? numeroImpresion = null,
			bool deshabilitado = false)
		{
			return await db.GruposPolizas
				.Where(e => deshabilitado || e.Deshabilitado == deshabilitado)
				.Where(e => id == null || e.Id == id)
				.Where(e => usuarioCreador == null || e.UsuarioCreadorId == usuarioCreador)
				.Where(e => usuarioModificador == null || e.UsuarioModificadorId == usuarioModificador)
				.Where(e => fechaHoraCreacion == null || e.FechaHoraCreacion >= fechaHoraCreacion)
				.Where(e => fechaHoraModificacion == null || e.FechaHoraModificacion <= fechaHoraModificacion)
				.Where(e => numeroImpresion == null || e.NumeroImpresion == numeroImpresion)
				.Include(e => e.Polizas).ThenInclude(p => p.PolizasDetalles).ThenInclude(pd => pd.Cuenta)
				.Include(e => e.Polizas).ThenInclude(p => p.Tipo)
				.Include(e => e.UsuarioCreador).ThenInclude(u => u.Empleado)
				.Include(e => e.UsuarioModificador).ThenInclude(u => u.Empleado)
				.ToListAsync();
		}

		public async Task<GrupoPoliza?> GetByIdAsync(int id)
		{
			return await db.GruposPolizas
				.Where(e => e.Id == id)
				.Include(e => e.UsuarioCreador)
				.Include(e => e.UsuarioModificador)
				.Include(e => e.Polizas).ThenInclude(p => p.PolizasDetalles).ThenInclude(pd => pd.Cuenta)
				.Include(e => e.Polizas).ThenInclude(p => p.Tipo)
				.FirstOrDefaultAsync();
		}

		//Verificar este método
		public async Task<GrupoPoliza?> GetByNameAsync(string desc)
		{
			return await db.GruposPolizas
				.Where(e => e.Polizas.Any(p => p.Concepto.Contains(desc)))
				.Include(e => e.UsuarioCreador)
				.Include(e => e.UsuarioModificador)
				.Include(e => e.Polizas).ThenInclude(p => p.PolizasDetalles).ThenInclude(pd => pd.Cuenta)
				.Include(e => e.Polizas).ThenInclude(p => p.Tipo)
				.FirstOrDefaultAsync();
		}
	}
}
