
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace ERPSEI.Services.TimeIntegration
{
    public interface ITimeReportExportService
    {
        Task<byte[]> GenerarExcelAsync(
            TimeReporteHorasFiltro filtro,
            CancellationToken cancellationToken = default);

        Task<byte[]> GenerarPdfAsync(
            TimeReporteHorasFiltro filtro,
            CancellationToken cancellationToken = default);
    }

    public sealed class TimeReportExportService : ITimeReportExportService
    {
        private readonly ITimeSqlReportService _reportes;

        private const int Lote = 100;
        private const int MaxExcel = 50000;
        private const int MaxPdf = 5000;

        private static readonly string[] Encabezados =
        {
            "Nombre",
            "Área",
            "Puesto",
            "Nivel de Cliente",
            "Grupo / Empresa",
            "Interna/externa",
            "Tipo",
            "Fecha",
            "Actividad",
            "Descripción",
            "Horas"
        };

        public TimeReportExportService(
            ITimeSqlReportService reportes)
        {
            _reportes = reportes;
        }

        private async Task<List<TimeReporteHorasFila>> ObtenerRegistrosAsync(
            TimeReporteHorasFiltro filtro,
            int limite,
            CancellationToken cancellationToken)
        {
            // Primera página y total real.
            var primera = await _reportes.ConsultarHorasAsync(
                CopiarFiltro(filtro, 1),
                cancellationToken);

            if (primera.TotalRegistros > limite)
            {
                throw new InvalidOperationException(
                    $"El reporte contiene {primera.TotalRegistros:N0} registros. " +
                    $"El máximo permitido para este formato es {limite:N0}. " +
                    "Reduce el rango de fechas o aplica más filtros.");
            }

            var registros = new List<TimeReporteHorasFila>(
                checked((int)primera.TotalRegistros));

            registros.AddRange(primera.Registros);

            // Se obtiene cada lote completo, no solamente
            // la página que el usuario ve en el navegador.
            for (int pagina = 2;
                 pagina <= primera.TotalPaginas;
                 pagina++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var respuesta = await _reportes.ConsultarHorasAsync(
                    CopiarFiltro(filtro, pagina),
                    cancellationToken);

                // Si el origen sincronizado cambia durante
                // la exportación, no entregar datos parciales.
                if (respuesta.TotalRegistros != primera.TotalRegistros)
                {
                    throw new InvalidOperationException(
                        "Los datos del reporte cambiaron durante " +
                        "la exportación. Intenta nuevamente.");
                }

                registros.AddRange(respuesta.Registros);
            }

            if (registros.Count != primera.TotalRegistros)
            {
                throw new InvalidOperationException(
                    "No se pudieron recuperar todos los registros " +
                    "del reporte. Intenta nuevamente.");
            }

            return registros;
        }

        private static TimeReporteHorasFiltro CopiarFiltro(
            TimeReporteHorasFiltro original,
            int pagina)
        {
            return new TimeReporteHorasFiltro
            {
                FechaInicio = original.FechaInicio,
                FechaFin = original.FechaFin,
                UsuarioId = original.UsuarioId,
                ProyectoId = original.ProyectoId,
                DepartamentoId = original.DepartamentoId,
                Pagina = pagina,
                TamanoPagina = Lote
            };
        }

        private static string Texto(string? valor)
            => valor ?? string.Empty;

        private static string Fecha(DateTime? fecha)
            => fecha?.ToString("dd/MM/yyyy",
                CultureInfo.InvariantCulture) ?? string.Empty;

        private static string Horas(double? valor)
            => valor?.ToString("0.##",
                CultureInfo.InvariantCulture) ?? string.Empty;

        private static string[] Valores(TimeReporteHorasFila fila)
        {
            return new[]
            {
                Texto(fila.Nombre),
                Texto(fila.Area),
                Texto(fila.Puesto),
                Texto(fila.NivelCliente),
                Texto(fila.GrupoEmpresa),
                Texto(fila.InternaExterna),
                Texto(fila.Tipo),
                Fecha(fila.Fecha),
                Texto(fila.Actividad),
                Texto(fila.Descripcion),
                Horas(fila.Horas)
            };
        }

        // =====================================================
        // EXPORTAR EXCEL
        // =====================================================

        public async Task<byte[]> GenerarExcelAsync(
            TimeReporteHorasFiltro filtro,
            CancellationToken cancellationToken = default)
        {
            var registros = await ObtenerRegistrosAsync(
                filtro, MaxExcel, cancellationToken);

            using var libro = new XLWorkbook();

            var hoja = libro.Worksheets.Add("Horas y actividades");

            hoja.Cell(1, 1).Value = "INTRANET | REPORTE TIME";
            hoja.Range(1, 1, 1, 11).Merge();

            hoja.Cell(2, 1).Value =
                "Reporte histórico de horas y actividades";
            hoja.Range(2, 1, 2, 11).Merge();

            hoja.Cell(3, 1).Value =
                $"Total de registros: {registros.Count:N0}";
            hoja.Range(3, 1, 3, 11).Merge();

            hoja.Cell(4, 1).Value =
                $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";
            hoja.Range(4, 1, 4, 11).Merge();

            const int filaEncabezado = 6;

            for (int col = 0; col < Encabezados.Length; col++)
            {
                hoja.Cell(filaEncabezado, col + 1).Value =
                    Encabezados[col];
            }

            var encabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, 11);

            encabezado.Style.Fill.BackgroundColor =
                XLColor.FromHtml("#21166B");

            encabezado.Style.Font.FontColor = XLColor.White;
            encabezado.Style.Font.Bold = true;
            encabezado.Style.Alignment.WrapText = true;

            hoja.Row(filaEncabezado).Height = 30;

            int filaActual = filaEncabezado + 1;

            foreach (var registro in registros)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var valores = Valores(registro);

                for (int col = 0; col < valores.Length; col++)
                {
                    // La fecha y las horas se escriben
                    // con sus tipos reales más abajo.
                    if (col == 7 || col == 10)
                        continue;

                    // Evita interpretar texto externo como fórmula.
                    hoja.Cell(filaActual, col + 1)
                        .SetValue(valores[col]);
                }

                if (registro.Fecha.HasValue)
                {
                    hoja.Cell(filaActual, 8).Value =
                        registro.Fecha.Value;

                    hoja.Cell(filaActual, 8)
                        .Style.DateFormat.Format = "dd/mm/yyyy";
                }

                if (registro.Horas.HasValue)
                {
                    hoja.Cell(filaActual, 11).Value =
                        registro.Horas.Value;

                    hoja.Cell(filaActual, 11)
                        .Style.NumberFormat.Format = "0.00";
                }

                filaActual++;
            }

            int ultimaFilaDatos = filaActual - 1;

            int filaTotal = filaActual + 1;

            hoja.Cell(filaTotal, 10).Value = "TOTAL HORAS";
            hoja.Cell(filaTotal, 10).Style.Font.Bold = true;

            hoja.Cell(filaTotal, 11).Value =
                registros.Sum(x => x.Horas ?? 0);

            hoja.Cell(filaTotal, 11).Style.Font.Bold = true;
            hoja.Cell(filaTotal, 11)
                .Style.NumberFormat.Format = "0.00";

            hoja.Range(1, 1, 1, 11).Style.Font.Bold = true;
            hoja.Range(1, 1, 1, 11).Style.Font.FontSize = 17;
            hoja.Range(1, 1, 1, 11).Style.Font.FontColor =
                XLColor.FromHtml("#21166B");

            // Anchos definidos para evitar un AutoFit
            // costoso sobre grandes volúmenes.
            hoja.Column(1).Width = 30;
            hoja.Column(2).Width = 25;
            hoja.Column(3).Width = 22;
            hoja.Column(4).Width = 20;
            hoja.Column(5).Width = 37;
            hoja.Column(6).Width = 20;
            hoja.Column(7).Width = 13;
            hoja.Column(8).Width = 14;
            hoja.Column(9).Width = 29;
            hoja.Column(10).Width = 48;
            hoja.Column(11).Width = 12;

            if (registros.Count > 0)
            {
                hoja.Range(
                    filaEncabezado, 1,
                    ultimaFilaDatos, 11).SetAutoFilter();
            }

            hoja.SheetView.FreezeRows(filaEncabezado);

            using var stream = new MemoryStream();
            libro.SaveAs(stream);

            return stream.ToArray();
        }

        // =====================================================
        // EXPORTAR PDF
        // =====================================================

        public async Task<byte[]> GenerarPdfAsync(
            TimeReporteHorasFiltro filtro,
            CancellationToken cancellationToken = default)
        {
            var registros = await ObtenerRegistrosAsync(
                filtro, MaxPdf, cancellationToken);

            double totalHoras =
                registros.Sum(x => x.Horas ?? 0);

            var documento = Document.Create(contenedor =>
            {
                contenedor.Page(pagina =>
                {
                    pagina.Size(PageSizes.A3.Landscape());
                    pagina.Margin(18);

                    pagina.DefaultTextStyle(
                        x => x.FontSize(7));

                    pagina.Header().Column(col =>
                    {
                        col.Item()
                            .Text("INTRANET | REPORTE TIME")
                            .Bold()
                            .FontSize(16)
                            .FontColor("#21166B");

                        col.Item().Text(
                            "Horas y actividades | " +
                            $"Registros: {registros.Count:N0} | " +
                            $"Total horas: {totalHoras:N2}");
                    });

                    pagina.Content().PaddingVertical(10)
                        .Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columnas =>
                            {
                                columnas.RelativeColumn(1.3f); // Nombre
                                columnas.RelativeColumn(1.1f); // Área
                                columnas.RelativeColumn(1f);   // Puesto
                                columnas.RelativeColumn(0.9f); // Nivel
                                columnas.RelativeColumn(1.5f); // Empresa
                                columnas.RelativeColumn(0.9f); // Interna
                                columnas.RelativeColumn(0.6f); // Tipo
                                columnas.RelativeColumn(0.8f); // Fecha
                                columnas.RelativeColumn(1.4f); // Actividad
                                columnas.RelativeColumn(2f);   // Descripción
                                columnas.RelativeColumn(0.6f); // Horas
                            });

                            tabla.Header(header =>
                            {
                                foreach (string titulo in Encabezados)
                                {
                                    header.Cell()
                                        .Background("#21166B")
                                        .Padding(4)
                                        .Text(titulo)
                                        .FontColor("#FFFFFF")
                                        .Bold()
                                        .FontSize(6);
                                }
                            });

                            foreach (var registro in registros)
                            {
                                var valores = Valores(registro);

                                foreach (string valor in valores)
                                {
                                    tabla.Cell()
                                        .BorderBottom(0.3f)
                                        .BorderColor("#DDDDDD")
                                        .Padding(3)
                                        .Text(valor)
                                        .FontSize(6);
                                }
                            }
                        });

                    pagina.Footer()
                        .AlignRight()
                        .Text(texto =>
                        {
                            texto.Span("Página ");
                            texto.CurrentPageNumber();
                        });
                });
            });

            return documento.GeneratePdf();
        }
    }
}
