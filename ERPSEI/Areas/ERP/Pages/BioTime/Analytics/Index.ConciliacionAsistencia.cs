
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    // =====================================================
    // INTRANET - CONCILIACIÓN DE ASISTENCIA
    // BioTime + Reloj Checador Digital
    // =====================================================

    // GET:
    // /ERP/BioTime/Analytics
    // ?handler=ConciliacionAsistencia
    // &empleadoId=35
    // &fecha=2026-11-24

    public async Task<IActionResult>
        OnGetConciliacionAsistenciaAsync(
            int empleadoId,
            DateTime? fecha,
            [FromServices]
            RelojConciliacionAsistenciaService servicio,
            CancellationToken ct)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return new UnauthorizedObjectResult(new
            {
                success = false,
                message = "Debes iniciar sesión."
            });
        }

        bool autorizado =
            User.IsInRole("Master") ||
            User.IsInRole("Administrador") ||
            User.IsInRole("Administrador TH");

        if (!autorizado)
            return Forbid();

        if (empleadoId <= 0 || !fecha.HasValue)
        {
            return BadRequest(new
            {
                success = false,
                message = "Selecciona un empleado y una fecha válidos."
            });
        }

        var zonaMexico =
            TimeZoneInfo.FindSystemTimeZoneById(
                "America/Mexico_City");

        var hoyMexico =
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                zonaMexico).Date;

        var fechaConsulta = fecha.Value.Date;

        if (fechaConsulta > hoyMexico ||
            fechaConsulta < hoyMexico.AddDays(-365))
        {
            return BadRequest(new
            {
                success = false,
                message =
                    "Selecciona una fecha dentro de los últimos 365 días."
            });
        }

        var resultado = await servicio.ConciliarDiaAsync(
            empleadoId,
            fechaConsulta,
            ct);

        return new JsonResult(new
        {
            success = true,
            resultado
        });
    }
}
