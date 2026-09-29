using ERPSEI.Data;
using ERPSEI.Data.Entities.ActivosFijos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.ERP.Pages
{
    [Authorize]
    public class ActivoFijoConsultaModel : PageModel
    {
        private readonly ApplicationDbContext db;

        public ActivoFijoConsultaModel(
            ApplicationDbContext db)
        {
            this.db = db;
        }

        public ActivoFijo? Activo { get; set; }

        public async Task<IActionResult> OnGetAsync(
            string? folio)
        {
            if (string.IsNullOrWhiteSpace(folio))
            {
                return Page();
            }

            folio = folio.Trim();

            Activo =
                await db.ActivosFijos
                    .AsNoTracking()
                    .Include(x => x.Oficina)
                    .Include(x => x.Categoria)
                    .Include(x => x.Tipo)
                    .Include(x => x.Empleado)
                    .FirstOrDefaultAsync(x =>
                        x.Folio == folio &&
                        !x.Deshabilitado
                    );

            return Page();
        }
    }
}