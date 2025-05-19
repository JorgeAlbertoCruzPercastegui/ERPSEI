using ERPSEI.Data.Entities.ActivosFijos;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<ActivoFijo>> GetFilteredAsync(
        int? folio,
        string? responsable,
        int? categoriaId,
        int? tipoId,
        DateTime? fechaInicio,
        DateTime? fechaFin)
        {
            var query = db.ActivosFijos
                .Include(a => a.Empleado)
                .Include(a => a.Categoria)
                .Include(a => a.Tipo)
                .AsQueryable();

            if (folio.HasValue)
                query = query.Where(a => a.Folio == folio.ToString());

            if (!string.IsNullOrWhiteSpace(responsable))
                query = query.Where(a => a.Empleado != null && a.Empleado.NombreCompleto.Contains(responsable));

            if (categoriaId.HasValue)
                query = query.Where(a => a.CategoriaId == categoriaId);

            if (tipoId.HasValue)
                query = query.Where(a => a.TipoId == tipoId);

            if (fechaInicio.HasValue)
                query = query.Where(a => a.FechaCompra >= fechaInicio);

            if (fechaFin.HasValue)
                query = query.Where(a => a.FechaCompra <= fechaFin);

            return await query.ToListAsync();
        }

    }
}
