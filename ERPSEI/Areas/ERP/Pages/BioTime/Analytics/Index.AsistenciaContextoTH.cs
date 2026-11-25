
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    // GET:
    // ?handler=AsistenciaContextoTH
    // &empleadoId=35
    // &fecha=2026-11-24

    public async Task<IActionResult>
        OnGetAsistenciaContextoTHAsync(
            int empleadoId,
            DateTime? fecha,
            [FromServices]
            RelojEvaluacionAsistenciaUnificadaService evaluador,
            [FromServices]
            RelojAsistenciaContextoTHService contextoTH,
            CancellationToken ct)
    {
        if (User.Identity?.IsAuthenticated != true)
            return new UnauthorizedResult();

        if (!(
            User.IsInRole("Master") ||
            User.IsInRole("Administrador") ||
            User.IsInRole("Administrador TH")))
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

        // Se consultan secuencialmente porque ambos
        // servicios comparten ApplicationDbContext.
        var evaluacion = await evaluador.EvaluarDiaAsync(
            empleadoId,
            dia,
            ct);


        var contexto = evaluacion.ContextoTH
            ?? await contextoTH.ObtenerAsync(
                empleadoId,
                dia,
                ct);


        bool? entradaPuntual =
            evaluacion.EvaluacionTH?.Puntualidad switch
            {
                "Puntual" => true,
                "RetardoPotencial" => false,
                _ => null
            };

        bool? salidaEnHorario =
            evaluacion.EvaluacionTH?.EstadoSalida switch
            {
                "SalidaEnHorario" => true,
                "SalidaAnticipadaPotencial" => false,
                _ => null
            };


        bool? regresoComidaPuntual = null;

        int? minutosEfectivos = null;

        if (evaluacion.SecuenciaCompleta &&
            evaluacion.EntradaReal.HasValue &&
            evaluacion.SalidaComidaReal.HasValue &&
            evaluacion.RegresoComidaReal.HasValue &&
            evaluacion.SalidaReal.HasValue &&
            evaluacion.EntradaReal <
                evaluacion.SalidaComidaReal &&
            evaluacion.SalidaComidaReal <
                evaluacion.RegresoComidaReal &&
            evaluacion.RegresoComidaReal <
                evaluacion.SalidaReal)
        {
            var antesDeComida =
                evaluacion.SalidaComidaReal.Value -
                evaluacion.EntradaReal.Value;

            var despuesDeComida =
                evaluacion.SalidaReal.Value -
                evaluacion.RegresoComidaReal.Value;

            minutosEfectivos = (int)Math.Floor(
                (antesDeComida + despuesDeComida)
                .TotalMinutes);
        }

        return new JsonResult(new
        {
            success = true,
            empleadoId,
            fecha = dia.ToString("yyyy-MM-dd"),

            evaluacion,

            contextoTH = contexto,

            indicadoresPreliminares = new
            {
                entradaPuntual,
                regresoComidaPuntual,
                salidaEnHorario,
                minutosEfectivos,

                requiereRevision =
                    evaluacion.RequiereRevision ||
                    contexto.RequiereRevisionTH,

                esResultadoLaboralDefinitivo = false
            }
        });
    }
}
