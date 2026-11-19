using ERPSEI.Data;
using ERPSEI.Services.BioTimeIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

[Authorize(Roles = "Master,Administrador,Administrador TH")]
public sealed class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly BioTimeHistoricoQueue _historico;
    private readonly ILogger<IndexModel> _logger;
    public IndexModel(ApplicationDbContext db, BioTimeHistoricoQueue historico, ILogger<IndexModel> logger)
    {
        _db = db;
        _historico = historico;
        _logger = logger;
    }

    // POST: registra un trabajo corto y devuelve el ID sin esperar la importación completa.
    public IActionResult OnPostSincronizarPeriodo(DateTime? fechaInicio, DateTime? fechaFin)
    {
        if (!fechaInicio.HasValue || !fechaFin.HasValue)
            return BadRequest(new { success = false, message = "Selecciona ambas fechas." });
        var desde = fechaInicio.Value.Date;
        var hasta = fechaFin.Value.Date;
        if (hasta < desde || (hasta - desde).Days + 1 > 365)
            return BadRequest(new { success = false, message = "Selecciona de 1 a 365 días." });
        var hoy = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City")).Date;
        if (hasta > hoy) return BadRequest(new { success = false, message = "No se permiten fechas futuras." });
        var respuesta = _historico.Enqueue(desde, hasta);
        if (!respuesta.Accepted)
            return StatusCode(429, new { success = false, message = respuesta.Error });
        return new JsonResult(new { success = true, trabajoId = respuesta.Id });
    }

    // Estado de un trabajo: acceso bajo los mismos roles autorizados que la página.
    public IActionResult OnGetEstadoSincronizacion(Guid trabajoId)
    {
        var estado = _historico.GetStatus(trabajoId);
        if (estado is null) return NotFound(new { success = false, message = "Trabajo no encontrado; pudo reiniciarse el servidor." });
        return new JsonResult(new { success = true, trabajo = estado });
    }

    public void OnGet() { }

    public async Task<IActionResult> OnGetCatalogosAsync(CancellationToken ct)
    {
        var empleados = await _db.BioTimeEmpleados.AsNoTracking().OrderBy(e => e.Nombre).ThenBy(e => e.Apellidos)
            .Select(e => new { codigo = e.EmpCode, nombre = ((e.Nombre ?? "") + " " + (e.Apellidos ?? "")).Trim(), departamento = e.Departamento }).ToListAsync(ct);
        var areas = empleados.Select(x => x.departamento).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x).ToList();
        return new JsonResult(new { success = true, empleados, areas });
    }

    public async Task<IActionResult> OnGetDashboardAsync(DateTime? fechaInicio, DateTime? fechaFin, string? empleado, string? departamento, CancellationToken ct)
    {
        if (!fechaInicio.HasValue || !fechaFin.HasValue)
            return BadRequest(new { success = false, message = "Selecciona fecha inicial y fecha final." });
        var desde = fechaInicio.Value.Date;
        var hasta = fechaFin.Value.Date;
        if (hasta < desde || (hasta - desde).TotalDays >= 365) return BadRequest(new { success = false, message = "Selecciona un periodo de máximo 365 días." });
        var q = from c in _db.BioTimeChecadas.AsNoTracking()
                join e in _db.BioTimeEmpleados.AsNoTracking() on c.EmpCode equals e.EmpCode into g
                from e in g.DefaultIfEmpty()
                where c.FechaHora >= desde && c.FechaHora < hasta.AddDays(1)
                select new { c.Id, c.EmpCode, c.FechaHora, c.PunchState, c.VerifyType, c.TerminalSn, Nombre = e == null ? "" : (e.Nombre ?? "") + " " + (e.Apellidos ?? ""), Departamento = e == null ? "" : e.Departamento };
        if (!string.IsNullOrWhiteSpace(empleado)) q = q.Where(x => x.EmpCode == empleado);
        if (!string.IsNullOrWhiteSpace(departamento)) q = q.Where(x => x.Departamento == departamento);
        var total = await q.CountAsync(ct);
        var personas = await q.Select(x => x.EmpCode).Distinct().CountAsync(ct);
        var porDia = await q.GroupBy(x => x.FechaHora.Date).Select(x => new { fecha = x.Key, total = x.Count() }).OrderBy(x => x.fecha).ToListAsync(ct);
        var ultima = await _db.BioTimeSincronizaciones.AsNoTracking().OrderByDescending(x => x.InicioUtc)
            .Select(x => new { x.Estado, x.InicioUtc, x.FinUtc, x.ChecadasInsertadas }).FirstOrDefaultAsync(ct);
        return new JsonResult(new { success = true, total, personas, porDia, ultima });
    }

    // Detalle paginado y búsqueda sobre TODOS los registros del periodo (SQL Server).
    public async Task<IActionResult> OnGetChecadasAsync(DateTime? fechaInicio, DateTime? fechaFin, string? empleado, string? departamento, string? buscar, int pagina = 1, int tamano = 10, CancellationToken ct = default)
    {
        if (!fechaInicio.HasValue || !fechaFin.HasValue)
            return BadRequest(new { success = false, message = "Selecciona fecha inicial y fecha final." });
        var desde = fechaInicio.Value.Date;
        var hasta = fechaFin.Value.Date;
        if (hasta < desde || (hasta - desde).TotalDays >= 365)
            return BadRequest(new { success = false, message = "Selecciona un periodo de máximo 365 días." });
        if (pagina < 1 || pagina > 100000 || !new[] { 10, 15, 20, 25, 50 }.Contains(tamano))
            return BadRequest(new { success = false, message = "Parámetros de paginación inválidos." });

        var q = from c in _db.BioTimeChecadas.AsNoTracking()
                join e in _db.BioTimeEmpleados.AsNoTracking() on c.EmpCode equals e.EmpCode into g
                from e in g.DefaultIfEmpty()
                where c.FechaHora >= desde && c.FechaHora < hasta.AddDays(1)
                select new
                {
                    c.Id,
                    c.EmpCode,
                    c.FechaHora,
                    c.PunchState,
                    c.VerifyType,
                    c.TerminalSn,
                    Nombre = e == null ? "" : (e.Nombre ?? "") + " " + (e.Apellidos ?? ""),
                    Departamento = e == null ? "" : (e.Departamento ?? "")
                };
        if (!string.IsNullOrWhiteSpace(empleado)) q = q.Where(x => x.EmpCode == empleado);
        if (!string.IsNullOrWhiteSpace(departamento)) q = q.Where(x => x.Departamento == departamento);
        buscar = buscar?.Trim();
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            if (buscar.Length > 100) return BadRequest(new { success = false, message = "La búsqueda es demasiado larga." });
            q = q.Where(x => x.EmpCode.Contains(buscar) || x.Nombre.Contains(buscar) ||
                x.Departamento.Contains(buscar) || (x.TerminalSn != null && x.TerminalSn.Contains(buscar)));
        }
        var total = await q.CountAsync(ct);
        var paginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamano));
        if (pagina > paginas) pagina = paginas;
        var registros = await q.OrderByDescending(x => x.FechaHora).ThenByDescending(x => x.Id)
            .Skip((pagina - 1) * tamano).Take(tamano)
            .Select(x => new {
                x.Id,
                codigo = x.EmpCode,
                nombre = x.Nombre,
                departamento = x.Departamento,
                fechaHora = x.FechaHora,
                estado = x.PunchState,
                verificacion = x.VerifyType,
                dispositivo = x.TerminalSn
            })
            .ToListAsync(ct);
        return new JsonResult(new { success = true, total, pagina, tamano, paginas, registros });
    }

}
