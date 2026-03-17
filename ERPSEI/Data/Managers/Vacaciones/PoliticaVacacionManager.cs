using ERPSEI.Data.Entities.Vacaciones;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Vacaciones
{
    public class PoliticaVacacionManager : IPoliticaVacacionManager
    {
        private readonly ApplicationDbContext db;

        public PoliticaVacacionManager(ApplicationDbContext _db)
        {
            db = _db;
        }

        public async Task<List<PoliticaVacacion>> GetActivasAsync()
        {
            return await db.PoliticasVacaciones
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        public async Task<PoliticaVacacion?> GetPorTipoAsync(string tipoVacacion)
        {
            return await db.PoliticasVacaciones
                .Include(p => p.Detalles.OrderBy(d => d.Orden))
                .FirstOrDefaultAsync(p => p.Activo && p.TipoVacacion == tipoVacacion);
        }

        public async Task<PoliticaVacacion?> GetByIdAsync(int id)
        {
            return await db.PoliticasVacaciones
                .Include(p => p.Detalles.OrderBy(d => d.Orden))
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PoliticaVacacion> CreateAsync(PoliticaVacacion politica)
        {
            db.PoliticasVacaciones.Add(politica);
            await db.SaveChangesAsync();
            return politica;
        }
    }
}