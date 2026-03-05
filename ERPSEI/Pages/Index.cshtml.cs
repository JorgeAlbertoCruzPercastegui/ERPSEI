using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Banner> BannersHome { get; set; } = new();

        public async Task OnGetAsync()
        {
            var now = DateTime.Now;

            BannersHome = await _db.Banners
                .AsNoTracking()
                .Where(b => b.Activo)
                .Where(b =>
                    b.EsPermanente ||
                    (
                        (!b.VigenciaInicio.HasValue || b.VigenciaInicio <= now) &&
                        (!b.VigenciaFin.HasValue || b.VigenciaFin >= now)
                    )
                )
                .OrderBy(b => b.Orden)
                .ThenByDescending(b => b.FechaCreacion)
                .ToListAsync();
        }
    }
}