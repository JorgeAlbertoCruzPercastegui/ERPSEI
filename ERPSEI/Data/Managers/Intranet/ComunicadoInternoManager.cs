using ERPSEI.Data.Entities.Intranet;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Intranet
{
    public class ComunicadoInternoManager : IComunicadoInternoManager
    {
        private readonly ApplicationDbContext _context;

        public ComunicadoInternoManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ComunicadoInterno>> GetAllAsync(bool incluirInactivos = true)
        {
            IQueryable<ComunicadoInterno> query = _context.ComunicadosInternos;

            if (!incluirInactivos)
                query = query.Where(x => x.Activo);

            return await query
                .OrderByDescending(x => x.FechaPublicacion)
                .ThenByDescending(x => x.FechaCreacion)
                .ToListAsync();
        }

        public async Task<ComunicadoInterno?> GetByIdAsync(int id)
        {
            return await _context.ComunicadosInternos
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ComunicadoInterno> AddAsync(ComunicadoInterno entity)
        {
            _context.ComunicadosInternos.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ComunicadoInterno> UpdateAsync(ComunicadoInterno entity)
        {
            _context.ComunicadosInternos.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> SoftDeleteAsync(int id, string? userId = null)
        {
            ComunicadoInterno? entity = await _context.ComunicadosInternos.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            entity.Activo = false;
            entity.FechaModificacion = DateTime.Now;
            entity.ModificadoPorId = userId;

            _context.ComunicadosInternos.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleActivoAsync(int id, string? userId = null)
        {
            ComunicadoInterno? entity = await _context.ComunicadosInternos.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            entity.Activo = !entity.Activo;
            entity.FechaModificacion = DateTime.Now;
            entity.ModificadoPorId = userId;

            _context.ComunicadosInternos.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PublicarAsync(int id, string? userId = null)
        {
            var entity = await _context.ComunicadosInternos.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            entity.Publicado = true;
            entity.FechaModificacion = DateTime.Now;
            entity.ModificadoPorId = userId;

            _context.ComunicadosInternos.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string? userId = null)
        {
            var entity = await _context.ComunicadosInternos.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            entity.Activo = false;
            entity.Publicado = false;
            entity.FechaModificacion = DateTime.Now;
            entity.ModificadoPorId = userId;

            _context.ComunicadosInternos.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ComunicadoInterno>> GetPublicadosVisiblesAsync(int? mes = null)
        {
            DateTime fechaLimite = DateTime.Today.AddYears(-1);

            IQueryable<ComunicadoInterno> query = _context.ComunicadosInternos
                .Where(x => x.Activo && x.Publicado &&
                       (x.EsPermanente || x.FechaPublicacion >= fechaLimite));

            if (mes.HasValue && mes.Value >= 1 && mes.Value <= 12)
            {
                query = query.Where(x => x.EsPermanente || x.FechaPublicacion.Month == mes.Value);
            }

            return await query
                .OrderByDescending(x => x.EsPermanente)
                .ThenByDescending(x => x.FechaPublicacion)
                .ThenByDescending(x => x.HoraPublicacion)
                .ThenByDescending(x => x.FechaCreacion)
                .ToListAsync();
        }
    }
}