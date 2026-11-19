using ClosedXML.Excel;
using ERPSEI.Data;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Services.BioTimeIntegration;

// Exportación operativa. Máximo 50 000 filas por solicitud para acotar memoria.
public sealed class BioTimeExcelService
{
    private readonly ApplicationDbContext _db;
    private const int MaxFilas = 50_000;
    public BioTimeExcelService(ApplicationDbContext db) { _db = db; }

    public async Task<byte[]> GenerarReporteAsync(DateTime desde, DateTime hasta,
        string? empleado, string? departamento, string tipo, CancellationToken ct)
    {
        if (desde.Date > hasta.Date || (hasta.Date - desde.Date).Days >= 365)
            throw new ArgumentException("Periodo inválido.");
        if (tipo != "checadas" && tipo != "resumen") throw new ArgumentException("Tipo inválido.");
        var q = from c in _db.BioTimeChecadas.AsNoTracking()
                join e in _db.BioTimeEmpleados.AsNoTracking() on c.EmpCode equals e.EmpCode into g
                from e in g.DefaultIfEmpty()
                where c.FechaHora >= desde.Date && c.FechaHora < hasta.Date.AddDays(1)
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
        using var libro = new XLWorkbook();
        var hoja = libro.AddWorksheet(tipo == "checadas" ? "Checadas" : "Resumen diario");
        var headers = tipo == "checadas"
            ? new[] { "ID BioTime", "Código", "Colaborador", "Departamento", "Fecha y hora", "Estado original", "Dispositivo" }
            : new[] { "Fecha", "Código", "Colaborador", "Departamento", "Checadas", "Primera", "Última", "Minutos entre extremos" };
        for (var j = 0; j < headers.Length; j++) hoja.Cell(1, j + 1).Value = headers[j];
        var fila = 2;
        if (tipo == "checadas")
        {
            await foreach (var c in q.OrderBy(x => x.FechaHora).ThenBy(x => x.Id).AsAsyncEnumerable().WithCancellation(ct))
            {
                if (fila - 2 >= MaxFilas) throw new InvalidOperationException("Límite de exportación excedido.");
                hoja.Cell(fila, 1).Value = c.Id;
                Texto(hoja.Cell(fila, 2), c.Codigo);
                Texto(hoja.Cell(fila, 3), c.Nombre);
                Texto(hoja.Cell(fila, 4), c.Departamento);
                hoja.Cell(fila, 5).Value = c.FechaHora;
                Texto(hoja.Cell(fila, 6), c.Estado);
                Texto(hoja.Cell(fila, 7), c.Dispositivo);
                fila++;
            }
            if (fila > 2) hoja.Range(2, 5, fila - 1, 5).Style.DateFormat.Format = "dd/mm/yyyy hh:mm:ss";
        }
        else
        {
            var agrupado = q.GroupBy(x => new { Fecha = x.FechaHora.Date, x.Codigo, x.Nombre, x.Departamento })
                .Select(g => new {
                    g.Key.Fecha,
                    g.Key.Codigo,
                    g.Key.Nombre,
                    g.Key.Departamento,
                    Total = g.Count(),
                    Primera = g.Min(x => x.FechaHora),
                    Ultima = g.Max(x => x.FechaHora)
                });
            await foreach (var x in agrupado.OrderBy(x => x.Fecha).ThenBy(x => x.Codigo).AsAsyncEnumerable().WithCancellation(ct))
            {
                if (fila - 2 >= MaxFilas) throw new InvalidOperationException("Límite de exportación excedido.");
                hoja.Cell(fila, 1).Value = x.Fecha;
                Texto(hoja.Cell(fila, 2), x.Codigo);
                Texto(hoja.Cell(fila, 3), x.Nombre);
                Texto(hoja.Cell(fila, 4), x.Departamento);
                hoja.Cell(fila, 5).Value = x.Total;
                hoja.Cell(fila, 6).Value = x.Primera;
                hoja.Cell(fila, 7).Value = x.Ultima;
                hoja.Cell(fila, 8).Value = (int)(x.Ultima - x.Primera).TotalMinutes;
                fila++;
            }
            if (fila > 2)
            {
                hoja.Range(2, 1, fila - 1, 1).Style.DateFormat.Format = "dd/mm/yyyy";
                hoja.Range(2, 6, fila - 1, 7).Style.DateFormat.Format = "dd/mm/yyyy hh:mm:ss";
            }
        }
        hoja.Range(1, 1, 1, headers.Length).Style.Font.Bold = true;
        hoja.Range(1, 1, 1, headers.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#21166b");
        hoja.Range(1, 1, 1, headers.Length).Style.Font.FontColor = XLColor.White;
        hoja.SheetView.FreezeRows(1);
        hoja.Columns().AdjustToContents(10, 50);
        using var ms = new MemoryStream();
        libro.SaveAs(ms);
        return ms.ToArray();
    }

    // Evita que los datos importados se ejecuten como fórmulas al abrir Excel.
    internal static void Texto(IXLCell celda, string? valor)
    {
        var texto = valor ?? "";
        if (texto.Length > 32_000) texto = texto[..32_000];
        if (texto.Length > 0 && "=+-@\t\r\n".Contains(texto[0])) texto = "'" + texto;
        celda.SetValue(texto);
        celda.Style.NumberFormat.Format = "@";
    }
}