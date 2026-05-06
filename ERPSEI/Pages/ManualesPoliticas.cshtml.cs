using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Pages
{
    public class ManualesPoliticasModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public ManualesPoliticasModel(
            ApplicationDbContext db,
            UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public List<ManualPoliticaIntranet> Lista { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Tipo { get; set; }

        public int TotalManuales { get; set; }
        public int TotalPoliticas { get; set; }
        public int TotalReglamentos { get; set; }
        public int TotalTodos { get; set; }

        public async Task OnGetAsync()
        {
            AppUser? usuario = await _userManager.GetUserAsync(User);

            int? areaIdUsuario = null;

            if (usuario != null && usuario.EmpleadoId.HasValue)
            {
                areaIdUsuario = await _db.Empleados
                    .AsNoTracking()
                    .Where(e => e.Id == usuario.EmpleadoId.Value)
                    .Select(e => e.AreaId)
                    .FirstOrDefaultAsync();
            }

            var baseQuery = _db.ManualesPoliticasIntranet
                .AsNoTracking()
                .Include(x => x.AreasPermitidas)
                .Where(x =>
                    x.Activo &&
                    x.Publicado &&
                    (
                        x.PublicacionGeneral ||
                        (
                            areaIdUsuario != null &&
                            x.AreasPermitidas.Any(a => a.AreaId == areaIdUsuario.Value)
                        )
                    )
                );

            TotalTodos = await baseQuery.CountAsync();

            TotalManuales = await baseQuery.CountAsync(x =>
                x.Tipo != null && x.Tipo.ToLower() == "manual");

            TotalPoliticas = await baseQuery.CountAsync(x =>
                x.Tipo != null && x.Tipo.ToLower() == "politica");

            TotalReglamentos = await baseQuery.CountAsync(x =>
                x.Tipo != null && x.Tipo.ToLower() == "reglamento");

            IQueryable<ManualPoliticaIntranet> query = baseQuery;

            if (!string.IsNullOrWhiteSpace(Tipo))
            {
                string tipoFiltro = Tipo.Trim().ToLower();

                query = query.Where(x =>
                    x.Tipo != null &&
                    x.Tipo.ToLower() == tipoFiltro);
            }

            Lista = await query
                .OrderBy(x => x.Orden)
                .ThenBy(x => x.Titulo)
                .ToListAsync();
        }

        public string ObtenerUrlVisualizacion(ManualPoliticaIntranet item)
        {
            if (item.ModoVisualizacion == "Link" && !string.IsNullOrWhiteSpace(item.UrlExterna))
                return item.UrlExterna;

            if (item.ModoVisualizacion == "Pdf" && !string.IsNullOrWhiteSpace(item.RutaArchivoPdf))
                return item.RutaArchivoPdf;

            return $"/VisorManualPolitica?id={item.Id}";
        }
    }
}