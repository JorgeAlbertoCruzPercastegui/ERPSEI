using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Entities.Clientes;
using ERPSEI.Data.Managers.Clientes;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.ActivosFijos
{
    public class TipoActivosFijosManager(ApplicationDbContext db) : ITipoActivosFijosManager
    {
        private async Task<int> GetNextId()
        {
            List<TipoActivoFijo> tipo = await db.TiposActivosFijos.ToListAsync();
            TipoActivoFijo? last = tipo.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(TipoActivoFijo tipo)
        {
            tipo.Id = await GetNextId();
            db.TiposActivosFijos.Add(tipo);
            await db.SaveChangesAsync();
            return tipo.Id;
        }
        public async Task UpdateAsync(TipoActivoFijo tipo)
        {
            TipoActivoFijo? a = db.Find<TipoActivoFijo>(tipo.Id);
            if (a != null)
            {
                a.Descripcion = tipo.Descripcion;
                a.PermiteMultiplesAsignaciones = tipo.PermiteMultiplesAsignaciones;
                a.Deshabilitado = tipo.Deshabilitado;

                await db.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(TipoActivoFijo tipo)
        {
            db.TiposActivosFijos.Remove(tipo);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            TipoActivoFijo? tipo = await GetByIdAsync(id);
            if (tipo != null)
            {
                db.Remove(tipo);
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
                    TipoActivoFijo? tipo = await GetByIdAsync(int.Parse(id));
                    if (tipo != null)
                    {
                        db.Remove(tipo);
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

        public async Task<List<TipoActivoFijo>> GetAllAsync()
        {
            return await db.TiposActivosFijos.ToListAsync();
        }

        public async Task<TipoActivoFijo?> GetByIdAsync(int id)
        {
            return await db.TiposActivosFijos.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<TipoActivoFijo?> GetByNameAsync(string name)
        {
            return await db.TiposActivosFijos.Where(a => a.Descripcion.ToLower() == name.ToLower() || a.Descripcion.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        }
    }
}
