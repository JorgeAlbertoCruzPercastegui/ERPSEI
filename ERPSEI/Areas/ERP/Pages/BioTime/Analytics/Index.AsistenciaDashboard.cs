
using System.Globalization;
using System.Text;
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    // GET ?handler=AsistenciaDashboard
    public async Task<IActionResult> OnGetAsistenciaDashboardAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        int? empleadoId,
        string? buscar,
        int pagina,
        [FromServices] RelojAsistenciaDashboardService servicio,
        CancellationToken ct)
    {
        if (!PuedeConsultarAsistenciaDashboard())
            return Forbid();

        if (!fechaInicio.HasValue || !fechaFin.HasValue)
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
                CrearFiltros(
                    fechaInicio.Value,
                    fechaFin.Value,
                    empleadoId,
                    buscar,
                    pagina),
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

    // GET ?handler=ExportarAsistenciaDashboard
    // Exportación informativa de la página consultada.
    public async Task<IActionResult>
        OnGetExportarAsistenciaDashboardAsync(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int? empleadoId,
            string? buscar,
            int pagina,
            [FromServices] RelojAsistenciaDashboardService servicio,
            CancellationToken ct)
    {
        if (!PuedeConsultarAsistenciaDashboard())
            return Forbid();

        if (!fechaInicio.HasValue || !fechaFin.HasValue)
            return BadRequest("Selecciona ambas fechas.");

        RelojAsistenciaDashboardService.ResumenDashboard resultado;

        try
        {
            resultado = await servicio.ConsultarAsync(
                CrearFiltros(
                    fechaInicio.Value,
                    fechaFin.Value,
                    empleadoId,
                    buscar,
                    pagina),
                ct);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }

        var csv = new StringBuilder();

        csv.AppendLine(
            "REPORTE PRELIMINAR DE ASISTENCIA - INTRANET");

        csv.AppendLine(
            "Los resultados requieren validación de Talento Humano. " +
            "No representan incidencias definitivas ni descuentos de nómina.");

        csv.AppendLine();

        csv.AppendLine(
            "EmpleadoId,Colaborador,Fecha,Horario,EstadoTecnico," +
            "IntegridadChecadas,CumplimientoHorario," +
            "Entrada,SalidaComer,RegresoComer,Salida," +
            "EntradaPuntual,RegresoComidaPuntual,SalidaEnHorario," +
            "MinutosEfectivos,MinutosRetardoPotencial," +
            "MinutosSalidaAnticipadaPotencial," +
            "Intranet,BioTime,Confirmados,Pendientes," +
            "HomeOfficeAutorizado,TieneSolicitudesAusencia," +
            "RequiereRevision,Observaciones");

        foreach (var r in resultado.Registros)
        {
            var valores = new[]
            {
                r.EmpleadoId.ToString(CultureInfo.InvariantCulture),
                r.Colaborador,
                r.Fecha.ToString("yyyy-MM-dd"),
                r.Horario,
                r.Estado,
                r.IntegridadChecadas,
                r.CumplimientoHorario,

                r.Entrada ?? "",
                r.SalidaComer ?? "",
                r.RegresoComer ?? "",
                r.Salida ?? "",

                TextoBooleano(r.EntradaPuntual),
                TextoBooleano(r.RegresoComidaPuntual),
                TextoBooleano(r.SalidaEnHorario),

                Numero(r.MinutosEfectivos),
                Numero(r.MinutosRetardoPotencial),
                Numero(r.MinutosSalidaAnticipadaPotencial),

                r.MovimientosIntranet.ToString(
                    CultureInfo.InvariantCulture),

                r.MovimientosBioTime.ToString(
                    CultureInfo.InvariantCulture),

                r.Confirmados.ToString(
                    CultureInfo.InvariantCulture),

                r.Pendientes.ToString(
                    CultureInfo.InvariantCulture),

                r.HomeOfficeAutorizado ? "Sí" : "No",
                r.TieneAusencias ? "Sí" : "No",
                r.RequiereRevision ? "Sí" : "No",
                r.Observaciones
            };

            csv.AppendLine(string.Join(
                ",",
                valores.Select(CeldaCsv)));
        }

        var encoding = new UTF8Encoding(true);

        var bytes = encoding.GetPreamble()
            .Concat(encoding.GetBytes(csv.ToString()))
            .ToArray();

        var nombre =
            $"INTRANET_Asistencia_Preliminar_" +
            $"{fechaInicio.Value:yyyyMMdd}_" +
            $"{fechaFin.Value:yyyyMMdd}_" +
            $"P{resultado.Pagina}.csv";

        return File(
            bytes,
            "text/csv; charset=utf-8",
            nombre);
    }

    private static RelojAsistenciaDashboardService.Filtros
        CrearFiltros(
            DateTime fechaInicio,
            DateTime fechaFin,
            int? empleadoId,
            string? buscar,
            int pagina)
    {
        return new RelojAsistenciaDashboardService.Filtros
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            EmpleadoId = empleadoId,
            Buscar = buscar,
            Pagina = pagina < 1 ? 1 : pagina,
            Tamano = 10
        };
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

    private static string Numero(int? valor)
    {
        return valor?.ToString(
            CultureInfo.InvariantCulture) ?? "";
    }

    private static string CeldaCsv(string? valor)
    {
        var texto = valor ?? "";

        // Evitar interpretación de fórmulas en Excel
        // incluso si existen espacios iniciales.
        var inicio = texto.TrimStart(
            ' ', '\t', '\r', '\n');

        if (inicio.Length > 0 &&
            "=+-@".Contains(inicio[0]))
        {
            texto = "'" + texto;
        }

        return "\"" +
            texto.Replace("\"", "\"\"") +
            "\"";
    }
}
