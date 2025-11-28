using ERPSEI.Data.Entities.Empleados;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Empleados
{
    public class OficinaManager : IRWCatalogoManager<Oficina>, IOficinaManager
    {
        ApplicationDbContext db { get; set; }

        public OficinaManager(ApplicationDbContext _db)
        {
            db = _db;
        }

        private async Task<int> getNextId()
        {
            List<Oficina> registros = await db.Oficinas.ToListAsync();
            Oficina? last = registros.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(Oficina oficina)
        {
            oficina.Id = await getNextId();
            db.Oficinas.Add(oficina);
            await db.SaveChangesAsync();
            return oficina.Id;
        }
        public async Task UpdateAsync(Oficina oficina)
        {
            Puesto? p = db.Find<Puesto>(oficina.Id);
            if (p != null)
            {
                p.Nombre = oficina.Nombre;
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Oficina oficina)
        {
            db.Oficinas.Remove(oficina);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            Oficina? oficina = await GetByIdAsync(id);
            if (oficina != null)
            {
                db.Remove(oficina);
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
                    Oficina? oficina = await GetByIdAsync(int.Parse(id));
                    if (oficina != null)
                    {
                        db.Remove(oficina);
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

        public async Task<List<Oficina>> GetAllAsync()
        {
            return await db.Oficinas.ToListAsync();
        }

        public async Task<Oficina?> GetByIdAsync(int id)
        {
            return await db.Oficinas.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Oficina?> GetByNameAsync(string name)
        {
            return await db.Oficinas.Where(a => a.Nombre.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        }

    }
}
