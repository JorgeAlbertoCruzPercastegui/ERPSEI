
using System.Security.Claims;
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    // Reutilizamos el ApplicationDbContext del IndexModel
    // para resolver los servicios con el mismo scope.

    private bool PuedeAdministrarVinculaciones =>
        User.IsInRole("Master") ||
        User.IsInRole("Administrador") ||
        User.IsInRole("Administrador TH");

    private string? UsuarioActualId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier);

    // -----------------------------------------------------
    // CONSULTAR VINCULACIONES
    // -----------------------------------------------------

    public async Task<IActionResult>
        OnGetVinculacionesBioTimeAsync(
            [FromServices]
            RelojVinculacionesBioTimeAdminService servicio,
            CancellationToken ct)
    {
        if (!PuedeAdministrarVinculaciones)
            return Forbid();

        var registros =
            await servicio.ObtenerTodasAsync(ct);

        return new JsonResult(new
        {
            success = true,
            total = registros.Count,
            registros
        });
    }

    // -----------------------------------------------------
    // VALIDAR IDENTIDAD Y VIGENCIA
    // -----------------------------------------------------

    public async Task<IActionResult>
        OnPostValidarVinculacionBioTimeAsync(
            long id,
            DateTime fechaInicio,
            DateTime? fechaFin,
            bool confirmarIdentidad,
            [FromServices]
            RelojVinculacionesBioTimeService servicio,
            CancellationToken ct)
    {
        if (!PuedeAdministrarVinculaciones)
            return Forbid();

        if (!confirmarIdentidad)
        {
            return BadRequest(new
            {
                success = false,
                message = "Debes confirmar que la identidad fue verificada."
            });
        }

        if (string.IsNullOrWhiteSpace(UsuarioActualId))
            return Unauthorized();

        if (fechaInicio == default)
        {
            return BadRequest(new
            {
                success = false,
                message = "Selecciona una fecha inicial."
            });
        }

        if (fechaFin.HasValue &&
            fechaFin.Value.Date < fechaInicio.Date)
        {
            return BadRequest(new
            {
                success = false,
                message = "La fecha final es anterior a la inicial."
            });
        }

        var resultado = await servicio.ValidarAsync(
            id,
            UsuarioActualId!,
            fechaInicio.Date,
            fechaFin?.Date,
            ct);

        if (!resultado.Correcto)
        {
            return BadRequest(new
            {
                success = false,
                codigo = resultado.Codigo,
                message = resultado.Mensaje
            });
        }

        return new JsonResult(new
        {
            success = true,
            codigo = resultado.Codigo,
            message = resultado.Mensaje
        });
    }

    // -----------------------------------------------------
    // CERRAR VIGENCIA CONSERVANDO HISTÓRICO
    // -----------------------------------------------------

    public async Task<IActionResult>
        OnPostCerrarVinculacionBioTimeAsync(
            long id,
            DateTime fechaFin,
            [FromServices]
            RelojVinculacionesBioTimeAdminService servicio,
            CancellationToken ct)
    {
        if (!PuedeAdministrarVinculaciones)
            return Forbid();

        if (string.IsNullOrWhiteSpace(UsuarioActualId))
            return Unauthorized();

        if (fechaFin == default)
        {
            return BadRequest(new
            {
                success = false,
                message = "Debes indicar la fecha final."
            });
        }

        var resultado = await servicio.CerrarVigenciaAsync(
            id,
            fechaFin,
            UsuarioActualId!,
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

    // -----------------------------------------------------
    // REVOCAR VINCULACIÓN
    // -----------------------------------------------------

    public async Task<IActionResult>
        OnPostRevocarVinculacionBioTimeAsync(
            long id,
            bool confirmarRevocacion,
            [FromServices]
            RelojVinculacionesBioTimeAdminService servicio,
            CancellationToken ct)
    {
        if (!PuedeAdministrarVinculaciones)
            return Forbid();

        if (string.IsNullOrWhiteSpace(UsuarioActualId))
            return Unauthorized();

        if (!confirmarRevocacion)
        {
            return BadRequest(new
            {
                success = false,
                message = "Confirma la revocación."
            });
        }

        var resultado = await servicio.RevocarAsync(
            id,
            UsuarioActualId!,
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
}