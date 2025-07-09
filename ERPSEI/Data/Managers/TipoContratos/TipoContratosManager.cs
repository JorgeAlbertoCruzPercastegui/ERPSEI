using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Entities.TipoContratos;
using ERPSEI.Data.Managers.Reportes;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.TipoContratos
{
    public class TipoContratosManager(ApplicationDbContext db) : ITipoContratosManager
    {
        private async Task<int> GetNextId()
        {
            List<TipoContrato> tipoContrato = await db.TipoContratos.ToListAsync();
            TipoContrato? last = tipoContrato.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(TipoContrato tipoContrato)
        {
            tipoContrato.Id = await GetNextId();
            db.TipoContratos.Add(tipoContrato);
            await db.SaveChangesAsync();
            return tipoContrato.Id;
        }

        public async Task UpdateAsync(TipoContrato tipoContrato)
        {
            TipoContrato? a = db.Find<TipoContrato>(tipoContrato.Id);
            if (a != null)
            {
                a.Descripcion = tipoContrato.Descripcion;
                a.Deshabilitado = tipoContrato.Deshabilitado;

                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(TipoContrato tipoContrato)
        {
            db.TipoContratos.Remove(tipoContrato);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            TipoContrato? tipoContrato = await GetByIdAsync(id);
            if (tipoContrato != null)
            {
                db.Remove(tipoContrato);
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
                    TipoContrato? tipoContrato = await GetByIdAsync(int.Parse(id));
                    if (tipoContrato != null)
                    {
                        db.Remove(tipoContrato);
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

        public async Task<List<TipoContrato>> GetAllAsync()
        {
            return await db.TipoContratos.ToListAsync();
        }

        public async Task<TipoContrato?> GetByIdAsync(int id)
        {
            return await db.TipoContratos.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<TipoContrato?> GetByNameAsync(string name)
        {
            return await db.TipoContratos.Where(a => a.Descripcion.ToLower() == name.ToLower() || a.Descripcion.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        }
    }
}
