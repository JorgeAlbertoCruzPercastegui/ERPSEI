
using System.Security.Claims;
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    public async Task<IActionResult>
        OnGetRevisionChecadasBioTimeAsync(
            int empleadoId,
            DateTime? fecha,
            [FromServices] RelojRevisionBioTimeService servicio,
            CancellationToken ct)
    {
        if (!PuedeRevisarChecadas())
            return Forbid();

        if (empleadoId <= 0 || !fecha.HasValue)
            return BadRequest(new
            {
                success = false,
                message = "Selecciona un empleado y una fecha."
            });

        var registros = await servicio.ObtenerDiaAsync(
            empleadoId, fecha.Value.Date, ct);

        return new JsonResult(new
        {
            success = true,
            empleadoId,
            fecha = fecha.Value.ToString("yyyy-MM-dd"),
            total = registros.Count,
            confirmadas = registros.Count(x =>
                x.Estado == "Confirmada"),
            registros
        });
    }

    public async Task<IActionResult>
        OnPostConfirmarChecadaBioTimeAsync(
            long checadaId,
            int empleadoId,
            string tipoMovimiento,
            string motivo,
            [FromServices] RelojRevisionBioTimeService servicio,
            CancellationToken ct)
    {
        if (!PuedeRevisarChecadas())
            return Forbid();

        var actor = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(actor))
            return new UnauthorizedResult();

        var resultado = await servicio.ConfirmarAsync(
            checadaId,
            empleadoId,
            tipoMovimiento,
            motivo,
            actor,
            ct);

        return resultado.Correcto
            ? new JsonResult(new
            {
                success = true,
                codigo = resultado.Codigo,
                message = resultado.Mensaje
            })
            : BadRequest(new
            {
                success = false,
                codigo = resultado.Codigo,
                message = resultado.Mensaje
            });
    }

    public async Task<IActionResult>
        OnPostRevocarChecadaBioTimeAsync(
            long checadaId,
            int empleadoId,
            string motivo,
            [FromServices] RelojRevisionBioTimeService servicio,
            CancellationToken ct)
    {
        if (!PuedeRevisarChecadas())
            return Forbid();

        var actor = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(actor))
            return new UnauthorizedResult();

        var resultado = await servicio.RevocarAsync(
            checadaId,
            empleadoId,
            motivo,
            actor,
            ct);

        return resultado.Correcto
            ? new JsonResult(new
            {
                success = true,
                message = resultado.Mensaje
            })
            : BadRequest(new
            {
                success = false,
                message = resultado.Mensaje
            });
    }

    private bool PuedeRevisarChecadas()
    {
        return User.Identity?.IsAuthenticated == true &&
               (
                   User.IsInRole("Master") ||
                   User.IsInRole("Administrador") ||
                   User.IsInRole("Administrador TH")
               );
    }
}
