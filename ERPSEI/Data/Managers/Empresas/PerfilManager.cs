using ERPSEI.Data.Entities.Empresas;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Empresas
{
    public class PerfilManager : IPerfilManager
    {
        ApplicationDbContext db { get; set; }

        public PerfilManager(ApplicationDbContext _db)
        {
            db = _db;
        }

		private async Task<int> getNextId()
		{
			List<Perfil> registros = await db.Perfiles.ToListAsync();
			Perfil? last = registros.OrderByDescending(r => r.Id).FirstOrDefault();
			int lastId = last != null ? last.Id : 0;
			lastId += 1;

			return lastId;
		}

		public async Task<int> CreateAsync(Perfil p)
        {
            p.Id = await getNextId();
            db.Perfiles.Add(p);
            await db.SaveChangesAsync();
            return p.Id;
        }
        public async Task UpdateAsync(Perfil p)
        {
			Perfil? a = db.Find<Perfil>(p.Id);
            if (a != null)
            {
                a.Nombre = p.Nombre;
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Perfil p)
        {
            db.Perfiles.Remove(p);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
			Perfil? p = await GetByIdAsync(id);
            if (p != null)
            {
                db.Remove(p);
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
					Perfil? p = await GetByIdAsync(int.Parse(id));
					if (p != null)
					{
						foreach(ProductoServicioPerfil psp in p.ProductosServiciosPerfil)
						{
							db.Remove(psp);
						}
						db.Remove(p);
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

		public async Task<List<Perfil>> GetAllAsync()
		{
			return await db.Perfiles
				.Include(p => p.ProductosServiciosPerfil)
				.ThenInclude(psp => psp.ProductoServicio)
				.ToListAsync();
		}

		public async Task<Perfil?> GetByIdAsync(int id)
        {
            return await db.Perfiles.Where(a => a.Id == id).Include(p => p.ProductosServiciosPerfil).FirstOrDefaultAsync();
        }

		public async Task<Perfil?> GetByNameAsync(string name)
		{
			return await db.Perfiles.Where(a => a.Nombre.ToLower() == name.ToLower()).FirstOrDefaultAsync();
		}

	}
}
