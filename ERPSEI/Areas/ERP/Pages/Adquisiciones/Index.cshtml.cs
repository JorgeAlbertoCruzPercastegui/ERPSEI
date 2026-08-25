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
        public List<int> AdjuntosEliminarIds
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

        [BindProperty]
        public int SolicitudCancelarUsuarioId
        {
            get;
            set;
        }


        [BindProperty]
        [StringLength(
            2000,
            ErrorMessage =
                "El motivo no puede superar los 2000 caracteres."
        )]
        public string? MotivoCancelacionUsuario
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
        // SOLICITUDES POR APROBAR
        // =========================================================

        public List<SolicitudPorAprobarDto> SolicitudesPorAprobar
        {
            get;
            private set;
        } = new();

        // =========================================================
        // BANDEJA DE ADQUISICIONES
        // =========================================================

        public bool EsUsuarioAdquisiciones
        {
            get;
            private set;
        }


        public bool PuedeAprobarAdquisiciones
        {
            get;
            private set;
        }


        public bool PuedeAsignarAdquisiciones
        {
            get;
            private set;
        }


        public List<SolicitudAdquisicionesDto> SolicitudesAdquisiciones
        {
            get;
            private set;
        } = new();


        public List<SelectListItem> AgentesCompras
        {
            get;
            private set;
        } = new();


        [BindProperty]
        public int SolicitudAdquisicionesId
        {
            get;
            set;
        }


        [BindProperty]
        [StringLength(
            2000,
            ErrorMessage =
                "El comentario no puede superar los 2000 caracteres.")]
        public string? ComentarioAdquisiciones
        {
            get;
            set;
        }


        [BindProperty]
        public string? UsuarioAsignadoAdqId
        {
            get;
            set;
        }


        public int TotalPorAprobar
        {
            get
            {
                return SolicitudesPorAprobar.Count;
            }
        }


        // =========================================================
        // DECISIÓN DEL GERENTE
        // =========================================================

        [BindProperty]
        public int SolicitudDecisionId
        {
            get;
            set;
        }


        [BindProperty]
        [StringLength(
            2000,
            ErrorMessage =
                "El comentario no puede superar los 2000 caracteres.")]
        public string? ComentarioDecision
        {
            get;
            set;
        }

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
        // DTO SOLICITUD POR APROBAR
        // =========================================================

        public class SolicitudPorAprobarDto
        {
            public int SolicitudId
            {
                get;
                set;
            }


            public string Folio
            {
                get;
                set;
            } = string.Empty;


            public string Titulo
            {
                get;
                set;
            } = string.Empty;


            public string Solicitante
            {
                get;
                set;
            } = string.Empty;


            public string Area
            {
                get;
                set;
            } = string.Empty;


            public DateTime FechaSolicitud
            {
                get;
                set;
            }
        }

        // =========================================================
        // DTO BANDEJA DE ADQUISICIONES
        // =========================================================

        public class SolicitudAdquisicionesDto
        {
            public int Id
            {
                get;
                set;
            }


            public string Folio
            {
                get;
                set;
            } = string.Empty;


            public string Titulo
            {
                get;
                set;
            } = string.Empty;


            public string Solicitante
            {
                get;
                set;
            } = string.Empty;


            public string Area
            {
                get;
                set;
            } = string.Empty;


            public DateTime FechaSolicitud
            {
                get;
                set;
            }


            public int EstatusId
            {
                get;
                set;
            }


            public string Estatus
            {
                get;
                set;
            } = string.Empty;
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

                    .Include(
                        x => x.Adjuntos
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


            if (
                solicitud.EstatusId != 1 &&
                solicitud.EstatusId != 2
            )
            {
                TempData["MensajeError"] =
                    "La solicitud ya no puede modificarse porque ya fue aprobada por el gerente.";

                return RedirectToPage();
            }


            DateTime ahora =
                DateTime.Now;

            int estatusAnterior =
                solicitud.EstatusId;


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

                // =========================================================
                // ELIMINAR ADJUNTOS SELECCIONADOS
                // =========================================================

                if (
                    AdjuntosEliminarIds != null &&
                    AdjuntosEliminarIds.Count > 0
                )
                {
                    List<AdqAdjunto> adjuntosEliminar =
                        solicitud.Adjuntos
                            .Where(
                                x =>
                                    AdjuntosEliminarIds.Contains(
                                        x.Id
                                    )
                                    &&
                                    !x.Eliminado
                            )
                            .ToList();


                    foreach (
                        AdqAdjunto adjunto
                        in adjuntosEliminar
                    )
                    {
                        /*
                         * Eliminación lógica.
                         * Conservamos el archivo físico para trazabilidad.
                         */
                        adjunto.Eliminado =
                            true;
                    }
                }

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
                            "El usuario modificó la información de la solicitud.",

                        EstatusAnteriorId =
                            estatusAnterior,

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
        // ENVIAR BORRADOR EXISTENTE
        // =========================================================

        public async Task<IActionResult>
            OnPostEnviarBorradorAsync()
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
                    "No se identificó el borrador a enviar.";

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

                    .Include(
                        x => x.Adjuntos
                    )

                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                SolicitudEditarId.Value
                            &&
                            x.UsuarioSolicitanteId ==
                                usuarioActual.Id
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return NotFound();
            }


            if (solicitud.EstatusId != 1)
            {
                TempData["MensajeError"] =
                    "Solamente se pueden enviar solicitudes que se encuentren en borrador.";

                return RedirectToPage();
            }


            Empleado? empleado =
                await ObtenerEmpleadoActualAsync(
                    usuarioActual
                );


            if (empleado == null)
            {
                TempData["MensajeError"] =
                    "No fue posible identificar al empleado solicitante.";

                return RedirectToPage();
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
                TempData["MensajeError"] =
                    "No fue posible identificar al gerente responsable.";

                return RedirectToPage();
            }


            DateTime ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // =====================================================
                // ACTUALIZAR DATOS DEL BORRADOR
                // =====================================================

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


                // =====================================================
                // ELIMINAR ADJUNTOS MARCADOS
                // =====================================================

                if (
                    AdjuntosEliminarIds != null &&
                    AdjuntosEliminarIds.Count > 0
                )
                {
                    foreach (
                        AdqAdjunto adjunto
                        in solicitud.Adjuntos.Where(
                            x =>
                                !x.Eliminado &&
                                AdjuntosEliminarIds.Contains(
                                    x.Id
                                )
                        )
                    )
                    {
                        adjunto.Eliminado =
                            true;
                    }
                }


                // =====================================================
                // ACTUALIZAR PRODUCTOS
                // =====================================================

                foreach (
                    AdqSolicitudDetalle detalle
                    in solicitud.Detalles.Where(
                        x => !x.Eliminado
                    )
                )
                {
                    detalle.Eliminado =
                        true;
                }


                int orden =
                    1;


                foreach (
                    NuevaSolicitudDetalleInput item
                    in Input.Detalles
                )
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


                // =====================================================
                // AGREGAR ARCHIVOS NUEVOS
                // =====================================================

                await GuardarAdjuntosAsync(
                    solicitud,
                    usuarioActual,
                    ahora
                );


                // =====================================================
                // ENVIAR AL GERENTE
                // =====================================================

                solicitud.EstatusId =
                    2;

                solicitud.FechaEnvio =
                    ahora;


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


                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            "BORRADOR_ENVIADO",

                        Descripcion =
                            "El borrador fue actualizado y enviado para aprobación del gerente.",

                        EstatusAnteriorId =
                            1,

                        EstatusNuevoId =
                            2,

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
                    "La solicitud fue enviada correctamente para aprobación del gerente.";


                return RedirectToPage();
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al enviar el borrador {SolicitudId}.",
                    SolicitudEditarId
                );


                TempData["MensajeError"] =
                    "No fue posible enviar el borrador.";


                return RedirectToPage();
            }
        }

        // =========================================================
        // APROBAR SOLICITUD COMO GERENTE
        // =========================================================

        public async Task<IActionResult>
            OnPostAprobarGerenteAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            string comentario =
                ComentarioDecision?
                    .Trim()
                ??
                string.Empty;


            DateTime ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                AdqAprobacion? aprobacion =
                    await _context.AdqAprobaciones
                        .FirstOrDefaultAsync(
                            x =>
                                x.SolicitudId ==
                                    SolicitudDecisionId
                                &&
                                x.UsuarioAprobadorId ==
                                    usuarioActual.Id
                                &&
                                x.TipoAprobacion ==
                                    "GerenteArea"
                                &&
                                x.Estatus ==
                                    "Pendiente"
                        );


                if (aprobacion == null)
                {
                    TempData["MensajeError"] =
                        "No tienes una aprobación pendiente para esta solicitud.";

                    return RedirectToPage();
                }


                AdqSolicitud? solicitud =
                    await _context.AdqSolicitudes
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                    SolicitudDecisionId
                                &&
                                !x.Eliminado
                        );


                if (solicitud == null)
                {
                    return NotFound();
                }


                if (solicitud.EstatusId != 2)
                {
                    TempData["MensajeError"] =
                        "La solicitud ya no se encuentra pendiente de aprobación del gerente.";

                    return RedirectToPage();
                }


                int estatusAnterior =
                    solicitud.EstatusId;


                // =====================================================
                // APROBACIÓN
                // =====================================================

                aprobacion.Estatus =
                    "Aprobada";

                aprobacion.Comentario =
                    string.IsNullOrWhiteSpace(
                        comentario
                    )
                        ? null
                        : comentario;

                aprobacion.FechaRespuesta =
                    ahora;


                // =====================================================
                // LA SOLICITUD PASA A ADQUISICIONES
                // =====================================================

                solicitud.EstatusId =
                    3;

                solicitud.FechaModificacion =
                    ahora;


                // =====================================================
                // HISTORIAL
                // =====================================================

                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            "APROBACION_GERENTE_APROBADA",

                        Descripcion =
                            string.IsNullOrWhiteSpace(
                                comentario
                            )
                                ? "El gerente aprobó la solicitud. La solicitud fue enviada al área de Adquisiciones."
                                : $"El gerente aprobó la solicitud. Comentario: {comentario}",

                        EstatusAnteriorId =
                            estatusAnterior,

                        EstatusNuevoId =
                            3,

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
                    "La solicitud fue aprobada y enviada al área de Adquisiciones.";


                return RedirectToPage();
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al aprobar la solicitud {SolicitudId}.",
                    SolicitudDecisionId
                );


                TempData["MensajeError"] =
                    "No fue posible aprobar la solicitud.";


                return RedirectToPage();
            }
        }

        // =========================================================
        // RECHAZAR SOLICITUD COMO GERENTE
        // =========================================================

        public async Task<IActionResult>
            OnPostRechazarGerenteAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            string comentario =
                ComentarioDecision?
                    .Trim()
                ??
                string.Empty;


            if (
                string.IsNullOrWhiteSpace(
                    comentario
                )
            )
            {
                TempData["MensajeError"] =
                    "Debes indicar el motivo del rechazo.";

                return RedirectToPage();
            }


            if (comentario.Length > 2000)
            {
                TempData["MensajeError"] =
                    "El comentario del rechazo no puede superar los 2000 caracteres.";

                return RedirectToPage();
            }


            DateTime ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                AdqAprobacion? aprobacion =
                    await _context.AdqAprobaciones
                        .FirstOrDefaultAsync(
                            x =>
                                x.SolicitudId ==
                                    SolicitudDecisionId
                                &&
                                x.UsuarioAprobadorId ==
                                    usuarioActual.Id
                                &&
                                x.TipoAprobacion ==
                                    "GerenteArea"
                                &&
                                x.Estatus ==
                                    "Pendiente"
                        );


                if (aprobacion == null)
                {
                    TempData["MensajeError"] =
                        "No tienes una aprobación pendiente para esta solicitud.";

                    return RedirectToPage();
                }


                AdqSolicitud? solicitud =
                    await _context.AdqSolicitudes
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id ==
                                    SolicitudDecisionId
                                &&
                                !x.Eliminado
                        );


                if (solicitud == null)
                {
                    return NotFound();
                }


                if (solicitud.EstatusId != 2)
                {
                    TempData["MensajeError"] =
                        "La solicitud ya no se encuentra pendiente de aprobación.";

                    return RedirectToPage();
                }


                int estatusAnterior =
                    solicitud.EstatusId;


                aprobacion.Estatus =
                    "Rechazada";

                aprobacion.Comentario =
                    comentario;

                aprobacion.FechaRespuesta =
                    ahora;


                solicitud.EstatusId =
                    6;

                solicitud.FechaModificacion =
                    ahora;


                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            "APROBACION_GERENTE_RECHAZADA",

                        Descripcion =
                            $"El gerente rechazó la solicitud. Motivo: {comentario}",

                        EstatusAnteriorId =
                            estatusAnterior,

                        EstatusNuevoId =
                            6,

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
                    "La solicitud fue rechazada correctamente.";


                return RedirectToPage();
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al rechazar solicitud {SolicitudId}.",
                    SolicitudDecisionId
                );


                TempData["MensajeError"] =
                    "No fue posible rechazar la solicitud.";


                return RedirectToPage();
            }
        }

        // =========================================================
        // APROBAR SOLICITUD - ADQUISICIONES
        // =========================================================

        public async Task<IActionResult>
            OnPostAprobarAdquisicionesAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            await CargarPermisosAdquisicionesAsync(
                usuarioActual
            );


            if (!PuedeAprobarAdquisiciones)
            {
                return Forbid();
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                SolicitudAdquisicionesId
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return NotFound();
            }


            if (solicitud.EstatusId != 3)
            {
                TempData["MensajeError"] =
                    "La solicitud ya no se encuentra pendiente de revisión por Adquisiciones.";

                return RedirectToPage();
            }


            DateTime ahora =
                DateTime.Now;


            int estatusAnterior =
                solicitud.EstatusId;


            string comentario =
                ComentarioAdquisiciones?
                    .Trim()
                ??
                string.Empty;


            solicitud.EstatusId =
                5;

            solicitud.FechaModificacion =
                ahora;


            _context.AdqHistorial.Add(
                new AdqHistorial
                {
                    SolicitudId =
                        solicitud.Id,

                    UsuarioId =
                        usuarioActual.Id,

                    TipoEvento =
                        "APROBADA_ADQUISICIONES",

                    Descripcion =
                        string.IsNullOrWhiteSpace(
                            comentario
                        )
                            ? "El área de Adquisiciones aprobó la solicitud."
                            : $"El área de Adquisiciones aprobó la solicitud. Comentario: {comentario}",

                    EstatusAnteriorId =
                        estatusAnterior,

                    EstatusNuevoId =
                        5,

                    FechaEvento =
                        ahora,

                    DireccionIp =
                        ObtenerDireccionIp()
                }
            );


            await _context
                .SaveChangesAsync();


            TempData["MensajeExito"] =
                "La solicitud fue aprobada por Adquisiciones y ya puede asignarse a un agente.";


            return RedirectToPage();
        }

        // =========================================================
        // CANCELAR SOLICITUD - ADQUISICIONES
        // =========================================================

        public async Task<IActionResult>
            OnPostCancelarAdquisicionesAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            await CargarPermisosAdquisicionesAsync(
                usuarioActual
            );


            if (!PuedeAprobarAdquisiciones)
            {
                return Forbid();
            }


            string comentario =
                ComentarioAdquisiciones?
                    .Trim()
                ??
                string.Empty;


            if (
                string.IsNullOrWhiteSpace(
                    comentario
                )
            )
            {
                TempData["MensajeError"] =
                    "Debes indicar el motivo de la cancelación.";

                return RedirectToPage();
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                SolicitudAdquisicionesId
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return NotFound();
            }


            if (
                solicitud.EstatusId != 3
                &&
                solicitud.EstatusId != 5
            )
            {
                TempData["MensajeError"] =
                    "La solicitud ya no puede ser cancelada desde esta etapa.";

                return RedirectToPage();
            }


            DateTime ahora =
                DateTime.Now;


            int estatusAnterior =
                solicitud.EstatusId;


            solicitud.EstatusId =
                7;

            solicitud.FechaModificacion =
                ahora;


            _context.AdqHistorial.Add(
                new AdqHistorial
                {
                    SolicitudId =
                        solicitud.Id,

                    UsuarioId =
                        usuarioActual.Id,

                    TipoEvento =
                        "CANCELADA_ADQUISICIONES",

                    Descripcion =
                        $"Adquisiciones canceló la solicitud. Motivo: {comentario}",

                    EstatusAnteriorId =
                        estatusAnterior,

                    EstatusNuevoId =
                        7,

                    FechaEvento =
                        ahora,

                    DireccionIp =
                        ObtenerDireccionIp()
                }
            );


            await _context
                .SaveChangesAsync();


            TempData["MensajeExito"] =
                "La solicitud fue cancelada correctamente.";


            return RedirectToPage();
        }

        // =========================================================
        // ASIGNAR AGENTE DE COMPRAS
        // =========================================================

        public async Task<IActionResult>
            OnPostAsignarAgenteAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            await CargarPermisosAdquisicionesAsync(
                usuarioActual
            );


            if (!PuedeAsignarAdquisiciones)
            {
                return Forbid();
            }


            if (
                string.IsNullOrWhiteSpace(
                    UsuarioAsignadoAdqId
                )
            )
            {
                TempData["MensajeError"] =
                    "Debes seleccionar un agente de compras.";

                return RedirectToPage();
            }


            bool agenteValido =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                UsuarioAsignadoAdqId
                            &&
                            (
                                x.PuedeGestionarSolicitudes
                                ||
                                x.PuedeAsignar
                                ||
                                x.PuedeCotizar
                                ||
                                x.PuedeAdministrar
                            )
                    );


            if (!agenteValido)
            {
                TempData["MensajeError"] =
                    "El usuario seleccionado no está configurado como agente de Adquisiciones.";

                return RedirectToPage();
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                SolicitudAdquisicionesId
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return NotFound();
            }


            if (solicitud.EstatusId != 5)
            {
                TempData["MensajeError"] =
                    "La solicitud debe estar aprobada antes de asignarla.";

                return RedirectToPage();
            }


            DateTime ahora =
                DateTime.Now;


            int estatusAnterior =
                solicitud.EstatusId;


            solicitud.UsuarioAsignadoId =
                UsuarioAsignadoAdqId;


            solicitud.EstatusId =
                8;


            solicitud.FechaModificacion =
                ahora;


            _context.AdqAsignaciones.Add(
                new AdqAsignacion
                {
                    SolicitudId =
                        solicitud.Id,

                    UsuarioAsignadoId =
                        UsuarioAsignadoAdqId,

                    UsuarioAsignadorId =
                        usuarioActual.Id,

                    FechaAsignacion =
                        ahora,

                    Activa =
                        true,

                    Observaciones =
                        string.IsNullOrWhiteSpace(
                            ComentarioAdquisiciones
                        )
                            ? null
                            : ComentarioAdquisiciones.Trim()
                }
            );


            _context.AdqHistorial.Add(
                new AdqHistorial
                {
                    SolicitudId =
                        solicitud.Id,

                    UsuarioId =
                        usuarioActual.Id,

                    TipoEvento =
                        "SOLICITUD_ASIGNADA",

                    Descripcion =
                        "La solicitud fue asignada a un agente de compras.",

                    EstatusAnteriorId =
                        estatusAnterior,

                    EstatusNuevoId =
                        8,

                    FechaEvento =
                        ahora,

                    DireccionIp =
                        ObtenerDireccionIp()
                }
            );


            await _context
                .SaveChangesAsync();


            TempData["MensajeExito"] =
                "La solicitud fue asignada correctamente.";


            return RedirectToPage();
        }

        // =========================================================
        // CANCELAR SOLICITUD - SOLICITANTE
        // =========================================================

        public async Task<IActionResult>
            OnPostCancelarSolicitudUsuarioAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            string motivo =
                MotivoCancelacionUsuario?
                    .Trim()
                ??
                string.Empty;


            if (
                string.IsNullOrWhiteSpace(
                    motivo
                )
            )
            {
                TempData["MensajeError"] =
                    "Debes indicar el motivo de la cancelación.";

                return RedirectToPage();
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                SolicitudCancelarUsuarioId
                            &&
                            x.UsuarioSolicitanteId ==
                                usuarioActual.Id
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return NotFound();
            }


            /*
             * El usuario solamente puede cancelar antes
             * de que Adquisiciones apruebe la solicitud.
             */
            if (
                solicitud.EstatusId != 1 &&
                solicitud.EstatusId != 2 &&
                solicitud.EstatusId != 3
            )
            {
                TempData["MensajeError"] =
                    "La solicitud ya no puede cancelarse porque avanzó en el proceso de compra.";

                return RedirectToPage();
            }


            DateTime ahora =
                DateTime.Now;


            int estatusAnterior =
                solicitud.EstatusId;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // =====================================================
                // SI ESTABA PENDIENTE DEL GERENTE
                // =====================================================

                if (estatusAnterior == 2)
                {
                    List<AdqAprobacion> aprobacionesPendientes =
                        await _context.AdqAprobaciones
                            .Where(
                                x =>
                                    x.SolicitudId ==
                                        solicitud.Id
                                    &&
                                    x.TipoAprobacion ==
                                        "GerenteArea"
                                    &&
                                    x.Estatus ==
                                        "Pendiente"
                            )
                            .ToListAsync();


                    foreach (
                        AdqAprobacion aprobacion
                        in aprobacionesPendientes
                    )
                    {
                        aprobacion.Estatus =
                            "Cancelada";

                        aprobacion.Comentario =
                            "La solicitud fue cancelada por el solicitante.";

                        aprobacion.FechaRespuesta =
                            ahora;
                    }
                }


                // =====================================================
                // CANCELAR SOLICITUD
                // =====================================================

                solicitud.EstatusId =
                    7;

                solicitud.FechaModificacion =
                    ahora;


                // =====================================================
                // HISTORIAL
                // =====================================================

                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            "SOLICITUD_CANCELADA_USUARIO",

                        Descripcion =
                            $"El solicitante canceló la solicitud. Motivo: {motivo}",

                        EstatusAnteriorId =
                            estatusAnterior,

                        EstatusNuevoId =
                            7,

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
                    "La solicitud fue cancelada correctamente.";


                return RedirectToPage();
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al cancelar la solicitud {SolicitudId}.",
                    SolicitudCancelarUsuarioId
                );


                TempData["MensajeError"] =
                    "No fue posible cancelar la solicitud.";


                return RedirectToPage();
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
        // DETALLE DE SOLICITUD
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
                        success = false,

                        message =
                            "Usuario no identificado."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }


            // =====================================================
            // VALIDAR QUE LA SOLICITUD EXISTA
            // =====================================================

            bool solicitudExiste =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id == id
                            &&
                            !x.Eliminado
                    );


            if (!solicitudExiste)
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "No se encontró la solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // 1. PROPIETARIO DE LA SOLICITUD
            // =====================================================

            bool esPropietario =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id == id
                            &&
                            !x.Eliminado
                            &&
                            x.UsuarioSolicitanteId ==
                                usuarioActual.Id
                    );


            // =====================================================
            // 2. APROBADOR / GERENTE
            // =====================================================
            //
            // IMPORTANTE:
            // No validamos aquí que siga en "Pendiente".
            //
            // Si el gerente aprobó o rechazó anteriormente,
            // debe poder seguir consultando la solicitud
            // para mantener trazabilidad.
            // =====================================================

            bool esAprobador =
                await _context.AdqAprobaciones
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId ==
                                id
                            &&
                            x.UsuarioAprobadorId ==
                                usuarioActual.Id
                    );


            // =====================================================
            // 3. PERSONAL DE ADQUISICIONES
            // =====================================================

            bool esUsuarioAdquisiciones =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            (
                                x.PuedeVisualizar
                                ||
                                x.PuedeGestionarSolicitudes
                                ||
                                x.PuedeAprobar
                                ||
                                x.PuedeAsignar
                                ||
                                x.PuedeCotizar
                                ||
                                x.PuedeAdministrar
                            )
                    );


            // =====================================================
            // 4. AGENTE ASIGNADO
            // =====================================================
            //
            // Lo dejamos preparado desde ahora para cuando
            // lleguemos a la etapa de asignación/cotización.
            // =====================================================

            bool esAgenteAsignado =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id == id
                            &&
                            !x.Eliminado
                            &&
                            x.UsuarioAsignadoId ==
                                usuarioActual.Id
                    );


            // =====================================================
            // AUTORIZACIÓN FINAL
            // =====================================================

            bool puedeConsultar =
                esPropietario
                ||
                esAprobador
                ||
                esUsuarioAdquisiciones
                ||
                esAgenteAsignado;


            if (!puedeConsultar)
            {
                _logger.LogWarning(
                    "Acceso denegado al detalle de solicitud. " +
                    "SolicitudId: {SolicitudId}, UsuarioId: {UsuarioId}, " +
                    "Propietario: {EsPropietario}, " +
                    "Aprobador: {EsAprobador}, " +
                    "Adquisiciones: {EsUsuarioAdquisiciones}, " +
                    "Asignado: {EsAgenteAsignado}",
                    id,
                    usuarioActual.Id,
                    esPropietario,
                    esAprobador,
                    esUsuarioAdquisiciones,
                    esAgenteAsignado
                );


                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "No tienes permisos para consultar esta solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // CONSULTA DEL DETALLE
            // =====================================================

            var solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.Id == id
                            &&
                            !x.Eliminado
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


                                // =========================================
                                // SOLICITANTE
                                // =========================================

                                Solicitante =
                                    _context.Empleados
                                        .Where(
                                            empleado =>
                                                empleado.Id ==
                                                x.EmpleadoSolicitanteId
                                        )
                                        .Select(
                                            empleado =>
                                                empleado.NombreCompleto
                                        )
                                        .FirstOrDefault()
                                    ??
                                    _context.Users
                                        .Where(
                                            usuario =>
                                                usuario.Id ==
                                                x.UsuarioSolicitanteId
                                        )
                                        .Select(
                                            usuario =>
                                                usuario.Email
                                                ??
                                                usuario.UserName
                                        )
                                        .FirstOrDefault()
                                    ??
                                    "Usuario",


                                // =========================================
                                // PRODUCTOS / SERVICIOS
                                // =========================================

                                Detalles =
                                    x.Detalles
                                        .Where(
                                            detalle =>
                                                !detalle.Eliminado
                                        )
                                        .OrderBy(
                                            detalle =>
                                                detalle.Orden
                                        )
                                        .Select(
                                            detalle =>
                                                new
                                                {
                                                    detalle.Id,

                                                    detalle.ProductoServicio,

                                                    detalle.Cantidad,

                                                    detalle.Unidad,

                                                    detalle.Descripcion
                                                }
                                        )
                                        .ToList(),


                                // =========================================
                                // ADJUNTOS
                                // =========================================

                                Adjuntos =
                                    x.Adjuntos
                                        .Where(
                                            adjunto =>
                                                !adjunto.Eliminado
                                        )
                                        .OrderBy(
                                            adjunto =>
                                                adjunto.FechaCarga
                                        )
                                        .Select(
                                            adjunto =>
                                                new
                                                {
                                                    adjunto.Id,

                                                    adjunto.NombreOriginal,

                                                    adjunto.RutaArchivo,

                                                    adjunto.Extension,

                                                    adjunto.MimeType,

                                                    adjunto.TamanoBytes,

                                                    adjunto.FechaCarga
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
                        success = false,

                        message =
                            "No se encontró la solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            return new JsonResult(
                new
                {
                    success = true,

                    solicitud
                }
            );
        }

        // =========================================================
        // PERMISOS DEL MÓDULO DE ADQUISICIONES
        // =========================================================

        private async Task CargarPermisosAdquisicionesAsync(
            AppUser usuarioActual)
        {
            var permiso =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.UsuarioId ==
                            usuarioActual.Id
                    )
                    .Select(
                        x =>
                            new
                            {
                                x.PuedeVisualizar,

                                x.PuedeGestionarSolicitudes,

                                x.PuedeAprobar,

                                x.PuedeAsignar,

                                x.PuedeCotizar,

                                x.PuedeAdministrar
                            }
                    )
                    .FirstOrDefaultAsync();


            if (permiso == null)
            {
                EsUsuarioAdquisiciones =
                    false;

                PuedeAprobarAdquisiciones =
                    false;

                PuedeAsignarAdquisiciones =
                    false;

                return;
            }


            EsUsuarioAdquisiciones =
                permiso.PuedeVisualizar
                ||
                permiso.PuedeGestionarSolicitudes
                ||
                permiso.PuedeAprobar
                ||
                permiso.PuedeAsignar
                ||
                permiso.PuedeCotizar
                ||
                permiso.PuedeAdministrar;


            PuedeAprobarAdquisiciones =
                permiso.PuedeAprobar
                ||
                permiso.PuedeGestionarSolicitudes
                ||
                permiso.PuedeAdministrar;


            PuedeAsignarAdquisiciones =
                permiso.PuedeAsignar
                ||
                permiso.PuedeAdministrar;
        }

        // =========================================================
        // CARGAR BANDEJA DE ADQUISICIONES
        // =========================================================

        private async Task CargarBandejaAdquisicionesAsync()
        {
            SolicitudesAdquisiciones =
                new List<SolicitudAdquisicionesDto>();


            if (!EsUsuarioAdquisiciones)
            {
                return;
            }


            SolicitudesAdquisiciones =
                await (
                    from solicitud
                        in _context.AdqSolicitudes
                            .AsNoTracking()

                    join area
                        in _context.Areas
                            .AsNoTracking()

                        on solicitud.AreaId
                        equals area.Id

                    join estatus
                        in _context.AdqEstatus
                            .AsNoTracking()

                        on solicitud.EstatusId
                        equals estatus.Id

                    where
                        !solicitud.Eliminado
                        &&
                        (
                            solicitud.EstatusId == 3
                            ||
                            solicitud.EstatusId == 5
                        )

                    orderby
                        solicitud.FechaSolicitud
                            descending

                    select
                        new SolicitudAdquisicionesDto
                        {
                            Id =
                                solicitud.Id,

                            Folio =
                                solicitud.Folio,

                            Titulo =
                                solicitud.Titulo,

                            Solicitante =
                                _context.Empleados
                                    .Where(
                                        empleado =>
                                            empleado.Id ==
                                            solicitud.EmpleadoSolicitanteId
                                    )
                                    .Select(
                                        empleado =>
                                            empleado.NombreCompleto
                                    )
                                    .FirstOrDefault()
                                ??
                                "Usuario",

                            Area =
                                area.Nombre,

                            FechaSolicitud =
                                solicitud.FechaSolicitud,

                            EstatusId =
                                solicitud.EstatusId,

                            Estatus =
                                estatus.Nombre
                        }
                )
                .ToListAsync();


            await CargarAgentesComprasAsync();
        }

        // =========================================================
        // CARGAR AGENTES DE COMPRA
        // =========================================================

        private async Task CargarAgentesComprasAsync()
        {
            List<string> idsAgentes =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.PuedeGestionarSolicitudes
                            ||
                            x.PuedeAsignar
                            ||
                            x.PuedeCotizar
                            ||
                            x.PuedeAdministrar
                    )
                    .Select(
                        x =>
                            x.UsuarioId
                    )
                    .Distinct()
                    .ToListAsync();


            AgentesCompras =
                await (
                    from usuario
                        in _context.Users
                            .AsNoTracking()

                    join empleado
                        in _context.Empleados
                            .AsNoTracking()

                        on usuario.Id
                        equals empleado.UserId
                        into empleadoJoin

                    from empleado
                        in empleadoJoin.DefaultIfEmpty()

                    where
                        idsAgentes.Contains(
                            usuario.Id
                        )
                        &&
                        !usuario.IsBanned

                    orderby
                        empleado != null
                            ? empleado.NombreCompleto
                            : usuario.Email

                    select
                        new SelectListItem
                        {
                            Value =
                                usuario.Id,

                            Text =
                                empleado != null
                                    ? empleado.NombreCompleto
                                    : (
                                        usuario.Email
                                        ??
                                        usuario.UserName
                                        ??
                                        "Usuario"
                                    )
                        }
                )
                .ToListAsync();
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


            await CargarSolicitudesPorAprobarAsync(
                usuarioActual
            );


            await CargarPermisosAdquisicionesAsync(
                usuarioActual
            );


            await CargarBandejaAdquisicionesAsync();


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
        // SOLICITUDES PENDIENTES DE APROBACIÓN DEL GERENTE
        // =========================================================

        private async Task CargarSolicitudesPorAprobarAsync(
            AppUser usuarioActual)
        {
            SolicitudesPorAprobar =
                await (
                    from aprobacion
                        in _context.AdqAprobaciones
                            .AsNoTracking()

                    join solicitud
                        in _context.AdqSolicitudes
                            .AsNoTracking()
                        on aprobacion.SolicitudId
                        equals solicitud.Id

                    join area
                        in _context.Areas
                            .AsNoTracking()
                        on solicitud.AreaId
                        equals area.Id

                    where
                        aprobacion.UsuarioAprobadorId ==
                            usuarioActual.Id
                        &&
                        aprobacion.TipoAprobacion ==
                            "GerenteArea"
                        &&
                        aprobacion.Estatus ==
                            "Pendiente"
                        &&
                        solicitud.EstatusId ==
                            2
                        &&
                        !solicitud.Eliminado

                    orderby
                        solicitud.FechaSolicitud
                            descending

                    select
                        new SolicitudPorAprobarDto
                        {
                            SolicitudId =
                                solicitud.Id,

                            Folio =
                                solicitud.Folio,

                            Titulo =
                                solicitud.Titulo,

                            Solicitante =
                                _context.Empleados
                                    .Where(
                                        empleado =>
                                            empleado.Id ==
                                            solicitud.EmpleadoSolicitanteId
                                    )
                                    .Select(
                                        empleado =>
                                            empleado.NombreCompleto
                                    )
                                    .FirstOrDefault()
                                ??
                                "Usuario",

                            Area =
                                area.Nombre,

                            FechaSolicitud =
                                solicitud.FechaSolicitud
                        }
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