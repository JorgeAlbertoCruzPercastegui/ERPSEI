using ERPSEI.Data.Entities.Documentos;
using Microsoft.EntityFrameworkCore;
using static ERPSEI.Areas.Reportes.Pages.DocumentacionModel;

namespace ERPSEI.Data.Managers.Documentos
{
    public class EstatusDocumentoManager(ApplicationDbContext db) : IEstatusDocumentoManager
    {
        private async Task<int> GetNextId()
        {
            List<EstatusDocumento> documento = await db.DocumentosEstatus.ToListAsync();
            EstatusDocumento? last = documento.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(EstatusDocumento estatusDocumento)
        {
            estatusDocumento.Id = await GetNextId();
            db.DocumentosEstatus.Add(estatusDocumento);
            await db.SaveChangesAsync();
            return estatusDocumento.Id;
        }

        public async Task UpdateAsync(EstatusDocumento estatusDocumento)
        {
            var e = await db.DocumentosEstatus.FindAsync(estatusDocumento.Id);
            if (e != null)
            {
                e.Nombre = estatusDocumento.Nombre;
                e.EsPublicable = estatusDocumento.EsPublicable;
                e.Activo = estatusDocumento.Activo;

                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Documento documento)
        {
            db.Documentos.Remove(documento);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var doc = await db.Documentos.FindAsync(id);
            if (doc == null) return;

            db.Documentos.Remove(doc);
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
                    EstatusDocumento? estatusDocumento = await GetByIdAsync(int.Parse(id));
                    if (estatusDocumento != null)
                    {
                        db.Remove(estatusDocumento);
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

        public async Task<List<EstatusDocumento>> GetAllAsync()
        {
            return await db.DocumentosEstatus
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<EstatusDocumento>> GetAllAsync(bool includeVersiones)
        {
            var query = db.DocumentosEstatus.AsQueryable();

            if (includeVersiones)
                query = query.Include(e => e.Versiones);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<EstatusDocumento?> GetByIdAsync(int id)
        {
            return await db.DocumentosEstatus
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<EstatusDocumento?> GetByNameAsync(string name)
        {
            var n = name.Trim().ToLower();

            return await db.DocumentosEstatus
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Nombre.ToLower() == n);
        }

        public async Task<List<EstatusDocumento>> GetByNameContainsAsync(string name)
        {
            var n = name.Trim();

            return await db.DocumentosEstatus
                .Where(e => e.Nombre.Contains(n))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<EstatusDocumento>> GetAllAsync(EstatusDocumentoFiltroModel? filtro = null)
        {
            var query = db.DocumentosEstatus.AsQueryable();

            if (filtro != null)
            {
                if (filtro.Id.HasValue && filtro.Id.Value != 0)
                    query = query.Where(e => e.Id == filtro.Id.Value);

                if (!string.IsNullOrWhiteSpace(filtro.Nombre))
                    query = query.Where(e => e.Nombre.Contains(filtro.Nombre));

                if (filtro.Activo.HasValue)
                    query = query.Where(e => e.Activo == filtro.Activo.Value);

                if (filtro.EsPublicable.HasValue)
                    query = query.Where(e => e.EsPublicable == filtro.EsPublicable.Value);
            }

            return await query.AsNoTracking().ToListAsync();
        }







    }
}
