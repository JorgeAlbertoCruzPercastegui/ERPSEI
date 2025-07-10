using ERPSEI.Data.Entities.TipoContratos;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.TipoContratos
{
    public class ClienteContratosManager(ApplicationDbContext db) : IClienteContratosManager
    {
        private async Task<int> GetNextId()
        {
            List<ClienteContrato> clienteContrato = await db.ClienteContratos.ToListAsync();
            ClienteContrato? last = clienteContrato.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(ClienteContrato clienteContrato)
        {
            clienteContrato.Id = await GetNextId();
            db.ClienteContratos.Add(clienteContrato);
            await db.SaveChangesAsync();
            return clienteContrato.Id;
        }

        public async Task UpdateAsync(ClienteContrato clienteContrato)
        {
            ClienteContrato? a = await db.ClienteContratos.FindAsync(clienteContrato.Id);

            if (a != null)
            {
                a.FechaConstitucion = clienteContrato.FechaConstitucion;
                a.RazonSocial = clienteContrato.RazonSocial;
                a.DomicilioFiscal = clienteContrato.DomicilioFiscal;
                a.RFC = clienteContrato.RFC;
                a.NoNotario = clienteContrato.NoNotario;
                a.Notario = clienteContrato.Notario;
                a.RepresentanteLegal = clienteContrato.RepresentanteLegal;
                a.Email = clienteContrato.Email;
                a.PaginaWeb = clienteContrato.PaginaWeb;
                a.Deshabilitado = clienteContrato.Deshabilitado;
                a.EmpresaContratoId = clienteContrato.EmpresaContratoId;

                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(ClienteContrato clienteContrato)
        {
            db.ClienteContratos.Remove(clienteContrato);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            ClienteContrato? clienteContrato = await GetByIdAsync(id);
            if (clienteContrato != null)
            {
                db.Remove(clienteContrato);
                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteMultipleByIdAsync(string[] ids)
        {
            await db.Database.BeginTransactionAsync();
            try
            {
                foreach (string id in ids)
                {
                    ClienteContrato? clienteContrato = await GetByIdAsync(int.Parse(id));
                    if (clienteContrato != null)
                    {
                        db.Remove(clienteContrato);
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

        public async Task<List<ClienteContrato>> GetAllAsync()
        {
            return await db.ClienteContratos.Include(c => c.EmpresaContrato).ToListAsync();
        }

        public async Task<ClienteContrato?> GetByIdAsync(int id)
        {
            return await db.ClienteContratos.Include(c => c.EmpresaContrato).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ClienteContrato?> GetByNameAsync(string name)
        {
            return await db.ClienteContratos
                .FirstOrDefaultAsync(a => a.RazonSocial != null &&
                                          a.RazonSocial.Trim().ToLower() == name.Trim().ToLower());
        }
    }
}