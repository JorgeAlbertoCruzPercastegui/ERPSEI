using ERPSEI.Data;
using ERPSEI.Services.BioTimeIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

[Authorize(Roles = "Master,Administrador,Administrador TH")]
public sealed class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly BioTimeHistoricoQueue _historico;
    private readonly ILogger<IndexModel> _logger;
    private readonly BioTimeExcelService _excel;
    public IndexModel(ApplicationDbContext db, BioTimeHistoricoQueue historico, ILogger<IndexModel> logger, BioTimeExcelService excel)
    {
        _db = db;
        _historico = historico;
        _logger = logger;
        _excel = excel;
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
        if (!PeriodoValido(fechaInicio, fechaFin))
            return BadRequest(new { success = false, message = "Selecciona ambas fechas (máximo 365 días)." });
        var desde = fechaInicio!.Value.Date;
        var hasta = fechaFin!.Value.Date;
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
        if (!PeriodoValido(fechaInicio, fechaFin))
            return BadRequest(new { success = false, message = "Selecciona ambas fechas (máximo 365 días)." });
        var desde = fechaInicio!.Value.Date;
        var hasta = fechaFin!.Value.Date;
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


    private static bool PeriodoValido(DateTime? inicio, DateTime? fin) =>
        inicio.HasValue && fin.HasValue && fin.Value.Date >= inicio.Value.Date &&
        (fin.Value.Date - inicio.Value.Date).Days < 365 &&
        fin.Value.Date <= TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City")).Date;

    private IQueryable<ChecadaAnalitica> Consulta(DateTime desde, DateTime hasta, string? empleado, string? departamento)
    {
        var q = from c in _db.BioTimeChecadas.AsNoTracking()
                join e in _db.BioTimeEmpleados.AsNoTracking() on c.EmpCode equals e.EmpCode into g
                from e in g.DefaultIfEmpty()
                where c.FechaHora >= desde && c.FechaHora < hasta.AddDays(1)
                select new ChecadaAnalitica
                {
                    Id = c.Id,
                    Codigo = c.EmpCode,
                    FechaHora = c.FechaHora,
                    Nombre = e == null ? "" : (e.Nombre ?? "") + " " + (e.Apellidos ?? ""),
                    Departamento = e == null ? "" : (e.Departamento ?? ""),
                    Estado = c.PunchState,
                    Dispositivo = c.TerminalSn
                };
        if (!string.IsNullOrWhiteSpace(empleado)) q = q.Where(x => x.Codigo == empleado);
        if (!string.IsNullOrWhiteSpace(departamento)) q = q.Where(x => x.Departamento == departamento);
        return q;
    }

    // Expresión SQL compatible con EF Core: nunca carga un año completo en memoria.
    public async Task<IActionResult> OnGetResumenDiarioAsync(DateTime? fechaInicio, DateTime? fechaFin,
        string? empleado, string? departamento, string? buscar, int pagina = 1, int tamano = 10,
        CancellationToken ct = default)
    {
        if (!PeriodoValido(fechaInicio, fechaFin))
            return BadRequest(new { success = false, message = "Selecciona ambas fechas (máximo 365 días)." });
        if (pagina < 1 || pagina > 100000 || !new[] { 10, 15, 20, 25, 50 }.Contains(tamano))
            return BadRequest(new { success = false, message = "Paginación inválida." });
        buscar = buscar?.Trim();
        if (buscar?.Length > 100) return BadRequest(new { success = false, message = "Búsqueda demasiado larga." });
        var q = Consulta(fechaInicio!.Value.Date, fechaFin!.Value.Date, empleado, departamento);
        if (!string.IsNullOrWhiteSpace(buscar))
            q = q.Where(x => x.Codigo.Contains(buscar) || x.Nombre.Contains(buscar) || x.Departamento.Contains(buscar));
        var agrupado = q.GroupBy(x => new { Fecha = x.FechaHora.Date, x.Codigo, x.Nombre, x.Departamento })
            .Select(g => new {
                fecha = g.Key.Fecha,
                codigo = g.Key.Codigo,
                nombre = g.Key.Nombre,
                departamento = g.Key.Departamento,
                totalChecadas = g.Count(),
                primeraChecada = g.Min(x => x.FechaHora),
                ultimaChecada = g.Max(x => x.FechaHora)
            });
        var total = await agrupado.CountAsync(ct);
        var paginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamano));
        pagina = Math.Min(pagina, paginas);
        var raw = await agrupado.OrderByDescending(x => x.fecha).ThenBy(x => x.nombre).ThenBy(x => x.codigo)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync(ct);
        var registros = raw.Select(x => new {
            x.fecha,
            x.codigo,
            x.nombre,
            x.departamento,
            x.totalChecadas,
            x.primeraChecada,
            x.ultimaChecada,
            minutosEntreExtremos = (int)(x.ultimaChecada - x.primeraChecada).TotalMinutes
        });
        return new JsonResult(new { success = true, total, pagina, tamano, paginas, registros });
    }

    public async Task<IActionResult> OnGetAnaliticaDepartamentosAsync(DateTime? fechaInicio, DateTime? fechaFin,
        string? empleado, string? departamento, CancellationToken ct)
    {
        if (!PeriodoValido(fechaInicio, fechaFin))
            return BadRequest(new { success = false, message = "Selecciona ambas fechas (máximo 365 días)." });
        var q = Consulta(fechaInicio!.Value.Date, fechaFin!.Value.Date, empleado, departamento);
        var filas = await q.GroupBy(x => new { x.Departamento, x.Codigo, Fecha = x.FechaHora.Date })
            .Select(g => new { g.Key.Departamento, g.Key.Codigo, g.Key.Fecha, Checadas = g.Count() })
            .ToListAsync(ct);
        var departamentos = filas.GroupBy(x => x.Departamento)
            .Select(g => new {
                departamento = string.IsNullOrWhiteSpace(g.Key) ? "Sin departamento" : g.Key,
                colaboradores = g.Select(x => x.Codigo).Distinct().Count(),
                diasConActividad = g.Count(),
                checadas = g.Sum(x => x.Checadas),
                promedioChecadasPorDiaEmpleado = Math.Round(g.Average(x => (double)x.Checadas), 2)
            }).OrderByDescending(x => x.checadas).ThenBy(x => x.departamento).ToList();
        return new JsonResult(new
        {
            success = true,
            departamentos,
            totalDiasEmpleado = filas.Count,
            promedioChecadasPorDiaEmpleado = filas.Count == 0 ? 0 : Math.Round(filas.Average(x => (double)x.Checadas), 2)
        });
    }

    public async Task<IActionResult> OnGetAnaliticaColaboradoresAsync(DateTime? fechaInicio, DateTime? fechaFin,
        string? empleado, string? departamento, string? buscar, int pagina = 1, int tamano = 10,
        CancellationToken ct = default)
    {
        if (!PeriodoValido(fechaInicio, fechaFin))
            return BadRequest(new { success = false, message = "Selecciona ambas fechas." });
        if (pagina < 1 || pagina > 100000 || !new[] { 10, 15, 20, 25, 50 }.Contains(tamano))
            return BadRequest(new { success = false, message = "Paginación inválida." });
        buscar = buscar?.Trim();
        if (buscar?.Length > 100) return BadRequest(new { success = false, message = "Búsqueda demasiado larga." });
        var q = Consulta(fechaInicio!.Value.Date, fechaFin!.Value.Date, empleado, departamento);
        if (!string.IsNullOrWhiteSpace(buscar))
            q = q.Where(x => x.Codigo.Contains(buscar) || x.Nombre.Contains(buscar) || x.Departamento.Contains(buscar));
        var grupo = q.GroupBy(x => new { x.Codigo, x.Nombre, x.Departamento })
            .Select(g => new {
                codigo = g.Key.Codigo,
                nombre = g.Key.Nombre,
                departamento = g.Key.Departamento,
                checadas = g.Count(),
                diasConActividad = g.Select(x => x.FechaHora.Date).Distinct().Count(),
                primeraChecada = g.Min(x => x.FechaHora),
                ultimaChecada = g.Max(x => x.FechaHora)
            });
        var total = await grupo.CountAsync(ct);
        var paginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamano));
        pagina = Math.Min(pagina, paginas);
        var registros = await grupo.OrderByDescending(x => x.checadas).ThenBy(x => x.codigo)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync(ct);
        return new JsonResult(new { success = true, total, pagina, tamano, paginas, registros });
    }

    // Excel de consulta, acotado para proteger memoria y recursos del servidor.
    public async Task<IActionResult> OnGetExportarExcelAsync(DateTime? fechaInicio, DateTime? fechaFin,
        string? empleado, string? departamento, string tipo = "checadas", CancellationToken ct = default)
    {
        if (!PeriodoValido(fechaInicio, fechaFin))
            return BadRequest("Selecciona ambas fechas (máximo 365 días).");
        if (tipo != "checadas" && tipo != "resumen") return BadRequest("Tipo de reporte inválido.");
        try
        {
            var archivo = await _excel.GenerarReporteAsync(fechaInicio!.Value.Date, fechaFin!.Value.Date,
                empleado, departamento, tipo, ct);
            return File(archivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"INTRANET_BioTime_{tipo}_{fechaInicio:yyyyMMdd}_{fechaFin:yyyyMMdd}.xlsx");
        }
        catch (InvalidOperationException)
        {
            return StatusCode(413, "El reporte supera 50000 filas. Selecciona un periodo o departamento más pequeño.");
        }
    }

    // =====================================================
    // PESTAÑAS: VALIDACIONES Y DATOS DIARIOS
    // =====================================================

    private static bool PaginacionPestanasValida(int pagina, int tamano) =>
        pagina >= 1 && pagina <= 100000 && new[] { 10, 15, 20, 25, 50 }.Contains(tamano);

    private IQueryable<ResumenDiaBioTime> ConsultaDiaria(DateTime inicio, DateTime fin,
        string? empleado, string? departamento, string? buscar)
    {
        var q = Consulta(inicio, fin, empleado, departamento);
        buscar = buscar?.Trim();
        if (!string.IsNullOrWhiteSpace(buscar))
            q = q.Where(x => x.Codigo.Contains(buscar) || x.Nombre.Contains(buscar) || x.Departamento.Contains(buscar));
        return q.GroupBy(x => new { Fecha = x.FechaHora.Date, x.Codigo, x.Nombre, x.Departamento })
            .Select(g => new ResumenDiaBioTime
            {
                Fecha = g.Key.Fecha,
                Codigo = g.Key.Codigo,
                Nombre = g.Key.Nombre,
                Departamento = g.Key.Departamento,
                TotalChecadas = g.Count(),
                PrimeraChecada = g.Min(x => x.FechaHora),
                UltimaChecada = g.Max(x => x.FechaHora)
            });
    }

    // La primera checada no constituye prueba de entrada laboral. La referencia
    // es exclusivamente exploratoria, sin calificación disciplinaria ni nómina.
    public async Task<IActionResult> OnGetPuntualidadAsync(DateTime? fechaInicio, DateTime? fechaFin,
        string? empleado, string? departamento, TimeSpan? horaReferencia, int toleranciaMinutos = 0,
        int pagina = 1, int tamano = 10, CancellationToken ct = default)
    {
        if (!PeriodoValido(fechaInicio, fechaFin))
            return BadRequest(new { success = false, message = "Selecciona un periodo válido." });
        if (!PaginacionPestanasValida(pagina, tamano) || toleranciaMinutos < 0 || toleranciaMinutos > 120 ||
            (horaReferencia.HasValue && (horaReferencia.Value < TimeSpan.Zero ||
                horaReferencia.Value.Add(TimeSpan.FromMinutes(toleranciaMinutos)) >= TimeSpan.FromDays(1))))
            return BadRequest(new { success = false, message = "Parámetros de referencia o paginación inválidos." });
        if (!horaReferencia.HasValue)
            return new JsonResult(new
            {
                success = true,
                requiereReferencia = true,
                total = 0,
                pagina = 1,
                paginas = 1,
                tamano,
                registros = Array.Empty<object>()
            });
        var q = ConsultaDiaria(fechaInicio!.Value.Date, fechaFin!.Value.Date, empleado, departamento, null);
        var total = await q.CountAsync(ct);
        var paginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamano));
        pagina = Math.Min(pagina, paginas);
        var filas = await q.OrderByDescending(x => x.Fecha).ThenBy(x => x.Codigo)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync(ct);
        var referencia = horaReferencia.Value.Add(TimeSpan.FromMinutes(toleranciaMinutos));
        var registros = filas.Select(x => new {
            fecha = x.Fecha,
            codigo = x.Codigo,
            nombre = x.Nombre,
            departamento = x.Departamento,
            primeraChecada = x.PrimeraChecada,
            horaLimiteExploratoria = referencia.ToString(@"hh\:mm"),
            observacion = x.PrimeraChecada.TimeOfDay > referencia
                ? "Primer registro después de referencia" : "Primer registro hasta la referencia"
        }).ToList();
        return new JsonResult(new
        {
            success = true,
            requiereReferencia = false,
            total,
            pagina,
            paginas,
            tamano,
            registros
        });
    }

    // Jornadas calendario: extremos y conteo, SIN horas efectivamente trabajadas.
    public async Task<IActionResult> OnGetJornadasAsync(DateTime? fechaInicio, DateTime? fechaFin,
        string? empleado, string? departamento, string? buscar, int pagina = 1, int tamano = 10,
        CancellationToken ct = default)
    {
        if (!PeriodoValido(fechaInicio, fechaFin) || !PaginacionPestanasValida(pagina, tamano) || buscar?.Length > 100)
            return BadRequest(new { success = false, message = "Filtros o paginación inválidos." });
        var q = ConsultaDiaria(fechaInicio!.Value.Date, fechaFin!.Value.Date, empleado, departamento, buscar);
        var total = await q.CountAsync(ct);
        var paginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamano));
        pagina = Math.Min(pagina, paginas);
        var filas = await q.OrderByDescending(x => x.Fecha).ThenBy(x => x.Codigo)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync(ct);
        var registros = filas.Select(x => new {
            fecha = x.Fecha,
            codigo = x.Codigo,
            nombre = x.Nombre,
            departamento = x.Departamento,
            totalChecadas = x.TotalChecadas,
            primeraChecada = x.PrimeraChecada,
            ultimaChecada = x.UltimaChecada,
            minutosEntreExtremos = (int)(x.UltimaChecada - x.PrimeraChecada).TotalMinutes
        });
        return new JsonResult(new { success = true, total, pagina, paginas, tamano, registros });
    }

    // Anomalías técnicas por conteo: no se etiquetan como ausencias ni incidencias validadas.
    public async Task<IActionResult> OnGetIncidenciasRevisionAsync(DateTime? fechaInicio, DateTime? fechaFin,
        string? empleado, string? departamento, string? buscar, int pagina = 1, int tamano = 10,
        CancellationToken ct = default)
    {
        if (!PeriodoValido(fechaInicio, fechaFin) || !PaginacionPestanasValida(pagina, tamano) || buscar?.Length > 100)
            return BadRequest(new { success = false, message = "Filtros o paginación inválidos." });
        var q = ConsultaDiaria(fechaInicio!.Value.Date, fechaFin!.Value.Date, empleado, departamento, buscar)
            .Where(x => x.TotalChecadas == 1 || x.TotalChecadas % 2 != 0);
        var total = await q.CountAsync(ct);
        var paginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamano));
        pagina = Math.Min(pagina, paginas);
        var filas = await q.OrderByDescending(x => x.Fecha).ThenBy(x => x.Codigo)
            .Skip((pagina - 1) * tamano).Take(tamano).ToListAsync(ct);
        var registros = filas.Select(x => new {
            fecha = x.Fecha,
            codigo = x.Codigo,
            nombre = x.Nombre,
            departamento = x.Departamento,
            totalChecadas = x.TotalChecadas,
            observacion = x.TotalChecadas == 1 ? "Un solo registro: revisar" : "Cantidad impar: revisar"
        });
        return new JsonResult(new { success = true, total, pagina, paginas, tamano, registros });
    }

    // La auditoría de sincronización permanece visible sin solicitar rango; el
    // acceso sigue restringido al atributo Authorize del PageModel.
    public async Task<IActionResult> OnGetAuditoriaSincronizacionAsync(int pagina = 1, int tamano = 10,
        CancellationToken ct = default)
    {
        if (!PaginacionPestanasValida(pagina, tamano))
            return BadRequest(new { success = false, message = "Paginación inválida." });
        var q = _db.BioTimeSincronizaciones.AsNoTracking();
        var total = await q.CountAsync(ct);
        var paginas = Math.Max(1, (int)Math.Ceiling(total / (double)tamano));
        pagina = Math.Min(pagina, paginas);
        var registros = await q.OrderByDescending(x => x.InicioUtc).ThenByDescending(x => x.Id)
            .Skip((pagina - 1) * tamano).Take(tamano)
            .Select(x => new {
                x.Id,
                x.InicioUtc,
                x.FinUtc,
                x.FechaInicioConsulta,
                x.FechaFinConsulta,
                x.Estado,
                x.EmpleadosProcesados,
                x.ChecadasInsertadas,
                x.Error
            })
            .ToListAsync(ct);
        return new JsonResult(new { success = true, total, pagina, paginas, tamano, registros });
    }

}
