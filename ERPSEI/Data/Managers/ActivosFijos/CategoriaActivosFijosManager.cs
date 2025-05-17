using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Entities.Clientes;
using ERPSEI.Data.Managers.Clientes;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.ActivosFijos
{
    public class CategoriaActivosFijosManager(ApplicationDbContext db) : ICategoriaActivosFijosManager
    {
        private async Task<int> GetNextId()
        {
            List<CategoriaActivoFijo> categoria = await db.CategoriasActivosFijos.ToListAsync();
            CategoriaActivoFijo? last = categoria.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(CategoriaActivoFijo categoria)
        {
            categoria.Id = await GetNextId();
            db.CategoriasActivosFijos.Add(categoria);
            await db.SaveChangesAsync();
            return categoria.Id;
        }
        public async Task UpdateAsync(CategoriaActivoFijo categoria)
        {
            CategoriaActivoFijo? a = db.Find<CategoriaActivoFijo>(categoria.Id);
            if (a != null)
            {
                a.Descripcion = categoria.Descripcion;
                a.Deshabilitado = categoria.Deshabilitado;

                await db.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(CategoriaActivoFijo categoria)
        {
            db.CategoriasActivosFijos.Remove(categoria);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            CategoriaActivoFijo? categoria = await GetByIdAsync(id);
            if (categoria != null)
            {
                db.Remove(categoria);
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
                    CategoriaActivoFijo? categoria = await GetByIdAsync(int.Parse(id));
                    if (categoria != null)
                    {
                        db.Remove(categoria);
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

        public async Task<List<CategoriaActivoFijo>> GetAllAsync()
        {
            return await db.CategoriasActivosFijos.ToListAsync();
        }

        public async Task<CategoriaActivoFijo?> GetByIdAsync(int id)
        {
            return await db.CategoriasActivosFijos.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<CategoriaActivoFijo?> GetByNameAsync(string name)
        {
            return await db.CategoriasActivosFijos.Where(a => a.Descripcion.ToLower() == name.ToLower() || a.Descripcion.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        }

    }
}
