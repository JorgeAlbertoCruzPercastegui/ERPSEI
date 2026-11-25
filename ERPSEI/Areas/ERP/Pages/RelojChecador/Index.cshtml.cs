
using ERPSEI.Data;
using ERPSEI.Services.RelojChecador;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERPSEI.Areas.ERP.Pages.RelojChecador;

[Authorize]
public sealed class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly RelojGeocercaService _geocercas;
    private readonly RelojAccesoService _relojAcceso;
    private readonly AsistenciaMovimientosService _asistenciaMovimientosService;
    private readonly RelojEvaluacionAsistenciaService _evaluacionAsistenciaService;

    public IndexModel(
        ApplicationDbContext db,
        RelojGeocercaService geocercas,
        RelojAccesoService relojAcceso,
        AsistenciaMovimientosService asistenciaMovimientosService,
        RelojEvaluacionAsistenciaService evaluacionAsistenciaService)
    {
        _db = db;
        _geocercas = geocercas;
        _relojAcceso = relojAcceso;
        _asistenciaMovimientosService = asistenciaMovimientosService;
        _evaluacionAsistenciaService = evaluacionAsistenciaService;
    }


    public async Task<IActionResult> OnGetMiEvaluacionPruebaAsync(
        DateTime? fecha)
    {
        try
        {
            var userId = User.FindFirstValue(
                System.Security.Claims.ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new UnauthorizedResult();
            }

            var empleado = await _db.Empleados
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.Deshabilitado == 0)
                .Select(x => new { x.Id })
                .SingleOrDefaultAsync();

            if (empleado == null)
            {
                return new BadRequestObjectResult(new
                {
                    tieneError = true,
                    mensaje = "No se encontró un colaborador activo."
                });
            }

            var zonaMexico = TimeZoneInfo
                .FindSystemTimeZoneById(
                    "America/Mexico_City");

            var hoyMexico = TimeZoneInfo
                .ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    zonaMexico)
                .Date;

            var fechaEvaluar = fecha?.Date ?? hoyMexico;

            var resultado = await _evaluacionAsistenciaService
                .EvaluarDiaAsync(
                    empleado.Id,
                    fechaEvaluar);

            return new JsonResult(new
            {
                tieneError = false,
                evaluacion = resultado
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            return new JsonResult(new
            {
                tieneError = true,
                mensaje = "No fue posible evaluar la asistencia."
            })
            {
                StatusCode = 500
            };
        }
    }


    public async Task<IActionResult> OnGetMisMovimientosPruebaAsync(
        DateTime? fechaInicio,
        DateTime? fechaFin)
    {
        try
        {
            // =====================================================
            // 1. OBTENER USUARIO AUTENTICADO
            // =====================================================

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new UnauthorizedObjectResult(new
                {
                    tieneError = true,
                    mensaje = "No se encontró un usuario autenticado."
                });
            }

            // =====================================================
            // 2. IDENTIFICAR EMPLEADO
            // =====================================================

            var empleado = await _db.Empleados
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.Deshabilitado == 0)
                .Select(x => new
                {
                    x.Id
                })
                .SingleOrDefaultAsync();

            if (empleado == null)
            {
                return new JsonResult(new
                {
                    tieneError = true,
                    mensaje = "El usuario no tiene un empleado activo relacionado."
                });
            }

            // =====================================================
            // 3. VALIDAR FECHAS
            // =====================================================

            var zonaMexico = TimeZoneInfo
                .FindSystemTimeZoneById(
                    "America/Mexico_City");

            var fechaActualMexico = TimeZoneInfo
                .ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    zonaMexico)
                .Date;

            var inicio = fechaInicio?.Date ??
                fechaActualMexico;

            var fin = fechaFin?.Date ?? inicio;

            if (fin < inicio)
            {
                return new BadRequestObjectResult(new
                {
                    tieneError = true,
                    mensaje = "La fecha final no puede ser menor que la inicial."
                });
            }

            if ((fin - inicio).TotalDays > 365)
            {
                return new BadRequestObjectResult(new
                {
                    tieneError = true,
                    mensaje = "El rango máximo permitido es de 366 días."
                });
            }

            // =====================================================
            // 4. CONSULTAR MOVIMIENTOS
            // =====================================================

            var movimientos = await _asistenciaMovimientosService
                .ObtenerMovimientosAsync(
                    empleado.Id,
                    inicio,
                    fin);

            // =====================================================
            // 5. RESPUESTA
            // =====================================================

            return new JsonResult(new
            {
                tieneError = false,
                empleadoId = empleado.Id,
                fechaInicio = inicio.ToString("yyyy-MM-dd"),
                fechaFin = fin.ToString("yyyy-MM-dd"),
                total = movimientos.Count,
                movimientos
            });
        }
        catch (ArgumentException ex)
        {
            return new BadRequestObjectResult(new
            {
                tieneError = true,
                mensaje = ex.Message
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);

            return new JsonResult(new
            {
                tieneError = true,
                mensaje = "No fue posible consultar los movimientos de asistencia."
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }
    }



    // =====================================================
    // SEGURIDAD
    // =====================================================

    private static IActionResult AccesoDenegado()
    {
        return new JsonResult(new
        {
            success = false,
            autorizado = false,
            codigo = "RELOJ_NO_AUTORIZADO",
            message = "No tienes una autorización Home Office vigente para utilizar el Reloj Checador."
        })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    private string? ObtenerUserId()
    {
        return User.FindFirstValue(
            ClaimTypes.NameIdentifier);
    }

    private static DateTime ObtenerFechaMexico(
        DateTime fechaUtc)
    {
        var zona = TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

        return TimeZoneInfo.ConvertTimeFromUtc(
            fechaUtc,
            zona).Date;
    }

    private static DateTime NormalizarUtc(
        DateTime fecha)
    {
        return fecha.Kind switch
        {
            DateTimeKind.Utc => fecha,
            DateTimeKind.Local => fecha.ToUniversalTime(),
            _ => DateTime.SpecifyKind(
                fecha,
                DateTimeKind.Utc)
        };
    }

    // =====================================================
    // PÁGINA PRINCIPAL
    // GET /ERP/RelojChecador
    // =====================================================

    public async Task<IActionResult> OnGetAsync(
        CancellationToken ct)
    {
        if (!await _relojAcceso.TieneAccesoAsync(
            User, ct))
        {
            return Forbid();
        }

        return Page();
    }

    // =====================================================
    // MODELOS DE ENTRADA
    // =====================================================

    public sealed class ValidarUbicacionInput
    {
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double PrecisionMetros { get; set; }
        public DateTime CapturadaUtc { get; set; }
    }

    public sealed class RegistrarMovimientoInput
    {
        public string TipoMovimiento { get; set; }
            = string.Empty;

        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double PrecisionMetros { get; set; }

        public DateTime CapturadaUtc { get; set; }

        public Guid IdempotencyKey { get; set; }

        public IFormFile? Fotografia { get; set; }
    }

    // =====================================================
    // CONSULTAR ESTADO DE JORNADA
    // GET ?handler=Estado
    // =====================================================

    public async Task<IActionResult> OnGetEstadoAsync(
        CancellationToken ct)
    {
        var userId = ObtenerUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        // Protección adicional del endpoint.
        if (!await _relojAcceso.TieneAccesoAsync(
            userId, ct))
        {
            return AccesoDenegado();
        }

        // Obtener exclusivamente el empleado
        // vinculado al usuario autenticado.
        var empleado = await _db.Empleados
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .Select(e => new
            {
                e.Id,
                e.Nombre,
                e.ApellidoPaterno,
                e.ApellidoMaterno,

                OficinaId = e.Oficina == null
                    ? (int?)null
                    : e.Oficina.Id,

                OficinaNombre = e.Oficina == null
                    ? null
                    : e.Oficina.Nombre
            })
            .FirstOrDefaultAsync(ct);

        if (empleado == null)
        {
            return new JsonResult(new
            {
                success = false,
                codigo = "EMPLEADO_NO_VINCULADO",
                message = "Tu cuenta no tiene un empleado vinculado."
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        var ahoraUtc = DateTime.UtcNow;

        var fechaLocal = ObtenerFechaMexico(ahoraUtc);
        var diaSiguiente = fechaLocal.AddDays(1);

        // Obtener autorización HO vigente.
        var autorizacionHO = await _db
            .RC_AutorizacionesModalidades
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.Modalidad == "Remoto" &&
                x.Estado == "Aprobada" &&
                x.FechaInicio < diaSiguiente &&
                x.FechaFin >= fechaLocal)
            .OrderByDescending(x => x.FechaInicio)
            .Select(x => new
            {
                x.Id,
                x.Modalidad,
                x.FechaInicio,
                x.FechaFin
            })
            .FirstOrDefaultAsync(ct);

        // Consultar la jornada abierta.

        // cerrada durante el día actual de México.

        // =====================================================
        // CONSULTAR JORNADA ABIERTA O CERRADA DEL DÍA
        // Zona horaria oficial de Ciudad de México
        // =====================================================

        var zonaMexico = TimeZoneInfo.FindSystemTimeZoneById(
            "America/Mexico_City");

        // Convertir el inicio y fin del día local a UTC.
        var inicioDiaUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                fechaLocal,
                DateTimeKind.Unspecified),
            zonaMexico);

        var finDiaUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(
                diaSiguiente,
                DateTimeKind.Unspecified),
            zonaMexico);


        var jornada = await _db.RC_Jornadas
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                (
                    x.Estado == "Abierta" ||
                    (
                        x.Estado == "Cerrada" &&
                        x.InicioUtc >= inicioDiaUtc &&
                        x.InicioUtc < finDiaUtc
                    )
                ))
            .OrderBy(x => x.Estado == "Abierta" ? 0 : 1)
            .ThenByDescending(x => x.InicioUtc)
            .Select(x => new
            {
                x.Id,
                x.Estado,
                x.InicioUtc,
                x.FinUtc,
                x.MinutosObjetivo,
                x.MinutosEfectivos,
                x.RequiereRevision,
                x.MotivoRevision,
                x.Modalidad
            })
            .FirstOrDefaultAsync(ct);


        // Obtener movimientos asociados.
        var movimientos = jornada == null
            ? new List<object>()
            : (await _db.RC_Checadas
                .AsNoTracking()
                .Where(x =>
                    x.JornadaId == jornada.Id &&
                    x.EstadoValidacion == "Validada")
                .OrderBy(x => x.FechaHoraUtc)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.TipoMovimiento,
                    x.FechaHoraUtc,
                    x.Origen
                })
                .ToListAsync(ct))
                .Select(x => (object)new
                {
                    tipo = x.TipoMovimiento,

                    // SQL Server devuelve DateTime
                    // sin Kind. Identificamos UTC
                    // antes de serializar a JSON.
                    fechaHoraUtc = DateTime.SpecifyKind(
                        x.FechaHoraUtc,
                        DateTimeKind.Utc),

                    origen = x.Origen
                })
                .ToList();

        return new JsonResult(new
        {
            success = true,
            servidorUtc = ahoraUtc,

            colaborador = new
            {
                nombre = string.Join(" ", new[]
                {
                    empleado.Nombre,
                    empleado.ApellidoPaterno,
                    empleado.ApellidoMaterno
                }.Where(x =>
                    !string.IsNullOrWhiteSpace(x))),

                oficinaId = empleado.OficinaId,

                oficina = empleado.OficinaNombre
                    ?? "Sin oficina asignada",

                modalidadActual = autorizacionHO != null
                    ? "Remoto"
                    : "Presencial",

                homeOfficeAutorizado =
                    autorizacionHO != null
            },

            autorizacion = autorizacionHO == null
                ? null
                : new
                {
                    modalidad = autorizacionHO.Modalidad,
                    fechaInicio = autorizacionHO.FechaInicio,
                    fechaFin = autorizacionHO.FechaFin
                },


            jornada = jornada == null
                ? null
                : new
                {
                    id = jornada.Id,
                    estado = jornada.Estado,

                    inicioUtc = DateTime.SpecifyKind(
                        jornada.InicioUtc,
                        DateTimeKind.Utc),

                    finUtc = jornada.FinUtc.HasValue
                        ? DateTime.SpecifyKind(
                            jornada.FinUtc.Value,
                            DateTimeKind.Utc)
                        : (DateTime?)null,

                    objetivoMinutos =
                        jornada.MinutosObjetivo,

                    minutosEfectivos =
                        jornada.MinutosEfectivos,

                    requiereRevision =
                        jornada.RequiereRevision,

                    motivoRevision =
                        jornada.MotivoRevision,

                    modalidad = jornada.Modalidad
                },

            movimientos

        });
    }

    // =====================================================
    // VALIDAR GEOLOCALIZACIÓN
    // POST ?handler=ValidarUbicacion
    // =====================================================

    [ValidateAntiForgeryToken]
    public async Task<IActionResult>
        OnPostValidarUbicacionAsync(
            [FromForm] ValidarUbicacionInput input,
            CancellationToken ct)
    {
        var userId = ObtenerUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        // No permitir GPS si el usuario no tiene
        // autorización vigente para el reloj.
        if (!await _relojAcceso.TieneAccesoAsync(
            userId, ct))
        {
            return AccesoDenegado();
        }

        var ahoraUtc = DateTime.UtcNow;

        // Validación de coordenadas.
        if (!double.IsFinite(input.Latitud) ||
            !double.IsFinite(input.Longitud) ||
            !double.IsFinite(input.PrecisionMetros) ||
            input.Latitud < -90 ||
            input.Latitud > 90 ||
            input.Longitud < -180 ||
            input.Longitud > 180 ||
            input.PrecisionMetros <= 0)
        {
            return BadRequest(new
            {
                success = false,
                autorizado = false,
                codigo = "UBICACION_INVALIDA",
                message = "No se recibió una ubicación GPS válida."
            });
        }

        // Verificar que la lectura GPS sea reciente.
        var capturaUtc = NormalizarUtc(
            input.CapturadaUtc);

        if (input.CapturadaUtc == default ||
            capturaUtc > ahoraUtc.AddSeconds(5) ||
            ahoraUtc - capturaUtc >
                TimeSpan.FromSeconds(15))
        {
            return BadRequest(new
            {
                success = false,
                autorizado = false,
                codigo = "UBICACION_CADUCADA",
                message = "Actualiza tu ubicación e inténtalo nuevamente."
            });
        }

        // Obtener oficina desde la base de datos.
        var empleado = await _db.Empleados
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                x.Id,

                OficinaId = x.Oficina == null
                    ? (int?)null
                    : x.Oficina.Id
            })
            .FirstOrDefaultAsync(ct);

        if (empleado == null)
        {
            return new JsonResult(new
            {
                success = false,
                autorizado = false,
                codigo = "EMPLEADO_NO_VINCULADO",
                message = "Tu cuenta no tiene un empleado asociado."
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        var fechaLocal = ObtenerFechaMexico(
            ahoraUtc);

        var diaSiguiente = fechaLocal.AddDays(1);

        // =================================================
        // HOME OFFICE APROBADO
        // =================================================

        var permisoHO = await _db
            .RC_AutorizacionesModalidades
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.Estado == "Aprobada" &&
                x.Modalidad == "Remoto" &&
                x.FechaInicio < diaSiguiente &&
                x.FechaFin >= fechaLocal)
            .OrderByDescending(x => x.FechaInicio)
            .FirstOrDefaultAsync(ct);

        if (permisoHO != null)
        {
            // Límite provisional para las pruebas.
            // No representa validación de domicilio.
            const double precisionRemotaMaxima = 1000;

            if (input.PrecisionMetros >
                precisionRemotaMaxima)
            {
                return new JsonResult(new
                {
                    success = true,
                    autorizado = false,
                    modalidad = "Remoto",
                    codigo = "PRECISION_REMOTA_INSUFICIENTE",
                    message = "Home Office autorizado, pero la ubicación es demasiado imprecisa.",
                    precisionMetros = input.PrecisionMetros
                });
            }

            return new JsonResult(new
            {
                success = true,
                autorizado = true,
                modalidad = "Remoto",
                codigo = "HO_AUTORIZADO",
                message = "Home Office autorizado. Ubicación recibida correctamente; no necesitas estar cerca de Polanco.",
                precisionMetros = input.PrecisionMetros,
                distanciaMetros = (double?)null,
                radioPermitidoMetros = (int?)null
            });
        }

        // =================================================
        // PRESENCIAL
        // =================================================
        // El flujo se conserva para una futura modalidad
        // presencial habilitada. Con la política actual
        // de acceso, sin HO vigente el endpoint devuelve
        // 403 antes de llegar a este punto.
        if (!empleado.OficinaId.HasValue)
        {
            return BadRequest(new
            {
                success = false,
                autorizado = false,
                codigo = "OFICINA_NO_ASIGNADA",
                message = "No tienes una oficina asignada."
            });
        }

        var resultado = await _geocercas.ValidarAsync(
            empleado.OficinaId.Value,
            input.Latitud,
            input.Longitud,
            input.PrecisionMetros,
            ct);

        return new JsonResult(new
        {
            success = true,
            autorizado = resultado.Autorizado,
            modalidad = "Presencial",
            codigo = resultado.Codigo,
            message = resultado.Mensaje,
            distanciaMetros = resultado.DistanciaMetros,
            precisionMetros = resultado.PrecisionMetros,
            radioPermitidoMetros =
                resultado.RadioPermitidoMetros
        });
    }

    // =====================================================
    // REGISTRAR MOVIMIENTO
    // POST ?handler=RegistrarMovimiento
    // =====================================================

    [ValidateAntiForgeryToken]
    public async Task<IActionResult>
        OnPostRegistrarMovimientoAsync(
            [FromForm] RegistrarMovimientoInput input,
            [FromServices] RelojChecadasService servicio,
            CancellationToken ct)
    {
        var userId = ObtenerUserId();

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        // Comprobar nuevamente la autorización.
        // No depender únicamente de que el botón
        // sea visible desde el layout.
        if (!await _relojAcceso.TieneAccesoAsync(
            userId, ct))
        {
            return AccesoDenegado();
        }

        if (input.Fotografia == null)
        {
            return BadRequest(new
            {
                success = false,
                codigo = "FOTO_REQUERIDA",
                message = "Debes capturar una fotografía para registrar el movimiento."
            });
        }

        if (input.IdempotencyKey == Guid.Empty)
        {
            return BadRequest(new
            {
                success = false,
                codigo = "SOLICITUD_INVALIDA",
                message = "No se recibió el identificador de la solicitud."
            });
        }

        var solicitud = new RelojRegistroSolicitud(
            UserId: userId,
            TipoMovimiento: input.TipoMovimiento,
            Latitud: input.Latitud,
            Longitud: input.Longitud,
            PrecisionMetros: input.PrecisionMetros,
            CapturadaUtc: input.CapturadaUtc,
            Fotografia: input.Fotografia,
            IdempotencyKey: input.IdempotencyKey
        );

        // El servicio vuelve a validar GPS,
        // secuencia, modalidad y fotografía.
        var resultado = await servicio.RegistrarAsync(
            solicitud,
            ct);

        return new JsonResult(new
        {
            success = resultado.Success,
            codigo = resultado.Codigo,
            message = resultado.Mensaje,
            jornadaId = resultado.JornadaId,
            checadaId = resultado.ChecadaId,
            movimiento = resultado.Movimiento,
            modalidad = resultado.Modalidad,
            fechaHoraUtc = resultado.FechaHoraUtc,
            minutosEfectivos =
                resultado.MinutosEfectivos,
            requiereRevision =
                resultado.RequiereRevision
        })
        {
            StatusCode = resultado.Success
                ? StatusCodes.Status200OK
                : StatusCodes.Status400BadRequest
        };
    }
}
