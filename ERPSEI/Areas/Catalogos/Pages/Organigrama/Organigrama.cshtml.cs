using ERPSEI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.Catalogos.Pages.Organigrama
{
    [Authorize]
    public class OrganigramaModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public OrganigramaModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<EmpleadoOrganigramaDto> Organigrama { get; set; } = new();
        public List<EmpleadoOrganigramaDto> EmpleadosSinJefe { get; set; } = new();

        public async Task OnGetAsync()
        {
            var empleados = await _db.Empleados
                .AsNoTracking()
                .Include(e => e.Area)
                .Include(e => e.Puesto)
                .Where(e => e.Deshabilitado == 0)
                .Select(e => new EmpleadoOrganigramaDto
                {
                    Id = e.Id,
                    JefeId = e.JefeId,
                    NombreCompleto =
                        ((e.Nombre ?? "") + " " +
                         (e.ApellidoPaterno ?? "") + " " +
                         (e.ApellidoMaterno ?? "")).Trim(),
                    Puesto = e.Puesto != null ? e.Puesto.Nombre : "Sin puesto",
                    Area = e.Area != null ? e.Area.Nombre : "Sin área"
                })
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();

            var diccionario = empleados.ToDictionary(e => e.Id);

            foreach (var empleado in empleados)
            {
                if (empleado.JefeId.HasValue && diccionario.ContainsKey(empleado.JefeId.Value))
                {
                    diccionario[empleado.JefeId.Value].Subordinados.Add(empleado);
                }
            }

            // Raíces reales: empleados que son jefes de alguien.
            // No muestra empleados sin jefe y sin subordinados.
            Organigrama = empleados
                .Where(e =>
                    (!e.JefeId.HasValue || !diccionario.ContainsKey(e.JefeId.Value)) &&
                    e.Subordinados.Any()
                )
                .OrderBy(e => e.NombreCompleto)
                .ToList();

            // Empleados pendientes de asignar jefe directo.
            EmpleadosSinJefe = empleados
                .Where(e => !e.JefeId.HasValue && !e.Subordinados.Any())
                .OrderBy(e => e.NombreCompleto)
                .ToList();
        }

        public class EmpleadoOrganigramaDto
        {
            public int Id { get; set; }
            public int? JefeId { get; set; }

            public string NombreCompleto { get; set; } = string.Empty;
            public string Puesto { get; set; } = string.Empty;
            public string Area { get; set; } = string.Empty;

            public List<EmpleadoOrganigramaDto> Subordinados { get; set; } = new();
        }
    }
}