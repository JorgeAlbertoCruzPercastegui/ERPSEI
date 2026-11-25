
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{

    public async Task<IActionResult>
        OnGetClasificacionBioTimeAsync(
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
        {
            return Forbid();
        }

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
                RelojClasificacionMovimientosService>();

        var resultado = await servicio.ClasificarDiaAsync(
            empleadoId,
            dia,
            ct);

        return new JsonResult(new
        {
            success = true,
            resultado.EmpleadoId,
            resultado.FechaLocal,
            resultado.Estado,
            resultado.Mensaje,
            resultado.HorarioId,
            resultado.HorarioDescripcion,
            resultado.EntradaProgramada,
            resultado.SalidaProgramada,
            resultado.TotalMovimientos,
            resultado.TotalBioTime,
            resultado.TotalIntranet,
            resultado.PropuestaCompleta,
            resultado.RequiereRevision,
            resultado.Observaciones,

            movimientos = resultado.Movimientos.Select(x => new
            {
                x.IdMovimiento,
                x.Fuente,
                x.Origen,
                x.TipoOriginal,
                x.TipoPropuesto,
                x.EstadoOriginal,
                x.FechaHoraMexico,
                x.FechaHoraUtc,
                x.Observacion
            })
        });
    }
}
