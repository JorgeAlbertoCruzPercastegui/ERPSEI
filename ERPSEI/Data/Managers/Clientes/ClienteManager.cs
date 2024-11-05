using ERPSEI.Data.Entities.Clientes;
using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Clientes
{
    public class ClienteManager(ApplicationDbContext db) : IClienteManager
    {
        private async Task<int> GetNextId()
        {
            List<Cliente> cliente = await db.Clientes.ToListAsync();
            Cliente? last = cliente.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(Cliente cliente)
        {
            cliente.Id = await GetNextId();
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();
            return cliente.Id;
        }
        public async Task UpdateAsync(Cliente cliente)
        {
            Cliente? a = db.Find<Cliente>(cliente.Id);
            if (a != null)
            {
                a.RegimenFiscalId = cliente.RegimenFiscalId;
                a.RegimenFiscal = cliente.RegimenFiscal;
                a.RazonSocial = cliente.RazonSocial;
                a.RFC = cliente.RFC;
                a.DomicilioFiscal = cliente.DomicilioFiscal;
                a.ResidenciaFiscal = cliente.ResidenciaFiscal;
                a.Telefono = cliente.Telefono;
                a.Correo = cliente.Correo;
                a.Deshabilitado = cliente.Deshabilitado;

                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Cliente cliente)
        {
            db.Clientes.Remove(cliente);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            Cliente? cliente = await GetByIdAsync(id);
            if (cliente != null)
            {
                db.Remove(cliente);
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
                    Cliente? cliente = await GetByIdAsync(int.Parse(id));
                    if (cliente != null)
                    {
                        db.Remove(cliente);
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

        public async Task<List<Cliente>> GetAllAsync()
        {
            return await db.Clientes.ToListAsync();
        }

        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await db.Clientes.Where(p => p.Id == id).FirstOrDefaultAsync();
        }
        //Verificar por el nombre que no existe en la entidad
        public async Task<Cliente?> GetByNameAsync(string name)
        {
            return await db.Clientes.Where(a => a.RazonSocial.ToLower() == name.ToLower() || a.RFC.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        }

        /*public async Task<List<Cliente>> SearchClientes(string texto)
        {
            List<Cliente> clientes = await db.Clientes
                .Where(c => !c.Deshabilitado)
                .Where(c =>
                    (c.RazonSocial != null && c.RazonSocial.Contains(texto)) ||
                    (c.RFC != null && c.RFC.Contains(texto)) ||
                    (c.DomicilioFiscal != null && c.DomicilioFiscal.Contains(texto)) ||
                    (c.Telefono != null && c.Telefono.Contains(texto)) ||
                    (c.Correo != null && c.Correo.Contains(texto)))
                .Take(20)
                .ToListAsync();

            return clientes;
        }

        Task<List<ClienteBuscado>> IClienteManager.SearchClientes(string texto)
        {
            throw new NotImplementedException();
        }*/
    }
}
