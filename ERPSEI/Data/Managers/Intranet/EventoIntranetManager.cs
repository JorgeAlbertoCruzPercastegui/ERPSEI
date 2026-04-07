using ERPSEI.Data.Entities.Intranet;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Intranet
{
    public class EventoIntranetManager : IEventoIntranetManager
    {
        private readonly ApplicationDbContext _context;

        public EventoIntranetManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventoIntranet>> GetAllAsync(bool incluirInactivos = true)
        {
            IQueryable<EventoIntranet> query = _context.EventosIntranet;

            if (!incluirInactivos)
                query = query.Where(x => x.Activo);

            return await query
                .OrderByDescending(x => x.FechaEvento)
                .ThenByDescending(x => x.FechaCreacion)
                .ToListAsync();
        }

        public async Task<EventoIntranet?> GetByIdAsync(int id)
        {
            return await _context.EventosIntranet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<EventoIntranet> AddAsync(EventoIntranet entity)
        {
            _context.EventosIntranet.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<EventoIntranet> UpdateAsync(EventoIntranet entity)
        {
            _context.EventosIntranet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> ToggleActivoAsync(int id, string? userId = null)
        {
            var entity = await _context.EventosIntranet.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            entity.Activo = !entity.Activo;
            entity.FechaModificacion = DateTime.Now;
            entity.ModificadoPorId = userId;

            _context.EventosIntranet.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PublicarAsync(int id, string? userId = null)
        {
            var entity = await _context.EventosIntranet.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            entity.Publicado = true;
            entity.FechaModificacion = DateTime.Now;
            entity.ModificadoPorId = userId;

            _context.EventosIntranet.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string? userId = null)
        {
            var entity = await _context.EventosIntranet.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return false;

            entity.Activo = false;
            entity.Publicado = false;
            entity.FechaModificacion = DateTime.Now;
            entity.ModificadoPorId = userId;

            _context.EventosIntranet.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<EventoIntranet>> GetPublicadosAsync(string? region = null)
        {
            IQueryable<EventoIntranet> query = _context.EventosIntranet
                .Where(x => x.Activo && x.Publicado);

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(x =>
                    !x.RequiereGeolocalizacion ||
                    x.Region == null ||
                    x.Region == region);
            }

            return await query
                .OrderByDescending(x => x.FechaEvento)
                .ToListAsync();
        }
    }
}