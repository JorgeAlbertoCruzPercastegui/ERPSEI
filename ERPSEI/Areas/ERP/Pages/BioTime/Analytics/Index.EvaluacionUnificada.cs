
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    // =====================================================
    // EVALUACIÓN UNIFICADA DE ASISTENCIA
    // INTRANET + BIOTIME
    // =====================================================

    public async Task<IActionResult>
        OnGetEvaluacionUnificadaAsync(
            int empleadoId,
            DateTime? fecha,
            CancellationToken ct)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return new UnauthorizedObjectResult(new
            {
                success = false,
                message = "Se requiere iniciar sesión."
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
                message = "Indica un empleado y una fecha válida."
            });
        }

        var zonaMexico =
            TimeZoneInfo.FindSystemTimeZoneById(
                "America/Mexico_City");

        var hoyMexico =
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                zonaMexico).Date;

        var dia = fecha.Value.Date;

        if (dia > hoyMexico ||
            dia < hoyMexico.AddDays(-365))
        {
            return BadRequest(new
            {
                success = false,
                message =
                    "La fecha debe encontrarse dentro de los últimos 365 días."
            });
        }

        var servicio =
            HttpContext.RequestServices.GetRequiredService<
                RelojEvaluacionAsistenciaUnificadaService>();

        var resultado = await servicio.EvaluarDiaAsync(
            empleadoId,
            dia,
            ct);

        return new JsonResult(new
        {
            success = true,
            resultado
        });
    }
}
