using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Pages
{
    public class VisorManualPoliticaModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public VisorManualPoliticaModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public ManualPoliticaIntranet? Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _db.ManualesPoliticasIntranet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.Activo && x.Publicado);

            if (Item == null)
                return NotFound();

            return Page();
        }
    }
}