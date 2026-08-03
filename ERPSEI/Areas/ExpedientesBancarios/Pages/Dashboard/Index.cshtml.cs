using ERPSEI.Data;
using ERPSEI.Data.Entities.ExpedientesBancarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace ERPSEI.Areas.ExpedientesBancarios.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        // =====================================================
        // EXPORTAR BITÁCORA DOCUMENTAL
        // GET ?handler=ExportarBitacora
        //     &fechaInicio=2026-08-01
        //     &fechaFin=2026-08-31
        // =====================================================
        public async Task<IActionResult> OnGetExportarBitacoraAsync(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            // =================================================
            // VALIDAR FECHAS
            // =================================================
            if (!fechaInicio.HasValue)
            {
                return BadRequest(
                    "Selecciona la fecha inicial."
                );
            }

            if (!fechaFin.HasValue)
            {
                return BadRequest(
                    "Selecciona la fecha final."
                );
            }

            DateTime inicio =
                fechaInicio.Value.Date;

            DateTime fin =
                fechaFin.Value.Date;

            if (fin < inicio)
            {
                return BadRequest(
                    "La fecha final no puede ser anterior a la fecha inicial."
                );
            }

            if ((fin - inicio).TotalDays > 1826)
            {
                return BadRequest(
                    "El periodo seleccionado no puede superar cinco años."
                );
            }

            /*
             * La fecha final se maneja como exclusiva para incluir
             * todos los registros del último día seleccionado.
             */
            DateTime finExclusivo =
                fin.AddDays(1);

            // =================================================
            // CONSULTAR BITÁCORA
            // =================================================
            var registros =
                await (
                    from bitacora in _context
                        .EbBitacoraDocumentos
                        .AsNoTracking()

                    join empresa in _context
                        .EbEmpresas
                        .AsNoTracking()
                        on bitacora.EmpresaId
                        equals empresa.Id

                    join tipoDocumento in _context
                        .EbTiposDocumento
                        .AsNoTracking()
                        on bitacora.TipoDocumentoId
                        equals tipoDocumento.Id
                        into tiposDocumentoRelacionados

                    from tipoDocumento
                        in tiposDocumentoRelacionados
                            .DefaultIfEmpty()

                    join documento in _context
                        .EbDocumentos
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        on bitacora.DocumentoId
                        equals documento.Id
                        into documentosRelacionados

                    from documento
                        in documentosRelacionados
                            .DefaultIfEmpty()

                    where
                        bitacora.FechaEvento >= inicio &&
                        bitacora.FechaEvento < finExclusivo

                    orderby
                        bitacora.FechaEvento descending,
                        bitacora.Id descending

                    select new
                    {
                        bitacora.FechaEvento,

                        Empresa =
                            empresa.RazonSocial,

                        TipoDocumento =
                            tipoDocumento != null
                                ? tipoDocumento.Nombre
                                : "No disponible",

                        NombreDocumento =
                            !string.IsNullOrWhiteSpace(
                                bitacora.NombreDocumento)
                                ? bitacora.NombreDocumento
                                : documento != null
                                    ? documento.NombreOriginal
                                    : "No disponible",

                        Version =
                            documento != null
                                ? documento.Version
                                : 0,

                        bitacora.Accion,

                        Banco =
                            bitacora.Banco,

                        Usuario =
                            !string.IsNullOrWhiteSpace(
                                bitacora.NombreUsuario)
                                ? bitacora.NombreUsuario
                                : bitacora.UsuarioId,

                        DireccionIp =
                            bitacora.DireccionIp,

                        Resultado =
                            bitacora.Exitoso
                                ? "Exitoso"
                                : "Fallido",

                        bitacora.Detalle
                    }
                )
                .ToListAsync();

            // =================================================
            // CONFIGURAR EPPLUS
            // =================================================
            ExcelPackage.License
                .SetNonCommercialPersonal(
                    "SEI Consulting Group"
                );

            using var paquete =
                new ExcelPackage();

            ExcelWorksheet hoja =
                paquete.Workbook.Worksheets.Add(
                    "Bitácora documental"
                );

            // =================================================
            // TÍTULO
            // =================================================
            hoja.Cells["A1:K1"].Merge = true;

            hoja.Cells["A1"].Value =
                "BITÁCORA DOCUMENTAL DE COMPLIANCE";

            hoja.Cells["A1"].Style.Font.Bold = true;
            hoja.Cells["A1"].Style.Font.Size = 16;

            hoja.Cells["A1"].Style.HorizontalAlignment =
                ExcelHorizontalAlignment.Center;

            hoja.Cells["A1"].Style.VerticalAlignment =
                ExcelVerticalAlignment.Center;

            hoja.Cells["A1"].Style.Font.Color.SetColor(
                Color.White
            );

            hoja.Cells["A1"].Style.Fill.PatternType =
                ExcelFillStyle.Solid;

            hoja.Cells["A1"].Style.Fill.BackgroundColor
                .SetColor(
                    Color.FromArgb(
                        33,
                        22,
                        111
                    )
                );

            hoja.Row(1).Height = 28;

            // =================================================
            // PERIODO
            // =================================================
            hoja.Cells["A2"].Value =
                "Periodo:";

            hoja.Cells["A2"].Style.Font.Bold =
                true;

            hoja.Cells["B2"].Value =
                $"{inicio:dd/MM/yyyy} al {fin:dd/MM/yyyy}";

            hoja.Cells["D2"].Value =
                "Fecha de generación:";

            hoja.Cells["D2"].Style.Font.Bold =
                true;

            hoja.Cells["E2"].Value =
                DateTime.Now;

            hoja.Cells["E2"].Style.Numberformat.Format =
                "dd/mm/yyyy hh:mm AM/PM";

            hoja.Cells["G2"].Value =
                "Total de movimientos:";

            hoja.Cells["G2"].Style.Font.Bold =
                true;

            hoja.Cells["H2"].Value =
                registros.Count;

            // =================================================
            // ENCABEZADOS
            // =================================================
            string[] encabezados =
            {
        "Fecha y hora",
        "Empresa",
        "Tipo de documento",
        "Nombre del archivo",
        "Versión",
        "Acción",
        "Banco",
        "Usuario",
        "Dirección IP",
        "Resultado",
        "Detalle"
    };

            const int filaEncabezados = 4;

            for (
                int columna = 0;
                columna < encabezados.Length;
                columna++
            )
            {
                hoja.Cells[
                    filaEncabezados,
                    columna + 1
                ].Value = encabezados[columna];
            }

            using (
                ExcelRange rangoEncabezados =
                    hoja.Cells[
                        filaEncabezados,
                        1,
                        filaEncabezados,
                        encabezados.Length
                    ]
            )
            {
                rangoEncabezados.Style.Font.Bold =
                    true;

                rangoEncabezados.Style.Font.Color
                    .SetColor(
                        Color.White
                    );

                rangoEncabezados.Style.Fill.PatternType =
                    ExcelFillStyle.Solid;

                rangoEncabezados.Style.Fill
                    .BackgroundColor
                    .SetColor(
                        Color.FromArgb(
                            33,
                            22,
                            111
                        )
                    );

                rangoEncabezados.Style
                    .HorizontalAlignment =
                    ExcelHorizontalAlignment.Center;

                rangoEncabezados.Style
                    .VerticalAlignment =
                    ExcelVerticalAlignment.Center;
            }

            hoja.Row(filaEncabezados).Height =
                24;

            // =================================================
            // DETALLE
            // =================================================
            int filaActual =
                filaEncabezados + 1;

            foreach (var registro in registros)
            {
                hoja.Cells[
                    filaActual,
                    1
                ].Value =
                    registro.FechaEvento;

                hoja.Cells[
                    filaActual,
                    1
                ].Style.Numberformat.Format =
                    "dd/mm/yyyy hh:mm AM/PM";

                hoja.Cells[
                    filaActual,
                    2
                ].Value =
                    registro.Empresa;

                hoja.Cells[
                    filaActual,
                    3
                ].Value =
                    registro.TipoDocumento;

                hoja.Cells[
                    filaActual,
                    4
                ].Value =
                    registro.NombreDocumento;

                hoja.Cells[
                    filaActual,
                    5
                ].Value =
                    registro.Version > 0
                        ? $"V{registro.Version}"
                        : "-";

                hoja.Cells[
                    filaActual,
                    6
                ].Value =
                    registro.Accion;

                hoja.Cells[
                    filaActual,
                    7
                ].Value =
                    string.IsNullOrWhiteSpace(
                        registro.Banco)
                        ? "-"
                        : registro.Banco;

                hoja.Cells[
                    filaActual,
                    8
                ].Value =
                    string.IsNullOrWhiteSpace(
                        registro.Usuario)
                        ? "Usuario desconocido"
                        : registro.Usuario;

                hoja.Cells[
                    filaActual,
                    9
                ].Value =
                    string.IsNullOrWhiteSpace(
                        registro.DireccionIp)
                        ? "-"
                        : registro.DireccionIp;

                hoja.Cells[
                    filaActual,
                    10
                ].Value =
                    registro.Resultado;

                hoja.Cells[
                    filaActual,
                    11
                ].Value =
                    string.IsNullOrWhiteSpace(
                        registro.Detalle)
                        ? "-"
                        : registro.Detalle;

                filaActual++;
            }

            // =================================================
            // FORMATO DEL DETALLE
            // =================================================
            int ultimaFila =
                registros.Count > 0
                    ? filaActual - 1
                    : filaEncabezados;

            if (registros.Count > 0)
            {
                using (
                    ExcelRange rangoDatos =
                        hoja.Cells[
                            filaEncabezados + 1,
                            1,
                            ultimaFila,
                            encabezados.Length
                        ]
                )
                {
                    rangoDatos.Style
                        .VerticalAlignment =
                        ExcelVerticalAlignment.Top;

                    rangoDatos.Style.WrapText =
                        true;

                    rangoDatos.Style.Border.Bottom.Style =
                        ExcelBorderStyle.Hair;
                }

                /*
                 * Resaltar resultado fallido.
                 */
                for (
                    int fila = filaEncabezados + 1;
                    fila <= ultimaFila;
                    fila++
                )
                {
                    string? resultado =
                        hoja.Cells[
                            fila,
                            10
                        ].Text;

                    if (resultado == "Fallido")
                    {
                        hoja.Cells[
                            fila,
                            10
                        ].Style.Font.Color.SetColor(
                            Color.FromArgb(
                                198,
                                40,
                                40
                            )
                        );

                        hoja.Cells[
                            fila,
                            10
                        ].Style.Font.Bold =
                            true;
                    }
                }
            }
            else
            {
                hoja.Cells[
                    filaEncabezados + 1,
                    1,
                    filaEncabezados + 1,
                    encabezados.Length
                ].Merge = true;

                hoja.Cells[
                    filaEncabezados + 1,
                    1
                ].Value =
                    "No se encontraron movimientos en el periodo seleccionado.";

                hoja.Cells[
                    filaEncabezados + 1,
                    1
                ].Style.HorizontalAlignment =
                    ExcelHorizontalAlignment.Center;

                hoja.Cells[
                    filaEncabezados + 1,
                    1
                ].Style.Font.Italic =
                    true;
            }

            // =================================================
            // TABLA Y FILTROS
            // =================================================
            hoja.Cells[
                filaEncabezados,
                1,
                Math.Max(
                    ultimaFila,
                    filaEncabezados + 1
                ),
                encabezados.Length
            ].AutoFilter = true;

            hoja.View.FreezePanes(
                filaEncabezados + 1,
                1
            );

            // =================================================
            // ANCHOS
            // =================================================
            hoja.Column(1).Width = 22;
            hoja.Column(2).Width = 36;
            hoja.Column(3).Width = 30;
            hoja.Column(4).Width = 38;
            hoja.Column(5).Width = 12;
            hoja.Column(6).Width = 20;
            hoja.Column(7).Width = 20;
            hoja.Column(8).Width = 32;
            hoja.Column(9).Width = 20;
            hoja.Column(10).Width = 14;
            hoja.Column(11).Width = 55;

            hoja.Cells[
                1,
                1,
                Math.Max(
                    ultimaFila,
                    filaEncabezados + 1
                ),
                encabezados.Length
            ].Style.VerticalAlignment =
                ExcelVerticalAlignment.Center;

            // =================================================
            // DEVOLVER ARCHIVO
            // =================================================
            byte[] contenidoExcel =
                paquete.GetAsByteArray();

            string nombreArchivo =
                $"Bitacora_Compliance_" +
                $"{inicio:yyyy-MM-dd}_" +
                $"{fin:yyyy-MM-dd}.xlsx";

            return File(
                contenidoExcel,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                nombreArchivo
            );
        }

        // =====================================================
        // DATOS DEL DASHBOARD
        // GET ?handler=Datos
        //     &fechaInicio=2026-07-01
        //     &fechaFin=2026-07-31
        // =====================================================
        public async Task<IActionResult> OnGetDatosAsync(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            if (!fechaInicio.HasValue)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Selecciona la fecha inicial."
                });
            }

            if (!fechaFin.HasValue)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "Selecciona la fecha final."
                });
            }

            DateTime inicio =
                fechaInicio.Value.Date;

            DateTime fin =
                fechaFin.Value.Date;

            if (fin < inicio)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "La fecha final no puede ser anterior a la fecha inicial."
                });
            }

            /*
             * Permitimos un periodo máximo de cinco años
             * para evitar consultas excesivamente grandes.
             */
            if ((fin - inicio).TotalDays > 1826)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "El periodo seleccionado no puede superar cinco años."
                });
            }

            /*
             * La fecha final se vuelve exclusiva para incluir
             * correctamente todo el último día seleccionado.
             *
             * Ejemplo:
             * fechaFin = 31/07/2026
             * finExclusivo = 01/08/2026
             */
            DateTime finExclusivo =
                fin.AddDays(1);

            int totalDias =
                (fin - inicio).Days + 1;

            // =================================================
            // EMPRESAS NUEVAS DEL PERIODO
            // =================================================
            var empresasNuevas = await _context
                .EbEmpresas
                .AsNoTracking()
                .Where(x =>
                    x.FechaCreacion >= inicio &&
                    x.FechaCreacion < finExclusivo)
                .Select(x => new
                {
                    id = x.Id,
                    nombre = x.NombreCorto,
                    razonSocial = x.RazonSocial,
                    rfc = x.Rfc,
                    fechaCreacion =
                        x.FechaCreacion
                })
                .ToListAsync();

            // =================================================
            // TIPOS DOCUMENTALES OBLIGATORIOS
            // =================================================
            var tiposObligatorios = await _context
                .EbTiposDocumento
                .AsNoTracking()
                .Where(x =>
                    !x.Eliminado &&
                    !x.Deshabilitado &&
                    x.EsObligatorio)
                .Select(x => x.Id)
                .ToListAsync();

            int totalTiposObligatorios =
                tiposObligatorios.Count;

            // =================================================
            // AVANCE DOCUMENTAL DE EMPRESAS NUEVAS
            // =================================================
            var idsEmpresasNuevas = empresasNuevas
                .Select(x => x.id)
                .ToList();

            var documentosEmpresas = await _context
                .EbDocumentos
                .AsNoTracking()
                .Where(x =>
                    idsEmpresasNuevas.Contains(
                        x.EmpresaId) &&
                    tiposObligatorios.Contains(
                        x.TipoDocumentoId) &&
                    !x.Eliminado &&
                    x.EsVersionActual)
                .Select(x => new
                {
                    x.EmpresaId,
                    x.TipoDocumentoId
                })
                .Distinct()
                .ToListAsync();

            var avanceEmpresas = empresasNuevas
                .Select(empresa =>
                {
                    int documentosCargados =
                        documentosEmpresas.Count(x =>
                            x.EmpresaId ==
                            empresa.id);

                    decimal porcentaje =
                        totalTiposObligatorios == 0
                            ? 0
                            : Math.Round(
                                documentosCargados *
                                100m /
                                totalTiposObligatorios,
                                2
                            );

                    string empresaEtiqueta =
                        string.IsNullOrWhiteSpace(
                            empresa.rfc)
                            ? empresa.nombre
                            : $"{empresa.nombre} — {empresa.rfc}";

                    return new
                    {
                        empresa =
                            empresaEtiqueta,

                        nombreCorto =
                            empresa.nombre,

                        razonSocial =
                            empresa.razonSocial,

                        rfc =
                            empresa.rfc,

                        requeridos =
                            totalTiposObligatorios,

                        cargados =
                            documentosCargados,

                        porcentaje
                    };
                })
                .OrderByDescending(x =>
                    x.porcentaje)
                .ThenBy(x =>
                    x.empresa)
                .ToList();

            // =================================================
            // DOCUMENTOS CARGADOS EN EL PERIODO
            // =================================================
            var documentosCargadosPeriodo =
                await _context
                    .EbDocumentos
                    .AsNoTracking()
                    .Where(x =>
                        x.FechaCarga >= inicio &&
                        x.FechaCarga <
                        finExclusivo)
                    .Select(x => new
                    {
                        x.Id,
                        x.FechaCarga
                    })
                    .ToListAsync();

            /*
             * Para periodos de hasta 62 días mostramos
             * información diaria.
             *
             * Para periodos mayores agrupamos por mes.
             */
            bool agruparPorMes =
                totalDias > 62;

            object actividadPeriodo;

            if (agruparPorMes)
            {
                DateTime primerMes =
                    new DateTime(
                        inicio.Year,
                        inicio.Month,
                        1
                    );

                DateTime ultimoMes =
                    new DateTime(
                        fin.Year,
                        fin.Month,
                        1
                    );

                int totalMeses =
                    ((ultimoMes.Year -
                      primerMes.Year) * 12) +
                    ultimoMes.Month -
                    primerMes.Month +
                    1;

                actividadPeriodo = Enumerable
                    .Range(0, totalMeses)
                    .Select(indice =>
                    {
                        DateTime mesActual =
                            primerMes
                                .AddMonths(indice);

                        int totalDocumentos =
                            documentosCargadosPeriodo
                                .Count(x =>
                                    x.FechaCarga.Year ==
                                    mesActual.Year &&
                                    x.FechaCarga.Month ==
                                    mesActual.Month);

                        return new
                        {
                            etiqueta =
                                mesActual.ToString(
                                    "MM/yyyy"
                                ),

                            documentosCargados =
                                totalDocumentos
                        };
                    })
                    .ToList();
            }
            else
            {
                actividadPeriodo = Enumerable
                    .Range(0, totalDias)
                    .Select(indice =>
                    {
                        DateTime fechaActual =
                            inicio.AddDays(
                                indice
                            );

                        int totalDocumentos =
                            documentosCargadosPeriodo
                                .Count(x =>
                                    x.FechaCarga.Date ==
                                    fechaActual.Date);

                        return new
                        {
                            etiqueta =
                                fechaActual.ToString(
                                    "dd/MM"
                                ),

                            documentosCargados =
                                totalDocumentos
                        };
                    })
                    .ToList();
            }

            // =================================================
            // BITÁCORA DOCUMENTAL DEL PERIODO
            // =================================================
            var bitacoraPeriodo = _context
                .EbBitacoraDocumentos
                .AsNoTracking()
                .Where(x =>
                    x.FechaEvento >= inicio &&
                    x.FechaEvento < finExclusivo &&
                    x.Exitoso);

            // =================================================
            // TOTAL DE DESCARGAS
            // =================================================
            int documentosDescargados =
                await bitacoraPeriodo
                    .CountAsync(x =>
                        x.Accion ==
                        EbAccionesBitacoraDocumento
                            .Descarga);

            // =================================================
            // TOTAL DE VISUALIZACIONES
            // =================================================
            int visualizaciones =
                await bitacoraPeriodo
                    .CountAsync(x =>
                        x.Accion ==
                        EbAccionesBitacoraDocumento
                            .Visualizacion);

            // =================================================
            // DESCARGAS AGRUPADAS POR USUARIO
            // =================================================
            var descargasUsuarios =
                await bitacoraPeriodo
                    .Where(x =>
                        x.Accion ==
                        EbAccionesBitacoraDocumento
                            .Descarga)
                    .GroupBy(x =>
                        x.NombreUsuario ??
                        x.UsuarioId ??
                        "Usuario desconocido")
                    .Select(grupo => new
                    {
                        usuario =
                            grupo.Key,

                        total =
                            grupo.Count()
                    })
                    .OrderByDescending(x =>
                        x.total)
                    .ThenBy(x =>
                        x.usuario)
                    .ToListAsync();

            // =================================================
            // DOCUMENTOS AGRUPADOS POR BANCO
            // =================================================
            var documentosPorBanco =
                await bitacoraPeriodo
                    .Where(x =>
                        x.Accion ==
                        EbAccionesBitacoraDocumento
                            .Descarga &&
                        x.Banco != null &&
                        x.Banco != "")
                    .GroupBy(x =>
                        x.Banco!)
                    .Select(grupo => new
                    {
                        banco =
                            grupo.Key,

                        total =
                            grupo.Count()
                    })
                    .OrderByDescending(x =>
                        x.total)
                    .ThenBy(x =>
                        x.banco)
                    .ToListAsync();

            // =================================================
            // RESPUESTA DEL DASHBOARD
            // =================================================
            return new JsonResult(new
            {
                success = true,

                periodo = new
                {
                    fechaInicio =
                        inicio,

                    fechaFin =
                        fin,

                    totalDias,

                    agrupacion =
                        agruparPorMes
                            ? "Mensual"
                            : "Diaria"
                },

                resumen = new
                {
                    documentosDescargados,

                    visualizaciones,

                    empresasNuevas =
                        empresasNuevas.Count,

                    documentosCargados =
                        documentosCargadosPeriodo
                            .Count
                },

                graficas = new
                {
                    avanceEmpresas,

                    descargasUsuarios,

                    documentosPorBanco,

                    actividadDiaria =
                        actividadPeriodo
                }
            });
        }
    }


}