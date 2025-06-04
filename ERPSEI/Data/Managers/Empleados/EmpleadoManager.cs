using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Reportes;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Data.Managers.Empleados
{
    public class EmpleadoManager(ApplicationDbContext _db) : IEmpleadoManager
    {
		private async Task<int> GetNextId()
        {
            List<Empleado> registros = await _db.Empleados.ToListAsync();
            Empleado? last = registros.OrderByDescending(r => r.Id).FirstOrDefault();
            int lastId = last != null ? last.Id : 0;
            lastId += 1;

            return lastId;
        }

        public async Task<int> CreateAsync(Empleado empleado)
        {
            empleado.Id = await GetNextId();
            _db.Empleados.Add(empleado);
            await _db.SaveChangesAsync();
            return empleado.Id;
        }
        public async Task UpdateAsync(Empleado empleado)
        {
            Empleado? a = _db.Find<Empleado>(empleado.Id);
            if (a != null)
            {
                //a.Nombre = empleado.Nombre;
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Empleado empleado)
        {
            _db.Empleados.Remove(empleado);
            await _db.SaveChangesAsync();
        }

        public async Task DisableByIdAsync(int id)
        {
            Empleado? empleado = await GetByIdAsync(id);
            AppUser? appUser = _db.Users.Where(u => u.EmpleadoId == id).FirstOrDefault();

            if (empleado != null)
            {
                //Deshabilita el empleado
                empleado.Deshabilitado = 1;
                if (appUser != null)
                {
                    appUser.IsBanned = true;
                    _db.Update(appUser);
                }

                _db.Update(empleado);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteByIdAsync(int id)
        {
            Empleado? empleado = await GetByIdAsync(id);
            if (empleado != null)
            {
                _db.Remove(empleado);
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteMultipleByIdAsync(string[] ids)
        {
            //Inicia una transacción.
            await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (string id in ids)
                {
                    Empleado? empleado = await GetByIdAsync(int.Parse(id));
                    if (empleado != null)
                    {
                        _db.Remove(empleado);
                        await _db.SaveChangesAsync();
                    }
                }

                await _db.Database.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _db.Database.RollbackTransactionAsync();
                throw;

            }
        }

        public async Task<List<Empleado>> GetAllAsync(
            DateTime? fechaIngresoInicio = null,
            DateTime? fechaIngresoFin = null,
            DateTime? fechaNacimientoInicio = null,
            DateTime? fechaNacimientoFin = null,
            int? puestoId = null,
            int? areaId = null,
            int? subareaId = null,
            int? oficinaId = null,
            bool deshabilitado = false
        )
        {

            return await _db.Empleados
                .Where(e => deshabilitado || e.Deshabilitado == 0)
                .Where(e => fechaIngresoInicio == null || e.FechaIngreso >= fechaIngresoInicio)
                .Where(e => fechaIngresoFin == null || e.FechaIngreso <= fechaIngresoFin)
                .Where(e => fechaNacimientoInicio == null || e.FechaNacimiento >= fechaNacimientoInicio)
                .Where(e => fechaNacimientoFin == null || e.FechaNacimiento <= fechaNacimientoFin)
                .Where(e => puestoId == null || e.PuestoId == puestoId)
                .Where(e => areaId == null || e.AreaId == areaId)
                .Where(e => subareaId == null || e.SubareaId == subareaId)
                .Where(e => oficinaId == null || e.OficinaId == oficinaId)
                .Include(e => e.Oficina)
                .Include(e => e.Puesto)
                .Include(e => e.Area)
                .Include(e => e.Subarea)
                .Include(e => e.Genero)
                .Include(e => e.EstadoCivil)
                .Include(e => e.Horario).ThenInclude(h => h.HorarioDetalles)
                .ToListAsync();
        }

        public async Task<Empleado?> GetEmpleadoLoginAsync(int id)
        {
            return await _db.Empleados
                .Where(e => e.Id == id)
                .Where(e => e.Deshabilitado == 0)
                .FirstOrDefaultAsync();
        }

        public async Task<Empleado?> GetEmpleadoOrganigramaAsync(int id)
        {
            return await _db.Empleados
                .Where(e => e.Deshabilitado == 0)
                .Where(e => e.Id == id)
                .Include(e => e.Oficina)
                .Include(e => e.Puesto)
                .Include(e => e.Area)
                .Include(e => e.Subarea)
                .Include(e => e.ArchivosEmpleado.Where(a => a.TipoArchivoId == (int)FileTypes.ImagenPerfil))
                .FirstOrDefaultAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosOrganigramaAsync(int? jefeId, int? puestoId, int? areaId, int? subareaId)
        {
            if (jefeId.HasValue)
            {
                return await _db.Empleados
                .Where(e => e.Deshabilitado == 0)
                .Where(e => !jefeId.HasValue || e.JefeId == jefeId)
                .Include(e => e.Oficina)
                .Include(e => e.Puesto)
                .Include(e => e.Area)
                .Include(e => e.Subarea)
                .Include(e => e.ArchivosEmpleado.Where(a => a.TipoArchivoId == (int)FileTypes.ImagenPerfil))
                .ToListAsync();
            }
            else
            {
                return await _db.Empleados
                .Where(e => e.Deshabilitado == 0)
                .Where(e => !puestoId.HasValue || e.PuestoId == puestoId)
                .Where(e => !areaId.HasValue || e.AreaId == areaId)
                .Where(e => !subareaId.HasValue || e.SubareaId == subareaId)
                .Include(e => e.Oficina)
                .Include(e => e.Puesto)
                .Include(e => e.Area)
                .Include(e => e.Subarea)
                .Include(e => e.ArchivosEmpleado.Where(a => a.TipoArchivoId == (int)FileTypes.ImagenPerfil))
                .ToListAsync();
            }

        }

        public async Task<Empleado?> GetByIdWithAdicionalesAsync(int id)
        {
            return await _db.Empleados
                .Where(e => e.Deshabilitado == 0)
                .Where(e => e.Id == id)
                .Include(e => e.ContactosEmergencia)
                .FirstOrDefaultAsync();
        }

        public async Task<Empleado?> GetByIdAsync(int id)
        {
            return await _db.Empleados.Where(a => a.Deshabilitado == 0 && a.Id == id).Include(e => e.Horario).ThenInclude(h => h.HorarioDetalles).FirstOrDefaultAsync();
        }

        public async Task<Empleado?> GetByCURPAsync(string curp)
        {
            return await _db.Empleados.Where(e => e.Deshabilitado == 0 && e.CURP == curp).FirstOrDefaultAsync();
        }

        /*public async Task<Empleado?> GetByNameAsync(string name)
        {
            return await _db.Empleados.Where(e => e.Deshabilitado == 0 && e.NombreCompleto.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
        }*/
		public async Task<Empleado?> GetByNameAsync(string name)
		{
			return await _db.Empleados.Where(a => a.NombreCompleto.ToLower() == name.ToLower()).FirstOrDefaultAsync();
		}

		public async Task<Empleado?> GetByEmailAsync(string email)
        {
            //return await _db.Empleados.Where(e => e.Deshabilitado == 0 && e.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
            /*return await _db.Empleados
            .Where(e => e.Deshabilitado == 0 && e.Email.ToLower() == email.ToLower())
            .FirstOrDefaultAsync();*/
            return await _db.Empleados
            .Where(e => e.Deshabilitado == 0 && e.Email != null && e.Email.ToLower() == email.ToLower())
            .FirstOrDefaultAsync();

        }

    }
}
