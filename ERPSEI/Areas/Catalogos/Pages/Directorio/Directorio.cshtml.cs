using ERPSEI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.Catalogos.Pages.Directorio
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<DirectorioEmpleadoDto> EmpleadosDirectorio { get; set; } = new();

        public async Task OnGetAsync()
        {
            EmpleadosDirectorio = await _db.Empleados
                .AsNoTracking()
                .Include(e => e.Puesto)
                .Include(e => e.Area)
                .Include(e => e.Subarea)
                .Include(e => e.Usuario)
                .Where(e => e.Deshabilitado == 0)
                .OrderBy(e => e.Nombre)
                .ThenBy(e => e.ApellidoPaterno)
                .Select(e => new DirectorioEmpleadoDto
                {
                    NombreEmpleado =
                        ((e.Nombre ?? "") + " " +
                         (e.ApellidoPaterno ?? "") + " " +
                         (e.ApellidoMaterno ?? "")).Trim(),

                    Cargo = e.Puesto != null ? e.Puesto.Nombre : "Sin asignar",
                    Area = e.Area != null ? e.Area.Nombre : "Sin asignar",
                    SubArea = e.Subarea != null ? e.Subarea.Nombre : "Sin asignar",
                    Correo = e.Usuario != null && e.Usuario.Email != null ? e.Usuario.Email : "Sin asignar"
                })
                .ToListAsync();
        }

        public class DirectorioEmpleadoDto
        {
            public string NombreEmpleado { get; set; } = string.Empty;
            public string Cargo { get; set; } = string.Empty;
            public string Area { get; set; } = string.Empty;
            public string SubArea { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
        }
    }
}