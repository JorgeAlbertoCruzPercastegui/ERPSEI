using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Entities.Documentos;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Documentos
{
    public class DocumentoManager(ApplicationDbContext db) : IDocumentoManager
    {
        private async Task<int> GetNextId()
        {
            List<Documento> documento = await db.Documentos.ToListAsync();
            Documento? last = documento.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(Documento documento)
        {
            documento.Id = await GetNextId();
            db.Documentos.Add(documento);
            await db.SaveChangesAsync();
            return documento.Id;
        }

        public async Task UpdateAsync(Documento documentos)
        {
            var a = await db.Documentos.FindAsync(documentos.Id);
            if (a != null)
            {
                a.AreaId = documentos.AreaId;
                a.TipoDocumentoId = documentos.TipoDocumentoId;
                a.Titulo = documentos.Titulo;
                a.Descripcion = documentos.Descripcion;
                a.Activo = documentos.Activo;
                a.ModificadoPorId = documentos.ModificadoPorId;
                a.FechaModificacion = DateTime.Now;

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
            Documento? documento = await GetByIdAsync(id);
            if (documento != null)
            {
                db.Remove(documento);
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
                    Documento? documento = await GetByIdAsync(int.Parse(id));
                    if (documento != null)
                    {
                        db.Remove(documento);
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

        public async Task<List<Documento>> GetAllAsync()
        {
            return await db.Documentos
                .Include(d => d.Area)
                .Include(d => d.TipoDocumento)
                .Include(d => d.Versiones)
                .Include(d => d.PalabrasClave)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<Documento?> GetByIdAsync(int id)
        {
            return await db.Documentos.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Documento?> GetByNameAsync(string name)
        {
            return await db.Documentos.Where(a => a.Descripcion.ToLower() == name.ToLower() || a.Descripcion.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<List<Documento>> GetAllAsync(
    ERPSEI.Areas.Reportes.Pages.DocumentacionModel.InputFiltroModel? filtro = null)
        {
            var query = db.Documentos
                .Include(d => d.Area)
                .Include(d => d.TipoDocumento)
                .Include(d => d.Versiones)
                .Include(d => d.PalabrasClave)
                .AsQueryable();

            if (filtro != null)
            {
                if (!string.IsNullOrWhiteSpace(filtro.Titulo))
                    query = query.Where(d => d.Titulo.Contains(filtro.Titulo));

                if (filtro.AreaId.HasValue && filtro.AreaId.Value != 0)
                    query = query.Where(d => d.AreaId == filtro.AreaId.Value);

                if (filtro.TipoDocumentoId.HasValue && filtro.TipoDocumentoId.Value != 0)
                    query = query.Where(d => d.TipoDocumentoId == filtro.TipoDocumentoId.Value);

                if (filtro.EstatusDocumentoId.HasValue && filtro.EstatusDocumentoId.Value != 0)
                {
                    int estatusId = filtro.EstatusDocumentoId.Value;
                    query = query.Where(d =>
                        d.Versiones != null &&
                        d.Versiones.Any(v => v.EsActual && v.EstatusDocumentoId == estatusId));
                }

                if (!string.IsNullOrWhiteSpace(filtro.PalabraClave))
                {
                    var kw = filtro.PalabraClave.Trim();
                    query = query.Where(d =>
                        d.PalabrasClave != null &&
                        d.PalabrasClave.Any(p => p.Palabra.Contains(kw)));
                }

                if (filtro.FechaCreacionInicio.HasValue && !filtro.FechaCreacionFin.HasValue)
                {
                    var d1 = filtro.FechaCreacionInicio.Value.Date;
                    query = query.Where(d => d.FechaCreacion >= d1 && d.FechaCreacion < d1.AddDays(1));
                }
                else if (filtro.FechaCreacionInicio.HasValue && filtro.FechaCreacionFin.HasValue)
                {
                    var d1 = filtro.FechaCreacionInicio.Value.Date;
                    var d2 = filtro.FechaCreacionFin.Value.Date;

                    if (d2 < d1)
                        throw new InvalidOperationException("La fecha fin no puede ser menor que la fecha inicio.");

                    query = query.Where(d => d.FechaCreacion >= d1 && d.FechaCreacion < d2.AddDays(1));
                }
                else if (!filtro.FechaCreacionInicio.HasValue && filtro.FechaCreacionFin.HasValue)
                {
                    var d2 = filtro.FechaCreacionFin.Value.Date;
                    query = query.Where(d => d.FechaCreacion >= d2 && d.FechaCreacion < d2.AddDays(1));
                }
            }

            return await query.AsNoTracking().ToListAsync();
        }

    }
}
