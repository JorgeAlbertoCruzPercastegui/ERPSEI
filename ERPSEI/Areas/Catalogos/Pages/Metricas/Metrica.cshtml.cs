using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Metricas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ERPSEI.Areas.Catalogos.Pages.Metricas
{
    [Authorize(Roles = "Administrador,Master")]
    public class MetricaModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public MetricaModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public class UsuarioActivoDto
        {
            public string Usuario { get; set; } = string.Empty;

            public int Total { get; set; }
        }

        public class ModuloVisitadoDto
        {
            public string Modulo { get; set; } = string.Empty;

            public int Total { get; set; }
        }

        public int LoginsHoy { get; set; }
        public int UsuariosActivosHoy { get; set; }
        public int VisitasMes { get; set; }
        public string ModuloMasUsado { get; set; } = "Sin datos";

        public List<IntranetActividad> UltimosAccesos { get; set; } = new();

        public object MetricasJson { get; set; } = new();

        public List<UsuarioActivoDto> UsuariosMasActivos { get; set; } = new();

        public List<ModuloVisitadoDto> ModulosMasVisitados { get; set; } = new();

        public async Task OnGetAsync()
        {
            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var inicio30Dias = hoy.AddDays(-29);

            LoginsHoy = await _db.IntranetActividades
                .AsNoTracking()
                .CountAsync(x => x.TipoEvento == "Login" && x.FechaHora.Date == hoy);

            UsuariosActivosHoy = await _db.IntranetActividades
                .AsNoTracking()
                .Where(x => x.FechaHora.Date == hoy && !string.IsNullOrWhiteSpace(x.UserId))
                .Select(x => x.UserId)
                .Distinct()
                .CountAsync();

            VisitasMes = await _db.IntranetActividades
                .AsNoTracking()
                .CountAsync(x => x.FechaHora >= inicioMes);

            var moduloMasUsado = await _db.IntranetActividades
                .AsNoTracking()
                .Where(x => x.TipoEvento == "VistaModulo")
                .GroupBy(x => x.Modulo)
                .Select(g => new
                {
                    Modulo = g.Key,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .FirstOrDefaultAsync();

            ModuloMasUsado = moduloMasUsado?.Modulo ?? "Sin datos";

            UltimosAccesos = await _db.IntranetActividades
                .AsNoTracking()
                .OrderByDescending(x => x.FechaHora)
                .Take(20)
                .ToListAsync();

            UsuariosMasActivos = await _db.IntranetActividades
                .AsNoTracking()
                .Where(x =>
                    x.TipoEvento == "Login" &&
                    !string.IsNullOrWhiteSpace(x.UserName))
                .GroupBy(x => x.UserName)
                .Select(g => new UsuarioActivoDto
                {
                    Usuario = g.Key!,
                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .Take(10)
                .ToListAsync();

            ModulosMasVisitados = await _db.IntranetActividades
                .AsNoTracking()
                .Where(x => x.TipoEvento == "VistaModulo")
                .GroupBy(x => x.Modulo)
                .Select(g => new ModuloVisitadoDto
                {
                    Modulo = string.IsNullOrWhiteSpace(g.Key)
                        ? "General"
                        : g.Key,

                    Total = g.Count()
                })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToListAsync();
        }
    }
}