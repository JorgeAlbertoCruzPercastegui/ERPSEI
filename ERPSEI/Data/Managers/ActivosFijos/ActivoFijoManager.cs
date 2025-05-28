using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Entities.Clientes;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ERPSEI.Data.Managers.ActivosFijos
{
    public class ActivoFijoManager(ApplicationDbContext db) : IActivoFijoManager
    {
        private async Task<int> GetNextId()
        {
            List<ActivoFijo> activoFijo = await db.ActivosFijos.ToListAsync();
            ActivoFijo? last = activoFijo.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(ActivoFijo activoFijo)
        {
            activoFijo.Id = await GetNextId();
            db.ActivosFijos.Add(activoFijo);
            await db.SaveChangesAsync();
            return activoFijo.Id;
        }
        public async Task UpdateAsync(ActivoFijo activoFijo)
        {
            var a = await db.ActivosFijos.FindAsync(activoFijo.Id);
            if (a != null)
            {
                a.Descripcion = activoFijo.Descripcion;
                a.Folio = activoFijo.Folio;
                a.CategoriaId = activoFijo.CategoriaId;
                a.TipoId = activoFijo.TipoId;
                a.EmpleadoId = activoFijo.EmpleadoId;
                a.Marca = activoFijo.Marca;
                a.NumeroSerie = activoFijo.NumeroSerie;
                a.Ubicacion = activoFijo.Ubicacion;
                a.FechaCompra = activoFijo.FechaCompra;
                a.Precio = activoFijo.Precio;
                a.Comentarios = activoFijo.Comentarios;
                a.FechaRenovacion = activoFijo.FechaRenovacion;
                a.LinkFacturaCompra = activoFijo.LinkFacturaCompra;
                a.Deshabilitado = activoFijo.Deshabilitado;

                await db.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(ActivoFijo activoFijo)
        {
            db.ActivosFijos.Remove(activoFijo);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            ActivoFijo? activoFijo = await GetByIdAsync(id);
            if (activoFijo != null)
            {
                db.Remove(activoFijo);
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
                    ActivoFijo? activoFijo = await GetByIdAsync(int.Parse(id));
                    if (activoFijo != null)
                    {
                        db.Remove(activoFijo);
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

        /*public async Task<List<ActivoFijo>> GetAllAsync()
        {
            return await db.ActivosFijos.ToListAsync();
        }*/

        public async Task<List<ActivoFijo>> GetAllAsync()
        {
            return await db.ActivosFijos
                .Include(a => a.Empleado)
                .Include(a => a.Categoria)
                .Include(a => a.Tipo)
                .ToListAsync();
        }



        public async Task<ActivoFijo?> GetByIdAsync(int id)
        {
            return await db.ActivosFijos.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<ActivoFijo?> GetByNameAsync(string name)
        {
            return await db.ActivosFijos.Where(a => a.Descripcion.ToLower() == name.ToLower() || a.Descripcion.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<List<ActivoFijo>> GetAllAsync(ERPSEI.Areas.ERP.Pages.ActivosFijosModel.InputFiltroModel? filtro = null)
        {
            var query = db.ActivosFijos
                .Include(a => a.Empleado)
                .Include(a => a.Categoria)
                .Include(a => a.Tipo).AsQueryable();

            if (filtro != null)
            {
                if (!string.IsNullOrWhiteSpace(filtro.Folio))
                    query = query.Where(a => a.Folio.Contains(filtro.Folio));

                if (!string.IsNullOrWhiteSpace(filtro.Responsable))
                    query = query.Where(a => a.Empleado != null && a.Empleado.NombreCompleto.Contains(filtro.Responsable));

                if (filtro.CategoriaId.HasValue && filtro.CategoriaId != 0)
                    query = query.Where(a => a.CategoriaId == filtro.CategoriaId);

                if (filtro.TipoId.HasValue && filtro.TipoId != 0)
                    query = query.Where(a => a.TipoId == filtro.TipoId);

                if (filtro.FechaCompraInicio.HasValue)
                    query = query.Where(a => a.FechaCompra >= filtro.FechaCompraInicio.Value);

                if (filtro.FechaCompraFin.HasValue)
                    query = query.Where(a => a.FechaCompra <= filtro.FechaCompraFin.Value);

                if (!string.IsNullOrWhiteSpace(filtro.Estatus))
                {
                    bool activo = filtro.Estatus.ToLower().Trim() == "activo";
                    query = query.Where(a => a.Deshabilitado == !activo); // ⬅️ esta lógica sí se queda
                }
            }


            return await query.ToListAsync();
        }
    }
}