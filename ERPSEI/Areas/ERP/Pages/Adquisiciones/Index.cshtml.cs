using ERPSEI.Data;
using ERPSEI.Data.Entities.Adquisiciones;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Entities.Intranet;
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

        public class CotizacionResumenDto
        {
            public int Id
            {
                get;
                set;
            }


            public string NombreProveedor
            {
                get;
                set;
            } = string.Empty;


            public string? RfcProveedor
            {
                get;
                set;
            }


            public decimal Subtotal
            {
                get;
                set;
            }


            public decimal ImporteIva
            {
                get;
                set;
            }


            public decimal Total
            {
                get;
                set;
            }


            public bool AplicaIva
            {
                get;
                set;
            }


            public decimal PorcentajeIva
            {
                get;
                set;
            }


            public bool Finalizada
            {
                get;
                set;
            }


            public bool EsPrincipal
            {
                get;
                set;
            }


            public DateTime FechaCreacion
            {
                get;
                set;
            }


            public int TotalArchivos
            {
                get;
                set;
            }


            public int TotalDetalles
            {
                get;
                set;
            }
        }

        public List<CotizacionResumenDto> CotizacionesSolicitud
        {
            get;
            private set;
        } = new();

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
        public int SolicitudComentarioId
        {
            get;
            set;
        }

        [BindProperty]
        public List<IFormFile> ArchivosComentarioAdq
        {
            get;
            set;
        } = new();


        [BindProperty]
        [StringLength(
            5000,
            ErrorMessage =
                "El comentario no puede superar los 5000 caracteres."
        )]
        public string? NuevoComentarioAdq
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

        public class AdqComentarioAdjuntoDto
        {
            public int Id
            {
                get;
                set;
            }

            public string NombreOriginal
            {
                get;
                set;
            } = string.Empty;

            public string RutaArchivo
            {
                get;
                set;
            } = string.Empty;

            public string? Extension
            {
                get;
                set;
            }

            public string? MimeType
            {
                get;
                set;
            }

            public long TamanoBytes
            {
                get;
                set;
            }
        }

        public class AdqComentarioSeguimientoDto
        {
            public int Id
            {
                get;
                set;
            }

            public string UsuarioId
            {
                get;
                set;
            } = string.Empty;

            public string Usuario
            {
                get;
                set;
            } = string.Empty;

            public string Comentario
            {
                get;
                set;
            } = string.Empty;

            public DateTime FechaCreacion
            {
                get;
                set;
            }

            public bool EsUsuarioActual
            {
                get;
                set;
            }

            public List<AdqComentarioAdjuntoDto> Adjuntos
            {
                get;
                set;
            } = new();
        }

        public class AdqAprobacionHistorialDto
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

            public string Decision
            {
                get;
                set;
            } = string.Empty;

            public string? Comentario
            {
                get;
                set;
            }

            public DateTime? FechaRespuesta
            {
                get;
                set;
            }

            public int EstatusSolicitudId
            {
                get;
                set;
            }

            public string EstatusSolicitud
            {
                get;
                set;
            } = string.Empty;

            public int MensajesPendientes
            {
                get;
                set;
            }
        }


        public class AdqHistorialSeguimientoDto
        {
            public int Id
            {
                get;
                set;
            }

            public string TipoEvento
            {
                get;
                set;
            } = string.Empty;

            public string Descripcion
            {
                get;
                set;
            } = string.Empty;

            public DateTime FechaEvento
            {
                get;
                set;
            }

            public string Usuario
            {
                get;
                set;
            } = string.Empty;
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

        public List<AdqAprobacionHistorialDto>
        HistorialAprobaciones
        {
            get;
            set;
        } = new();

        public int TotalHistorialAprobaciones =>
        HistorialAprobaciones.Count;

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

        public List<OrdenAsignadaDto> OrdenesAsignadas
        {
            get;
            private set;
        } = new();

        public bool EsAgenteCompras
        {
            get;
            private set;
        }


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
        // COTIZACIONES
        // =========================================================

        [BindProperty]
        public CotizacionInput InputCotizacion
        {
            get;
            set;
        } = new();


        [BindProperty]
        public List<IFormFile> ArchivosCotizacion
        {
            get;
            set;
        } = new();


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
        // CARGAR COTIZACIONES DE LA SOLICITUD
        // =========================================================

        private async Task
            CargarCotizacionesSolicitudAsync(
                int solicitudId)
        {
            CotizacionesSolicitud =
                await _context.AdqCotizaciones
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.SolicitudId ==
                                solicitudId
                            &&
                            !x.Eliminado
                    )
                    .OrderBy(
                        x => x.Total
                    )
                    .ThenBy(
                        x => x.FechaCreacion
                    )
                    .Select(
                        x =>
                            new CotizacionResumenDto
                            {
                                Id =
                                    x.Id,

                                NombreProveedor =
                                    x.NombreProveedor,

                                RfcProveedor =
                                    x.RfcProveedor,

                                Subtotal =
                                    x.Subtotal,

                                ImporteIva =
                                    x.ImporteIva,

                                Total =
                                    x.Total,

                                AplicaIva =
                                    x.AplicaIva,

                                PorcentajeIva =
                                    x.PorcentajeIva,

                                Finalizada =
                                    x.Finalizada,

                                EsPrincipal =
                                    x.EsPrincipal,

                                FechaCreacion =
                                    x.FechaCreacion,

                                TotalArchivos =
                                    x.Adjuntos.Count(
                                        a =>
                                            !a.Eliminado
                                    ),

                                TotalDetalles =
                                    x.Detalles.Count(
                                        d =>
                                            !d.Eliminado
                                    )
                            }
                    )
                    .ToListAsync();
        }

        // =========================================================
        // OBTENER COTIZACIONES DE UNA SOLICITUD
        // =========================================================

        public async Task<IActionResult>
            OnGetCotizacionesSolicitudAsync(
                int id)
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Unauthorized();
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                id
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return NotFound();
            }


            bool puedeVer =
                solicitud.UsuarioSolicitanteId ==
                    usuarioActual.Id
                ||
                solicitud.UsuarioAsignadoId ==
                    usuarioActual.Id;


            if (!puedeVer)
            {
                bool usuarioAdquisiciones =
                    await _context.AdqPermisosUsuarios
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.UsuarioId ==
                                    usuarioActual.Id
                                &&
                                (
                                    x.PuedeGestionarSolicitudes
                                    ||
                                    x.PuedeCotizar
                                    ||
                                    x.PuedeAprobar
                                    ||
                                    x.PuedeAdministrar
                                )
                        );


                puedeVer =
                    usuarioAdquisiciones;
            }


            if (!puedeVer)
            {
                return Forbid();
            }


            var cotizaciones =
                await _context.AdqCotizaciones
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.SolicitudId ==
                                id
                            &&
                            !x.Eliminado
                    )
                    .OrderBy(
                        x => x.Total
                    )
                    .ThenBy(
                        x => x.FechaCreacion
                    )
                    .Select(
                        x =>
                            new
                            {
                                x.Id,

                                x.NombreProveedor,

                                x.RfcProveedor,

                                x.ContactoProveedor,

                                x.EmailProveedor,

                                x.TelefonoProveedor,

                                x.Subtotal,

                                x.AplicaIva,

                                x.PorcentajeIva,

                                x.ImporteIva,

                                x.Total,

                                x.Observaciones,

                                x.EsPrincipal,

                                x.Finalizada,

                                x.FechaCreacion,

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

                                                    d.Descripcion,

                                                    d.Cantidad,

                                                    d.Unidad,

                                                    d.PrecioUnitario,

                                                    d.Importe,

                                                    Evidencias =
                                                        d.Adjuntos
                                                            .Where(
                                                                a =>
                                                                    !a.Eliminado
                                                            )
                                                            .Select(
                                                                a =>
                                                                    new
                                                                    {
                                                                        a.Id,

                                                                        a.NombreOriginal,

                                                                        a.RutaArchivo,

                                                                        a.Extension,

                                                                        a.TamanoBytes
                                                                    }
                                                            )
                                                            .ToList()
                                                }
                                        )
                                        .ToList(),

                                ArchivosAdicionales =
                                    x.Adjuntos
                                        .Where(
                                            a =>
                                                !a.Eliminado
                                                &&
                                                a.CotizacionDetalleId ==
                                                    null
                                        )
                                        .Select(
                                            a =>
                                                new
                                                {
                                                    a.Id,

                                                    a.NombreOriginal,

                                                    a.RutaArchivo,

                                                    a.Extension,

                                                    a.TamanoBytes
                                                }
                                        )
                                        .ToList()
                            }
                    )
                    .ToListAsync();


            return new JsonResult(
                new
                {
                    ok =
                        true,

                    cotizaciones
                }
            );
        }

        // =========================================================
        // HISTORIAL DE APROBACIONES DEL GERENTE
        // =========================================================

        private async Task
            CargarHistorialAprobacionesAsync(
                AppUser usuarioActual)
        {
            HistorialAprobaciones =
                await (
                    from aprobacion
                        in _context.AdqAprobaciones
                            .AsNoTracking()

                    join solicitud
                        in _context.AdqSolicitudes
                            .AsNoTracking()
                        on aprobacion.SolicitudId
                        equals solicitud.Id

                    join estatus
                        in _context.AdqEstatus
                            .AsNoTracking()
                        on solicitud.EstatusId
                        equals estatus.Id

                    join empleado
                        in _context.Empleados
                            .AsNoTracking()
                        on solicitud.EmpleadoSolicitanteId
                        equals empleado.Id
                        into empleadoJoin

                    from empleado
                        in empleadoJoin.DefaultIfEmpty()

                    join area
                        in _context.Areas
                            .AsNoTracking()
                        on solicitud.AreaId
                        equals area.Id
                        into areaJoin

                    from area
                        in areaJoin.DefaultIfEmpty()

                    where
                        aprobacion.UsuarioAprobadorId ==
                            usuarioActual.Id
                        &&
                        aprobacion.TipoAprobacion ==
                            "GerenteArea"
                        &&
                        (
                            aprobacion.Estatus ==
                                "Aprobada"
                            ||
                            aprobacion.Estatus ==
                                "Rechazada"
                            ||
                            aprobacion.Estatus ==
                                "Cancelada"
                        )
                        &&
                        !solicitud.Eliminado

                    orderby
                        aprobacion.FechaRespuesta descending

                    select
                        new AdqAprobacionHistorialDto
                        {
                            SolicitudId =
                                solicitud.Id,

                            Folio =
                                solicitud.Folio,

                            Titulo =
                                solicitud.Titulo,

                            Solicitante =
                                empleado != null
                                    ? empleado.NombreCompleto
                                    : "No disponible",

                            Area =
                                area != null
                                    ? area.Nombre
                                    : "No disponible",

                            Decision =
                                aprobacion.Estatus,

                            Comentario =
                                aprobacion.Comentario,

                            FechaRespuesta =
                                aprobacion.FechaRespuesta,

                            EstatusSolicitudId =
                                solicitud.EstatusId,

                            EstatusSolicitud =
                                estatus.Nombre
                        }
                )
                .ToListAsync();


            // =====================================================
            // MENSAJES PENDIENTES DE RESPUESTA
            // =====================================================

            foreach (
                AdqAprobacionHistorialDto item
                in HistorialAprobaciones
            )
            {
                /*
                 * Obtenemos el último mensaje enviado por
                 * el gerente dentro de esta solicitud.
                 */
                DateTime? ultimoMensajePropio =
                    await _context.AdqComentarios
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.SolicitudId ==
                                    item.SolicitudId
                                &&
                                x.UsuarioId ==
                                    usuarioActual.Id
                                &&
                                !x.Eliminado
                                &&
                                !x.EsNotaInterna
                        )
                        .MaxAsync(
                            x =>
                                (DateTime?)
                                x.FechaCreacion
                        );


                /*
                 * Se consideran pendientes todos los mensajes
                 * enviados por otra persona después de la última
                 * respuesta del gerente.
                 *
                 * Si el gerente nunca ha contestado, todos los
                 * mensajes existentes de otras personas quedan
                 * como pendientes.
                 */
                item.MensajesPendientes =
                    await _context.AdqComentarios
                        .AsNoTracking()
                        .CountAsync(
                            x =>
                                x.SolicitudId ==
                                    item.SolicitudId
                                &&
                                x.UsuarioId !=
                                    usuarioActual.Id
                                &&
                                !x.Eliminado
                                &&
                                !x.EsNotaInterna
                                &&
                                (
                                    !ultimoMensajePropio.HasValue
                                    ||
                                    x.FechaCreacion >
                                        ultimoMensajePropio.Value
                                )
                        );
            }
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
        // DTO MIS ÓRDENES ASIGNADAS
        // =========================================================

        public class OrdenAsignadaDto
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

            public DateTime? FechaAsignacion
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

            public int MensajesPendientes
            {
                get;
                set;
            }
        }

        // =========================================================
        // INPUT COTIZACIÓN
        // =========================================================

        public class CotizacionInput
        {
            [Range(
                1,
                int.MaxValue,
                ErrorMessage =
                    "No se identificó la solicitud a cotizar."
            )]
            public int SolicitudId
            {
                get;
                set;
            }


            [Required(
                ErrorMessage =
                    "El nombre del proveedor es obligatorio."
            )]
            [StringLength(
                250,
                ErrorMessage =
                    "El nombre del proveedor no puede superar los 250 caracteres."
            )]
            public string NombreProveedor
            {
                get;
                set;
            } = string.Empty;


            [StringLength(
                50,
                ErrorMessage =
                    "El RFC no puede superar los 50 caracteres."
            )]
            public string? RfcProveedor
            {
                get;
                set;
            }


            [StringLength(
                250,
                ErrorMessage =
                    "El contacto no puede superar los 250 caracteres."
            )]
            public string? ContactoProveedor
            {
                get;
                set;
            }


            [EmailAddress(
                ErrorMessage =
                    "El correo electrónico del proveedor no es válido."
            )]
            [StringLength(
                250,
                ErrorMessage =
                    "El correo electrónico no puede superar los 250 caracteres."
            )]
            public string? EmailProveedor
            {
                get;
                set;
            }


            [StringLength(
                50,
                ErrorMessage =
                    "El teléfono no puede superar los 50 caracteres."
            )]
            public string? TelefonoProveedor
            {
                get;
                set;
            }


            public bool AplicaIva
            {
                get;
                set;
            } = true;


            [Range(
                0,
                100,
                ErrorMessage =
                    "El porcentaje de IVA debe encontrarse entre 0 y 100."
            )]
            public decimal PorcentajeIva
            {
                get;
                set;
            } = 16m;


            [StringLength(
                3000,
                ErrorMessage =
                    "Las observaciones no pueden superar los 3000 caracteres."
            )]
            public string? Observaciones
            {
                get;
                set;
            }


            public List<CotizacionDetalleInput> Detalles
            {
                get;
                set;
            } = new();
        }


        // =========================================================
        // INPUT DETALLE COTIZACIÓN
        // =========================================================

        public class CotizacionDetalleInput
        {
            [Range(
                1,
                int.MaxValue,
                ErrorMessage =
                    "No se identificó el producto de la solicitud."
            )]
            public int SolicitudDetalleId
            {
                get;
                set;
            }


            [Range(
                typeof(decimal),
                "0.01",
                "9999999999999999",
                ErrorMessage =
                    "El precio unitario debe ser mayor a cero."
            )]
            public decimal PrecioUnitario
            {
                get;
                set;
            }


            [StringLength(
                2000,
                ErrorMessage =
                    "La descripción no puede superar los 2000 caracteres."
            )]
            public string? DescripcionProveedor
            {
                get;
                set;
            }

            public IFormFile? ArchivoEvidencia
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


        public async Task<IActionResult>
            OnPostGuardarBorradorAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            // =========================================================
            // VALIDACIÓN EXCLUSIVA DEL FORMULARIO DE SOLICITUD
            // =========================================================

            ModelState.Clear();


            NormalizarInput();


            TryValidateModel(
                Input,
                nameof(Input)
            );


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


            // =========================================================
            // VALIDACIÓN EXCLUSIVA DEL FORMULARIO DE SOLICITUD
            // =========================================================

            ModelState.Clear();


            NormalizarInput();


            TryValidateModel(
                Input,
                nameof(Input)
            );


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


            // =========================================================
            // VALIDACIÓN EXCLUSIVA DEL FORMULARIO DE SOLICITUD
            // =========================================================

            ModelState.Clear();


            NormalizarInput();


            TryValidateModel(
                Input,
                nameof(Input)
            );


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


            // =========================================================
            // VALIDACIÓN EXCLUSIVA DEL FORMULARIO DE SOLICITUD
            // =========================================================

            ModelState.Clear();


            NormalizarInput();


            TryValidateModel(
                Input,
                nameof(Input)
            );


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

                // =====================================================
                // NOTIFICAR A ADQUISICIONES
                // =====================================================

                List<string> usuariosAdquisiciones =
                    await _context.AdqPermisosUsuarios
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.PuedeGestionarSolicitudes
                                ||
                                x.PuedeAprobar
                                ||
                                x.PuedeAdministrar
                        )
                        .Select(
                            x =>
                                x.UsuarioId
                        )
                        .Distinct()
                        .ToListAsync();


                await CrearNotificacionAdquisicionesAsync(
                    usuariosAdquisiciones,

                    "Nueva solicitud de compra",

                    $"La solicitud {solicitud.Folio} - {solicitud.Titulo} fue aprobada por el gerente y requiere revisión de Adquisiciones.",

                    $"/ERP/Adquisiciones?openId={solicitud.Id}",

                    usuarioActual.Id
                );


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
                                x.PuedeCotizar
                                ||
                                x.PuedeAdministrar
                            )
                    );


            if (!agenteValido)
            {
                TempData["MensajeError"] =
                    "El usuario seleccionado no está configurado como agente de compras.";

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

            // =====================================================
            // NOTIFICAR AL AGENTE ASIGNADO
            // =====================================================

            await CrearNotificacionAdquisicionesAsync(
                new[]
                {
                UsuarioAsignadoAdqId
                },

                "Nueva orden de compra asignada",

                $"Se te asignó la solicitud {solicitud.Folio} - {solicitud.Titulo} para continuar con el proceso de compra.",

                $"/ERP/Adquisiciones?openId={solicitud.Id}",

                usuarioActual.Id
            );


            TempData["MensajeExito"] =
                "La solicitud fue asignada correctamente.";


            return RedirectToPage();
        }

        // =========================================================
        // GUARDAR COTIZACIÓN
        // =========================================================

        public async Task<IActionResult>
            OnPostGuardarCotizacionAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }

            // =========================================================
            // VALIDACIÓN EXCLUSIVA DEL FORMULARIO DE COTIZACIÓN
            // =========================================================

            ModelState.Clear();

            TryValidateModel(
                InputCotizacion,
                nameof(InputCotizacion)
            );

            // =========================================================
            // VALIDAR EVIDENCIAS POR PRODUCTO
            // =========================================================

            string? errorEvidencias =
                ValidarEvidenciasDetallesCotizacionAdq();


            if (
                !string.IsNullOrWhiteSpace(
                    errorEvidencias
                )
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    errorEvidencias
                );
            }


            // =====================================================
            // VALIDAR QUE SEA AGENTE DE COMPRAS
            // =====================================================

            bool esAgenteCompras =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            (
                                x.PuedeCotizar
                                ||
                                x.PuedeAdministrar
                            )
                    );


            if (!esAgenteCompras)
            {
                return Forbid();
            }


            // =====================================================
            // NORMALIZAR DATOS
            // =====================================================

            InputCotizacion.NombreProveedor =
                InputCotizacion.NombreProveedor?
                    .Trim()
                ??
                string.Empty;


            InputCotizacion.RfcProveedor =
                string.IsNullOrWhiteSpace(
                    InputCotizacion.RfcProveedor
                )
                    ? null
                    : InputCotizacion.RfcProveedor
                        .Trim()
                        .ToUpperInvariant();


            InputCotizacion.ContactoProveedor =
                string.IsNullOrWhiteSpace(
                    InputCotizacion.ContactoProveedor
                )
                    ? null
                    : InputCotizacion.ContactoProveedor
                        .Trim();


            InputCotizacion.EmailProveedor =
                string.IsNullOrWhiteSpace(
                    InputCotizacion.EmailProveedor
                )
                    ? null
                    : InputCotizacion.EmailProveedor
                        .Trim()
                        .ToLowerInvariant();


            InputCotizacion.TelefonoProveedor =
                string.IsNullOrWhiteSpace(
                    InputCotizacion.TelefonoProveedor
                )
                    ? null
                    : InputCotizacion.TelefonoProveedor
                        .Trim();


            InputCotizacion.Observaciones =
                string.IsNullOrWhiteSpace(
                    InputCotizacion.Observaciones
                )
                    ? null
                    : InputCotizacion.Observaciones
                        .Trim();


            // =====================================================
            // VALIDACIÓN BÁSICA
            // =====================================================

            if (
                InputCotizacion.Detalles == null
                ||
                InputCotizacion.Detalles.Count == 0
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    "La cotización debe contener al menos un producto o servicio."
                );
            }


            string? errorArchivos =
                ValidarArchivosCotizacionAdq();


            if (
                !string.IsNullOrWhiteSpace(
                    errorArchivos
                )
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    errorArchivos
                );
            }


            if (!ModelState.IsValid)
            {
                TempData["MensajeError"] =
                    ModelState.Values
                        .SelectMany(
                            x => x.Errors
                        )
                        .Select(
                            x => x.ErrorMessage
                        )
                        .FirstOrDefault(
                            x =>
                                !string.IsNullOrWhiteSpace(
                                    x
                                )
                        )
                    ??
                    "Verifica la información de la cotización.";

                return RedirectToPage(
                    new
                    {
                        openId =
                            InputCotizacion.SolicitudId
                    }
                );
            }


            // =====================================================
            // CONSULTAR SOLICITUD
            // =====================================================

            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .Include(
                        x => x.Detalles
                    )
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                InputCotizacion.SolicitudId
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return NotFound();
            }


            // =====================================================
            // SEGURIDAD:
            // SOLAMENTE EL AGENTE ASIGNADO PUEDE COTIZAR
            // =====================================================

            if (
                solicitud.UsuarioAsignadoId !=
                    usuarioActual.Id
            )
            {
                TempData["MensajeError"] =
                    "La solicitud no se encuentra asignada a tu usuario.";

                return RedirectToPage(
                    new
                    {
                        openId =
                            solicitud.Id
                    }
                );
            }


            // =====================================================
            // ESTADO PERMITIDO
            // =====================================================

            if (
                solicitud.EstatusId != 8
                &&
                solicitud.EstatusId != 9
            )
            {
                TempData["MensajeError"] =
                    "La solicitud no se encuentra disponible para cotización.";

                return RedirectToPage(
                    new
                    {
                        openId =
                            solicitud.Id
                    }
                );
            }


            // =====================================================
            // DETALLES ORIGINALES DE LA SOLICITUD
            // =====================================================

            List<AdqSolicitudDetalle> detallesSolicitud =
                solicitud.Detalles
                    .Where(
                        x => !x.Eliminado
                    )
                    .OrderBy(
                        x => x.Orden
                    )
                    .ToList();


            if (detallesSolicitud.Count == 0)
            {
                TempData["MensajeError"] =
                    "La solicitud no contiene productos activos para cotizar.";

                return RedirectToPage(
                    new
                    {
                        openId =
                            solicitud.Id
                    }
                );
            }


            // =====================================================
            // VALIDAR QUE SE COTICEN TODOS LOS PRODUCTOS
            // =====================================================

            List<int> idsSolicitud =
                detallesSolicitud
                    .Select(
                        x => x.Id
                    )
                    .OrderBy(
                        x => x
                    )
                    .ToList();


            List<int> idsCotizados =
                InputCotizacion.Detalles
                    .Select(
                        x => x.SolicitudDetalleId
                    )
                    .Distinct()
                    .OrderBy(
                        x => x
                    )
                    .ToList();


            if (
                idsSolicitud.Count !=
                    idsCotizados.Count
                ||
                !idsSolicitud.SequenceEqual(
                    idsCotizados
                )
            )
            {
                TempData["MensajeError"] =
                    "La cotización debe incluir todos los productos activos de la solicitud.";

                return RedirectToPage(
                    new
                    {
                        openId =
                            solicitud.Id
                    }
                );
            }


            // =====================================================
            // VALIDAR PRECIOS
            // =====================================================

            if (
                InputCotizacion.Detalles.Any(
                    x =>
                        x.PrecioUnitario <=
                        0
                )
            )
            {
                TempData["MensajeError"] =
                    "Todos los productos deben tener un precio unitario mayor a cero.";

                return RedirectToPage(
                    new
                    {
                        openId =
                            solicitud.Id
                    }
                );
            }


            DateTime ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // =================================================
                // RECALCULAR IMPORTES EN EL SERVIDOR
                // =================================================

                decimal subtotal =
                    0m;


                List<AdqCotizacionDetalle>
                    detallesCotizacion =
                        new();


                int orden =
                    1;

                Dictionary<AdqCotizacionDetalle, IFormFile>
                evidenciasDetalles =
                    new();

                foreach (
                    AdqSolicitudDetalle detalleSolicitud
                    in detallesSolicitud
                )
                {
                    CotizacionDetalleInput?
                        detalleInput =
                            InputCotizacion.Detalles
                                .FirstOrDefault(
                                    x =>
                                        x.SolicitudDetalleId ==
                                            detalleSolicitud.Id
                                );


                    if (detalleInput == null)
                    {
                        throw new InvalidOperationException(
                            "No fue posible relacionar uno de los productos de la cotización."
                        );
                    }


                    decimal precioUnitario =
                        decimal.Round(
                            detalleInput.PrecioUnitario,
                            2,
                            MidpointRounding.AwayFromZero
                        );


                    decimal importe =
                        decimal.Round(
                            detalleSolicitud.Cantidad *
                            precioUnitario,
                            2,
                            MidpointRounding.AwayFromZero
                        );


                    subtotal +=
                        importe;


                    AdqCotizacionDetalle nuevoDetalle =
                        new()
                        {
                            ProductoServicio =
                                detalleSolicitud.ProductoServicio,

                            Descripcion =
                                string.IsNullOrWhiteSpace(
                                    detalleInput.DescripcionProveedor
                                )
                                    ? detalleSolicitud.Descripcion
                                    : detalleInput
                                        .DescripcionProveedor
                                        .Trim(),

                            Cantidad =
                                detalleSolicitud.Cantidad,

                            Unidad =
                                detalleSolicitud.Unidad,

                            PrecioUnitario =
                                precioUnitario,

                            Importe =
                                importe,

                            Orden =
                                orden++,

                            Eliminado =
                                false
                        };


                    detallesCotizacion.Add(
                        nuevoDetalle
                    );


                    if (
                        detalleInput.ArchivoEvidencia !=
                        null
                    )
                    {
                        evidenciasDetalles[
                            nuevoDetalle
                        ] =
                            detalleInput.ArchivoEvidencia;
                    }
                }


                subtotal =
                    decimal.Round(
                        subtotal,
                        2,
                        MidpointRounding.AwayFromZero
                    );


                // =================================================
                // IVA
                // =================================================

                decimal porcentajeIva =
                    InputCotizacion.AplicaIva
                        ? InputCotizacion.PorcentajeIva
                        : 0m;


                decimal importeIva =
                    InputCotizacion.AplicaIva
                        ? decimal.Round(
                            subtotal *
                            (
                                porcentajeIva /
                                100m
                            ),
                            2,
                            MidpointRounding.AwayFromZero
                        )
                        : 0m;


                decimal total =
                    decimal.Round(
                        subtotal +
                        importeIva,
                        2,
                        MidpointRounding.AwayFromZero
                    );


                // =================================================
                // SABER SI ES LA PRIMERA COTIZACIÓN
                // =================================================

                bool existeCotizacion =
                    await _context.AdqCotizaciones
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.SolicitudId ==
                                    solicitud.Id
                                &&
                                !x.Eliminado
                        );


                // =================================================
                // CREAR COTIZACIÓN
                // =================================================

                AdqCotizacion cotizacion =
                    new()
                    {
                        SolicitudId =
                            solicitud.Id,

                        ProveedorId =
                            null,

                        NombreProveedor =
                            InputCotizacion.NombreProveedor,

                        RfcProveedor =
                            InputCotizacion.RfcProveedor,

                        ContactoProveedor =
                            InputCotizacion.ContactoProveedor,

                        EmailProveedor =
                            InputCotizacion.EmailProveedor,

                        TelefonoProveedor =
                            InputCotizacion.TelefonoProveedor,

                        Subtotal =
                            subtotal,

                        AplicaIva =
                            InputCotizacion.AplicaIva,

                        PorcentajeIva =
                            porcentajeIva,

                        ImporteIva =
                            importeIva,

                        Total =
                            total,

                        Observaciones =
                            InputCotizacion.Observaciones,

                        EsPrincipal =
                            false,

                        Finalizada =
                            false,

                        Eliminado =
                            false,

                        UsuarioCreadorId =
                            usuarioActual.Id,

                        FechaCreacion =
                            ahora,

                        FechaModificacion =
                            null,

                        FechaFinalizacion =
                            null
                    };


                foreach (
                    AdqCotizacionDetalle detalle
                    in detallesCotizacion
                )
                {
                    cotizacion.Detalles.Add(
                        detalle
                    );
                }


                _context.AdqCotizaciones.Add(
                    cotizacion
                );


                /*
                 * Primer guardado para obtener
                 * el ID identity de la cotización.
                 */
                await _context
                    .SaveChangesAsync();


                // =================================================
                // EVIDENCIAS POR PRODUCTO
                // =================================================

                foreach (
                    KeyValuePair<
                        AdqCotizacionDetalle,
                        IFormFile
                    > evidencia
                    in evidenciasDetalles
                )
                {
                    await GuardarEvidenciaDetalleCotizacionAdqAsync(
                        cotizacion,
                        evidencia.Key,
                        evidencia.Value,
                        usuarioActual,
                        ahora
                    );
                }


                // =================================================
                // ARCHIVOS DE LA COTIZACIÓN
                // =================================================

                await GuardarAdjuntosCotizacionAdqAsync(
                    cotizacion,
                    usuarioActual,
                    ahora
                );


                // =================================================
                // ESTATUS DE LA SOLICITUD
                // =================================================

                int estatusAnterior =
                    solicitud.EstatusId;


                if (
                    solicitud.EstatusId ==
                    8
                )
                {
                    solicitud.EstatusId =
                        9;

                    solicitud.FechaModificacion =
                        ahora;
                }


                // =================================================
                // HISTORIAL
                // =================================================

                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            estatusAnterior == 8
                                ? "COTIZACION_INICIADA"
                                : "COTIZACION_AGREGADA",

                        Descripcion =
                            estatusAnterior == 8
                                ? $"El agente inició la etapa de cotización con el proveedor {cotizacion.NombreProveedor}."
                                : $"Se agregó una cotización del proveedor {cotizacion.NombreProveedor}.",

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
                    existeCotizacion
                        ? "La cotización del proveedor se agregó correctamente."
                        : "La cotización se creó correctamente y la solicitud pasó a En cotización.";


                return RedirectToPage(
                    new
                    {
                        openId =
                            solicitud.Id
                    }
                );
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al guardar cotización de la solicitud {SolicitudId}.",
                    InputCotizacion.SolicitudId
                );


                TempData["MensajeError"] =
                    "No fue posible guardar la cotización.";


                return RedirectToPage(
                    new
                    {
                        openId =
                            InputCotizacion.SolicitudId
                    }
                );
            }
        }

        // =========================================================
        // SELECCIONAR COTIZACIÓN
        // =========================================================

        public async Task<IActionResult>
            OnPostSeleccionarCotizacionAsync(
                int cotizacionId)
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
                            "Tu sesión ya no se encuentra disponible."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }


            if (
                cotizacionId <=
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se identificó la cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // VALIDAR PERMISO DE COTIZACIÓN
            // =====================================================

            bool esAgenteCompras =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            (
                                x.PuedeCotizar
                                ||
                                x.PuedeAdministrar
                            )
                    );


            if (!esAgenteCompras)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para seleccionar cotizaciones."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // CONSULTAR COTIZACIÓN
            // =====================================================

            AdqCotizacion? cotizacion =
                await _context.AdqCotizaciones
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                cotizacionId
                            &&
                            !x.Eliminado
                    );


            if (cotizacion == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La cotización seleccionada no existe."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // CONSULTAR SOLICITUD
            // =====================================================

            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                cotizacion.SolicitudId
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No fue posible localizar la solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // SOLAMENTE EL AGENTE ASIGNADO
            // =====================================================

            if (
                solicitud.UsuarioAsignadoId !=
                    usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Solamente el agente asignado puede seleccionar la cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // ESTATUS PERMITIDO
            // =====================================================

            if (
                solicitud.EstatusId !=
                9
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud ya no se encuentra en proceso de cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // CONSULTAR TODAS LAS COTIZACIONES ACTIVAS
            // =====================================================

            List<AdqCotizacion> cotizaciones =
                await _context.AdqCotizaciones
                    .Where(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            !x.Eliminado
                    )
                    .ToListAsync();


            if (
                cotizaciones.Count ==
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud no contiene cotizaciones registradas."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            if (
                !cotizaciones.Any(
                    x =>
                        x.Id ==
                        cotizacionId
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La cotización no pertenece a esta solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                cotizacion.EsPrincipal
            )
            {
                return new JsonResult(
                    new
                    {
                        success = true,
                        message =
                            "La cotización ya se encuentra seleccionada.",
                        cotizacionId =
                            cotizacion.Id,
                        solicitudId =
                            solicitud.Id
                    }
                );
            }


            DateTime ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // =================================================
                // QUITAR SELECCIÓN ANTERIOR
                // =================================================

                foreach (
                    AdqCotizacion item
                    in cotizaciones
                )
                {
                    bool nuevaSeleccion =
                        item.Id ==
                        cotizacion.Id;


                    if (
                        item.EsPrincipal !=
                            nuevaSeleccion
                    )
                    {
                        item.EsPrincipal =
                            nuevaSeleccion;

                        item.FechaModificacion =
                            ahora;
                    }
                }


                // =================================================
                // HISTORIAL
                // =================================================

                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            "COTIZACION_SELECCIONADA",

                        Descripcion =
                            $"El agente seleccionó la cotización del proveedor {cotizacion.NombreProveedor} por un total de {cotizacion.Total:C2}.",

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


                solicitud.FechaModificacion =
                    ahora;


                await _context
                    .SaveChangesAsync();


                await transaccion
                    .CommitAsync();


                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            $"La cotización de {cotizacion.NombreProveedor} fue seleccionada correctamente.",

                        solicitudId =
                            solicitud.Id,

                        cotizacionId =
                            cotizacion.Id,

                        proveedor =
                            cotizacion.NombreProveedor,

                        total =
                            cotizacion.Total
                    }
                );
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al seleccionar la cotización {CotizacionId} de la solicitud {SolicitudId}.",
                    cotizacionId,
                    solicitud.Id
                );


                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No fue posible seleccionar la cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }

        // =========================================================
        // FINALIZAR ETAPA DE COTIZACIÓN
        // =========================================================

        public async Task<IActionResult>
            OnPostFinalizarCotizacionAsync(
                int solicitudId)
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
                            "Tu sesión ya no se encuentra disponible."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }


            if (
                solicitudId <=
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se identificó la solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // PERMISO
            // =====================================================

            bool esAgenteCompras =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            (
                                x.PuedeCotizar
                                ||
                                x.PuedeAdministrar
                            )
                    );


            if (!esAgenteCompras)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para finalizar la cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // SOLICITUD
            // =====================================================

            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                solicitudId
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud no existe."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // AGENTE ASIGNADO
            // =====================================================

            if (
                solicitud.UsuarioAsignadoId !=
                    usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Solamente el agente asignado puede finalizar la cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // ESTATUS
            // =====================================================

            if (
                solicitud.EstatusId !=
                9
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud ya no se encuentra en proceso de cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // COTIZACIONES ACTIVAS
            // =====================================================

            List<AdqCotizacion> cotizaciones =
                await _context.AdqCotizaciones
                    .Where(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            !x.Eliminado
                    )
                    .ToListAsync();


            if (
                cotizaciones.Count ==
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No existen cotizaciones registradas para finalizar."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            List<AdqCotizacion> seleccionadas =
                cotizaciones
                    .Where(
                        x => x.EsPrincipal
                    )
                    .ToList();


            if (
                seleccionadas.Count ==
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Debes seleccionar una cotización antes de finalizar esta etapa."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            if (
                seleccionadas.Count >
                1
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Existe más de una cotización seleccionada. Corrige la selección antes de continuar."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            AdqCotizacion cotizacionSeleccionada =
                seleccionadas[0];


            DateTime ahora =
                DateTime.Now;

            int estatusAnterior =
                solicitud.EstatusId;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // =================================================
                // FINALIZAR COTIZACIÓN SELECCIONADA
                // =================================================

                foreach (
                    AdqCotizacion cotizacion
                    in cotizaciones
                )
                {
                    if (
                        cotizacion.Id ==
                        cotizacionSeleccionada.Id
                    )
                    {
                        cotizacion.Finalizada =
                            true;

                        cotizacion.FechaFinalizacion =
                            ahora;

                        cotizacion.FechaModificacion =
                            ahora;
                    }
                    else
                    {
                        cotizacion.Finalizada =
                            false;

                        cotizacion.FechaFinalizacion =
                            null;
                    }
                }


                // =================================================
                // ESTATUS SOLICITUD: 9 → 10
                // =================================================

                solicitud.EstatusId =
                    10;

                solicitud.FechaModificacion =
                    ahora;


                // =================================================
                // HISTORIAL
                // =================================================

                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            "COTIZACION_FINALIZADA",

                        Descripcion =
                            $"La etapa de cotización fue finalizada. Proveedor seleccionado: {cotizacionSeleccionada.NombreProveedor}. Total: {cotizacionSeleccionada.Total:C2}.",

                        EstatusAnteriorId =
                            estatusAnterior,

                        EstatusNuevoId =
                            10,

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


                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            "La etapa de cotización fue finalizada correctamente.",

                        solicitudId =
                            solicitud.Id,

                        estatusId =
                            10,

                        proveedor =
                            cotizacionSeleccionada.NombreProveedor,

                        total =
                            cotizacionSeleccionada.Total
                    }
                );
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al finalizar cotización de la solicitud {SolicitudId}.",
                    solicitud.Id
                );


                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No fue posible finalizar la etapa de cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }

        // =========================================================
        // REABRIR ETAPA DE COTIZACIÓN
        // =========================================================

        public async Task<IActionResult>
            OnPostReabrirCotizacionAsync(
                int solicitudId)
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
                            "Tu sesión ya no se encuentra disponible."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }


            if (
                solicitudId <=
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se identificó la solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // VALIDAR PERMISO
            // =====================================================

            bool esAgenteCompras =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            (
                                x.PuedeCotizar
                                ||
                                x.PuedeAdministrar
                            )
                    );


            if (!esAgenteCompras)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para modificar las cotizaciones."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // CONSULTAR SOLICITUD
            // =====================================================

            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                solicitudId
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud no existe."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // SOLAMENTE AGENTE ASIGNADO
            // =====================================================

            if (
                solicitud.UsuarioAsignadoId !=
                usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Solamente el agente asignado puede modificar las cotizaciones."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // SOLAMENTE DESDE COTIZACIÓN FINALIZADA
            // =====================================================

            if (
                solicitud.EstatusId !=
                10
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud ya no se encuentra disponible para modificar cotizaciones."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // NO PERMITIR SI YA EXISTE PROCESO PRESUPUESTAL ACTIVO
            // =====================================================

            bool existePresupuestoActivo =
                await _context.AdqAprobacionesPresupuestales
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            !x.Eliminado
                            &&
                            (
                                x.Estatus ==
                                    "Pendiente"
                                ||
                                x.Estatus ==
                                    "EnRevision"
                                ||
                                x.Estatus ==
                                    "Aprobada"
                            )
                    );


            if (existePresupuestoActivo)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud ya cuenta con un proceso presupuestal activo y las cotizaciones ya no pueden modificarse."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            List<AdqCotizacion> cotizaciones =
                await _context.AdqCotizaciones
                    .Where(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            !x.Eliminado
                    )
                    .ToListAsync();


            if (
                cotizaciones.Count ==
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No existen cotizaciones registradas para reabrir."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
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
                // =================================================
                // QUITAR CIERRE DE COTIZACIONES
                // =================================================

                foreach (
                    AdqCotizacion cotizacion
                    in cotizaciones
                )
                {
                    cotizacion.Finalizada =
                        false;

                    cotizacion.FechaFinalizacion =
                        null;

                    cotizacion.FechaModificacion =
                        ahora;
                }


                // =================================================
                // 10 → 9
                // =================================================

                solicitud.EstatusId =
                    9;

                solicitud.FechaModificacion =
                    ahora;


                // =================================================
                // HISTORIAL
                // =================================================

                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            "COTIZACION_REABIERTA",

                        Descripcion =
                            "El agente reabrió la etapa de cotización para revisar o registrar nuevas propuestas.",

                        EstatusAnteriorId =
                            estatusAnterior,

                        EstatusNuevoId =
                            9,

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


                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            "La etapa de cotización fue reabierta correctamente.",

                        solicitudId =
                            solicitud.Id,

                        estatusId =
                            solicitud.EstatusId
                    }
                );
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al reabrir cotización de solicitud {SolicitudId}.",
                    solicitudId
                );


                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "No fue posible reabrir la etapa de cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }

        // =========================================================
        // SOLICITAR APROBACIÓN PRESUPUESTAL
        // =========================================================

        public async Task<IActionResult>
            OnPostSolicitarPresupuestoAsync(
                int solicitudId,
                string? comentario)
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
                            "Tu sesión ya no se encuentra disponible."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }


            if (
                solicitudId <=
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se identificó la solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            comentario =
                string.IsNullOrWhiteSpace(
                    comentario
                )
                    ? null
                    : comentario.Trim();


            if (
                comentario?.Length >
                3000
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El comentario no puede superar los 3000 caracteres."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // VALIDAR QUE SEA AGENTE DE COMPRAS
            // =====================================================

            bool esAgenteCompras =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            (
                                x.PuedeCotizar
                                ||
                                x.PuedeAdministrar
                            )
                    );


            if (!esAgenteCompras)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para solicitar aprobación presupuestal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // CONSULTAR SOLICITUD
            // =====================================================

            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                solicitudId
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud no existe."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // SOLAMENTE EL AGENTE ASIGNADO
            // =====================================================

            if (
                solicitud.UsuarioAsignadoId !=
                    usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Solamente el agente asignado puede solicitar la aprobación presupuestal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // ESTATUS PERMITIDO
            // =====================================================

            if (
                solicitud.EstatusId !=
                10
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud debe tener la cotización finalizada antes de solicitar presupuesto."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // OBTENER COTIZACIÓN SELECCIONADA Y FINALIZADA
            // =====================================================

            List<AdqCotizacion> cotizacionesSeleccionadas =
                await _context.AdqCotizaciones
                    .Where(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            !x.Eliminado
                            &&
                            x.EsPrincipal
                            &&
                            x.Finalizada
                    )
                    .ToListAsync();


            if (
                cotizacionesSeleccionadas.Count ==
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No existe una cotización seleccionada y finalizada para esta solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            if (
                cotizacionesSeleccionadas.Count >
                1
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Existe más de una cotización seleccionada. Revisa la información antes de continuar."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            AdqCotizacion cotizacion =
                cotizacionesSeleccionadas[0];


            // =====================================================
            // EVITAR SOLICITUD PRESUPUESTAL DUPLICADA
            // =====================================================

            bool existeSolicitudPresupuesto =
                await _context.AdqAprobacionesPresupuestales
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            !x.Eliminado
                            &&
                            (
                                x.Estatus ==
                                    "Pendiente"
                                ||
                                x.Estatus ==
                                    "EnRevision"
                                ||
                                x.Estatus ==
                                    "Aprobada"
                            )
                    );


            if (existeSolicitudPresupuesto)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud ya cuenta con un proceso presupuestal activo."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // RESPONSABLES DE APROBACIÓN PRESUPUESTAL
            // =====================================================

            List<string> usuariosAprobadores =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.PuedeAprobarPresupuesto
                            ||
                            x.PuedeAdministrar
                    )
                    .Select(
                        x =>
                            x.UsuarioId
                    )
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x
                            )
                    )
                    .Distinct()
                    .ToListAsync();


            if (
                usuariosAprobadores.Count ==
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No existen usuarios configurados para aprobar presupuestos."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
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
                // =================================================
                // CREAR SOLICITUD PRESUPUESTAL
                // =================================================

                AdqAprobacionPresupuestal aprobacionPresupuestal =
                    new()
                    {
                        SolicitudId =
                            solicitud.Id,

                        CotizacionId =
                            cotizacion.Id,

                        MontoSolicitado =
                            cotizacion.Total,

                        UsuarioSolicitaId =
                            usuarioActual.Id,

                        FechaSolicitud =
                            ahora,

                        UsuarioAprobadorId =
                            null,

                        FechaRespuesta =
                            null,

                        Estatus =
                            "Pendiente",

                        ComentarioSolicitud =
                            comentario,

                        ComentarioRespuesta =
                            null,

                        Eliminado =
                            false
                    };


                _context.AdqAprobacionesPresupuestales.Add(
                    aprobacionPresupuestal
                );


                // =================================================
                // CAMBIAR ESTATUS: 10 → 11
                // =================================================

                solicitud.EstatusId =
                    11;

                solicitud.FechaModificacion =
                    ahora;


                // =================================================
                // HISTORIAL
                // =================================================

                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            "PRESUPUESTO_SOLICITADO",

                        Descripcion =
                            string.IsNullOrWhiteSpace(
                                comentario
                            )
                                ? $"El agente solicitó aprobación presupuestal para la cotización de {cotizacion.NombreProveedor} por un monto de {cotizacion.Total:C2}."
                                : $"El agente solicitó aprobación presupuestal para la cotización de {cotizacion.NombreProveedor} por un monto de {cotizacion.Total:C2}. Comentario: {comentario}",

                        EstatusAnteriorId =
                            estatusAnterior,

                        EstatusNuevoId =
                            11,

                        FechaEvento =
                            ahora,

                        DireccionIp =
                            ObtenerDireccionIp()
                    }
                );


                await _context
                    .SaveChangesAsync();


                // =================================================
                // NOTIFICAR A RESPONSABLES DE PRESUPUESTO
                // =================================================

                await CrearNotificacionAdquisicionesAsync(
                    usuariosAprobadores,

                    "Aprobación presupuestal pendiente",

                    $"La solicitud {solicitud.Folio} - {solicitud.Titulo} requiere aprobación presupuestal por {cotizacion.Total:C2}. Proveedor seleccionado: {cotizacion.NombreProveedor}.",

                    $"/ERP/Adquisiciones?openId={solicitud.Id}",

                    usuarioActual.Id
                );


                await transaccion
                    .CommitAsync();


                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            "La solicitud fue enviada correctamente a aprobación presupuestal.",

                        solicitudId =
                            solicitud.Id,

                        aprobacionPresupuestalId =
                            aprobacionPresupuestal.Id,

                        estatusId =
                            11,

                        proveedor =
                            cotizacion.NombreProveedor,

                        subtotal =
                            cotizacion.Subtotal,

                        iva =
                            cotizacion.ImporteIva,

                        total =
                            cotizacion.Total
                    }
                );
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al solicitar aprobación presupuestal de la solicitud {SolicitudId}.",
                    solicitud.Id
                );


                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No fue posible solicitar la aprobación presupuestal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }


        // =========================================================
        // GUARDAR EVIDENCIA POR PRODUCTO DE COTIZACIÓN
        // =========================================================

        private async Task
            GuardarEvidenciaDetalleCotizacionAdqAsync(
                AdqCotizacion cotizacion,
                AdqCotizacionDetalle detalle,
                IFormFile archivo,
                AppUser usuarioActual,
                DateTime ahora)
            {
            if (
                archivo == null
                ||
                archivo.Length <=
                    0
            )
            {
                return;
            }

            string carpetaRelativa =
                Path.Combine(
                    "uploads",
                    "adquisiciones",
                    cotizacion.SolicitudId
                        .ToString(),
                    "cotizaciones",
                    cotizacion.Id
                        .ToString(),
                    "detalles",
                    detalle.Id
                        .ToString()
                );


            string carpetaFisica =
                Path.Combine(
                    _environment.WebRootPath,
                    carpetaRelativa
                );


            Directory.CreateDirectory(
                carpetaFisica
            );


            string extension =
                Path.GetExtension(
                    archivo.FileName
                )
                .ToLowerInvariant();


            string nombreAlmacenado =
                $"{Guid.NewGuid():N}{extension}";


            string rutaFisica =
                Path.Combine(
                    carpetaFisica,
                    nombreAlmacenado
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


            string rutaPublica =
                "/" +
                Path.Combine(
                    carpetaRelativa,
                    nombreAlmacenado
                )
                .Replace(
                    "\\",
                    "/"
                );


            _context.AdqCotizacionAdjuntos.Add(
                new AdqCotizacionAdjunto
                {
                    CotizacionId =
                        cotizacion.Id,

                    CotizacionDetalleId =
                        detalle.Id,

                    NombreOriginal =
                        Path.GetFileName(
                            archivo.FileName
                        ),

                    NombreAlmacenado =
                        nombreAlmacenado,

                    RutaArchivo =
                        rutaPublica,

                    Extension =
                        extension,

                    MimeType =
                        string.IsNullOrWhiteSpace(
                            archivo.ContentType
                        )
                            ? "application/octet-stream"
                            : archivo.ContentType,

                    TamanoBytes =
                        archivo.Length,

                    UsuarioCargaId =
                        usuarioActual.Id,

                    FechaCarga =
                        ahora,

                    Eliminado =
                        false
                }
            );
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

                    // =====================================================
                    // NOTIFICAR AL JEFE DIRECTO
                    // =====================================================

                    await CrearNotificacionAdquisicionesAsync(
                        new[]
                        {
                        jefe.UserId
                        },

                        "Solicitud pendiente de aprobación",

                        $"La solicitud {solicitud.Folio} - {solicitud.Titulo} requiere tu aprobación como jefe directo.",

                        $"/ERP/Adquisiciones?openId={solicitud.Id}",

                        usuarioActual.Id
                    );


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
        // VALIDAR ARCHIVOS DEL CHAT
        // =========================================================

        private string? ValidarArchivosComentarioAdq()
        {
            if (
                ArchivosComentarioAdq == null
                ||
                ArchivosComentarioAdq.Count ==
                0
            )
            {
                return null;
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
                15L *
                1024L *
                1024L;


            foreach (
                IFormFile archivo
                in ArchivosComentarioAdq
            )
            {
                if (
                    archivo.Length <=
                    0
                )
                {
                    return
                        $"El archivo {archivo.FileName} está vacío.";
                }


                if (
                    archivo.Length >
                    tamanoMaximo
                )
                {
                    return
                        $"El archivo {archivo.FileName} supera el límite de 15 MB.";
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
                    return
                        $"El formato del archivo {archivo.FileName} no está permitido.";
                }
            }


            return null;
        }

        // =========================================================
        // VALIDAR ARCHIVOS DE COTIZACIÓN
        // =========================================================

        private string? ValidarArchivosCotizacionAdq()
        {


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
                15L *
                1024L *
                1024L;


            foreach (
                IFormFile archivo
                in ArchivosCotizacion
            )
            {
                if (
                    archivo == null
                    ||
                    archivo.Length <=
                        0
                )
                {
                    return
                        "Uno de los archivos de cotización está vacío.";
                }


                if (
                    archivo.Length >
                    tamanoMaximo
                )
                {
                    return
                        $"El archivo {archivo.FileName} supera el límite de 15 MB.";
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
                    return
                        $"El formato del archivo {archivo.FileName} no está permitido.";
                }
            }


            return null;
        }

        // =========================================================
        // VALIDAR EVIDENCIAS POR PRODUCTO
        // =========================================================

        private string? ValidarEvidenciasDetallesCotizacionAdq()
        {
            if (
                InputCotizacion.Detalles == null
                ||
                InputCotizacion.Detalles.Count == 0
            )
            {
                return
                    "La cotización no contiene productos.";
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
                15L *
                1024L *
                1024L;


            foreach (
                CotizacionDetalleInput detalle
                in InputCotizacion.Detalles
            )
            {
                IFormFile? archivo =
                    detalle.ArchivoEvidencia;


                if (
                    archivo == null
                    ||
                    archivo.Length <=
                        0
                )
                {
                    return
                        "Cada producto o servicio debe incluir su archivo de evidencia.";
                }


                if (
                    archivo.Length >
                    tamanoMaximo
                )
                {
                    return
                        $"El archivo {archivo.FileName} supera el límite de 15 MB.";
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
                    return
                        $"El formato del archivo {archivo.FileName} no está permitido.";
                }
            }


            return null;
        }

        // =========================================================
        // GUARDAR ADJUNTOS DE COTIZACIÓN
        // =========================================================

        private async Task
            GuardarAdjuntosCotizacionAdqAsync(
                AdqCotizacion cotizacion,
                AppUser usuarioActual,
                DateTime ahora)
        {
            if (
                ArchivosCotizacion == null
                ||
                ArchivosCotizacion.Count ==
                0
            )
            {
                return;
            }


            string carpetaRelativa =
                Path.Combine(
                    "uploads",
                    "adquisiciones",
                    cotizacion.SolicitudId
                        .ToString(),
                    "cotizaciones",
                    cotizacion.Id
                        .ToString()
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
                in ArchivosCotizacion
            )
            {
                if (
                    archivo == null
                    ||
                    archivo.Length <=
                        0
                )
                {
                    continue;
                }


                string extension =
                    Path.GetExtension(
                        archivo.FileName
                    )
                    .ToLowerInvariant();


                string nombreAlmacenado =
                    $"{Guid.NewGuid():N}{extension}";


                string rutaFisica =
                    Path.Combine(
                        carpetaFisica,
                        nombreAlmacenado
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


                string rutaPublica =
                    "/" +
                    Path.Combine(
                        carpetaRelativa,
                        nombreAlmacenado
                    )
                    .Replace(
                        "\\",
                        "/"
                    );


                cotizacion.Adjuntos.Add(
                    new AdqCotizacionAdjunto
                    {
                        NombreOriginal =
                            Path.GetFileName(
                                archivo.FileName
                            ),

                        NombreAlmacenado =
                            nombreAlmacenado,

                        RutaArchivo =
                            rutaPublica,

                        Extension =
                            extension,

                        MimeType =
                            string.IsNullOrWhiteSpace(
                                archivo.ContentType
                            )
                                ? "application/octet-stream"
                                : archivo.ContentType,

                        TamanoBytes =
                            archivo.Length,

                        UsuarioCargaId =
                            usuarioActual.Id,

                        FechaCarga =
                            ahora,

                        Eliminado =
                            false
                    }
                );
            }
        }

        // =========================================================
        // GUARDAR ADJUNTOS DEL CHAT
        // =========================================================

        private async Task GuardarAdjuntosComentarioAdqAsync(
            AdqComentario comentario,
            DateTime ahora)
        {
            if (
                ArchivosComentarioAdq == null
                ||
                ArchivosComentarioAdq.Count ==
                0
            )
            {
                return;
            }


            string carpetaRelativa =
                Path.Combine(
                    "uploads",
                    "adquisiciones",
                    comentario.SolicitudId.ToString(),
                    "comentarios",
                    comentario.Id.ToString()
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
                in ArchivosComentarioAdq
            )
            {
                if (
                    archivo.Length <=
                    0
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
                    Guid.NewGuid()
                        .ToString(
                            "N"
                        )
                    +
                    extension;


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


                _context.AdqComentariosAdjuntos.Add(
                    new AdqComentarioAdjunto
                    {
                        ComentarioId =
                            comentario.Id,

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

                        FechaCarga =
                            ahora,

                        Eliminado =
                            false
                    }
                );
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
        // SEGUIMIENTO / CHAT DE LA SOLICITUD
        // =========================================================

        public async Task<IActionResult>
            OnGetSeguimientoSolicitudAsync(
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
                            "No fue posible identificar al usuario."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .Include(
                        x => x.Estatus
                    )
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == id
                            &&
                            !x.Eliminado
                    );


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


            // =====================================================
            // VALIDAR ACCESO
            // =====================================================

            bool esSolicitante =
                solicitud.UsuarioSolicitanteId ==
                    usuarioActual.Id;


            /*
             * Un gerente que participó en el flujo de aprobación
             * conserva acceso al seguimiento de la solicitud,
             * incluso después de aprobarla o rechazarla.
             */
            bool esAprobador =
                await _context.AdqAprobaciones
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            x.UsuarioAprobadorId ==
                                usuarioActual.Id
                    );


            AdqPermisoUsuario? permisoAdquisiciones =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                    );


            bool esUsuarioAdquisiciones =
                permisoAdquisiciones != null
                &&
                (
                    permisoAdquisiciones.PuedeVisualizar
                    ||
                    permisoAdquisiciones.PuedeGestionarSolicitudes
                    ||
                    permisoAdquisiciones.PuedeAprobar
                    ||
                    permisoAdquisiciones.PuedeAsignar
                    ||
                    permisoAdquisiciones.PuedeCotizar
                    ||
                    permisoAdquisiciones.PuedeAdministrar
                );


            bool esAgenteAsignado =
                await _context.AdqAsignaciones
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            x.UsuarioAsignadoId ==
                                usuarioActual.Id
                            &&
                            x.Activa
                    );


            if (
                !esSolicitante
                &&
                !esAprobador
                &&
                !esUsuarioAdquisiciones
                &&
                !esAgenteAsignado
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para consultar el seguimiento de esta solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // BORRADOR TODAVÍA NO TIENE CHAT
            // =====================================================

            if (
                solicitud.EstatusId ==
                1
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El chat estará disponible cuando la solicitud sea enviada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // COMENTARIOS
            // =====================================================

            List<AdqComentarioSeguimientoDto> comentarios =
                await (
                    from comentario
                        in _context.AdqComentarios
                            .AsNoTracking()

                    join usuario
                        in _context.Users
                            .AsNoTracking()
                        on comentario.UsuarioId
                        equals usuario.Id

                    where
                        comentario.SolicitudId ==
                            solicitud.Id
                        &&
                        !comentario.Eliminado
                        &&
                        !comentario.EsNotaInterna

                    orderby
                        comentario.FechaCreacion

                    select
                        new AdqComentarioSeguimientoDto
                        {
                            Id =
                                comentario.Id,

                            UsuarioId =
                                comentario.UsuarioId,

                            Usuario =
                                usuario.Email
                                ??
                                usuario.UserName
                                ??
                                "Usuario",

                            Comentario =
                                comentario.Comentario,

                            FechaCreacion =
                                comentario.FechaCreacion,

                            EsUsuarioActual =
                                comentario.UsuarioId ==
                                    usuarioActual.Id
                        }
                )
                .ToListAsync();

            // =====================================================
            // ADJUNTOS DE LOS MENSAJES
            // =====================================================

            if (
                comentarios.Count >
                0
            )
            {
                List<int> comentarioIds =
                    comentarios
                        .Select(
                            x =>
                                x.Id
                        )
                        .ToList();


                var adjuntosComentarios =
                    await _context.AdqComentariosAdjuntos
                        .AsNoTracking()
                        .Where(
                            x =>
                                comentarioIds.Contains(
                                    x.ComentarioId
                                )
                                &&
                                !x.Eliminado
                        )
                        .Select(
                            x =>
                                new
                                {
                                    x.Id,

                                    x.ComentarioId,

                                    x.NombreOriginal,

                                    x.RutaArchivo,

                                    x.Extension,

                                    x.MimeType,

                                    x.TamanoBytes
                                }
                        )
                        .ToListAsync();


                foreach (
                    AdqComentarioSeguimientoDto comentario
                    in comentarios
                )
                {
                    comentario.Adjuntos =
                        adjuntosComentarios
                            .Where(
                                x =>
                                    x.ComentarioId ==
                                        comentario.Id
                            )
                            .Select(
                                x =>
                                    new AdqComentarioAdjuntoDto
                                    {
                                        Id =
                                            x.Id,

                                        NombreOriginal =
                                            x.NombreOriginal,

                                        RutaArchivo =
                                            x.RutaArchivo,

                                        Extension =
                                            x.Extension,

                                        MimeType =
                                            x.MimeType,

                                        TamanoBytes =
                                            x.TamanoBytes
                                    }
                            )
                            .ToList();
                }
            }


            // =====================================================
            // MENSAJES PENDIENTES DE RESPUESTA
            // =====================================================

            /*
             * Tomamos el último mensaje que escribió
             * el usuario que está viendo el chat.
             */
            DateTime? ultimoMensajePropio =
                comentarios
                    .Where(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                    )
                    .Select(
                        x =>
                            (DateTime?)
                            x.FechaCreacion
                    )
                    .Max();


            /*
             * Cualquier mensaje posterior enviado por otra
             * persona significa que existe conversación
             * pendiente de respuesta.
             *
             * Cuando el usuario responde, su mensaje pasa
             * a ser el último y el contador vuelve a cero.
             */
            int mensajesPendientes =
                comentarios.Count(
                    x =>
                        x.UsuarioId !=
                            usuarioActual.Id
                        &&
                        (
                            !ultimoMensajePropio.HasValue
                            ||
                            x.FechaCreacion >
                                ultimoMensajePropio.Value
                        )
                );


            // =====================================================
            // HISTORIAL
            // =====================================================

            List<AdqHistorialSeguimientoDto> historial =
                await (
                    from evento
                        in _context.AdqHistorial
                            .AsNoTracking()

                    join usuario
                        in _context.Users
                            .AsNoTracking()
                        on evento.UsuarioId
                        equals usuario.Id
                        into usuarioJoin

                    from usuario
                        in usuarioJoin.DefaultIfEmpty()

                    where
                        evento.SolicitudId ==
                            solicitud.Id

                    orderby
                        evento.FechaEvento descending

                    select
                        new AdqHistorialSeguimientoDto
                        {
                            Id =
                                evento.Id,

                            TipoEvento =
                                evento.TipoEvento,

                            Descripcion =
                                evento.Descripcion,

                            FechaEvento =
                                evento.FechaEvento,

                            Usuario =
                                usuario != null
                                    ? (
                                        usuario.Email
                                        ??
                                        usuario.UserName
                                        ??
                                        "Usuario"
                                    )
                                    : "Sistema"
                        }
                )
                .ToListAsync();


            // =====================================================
            // ¿PUEDE ESCRIBIR EN EL CHAT?
            // =====================================================

            /*
             * Ahora el gerente/aprobador también puede
             * participar en la conversación.
             */
            bool puedeEscribir =
                esSolicitante
                ||
                esAprobador
                ||
                esUsuarioAdquisiciones
                ||
                esAgenteAsignado;


            // =====================================================
            // RESPUESTA
            // =====================================================

            return new JsonResult(
                new
                {
                    success = true,

                    seguimiento =
                        new
                        {
                            solicitudId =
                                solicitud.Id,

                            folio =
                                solicitud.Folio,

                            estatusId =
                                solicitud.EstatusId,

                            estatus =
                                solicitud.Estatus?.Nombre
                                ??
                                "Sin estatus",

                            puedeEscribir,

                            mensajesPendientes,

                            comentarios,

                            historial
                        }
                }
            );
        }

        // =========================================================
        // ENVIAR MENSAJE DEL CHAT
        // =========================================================

        public async Task<IActionResult>
            OnPostAgregarComentarioAdqAsync()
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
                            "No fue posible identificar al usuario."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }


            string comentarioTexto =
                NuevoComentarioAdq?
                    .Trim()
                ??
                string.Empty;


            bool tieneArchivos =
                ArchivosComentarioAdq != null
                &&
                ArchivosComentarioAdq.Count >
                0;


            // =====================================================
            // VALIDAR SOLICITUD
            // =====================================================

            if (
                SolicitudComentarioId <=
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "La solicitud no es válida."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // MENSAJE O ARCHIVO OBLIGATORIO
            // =====================================================

            if (
                string.IsNullOrWhiteSpace(
                    comentarioTexto
                )
                &&
                !tieneArchivos
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "Escribe un mensaje o adjunta al menos un archivo."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                comentarioTexto.Length >
                5000
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "El mensaje no puede superar los 5000 caracteres."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // VALIDAR ARCHIVOS
            // =====================================================

            string? errorArchivos =
                ValidarArchivosComentarioAdq();


            if (
                !string.IsNullOrWhiteSpace(
                    errorArchivos
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            errorArchivos
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                SolicitudComentarioId
                            &&
                            !x.Eliminado
                    );


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


            if (
                solicitud.EstatusId ==
                1
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "Debes enviar la solicitud antes de utilizar el chat."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            bool esSolicitante =
                solicitud.UsuarioSolicitanteId ==
                    usuarioActual.Id;


            bool esAprobador =
                await _context.AdqAprobaciones
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            x.UsuarioAprobadorId ==
                                usuarioActual.Id
                    );


            bool esUsuarioAdquisiciones =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            (
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


            bool esAgenteAsignado =
                await _context.AdqAsignaciones
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            x.UsuarioAsignadoId ==
                                usuarioActual.Id
                            &&
                            x.Activa
                    );


            if (
                !esSolicitante
                &&
                !esAprobador
                &&
                !esUsuarioAdquisiciones
                &&
                !esAgenteAsignado
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "No tienes permisos para participar en este chat."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            DateTime ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                AdqComentario nuevoComentario =
                    new()
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        Comentario =
                            comentarioTexto,

                        EsNotaInterna =
                            false,

                        FechaCreacion =
                            ahora,

                        Eliminado =
                            false
                    };


                _context.AdqComentarios.Add(
                    nuevoComentario
                );


                await _context
                    .SaveChangesAsync();


                await GuardarAdjuntosComentarioAdqAsync(
                    nuevoComentario,
                    ahora
                );


                await _context
                    .SaveChangesAsync();


                await transaccion
                    .CommitAsync();


                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            "Mensaje enviado correctamente.",

                        comentarioId =
                            nuevoComentario.Id
                    }
                );
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al enviar mensaje de seguimiento de la solicitud {SolicitudId}.",
                    SolicitudComentarioId
                );


                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "No fue posible enviar el mensaje."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }

        // =========================================================
        // PERMISOS DEL MÓDULO DE ADQUISICIONES
        // =========================================================

        private async Task CrearNotificacionAdquisicionesAsync(
            IEnumerable<string> usuariosDestinoIds,
            string titulo,
            string descripcion,
            string url,
            string? usuarioCreadorId)
        {
            List<string> destinatarios =
                usuariosDestinoIds
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x
                            )
                    )
                    .Distinct()
                    .ToList();


            if (
                destinatarios.Count ==
                0
            )
            {
                return;
            }


            DateTime ahora =
                DateTime.Now;


            NotificacionIntranet notificacion =
                new()
                {
                    Titulo =
                        titulo,

                    Descripcion =
                        descripcion,

                    Tipo =
                        "Adquisiciones",

                    Modulo =
                        "Adquisiciones",

                    Url =
                        url,

                    Icono =
                        "bi bi-cart-check-fill",

                    FechaPublicacion =
                        ahora,

                    Activa =
                        true,

                    UserIdCreador =
                        usuarioCreadorId
                };


            foreach (
                string usuarioId
                in destinatarios
            )
            {
                notificacion.UsuariosNotificados.Add(
                    new NotificacionIntranetUsuario
                    {
                        UserId =
                            usuarioId,

                        Leida =
                            false,

                        FechaCreacion =
                            ahora
                    }
                );
            }


            _context.NotificacionesIntranet.Add(
                notificacion
            );


            await _context
                .SaveChangesAsync();
        }



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
                EmpleadoActual?.NombreCompleto
                ??
                usuarioActual.UserName
                ??
                usuarioActual.Email
                ??
                "Usuario";


            NombreArea =
                EmpleadoActual?.Area?.Nombre
                ??
                "Sin área asignada";


            Empleado? jefe =
                EmpleadoActual != null
                    ? await ObtenerJefeAsync(
                        EmpleadoActual
                    )
                    : null;


            TieneJefeConfigurado =
                jefe != null
                &&
                !string.IsNullOrWhiteSpace(
                    jefe.UserId
                );


            NombreJefe =
                jefe?.NombreCompleto
                ??
                "Sin jefe configurado";


            await CargarAreasAsync();


            await CargarSolicitudesAsync(
                usuarioActual
            );


            await CargarSolicitudesPorAprobarAsync(
                usuarioActual
            );


            /*
             * Historial del gerente:
             * aprobadas, rechazadas y canceladas.
             */
            await CargarHistorialAprobacionesAsync(
                usuarioActual
            );


            await CargarPermisosAdquisicionesAsync(
                usuarioActual
            );


            await CargarBandejaAdquisicionesAsync();


            // =========================================================
            // MIS ÓRDENES ASIGNADAS
            // =========================================================

            await CargarOrdenesAsignadasAsync(
                usuarioActual
            );


            CalcularKpis();


            if (
                Input.AreaId == 0
                &&
                EmpleadoActual?.AreaId != null
            )
            {
                Input.AreaId =
                    EmpleadoActual.AreaId.Value;
            }
        }

        // =========================================================
        // CARGAR MIS ÓRDENES ASIGNADAS
        // =========================================================

        private async Task CargarOrdenesAsignadasAsync(
            AppUser usuarioActual)
        {
            EsAgenteCompras =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            (
                                x.PuedeCotizar
                                ||
                                x.PuedeAdministrar
                            )
                    );

            if (!EsAgenteCompras)
            {
                OrdenesAsignadas =
                    new List<OrdenAsignadaDto>();

                return;
            }

            OrdenesAsignadas =
                await (
                    from solicitud
                        in _context.AdqSolicitudes
                            .AsNoTracking()

                    join estatus
                        in _context.AdqEstatus
                            .AsNoTracking()
                        on solicitud.EstatusId
                        equals estatus.Id

                    join empleado
                        in _context.Empleados
                            .AsNoTracking()
                        on solicitud.EmpleadoSolicitanteId
                        equals empleado.Id
                        into empleadoJoin

                    from empleado
                        in empleadoJoin.DefaultIfEmpty()

                    join area
                        in _context.Areas
                            .AsNoTracking()
                        on solicitud.AreaId
                        equals area.Id
                        into areaJoin

                    from area
                        in areaJoin.DefaultIfEmpty()

                    where
                        solicitud.UsuarioAsignadoId ==
                            usuarioActual.Id
                        &&
                        !solicitud.Eliminado
                        &&
                        solicitud.EstatusId >= 8

                    orderby
                        solicitud.FechaModificacion descending,
                        solicitud.FechaSolicitud descending

                    select new OrdenAsignadaDto
                    {
                        Id =
                            solicitud.Id,

                        Folio =
                            solicitud.Folio,

                        Titulo =
                            solicitud.Titulo,

                        Solicitante =
                            empleado != null
                                ? empleado.NombreCompleto
                                : "Sin información",

                        Area =
                            area != null
                                ? area.Nombre
                                : "Sin área",

                        FechaSolicitud =
                            solicitud.FechaSolicitud,

                        FechaAsignacion =
                            _context.AdqAsignaciones
                                .Where(
                                    x =>
                                        x.SolicitudId ==
                                            solicitud.Id
                                        &&
                                        x.UsuarioAsignadoId ==
                                            usuarioActual.Id
                                        &&
                                        x.Activa
                                )
                                .OrderByDescending(
                                    x =>
                                        x.FechaAsignacion
                                )
                                .Select(
                                    x =>
                                        (DateTime?)
                                        x.FechaAsignacion
                                )
                                .FirstOrDefault(),

                        EstatusId =
                            solicitud.EstatusId,

                        Estatus =
                            estatus.Nombre,

                        MensajesPendientes =
                            0
                    }
                )
                .ToListAsync();
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