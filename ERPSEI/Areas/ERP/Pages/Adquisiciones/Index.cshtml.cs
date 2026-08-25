using ERPSEI.Data;
using ERPSEI.Data.Entities.Adquisiciones;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ERPSEI.Areas.ERP.Pages.Adquisiciones
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _environment;


        public IndexModel(
            ApplicationDbContext context,
            AppUserManager userManager,
            ILogger<IndexModel> logger,
            IWebHostEnvironment environment)
        {
            _context =
                context;

            _userManager =
                userManager;

            _logger =
                logger;

            _environment =
                environment;
        }


        // =========================================================
        // INPUTS
        // =========================================================

        [BindProperty]
        public NuevaSolicitudInput Input
        {
            get;
            set;
        } = new();


        [BindProperty]
        public List<IFormFile> ArchivosSolicitud
        {
            get;
            set;
        } = new();


        [BindProperty]
        public int? SolicitudEditarId
        {
            get;
            set;
        }


        // =========================================================
        // USUARIO
        // =========================================================

        public AppUser? UsuarioActual
        {
            get;
            private set;
        }


        public Empleado? EmpleadoActual
        {
            get;
            private set;
        }


        public string NombreSolicitante
        {
            get;
            private set;
        } = string.Empty;


        public string NombreArea
        {
            get;
            private set;
        } = string.Empty;


        public string NombreJefe
        {
            get;
            private set;
        } = string.Empty;


        public bool TieneJefeConfigurado
        {
            get;
            private set;
        }


        // =========================================================
        // CATÁLOGOS
        // =========================================================

        public List<SelectListItem> Areas
        {
            get;
            private set;
        } = new();


        // =========================================================
        // SOLICITUDES
        // =========================================================

        public List<AdqSolicitud> Solicitudes
        {
            get;
            private set;
        } = new();


        // =========================================================
        // KPIs
        // =========================================================

        public int TotalSolicitudes
        {
            get;
            private set;
        }


        public int TotalBorradores
        {
            get;
            private set;
        }


        public int TotalPendientes
        {
            get;
            private set;
        }


        public int TotalEnProceso
        {
            get;
            private set;
        }


        public int TotalFinalizadas
        {
            get;
            private set;
        }


        // =========================================================
        // INPUT SOLICITUD
        // =========================================================

        public class NuevaSolicitudInput
        {
            [Required(
                ErrorMessage =
                    "El título de la solicitud es obligatorio.")]
            [StringLength(
                250,
                ErrorMessage =
                    "El título no puede superar los 250 caracteres.")]
            public string Titulo
            {
                get;
                set;
            } = string.Empty;


            [Range(
                1,
                int.MaxValue,
                ErrorMessage =
                    "Debes seleccionar un área.")]
            public int AreaId
            {
                get;
                set;
            }


            [Required(
                ErrorMessage =
                    "La descripción es obligatoria.")]
            [StringLength(
                5000,
                ErrorMessage =
                    "La descripción no puede superar los 5000 caracteres.")]
            public string Descripcion
            {
                get;
                set;
            } = string.Empty;


            [Required(
                ErrorMessage =
                    "La justificación es obligatoria.")]
            [StringLength(
                5000,
                ErrorMessage =
                    "La justificación no puede superar los 5000 caracteres.")]
            public string Justificacion
            {
                get;
                set;
            } = string.Empty;


            public List<NuevaSolicitudDetalleInput> Detalles
            {
                get;
                set;
            } = new();
        }


        public class NuevaSolicitudDetalleInput
        {
            [Required(
                ErrorMessage =
                    "El producto o servicio es obligatorio.")]
            [StringLength(500)]
            public string ProductoServicio
            {
                get;
                set;
            } = string.Empty;


            [Range(
                0.0001,
                double.MaxValue,
                ErrorMessage =
                    "La cantidad debe ser mayor a cero.")]
            public decimal Cantidad
            {
                get;
                set;
            }


            [Required(
                ErrorMessage =
                    "La unidad es obligatoria.")]
            [StringLength(100)]
            public string Unidad
            {
                get;
                set;
            } = string.Empty;


            [StringLength(2000)]
            public string? Descripcion
            {
                get;
                set;
            }
        }


        // =========================================================
        // GET
        // =========================================================

        public async Task<IActionResult>
            OnGetAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            await CargarPantallaAsync(
                usuarioActual
            );


            return Page();
        }


        // =========================================================
        // GUARDAR BORRADOR
        // =========================================================

        public async Task<IActionResult>
            OnPostGuardarBorradorAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            NormalizarInput();

            ValidarDetalles();

            ValidarArchivos();


            if (!ModelState.IsValid)
            {
                await CargarPantallaAsync(
                    usuarioActual
                );

                return Page();
            }


            try
            {
                await CrearSolicitudAsync(
                    usuarioActual,
                    enviar: false
                );


                TempData["MensajeExito"] =
                    "La solicitud se guardó como borrador correctamente.";


                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al guardar borrador de Adquisiciones."
                );


                ModelState.AddModelError(
                    string.Empty,
                    ex is InvalidOperationException
                        ? ex.Message
                        : "No fue posible guardar la solicitud."
                );


                await CargarPantallaAsync(
                    usuarioActual
                );


                return Page();
            }
        }


        // =========================================================
        // ENVIAR SOLICITUD
        // =========================================================

        public async Task<IActionResult>
            OnPostEnviarSolicitudAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            NormalizarInput();

            ValidarDetalles();

            ValidarArchivos();


            Empleado? empleado =
                await ObtenerEmpleadoActualAsync(
                    usuarioActual
                );


            if (empleado == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Tu usuario no se encuentra relacionado con un empleado."
                );
            }
            else
            {
                Empleado? jefe =
                    await ObtenerJefeAsync(
                        empleado
                    );


                if (jefe == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "No se encontró un jefe configurado para tu empleado."
                    );
                }
                else if (
                    string.IsNullOrWhiteSpace(
                        jefe.UserId
                    )
                )
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "El jefe configurado no tiene un usuario de Intranet relacionado."
                    );
                }
            }


            if (!ModelState.IsValid)
            {
                await CargarPantallaAsync(
                    usuarioActual
                );

                return Page();
            }


            try
            {
                await CrearSolicitudAsync(
                    usuarioActual,
                    enviar: true
                );


                TempData["MensajeExito"] =
                    "La solicitud se envió correctamente para aprobación.";


                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al enviar solicitud de Adquisiciones."
                );


                ModelState.AddModelError(
                    string.Empty,
                    ex is InvalidOperationException
                        ? ex.Message
                        : "No fue posible enviar la solicitud."
                );


                await CargarPantallaAsync(
                    usuarioActual
                );


                return Page();
            }
        }


        // =========================================================
        // EDITAR SOLICITUD
        // =========================================================

        public async Task<IActionResult>
            OnPostEditarSolicitudAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            if (!SolicitudEditarId.HasValue)
            {
                TempData["MensajeError"] =
                    "No se identificó la solicitud a modificar.";

                return RedirectToPage();
            }


            NormalizarInput();

            ValidarDetalles();

            ValidarArchivos();


            if (!ModelState.IsValid)
            {
                await CargarPantallaAsync(
                    usuarioActual
                );

                return Page();
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .Include(
                        x => x.Detalles
                    )
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                SolicitudEditarId.Value &&
                            x.UsuarioSolicitanteId ==
                                usuarioActual.Id &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return NotFound();
            }


            if (solicitud.EstatusId != 1)
            {
                TempData["MensajeError"] =
                    "Solamente se pueden editar solicitudes en borrador.";

                return RedirectToPage();
            }


            DateTime ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                solicitud.Titulo =
                    Input.Titulo;

                solicitud.AreaId =
                    Input.AreaId;

                solicitud.Descripcion =
                    Input.Descripcion;

                solicitud.Justificacion =
                    Input.Justificacion;

                solicitud.FechaModificacion =
                    ahora;


                /*
                 * Los detalles anteriores se conservan
                 * como eliminados lógicamente.
                 */
                foreach (
                    AdqSolicitudDetalle detalle
                    in solicitud.Detalles.Where(
                        x =>
                            !x.Eliminado
                    ))
                {
                    detalle.Eliminado =
                        true;
                }


                int orden =
                    1;


                foreach (
                    NuevaSolicitudDetalleInput item
                    in Input.Detalles)
                {
                    solicitud.Detalles.Add(
                        new AdqSolicitudDetalle
                        {
                            ProductoServicio =
                                item.ProductoServicio,

                            Cantidad =
                                item.Cantidad,

                            Unidad =
                                item.Unidad,

                            Descripcion =
                                item.Descripcion,

                            Orden =
                                orden++,

                            Eliminado =
                                false
                        }
                    );
                }


                /*
                 * Los archivos nuevos se agregan
                 * a los previamente existentes.
                 */
                await GuardarAdjuntosAsync(
                    solicitud,
                    usuarioActual,
                    ahora
                );


                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            "SOLICITUD_EDITADA",

                        Descripcion =
                            "El usuario modificó la solicitud en borrador.",

                        EstatusAnteriorId =
                            solicitud.EstatusId,

                        EstatusNuevoId =
                            solicitud.EstatusId,

                        FechaEvento =
                            ahora,

                        DireccionIp =
                            ObtenerDireccionIp()
                    }
                );


                await _context
                    .SaveChangesAsync();


                await transaccion
                    .CommitAsync();


                TempData["MensajeExito"] =
                    "La solicitud se actualizó correctamente.";


                return RedirectToPage();
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al editar solicitud {SolicitudId}.",
                    SolicitudEditarId
                );


                ModelState.AddModelError(
                    string.Empty,
                    ex is InvalidOperationException
                        ? ex.Message
                        : "No fue posible actualizar la solicitud."
                );


                await CargarPantallaAsync(
                    usuarioActual
                );


                return Page();
            }
        }


        // =========================================================
        // CREAR SOLICITUD
        // =========================================================

        private async Task CrearSolicitudAsync(
            AppUser usuarioActual,
            bool enviar)
        {
            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync(
                        IsolationLevel.Serializable
                    );


            try
            {
                DateTime ahora =
                    DateTime.Now;


                Empleado? empleado =
                    await ObtenerEmpleadoActualAsync(
                        usuarioActual
                    );


                int estatusInicialId =
                    enviar
                        ? 2
                        : 1;


                string folioTemporal =
                    "TMP-" +
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(
                            0,
                            20
                        );


                AdqSolicitud solicitud =
                    new()
                    {
                        Folio =
                            folioTemporal,

                        Titulo =
                            Input.Titulo,

                        FechaSolicitud =
                            ahora,

                        UsuarioSolicitanteId =
                            usuarioActual.Id,

                        EmpleadoSolicitanteId =
                            empleado?.Id,

                        AreaId =
                            Input.AreaId,

                        Descripcion =
                            Input.Descripcion,

                        Justificacion =
                            Input.Justificacion,

                        EstatusId =
                            estatusInicialId,

                        FechaCreacion =
                            ahora,

                        FechaEnvio =
                            enviar
                                ? ahora
                                : null,

                        Eliminado =
                            false
                    };


                int orden =
                    1;


                foreach (
                    NuevaSolicitudDetalleInput item
                    in Input.Detalles)
                {
                    solicitud.Detalles.Add(
                        new AdqSolicitudDetalle
                        {
                            ProductoServicio =
                                item.ProductoServicio,

                            Cantidad =
                                item.Cantidad,

                            Unidad =
                                item.Unidad,

                            Descripcion =
                                item.Descripcion,

                            Orden =
                                orden++,

                            Eliminado =
                                false
                        }
                    );
                }


                _context.AdqSolicitudes.Add(
                    solicitud
                );


                /*
                 * Primero obtenemos el ID identity.
                 */
                await _context
                    .SaveChangesAsync();


                solicitud.Folio =
                    $"ADQ-{ahora.Year}-{solicitud.Id:D6}";


                /*
                 * Guarda todos los archivos enviados.
                 */
                await GuardarAdjuntosAsync(
                    solicitud,
                    usuarioActual,
                    ahora
                );


                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            enviar
                                ? "SOLICITUD_ENVIADA"
                                : "BORRADOR_CREADO",

                        Descripcion =
                            enviar
                                ? "La solicitud fue creada y enviada para aprobación del gerente."
                                : "La solicitud fue creada como borrador.",

                        EstatusAnteriorId =
                            null,

                        EstatusNuevoId =
                            estatusInicialId,

                        FechaEvento =
                            ahora,

                        DireccionIp =
                            ObtenerDireccionIp()
                    }
                );


                if (enviar)
                {
                    if (empleado == null)
                    {
                        throw new InvalidOperationException(
                            "No se encontró el empleado solicitante."
                        );
                    }


                    Empleado? jefe =
                        await ObtenerJefeAsync(
                            empleado
                        );


                    if (
                        jefe == null ||
                        string.IsNullOrWhiteSpace(
                            jefe.UserId
                        )
                    )
                    {
                        throw new InvalidOperationException(
                            "No fue posible identificar al jefe aprobador."
                        );
                    }


                    _context.AdqAprobaciones.Add(
                        new AdqAprobacion
                        {
                            SolicitudId =
                                solicitud.Id,

                            TipoAprobacion =
                                "GerenteArea",

                            Orden =
                                1,

                            UsuarioAprobadorId =
                                jefe.UserId,

                            Estatus =
                                "Pendiente",

                            FechaCreacion =
                                ahora
                        }
                    );
                }


                await _context
                    .SaveChangesAsync();


                await transaccion
                    .CommitAsync();
            }
            catch
            {
                await transaccion
                    .RollbackAsync();

                throw;
            }
        }


        // =========================================================
        // VALIDAR ARCHIVOS
        // =========================================================

        private void ValidarArchivos()
        {
            if (
                ArchivosSolicitud == null ||
                ArchivosSolicitud.Count == 0
            )
            {
                return;
            }


            string[] extensionesPermitidas =
            {
                ".pdf",
                ".doc",
                ".docx",
                ".xls",
                ".xlsx",
                ".png",
                ".jpg",
                ".jpeg"
            };


            const long tamanoMaximo =
                15 * 1024 * 1024;


            foreach (
                IFormFile archivo
                in ArchivosSolicitud)
            {
                if (
                    archivo == null ||
                    archivo.Length == 0
                )
                {
                    continue;
                }


                if (
                    archivo.Length >
                    tamanoMaximo
                )
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"El archivo {archivo.FileName} supera el límite de 15 MB."
                    );

                    continue;
                }


                string extension =
                    Path.GetExtension(
                        archivo.FileName
                    )
                    .ToLowerInvariant();


                if (
                    !extensionesPermitidas.Contains(
                        extension
                    )
                )
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"El formato del archivo {archivo.FileName} no está permitido."
                    );
                }
            }
        }


        // =========================================================
        // GUARDAR ADJUNTOS
        // =========================================================

        private async Task GuardarAdjuntosAsync(
            AdqSolicitud solicitud,
            AppUser usuarioActual,
            DateTime ahora)
        {
            if (
                ArchivosSolicitud == null ||
                ArchivosSolicitud.Count == 0
            )
            {
                return;
            }


            string[] extensionesPermitidas =
            {
                ".pdf",
                ".doc",
                ".docx",
                ".xls",
                ".xlsx",
                ".png",
                ".jpg",
                ".jpeg"
            };


            const long tamanoMaximo =
                15 * 1024 * 1024;


            /*
             * Validación nuevamente en servidor.
             */
            foreach (
                IFormFile archivo
                in ArchivosSolicitud)
            {
                if (
                    archivo == null ||
                    archivo.Length == 0
                )
                {
                    continue;
                }


                if (
                    archivo.Length >
                    tamanoMaximo
                )
                {
                    throw new InvalidOperationException(
                        $"El archivo {archivo.FileName} supera el límite permitido de 15 MB."
                    );
                }


                string extension =
                    Path.GetExtension(
                        archivo.FileName
                    )
                    .ToLowerInvariant();


                if (
                    !extensionesPermitidas.Contains(
                        extension
                    )
                )
                {
                    throw new InvalidOperationException(
                        $"El formato del archivo {archivo.FileName} no está permitido."
                    );
                }
            }


            string carpetaRelativa =
                Path.Combine(
                    "uploads",
                    "adquisiciones",
                    solicitud.Id.ToString()
                );


            string carpetaFisica =
                Path.Combine(
                    _environment.WebRootPath,
                    carpetaRelativa
                );


            Directory.CreateDirectory(
                carpetaFisica
            );


            foreach (
                IFormFile archivo
                in ArchivosSolicitud)
            {
                if (
                    archivo == null ||
                    archivo.Length == 0
                )
                {
                    continue;
                }


                string extension =
                    Path.GetExtension(
                        archivo.FileName
                    )
                    .ToLowerInvariant();


                string nombreGuardado =
                    $"{Guid.NewGuid():N}{extension}";


                string rutaFisica =
                    Path.Combine(
                        carpetaFisica,
                        nombreGuardado
                    );


                await using (
                    FileStream stream =
                        new(
                            rutaFisica,
                            FileMode.Create
                        )
                )
                {
                    await archivo.CopyToAsync(
                        stream
                    );
                }


                string rutaWeb =
                    "/" +
                    Path.Combine(
                        carpetaRelativa,
                        nombreGuardado
                    )
                    .Replace(
                        "\\",
                        "/"
                    );


                _context.AdqAdjuntos.Add(
                    new AdqAdjunto
                    {
                        SolicitudId =
                            solicitud.Id,

                        NombreOriginal =
                            Path.GetFileName(
                                archivo.FileName
                            ),

                        NombreGuardado =
                            nombreGuardado,

                        RutaArchivo =
                            rutaWeb,

                        Extension =
                            extension,

                        MimeType =
                            archivo.ContentType,

                        TamanoBytes =
                            archivo.Length,

                        UsuarioCargaId =
                            usuarioActual.Id,

                        FechaCarga =
                            ahora,

                        TipoDocumento =
                            "General",

                        Eliminado =
                            false
                    }
                );
            }
        }


        // =========================================================
        // DETALLE SOLICITUD
        // =========================================================

        public async Task<IActionResult>
            OnGetDetalleSolicitudAsync(
                int id)
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Usuario no identificado."
                    }
                );
            }


            var solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.Id == id &&
                            !x.Eliminado &&
                            x.UsuarioSolicitanteId ==
                                usuarioActual.Id
                    )
                    .Select(
                        x =>
                            new
                            {
                                x.Id,

                                x.Folio,

                                x.Titulo,

                                x.Descripcion,

                                x.Justificacion,

                                x.FechaSolicitud,

                                x.AreaId,

                                Area =
                                    x.Area.Nombre,

                                x.EstatusId,

                                Estatus =
                                    x.Estatus.Nombre,

                                Detalles =
                                    x.Detalles
                                        .Where(
                                            d =>
                                                !d.Eliminado
                                        )
                                        .OrderBy(
                                            d =>
                                                d.Orden
                                        )
                                        .Select(
                                            d =>
                                                new
                                                {
                                                    d.Id,

                                                    d.ProductoServicio,

                                                    d.Cantidad,

                                                    d.Unidad,

                                                    d.Descripcion
                                                }
                                        )
                                        .ToList(),

                                Adjuntos =
                                    x.Adjuntos
                                        .Where(
                                            a =>
                                                !a.Eliminado
                                        )
                                        .OrderBy(
                                            a =>
                                                a.FechaCarga
                                        )
                                        .Select(
                                            a =>
                                                new
                                                {
                                                    a.Id,

                                                    a.NombreOriginal,

                                                    a.RutaArchivo,

                                                    a.Extension,

                                                    a.MimeType,

                                                    a.TamanoBytes
                                                }
                                        )
                                        .ToList()
                            }
                    )
                    .FirstOrDefaultAsync();


            if (solicitud == null)
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "No se encontró la solicitud."
                    }
                );
            }


            return new JsonResult(
                new
                {
                    success =
                        true,

                    solicitud
                }
            );
        }


        // =========================================================
        // CARGAR PANTALLA
        // =========================================================

        private async Task CargarPantallaAsync(
            AppUser usuarioActual)
        {
            UsuarioActual =
                usuarioActual;


            EmpleadoActual =
                await ObtenerEmpleadoActualAsync(
                    usuarioActual
                );


            NombreSolicitante =
                EmpleadoActual?.NombreCompleto ??
                usuarioActual.UserName ??
                usuarioActual.Email ??
                "Usuario";


            NombreArea =
                EmpleadoActual?.Area?.Nombre ??
                "Sin área asignada";


            Empleado? jefe =
                EmpleadoActual != null
                    ? await ObtenerJefeAsync(
                        EmpleadoActual
                    )
                    : null;


            TieneJefeConfigurado =
                jefe != null &&
                !string.IsNullOrWhiteSpace(
                    jefe.UserId
                );


            NombreJefe =
                jefe?.NombreCompleto ??
                "Sin jefe configurado";


            await CargarAreasAsync();


            await CargarSolicitudesAsync(
                usuarioActual
            );


            CalcularKpis();


            if (
                Input.AreaId == 0 &&
                EmpleadoActual?.AreaId != null
            )
            {
                Input.AreaId =
                    EmpleadoActual.AreaId.Value;
            }
        }


        // =========================================================
        // USUARIO
        // =========================================================

        private async Task<AppUser?>
            ObtenerUsuarioActualAsync()
        {
            return await _userManager
                .GetUserAsync(
                    User
                );
        }


        // =========================================================
        // EMPLEADO
        // =========================================================

        private async Task<Empleado?>
            ObtenerEmpleadoActualAsync(
                AppUser usuario)
        {
            if (
                usuario.EmpleadoId.HasValue
            )
            {
                Empleado? empleado =
                    await _context.Empleados
                        .AsNoTracking()
                        .Include(
                            x => x.Area
                        )
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                usuario.EmpleadoId.Value
                        );


                if (
                    empleado != null
                )
                {
                    return empleado;
                }
            }


            return await _context.Empleados
                .AsNoTracking()
                .Include(
                    x => x.Area
                )
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId ==
                        usuario.Id
                );
        }


        // =========================================================
        // JEFE
        // =========================================================

        private async Task<Empleado?>
            ObtenerJefeAsync(
                Empleado empleado)
        {
            if (
                !empleado.JefeId.HasValue
            )
            {
                return null;
            }


            return await _context.Empleados
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.Id ==
                            empleado.JefeId.Value &&
                        x.Deshabilitado ==
                            0
                );
        }


        // =========================================================
        // ÁREAS
        // =========================================================

        private async Task CargarAreasAsync()
        {
            Areas =
                await _context.Areas
                    .AsNoTracking()
                    .OrderBy(
                        x =>
                            x.Nombre
                    )
                    .Select(
                        x =>
                            new SelectListItem
                            {
                                Value =
                                    x.Id.ToString(),

                                Text =
                                    x.Nombre
                            }
                    )
                    .ToListAsync();
        }


        // =========================================================
        // SOLICITUDES
        // =========================================================

        private async Task CargarSolicitudesAsync(
            AppUser usuarioActual)
        {
            Solicitudes =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .Include(
                        x => x.Area
                    )
                    .Include(
                        x => x.Estatus
                    )
                    .Where(
                        x =>
                            x.UsuarioSolicitanteId ==
                                usuarioActual.Id &&
                            !x.Eliminado
                    )
                    .OrderByDescending(
                        x =>
                            x.FechaCreacion
                    )
                    .ToListAsync();
        }


        // =========================================================
        // KPIs
        // =========================================================

        private void CalcularKpis()
        {
            TotalSolicitudes =
                Solicitudes.Count;


            TotalBorradores =
                Solicitudes.Count(
                    x =>
                        x.EstatusId ==
                        1
                );


            TotalPendientes =
                Solicitudes.Count(
                    x =>
                        x.EstatusId == 2 ||
                        x.EstatusId == 3 ||
                        x.EstatusId == 4
                );


            TotalEnProceso =
                Solicitudes.Count(
                    x =>
                        x.EstatusId >= 5 &&
                        x.EstatusId <= 16 &&
                        x.EstatusId != 6 &&
                        x.EstatusId != 7
                );


            TotalFinalizadas =
                Solicitudes.Count(
                    x =>
                        x.EstatusId ==
                        17
                );
        }


        // =========================================================
        // NORMALIZAR
        // =========================================================

        private void NormalizarInput()
        {
            Input.Titulo =
                Input.Titulo?
                    .Trim() ??
                string.Empty;


            Input.Descripcion =
                Input.Descripcion?
                    .Trim() ??
                string.Empty;


            Input.Justificacion =
                Input.Justificacion?
                    .Trim() ??
                string.Empty;


            Input.Detalles ??=
                new List<NuevaSolicitudDetalleInput>();


            foreach (
                NuevaSolicitudDetalleInput item
                in Input.Detalles)
            {
                item.ProductoServicio =
                    item.ProductoServicio?
                        .Trim() ??
                    string.Empty;


                item.Unidad =
                    item.Unidad?
                        .Trim() ??
                    string.Empty;


                item.Descripcion =
                    item.Descripcion?
                        .Trim();
            }
        }


        // =========================================================
        // VALIDAR PRODUCTOS
        // =========================================================

        private void ValidarDetalles()
        {
            Input.Detalles ??=
                new List<NuevaSolicitudDetalleInput>();


            Input.Detalles =
                Input.Detalles
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x.ProductoServicio
                            ) ||
                            x.Cantidad > 0 ||
                            !string.IsNullOrWhiteSpace(
                                x.Unidad
                            ) ||
                            !string.IsNullOrWhiteSpace(
                                x.Descripcion
                            )
                    )
                    .ToList();


            if (
                Input.Detalles.Count ==
                0
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Debes agregar al menos un producto o servicio."
                );

                return;
            }


            for (
                int indice = 0;
                indice < Input.Detalles.Count;
                indice++)
            {
                NuevaSolicitudDetalleInput item =
                    Input.Detalles[indice];


                if (
                    string.IsNullOrWhiteSpace(
                        item.ProductoServicio
                    )
                )
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"El producto o servicio #{indice + 1} es obligatorio."
                    );
                }


                if (
                    item.Cantidad <=
                    0
                )
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"La cantidad del producto #{indice + 1} debe ser mayor a cero."
                    );
                }


                if (
                    string.IsNullOrWhiteSpace(
                        item.Unidad
                    )
                )
                {
                    ModelState.AddModelError(
                        string.Empty,
                        $"La unidad del producto #{indice + 1} es obligatoria."
                    );
                }
            }
        }


        // =========================================================
        // IP
        // =========================================================

        private string? ObtenerDireccionIp()
        {
            string? ip =
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString();


            if (
                ip ==
                "::1"
            )
            {
                ip =
                    "127.0.0.1";
            }


            return ip;
        }
    }
}