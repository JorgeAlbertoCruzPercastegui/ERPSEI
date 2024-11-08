using ERPSEI.Data.Entities.Cuentas;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Cuentas
{
	public class CuentaContableProductoServicioManager(ApplicationDbContext _db) : ICuentaContableProductoServicioManager
	{
		private async Task<int> getNextId()
		{
			List<CuentaContableProductoServicio> registros = await _db.CuentaContableProductosServicios.ToListAsync();
			CuentaContableProductoServicio? last = registros.OrderByDescending(r => r.Id).FirstOrDefault();
			int lastId = last != null ? last.Id : 0;
			lastId += 1;

			return lastId;
		}

		public async Task<int> CreateAsync(CuentaContableProductoServicio c)
        {
            c.Id = await getNextId();
            _db.CuentaContableProductosServicios.Add(c);
            await _db.SaveChangesAsync();
            return c.Id;
        }
        public async Task UpdateAsync(CuentaContableProductoServicio c)
        {
			CuentaContableProductoServicio? n = _db.Find<CuentaContableProductoServicio>(c.Id);
            if (n != null)
            {
                n.CuentaContableId = c.CuentaContableId;
				n.ProductoServicioId = c.ProductoServicioId;
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(CuentaContableProductoServicio c)
        {
            _db.CuentaContableProductosServicios.Remove(c);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
			CuentaContableProductoServicio? a = _db.Find<CuentaContableProductoServicio>(id);
            if (a != null)
            {
                _db.Remove(a);
                await _db.SaveChangesAsync();
            }
        }

		public async Task DeleteByCuentaIdAsync(int id)
		{
			List<CuentaContableProductoServicio> productosServicios = await _db.CuentaContableProductosServicios.Where(a => a.CuentaContableId == id).ToListAsync();
			if (productosServicios != null && productosServicios.Count >= 1) { _db.CuentaContableProductosServicios.RemoveRange(productosServicios); }
			await _db.SaveChangesAsync();
		}

		public async Task DeleteMultipleByIdAsync(string[] ids)
		{
			//Inicia una transacción.
			await _db.Database.BeginTransactionAsync();
			try
			{
				foreach (string id in ids) { await DeleteByIdAsync(int.Parse(id)); }

				await _db.Database.CommitTransactionAsync();
			}
			catch (Exception)
			{
				await _db.Database.RollbackTransactionAsync();
				throw;

			}
		}

		public async Task<List<CuentaContableProductoServicio>> GetAllAsync()
		{
			return await _db.CuentaContableProductosServicios.ToListAsync();
		}

		public async Task<CuentaContableProductoServicio?> GetByIdAsync(int id)
        {
            return await _db.CuentaContableProductosServicios.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

		public async Task<CuentaContableProductoServicio?> GetByNameAsync(string name)
		{
			return await _db.CuentaContableProductosServicios
				.Include(p => p.ProductoServicio)
				.Where(p => p.ProductoServicio != null && p.ProductoServicio.Descripcion == name).FirstOrDefaultAsync();
		}

	}
}
