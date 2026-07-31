using ERPSEI.Data;
using ERPSEI.Data.Entities.ExpedientesBancarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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