using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Pages
{
    public class ManualesPoliticasModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ManualesPoliticasModel(ApplicationDbContext db)
        {
            _db = db;
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
            var baseQuery = _db.ManualesPoliticasIntranet
                .AsNoTracking()
                .Where(x => x.Activo && x.Publicado);

            TotalTodos = await baseQuery.CountAsync();
            TotalManuales = await baseQuery.CountAsync(x => x.Tipo != null && x.Tipo.ToLower() == "manual");
            TotalPoliticas = await baseQuery.CountAsync(x => x.Tipo != null && x.Tipo.ToLower() == "politica");
            TotalReglamentos = await baseQuery.CountAsync(x => x.Tipo != null && x.Tipo.ToLower() == "reglamento");

            IQueryable<ManualPoliticaIntranet> query = baseQuery;

            if (!string.IsNullOrWhiteSpace(Tipo))
            {
                string tipoFiltro = Tipo.Trim().ToLower();

                query = query.Where(x => x.Tipo != null && x.Tipo.ToLower() == tipoFiltro);
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