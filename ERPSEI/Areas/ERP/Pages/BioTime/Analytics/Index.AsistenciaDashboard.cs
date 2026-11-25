
using System.Globalization;
using System.Text;
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    // GET:
    // ?handler=AsistenciaDashboard
    // &fechaInicio=2026-11-24
    // &fechaFin=2026-11-25
    // &empleadoId=35
    // &pagina=1

    public async Task<IActionResult>
        OnGetAsistenciaDashboardAsync(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int? empleadoId,
            string? buscar,
            int pagina,
            [FromServices]
            RelojAsistenciaDashboardService servicio,
            CancellationToken ct)
    {
        if (!PuedeConsultarAsistenciaDashboard())
            return Forbid();

        if (!fechaInicio.HasValue ||
            !fechaFin.HasValue)
        {
            return BadRequest(new
            {
                success = false,
                message = "Selecciona ambas fechas."
            });
        }

        try
        {
            var resultado = await servicio.ConsultarAsync(
                new RelojAsistenciaDashboardService.Filtros
                {
                    FechaInicio = fechaInicio.Value,
                    FechaFin = fechaFin.Value,
                    EmpleadoId = empleadoId,
                    Buscar = buscar,
                    Pagina = pagina < 1 ? 1 : pagina,
                    Tamano = 10
                },
                ct);

            return new JsonResult(new
            {
                success = true,
                resultado
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
    }

    // GET:
    // ?handler=ExportarAsistenciaDashboard
    //
    // Exporta la página seleccionada.
    // No modifica asistencia ni clasificaciones.

    public async Task<IActionResult>
        OnGetExportarAsistenciaDashboardAsync(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int? empleadoId,
            string? buscar,
            int pagina,
            [FromServices]
            RelojAsistenciaDashboardService servicio,
            CancellationToken ct)
    {
        if (!PuedeConsultarAsistenciaDashboard())
            return Forbid();

        if (!fechaInicio.HasValue ||
            !fechaFin.HasValue)
        {
            return BadRequest(
                "Selecciona ambas fechas.");
        }

        RelojAsistenciaDashboardService.ResumenDashboard resultado;

        try
        {
            resultado = await servicio.ConsultarAsync(
                new RelojAsistenciaDashboardService.Filtros
                {
                    FechaInicio = fechaInicio.Value,
                    FechaFin = fechaFin.Value,
                    EmpleadoId = empleadoId,
                    Buscar = buscar,
                    Pagina = pagina < 1 ? 1 : pagina,
                    Tamano = 10
                },
                ct);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var csv = new StringBuilder();

        csv.AppendLine(
            "EmpleadoId,Colaborador,Fecha,Horario,Estado," +
            "Entrada,SalidaComer,RegresoComer,Salida," +
            "EntradaPuntual,RegresoComidaPuntual,SalidaEnHorario," +
            "MinutosEfectivos,Intranet,BioTime,Confirmados," +
            "Pendientes,HomeOffice,Ausencias,RequiereRevision,Observaciones");

        foreach (var r in resultado.Registros)
        {
            var valores = new[]
            {
                r.EmpleadoId.ToString(CultureInfo.InvariantCulture),
                r.Colaborador,
                r.Fecha.ToString("yyyy-MM-dd"),
                r.Horario,
                r.Estado,
                r.Entrada ?? "",
                r.SalidaComer ?? "",
                r.RegresoComer ?? "",
                r.Salida ?? "",
                TextoBooleano(r.EntradaPuntual),
                TextoBooleano(r.RegresoComidaPuntual),
                TextoBooleano(r.SalidaEnHorario),
                r.MinutosEfectivos?.ToString(
                    CultureInfo.InvariantCulture) ?? "",
                r.MovimientosIntranet.ToString(),
                r.MovimientosBioTime.ToString(),
                r.Confirmados.ToString(),
                r.Pendientes.ToString(),
                r.HomeOfficeAutorizado ? "Sí" : "No",
                r.TieneAusencias ? "Sí" : "No",
                r.RequiereRevision ? "Sí" : "No",
                r.Observaciones
            };

            csv.AppendLine(string.Join(
                ",",
                valores.Select(CeldaCsv)));
        }

        // BOM UTF-8 para acentos en Excel.
        var bytes = new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: true)
            .GetPreamble()
            .Concat(Encoding.UTF8.GetBytes(csv.ToString()))
            .ToArray();

        var nombre =
            $"INTRANET_Asistencia_{fechaInicio:yyyyMMdd}" +
            $"_{fechaFin:yyyyMMdd}_P{resultado.Pagina}.csv";

        return File(
            bytes,
            "text/csv; charset=utf-8",
            nombre);
    }

    private bool PuedeConsultarAsistenciaDashboard()
    {
        return User.Identity?.IsAuthenticated == true &&
               (
                   User.IsInRole("Master") ||
                   User.IsInRole("Administrador") ||
                   User.IsInRole("Administrador TH")
               );
    }

    private static string TextoBooleano(bool? valor)
    {
        return valor.HasValue
            ? valor.Value ? "Sí" : "No"
            : "Sin determinar";
    }

    private static string CeldaCsv(string? valor)
    {
        var texto = valor ?? "";

        // Protección contra interpretación de fórmulas
        // al abrir reportes en Excel.
        if (texto.Length > 0 &&
            (texto.TrimStart().StartsWith("=") ||
             texto.TrimStart().StartsWith("+") ||
             texto.TrimStart().StartsWith("-") ||
             texto.TrimStart().StartsWith("@")))
        {
            texto = "'" + texto;
        }

        return "\"" +
               texto.Replace("\"", "\"\"") +
               "\"";
    }
}
