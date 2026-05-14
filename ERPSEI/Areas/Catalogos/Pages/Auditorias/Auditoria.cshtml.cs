using ERPSEI.Data;
using ERPSEI.Data.Entities.Metricas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.Catalogos.Pages.Auditoria
{
    [Authorize(Roles = "Administrador,Master")]
    public class AuditoriaModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public AuditoriaModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public int TotalAltas { get; set; }
        public int TotalEdiciones { get; set; }
        public int TotalBajas { get; set; }
        public int CambiosHoy { get; set; }

        public List<IntranetAuditoria> RegistrosAuditoria { get; set; } = new();

        public async Task OnGetAsync()
        {
            var hoy = DateTime.Today;

            TotalAltas = await _db.IntranetAuditorias
                .AsNoTracking()
                .CountAsync(x => x.Accion == "Alta");

            TotalEdiciones = await _db.IntranetAuditorias
                .AsNoTracking()
                .CountAsync(x => x.Accion == "Edición");

            TotalBajas = await _db.IntranetAuditorias
                .AsNoTracking()
                .CountAsync(x => x.Accion == "Baja" || x.Accion == "Eliminación");

            CambiosHoy = await _db.IntranetAuditorias
                .AsNoTracking()
                .CountAsync(x => x.FechaHora.Date == hoy);

            RegistrosAuditoria = await _db.IntranetAuditorias
                .AsNoTracking()
                .OrderByDescending(x => x.FechaHora)
                .Take(500)
                .ToListAsync();
        }
    }
}