
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ERPSEI.Areas.ERP.Pages.BioTime.Analytics;

public sealed partial class IndexModel
{
    // =========================================================
    // DIAGNÓSTICO ADMINISTRATIVO DE MOVIMIENTOS
    // BIOTIME + INTRANET
    // =========================================================
    //
    // GET:
    // /ERP/BioTime/Analytics
    //   ?handler=MovimientosDiagnostico
    //   &empleadoId=35
    //   &fechaInicio=2026-11-24
    //   &fechaFin=2026-11-25
    //
    // Seguridad:
    // - Solo roles administrativos autorizados.
    // - No permite consultas anónimas.
    // - No modifica checadas ni jornadas.
    // - No clasifica movimientos.
    // - No genera incidencias laborales.
    // - Limita el intervalo de consulta.
    //
    // =========================================================

    public async Task<IActionResult>
        OnGetMovimientosDiagnosticoAsync(
            int empleadoId,
            DateTime? fechaInicio,
            DateTime? fechaFin,
            CancellationToken ct)
    {
        // -----------------------------------------------------
        // 1. AUTORIZACIÓN
        // -----------------------------------------------------

        if (User.Identity?.IsAuthenticated != true)
        {
            return new UnauthorizedObjectResult(new
            {
                success = false,
                message = "Se requiere iniciar sesión."
            });
        }

        bool autorizado =
            User.IsInRole("Master") ||
            User.IsInRole("Administrador") ||
            User.IsInRole("Administrador TH");

        if (!autorizado)
        {
            return Forbid();
        }

        // -----------------------------------------------------
        // 2. VALIDAR EMPLEADO
        // -----------------------------------------------------

        if (empleadoId <= 0)
        {
            return BadRequest(new
            {
                success = false,
                message = "El identificador del empleado es inválido."
            });
        }

        // -----------------------------------------------------
        // 3. VALIDAR PERIODO
        // -----------------------------------------------------

        if (!fechaInicio.HasValue || !fechaFin.HasValue)
        {
            return BadRequest(new
            {
                success = false,
                message = "Debes proporcionar fecha inicial y final."
            });
        }

        DateTime inicio = fechaInicio.Value.Date;
        DateTime fin = fechaFin.Value.Date;

        if (fin < inicio)
        {
            return BadRequest(new
            {
                success = false,
                message = "La fecha final no puede ser menor a la inicial."
            });
        }

        // Máximo 31 días por consulta de diagnóstico.
        if ((fin - inicio).TotalDays > 30)
        {
            return BadRequest(new
            {
                success = false,
                message = "El diagnóstico permite un máximo de 31 días."
            });
        }

        var zonaMexico =
            TimeZoneInfo.FindSystemTimeZoneById(
                "America/Mexico_City");

        var hoyMexico =
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow,
                zonaMexico).Date;

        if (fin > hoyMexico)
        {
            return BadRequest(new
            {
                success = false,
                message = "No se permiten fechas futuras."
            });
        }

        // -----------------------------------------------------
        // 4. OBTENER SERVICIO REGISTRADO
        // -----------------------------------------------------
        //
        // IndexModel ya tiene un constructor existente.
        // Resolvemos este servicio desde el scope actual
        // para no modificar ese constructor.
        //
        // -----------------------------------------------------

        var movimientosService =
            HttpContext.RequestServices
                .GetRequiredService<AsistenciaMovimientosService>();

        // -----------------------------------------------------
        // 5. CONSULTAR MOVIMIENTOS NORMALIZADOS
        // -----------------------------------------------------

        var movimientos =
            await movimientosService.ObtenerMovimientosAsync(
                empleadoId,
                inicio,
                fin,
                ct);

        // -----------------------------------------------------
        // 6. PREPARAR RESULTADO DE DIAGNÓSTICO
        // -----------------------------------------------------

        var registros = movimientos
            .Select(m => new
            {
                idMovimiento = m.IdMovimiento,
                empleadoId = m.EmpleadoId,

                fuente = m.Fuente,
                origen = m.Origen,

                tipoMovimiento = m.TipoMovimiento,

                fechaHoraUtc =
                    m.FechaHoraUtc.ToString(
                        "yyyy-MM-ddTHH:mm:ss"),

                fechaHoraMexico =
                    m.FechaHoraMexico.ToString(
                        "yyyy-MM-ddTHH:mm:ss"),

                estadoValidacion = m.EstadoValidacion,

                jornadaId = m.JornadaId,
                checadaId = m.ChecadaId,

                tieneUbicacion = m.TieneUbicacion,

                esMovimientoClasificado =
                    m.EsMovimientoClasificado
            })
            .ToList();

        // -----------------------------------------------------
        // 7. AGRUPACIÓN POR FUENTE
        // -----------------------------------------------------

        var porFuente = movimientos
            .GroupBy(x => x.Fuente)
            .Select(g => new
            {
                fuente = g.Key,
                total = g.Count()
            })
            .OrderBy(x => x.fuente)
            .ToList();

        var pendientesClasificacion =
            movimientos.Count(
                x => !x.EsMovimientoClasificado);

        // -----------------------------------------------------
        // 8. RESPUESTA JSON
        // -----------------------------------------------------

        return new JsonResult(new
        {
            success = true,

            empleadoId,

            periodo = new
            {
                fechaInicio = inicio.ToString("yyyy-MM-dd"),
                fechaFin = fin.ToString("yyyy-MM-dd")
            },

            resumen = new
            {
                totalMovimientos = movimientos.Count,

                movimientosBioTime =
                    movimientos.Count(
                        x => x.Fuente.Equals(
                            "BIOTIME",
                            StringComparison.OrdinalIgnoreCase)),

                movimientosIntranet =
                    movimientos.Count(
                        x => x.Fuente.Equals(
                            "INTRANET",
                            StringComparison.OrdinalIgnoreCase)),

                pendientesClasificacion,

                porFuente
            },

            registros
        });
    }
}