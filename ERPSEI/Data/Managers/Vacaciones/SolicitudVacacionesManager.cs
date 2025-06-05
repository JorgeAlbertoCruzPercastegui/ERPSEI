using ERPSEI.Data.Entities.Vacaciones;
using ERPSEI.Data.Managers.ActivosFijos;
using Microsoft.EntityFrameworkCore;
using static ERPSEI.Areas.ERP.Pages.VacacionesModel;

namespace ERPSEI.Data.Managers.Vacaciones
{
    public class SolicitudVacacionesManager(ApplicationDbContext db) : ISolicitudVacacionesManager
    {
        private async Task<int> GetNextId()
        {
            List<SolicitudVacaciones> solicitudVacaciones = await db.SolicitudesVacaciones.ToListAsync();
            SolicitudVacaciones? last = solicitudVacaciones.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }
        public async Task<int> CreateAsync(SolicitudVacaciones solicitudVacaciones)
        {
            //solicitudVacaciones.Id = await GetNextId();
            db.SolicitudesVacaciones.Add(solicitudVacaciones);
            await db.SaveChangesAsync();
            return solicitudVacaciones.Id;
        }

        public async Task UpdateAsync(SolicitudVacaciones solicitudVacaciones)
        {
            var a = await db.SolicitudesVacaciones.FindAsync(solicitudVacaciones.Id);
            if (a != null)
            {
                a.Empleado = solicitudVacaciones.Empleado;
                a.FechaSolicitud = solicitudVacaciones.FechaSolicitud;
                a.FechaInicio = solicitudVacaciones.FechaInicio;
                a.FechaFin = solicitudVacaciones.FechaFin;
                a.DiasSolicitados = solicitudVacaciones.DiasSolicitados;
                a.ComentarioEmpleado = solicitudVacaciones.ComentarioEmpleado;
                a.ComentarioAutorizador = solicitudVacaciones.ComentarioAutorizador;
                a.Estado = solicitudVacaciones.Estado;
                a.AutorizadorId = solicitudVacaciones.AutorizadorId;
                a.FechaRespuesta = solicitudVacaciones.FechaRespuesta;

                await db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(SolicitudVacaciones solicitudVacaciones)
        {
            db.SolicitudesVacaciones.Remove(solicitudVacaciones);
            await db.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            SolicitudVacaciones? solicitudVacaciones = await GetByIdAsync(id);
            if (solicitudVacaciones != null)
            {
                db.Remove(solicitudVacaciones);
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
                    SolicitudVacaciones? solicitudVacaciones = await GetByIdAsync(int.Parse(id));
                    if (solicitudVacaciones != null)
                    {
                        db.Remove(solicitudVacaciones);
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

        public async Task<List<SolicitudVacaciones>> GetAllAsync()
        {
            return await db.SolicitudesVacaciones
                .Include(a => a.Empleado)
                .Include(a => a.Autorizador)
                .ToListAsync();
        }

        public async Task<SolicitudVacaciones?> GetByIdAsync(int id)
        {
            return await db.SolicitudesVacaciones.Where(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<SolicitudVacaciones?> GetByNameAsync(string name)
        {
            return await db.SolicitudesVacaciones.Where(a => a.ComentarioAutorizador.ToLower() == name.ToLower() || a.ComentarioAutorizador.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<List<SolicitudVacaciones>> GetAllAsync(ERPSEI.Areas.ERP.Pages.VacacionesModel.InputFiltroVacacionesModel? filtro = null)
        {
            var query = db.SolicitudesVacaciones
                .Include(a => a.Empleado)
                .Include(a => a.Autorizador)
                .AsQueryable();

            if (filtro != null)
            {
                if (!string.IsNullOrWhiteSpace(filtro.Empleado))
                    query = query.Where(a => a.Empleado != null && a.Empleado.NombreCompleto.Contains(filtro.Empleado));

                if (!string.IsNullOrWhiteSpace(filtro.Autorizador))
                    query = query.Where(a => a.Autorizador != null && a.Autorizador.NombreCompleto.Contains(filtro.Autorizador));

                if (filtro.Estado.HasValue)
                    query = query.Where(a => a.Estado == filtro.Estado.Value);

                if (filtro.FechaInicioDesde.HasValue)
                    query = query.Where(a => a.FechaInicio >= filtro.FechaInicioDesde.Value);

                if (filtro.FechaFinHasta.HasValue)
                    query = query.Where(a => a.FechaFin <= filtro.FechaFinHasta.Value);
            }

            return await query.ToListAsync();
        }


    }
}
