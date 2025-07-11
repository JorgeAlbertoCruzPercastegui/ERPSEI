using ERPSEI.Data.Entities.TipoContratos;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.TipoContratos
{
    public class EmpresaContratosManager(ApplicationDbContext db) : IEmpresaContratosManager
    {
        private async Task<int> GetNextId()
        {
            List<EmpresaContrato> empresaContrato = await db.EmpresaContratos.ToListAsync();
            EmpresaContrato? last = empresaContrato.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(EmpresaContrato empresaContrato)
        {
            empresaContrato.Id = await GetNextId();
            db.EmpresaContratos.Add(empresaContrato);
            await db.SaveChangesAsync();
            return empresaContrato.Id;
        }

        public async Task UpdateAsync(EmpresaContrato empresaContrato)
        {
            EmpresaContrato? a = await db.EmpresaContratos.FindAsync(empresaContrato.Id);

            if (a != null)
            {
                a.FechaConstitucion = empresaContrato.FechaConstitucion;
                a.RazonSocial = empresaContrato.RazonSocial;
                a.DomicilioFiscal = empresaContrato.DomicilioFiscal;
                a.RFC = empresaContrato.RFC;
                a.NoNotario = empresaContrato.NoNotario;
                a.Notario = empresaContrato.Notario;
                a.RepresentanteLegal = empresaContrato.RepresentanteLegal;
                a.Email = empresaContrato.Email;
                a.PaginaWeb = empresaContrato.PaginaWeb;
                a.Deshabilitado = empresaContrato.Deshabilitado;
                a.TipoContratoId = empresaContrato.TipoContratoId;

                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(EmpresaContrato empresaContrato)
        {
            db.EmpresaContratos.Remove(empresaContrato);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            EmpresaContrato? empresaContrato = await GetByIdAsync(id);
            if (empresaContrato != null)
            {
                db.Remove(empresaContrato);
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
                    EmpresaContrato? empresaContrato = await GetByIdAsync(int.Parse(id));
                    if (empresaContrato != null)
                    {
                        db.Remove(empresaContrato);
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

        public async Task<List<EmpresaContrato>> GetAllAsync()
        {
            //return await db.EmpresaContratos.ToListAsync();
            return await db.EmpresaContratos
               .Include(ec => ec.TipoContrato)
               .ToListAsync();
        }

        public async Task<EmpresaContrato?> GetByIdAsync(int id)
        {
            return await db.EmpresaContratos.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<EmpresaContrato?> GetByNameAsync(string name)
        {
            return await db.EmpresaContratos
                .FirstOrDefaultAsync(a => a.RazonSocial != null &&
                                          a.RazonSocial.Trim().ToLower() == name.Trim().ToLower());
        }

        public async Task<List<EmpresaContrato>> GetAllAsync(ERPSEI.Areas.Reportes.Pages.GeneradorContratoModel.InputFiltroModel? filtro = null)
        {
            var query = db.EmpresaContratos
                .Include(e => e.TipoContrato)
                .AsQueryable();

            if (filtro != null)
            {
                if (filtro.TipoContratoId.HasValue && filtro.TipoContratoId.Value != 0)
                    query = query.Where(e => e.TipoContratoId == filtro.TipoContratoId.Value);

                /*if (filtro.PrestadorId.HasValue && filtro.PrestadorId.Value != 0)
                    query = query.Where(e => e.PrestadorId == filtro.PrestadorId.Value);

                if (filtro.PrestatarioId.HasValue && filtro.PrestatarioId.Value != 0)
                    query = query.Where(e => e.PrestatarioId == filtro.PrestatarioId.Value);*/
            }

            return await query.ToListAsync();
        }


    }
}
