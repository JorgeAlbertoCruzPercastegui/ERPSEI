
using System.Security.Claims;
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    private bool PuedeResolverConflictos()
    {
        return User.Identity?.IsAuthenticated == true &&
            (
                User.IsInRole("Master") ||
                User.IsInRole("Administrador") ||
                User.IsInRole("Administrador TH")
            );
    }

    private static bool FechaResolucionValida(DateTime fecha)
    {
        var zona = TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

        var hoy = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow, zona).Date;

        return fecha.Date >= hoy.AddDays(-365) &&
               fecha.Date <= hoy;
    }

    // GET ?handler=ResolucionesConflictos
    public async Task<IActionResult> OnGetResolucionesConflictosAsync(
        int empleadoId,
        DateTime? fecha,
        [FromServices]
        RelojEvaluacionAsistenciaUnificadaService evaluador,
        [FromServices]
        RelojResolucionConflictosService resoluciones,
        CancellationToken ct)
    {
        if (!PuedeResolverConflictos()) return Forbid();

        if (empleadoId <= 0 || !fecha.HasValue ||
            !FechaResolucionValida(fecha.Value))
        {
            return BadRequest(new
            {
                success = false,
                message = "Empleado o fecha no válidos."
            });
        }

        var evaluacion = await evaluador.EvaluarDiaAsync(
            empleadoId, fecha.Value.Date, ct);

        var grupos = await resoluciones.ObtenerGruposAsync(
            empleadoId,
            fecha.Value.Date,
            evaluacion.Movimientos,
            ct);

        return new JsonResult(new
        {
            success = true,
            evaluacion = new
            {
                evaluacion.Estado,
                evaluacion.Mensaje,
                evaluacion.ResolucionesAplicadas
            },
            grupos,
            movimientos = evaluacion.Movimientos.Select(x => new
            {
                x.IdMovimiento,
                x.Fuente,
                x.Origen,
                x.TipoEvaluacion,
                x.FechaHoraMexico,
                x.Confirmado,
                x.ParticipaEnEvaluacion,
                x.Observacion
            })
        });
    }

    // POST ?handler=ResolverConflicto
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostResolverConflictoAsync(
        int empleadoId,
        DateTime fecha,
        string tipoMovimiento,
        string movimientoSeleccionado,
        string firmaEsperada,
        string motivo,
        [FromServices]
        RelojEvaluacionAsistenciaUnificadaService evaluador,
        [FromServices]
        RelojResolucionConflictosService resoluciones,
        CancellationToken ct)
    {
        if (!PuedeResolverConflictos()) return Forbid();

        var actor = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(actor))
            return Unauthorized();

        if (empleadoId <= 0 ||
            !FechaResolucionValida(fecha))
        {
            return BadRequest(new
            {
                success = false,
                message = "Empleado o fecha inválidos."
            });
        }

        var evaluacion = await evaluador.EvaluarDiaAsync(
            empleadoId, fecha.Date, ct);

        try
        {
            var resultado = await resoluciones.ResolverAsync(
                empleadoId,
                fecha.Date,
                tipoMovimiento,
                movimientoSeleccionado,
                firmaEsperada,
                motivo,
                actor,
                evaluacion.Movimientos,
                ct);

            return resultado.Success
                ? new JsonResult(resultado)
                : BadRequest(resultado);
        }
        catch (SqlException ex) when (
            ex.Number is 2601 or 2627 or 1205)
        {
            return StatusCode(409, new
            {
                success = false,
                message =
                    "Hubo un conflicto de concurrencia. " +
                    "Actualiza los movimientos e inténtalo de nuevo."
            });
        }
    }

    // POST ?handler=RevocarResolucionConflicto
    [ValidateAntiForgeryToken]
    public async Task<IActionResult>
        OnPostRevocarResolucionConflictoAsync(
            int empleadoId,
            DateTime fecha,
            string tipoMovimiento,
            string motivo,
            [FromServices]
            RelojResolucionConflictosService resoluciones,
            CancellationToken ct)
    {
        if (!PuedeResolverConflictos()) return Forbid();

        var actor = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(actor))
            return Unauthorized();

        if (empleadoId <= 0 ||
            !FechaResolucionValida(fecha))
        {
            return BadRequest(new
            {
                success = false,
                message = "Empleado o fecha inválidos."
            });
        }

        try
        {
            var resultado = await resoluciones.RevocarAsync(
                empleadoId,
                fecha.Date,
                tipoMovimiento,
                motivo,
                actor,
                ct);

            return resultado.Success
                ? new JsonResult(resultado)
                : BadRequest(resultado);
        }
        catch (SqlException ex) when (
            ex.Number is 1205)
        {
            return StatusCode(409, new
            {
                success = false,
                message =
                    "La resolución está siendo modificada. " +
                    "Actualiza e inténtalo nuevamente."
            });
        }
    }
}
