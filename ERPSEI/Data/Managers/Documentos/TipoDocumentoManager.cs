using ERPSEI.Data.Entities.Documentos;
using Microsoft.EntityFrameworkCore;
using static ERPSEI.Areas.Reportes.Pages.DocumentacionModel;

namespace ERPSEI.Data.Managers.Documentos
{
    public class TipoDocumentoManager(ApplicationDbContext db) : ITipoDocumentoManager
    {
        private async Task<int> GetNextId()
        {
            List<TipoDocumento> tipodocumento = await db.TiposDocumento.ToListAsync();
            TipoDocumento? last = tipodocumento.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(TipoDocumento tipoDocumento)
        {
            tipoDocumento.Id = await GetNextId();
            db.TiposDocumento.Add(tipoDocumento);
            await db.SaveChangesAsync();
            return tipoDocumento.Id;
        }

        public async Task UpdateAsync(TipoDocumento tipoDocumento)
        {
            var e = await db.TiposDocumento.FindAsync(tipoDocumento.Id);
            if (e != null)
            {
                e.Nombre = tipoDocumento.Nombre;
                e.FechaCreacion = tipoDocumento.FechaCreacion;
                e.Activo = tipoDocumento.Activo;

                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(TipoDocumento tipoDocumento)
        {
            db.TiposDocumento.Remove(tipoDocumento);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var doc = await db.TiposDocumento.FindAsync(id);
            if (doc == null) return;

            db.TiposDocumento.Remove(doc);
            await db.SaveChangesAsync();
        }

        public async Task DeleteMultipleByIdAsync(string[] ids)
        {
            //Inicia una transacción.
            await db.Database.BeginTransactionAsync();
            try
            {
                foreach (string id in ids)
                {
                    TipoDocumento? tipoDocumento = await GetByIdAsync(int.Parse(id));
                    if (tipoDocumento != null)
                    {
                        db.Remove(tipoDocumento);
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

        public async Task<List<TipoDocumento>> GetAllAsync()
        {
            return await db.TiposDocumento
                .AsNoTracking()
                .ToListAsync();
        }

        // ✅ Si quieres un overload parecido, cámbialo a includeDocumentos
        public async Task<List<TipoDocumento>> GetAllAsync(bool includeDocumentos)
        {
            var query = db.TiposDocumento.AsQueryable();

            if (includeDocumentos)
                query = query.Include(t => t.Documentos);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<TipoDocumento?> GetByIdAsync(int id)
        {
            return await db.TiposDocumento
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TipoDocumento?> GetByNameAsync(string name)
        {
            var n = (name ?? "").Trim().ToLower();

            return await db.TiposDocumento
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Nombre.ToLower() == n);
        }

        public async Task<List<TipoDocumento>> GetByNameContainsAsync(string name)
        {
            var n = (name ?? "").Trim();

            return await db.TiposDocumento
                .Where(t => t.Nombre.Contains(n))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<TipoDocumento>> GetAllAsync(TipoDocumentoFiltroModel? filtro = null)
        {
            var query = db.TiposDocumento.AsQueryable();

            if (filtro != null)
            {
                if (filtro.Id.HasValue && filtro.Id.Value != 0)
                    query = query.Where(t => t.Id == filtro.Id.Value);

                if (!string.IsNullOrWhiteSpace(filtro.Nombre))
                    query = query.Where(t => t.Nombre.Contains(filtro.Nombre));

                if (filtro.Activo.HasValue)
                    query = query.Where(t => t.Activo == filtro.Activo.Value);
            }

            return await query.AsNoTracking().ToListAsync();
        }

    }
}
