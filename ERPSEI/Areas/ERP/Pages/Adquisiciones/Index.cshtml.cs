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
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

        [BindProperty]
        public SolicitudPagoInput InputSolicitudPago
        {
            get;
            set;
        } = new();

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

        private class ResultadoPdfSolicitudPago
        {
            public string NombreArchivo
            {
                get;
                set;
            } = string.Empty;


            public string RutaRelativa
            {
                get;
                set;
            } = string.Empty;


            public string RutaFisica
            {
                get;
                set;
            } = string.Empty;


            public string HashSha256
            {
                get;
                set;
            } = string.Empty;
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
        public List<IFormFile>? ArchivosCotizacion
        {
            get;
            set;
        }

        [BindProperty]
        public List<int> ArchivosCotizacionEliminarIds
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
        // DASHBOARD GENERAL DE ADQUISICIONES - HU #5
        // =========================================================

        public int DashboardTotalOrdenes
        {
            get;
            private set;
        }


        public int DashboardEnCurso
        {
            get;
            private set;
        }


        public int DashboardPendientes
        {
            get;
            private set;
        }


        public int DashboardRechazadas
        {
            get;
            private set;
        }


        public int DashboardCanceladas
        {
            get;
            private set;
        }


        public int DashboardFinalizadas
        {
            get;
            private set;
        }


        public int DashboardEnCotizacion
        {
            get;
            private set;
        }


        public int DashboardEnAprobacionPresupuestal
        {
            get;
            private set;
        }


        public int DashboardSolicitudesPago
        {
            get;
            private set;
        }


        public decimal DashboardPorcentajeFinalizadas
        {
            get;
            private set;
        }


        public List<DashboardEstatusDto> DashboardDistribucionEstatus
        {
            get;
            private set;
        } = new();


        public class DashboardEstatusDto
        {
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


            public int Total
            {
                get;
                set;
            }


            public decimal Porcentaje
            {
                get;
                set;
            }
        }

        // =========================================================
        // INPUT - PERMISOS DE ADQUISICIONES
        // =========================================================

        public class GuardarPermisosAdquisicionesRequest
        {
            public List<PermisoAdquisicionesInput> Permisos
            {
                get;
                set;
            } = new();
        }


        public class PermisoAdquisicionesInput
        {
            public string UsuarioId
            {
                get;
                set;
            } = string.Empty;


            public bool PuedeVisualizar
            {
                get;
                set;
            }


            public bool PuedeCrearSolicitud
            {
                get;
                set;
            }


            public bool PuedeGestionarSolicitudes
            {
                get;
                set;
            }


            public bool PuedeAprobar
            {
                get;
                set;
            }


            public bool PuedeAsignar
            {
                get;
                set;
            }


            public bool PuedeCotizar
            {
                get;
                set;
            }


            public bool PuedeGestionarProveedores
            {
                get;
                set;
            }


            public bool PuedeGenerarSolicitudPago
            {
                get;
                set;
            }


            public bool PuedeVerReportes
            {
                get;
                set;
            }


            public bool PuedeAprobarPresupuesto
            {
                get;
                set;
            }


            public int? NivelPresupuestal
            {
                get;
                set;
            }


            public bool PuedeAdministrar
            {
                get;
                set;
            }
        }

        public class AprobacionPresupuestalPendienteDto
        {
            public int DetalleId
            {
                get;
                set;
            }


            public int SolicitudId
            {
                get;
                set;
            }


            public int AprobacionPresupuestalId
            {
                get;
                set;
            }


            public int Orden
            {
                get;
                set;
            }


            public string NombreEtapa
            {
                get;
                set;
            } = string.Empty;


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


            public string Proveedor
            {
                get;
                set;
            } = string.Empty;


            public decimal Monto
            {
                get;
                set;
            }


            public DateTime FechaSolicitud
            {
                get;
                set;
            }


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


            public string? ComentarioSolicitud
            {
                get;
                set;
            }
        }

        public List<AprobacionPresupuestalPendienteDto>
            AprobacionesPresupuestalesPendientes
        {
            get;
            private set;
        } = new();

        public bool EsAprobadorPresupuestal
        {
            get;
            private set;
        }

        public class ConfiguracionAprobacionPresupuestalDto
        {
            public int Orden
            {
                get;
                set;
            }

            public string TipoEtapa
            {
                get;
                set;
            } = string.Empty;

            public string NombreEtapa
            {
                get;
                set;
            } = string.Empty;

            public string? UsuarioResponsableId
            {
                get;
                set;
            }

            public string? UsuarioResponsableNombre
            {
                get;
                set;
            }

            public string? UsuarioAsistenteId
            {
                get;
                set;
            }

            public string? UsuarioAsistenteNombre
            {
                get;
                set;
            }

            public bool AsistenteRecibeCopia
            {
                get;
                set;
            }

            public int? RecibirCopiaDesdeOrden
            {
                get;
                set;
            }
        }


        public class GuardarConfiguracionAprobacionPresupuestalRequest
        {
            public List<ConfiguracionAprobacionPresupuestalInput> Etapas
            {
                get;
                set;
            } = new();
        }


        public class ConfiguracionAprobacionPresupuestalInput
        {
            public int Orden
            {
                get;
                set;
            }

            public string? UsuarioResponsableId
            {
                get;
                set;
            }

            public string? UsuarioAsistenteId
            {
                get;
                set;
            }

            public bool AsistenteRecibeCopia
            {
                get;
                set;
            }

            public int? RecibirCopiaDesdeOrden
            {
                get;
                set;
            }
        }

        public class SeguimientoPresupuestalDto
        {
            public int AprobacionPresupuestalId
            {
                get;
                set;
            }

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

            public string Proveedor
            {
                get;
                set;
            } = string.Empty;

            public decimal Monto
            {
                get;
                set;
            }

            public string EtapaActual
            {
                get;
                set;
            } = string.Empty;

            public int OrdenEtapaActual
            {
                get;
                set;
            }

            public string NombreOrigen
            {
                get;
                set;
            } = string.Empty;

            public DateTime? FechaActivacion
            {
                get;
                set;
            }

            public DateTime FechaSolicitud
            {
                get;
                set;
            }

            public string EstatusFlujo
            {
                get;
                set;
            } = string.Empty;
        }

        public class HistorialEtapaPresupuestalDto
        {
            public int DetalleId
            {
                get;
                set;
            }


            public int Orden
            {
                get;
                set;
            }


            public string NombreEtapa
            {
                get;
                set;
            } = string.Empty;


            public string Estatus
            {
                get;
                set;
            } = string.Empty;


            public bool EsActual
            {
                get;
                set;
            }


            public string? UsuarioAprobadorId
            {
                get;
                set;
            }


            public string Aprobador
            {
                get;
                set;
            } = string.Empty;


            public string? Comentario
            {
                get;
                set;
            }


            public DateTime? FechaDecision
            {
                get;
                set;
            }


            public bool TieneFirma
            {
                get;
                set;
            }


            public int? FirmaAprobacionId
            {
                get;
                set;
            }


            public string? NombreFirmante
            {
                get;
                set;
            }


            public string? EmailFirmante
            {
                get;
                set;
            }


            public string? TipoFirma
            {
                get;
                set;
            }


            public string? DecisionFirma
            {
                get;
                set;
            }


            public DateTime? FechaFirma
            {
                get;
                set;
            }


            public string? DireccionIp
            {
                get;
                set;
            }


            public string? HashFirma
            {
                get;
                set;
            }


            public string? HashContextoFirmado
            {
                get;
                set;
            }


            public string? RutaFirma
            {
                get;
                set;
            }
        }

        public class FirmaUsuarioDto
        {
            public int Id { get; set; }

            public string NombreFirma { get; set; } =
                string.Empty;

            public string TipoFirma { get; set; } =
                string.Empty;

            public string RutaArchivo { get; set; } =
                string.Empty;

            public bool EsPredeterminada { get; set; }

            public int TotalUsos { get; set; }

            public DateTime? FechaUltimoUso { get; set; }

            public DateTime FechaCreacion { get; set; }
        }


        public class ConfigurarPinFirmaRequest
        {
            [Required]
            [StringLength(
                20,
                MinimumLength = 4
            )]
            public string Pin
            {
                get;
                set;
            } = string.Empty;


            [Required]
            public string ConfirmarPin
            {
                get;
                set;
            } = string.Empty;
        }


        public class GuardarFirmaUsuarioRequest
        {
            [Required]
            [StringLength(150)]
            public string NombreFirma
            {
                get;
                set;
            } = string.Empty;


            [Required]
            [StringLength(30)]
            public string TipoFirma
            {
                get;
                set;
            } = string.Empty;


            [Required]
            public string FirmaBase64
            {
                get;
                set;
            } = string.Empty;


            public bool EsPredeterminada
            {
                get;
                set;
            }
        }

        public List<SeguimientoPresupuestalDto>
        SeguimientosPresupuestales
        {
            get;
            private set;
        } = new();

        // =========================================================
        // MIS FIRMAS
        // GET ?handler=MisFirmas
        // =========================================================

        // =========================================================
        // GENERAR PDF FINAL DE SOLICITUD DE PAGO
        // =========================================================

        private async Task<ResultadoPdfSolicitudPago>
            GenerarPdfSolicitudPagoAsync(
                AdqSolicitudPago solicitudPago
            )
        {
            // =====================================================
            // SOLICITUD
            // =====================================================

            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                solicitudPago.SolicitudId
                            &&
                            !x.Eliminado
                    );


            if (
                solicitud ==
                null
            )
            {
                throw new InvalidOperationException(
                    "No fue posible localizar la solicitud."
                );
            }


            // =====================================================
            // ETAPAS
            // =====================================================

            List<AdqAprobacionPresupuestalDetalle> etapas =
                await _context
                    .AdqAprobacionesPresupuestalesDetalle
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.AprobacionPresupuestalId ==
                                solicitudPago.AprobacionPresupuestalId
                            &&
                            !x.Eliminado
                    )
                    .OrderBy(
                        x =>
                            x.Orden
                    )
                    .ToListAsync();


            if (
                etapas.Count !=
                4
                ||
                etapas.Any(
                    x =>
                        x.Estatus !=
                            "Aprobada"
                )
            )
            {
                throw new InvalidOperationException(
                    "El flujo presupuestal no contiene las cuatro aprobaciones requeridas."
                );
            }


            List<int> detalleIds =
                etapas
                    .Select(
                        x =>
                            x.Id
                    )
                    .ToList();


            // =====================================================
            // FIRMAS SNAPSHOT
            // =====================================================

            List<AdqFirmaAprobacionPresupuestal> firmas =
                await _context
                    .AdqFirmasAprobacionesPresupuestales
                    .AsNoTracking()
                    .Where(
                        x =>
                            detalleIds.Contains(
                                x.AprobacionPresupuestalDetalleId
                            )
                            &&
                            !x.Eliminado
                            &&
                            x.Decision ==
                                "APROBAR"
                    )
                    .OrderBy(
                        x =>
                            x.OrdenEtapa
                    )
                    .ToListAsync();


            if (
                firmas.Count !=
                4
            )
            {
                throw new InvalidOperationException(
                    "No se encontraron las cuatro firmas de aprobación."
                );
            }


            // =====================================================
            // CARGAR IMÁGENES DE FIRMA
            // =====================================================

            Dictionary<int, byte[]> imagenesFirma =
                new();


            string raizFirmas =
                Path.GetFullPath(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "App_Data",
                        "Adquisiciones",
                        "FirmasAprobaciones"
                    )
                );


            foreach (
                AdqFirmaAprobacionPresupuestal firma
                in firmas
            )
            {
                if (
                    string.IsNullOrWhiteSpace(
                        firma.RutaFirmaSnapshot
                    )
                )
                {
                    throw new InvalidOperationException(
                        $"La firma del nivel {firma.OrdenEtapa} no tiene evidencia física."
                    );
                }


                string rutaFirma =
                    Path.IsPathRooted(
                        firma.RutaFirmaSnapshot
                    )
                        ? firma.RutaFirmaSnapshot
                        : Path.Combine(
                            Directory.GetCurrentDirectory(),
                            firma.RutaFirmaSnapshot
                                .TrimStart(
                                    Path.DirectorySeparatorChar,
                                    Path.AltDirectorySeparatorChar
                                )
                        );


                rutaFirma =
                    Path.GetFullPath(
                        rutaFirma
                    );


                if (
                    !rutaFirma.StartsWith(
                        raizFirmas,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    throw new InvalidOperationException(
                        "Se detectó una ruta de firma inválida."
                    );
                }


                if (
                    !System.IO.File.Exists(
                        rutaFirma
                    )
                )
                {
                    throw new FileNotFoundException(
                        $"No se encontró físicamente la firma del nivel {firma.OrdenEtapa}."
                    );
                }


                byte[] bytesFirma =
                    await System.IO.File
                        .ReadAllBytesAsync(
                            rutaFirma
                        );


                string hashActual =
                    Convert.ToHexString(
                        SHA256.HashData(
                            bytesFirma
                        )
                    );


                if (
                    !string.Equals(
                        hashActual,
                        firma.HashFirma,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    throw new InvalidOperationException(
                        $"La evidencia de firma del nivel {firma.OrdenEtapa} no superó la validación de integridad."
                    );
                }


                imagenesFirma[
                    firma.OrdenEtapa
                ] =
                    bytesFirma;
            }


            // =====================================================
            // ARCHIVO DE DESTINO
            // =====================================================

            string carpetaRelativa =
                Path.Combine(
                    "App_Data",
                    "Adquisiciones",
                    "SolicitudesPago",
                    solicitud.Id.ToString()
                );


            string carpetaFisica =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    carpetaRelativa
                );


            Directory.CreateDirectory(
                carpetaFisica
            );


            string folioSeguro =
                string.Concat(
                    solicitud.Folio
                        .Select(
                            c =>
                                Path
                                    .GetInvalidFileNameChars()
                                    .Contains(c)
                                    ? '_'
                                    : c
                        )
                );


            string nombreArchivo =
                $"SolicitudPago_{folioSeguro}.pdf";


            string rutaFisica =
                Path.Combine(
                    carpetaFisica,
                    nombreArchivo
                );


            // =====================================================
            // HELPERS DEL DOCUMENTO
            // =====================================================

            string Marca(
                bool seleccionado
            )
            {
                return seleccionado
                    ? "X"
                    : "";
            }


            string Dinero(
                decimal valor
            )
            {
                return valor.ToString(
                    "N2"
                );
            }


            string encabezadoFirma1 =
                "Revisó";

            string encabezadoFirma2 =
                "Revisó";

            string encabezadoFirma3 =
                "Autorizó";

            string encabezadoFirma4 =
                "Vo. Bo.";


            // =====================================================
            // DOCUMENTO QUESTPDF
            // =====================================================

            byte[] pdf =
                Document
                    .Create(
                        container =>
                        {
                            container.Page(
                                page =>
                                {
                                    page.Size(
                                        PageSizes.Letter.Landscape()
                                    );

                                    page.Margin(
                                        22
                                    );

                                    page.DefaultTextStyle(
                                        x =>
                                            x.FontSize(
                                                8
                                            )
                                    );


                                    page.Content()
                                        .Column(
                                            column =>
                                            {
                                                column.Spacing(
                                                    5
                                                );


                                                // =================================
                                                // TÍTULO
                                                // =================================

                                                column.Item()
                                                    .AlignCenter()
                                                    .Text(
                                                        "Solicitud de Pago"
                                                    )
                                                    .Bold()
                                                    .FontSize(
                                                        13
                                                    );


                                                // =================================
                                                // ENCABEZADO
                                                // =================================

                                                column.Item()
                                                    .Table(
                                                        table =>
                                                        {
                                                            table.ColumnsDefinition(
                                                                columns =>
                                                                {
                                                                    columns.RelativeColumn(
                                                                        1
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        3
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        2
                                                                    );
                                                                }
                                                            );


                                                            table.Cell()
                                                                .Text(
                                                                    "Compañía:"
                                                                )
                                                                .Bold();

                                                            table.Cell()
                                                                .BorderBottom(
                                                                    1
                                                                )
                                                                .PaddingHorizontal(
                                                                    4
                                                                )
                                                                .Text(
                                                                    solicitudPago.Compania
                                                                );


                                                            table.Cell()
                                                                .Text(
                                                                    "Moneda:"
                                                                )
                                                                .Bold();

                                                            table.Cell()
                                                                .Row(
                                                                    row =>
                                                                    {
                                                                        row.Spacing(
                                                                            8
                                                                        );

                                                                        row.RelativeItem()
                                                                            .Text(
                                                                                $"Pesos [{Marca(solicitudPago.Moneda == "Pesos")}]"
                                                                            );

                                                                        row.RelativeItem()
                                                                            .Text(
                                                                                $"Dólares [{Marca(solicitudPago.Moneda == "Dolares")}]"
                                                                            );
                                                                    }
                                                                );


                                                            table.Cell()
                                                                .Text(
                                                                    "Área Solicitante:"
                                                                )
                                                                .Bold();

                                                            table.Cell()
                                                                .BorderBottom(
                                                                    1
                                                                )
                                                                .PaddingHorizontal(
                                                                    4
                                                                )
                                                                .Text(
                                                                    solicitudPago.AreaSolicitante
                                                                );


                                                            table.Cell()
                                                                .Text(
                                                                    "Forma de pago:"
                                                                )
                                                                .Bold();

                                                            table.Cell()
                                                                .Row(
                                                                    row =>
                                                                    {
                                                                        row.Spacing(
                                                                            7
                                                                        );

                                                                        row.AutoItem()
                                                                            .Text(
                                                                                $"Transferencia [{Marca(solicitudPago.FormaPago == "Transferencia")}]"
                                                                            );

                                                                        row.AutoItem()
                                                                            .Text(
                                                                                $"Efectivo [{Marca(solicitudPago.FormaPago == "Efectivo")}]"
                                                                            );

                                                                        row.AutoItem()
                                                                            .Text(
                                                                                $"Cheque [{Marca(solicitudPago.FormaPago == "Cheque")}]"
                                                                            );
                                                                    }
                                                                );


                                                            table.Cell()
                                                                .ColumnSpan(
                                                                    2
                                                                )
                                                                .Text(
                                                                    ""
                                                                );


                                                            table.Cell()
                                                                .Text(
                                                                    "Fecha Solicitud:"
                                                                )
                                                                .Bold();

                                                            table.Cell()
                                                                .BorderBottom(
                                                                    1
                                                                )
                                                                .PaddingHorizontal(
                                                                    4
                                                                )
                                                                .Text(
                                                                    solicitudPago
                                                                        .FechaSolicitud
                                                                        .ToString(
                                                                            "dd MMMM yyyy",
                                                                            new System.Globalization.CultureInfo(
                                                                                "es-MX"
                                                                            )
                                                                        )
                                                                        .ToUpperInvariant()
                                                                );
                                                        }
                                                    );


                                                // =================================
                                                // CONCEPTO
                                                // =================================

                                                column.Item()
                                                    .PaddingTop(
                                                        4
                                                    )
                                                    .Row(
                                                        row =>
                                                        {
                                                            row.ConstantItem(
                                                                90
                                                            )
                                                            .Text(
                                                                "Concepto de pago:"
                                                            )
                                                            .Bold();

                                                            row.RelativeItem()
                                                                .BorderBottom(
                                                                    1
                                                                )
                                                                .PaddingBottom(
                                                                    2
                                                                )
                                                                .Text(
                                                                    solicitudPago.ConceptoPago
                                                                );
                                                        }
                                                    );


                                                // =================================
                                                // PROVEEDOR
                                                // =================================

                                                column.Item()
                                                    .PaddingTop(
                                                        4
                                                    )
                                                    .Text(
                                                        "Datos del Proveedor"
                                                    )
                                                    .Bold();


                                                column.Item()
                                                    .Table(
                                                        table =>
                                                        {
                                                            table.ColumnsDefinition(
                                                                columns =>
                                                                {
                                                                    columns.ConstantColumn(
                                                                        30
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        2
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.2f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1.3f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        2.1f
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1
                                                                    );
                                                                }
                                                            );


                                                            string[] headers =
                                                            {
                                                        "",
                                                        "Importe",
                                                        "Nombre / Razón Social",
                                                        "Banco",
                                                        "Cuenta",
                                                        "CLABE Interbancaria",
                                                        "Comp. Adjunto"
                                                            };


                                                            foreach (
                                                                string header
                                                                in headers
                                                            )
                                                            {
                                                                table.Cell()
                                                                    .Border(
                                                                        1
                                                                    )
                                                                    .Padding(
                                                                        3
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        header
                                                                    )
                                                                    .Bold()
                                                                    .FontSize(
                                                                        7
                                                                    );
                                                            }


                                                            table.Cell()
                                                                .Border(
                                                                    1
                                                                )
                                                                .Padding(
                                                                    3
                                                                )
                                                                .AlignCenter()
                                                                .Text(
                                                                    "1"
                                                                );

                                                            table.Cell()
                                                                .Border(
                                                                    1
                                                                )
                                                                .Padding(
                                                                    3
                                                                )
                                                                .Text(
                                                                    $"${Dinero(solicitudPago.Total)}"
                                                                );

                                                            table.Cell()
                                                                .Border(
                                                                    1
                                                                )
                                                                .Padding(
                                                                    3
                                                                )
                                                                .Text(
                                                                    solicitudPago.NombreProveedor
                                                                );

                                                            table.Cell()
                                                                .Border(
                                                                    1
                                                                )
                                                                .Padding(
                                                                    3
                                                                )
                                                                .Text(
                                                                    solicitudPago.Banco
                                                                    ??
                                                                    string.Empty
                                                                );

                                                            table.Cell()
                                                                .Border(
                                                                    1
                                                                )
                                                                .Padding(
                                                                    3
                                                                )
                                                                .Text(
                                                                    solicitudPago.Cuenta
                                                                    ??
                                                                    string.Empty
                                                                );

                                                            table.Cell()
                                                                .Border(
                                                                    1
                                                                )
                                                                .Padding(
                                                                    3
                                                                )
                                                                .Text(
                                                                    solicitudPago.ClabeInterbancaria
                                                                    ??
                                                                    string.Empty
                                                                );

                                                            table.Cell()
                                                                .Border(
                                                                    1
                                                                )
                                                                .Padding(
                                                                    3
                                                                )
                                                                .AlignCenter()
                                                                .Text(
                                                                    solicitudPago.ComprobanteAdjunto
                                                                        ? "SI"
                                                                        : "NO"
                                                                );


                                                            for (
                                                                int fila = 2;
                                                                fila <= 5;
                                                                fila++
                                                            )
                                                            {
                                                                table.Cell()
                                                                    .Border(
                                                                        1
                                                                    )
                                                                    .Height(
                                                                        16
                                                                    )
                                                                    .AlignCenter()
                                                                    .Text(
                                                                        fila.ToString()
                                                                    );

                                                                for (
                                                                    int columna = 0;
                                                                    columna < 6;
                                                                    columna++
                                                                )
                                                                {
                                                                    table.Cell()
                                                                        .Border(
                                                                            1
                                                                        )
                                                                        .Text(
                                                                            ""
                                                                        );
                                                                }
                                                            }
                                                        }
                                                    );


                                                // =================================
                                                // TOTALES
                                                // =================================

                                                column.Item()
                                                    .PaddingTop(
                                                        4
                                                    )
                                                    .Width(
                                                        220
                                                    )
                                                    .Table(
                                                        table =>
                                                        {
                                                            table.ColumnsDefinition(
                                                                columns =>
                                                                {
                                                                    columns.RelativeColumn(
                                                                        2
                                                                    );

                                                                    columns.RelativeColumn(
                                                                        1
                                                                    );
                                                                }
                                                            );


                                                            void Fila(
                                                                string titulo,
                                                                decimal valor,
                                                                bool negrita = false
                                                            )
                                                            {
                                                                IContainer tituloCelda =
                                                                    table.Cell()
                                                                        .PaddingVertical(
                                                                            1
                                                                        );

                                                                IContainer valorCelda =
                                                                    table.Cell()
                                                                        .PaddingVertical(
                                                                            1
                                                                        );


                                                                if (
                                                                    negrita
                                                                )
                                                                {
                                                                    tituloCelda
                                                                        .Text(
                                                                            titulo
                                                                        )
                                                                        .Bold();

                                                                    valorCelda
                                                                        .BorderBottom(
                                                                            1
                                                                        )
                                                                        .Text(
                                                                            $"${Dinero(valor)}"
                                                                        )
                                                                        .Bold();
                                                                }
                                                                else
                                                                {
                                                                    tituloCelda
                                                                        .Text(
                                                                            titulo
                                                                        );

                                                                    valorCelda
                                                                        .Text(
                                                                            valor ==
                                                                            0
                                                                                ? ""
                                                                                : $"${Dinero(valor)}"
                                                                        );
                                                                }
                                                            }


                                                            Fila(
                                                                "Subtotal",
                                                                solicitudPago.Subtotal,
                                                                true
                                                            );

                                                            Fila(
                                                                "I.V.A.",
                                                                solicitudPago.Iva
                                                            );

                                                            Fila(
                                                                "Retención I.V.A.",
                                                                solicitudPago.RetencionIva
                                                            );

                                                            Fila(
                                                                "Retención I.S.R.",
                                                                solicitudPago.RetencionIsr
                                                            );

                                                            Fila(
                                                                "Otros Impuestos",
                                                                solicitudPago.OtrosImpuestos
                                                            );

                                                            Fila(
                                                                "Otros Servicios",
                                                                solicitudPago.OtrosServicios
                                                            );

                                                            Fila(
                                                                "Total",
                                                                solicitudPago.Total,
                                                                true
                                                            );
                                                        }
                                                    );


                                                // =================================
                                                // DOCUMENTOS ANEXOS
                                                // =================================

                                                column.Item()
                                                    .PaddingTop(
                                                        4
                                                    )
                                                    .BorderBottom(
                                                        1
                                                    )
                                                    .PaddingBottom(
                                                        3
                                                    )
                                                    .Text(
                                                        "Documentos Anexos:"
                                                    )
                                                    .Bold();


                                                column.Item()
                                                    .Row(
                                                        row =>
                                                        {
                                                            row.RelativeItem()
                                                                .AlignCenter()
                                                                .Text(
                                                                    $"Factura: [{Marca(solicitudPago.TipoDocumentoSolicitud == "Factura")}]"
                                                                );

                                                            row.RelativeItem()
                                                                .AlignCenter()
                                                                .Text(
                                                                    $"Contrato: [{Marca(solicitudPago.TipoDocumentoSolicitud == "Contrato")}]"
                                                                );

                                                            row.RelativeItem()
                                                                .AlignCenter()
                                                                .Text(
                                                                    $"Cotizaciones: [{Marca(solicitudPago.TipoDocumentoSolicitud == "Cotizaciones")}]"
                                                                );

                                                            row.RelativeItem()
                                                                .AlignCenter()
                                                                .Text(
                                                                    $"Otros: [{Marca(solicitudPago.TipoDocumentoSolicitud == "Otros")}]"
                                                                );
                                                        }
                                                    );


                                                // =================================
                                                // FIRMAS
                                                // =================================

                                                column.Item()
                                                    .PaddingTop(
                                                        8
                                                    )
                                                    .Row(
                                                        row =>
                                                        {
                                                            for (
                                                                int orden = 1;
                                                                orden <= 4;
                                                                orden++
                                                            )
                                                            {
                                                                int ordenActual =
                                                                    orden;


                                                                AdqFirmaAprobacionPresupuestal firma =
                                                                    firmas.First(
                                                                        x =>
                                                                            x.OrdenEtapa ==
                                                                            ordenActual
                                                                    );


                                                                string encabezado =
                                                                    ordenActual switch
                                                                    {
                                                                        1 =>
                                                                            encabezadoFirma1,

                                                                        2 =>
                                                                            encabezadoFirma2,

                                                                        3 =>
                                                                            encabezadoFirma3,

                                                                        _ =>
                                                                            encabezadoFirma4
                                                                    };


                                                                row.RelativeItem()
                                                                    .PaddingHorizontal(
                                                                        8
                                                                    )
                                                                    .Column(
                                                                        firmaColumn =>
                                                                        {
                                                                            firmaColumn.Item()
                                                                                .AlignCenter()
                                                                                .Text(
                                                                                    encabezado
                                                                                )
                                                                                .FontSize(
                                                                                    7
                                                                                );


                                                                            firmaColumn.Item()
                                                                                .Height(
                                                                                    38
                                                                                )
                                                                                .AlignCenter()
                                                                                .AlignMiddle()
                                                                                .Image(
                                                                                    imagenesFirma[
                                                                                        ordenActual
                                                                                    ]
                                                                                )
                                                                                .FitArea();


                                                                            firmaColumn.Item()
                                                                                .BorderBottom(
                                                                                    1
                                                                                )
                                                                                .PaddingBottom(
                                                                                    2
                                                                                );


                                                                            firmaColumn.Item()
                                                                                .PaddingTop(
                                                                                    2
                                                                                )
                                                                                .AlignCenter()
                                                                                .Text(
                                                                                    firma.NombreFirmante
                                                                                )
                                                                                .Bold()
                                                                                .FontSize(
                                                                                    7
                                                                                );


                                                                            firmaColumn.Item()
                                                                                .AlignCenter()
                                                                                .Text(
                                                                                    firma.NombreEtapa
                                                                                )
                                                                                .FontSize(
                                                                                    6
                                                                                );
                                                                        }
                                                                    );
                                                            }
                                                        }
                                                    );
                                            }
                                        );
                                }
                            );
                        }
                    )
                    .GeneratePdf();


            // =====================================================
            // GUARDAR ARCHIVO
            // =====================================================

            await System.IO.File.WriteAllBytesAsync(
                rutaFisica,
                pdf
            );


            string hashPdf =
                Convert.ToHexString(
                    SHA256.HashData(
                        pdf
                    )
                );


            return new ResultadoPdfSolicitudPago
            {
                NombreArchivo =
                    nombreArchivo,

                RutaFisica =
                    rutaFisica,

                RutaRelativa =
                    carpetaRelativa,

                HashSha256 =
                    hashPdf
            };
        }

        public async Task<IActionResult>
            OnGetMisFirmasAsync()
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
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


            bool tienePin =
                await _context.AdqSeguridadFirmaUsuario
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.Activo
                            &&
                            !x.Eliminado
                    );


            List<FirmaUsuarioDto> firmas =
                await _context.AdqFirmasUsuario
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.Activa
                            &&
                            !x.Eliminado
                    )
                    .OrderByDescending(
                        x =>
                            x.EsPredeterminada
                    )
                    .ThenByDescending(
                        x =>
                            x.FechaUltimoUso
                    )
                    .ThenByDescending(
                        x =>
                            x.FechaCreacion
                    )
                    .Select(
                        x =>
                            new FirmaUsuarioDto
                            {
                                Id =
                                    x.Id,

                                NombreFirma =
                                    x.NombreFirma,

                                TipoFirma =
                                    x.TipoFirma,

                                RutaArchivo =
                                    $"/ERP/Adquisiciones/Index?handler=ImagenFirma&firmaId={x.Id}",

                                EsPredeterminada =
                                    x.EsPredeterminada,

                                TotalUsos =
                                    x.TotalUsos,

                                FechaUltimoUso =
                                    x.FechaUltimoUso,

                                FechaCreacion =
                                    x.FechaCreacion
                            }
                    )
                    .ToListAsync();


            List<FirmaUsuarioDto> ultimasFirmas =
                firmas
                    .Where(
                        x =>
                            x.FechaUltimoUso.HasValue
                    )
                    .OrderByDescending(
                        x =>
                            x.FechaUltimoUso
                    )
                    .Take(
                        3
                    )
                    .ToList();


            /*
             * Si todavía nunca ha usado ninguna,
             * mostramos hasta las primeras tres disponibles.
             */
            if (
                ultimasFirmas.Count ==
                0
            )
            {
                ultimasFirmas =
                    firmas
                        .Take(
                            3
                        )
                        .ToList();
            }

            Empleado? empleadoActual =
            await ObtenerEmpleadoActualAsync(
                usuarioActual
            );


            string nombreUsuario =
                empleadoActual?.NombreCompleto
                ??
                usuarioActual.UserName
                ??
                usuarioActual.Email
                ??
                "Usuario";


            return new JsonResult(
                new
                {
                    success = true,

                    tienePin,

                    nombreUsuario,

                    totalFirmas =
                        firmas.Count,

                    firmas,

                    ultimasFirmas
                }
            );
        }

        // =========================================================
        // DATOS PARA SOLICITUD DE PAGO
        // GET ?handler=DatosSolicitudPago&solicitudId=1
        // =========================================================

        public async Task<IActionResult>
            OnGetDatosSolicitudPagoAsync(
                int solicitudId
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
            {
                return Unauthorized();
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                solicitudId
                            &&
                            !x.Eliminado
                    );


            if (
                solicitud ==
                null
            )
            {
                return NotFound();
            }


            if (
                solicitud.EstatusId <
                13
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud aún no ha completado la aprobación presupuestal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            AdqAprobacionPresupuestal? aprobacion =
                await _context
                    .AdqAprobacionesPresupuestales
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.SolicitudId ==
                                solicitudId
                            &&
                            !x.Eliminado
                            &&
                            x.Estatus ==
                                "Aprobada"
                    )
                    .OrderByDescending(
                        x =>
                            x.Id
                    )
                    .FirstOrDefaultAsync();


            if (
                aprobacion ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró una aprobación presupuestal finalizada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            AdqCotizacion? cotizacion =
                await _context.AdqCotizaciones
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                aprobacion.CotizacionId
                            &&
                            !x.Eliminado
                    );


            if (
                cotizacion ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró la cotización seleccionada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            string area =
                await _context.Areas
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.Id ==
                                solicitud.AreaId
                    )
                    .Select(
                        x =>
                            x.Nombre
                    )
                    .FirstOrDefaultAsync()
                ??
                string.Empty;


            AdqSolicitudPago? solicitudPagoExistente =
                await _context.AdqSolicitudesPago
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.SolicitudId ==
                                solicitudId
                            &&
                            !x.Eliminado
                    );


            return new JsonResult(
                new
                {
                    success = true,

                    solicitudId =
                        solicitud.Id,

                    folio =
                        solicitud.Folio,

                    area,

                    proveedor =
                        cotizacion.NombreProveedor,

                    subtotal =
                        cotizacion.Subtotal,

                    iva =
                        cotizacion.ImporteIva,

                    total =
                        cotizacion.Total,

                    tipoDocumento =
                        solicitud.TipoDocumentoSolicitud,

                    compania =
                        solicitudPagoExistente?.Compania
                        ??
                        string.Empty,

                    moneda =
                        solicitudPagoExistente?.Moneda
                        ??
                        "Pesos",

                    formaPago =
                        solicitudPagoExistente?.FormaPago
                        ??
                        "Transferencia",

                    conceptoPago =
                        solicitudPagoExistente?.ConceptoPago
                        ??
                        solicitud.Descripcion
                        ??
                        solicitud.Titulo,

                    banco =
                        solicitudPagoExistente?.Banco
                        ??
                        string.Empty,

                    cuenta =
                        solicitudPagoExistente?.Cuenta
                        ??
                        string.Empty,

                    clabeInterbancaria =
                        solicitudPagoExistente?.ClabeInterbancaria
                        ??
                        string.Empty,

                    comprobanteAdjunto =
                        solicitudPagoExistente?.ComprobanteAdjunto
                        ??
                        false,

                    retencionIva =
                        solicitudPagoExistente?.RetencionIva
                        ??
                        0m,

                    retencionIsr =
                        solicitudPagoExistente?.RetencionIsr
                        ??
                        0m,

                    otrosImpuestos =
                        solicitudPagoExistente?.OtrosImpuestos
                        ??
                        0m,

                    otrosServicios =
                        solicitudPagoExistente?.OtrosServicios
                        ??
                        0m,

                    pdfGenerado =
                        solicitudPagoExistente?.PdfGenerado
                        ??
                        false,

                    nombreArchivo =
                        solicitudPagoExistente?.NombreArchivo
                        ??
                        string.Empty,

                    descargarUrl =
                        solicitudPagoExistente?.PdfGenerado == true
                            ? $"{Request.Path}?handler=DescargarSolicitudPago&solicitudId={solicitudId}"
                            : string.Empty
                }
            );
        }

        // =========================================================
        // CONFIGURAR PIN DE FIRMA
        // EL USUARIO SOLO PUEDE CONFIGURARLO UNA VEZ.
        // PARA CAMBIARLO, UN ADMINISTRADOR DEBE RESTABLECERLO.
        // =========================================================

        public async Task<IActionResult>
            OnPostConfigurarPinFirmaAsync(
                [FromBody]
        ConfigurarPinFirmaRequest request
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
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


            if (
                request ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La información del PIN es obligatoria."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            string pin =
                request.Pin?
                    .Trim()
                ??
                string.Empty;


            string confirmarPin =
                request.ConfirmarPin?
                    .Trim()
                ??
                string.Empty;


            if (
                pin.Length <
                4
                ||
                pin.Length >
                20
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El PIN debe contener entre 4 y 20 caracteres."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                pin !=
                confirmarPin
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El PIN y su confirmación no coinciden."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            AdqSeguridadFirmaUsuario? seguridad =
                await _context
                    .AdqSeguridadFirmaUsuario
                    .FirstOrDefaultAsync(
                        x =>
                            x.UsuarioId ==
                            usuarioActual.Id
                    );


            /*
             * Si existe y está activo, significa que el usuario
             * ya configuró su PIN.
             *
             * NO permitimos modificarlo desde Mis firmas.
             */
            if (
                seguridad !=
                null
                &&
                seguridad.Activo
                &&
                !seguridad.Eliminado
                &&
                !string.IsNullOrWhiteSpace(
                    seguridad.PinHash
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        pinYaConfigurado = true,

                        message =
                            "Tu PIN de firma ya se encuentra configurado. " +
                            "Por seguridad no puede modificarse directamente. " +
                            "Si necesitas cambiarlo, solicita al administrador del sistema que lo restablezca."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            DateTime ahora =
                DateTime.Now;


            if (
                seguridad ==
                null
            )
            {
                seguridad =
                    new AdqSeguridadFirmaUsuario
                    {
                        UsuarioId =
                            usuarioActual.Id,

                        FechaConfiguracion =
                            ahora,

                        IntentosFallidos =
                            0,

                        BloqueadoHasta =
                            null,

                        Activo =
                            true,

                        Eliminado =
                            false
                    };


                _context
                    .AdqSeguridadFirmaUsuario
                    .Add(
                        seguridad
                    );
            }
            else
            {
                /*
                 * Registro previamente restablecido por administrador.
                 * Se reutiliza por el índice único UsuarioId.
                 */

                seguridad.FechaConfiguracion =
                    ahora;

                seguridad.FechaModificacion =
                    ahora;

                seguridad.IntentosFallidos =
                    0;

                seguridad.BloqueadoHasta =
                    null;

                seguridad.Activo =
                    true;

                seguridad.Eliminado =
                    false;
            }


            PasswordHasher<
                AdqSeguridadFirmaUsuario
            > hasher =
                new();


            seguridad.PinHash =
                hasher.HashPassword(
                    seguridad,
                    pin
                );


            await _context.SaveChangesAsync();


            return new JsonResult(
                new
                {
                    success = true,

                    message =
                        "Tu PIN de firma fue configurado correctamente. " +
                        "Por seguridad, después de este momento solo un administrador podrá restablecerlo."
                }
            );
        }

        // =========================================================
        // GUARDAR DATOS DE SOLICITUD DE PAGO
        // POST ?handler=GuardarSolicitudPago
        // =========================================================

        public async Task<IActionResult>
            OnPostGuardarSolicitudPagoAsync(
                [FromBody]
        SolicitudPagoInput input
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
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


            if (
                input ==
                null
                ||
                input.SolicitudId <=
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


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                input.SolicitudId
                            &&
                            !x.Eliminado
                    );


            if (
                solicitud ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud ya no se encuentra disponible."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // SOLO SOLICITANTE ORIGINAL
            // =====================================================

            if (
                solicitud.UsuarioSolicitanteId !=
                usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Solamente el solicitante original puede generar la solicitud de pago."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // VALIDAR ESTATUS
            // =====================================================

            if (
                solicitud.EstatusId !=
                13
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La aprobación presupuestal todavía no se encuentra completada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // VALIDACIONES
            // =====================================================

            string compania =
                input.Compania?
                    .Trim()
                ??
                string.Empty;


            string conceptoPago =
                input.ConceptoPago?
                    .Trim()
                ??
                string.Empty;


            string moneda =
                input.Moneda?
                    .Trim()
                ??
                string.Empty;


            string formaPago =
                input.FormaPago?
                    .Trim()
                ??
                string.Empty;


            if (
                string.IsNullOrWhiteSpace(
                    compania
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La compañía es obligatoria."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                string.IsNullOrWhiteSpace(
                    conceptoPago
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El concepto de pago es obligatorio."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                moneda !=
                    "Pesos"
                &&
                moneda !=
                    "Dolares"
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La moneda seleccionada no es válida."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                formaPago !=
                    "Transferencia"
                &&
                formaPago !=
                    "Efectivo"
                &&
                formaPago !=
                    "Cheque"
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La forma de pago seleccionada no es válida."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // APROBACIÓN PRESUPUESTAL
            // =====================================================

            AdqAprobacionPresupuestal? aprobacion =
                await _context
                    .AdqAprobacionesPresupuestales
                    .Where(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                            &&
                            !x.Eliminado
                            &&
                            x.Estatus ==
                                "Aprobada"
                    )
                    .OrderByDescending(
                        x =>
                            x.Id
                    )
                    .FirstOrDefaultAsync();


            if (
                aprobacion ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró una aprobación presupuestal finalizada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // VALIDAR 4 ETAPAS APROBADAS
            // =====================================================

            List<AdqAprobacionPresupuestalDetalle> etapas =
                await _context
                    .AdqAprobacionesPresupuestalesDetalle
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.AprobacionPresupuestalId ==
                                aprobacion.Id
                            &&
                            !x.Eliminado
                    )
                    .OrderBy(
                        x =>
                            x.Orden
                    )
                    .ToListAsync();


            if (
                etapas.Count !=
                4
                ||
                etapas.Any(
                    x =>
                        x.Estatus !=
                            "Aprobada"
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Las cuatro etapas presupuestales deben estar aprobadas."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // VALIDAR 4 FIRMAS
            // =====================================================

            List<int> detalleIds =
                etapas
                    .Select(
                        x =>
                            x.Id
                    )
                    .ToList();


            int totalFirmas =
                await _context
                    .AdqFirmasAprobacionesPresupuestales
                    .AsNoTracking()
                    .CountAsync(
                        x =>
                            detalleIds.Contains(
                                x.AprobacionPresupuestalDetalleId
                            )
                            &&
                            !x.Eliminado
                            &&
                            x.Decision ==
                                "APROBAR"
                    );


            if (
                totalFirmas !=
                4
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontraron las cuatro evidencias de firma."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // COTIZACIÓN
            // =====================================================

            AdqCotizacion? cotizacion =
                await _context.AdqCotizaciones
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                aprobacion.CotizacionId
                            &&
                            !x.Eliminado
                    );


            if (
                cotizacion ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró la cotización seleccionada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            string area =
                await _context.Areas
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.Id ==
                                solicitud.AreaId
                    )
                    .Select(
                        x =>
                            x.Nombre
                    )
                    .FirstOrDefaultAsync()
                ??
                string.Empty;


            DateTime ahora =
                DateTime.Now;


            // =====================================================
            // TOTAL FINAL
            // =====================================================

            decimal totalFinal =
                cotizacion.Subtotal
                +
                cotizacion.ImporteIva
                -
                input.RetencionIva
                -
                input.RetencionIsr
                +
                input.OtrosImpuestos
                +
                input.OtrosServicios;


            totalFinal =
                decimal.Round(
                    totalFinal,
                    2,
                    MidpointRounding.AwayFromZero
                );


            if (
                totalFinal <
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El total resultante de la solicitud de pago no puede ser negativo."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // CREAR / ACTUALIZAR
            // =====================================================

            AdqSolicitudPago? solicitudPago =
                await _context.AdqSolicitudesPago
                    .FirstOrDefaultAsync(
                        x =>
                            x.SolicitudId ==
                                solicitud.Id
                    );

            // =====================================================
            // DOCUMENTO OFICIAL YA GENERADO
            // =====================================================

            if (
                solicitudPago != null
                &&
                !solicitudPago.Eliminado
                &&
                solicitudPago.PdfGenerado
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "La Solicitud de Pago ya fue generada y se encuentra cerrada para edición.",

                        pdfGenerado = true,

                        nombreArchivo =
                            solicitudPago.NombreArchivo,

                        descargarUrl =
                            $"{Request.Path}?handler=DescargarSolicitudPago&solicitudId={solicitud.Id}"
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            if (
                solicitudPago ==
                null
            )
            {
                solicitudPago =
                    new AdqSolicitudPago
                    {
                        SolicitudId =
                            solicitud.Id,

                        AprobacionPresupuestalId =
                            aprobacion.Id,

                        CotizacionId =
                            cotizacion.Id,

                        FechaSolicitud =
                            solicitud.FechaSolicitud,

                        FechaGeneracion =
                            ahora,

                        UsuarioGeneracionId =
                            usuarioActual.Id,

                        PdfGenerado =
                            false,

                        Eliminado =
                            false
                    };


                _context.AdqSolicitudesPago.Add(
                    solicitudPago
                );
            }


            solicitudPago.Compania =
                compania;

            solicitudPago.AreaSolicitante =
                area;

            solicitudPago.Moneda =
                moneda;

            solicitudPago.FormaPago =
                formaPago;

            solicitudPago.ConceptoPago =
                conceptoPago;

            solicitudPago.NombreProveedor =
                cotizacion.NombreProveedor;

            solicitudPago.Banco =
                input.Banco?
                    .Trim();

            solicitudPago.Cuenta =
                input.Cuenta?
                    .Trim();

            solicitudPago.ClabeInterbancaria =
                input.ClabeInterbancaria?
                    .Trim();

            solicitudPago.ComprobanteAdjunto =
                input.ComprobanteAdjunto;

            solicitudPago.Subtotal =
                cotizacion.Subtotal;

            solicitudPago.Iva =
                cotizacion.ImporteIva;

            solicitudPago.RetencionIva =
                input.RetencionIva;

            solicitudPago.RetencionIsr =
                input.RetencionIsr;

            solicitudPago.OtrosImpuestos =
                input.OtrosImpuestos;

            solicitudPago.OtrosServicios =
                input.OtrosServicios;

            solicitudPago.Total =
                totalFinal;

            solicitudPago.TipoDocumentoSolicitud =
                solicitud.TipoDocumentoSolicitud;

            solicitudPago.FechaGeneracion =
                ahora;

            solicitudPago.UsuarioGeneracionId =
                usuarioActual.Id;


            await _context
    .SaveChangesAsync();


            try
            {
                ResultadoPdfSolicitudPago resultadoPdf =
                    await GenerarPdfSolicitudPagoAsync(
                        solicitudPago
                    );


                solicitudPago.NombreArchivo =
                    resultadoPdf.NombreArchivo;


                solicitudPago.RutaArchivo =
                    Path.Combine(
                        resultadoPdf.RutaRelativa,
                        resultadoPdf.NombreArchivo
                    );


                solicitudPago.HashArchivo =
                    resultadoPdf.HashSha256;


                solicitudPago.PdfGenerado =
    true;


                DateTime fechaGeneracionPdf =
                    DateTime.Now;


                solicitudPago.FechaGeneracion =
                    fechaGeneracionPdf;


                // =====================================================
                // HISTORIAL - SOLICITUD DE PAGO GENERADA
                // =====================================================

                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        TipoEvento =
                            "SOLICITUD_PAGO_GENERADA",

                        Descripcion =
                            "La Solicitud de Pago fue generada correctamente.",

                        UsuarioId =
                            usuarioActual.Id,

                        EstatusAnteriorId =
                            solicitud.EstatusId,

                        EstatusNuevoId =
                            solicitud.EstatusId,

                        FechaEvento =
                            fechaGeneracionPdf,

                        DireccionIp =
                            HttpContext
                                .Connection
                                .RemoteIpAddress?
                                .ToString()
                    }
                );


                await _context
                    .SaveChangesAsync();


                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            "La Solicitud de Pago se generó correctamente.",

                        solicitudPagoId =
                            solicitudPago.Id,

                        solicitudId =
                            solicitud.Id,

                        estatusId =
                            solicitud.EstatusId,

                        total =
                            solicitudPago.Total,

                        pdfGenerado =
                            true,

                        nombreArchivo =
                            solicitudPago.NombreArchivo,

                        descargarUrl =
                            $"{Request.Path}?handler=DescargarSolicitudPago&solicitudId={solicitudPago.SolicitudId}"
                    }
                );
            }
            catch (
                Exception ex
            )
            {
                _logger.LogError(
                    ex,
                    "Error al generar la Solicitud de Pago de la solicitud {SolicitudId}.",
                    solicitudPago.SolicitudId
                );


                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "Los datos se guardaron, pero no fue posible generar el PDF. " +
                            ex.Message
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }

        // =========================================================
        // DESCARGAR SOLICITUD DE PAGO
        // GET ?handler=DescargarSolicitudPago&solicitudId=12
        // =========================================================

        public async Task<IActionResult>
            OnGetDescargarSolicitudPagoAsync(
                int solicitudId
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
            {
                return Unauthorized();
            }


            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                solicitudId
                            &&
                            !x.Eliminado
                    );


            if (
                solicitud ==
                null
            )
            {
                return NotFound();
            }


            bool puedeDescargar =
                solicitud.UsuarioSolicitanteId ==
                    usuarioActual.Id;


            if (
                !puedeDescargar
            )
            {
                puedeDescargar =
                    await _context
                        .AdqPermisosUsuarios
                        .AsNoTracking()
                        .AnyAsync(
                            x =>
                                x.UsuarioId ==
                                    usuarioActual.Id
                                &&
                                (
                                    x.PuedeAdministrar
                                    ||
                                    x.PuedeGenerarSolicitudPago
                                )
                        );
            }


            if (
                !puedeDescargar
            )
            {
                return Forbid();
            }


            AdqSolicitudPago? solicitudPago =
                await _context.AdqSolicitudesPago
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.SolicitudId ==
                                solicitudId
                            &&
                            !x.Eliminado
                            &&
                            x.PdfGenerado
                    );


            if (
                solicitudPago ==
                null
                ||
                string.IsNullOrWhiteSpace(
                    solicitudPago.RutaArchivo
                )
                ||
                string.IsNullOrWhiteSpace(
                    solicitudPago.HashArchivo
                )
            )
            {
                return NotFound();
            }


            string rutaBase =
                Path.GetFullPath(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "App_Data",
                        "Adquisiciones",
                        "SolicitudesPago"
                    )
                );


            string rutaFisica =
                Path.GetFullPath(
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        solicitudPago.RutaArchivo
                    )
                );


            if (
                !rutaFisica.StartsWith(
                    rutaBase,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return BadRequest();
            }


            if (
                !System.IO.File.Exists(
                    rutaFisica
                )
            )
            {
                return NotFound();
            }


            byte[] archivo =
                await System.IO.File
                    .ReadAllBytesAsync(
                        rutaFisica
                    );


            string hashActual =
                Convert.ToHexString(
                    SHA256.HashData(
                        archivo
                    )
                );


            if (
                !string.Equals(
                    hashActual,
                    solicitudPago.HashArchivo,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El documento no superó la validación de integridad."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            Response.Headers.CacheControl =
                "no-store, no-cache";

            Response.Headers.Pragma =
                "no-cache";


            return File(
                archivo,
                "application/pdf",
                solicitudPago.NombreArchivo
                ??
                $"SolicitudPago_{solicitud.Folio}.pdf"
            );
        }

        // =========================================================
        // RESTABLECER PIN DE FIRMA - ADMINISTRADOR
        // POST ?handler=RestablecerPinFirmaUsuario
        // =========================================================

        public async Task<IActionResult>
            OnPostRestablecerPinFirmaUsuarioAsync(
                string usuarioId
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
            {
                return Unauthorized();
            }


            bool puedeAdministrar =
                await _context
                    .AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.PuedeAdministrar
                    );


            if (
                !puedeAdministrar
            )
            {
                return Forbid();
            }


            if (
                string.IsNullOrWhiteSpace(
                    usuarioId
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se identificó al usuario."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            AdqSeguridadFirmaUsuario? seguridad =
                await _context
                    .AdqSeguridadFirmaUsuario
                    .FirstOrDefaultAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioId
                    );


            if (
                seguridad ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El usuario todavía no tiene un PIN de firma configurado."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            seguridad.Activo =
                false;

            seguridad.Eliminado =
                true;

            seguridad.IntentosFallidos =
                0;

            seguridad.BloqueadoHasta =
                null;

            seguridad.FechaModificacion =
                DateTime.Now;


            await _context.SaveChangesAsync();


            return new JsonResult(
                new
                {
                    success = true,

                    message =
                        "El PIN fue restablecido. El usuario deberá configurar uno nuevo la próxima vez que ingrese a Mis firmas."
                }
            );
        }


        // =========================================================
        // GUARDAR FIRMA DEL USUARIO
        // POST ?handler=GuardarFirmaUsuario
        // =========================================================

        public async Task<IActionResult>
            OnPostGuardarFirmaUsuarioAsync(
                [FromBody]
        GuardarFirmaUsuarioRequest request
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
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


            if (
                request ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La información de la firma es obligatoria."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            string nombreFirma =
                request.NombreFirma?
                    .Trim()
                ??
                string.Empty;


            string tipoFirma =
                request.TipoFirma?
                    .Trim()
                ??
                string.Empty;


            if (
                string.IsNullOrWhiteSpace(
                    nombreFirma
                )
                ||
                nombreFirma.Length >
                150
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Debes proporcionar un nombre válido para la firma."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            string[] tiposPermitidos =
            {
        "Dibujada",
        "Archivo",
        "Tipografica"
    };


            if (
                !tiposPermitidos.Contains(
                    tipoFirma,
                    StringComparer.OrdinalIgnoreCase
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El tipo de firma seleccionado no es válido."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            /*
             * Normalizamos la escritura para que en BD siempre
             * tengamos exactamente estos valores.
             */
            tipoFirma =
                tipoFirma.ToLowerInvariant()
                switch
                {
                    "dibujada" =>
                        "Dibujada",

                    "archivo" =>
                        "Archivo",

                    "tipografica" =>
                        "Tipografica",

                    _ =>
                        tipoFirma
                };


            byte[] imagenFirma;


            try
            {
                imagenFirma =
                    ConvertirFirmaBase64APng(
                        request.FirmaBase64
                    );
            }
            catch (
                InvalidOperationException ex
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            ex.Message
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // Máximo 2 MB por firma.

            const int tamanoMaximoFirma =
                2 * 1024 * 1024;


            if (
                imagenFirma.Length >
                tamanoMaximoFirma
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La firma no puede superar los 2 MB."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            string carpetaFirmas =
                Path.Combine(
                    _environment.ContentRootPath,
                    "App_Data",
                    "Adquisiciones",
                    "Firmas"
                );


            Directory.CreateDirectory(
                carpetaFirmas
            );


            string nombreArchivo =
                $"{Guid.NewGuid():N}.png";


            string rutaFisica =
                Path.Combine(
                    carpetaFirmas,
                    nombreArchivo
                );

            if (
                System.IO.File.Exists(
                    rutaFisica
                )
            )
            {
                throw new InvalidOperationException(
                    "Ya existe un documento oficial de Solicitud de Pago para esta solicitud."
                );
            }

            /*
             * La ruta almacenada NO es pública.
             * El archivo solamente podrá obtenerse mediante
             * el handler ImagenFirma.
             */

            string rutaRelativa =
                Path.Combine(
                    "App_Data",
                    "Adquisiciones",
                    "Firmas",
                    nombreArchivo
                )
                .Replace(
                    "\\",
                    "/"
                );


            try
            {
                await System.IO.File.WriteAllBytesAsync(
                    rutaFisica,
                    imagenFirma
                );


                string hashArchivo =
                    Convert.ToHexString(
                        SHA256.HashData(
                            imagenFirma
                        )
                    );


                bool tieneFirmas =
                    await _context.AdqFirmasUsuario
                        .AnyAsync(
                            x =>
                                x.UsuarioId ==
                                    usuarioActual.Id
                                &&
                                x.Activa
                                &&
                                !x.Eliminado
                        );


                /*
                 * La primera firma siempre será predeterminada.
                 */

                bool hacerPredeterminada =
                    request.EsPredeterminada
                    ||
                    !tieneFirmas;


                if (
                    hacerPredeterminada
                )
                {
                    List<AdqFirmaUsuario>
                        firmasPredeterminadas =
                            await _context
                                .AdqFirmasUsuario
                                .Where(
                                    x =>
                                        x.UsuarioId ==
                                            usuarioActual.Id
                                        &&
                                        x.EsPredeterminada
                                )
                                .ToListAsync();


                    foreach (
                        AdqFirmaUsuario firmaAnterior
                        in firmasPredeterminadas
                    )
                    {
                        firmaAnterior.EsPredeterminada =
                            false;
                    }
                }


                AdqFirmaUsuario firma =
                    new()
                    {
                        UsuarioId =
                            usuarioActual.Id,

                        NombreFirma =
                            nombreFirma,

                        TipoFirma =
                            tipoFirma,

                        RutaArchivo =
                            rutaRelativa,

                        HashArchivo =
                            hashArchivo,

                        EsPredeterminada =
                            hacerPredeterminada,

                        TotalUsos =
                            0,

                        FechaUltimoUso =
                            null,

                        FechaCreacion =
                            DateTime.Now,

                        Activa =
                            true,

                        Eliminado =
                            false
                    };


                _context.AdqFirmasUsuario.Add(
                    firma
                );


                await _context.SaveChangesAsync();


                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                            "La firma fue guardada correctamente.",

                        firma =
                            new
                            {
                                firma.Id,

                                firma.NombreFirma,

                                firma.TipoFirma,

                                firma.EsPredeterminada,

                                firma.TotalUsos,

                                firma.FechaUltimoUso,

                                rutaArchivo =
                                    $"/ERP/Adquisiciones/Index?handler=ImagenFirma&firmaId={firma.Id}"
                            }
                    }
                );
            }
            catch (
                Exception ex
            )
            {
                /*
                 * Si la BD falla después de haber creado el archivo,
                 * eliminamos el archivo huérfano.
                 */

                if (
                    System.IO.File.Exists(
                        rutaFisica
                    )
                )
                {
                    System.IO.File.Delete(
                        rutaFisica
                    );
                }


                _logger.LogError(
                    ex,
                    "Error al guardar una firma para el usuario {UsuarioId}.",
                    usuarioActual.Id
                );


                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "Ocurrió un error al guardar la firma."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }


        // =========================================================
        // ESTABLECER FIRMA PREDETERMINADA
        // POST ?handler=PredeterminarFirma&firmaId=1
        // =========================================================

        public async Task<IActionResult>
            OnPostPredeterminarFirmaAsync(
                int firmaId
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
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


            AdqFirmaUsuario? firma =
                await _context.AdqFirmasUsuario
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                firmaId
                            &&
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.Activa
                            &&
                            !x.Eliminado
                    );


            if (
                firma ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "No se encontró la firma seleccionada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            List<AdqFirmaUsuario> firmasUsuario =
                await _context.AdqFirmasUsuario
                    .Where(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.Activa
                            &&
                            !x.Eliminado
                    )
                    .ToListAsync();


            foreach (
                AdqFirmaUsuario item
                in firmasUsuario
            )
            {
                item.EsPredeterminada =
                    item.Id ==
                    firma.Id;
            }


            firma.FechaModificacion =
                DateTime.Now;


            await _context.SaveChangesAsync();


            return new JsonResult(
                new
                {
                    success = true,

                    message =
                        "La firma predeterminada fue actualizada correctamente."
                }
            );
        }


        // =========================================================
        // ELIMINAR FIRMA
        // POST ?handler=EliminarFirma&firmaId=1
        // =========================================================

        public async Task<IActionResult>
            OnPostEliminarFirmaAsync(
                int firmaId
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
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


            AdqFirmaUsuario? firma =
                await _context.AdqFirmasUsuario
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                firmaId
                            &&
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.Activa
                            &&
                            !x.Eliminado
                    );


            if (
                firma ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "No se encontró la firma seleccionada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            bool eraPredeterminada =
                firma.EsPredeterminada;


            firma.Activa =
                false;

            firma.Eliminado =
                true;

            firma.EsPredeterminada =
                false;

            firma.FechaModificacion =
                DateTime.Now;


            /*
             * NO eliminamos físicamente el PNG.
             *
             * En el futuro una aprobación histórica puede estar
             * vinculada con esa firma.
             */


            if (
                eraPredeterminada
            )
            {
                AdqFirmaUsuario? siguienteFirma =
                    await _context.AdqFirmasUsuario
                        .Where(
                            x =>
                                x.UsuarioId ==
                                    usuarioActual.Id
                                &&
                                x.Id !=
                                    firma.Id
                                &&
                                x.Activa
                                &&
                                !x.Eliminado
                        )
                        .OrderByDescending(
                            x =>
                                x.FechaUltimoUso
                        )
                        .ThenByDescending(
                            x =>
                                x.FechaCreacion
                        )
                        .FirstOrDefaultAsync();


                if (
                    siguienteFirma !=
                    null
                )
                {
                    siguienteFirma.EsPredeterminada =
                        true;

                    siguienteFirma.FechaModificacion =
                        DateTime.Now;
                }
            }


            await _context.SaveChangesAsync();


            return new JsonResult(
                new
                {
                    success = true,

                    message =
                        "La firma fue eliminada de tus firmas disponibles."
                }
            );
        }


        // =========================================================
        // OBTENER IMAGEN DE FIRMA
        // GET ?handler=ImagenFirma&firmaId=1
        // =========================================================

        public async Task<IActionResult>
            OnGetImagenFirmaAsync(
                int firmaId
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
            {
                return Unauthorized();
            }


            AdqFirmaUsuario? firma =
                await _context.AdqFirmasUsuario
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                firmaId
                            &&
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.Activa
                            &&
                            !x.Eliminado
                    );


            if (
                firma ==
                null
            )
            {
                return NotFound();
            }


            string rutaFisica =
                Path.Combine(
                    _environment.ContentRootPath,
                    firma.RutaArchivo.Replace(
                        "/",
                        Path.DirectorySeparatorChar.ToString()
                    )
                );


            string carpetaPermitida =
                Path.GetFullPath(
                    Path.Combine(
                        _environment.ContentRootPath,
                        "App_Data",
                        "Adquisiciones",
                        "Firmas"
                    )
                );


            string rutaCompleta =
                Path.GetFullPath(
                    rutaFisica
                );


            if (
                !rutaCompleta.StartsWith(
                    carpetaPermitida,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return Forbid();
            }


            if (
                !System.IO.File.Exists(
                    rutaCompleta
                )
            )
            {
                return NotFound();
            }


            byte[] archivo =
                await System.IO.File.ReadAllBytesAsync(
                    rutaCompleta
                );


            return File(
                archivo,
                "image/png"
            );
        }

        // =========================================================
        // HISTORIAL DE APROBACIÓN PRESUPUESTAL
        // GET ?handler=HistorialAprobacionPresupuestal&solicitudId=1
        // =========================================================

        public async Task<IActionResult>
            OnGetHistorialAprobacionPresupuestalAsync(
                int solicitudId
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
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


            if (
                solicitudId <= 0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La solicitud indicada no es válida."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // VALIDAR SOLICITUD
            // =====================================================

            AdqSolicitud? solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                solicitudId
                            &&
                            !x.Eliminado
                    );


            if (
                solicitud ==
                null
            )
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
            // MISMA LÓGICA DEL DETALLE DE SOLICITUD
            // =====================================================

            bool esPropietario =
                solicitud.UsuarioSolicitanteId ==
                usuarioActual.Id;


            bool esAprobadorGerente =
                await _context.AdqAprobaciones
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId ==
                                solicitudId
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


            bool esAgenteAsignado =
                solicitud.UsuarioAsignadoId ==
                usuarioActual.Id;


            bool esObservador =
            await (
                from observador
                    in _context
                        .AdqAprobacionesPresupuestalesObservadores
                        .AsNoTracking()

                join aprobacionObs
                    in _context
                        .AdqAprobacionesPresupuestales
                        .AsNoTracking()
                    on observador.AprobacionPresupuestalId
                    equals aprobacionObs.Id

                where
                    aprobacionObs.SolicitudId ==
                        solicitudId
                    &&
                    observador.UsuarioId ==
                        usuarioActual.Id
                    &&
                    observador.Activo
                    &&
                    !observador.Eliminado
                    &&
                    !aprobacionObs.Eliminado

                select observador.Id
            )
            .AnyAsync();


            bool esAprobadorPresupuestal =
                await (
                    from detalle
                        in _context
                            .AdqAprobacionesPresupuestalesDetalle
                            .AsNoTracking()

                    join aprobacionPres
                        in _context
                            .AdqAprobacionesPresupuestales
                            .AsNoTracking()
                        on detalle.AprobacionPresupuestalId
                        equals aprobacionPres.Id

                    where
                        aprobacionPres.SolicitudId ==
                            solicitudId
                        &&
                        detalle.UsuarioAprobadorId ==
                            usuarioActual.Id
                        &&
                        !detalle.Eliminado
                        &&
                        !aprobacionPres.Eliminado

                    select detalle.Id
                )
                .AnyAsync();


            bool puedeConsultar =
                esPropietario
                ||
                esAprobadorGerente
                ||
                esUsuarioAdquisiciones
                ||
                esAgenteAsignado
                ||
                esObservador
                ||
                esAprobadorPresupuestal;


            if (
                !puedeConsultar
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para consultar el historial presupuestal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // OBTENER ÚLTIMO FLUJO PRESUPUESTAL
            // =====================================================

            AdqAprobacionPresupuestal? aprobacion =
                await _context
                    .AdqAprobacionesPresupuestales
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.SolicitudId ==
                                solicitudId
                            &&
                            !x.Eliminado
                    )
                    .OrderByDescending(
                        x =>
                            x.Id
                    )
                    .FirstOrDefaultAsync();


            if (
                aprobacion ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = true,
                        tieneFlujo = false,
                        etapas =
                            Array.Empty<object>()
                    }
                );
            }


            // =====================================================
            // ETAPAS
            // =====================================================

            List<AdqAprobacionPresupuestalDetalle> detalles =
                await _context
                    .AdqAprobacionesPresupuestalesDetalle
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.AprobacionPresupuestalId ==
                                aprobacion.Id
                            &&
                            !x.Eliminado
                    )
                    .OrderBy(
                        x =>
                            x.Orden
                    )
                    .ToListAsync();


            List<int> idsDetalles =
                detalles
                    .Select(
                        x =>
                            x.Id
                    )
                    .ToList();


            List<AdqFirmaAprobacionPresupuestal> firmas =
                await _context
                    .AdqFirmasAprobacionesPresupuestales
                    .AsNoTracking()
                    .Where(
                        x =>
                            idsDetalles.Contains(
                                x.AprobacionPresupuestalDetalleId
                            )
                            &&
                            !x.Eliminado
                    )
                    .ToListAsync();


            List<string> idsUsuarios =
                detalles
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x.UsuarioAprobadorId
                            )
                    )
                    .Select(
                        x =>
                            x.UsuarioAprobadorId!
                    )
                    .Distinct()
                    .ToList();


            var nombresUsuarios =
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
                        idsUsuarios.Contains(
                            usuario.Id
                        )

                    select new
                    {
                        usuario.Id,

                        Nombre =
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
                .ToDictionaryAsync(
                    x =>
                        x.Id,

                    x =>
                        x.Nombre
                );


            List<HistorialEtapaPresupuestalDto> etapas =
                detalles
                    .Select(
                        detalle =>
                        {
                            AdqFirmaAprobacionPresupuestal?
                                firma =
                                    firmas
                                        .Where(
                                            x =>
                                                x.AprobacionPresupuestalDetalleId ==
                                                    detalle.Id
                                        )
                                        .OrderByDescending(
                                            x =>
                                                x.FechaFirma
                                        )
                                        .FirstOrDefault();


                            string aprobador =
                                !string.IsNullOrWhiteSpace(
                                    detalle.UsuarioAprobadorId
                                )
                                &&
                                nombresUsuarios.TryGetValue(
                                    detalle.UsuarioAprobadorId,
                                    out string? nombre
                                )
                                    ? nombre
                                    : "Sin responsable";


                            return new HistorialEtapaPresupuestalDto
                            {
                                DetalleId =
                                    detalle.Id,

                                Orden =
                                    detalle.Orden,

                                NombreEtapa =
                                    detalle.NombreEtapa,

                                Estatus =
                                    detalle.Estatus,

                                EsActual =
                                    detalle.EsActual,

                                UsuarioAprobadorId =
                                    detalle.UsuarioAprobadorId,

                                Aprobador =
                                    aprobador,

                                Comentario =
                                    detalle.Comentario,

                                FechaDecision =
                                    detalle.FechaDecision,

                                TieneFirma =
                                    firma != null,

                                FirmaAprobacionId =
                                    firma?.Id,

                                NombreFirmante =
                                    firma?.NombreFirmante,

                                EmailFirmante =
                                    firma?.EmailFirmante,

                                TipoFirma =
                                    firma?.TipoFirma,

                                DecisionFirma =
                                    firma?.Decision,

                                FechaFirma =
                                    firma?.FechaFirma,

                                DireccionIp =
                                    firma?.DireccionIp,

                                HashFirma =
                                    firma?.HashFirma,

                                HashContextoFirmado =
                                    firma?.HashContextoFirmado,

                                RutaFirma =
                                    firma != null
                                        ? $"/ERP/Adquisiciones/Index?handler=ImagenFirmaAprobacion&firmaAprobacionId={firma.Id}"
                                        : null
                            };
                        }
                    )
                    .ToList();


            return new JsonResult(
                new
                {
                    success = true,

                    tieneFlujo = true,

                    aprobacionId =
                        aprobacion.Id,

                    estatusFlujo =
                        aprobacion.Estatus,

                    monto =
                        aprobacion.MontoSolicitado,

                    etapas
                }
            );
        }

        // =========================================================
        // IMAGEN HISTÓRICA DE FIRMA DE APROBACIÓN PRESUPUESTAL
        // GET ?handler=ImagenFirmaAprobacion&firmaAprobacionId=1
        // =========================================================

        public async Task<IActionResult>
            OnGetImagenFirmaAprobacionAsync(
                int firmaAprobacionId
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
            {
                return Unauthorized();
            }


            if (
                firmaAprobacionId <=
                0
            )
            {
                return NotFound();
            }


            // =====================================================
            // OBTENER FIRMA HISTÓRICA
            // =====================================================

            AdqFirmaAprobacionPresupuestal? firma =
                await _context
                    .AdqFirmasAprobacionesPresupuestales
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                firmaAprobacionId
                            &&
                            !x.Eliminado
                    );


            if (
                firma ==
                null
            )
            {
                return NotFound();
            }


            // =====================================================
            // LOCALIZAR LA SOLICITUD A LA QUE PERTENECE
            // =====================================================

            var datosFlujo =
                await (
                    from detalle
                        in _context
                            .AdqAprobacionesPresupuestalesDetalle
                            .AsNoTracking()

                    join flujo
                        in _context
                            .AdqAprobacionesPresupuestales
                            .AsNoTracking()

                        on detalle.AprobacionPresupuestalId
                        equals flujo.Id

                    where
                        detalle.Id ==
                            firma.AprobacionPresupuestalDetalleId
                        &&
                        !detalle.Eliminado
                        &&
                        !flujo.Eliminado

                    select new
                    {
                        SolicitudId =
                            flujo.SolicitudId,

                        AprobacionPresupuestalId =
                            flujo.Id,

                        DetalleId =
                            detalle.Id
                    }
                )
                .FirstOrDefaultAsync();


            if (
                datosFlujo ==
                null
            )
            {
                return NotFound();
            }


            int solicitudId =
                datosFlujo.SolicitudId;


            // =====================================================
            // 1. PROPIETARIO
            // =====================================================

            bool esPropietario =
                await _context
                    .AdqSolicitudes
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id ==
                                solicitudId
                            &&
                            !x.Eliminado
                            &&
                            x.UsuarioSolicitanteId ==
                                usuarioActual.Id
                    );


            // =====================================================
            // 2. APROBADOR / GERENTE
            // =====================================================

            bool esAprobador =
                await _context
                    .AdqAprobaciones
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId ==
                                solicitudId
                            &&
                            x.UsuarioAprobadorId ==
                                usuarioActual.Id
                    );


            // =====================================================
            // 3. PERSONAL DE ADQUISICIONES
            // =====================================================

            bool esUsuarioAdquisiciones =
                await _context
                    .AdqPermisosUsuarios
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

            bool esAgenteAsignado =
                await _context
                    .AdqSolicitudes
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id ==
                                solicitudId
                            &&
                            !x.Eliminado
                            &&
                            x.UsuarioAsignadoId ==
                                usuarioActual.Id
                    );


            // =====================================================
            // 5. OBSERVADOR PRESUPUESTAL
            // =====================================================

            bool esObservadorPresupuestal =
                await (
                    from observador
                        in _context
                            .AdqAprobacionesPresupuestalesObservadores
                            .AsNoTracking()

                    join flujoObservador
                        in _context
                            .AdqAprobacionesPresupuestales
                            .AsNoTracking()

                        on observador.AprobacionPresupuestalId
                        equals flujoObservador.Id

                    where
                        flujoObservador.SolicitudId ==
                            solicitudId
                        &&
                        observador.UsuarioId ==
                            usuarioActual.Id
                        &&
                        observador.Activo
                        &&
                        !observador.Eliminado
                        &&
                        !flujoObservador.Eliminado

                    select observador.Id
                )
                .AnyAsync();


            // =====================================================
            // 6. APROBADOR PRESUPUESTAL
            // =====================================================

            bool esAprobadorPresupuestal =
                await (
                    from detalle
                        in _context
                            .AdqAprobacionesPresupuestalesDetalle
                            .AsNoTracking()

                    join flujoAprobador
                        in _context
                            .AdqAprobacionesPresupuestales
                            .AsNoTracking()

                        on detalle.AprobacionPresupuestalId
                        equals flujoAprobador.Id

                    where
                        flujoAprobador.SolicitudId ==
                            solicitudId
                        &&
                        detalle.UsuarioAprobadorId ==
                            usuarioActual.Id
                        &&
                        !detalle.Eliminado
                        &&
                        !flujoAprobador.Eliminado

                    select detalle.Id
                )
                .AnyAsync();


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
                esAgenteAsignado
                ||
                esObservadorPresupuestal
                ||
                esAprobadorPresupuestal;


            if (
                !puedeConsultar
            )
            {
                _logger.LogWarning(
                    "Acceso denegado a firma histórica de aprobación. " +
                    "FirmaAprobacionId: {FirmaAprobacionId}, " +
                    "SolicitudId: {SolicitudId}, " +
                    "UsuarioId: {UsuarioId}.",
                    firmaAprobacionId,
                    solicitudId,
                    usuarioActual.Id
                );


                return Forbid();
            }


            // =====================================================
            // VALIDAR RUTA FÍSICA
            // =====================================================

            string carpetaPermitida =
                Path.GetFullPath(
                    Path.Combine(
                        _environment.ContentRootPath,
                        "App_Data",
                        "Adquisiciones",
                        "FirmasAprobaciones"
                    )
                );


            string rutaCompleta =
                Path.GetFullPath(
                    Path.Combine(
                        _environment.ContentRootPath,
                        firma.RutaFirmaSnapshot
                            .Replace(
                                "/",
                                Path.DirectorySeparatorChar
                                    .ToString()
                            )
                    )
                );


            string prefijoPermitido =
                carpetaPermitida
                    .TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar
                    )
                +
                Path.DirectorySeparatorChar;


            if (
                !rutaCompleta.StartsWith(
                    prefijoPermitido,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                _logger.LogWarning(
                    "Ruta inválida detectada para snapshot de firma {FirmaAprobacionId}.",
                    firmaAprobacionId
                );


                return Forbid();
            }


            // =====================================================
            // VALIDAR ARCHIVO
            // =====================================================

            if (
                !System.IO.File.Exists(
                    rutaCompleta
                )
            )
            {
                return NotFound();
            }


            byte[] archivo =
                await System.IO.File
                    .ReadAllBytesAsync(
                        rutaCompleta
                    );


            // =====================================================
            // VALIDAR HASH ANTES DE ENTREGAR LA EVIDENCIA
            // =====================================================

            string hashActual =
                Convert.ToHexString(
                    SHA256.HashData(
                        archivo
                    )
                );


            if (
                !string.Equals(
                    hashActual,
                    firma.HashFirma,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                _logger.LogError(
                    "El snapshot de firma {FirmaAprobacionId} no superó la validación SHA-256.",
                    firmaAprobacionId
                );


                return new StatusCodeResult(
                    StatusCodes.Status409Conflict
                );
            }


            // =====================================================
            // DEVOLVER PNG
            // =====================================================

            Response.Headers[
                "Cache-Control"
            ] =
                "private, no-store, no-cache, must-revalidate";


            Response.Headers[
                "Pragma"
            ] =
                "no-cache";


            return File(
                archivo,
                "image/png"
            );
        }

        public int TotalSeguimientosPresupuestales =>
            SeguimientosPresupuestales.Count;

        public async Task<IActionResult>
            OnGetConfiguracionAprobacionPresupuestalAsync()
                {
                    var usuarioActual =
                        await _userManager.GetUserAsync(
                            User
                        );

                    if (
                        usuarioActual == null
                    )
                    {
                        return Unauthorized();
                    }


                    var permiso =
                    await _context
                        .AdqPermisosUsuarios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            x =>
                                x.UsuarioId ==
                                usuarioActual.Id
                        );


                    if (
                        permiso == null
                        ||
                        !permiso.PuedeAdministrar
                    )
                    {
                        return Forbid();
                    }


                    var usuarios =
            await _userManager
                .Users
                .AsNoTracking()
                .Where(
                    x =>
                        !x.IsBanned
                )
                .Select(
                    x =>
                        new
                        {
                            id =
                                x.Id,

                            nombre =
                                x.UserName
                                ??
                                x.Email
                                ??
                                "Usuario",

                            email =
                                x.Email
                                ??
                                x.UserName
                                ??
                                string.Empty
                        }
                )
                .OrderBy(
                    x =>
                        x.nombre
                )
                .ThenBy(
                    x =>
                        x.email
                )
                .ToListAsync();


            var configuracion =
                await _context
                    .AdqConfiguracionAprobacionPresupuestal
                    .AsNoTracking()
                    .Where(
                        x =>
                            !x.Eliminado
                            &&
                            x.Activo
                    )
                    .OrderBy(
                        x =>
                            x.Orden
                    )
                    .ToListAsync();


            var etapasDefinidas =
                new[]
                {
            new
            {
                Orden = 1,
                TipoEtapa = "GerenciaAdquisiciones",
                NombreEtapa = "Gerencia de Adquisiciones"
            },

            new
            {
                Orden = 2,
                TipoEtapa = "PlaneacionFinanciera",
                NombreEtapa = "Planeación Financiera"
            },

            new
            {
                Orden = 3,
                TipoEtapa = "DireccionOperacionesInternas",
                NombreEtapa = "Dirección de Operaciones Internas"
            },

            new
            {
                Orden = 4,
                TipoEtapa = "DireccionGeneralSocios",
                NombreEtapa = "Dirección General / Socios"
            }
                };


            var resultado =
                etapasDefinidas
                    .Select(
                        etapa =>
                        {
                            var existente =
                                configuracion
                                    .FirstOrDefault(
                                        x =>
                                            x.Orden ==
                                            etapa.Orden
                                    );


                            var responsable =
                                usuarios
                                    .FirstOrDefault(
                                        x =>
                                            x.id ==
                                            existente?.UsuarioResponsableId
                                    );


                            var asistente =
                                usuarios
                                    .FirstOrDefault(
                                        x =>
                                            x.id ==
                                            existente?.UsuarioAsistenteId
                                    );


                            return new
                            {
                                orden =
                                    etapa.Orden,

                                tipoEtapa =
                                    etapa.TipoEtapa,

                                nombreEtapa =
                                    etapa.NombreEtapa,

                                usuarioResponsableId =
                                    existente?.UsuarioResponsableId,

                                usuarioResponsableNombre =
                                    responsable == null
                                        ? null
                                        : (
                                            string.IsNullOrWhiteSpace(
                                                responsable.nombre
                                            )
                                                ? responsable.email
                                                : responsable.nombre
                                          ),

                                usuarioAsistenteId =
                                    existente?.UsuarioAsistenteId,

                                usuarioAsistenteNombre =
                                    asistente == null
                                        ? null
                                        : (
                                            string.IsNullOrWhiteSpace(
                                                asistente.nombre
                                            )
                                                ? asistente.email
                                                : asistente.nombre
                                          ),

                                asistenteRecibeCopia =
                                    existente?.AsistenteRecibeCopia
                                    ??
                                    false,

                                recibirCopiaDesdeOrden =
                                    existente?.RecibirCopiaDesdeOrden
                            };
                        }
                    )
                    .ToList();


            return new JsonResult(
                new
                {
                    success =
                        true,

                    usuarios,

                    etapas =
                        resultado
                }
            );
        }

        public async Task<IActionResult>
    OnPostGuardarConfiguracionAprobacionPresupuestalAsync(
        [FromBody]
        GuardarConfiguracionAprobacionPresupuestalRequest request
    )
        {
            var usuarioActual =
                await _userManager.GetUserAsync(
                    User
                );

            if (
                usuarioActual == null
            )
            {
                return Unauthorized();
            }


            var permiso =
                await _context
                    .AdqPermisosUsuarios
                    .FirstOrDefaultAsync(
                        x =>
                            x.UsuarioId ==
                            usuarioActual.Id
                    );


            if (
                permiso == null
                ||
                !permiso.PuedeAdministrar
            )
            {
                return Forbid();
            }


            if (
                request == null
                ||
                request.Etapas == null
                ||
                request.Etapas.Count != 4
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "La configuración debe contener exactamente las cuatro etapas de aprobación."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            var ordenesEsperados =
                new[]
                {
            1,
            2,
            3,
            4
                };


            var ordenesRecibidos =
                request
                    .Etapas
                    .Select(
                        x =>
                            x.Orden
                    )
                    .OrderBy(
                        x =>
                            x
                    )
                    .ToArray();


            if (
                !ordenesRecibidos.SequenceEqual(
                    ordenesEsperados
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Las etapas de aprobación presupuestal no son válidas."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                request
                    .Etapas
                    .Any(
                        x =>
                            string.IsNullOrWhiteSpace(
                                x.UsuarioResponsableId
                            )
                    )
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Debes asignar un responsable a las cuatro etapas."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            var responsablesDuplicados =
                request
                    .Etapas
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x.UsuarioResponsableId
                            )
                    )
                    .GroupBy(
                        x =>
                            x.UsuarioResponsableId
                    )
                    .Any(
                        g =>
                            g.Count() > 1
                    );


            if (
                responsablesDuplicados
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Un mismo usuario no puede ser responsable de más de una etapa presupuestal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            foreach (
                var etapa in request.Etapas
            )
            {
                if (
                    !string.IsNullOrWhiteSpace(
                        etapa.UsuarioAsistenteId
                    )
                    &&
                    etapa.UsuarioAsistenteId ==
                    etapa.UsuarioResponsableId
                )
                {
                    return new JsonResult(
                        new
                        {
                            success =
                                false,

                            message =
                                $"El responsable y el asistente de la etapa {etapa.Orden} no pueden ser la misma persona."
                        }
                    )
                    {
                        StatusCode =
                            StatusCodes.Status409Conflict
                    };
                }


                if (
                    etapa.AsistenteRecibeCopia
                )
                {
                    if (
                        string.IsNullOrWhiteSpace(
                            etapa.UsuarioAsistenteId
                        )
                    )
                    {
                        return new JsonResult(
                            new
                            {
                                success =
                                    false,

                                message =
                                    $"Debes seleccionar un asistente para la etapa {etapa.Orden} antes de habilitar la copia."
                            }
                        )
                        {
                            StatusCode =
                                StatusCodes.Status400BadRequest
                        };
                    }


                    if (
                        !etapa.RecibirCopiaDesdeOrden.HasValue
                        ||
                        etapa.RecibirCopiaDesdeOrden < 1
                        ||
                        etapa.RecibirCopiaDesdeOrden > 4
                    )
                    {
                        return new JsonResult(
                            new
                            {
                                success =
                                    false,

                                message =
                                    $"Selecciona desde qué etapa recibirá copia el asistente de la etapa {etapa.Orden}."
                            }
                        )
                        {
                            StatusCode =
                                StatusCodes.Status400BadRequest
                        };
                    }
                }
                else
                {
                    etapa.RecibirCopiaDesdeOrden =
                        null;
                }
            }


            var idsUsuarios =
                request
                    .Etapas
                    .SelectMany(
                        x =>
                            new[]
                            {
                        x.UsuarioResponsableId,
                        x.UsuarioAsistenteId
                            }
                    )
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x
                            )
                    )
                    .Distinct()
                    .ToList();


            var usuariosValidos =
                await _userManager
                    .Users
                    .Where(
                        x =>
                            idsUsuarios.Contains(
                                x.Id
                            )
                            &&
                            !x.IsBanned
                    )
                    .Select(
                        x =>
                            x.Id
                    )
                    .ToListAsync();


            if (
                idsUsuarios.Any(
                    id =>
                        !usuariosValidos.Contains(
                            id!
                        )
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Uno o más usuarios seleccionados no existen o ya no se encuentran activos."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            var definicionEtapas =
                new Dictionary<int, (string Tipo, string Nombre)>
                {
            {
                1,
                (
                    "GerenciaAdquisiciones",
                    "Gerencia de Adquisiciones"
                )
            },

            {
                2,
                (
                    "PlaneacionFinanciera",
                    "Planeación Financiera"
                )
            },

            {
                3,
                (
                    "DireccionOperacionesInternas",
                    "Dirección de Operaciones Internas"
                )
            },

            {
                4,
                (
                    "DireccionGeneralSocios",
                    "Dirección General / Socios"
                )
            }
                };


            var ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database.BeginTransactionAsync();


            try
            {
                var existentes =
                    await _context
                        .AdqConfiguracionAprobacionPresupuestal
                        .Where(
                            x =>
                                !x.Eliminado
                        )
                        .ToListAsync();


                foreach (
                    var entrada in request.Etapas
                )
                {
                    var definicion =
                        definicionEtapas[
                            entrada.Orden
                        ];


                    var registro =
                        existentes
                            .FirstOrDefault(
                                x =>
                                    x.Orden ==
                                    entrada.Orden
                            );


                    if (
                        registro == null
                    )
                    {
                        registro =
                            new AdqConfiguracionAprobacionPresupuestal
                            {
                                Orden =
                                    entrada.Orden,

                                TipoEtapa =
                                    definicion.Tipo,

                                NombreEtapa =
                                    definicion.Nombre,

                                UsuarioResponsableId =
                                    entrada.UsuarioResponsableId!,

                                UsuarioAsistenteId =
                                    string.IsNullOrWhiteSpace(
                                        entrada.UsuarioAsistenteId
                                    )
                                        ? null
                                        : entrada.UsuarioAsistenteId,

                                AsistenteRecibeCopia =
                                    entrada.AsistenteRecibeCopia,

                                RecibirCopiaDesdeOrden =
                                    entrada.AsistenteRecibeCopia
                                        ? entrada.RecibirCopiaDesdeOrden
                                        : null,

                                Activo =
                                    true,

                                FechaCreacion =
                                    ahora,

                                FechaModificacion =
                                    null,

                                UsuarioModificacionId =
                                    usuarioActual.Id,

                                Eliminado =
                                    false
                            };


                        _context
                            .AdqConfiguracionAprobacionPresupuestal
                            .Add(
                                registro
                            );
                    }
                    else
                    {
                        registro.TipoEtapa =
                            definicion.Tipo;

                        registro.NombreEtapa =
                            definicion.Nombre;

                        registro.UsuarioResponsableId =
                            entrada.UsuarioResponsableId!;

                        registro.UsuarioAsistenteId =
                            string.IsNullOrWhiteSpace(
                                entrada.UsuarioAsistenteId
                            )
                                ? null
                                : entrada.UsuarioAsistenteId;

                        registro.AsistenteRecibeCopia =
                            entrada.AsistenteRecibeCopia;

                        registro.RecibirCopiaDesdeOrden =
                            entrada.AsistenteRecibeCopia
                                ? entrada.RecibirCopiaDesdeOrden
                                : null;

                        registro.Activo =
                            true;

                        registro.FechaModificacion =
                            ahora;

                        registro.UsuarioModificacionId =
                            usuarioActual.Id;
                    }

                    // =====================================================
                    // ACTUALIZAR RESPONSABLE EN FLUJOS AÚN NO RESUELTOS
                    // =====================================================

                    List<AdqAprobacionPresupuestalDetalle>
                        detallesPendientes =
                            await _context
                                .AdqAprobacionesPresupuestalesDetalle
                                .Where(
                                    x =>
                                        !x.Eliminado
                                        &&
                                        x.Orden ==
                                            entrada.Orden
                                        &&
                                        (
                                            x.Estatus ==
                                                "Pendiente"
                                            ||
                                            x.Estatus ==
                                                "EnEspera"
                                        )
                                )
                                .ToListAsync();


                    foreach (
                        var detalle
                        in detallesPendientes
                    )
                    {
                        detalle.UsuarioAprobadorId =
                            entrada.UsuarioResponsableId;
                    }


                    // =====================================================
                    // ACTUALIZAR CABECERA SI ESA ETAPA ES LA ACTUAL
                    // =====================================================

                    List<int> idsAprobacionesActuales =
                        detallesPendientes
                            .Where(
                                x =>
                                    x.EsActual
                                    &&
                                    x.Estatus ==
                                        "Pendiente"
                            )
                            .Select(
                                x =>
                                    x.AprobacionPresupuestalId
                            )
                            .Distinct()
                            .ToList();


                    if (
                        idsAprobacionesActuales.Count >
                        0
                    )
                    {
                        List<AdqAprobacionPresupuestal>
                            aprobacionesActuales =
                                await _context
                                    .AdqAprobacionesPresupuestales
                                    .Where(
                                        x =>
                                            idsAprobacionesActuales.Contains(
                                                x.Id
                                            )
                                            &&
                                            !x.Eliminado
                                    )
                                    .ToListAsync();


                        foreach (
                            var aprobacionActual
                            in aprobacionesActuales
                        )
                        {
                            aprobacionActual.UsuarioAprobadorId =
                                entrada.UsuarioResponsableId;
                        }
                    }
                }


                await _context.SaveChangesAsync();

                await transaccion.CommitAsync();


                return new JsonResult(
                    new
                    {
                        success =
                            true,

                        message =
                            "La configuración de aprobación presupuestal se guardó correctamente."
                    }
                );
            }
            catch (
                Exception ex
            )
            {
                await transaccion.RollbackAsync();


                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "No fue posible guardar la configuración de aprobación presupuestal.",

                        detail =
                            ex.Message
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
        }


        public int TotalAprobacionesPresupuestalesPendientes =>
            AprobacionesPresupuestalesPendientes.Count;

        // =========================================================
        // CARGAR APROBACIONES PRESUPUESTALES PENDIENTES
        // =========================================================

        private async Task
            CargarAprobacionesPresupuestalesPendientesAsync(
                AppUser usuarioActual)
        {
            AprobacionesPresupuestalesPendientes =
                await (
                    from detalle
                        in _context
                            .AdqAprobacionesPresupuestalesDetalle
                            .AsNoTracking()

                    join aprobacion
                        in _context
                            .AdqAprobacionesPresupuestales
                            .AsNoTracking()
                        on detalle.AprobacionPresupuestalId
                        equals aprobacion.Id

                    join solicitud
                        in _context
                            .AdqSolicitudes
                            .AsNoTracking()
                        on aprobacion.SolicitudId
                        equals solicitud.Id

                    join cotizacion
                        in _context
                            .AdqCotizaciones
                            .AsNoTracking()
                        on aprobacion.CotizacionId
                        equals cotizacion.Id

                    join area
                        in _context
                            .Areas
                            .AsNoTracking()
                        on solicitud.AreaId
                        equals area.Id
                        into areaJoin

                    from area
                        in areaJoin.DefaultIfEmpty()

                    join empleado
                        in _context
                            .Empleados
                            .AsNoTracking()
                        on solicitud.EmpleadoSolicitanteId
                        equals empleado.Id
                        into empleadoJoin

                    from empleado
                        in empleadoJoin.DefaultIfEmpty()

                    where
                        detalle.UsuarioAprobadorId ==
                            usuarioActual.Id
                        &&
                        detalle.EsActual
                        &&
                        detalle.Estatus ==
                            "Pendiente"
                        &&
                        !detalle.Eliminado
                        &&
                        !aprobacion.Eliminado
                        &&
                        !solicitud.Eliminado
                        &&
                        !cotizacion.Eliminado
                        &&
                        solicitud.EstatusId ==
                            12

                    orderby
                        aprobacion.FechaSolicitud
                            ascending

                    select
                        new AprobacionPresupuestalPendienteDto
                        {
                            DetalleId =
                                detalle.Id,

                            SolicitudId =
                                solicitud.Id,

                            AprobacionPresupuestalId =
                                aprobacion.Id,

                            Orden =
                                detalle.Orden,

                            NombreEtapa =
                                detalle.NombreEtapa,

                            Folio =
                                solicitud.Folio,

                            Titulo =
                                solicitud.Titulo,

                            Proveedor =
                                cotizacion.NombreProveedor,

                            Monto =
                                aprobacion.MontoSolicitado,

                            FechaSolicitud =
                                aprobacion.FechaSolicitud,

                            Solicitante =
                                empleado != null
                                    ? empleado.NombreCompleto
                                    : "No disponible",

                            Area =
                                area != null
                                    ? area.Nombre
                                    : "No disponible",

                            ComentarioSolicitud =
                                aprobacion.ComentarioSolicitud
                        }
                )
                .ToListAsync();
        }

        // =========================================================
        // CARGAR SEGUIMIENTOS PRESUPUESTALES
        // =========================================================

        private async Task
            CargarSeguimientosPresupuestalesAsync(
                AppUser usuarioActual)
        {
            SeguimientosPresupuestales =
                await (
                    from observador
                        in _context
                            .AdqAprobacionesPresupuestalesObservadores
                            .AsNoTracking()

                    join aprobacion
                        in _context
                            .AdqAprobacionesPresupuestales
                            .AsNoTracking()
                        on observador.AprobacionPresupuestalId
                        equals aprobacion.Id

                    join solicitud
                        in _context
                            .AdqSolicitudes
                            .AsNoTracking()
                        on aprobacion.SolicitudId
                        equals solicitud.Id

                    join cotizacion
                        in _context
                            .AdqCotizaciones
                            .AsNoTracking()
                        on aprobacion.CotizacionId
                        equals cotizacion.Id

                    where
                        observador.UsuarioId ==
                            usuarioActual.Id
                        &&
                        observador.Activo
                        &&
                        !observador.Eliminado
                        &&
                        !aprobacion.Eliminado
                        &&
                        !solicitud.Eliminado
                        &&
                        !cotizacion.Eliminado

                    let etapaActual =
                        _context
                            .AdqAprobacionesPresupuestalesDetalle
                            .Where(
                                d =>
                                    d.AprobacionPresupuestalId ==
                                        aprobacion.Id
                                    &&
                                    !d.Eliminado
                                    &&
                                    d.EsActual
                            )
                            .OrderBy(
                                d =>
                                    d.Orden
                            )
                            .FirstOrDefault()

                    orderby
                        observador.FechaActivacion descending,
                        aprobacion.FechaSolicitud descending

                    select
                        new SeguimientoPresupuestalDto
                        {
                            AprobacionPresupuestalId =
                                aprobacion.Id,

                            SolicitudId =
                                solicitud.Id,

                            Folio =
                                solicitud.Folio,

                            Titulo =
                                solicitud.Titulo,

                            Proveedor =
                                cotizacion.NombreProveedor,

                            Monto =
                                aprobacion.MontoSolicitado,

                            EtapaActual =
                                etapaActual != null
                                    ? etapaActual.NombreEtapa
                                    : (
                                        aprobacion.Estatus ==
                                        "Aprobada"
                                            ? "Flujo finalizado"
                                            : "Sin etapa activa"
                                      ),

                            OrdenEtapaActual =
                                etapaActual != null
                                    ? etapaActual.Orden
                                    : 0,

                            NombreOrigen =
                                observador.NombreOrigen,

                            FechaActivacion =
                                observador.FechaActivacion,

                            FechaSolicitud =
                                aprobacion.FechaSolicitud,

                            EstatusFlujo =
                                aprobacion.Estatus
                        }
                )
                .ToListAsync();
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
                bool observadorPresupuestal =
                    await (
                        from observador
                            in _context
                                .AdqAprobacionesPresupuestalesObservadores
                                .AsNoTracking()

                        join aprobacionPresupuestal
                            in _context
                                .AdqAprobacionesPresupuestales
                                .AsNoTracking()
                            on observador.AprobacionPresupuestalId
                            equals aprobacionPresupuestal.Id

                        where
                            aprobacionPresupuestal.SolicitudId ==
                                id
                            &&
                            observador.UsuarioId ==
                                usuarioActual.Id
                            &&
                            observador.Activo
                            &&
                            !observador.Eliminado
                            &&
                            !aprobacionPresupuestal.Eliminado

                        select observador.Id
                    )
                    .AnyAsync();


                puedeVer =
                    observadorPresupuestal;
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

                                                    d.Orden,

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

        public class SolicitudPagoInput
        {
            public int SolicitudId
            {
                get;
                set;
            }


            [Required(
                ErrorMessage =
                    "La compañía es obligatoria."
            )]
            [StringLength(250)]
            public string Compania
            {
                get;
                set;
            } = string.Empty;


            [Required(
                ErrorMessage =
                    "Debes seleccionar la moneda."
            )]
            [StringLength(30)]
            public string Moneda
            {
                get;
                set;
            } = "Pesos";


            [Required(
                ErrorMessage =
                    "Debes seleccionar la forma de pago."
            )]
            [StringLength(30)]
            public string FormaPago
            {
                get;
                set;
            } = "Transferencia";


            [Required(
                ErrorMessage =
                    "El concepto de pago es obligatorio."
            )]
            [StringLength(5000)]
            public string ConceptoPago
            {
                get;
                set;
            } = string.Empty;


            [StringLength(250)]
            public string? Banco
            {
                get;
                set;
            }


            [StringLength(100)]
            public string? Cuenta
            {
                get;
                set;
            }


            [StringLength(100)]
            public string? ClabeInterbancaria
            {
                get;
                set;
            }


            public bool ComprobanteAdjunto
            {
                get;
                set;
            }


            [Range(
                typeof(decimal),
                "0",
                "9999999999999999"
            )]
            public decimal RetencionIva
            {
                get;
                set;
            }


            [Range(
                typeof(decimal),
                "0",
                "9999999999999999"
            )]
            public decimal RetencionIsr
            {
                get;
                set;
            }


            [Range(
                typeof(decimal),
                "0",
                "9999999999999999"
            )]
            public decimal OtrosImpuestos
            {
                get;
                set;
            }


            [Range(
                typeof(decimal),
                "0",
                "9999999999999999"
            )]
            public decimal OtrosServicios
            {
                get;
                set;
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


            [Required(
                ErrorMessage =
                    "Debes seleccionar el tipo de solicitud."
            )]
            [StringLength(
                30,
                ErrorMessage =
                    "El tipo de solicitud no puede superar los 30 caracteres."
            )]
            public string TipoDocumentoSolicitud
            {
                get;
                set;
            } = "Cotizaciones";


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

            ValidarTipoDocumentoSolicitud();

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

            ValidarTipoDocumentoSolicitud();


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

            ValidarTipoDocumentoSolicitud();


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

                solicitud.TipoDocumentoSolicitud =
                    Input.TipoDocumentoSolicitud;

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

            ValidarTipoDocumentoSolicitud();


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

                solicitud.TipoDocumentoSolicitud =
                    Input.TipoDocumentoSolicitud;

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
        // EDITAR COTIZACIÓN
        // =========================================================

        public async Task<IActionResult>
            OnPostEditarCotizacionAsync(
                int cotizacionEditarId)
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (usuarioActual == null)
            {
                return Challenge();
            }


            if (
                cotizacionEditarId <=
                0
            )
            {
                TempData["MensajeError"] =
                    "No se identificó la cotización a modificar.";

                return RedirectToPage(
                    new
                    {
                        openCotizacionId =
                            InputCotizacion.SolicitudId
                    }
                );
         }


            // =====================================================
            // VALIDACIÓN EXCLUSIVA DE COTIZACIÓN
            // =====================================================

            ModelState.Clear();


            TryValidateModel(
                InputCotizacion,
                nameof(InputCotizacion)
            );


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
                return Forbid();
            }


            // =====================================================
            // NORMALIZAR
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


            if (
                InputCotizacion.Detalles == null
                ||
                InputCotizacion.Detalles.Count ==
                0
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    "La cotización debe contener al menos un producto o servicio."
                );
            }


            if (
                InputCotizacion.Detalles != null
                &&
                InputCotizacion.Detalles.Any(
                    x =>
                        x.PrecioUnitario <=
                        0
                )
            )
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Todos los productos deben tener un precio unitario mayor a cero."
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
                        openCotizacionId =
                            InputCotizacion.SolicitudId
                    }
                );
            }


            // =====================================================
            // CONSULTAR COTIZACIÓN
            // =====================================================

            AdqCotizacion? cotizacion =
                await _context.AdqCotizaciones
                    .Include(
                        x => x.Detalles
                    )
                    .Include(
                        x => x.Adjuntos
                    )
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                cotizacionEditarId
                            &&
                            !x.Eliminado
                    );


            if (cotizacion == null)
            {
                return NotFound();
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
                                cotizacion.SolicitudId
                            &&
                            !x.Eliminado
                    );


            if (solicitud == null)
            {
                return NotFound();
            }


            if (
                InputCotizacion.SolicitudId !=
                solicitud.Id
            )
            {
                return BadRequest();
            }


            // =====================================================
            // SEGURIDAD
            // =====================================================

            if (
                solicitud.UsuarioAsignadoId !=
                usuarioActual.Id
            )
            {
                return Forbid();
            }


            if (
                solicitud.EstatusId !=
                9
            )
            {
                TempData["MensajeError"] =
                    "La cotización solamente puede modificarse mientras la solicitud se encuentra En cotización.";


                return RedirectToPage(
                    new
                    {
                        openId =
                            solicitud.Id
                    }
                );
            }


            // =====================================================
            // DETALLES ORIGINALES DE SOLICITUD
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


            List<int> idsSolicitud =
                detallesSolicitud
                    .Select(
                        x => x.Id
                    )
                    .OrderBy(
                        x => x
                    )
                    .ToList();


            List<int> idsRecibidos =
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
                    idsRecibidos.Count
                ||
                !idsSolicitud.SequenceEqual(
                    idsRecibidos
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
            // DETALLES ACTUALES DE LA COTIZACIÓN
            // =====================================================

            List<AdqCotizacionDetalle> detallesActuales =
                cotizacion.Detalles
                    .Where(
                        x => !x.Eliminado
                    )
                    .OrderBy(
                        x => x.Orden
                    )
                    .ToList();


            if (
                detallesActuales.Count !=
                detallesSolicitud.Count
            )
            {
                TempData["MensajeError"] =
                    "La estructura de productos de la cotización ya no coincide con la solicitud.";


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


            decimal subtotal =
                0m;


            // =====================================================
            // ACTUALIZAR DETALLES
            // =====================================================

            for (
                int i = 0;
                i < detallesSolicitud.Count;
                i++
            )
            {
                AdqSolicitudDetalle detalleSolicitud =
                    detallesSolicitud[i];


                CotizacionDetalleInput? detalleInput =
                    InputCotizacion.Detalles
                        .FirstOrDefault(
                            x =>
                                x.SolicitudDetalleId ==
                                detalleSolicitud.Id
                        );


                if (detalleInput == null)
                {
                    continue;
                }


                AdqCotizacionDetalle detalleCotizacion =
                    detallesActuales[i];


                decimal importe =
                    decimal.Round(
                        detalleSolicitud.Cantidad *
                        detalleInput.PrecioUnitario,
                        2,
                        MidpointRounding.AwayFromZero
                    );


                detalleCotizacion.ProductoServicio =
                    detalleSolicitud.ProductoServicio;

                detalleCotizacion.Descripcion =
                    string.IsNullOrWhiteSpace(
                        detalleInput.DescripcionProveedor
                    )
                        ? detalleSolicitud.Descripcion
                        : detalleInput.DescripcionProveedor
                            .Trim();

                detalleCotizacion.Cantidad =
                    detalleSolicitud.Cantidad;

                detalleCotizacion.Unidad =
                    detalleSolicitud.Unidad;

                detalleCotizacion.PrecioUnitario =
                    detalleInput.PrecioUnitario;

                detalleCotizacion.Importe =
                    importe;

                detalleCotizacion.Orden =
                    detalleSolicitud.Orden;


                subtotal +=
                    importe;


                // =================================================
                // REEMPLAZAR EVIDENCIA SÓLO SI SUBIERON UNA NUEVA
                // =================================================

                if (
                    detalleInput.ArchivoEvidencia !=
                    null
                    &&
                    detalleInput.ArchivoEvidencia.Length >
                    0
                )
                {
                    foreach (
                        AdqCotizacionAdjunto evidenciaAnterior
                        in cotizacion.Adjuntos.Where(
                            x =>
                                x.CotizacionDetalleId ==
                                    detalleCotizacion.Id
                                &&
                                !x.Eliminado
                        )
                    )
                    {
                        evidenciaAnterior.Eliminado =
                            true;
                    }


                    await GuardarEvidenciaDetalleCotizacionAdqAsync(
                        cotizacion,
                        detalleCotizacion,
                        detalleInput.ArchivoEvidencia,
                        usuarioActual,
                        ahora
                    );
                }
            }


            // =====================================================
            // RECALCULAR TOTALES
            // =====================================================

            subtotal =
                decimal.Round(
                    subtotal,
                    2,
                    MidpointRounding.AwayFromZero
                );


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


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // =================================================
                // ACTUALIZAR CABECERA
                // =================================================

                string proveedorAnterior =
                    cotizacion.NombreProveedor;


                decimal totalAnterior =
                    cotizacion.Total;


                cotizacion.NombreProveedor =
                    InputCotizacion.NombreProveedor;

                cotizacion.RfcProveedor =
                    InputCotizacion.RfcProveedor;

                cotizacion.ContactoProveedor =
                    InputCotizacion.ContactoProveedor;

                cotizacion.EmailProveedor =
                    InputCotizacion.EmailProveedor;

                cotizacion.TelefonoProveedor =
                    InputCotizacion.TelefonoProveedor;

                cotizacion.Subtotal =
                    subtotal;

                cotizacion.AplicaIva =
                    InputCotizacion.AplicaIva;

                cotizacion.PorcentajeIva =
                    porcentajeIva;

                cotizacion.ImporteIva =
                    importeIva;

                cotizacion.Total =
                    total;

                cotizacion.Observaciones =
                    InputCotizacion.Observaciones;

                cotizacion.FechaModificacion =
                    ahora;


                // =================================================
                // ARCHIVOS ADICIONALES EXISTENTES A ELIMINAR
                // =================================================

                if (
                    ArchivosCotizacionEliminarIds != null
                    &&
                    ArchivosCotizacionEliminarIds.Count > 0
                )
                {
                    List<AdqCotizacionAdjunto> adjuntosEliminar =
                        cotizacion.Adjuntos
                            .Where(
                                x =>
                                    !x.Eliminado
                                    &&
                                    x.CotizacionDetalleId == null
                                    &&
                                    ArchivosCotizacionEliminarIds.Contains(
                                        x.Id
                                    )
                            )
                            .ToList();


                    foreach (
                        AdqCotizacionAdjunto adjunto
                        in adjuntosEliminar
                    )
                    {
                        adjunto.Eliminado =
                            true;
                    }
                }


                // =================================================
                // ARCHIVOS ADICIONALES NUEVOS
                // =================================================

                await GuardarAdjuntosCotizacionAdqAsync(
                    cotizacion,
                    usuarioActual,
                    ahora
                );


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
                            "COTIZACION_EDITADA",

                        Descripcion =
                            $"Se modificó la cotización del proveedor {proveedorAnterior}. Total anterior: {totalAnterior:C2}. Nuevo total: {total:C2}.",

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


                TempData["MensajeExito"] =
                    "La cotización se actualizó correctamente.";

                return RedirectToPage(
                    new
                    {
                        openCotizacionId =
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
                    "Error al editar la cotización {CotizacionId}.",
                    cotizacionEditarId
                );


                TempData["MensajeError"] =
                    "No fue posible actualizar la cotización.";


                return RedirectToPage(
                    new
                    {
                        openCotizacionId =
                            solicitud.Id
                    }
                );
            }
        }


        // =========================================================
        // ELIMINAR UNA O VARIAS COTIZACIONES
        // =========================================================

        public async Task<IActionResult>
            OnPostEliminarCotizacionesAsync(
                List<int> cotizacionIds)
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "No fue posible identificar al usuario."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }


            // =====================================================
            // NORMALIZAR IDS RECIBIDOS
            // =====================================================

            List<int> ids =
                cotizacionIds?
                    .Where(
                        x =>
                            x >
                            0
                    )
                    .Distinct()
                    .ToList()
                ??
                new List<int>();


            if (
                ids.Count ==
                0
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Debes seleccionar al menos una cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // VALIDAR PERMISOS
            // =====================================================

            bool puedeEliminarCotizaciones =
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


            if (
                !puedeEliminarCotizaciones
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "No tienes permisos para eliminar cotizaciones."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // CONSULTAR COTIZACIONES
            // =====================================================

            List<AdqCotizacion> cotizaciones =
                await _context.AdqCotizaciones
                    .Include(
                        x => x.Detalles
                    )
                    .Include(
                        x => x.Adjuntos
                    )
                    .Where(
                        x =>
                            ids.Contains(
                                x.Id
                            )
                            &&
                            !x.Eliminado
                    )
                    .ToListAsync();


            if (
                cotizaciones.Count !=
                ids.Count
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Una o más cotizaciones ya no existen o fueron eliminadas."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // VALIDAR QUE SEAN DE LA MISMA SOLICITUD
            // =====================================================

            List<int> solicitudesIds =
                cotizaciones
                    .Select(
                        x =>
                            x.SolicitudId
                    )
                    .Distinct()
                    .ToList();


            if (
                solicitudesIds.Count !=
                1
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Las cotizaciones seleccionadas no pertenecen a la misma solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            int solicitudId =
                solicitudesIds[0];


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


            if (
                solicitud ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

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
            // VALIDAR AGENTE ASIGNADO
            // =====================================================

            if (
                solicitud.UsuarioAsignadoId !=
                usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Solamente el agente asignado puede eliminar cotizaciones de esta solicitud."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // VALIDAR ESTATUS
            // =====================================================

            if (
                solicitud.EstatusId !=
                9
            )
            {
                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "Las cotizaciones solamente pueden eliminarse mientras la solicitud se encuentre En cotización."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            DateTime ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // =================================================
                // DATOS PARA HISTORIAL
                // =================================================

                List<string> proveedores =
                    cotizaciones
                        .Select(
                            x =>
                                string.IsNullOrWhiteSpace(
                                    x.NombreProveedor
                                )
                                    ? $"Cotización #{x.Id}"
                                    : x.NombreProveedor
                        )
                        .ToList();


                bool seEliminoPrincipal =
                    cotizaciones.Any(
                        x =>
                            x.EsPrincipal
                    );


                // =================================================
                // ELIMINACIÓN LÓGICA DE COTIZACIONES
                // =================================================

                foreach (
                    AdqCotizacion cotizacion
                    in cotizaciones
                )
                {
                    cotizacion.Eliminado =
                        true;


                    cotizacion.EsPrincipal =
                        false;


                    cotizacion.Finalizada =
                        false;


                    cotizacion.FechaModificacion =
                        ahora;


                    // =============================================
                    // DETALLES
                    // =============================================

                    foreach (
                        AdqCotizacionDetalle detalle
                        in cotizacion.Detalles
                            .Where(
                                x =>
                                    !x.Eliminado
                            )
                    )
                    {
                        detalle.Eliminado =
                            true;
                    }


                    // =============================================
                    // EVIDENCIAS Y ARCHIVOS ADICIONALES
                    // =============================================

                    foreach (
                        AdqCotizacionAdjunto adjunto
                        in cotizacion.Adjuntos
                            .Where(
                                x =>
                                    !x.Eliminado
                            )
                    )
                    {
                        adjunto.Eliminado =
                            true;
                    }
                }


                // =================================================
                // ACTUALIZAR SOLICITUD
                // =================================================

                solicitud.FechaModificacion =
                    ahora;


                // =================================================
                // HISTORIAL
                // =================================================

                string proveedoresTexto =
                    string.Join(
                        ", ",
                        proveedores
                    );


                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        UsuarioId =
                            usuarioActual.Id,

                        TipoEvento =
                            cotizaciones.Count ==
                            1
                                ? "COTIZACION_ELIMINADA"
                                : "COTIZACIONES_ELIMINADAS",

                        Descripcion =
                            cotizaciones.Count ==
                            1
                                ? $"Se eliminó la cotización del proveedor {proveedoresTexto}."
                                : $"Se eliminaron {cotizaciones.Count} cotizaciones: {proveedoresTexto}.",

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


                // =================================================
                // CONTAR COTIZACIONES RESTANTES
                // =================================================

                int cotizacionesRestantes =
                    await _context.AdqCotizaciones
                        .AsNoTracking()
                        .CountAsync(
                            x =>
                                x.SolicitudId ==
                                    solicitud.Id
                                &&
                                !x.Eliminado
                        );


                // =================================================
                // RESPUESTA
                // =================================================

                return new JsonResult(
                    new
                    {
                        success =
                            true,

                        message =
                            cotizaciones.Count ==
                            1
                                ? "La cotización se eliminó correctamente."
                                : $"Las {cotizaciones.Count} cotizaciones se eliminaron correctamente.",

                        solicitudId =
                            solicitud.Id,

                        eliminadas =
                            cotizaciones.Count,

                        restantes =
                            cotizacionesRestantes,

                        seEliminoPrincipal =
                            seEliminoPrincipal
                    }
                );
            }
            catch (
                Exception ex
            )
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al eliminar cotizaciones de la solicitud {SolicitudId}.",
                    solicitud.Id
                );


                return new JsonResult(
                    new
                    {
                        success =
                            false,

                        message =
                            "No fue posible eliminar las cotizaciones seleccionadas."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
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
            // CONFIGURACIÓN DEL FLUJO PRESUPUESTAL
            // =====================================================

            List<AdqConfiguracionAprobacionPresupuestal>
                configuracionFlujo =
                    await _context
                        .AdqConfiguracionAprobacionPresupuestal
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.Activo
                                &&
                                !x.Eliminado
                                &&
                                x.Orden >= 1
                                &&
                                x.Orden <= 4
                        )
                        .OrderBy(
                            x =>
                                x.Orden
                        )
                        .ToListAsync();


            // =====================================================
            // VALIDAR LAS CUATRO ETAPAS
            // =====================================================

            if (
                configuracionFlujo.Count !=
                4
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "La configuración de aprobación presupuestal está incompleta. Deben existir las cuatro etapas."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            for (
                int orden = 1;
                orden <= 4;
                orden++
            )
            {
                int total =
                    configuracionFlujo.Count(
                        x =>
                            x.Orden ==
                            orden
                    );


                if (
                    total !=
                    1
                )
                {
                    return new JsonResult(
                        new
                        {
                            success = false,

                            message =
                                $"La etapa presupuestal {orden} no se encuentra configurada correctamente."
                        }
                    )
                    {
                        StatusCode =
                            StatusCodes.Status409Conflict
                    };
                }
            }


            // =====================================================
            // VALIDAR RESPONSABLES
            // =====================================================

            if (
                configuracionFlujo.Any(
                    x =>
                        string.IsNullOrWhiteSpace(
                            x.UsuarioResponsableId
                        )
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "Las cuatro etapas deben tener un responsable configurado."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // VALIDAR RESPONSABLES Y ASISTENTES ACTIVOS
            // =====================================================

            List<string> idsUsuariosConfigurados =
                configuracionFlujo
                    .SelectMany(
                        x =>
                            new[]
                            {
                    x.UsuarioResponsableId,
                    x.UsuarioAsistenteId
                            }
                    )
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x
                            )
                    )
                    .Select(
                        x =>
                            x!
                    )
                    .Distinct()
                    .ToList();


            List<string> usuariosActivos =
                await _userManager
                    .Users
                    .AsNoTracking()
                    .Where(
                        x =>
                            idsUsuariosConfigurados.Contains(
                                x.Id
                            )
                            &&
                            !x.IsBanned
                    )
                    .Select(
                        x =>
                            x.Id
                    )
                    .ToListAsync();


            if (
                idsUsuariosConfigurados.Any(
                    id =>
                        !usuariosActivos.Contains(
                            id
                        )
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            "Uno o más responsables o asistentes configurados ya no se encuentran activos."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // RESPONSABLE ACTUAL - NIVEL 1
            // =====================================================

            AdqConfiguracionAprobacionPresupuestal
                configuracionNivel1 =
                    configuracionFlujo.Single(
                        x =>
                            x.Orden ==
                            1
                    );


            string usuarioNivel1 =
                configuracionNivel1
                    .UsuarioResponsableId;

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
                        usuarioNivel1,

                        FechaRespuesta =
                        null,

                        Estatus =
                        "EnRevision",

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
                // GUARDAR CABECERA PARA OBTENER ID
                // =================================================

                await _context
                    .SaveChangesAsync();

                List<AdqAprobacionPresupuestalDetalle>
    detallesAprobacion =
        configuracionFlujo
            .Select(
                etapa =>
                    new AdqAprobacionPresupuestalDetalle
                    {
                        AprobacionPresupuestalId =
                            aprobacionPresupuestal.Id,

                        Orden =
                            etapa.Orden,

                        TipoAprobador =
                            etapa.TipoEtapa,

                        NombreEtapa =
                            etapa.NombreEtapa,

                        UsuarioAprobadorId =
                            etapa.UsuarioResponsableId,

                        Estatus =
                            etapa.Orden == 1
                                ? "Pendiente"
                                : "EnEspera",

                        EsActual =
                            etapa.Orden == 1,

                        Comentario =
                            null,

                        FechaDecision =
                            null,

                        FechaCreacion =
                            ahora,

                        Eliminado =
                            false
                    }
            )
            .ToList();


                _context
                    .AdqAprobacionesPresupuestalesDetalle
                    .AddRange(
                        detallesAprobacion
                    );

                // =====================================================
                // SNAPSHOT DE ASISTENTES / OBSERVADORES
                // =====================================================

                List<AdqAprobacionPresupuestalObservador>
                    observadores =
                        configuracionFlujo
                            .Where(
                                etapa =>
                                    !string.IsNullOrWhiteSpace(
                                        etapa.UsuarioAsistenteId
                                    )
                                    &&
                                    etapa.AsistenteRecibeCopia
                                    &&
                                    etapa.RecibirCopiaDesdeOrden.HasValue
                            )
                            .Select(
                                etapa =>
                                    new AdqAprobacionPresupuestalObservador
                                    {
                                        AprobacionPresupuestalId =
                                            aprobacionPresupuestal.Id,

                                        UsuarioId =
                                            etapa.UsuarioAsistenteId!,

                                        TipoObservador =
                                            "Asistente",

                                        NombreOrigen =
                                            etapa.NombreEtapa,

                                        OrdenActivacion =
                                            etapa.RecibirCopiaDesdeOrden!.Value,

                                        Activo =
                                            false,

                                        FechaActivacion =
                                            null,

                                        FechaCreacion =
                                            ahora,

                                        Eliminado =
                                            false
                                    }
                            )
                            .ToList();


                if (
                    observadores.Count >
                    0
                )
                {
                    _context
                        .AdqAprobacionesPresupuestalesObservadores
                        .AddRange(
                            observadores
                        );
                }


                // =================================================
                // CAMBIAR ESTATUS: 10 → 12
                // =================================================

                solicitud.EstatusId =
                    12;

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
                            "FLUJO_PRESUPUESTAL_INICIADO",

                        Descripcion =
                            string.IsNullOrWhiteSpace(
                                comentario
                            )
                                ? $"Se inició el flujo de aprobación presupuestal para la cotización de {cotizacion.NombreProveedor} por un monto de {cotizacion.Total:C2}."
                                : $"Se inició el flujo de aprobación presupuestal para la cotización de {cotizacion.NombreProveedor} por un monto de {cotizacion.Total:C2}. Comentario: {comentario}",

                        EstatusAnteriorId =
                            estatusAnterior,

                        EstatusNuevoId =
                            12,

                        FechaEvento =
                            ahora,

                        DireccionIp =
                            ObtenerDireccionIp()
                    }
                );


                await _context
                    .SaveChangesAsync();

                // =====================================================
                // NOTIFICAR SOLO AL APROBADOR ACTUAL - NIVEL 1
                // =====================================================

                await CrearNotificacionAdquisicionesAsync(
                    new List<string>
                    {
                    usuarioNivel1
                    },
                    "Aprobación presupuestal pendiente",
                    $"La solicitud {solicitud.Folio} - {solicitud.Titulo} requiere tu aprobación presupuestal por {cotizacion.Total:C2}. Proveedor seleccionado: {cotizacion.NombreProveedor}.",
                    $"/ERP/Adquisiciones?openId={solicitud.Id}",
                    usuarioActual.Id
                );

                // =================================================
                // NOTIFICAR A RESPONSABLES DE PRESUPUESTO
                // =================================================


                await transaccion
                    .CommitAsync();


                return new JsonResult(
                    new
                    {
                        success = true,

                        message =
                        "El flujo de aprobación presupuestal se inició correctamente.",

                        solicitudId =
                            solicitud.Id,

                        aprobacionPresupuestalId =
                            aprobacionPresupuestal.Id,

                        estatusId =
                            12,

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
        // DECISIÓN DE APROBACIÓN PRESUPUESTAL CON FIRMA Y PIN
        // POST ?handler=DecisionPresupuestal
        // =========================================================

        public async Task<IActionResult>
            OnPostDecisionPresupuestalAsync(
                int detalleId,
                string decision,
                string? comentario,
                int firmaId,
                string pin
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
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


            // =====================================================
            // NORMALIZAR ENTRADA
            // =====================================================

            decision =
                decision?
                    .Trim()
                    .ToUpperInvariant()
                ??
                string.Empty;


            comentario =
                comentario?
                    .Trim();


            pin =
                pin?
                    .Trim()
                ??
                string.Empty;


            // =====================================================
            // VALIDACIONES BÁSICAS
            // =====================================================

            if (
                detalleId <= 0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La aprobación seleccionada no es válida."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                firmaId <= 0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Debes seleccionar una firma para autorizar esta decisión."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                decision != "APROBAR"
                &&
                decision != "DECLINAR"
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La decisión enviada no es válida."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            if (
                pin.Length < 4
                ||
                pin.Length > 20
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Debes ingresar un PIN de autorización válido."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


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


            /*
             * Para una declinación necesitamos conocer
             * obligatoriamente el motivo.
             */
            if (
                decision ==
                    "DECLINAR"
                &&
                string.IsNullOrWhiteSpace(
                    comentario
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Debes indicar el motivo de la declinación."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // OBTENER ETAPA
            // =====================================================

            AdqAprobacionPresupuestalDetalle? detalleActual =
                await _context
                    .AdqAprobacionesPresupuestalesDetalle
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                detalleId
                            &&
                            !x.Eliminado
                    );


            if (
                detalleActual ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró la aprobación presupuestal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // VALIDAR ETAPA ACTUAL
            // =====================================================

            if (
                !detalleActual.EsActual
                ||
                detalleActual.Estatus !=
                    "Pendiente"
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Esta etapa ya no se encuentra pendiente de aprobación."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // VALIDAR APROBADOR
            // =====================================================

            if (
                detalleActual.UsuarioAprobadorId !=
                usuarioActual.Id
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes autorización para responder esta aprobación presupuestal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // VALIDAR QUE NO EXISTA UNA FIRMA PREVIA
            // =====================================================

            bool yaFirmada =
                await _context
                    .AdqFirmasAprobacionesPresupuestales
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.AprobacionPresupuestalDetalleId ==
                                detalleActual.Id
                            &&
                            !x.Eliminado
                    );


            if (
                yaFirmada
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Esta etapa ya cuenta con una firma registrada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // FLUJO DE APROBACIÓN
            // =====================================================

            AdqAprobacionPresupuestal? aprobacion =
                await _context
                    .AdqAprobacionesPresupuestales
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                detalleActual.AprobacionPresupuestalId
                            &&
                            !x.Eliminado
                    );


            if (
                aprobacion ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró el flujo presupuestal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            AdqSolicitud? solicitud =
                await _context
                    .AdqSolicitudes
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                aprobacion.SolicitudId
                            &&
                            !x.Eliminado
                    );


            if (
                solicitud ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se encontró la solicitud relacionada."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // VALIDAR FIRMA SELECCIONADA
            // =====================================================

            AdqFirmaUsuario? firmaUsuario =
                await _context
                    .AdqFirmasUsuario
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id ==
                                firmaId
                            &&
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.Activa
                            &&
                            !x.Eliminado
                    );


            if (
                firmaUsuario ==
                null
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La firma seleccionada no existe o ya no se encuentra disponible."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            // =====================================================
            // VALIDAR SEGURIDAD / PIN
            // =====================================================

            AdqSeguridadFirmaUsuario? seguridadFirma =
                await _context
                    .AdqSeguridadFirmaUsuario
                    .FirstOrDefaultAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.Activo
                            &&
                            !x.Eliminado
                    );


            if (
                seguridadFirma ==
                null
                ||
                string.IsNullOrWhiteSpace(
                    seguridadFirma.PinHash
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        requiereConfigurarPin = true,
                        message =
                            "No tienes un PIN de firma configurado. Configúralo desde Mis firmas antes de continuar."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            DateTime ahora =
                DateTime.Now;


            // =====================================================
            // VALIDAR BLOQUEO
            // =====================================================

            if (
                seguridadFirma.BloqueadoHasta.HasValue
                &&
                seguridadFirma.BloqueadoHasta.Value >
                    ahora
            )
            {
                TimeSpan tiempoRestante =
                    seguridadFirma.BloqueadoHasta.Value -
                    ahora;


                int minutosRestantes =
                    Math.Max(
                        1,
                        (int)Math.Ceiling(
                            tiempoRestante.TotalMinutes
                        )
                    );


                return new JsonResult(
                    new
                    {
                        success = false,
                        bloqueado = true,
                        bloqueadoHasta =
                            seguridadFirma.BloqueadoHasta,

                        message =
                            $"La autorización mediante PIN se encuentra bloqueada temporalmente. Intenta nuevamente en aproximadamente {minutosRestantes} minuto(s)."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status429TooManyRequests
                };
            }


            /*
             * Si un bloqueo anterior ya venció,
             * reiniciamos el contador.
             */
            if (
                seguridadFirma.BloqueadoHasta.HasValue
                &&
                seguridadFirma.BloqueadoHasta.Value <=
                    ahora
            )
            {
                seguridadFirma.BloqueadoHasta =
                    null;

                seguridadFirma.IntentosFallidos =
                    0;
            }


            PasswordHasher<
                AdqSeguridadFirmaUsuario
            > hasher =
                new();


            PasswordVerificationResult resultadoPin =
                hasher.VerifyHashedPassword(
                    seguridadFirma,
                    seguridadFirma.PinHash,
                    pin
                );


            // =====================================================
            // PIN INCORRECTO
            // =====================================================

            if (
                resultadoPin ==
                PasswordVerificationResult.Failed
            )
            {
                seguridadFirma.IntentosFallidos++;


                int intentosRestantes =
                    Math.Max(
                        0,
                        5 -
                        seguridadFirma.IntentosFallidos
                    );


                if (
                    seguridadFirma.IntentosFallidos >=
                    5
                )
                {
                    seguridadFirma.BloqueadoHasta =
                        ahora.AddMinutes(
                            15
                        );


                    await _context.SaveChangesAsync();


                    return new JsonResult(
                        new
                        {
                            success = false,
                            bloqueado = true,
                            intentosRestantes = 0,
                            bloqueadoHasta =
                                seguridadFirma.BloqueadoHasta,

                            message =
                                "Se alcanzó el número máximo de intentos. La autorización mediante PIN quedó bloqueada durante 15 minutos."
                        }
                    )
                    {
                        StatusCode =
                            StatusCodes.Status429TooManyRequests
                    };
                }


                await _context.SaveChangesAsync();


                return new JsonResult(
                    new
                    {
                        success = false,

                        pinIncorrecto = true,

                        intentosRestantes,

                        message =
                            $"PIN incorrecto. Te quedan {intentosRestantes} intento(s) antes del bloqueo temporal."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }


            // =====================================================
            // PIN CORRECTO
            // =====================================================

            seguridadFirma.IntentosFallidos =
                0;

            seguridadFirma.BloqueadoHasta =
                null;


            /*
             * Identity puede solicitar regenerar el hash
             * cuando cambia internamente el algoritmo/configuración.
             */
            if (
                resultadoPin ==
                PasswordVerificationResult.SuccessRehashNeeded
            )
            {
                seguridadFirma.PinHash =
                    hasher.HashPassword(
                        seguridadFirma,
                        pin
                    );
            }


            // =====================================================
            // VALIDAR INTEGRIDAD DE LA FIRMA ORIGINAL
            // =====================================================

            string rutaFirmaOriginal =
                Path.GetFullPath(
                    Path.Combine(
                        _environment.ContentRootPath,
                        firmaUsuario.RutaArchivo.Replace(
                            "/",
                            Path.DirectorySeparatorChar.ToString()
                        )
                    )
                );


            string carpetaFirmasPermitida =
                Path.GetFullPath(
                    Path.Combine(
                        _environment.ContentRootPath,
                        "App_Data",
                        "Adquisiciones",
                        "Firmas"
                    )
                );


            string prefijoCarpetaFirmas =
                carpetaFirmasPermitida
                    .TrimEnd(
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar
                    )
                +
                Path.DirectorySeparatorChar;


            if (
                !rutaFirmaOriginal.StartsWith(
                    prefijoCarpetaFirmas,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No fue posible validar la ubicación de la firma."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            if (
                !System.IO.File.Exists(
                    rutaFirmaOriginal
                )
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "El archivo de la firma seleccionada no se encuentra disponible."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }


            byte[] bytesFirma =
                await System.IO.File
                    .ReadAllBytesAsync(
                        rutaFirmaOriginal
                    );


            string hashFirmaActual =
                Convert.ToHexString(
                    SHA256.HashData(
                        bytesFirma
                    )
                );


            /*
             * Si el archivo físico fue alterado desde
             * que se registró la firma, no permitimos usarlo.
             */
            if (
                !string.Equals(
                    hashFirmaActual,
                    firmaUsuario.HashArchivo,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                _logger.LogWarning(
                    "La firma {FirmaId} del usuario {UsuarioId} no superó la validación de integridad.",
                    firmaUsuario.Id,
                    usuarioActual.Id
                );


                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "La firma seleccionada no superó la validación de integridad. Contacta al administrador del sistema."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status409Conflict
                };
            }


            // =====================================================
            // DATOS DEL FIRMANTE
            // =====================================================

            Empleado? empleadoFirmante =
                await ObtenerEmpleadoActualAsync(
                    usuarioActual
                );


            string nombreFirmante =
                empleadoFirmante?.NombreCompleto
                ??
                usuarioActual.UserName
                ??
                usuarioActual.Email
                ??
                "Usuario";


            string? correoFirmante =
                usuarioActual.Email;


            string direccionIp =
                ObtenerDireccionIp()
                ??
                string.Empty;


            string userAgent =
                Request.Headers[
                    "User-Agent"
                ]
                    .ToString();


            if (
                userAgent.Length >
                500
            )
            {
                userAgent =
                    userAgent.Substring(
                        0,
                        500
                    );
            }


            // =====================================================
            // HASH DEL CONTEXTO AUTORIZADO
            // =====================================================

            string contextoFirmado =
                string.Join(
                    "|",
                    new[]
                    {
                "ERPSEI-ADQUISICIONES",
                $"SolicitudId:{solicitud.Id}",
                $"Folio:{solicitud.Folio}",
                $"TipoSolicitud:{solicitud.TipoDocumentoSolicitud}",
                $"AprobacionId:{aprobacion.Id}",
                $"DetalleId:{detalleActual.Id}",
                $"CotizacionId:{aprobacion.CotizacionId}",
                $"Monto:{aprobacion.MontoSolicitado:0.00}",
                $"OrdenEtapa:{detalleActual.Orden}",
                $"Etapa:{detalleActual.NombreEtapa}",
                $"Decision:{decision}",
                $"Usuario:{usuarioActual.Id}",
                $"Fecha:{ahora:O}"
                    }
                );


            string hashContexto =
                Convert.ToHexString(
                    SHA256.HashData(
                        System.Text.Encoding.UTF8.GetBytes(
                            contextoFirmado
                        )
                    )
                );


            // =====================================================
            // PREPARAR SNAPSHOT INMUTABLE
            // =====================================================

            string carpetaSnapshot =
                Path.Combine(
                    _environment.ContentRootPath,
                    "App_Data",
                    "Adquisiciones",
                    "FirmasAprobaciones",
                    aprobacion.Id.ToString(),
                    detalleActual.Id.ToString()
                );


            Directory.CreateDirectory(
                carpetaSnapshot
            );


            string nombreSnapshot =
                $"{Guid.NewGuid():N}.png";


            string rutaSnapshotFisica =
                Path.Combine(
                    carpetaSnapshot,
                    nombreSnapshot
                );


            string rutaSnapshotRelativa =
                Path.Combine(
                    "App_Data",
                    "Adquisiciones",
                    "FirmasAprobaciones",
                    aprobacion.Id.ToString(),
                    detalleActual.Id.ToString(),
                    nombreSnapshot
                )
                .Replace(
                    "\\",
                    "/"
                );


            // =====================================================
            // OBSERVADORES ACTIVADOS
            // =====================================================

            List<string> usuariosObservadoresActivados =
                new();


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            bool snapshotCreado =
                false;


            try
            {
                // =================================================
                // VERIFICACIÓN FINAL CONTRA DOBLE ENVÍO
                // =================================================

                await _context.Entry(
                    detalleActual
                )
                    .ReloadAsync();


                if (
                    !detalleActual.EsActual
                    ||
                    detalleActual.Estatus !=
                        "Pendiente"
                    ||
                    detalleActual.UsuarioAprobadorId !=
                        usuarioActual.Id
                )
                {
                    await transaccion.RollbackAsync();


                    return new JsonResult(
                        new
                        {
                            success = false,
                            message =
                                "La etapa cambió de estado antes de completar la autorización. Actualiza la pantalla e inténtalo nuevamente."
                        }
                    )
                    {
                        StatusCode =
                            StatusCodes.Status409Conflict
                    };
                }


                bool firmaRegistradaDuranteProceso =
                    await _context
                        .AdqFirmasAprobacionesPresupuestales
                        .AnyAsync(
                            x =>
                                x.AprobacionPresupuestalDetalleId ==
                                    detalleActual.Id
                                &&
                                !x.Eliminado
                        );


                if (
                    firmaRegistradaDuranteProceso
                )
                {
                    await transaccion.RollbackAsync();


                    return new JsonResult(
                        new
                        {
                            success = false,
                            message =
                                "Esta etapa ya fue firmada previamente."
                        }
                    )
                    {
                        StatusCode =
                            StatusCodes.Status409Conflict
                    };
                }


                // =================================================
                // CREAR ARCHIVO SNAPSHOT
                // =================================================

                await System.IO.File
                    .WriteAllBytesAsync(
                        rutaSnapshotFisica,
                        bytesFirma
                    );


                snapshotCreado =
                    true;


                // =================================================
                // REGISTRAR SNAPSHOT DE FIRMA
                // =================================================

                AdqFirmaAprobacionPresupuestal
                    firmaAprobacion =
                        new()
                        {
                            AprobacionPresupuestalDetalleId =
                                detalleActual.Id,

                            FirmaUsuarioId =
                                firmaUsuario.Id,

                            UsuarioFirmanteId =
                                usuarioActual.Id,

                            NombreFirmante =
                                nombreFirmante,

                            EmailFirmante =
                                correoFirmante,

                            OrdenEtapa =
                                detalleActual.Orden,

                            NombreEtapa =
                                detalleActual.NombreEtapa,

                            TipoFirma =
                                firmaUsuario.TipoFirma,

                            RutaFirmaSnapshot =
                                rutaSnapshotRelativa,

                            HashFirma =
                                hashFirmaActual,

                            HashContextoFirmado =
                                hashContexto,

                            Decision =
                                decision,

                            FechaFirma =
                                ahora,

                            DireccionIp =
                                string.IsNullOrWhiteSpace(
                                    direccionIp
                                )
                                    ? null
                                    : direccionIp,

                            UserAgent =
                                string.IsNullOrWhiteSpace(
                                    userAgent
                                )
                                    ? null
                                    : userAgent,

                            Eliminado =
                                false
                        };


                _context
                    .AdqFirmasAprobacionesPresupuestales
                    .Add(
                        firmaAprobacion
                    );


                // =================================================
                // AUDITORÍA INMUTABLE
                // =================================================

                _context
                    .AdqAprobacionesPresupuestalesEventos
                    .Add(
                        new AdqAprobacionPresupuestalEvento
                        {
                            AprobacionPresupuestalId =
                                aprobacion.Id,

                            AprobacionPresupuestalDetalleId =
                                detalleActual.Id,

                            TipoEvento =
                                decision ==
                                    "APROBAR"
                                    ? "APROBACION_FIRMADA"
                                    : "DECLINACION_FIRMADA",

                            Descripcion =
                                decision ==
                                    "APROBAR"
                                    ? $"La etapa {detalleActual.NombreEtapa} fue autorizada mediante firma y PIN."
                                    : $"La etapa {detalleActual.NombreEtapa} fue declinada mediante firma y PIN.",

                            UsuarioId =
                                usuarioActual.Id,

                            OrdenEtapa =
                                detalleActual.Orden,

                            NombreEtapa =
                                detalleActual.NombreEtapa,

                            EstatusAnterior =
                                "Pendiente",

                            EstatusNuevo =
                                decision ==
                                    "APROBAR"
                                    ? "Aprobada"
                                    : "Declinada",

                            FechaEvento =
                                ahora,

                            DireccionIp =
                                string.IsNullOrWhiteSpace(
                                    direccionIp
                                )
                                    ? null
                                    : direccionIp,

                            Eliminado =
                                false
                        }
                    );


                // =================================================
                // ACTUALIZAR USO DE FIRMA
                // =================================================

                firmaUsuario.TotalUsos++;


                firmaUsuario.FechaUltimoUso =
                    ahora;


                firmaUsuario.FechaModificacion =
                    ahora;


                // =================================================
                // ACTIVAR OBSERVADORES DE LA ETAPA
                // =================================================

                List<AdqAprobacionPresupuestalObservador>
                    observadoresActivados =
                        await _context
                            .AdqAprobacionesPresupuestalesObservadores
                            .Where(
                                x =>
                                    x.AprobacionPresupuestalId ==
                                        aprobacion.Id
                                    &&
                                    !x.Eliminado
                                    &&
                                    !x.Activo
                                    &&
                                    x.OrdenActivacion ==
                                        detalleActual.Orden
                            )
                            .ToListAsync();


                foreach (
                    AdqAprobacionPresupuestalObservador observador
                    in observadoresActivados
                )
                {
                    observador.Activo =
                        true;

                    observador.FechaActivacion =
                        ahora;
                }


                usuariosObservadoresActivados =
                    observadoresActivados
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
                        .ToList();


                // =================================================
                // DECLINAR
                // =================================================

                if (
                    decision ==
                    "DECLINAR"
                )
                {
                    detalleActual.Estatus =
                        "Declinada";

                    detalleActual.EsActual =
                        false;

                    detalleActual.Comentario =
                        comentario;

                    detalleActual.FechaDecision =
                        ahora;


                    aprobacion.Estatus =
                        "Declinada";

                    aprobacion.UsuarioAprobadorId =
                        usuarioActual.Id;

                    aprobacion.FechaRespuesta =
                        ahora;

                    aprobacion.ComentarioRespuesta =
                        comentario;


                    solicitud.FechaModificacion =
                        ahora;


                    _context.AdqHistorial.Add(
                        new AdqHistorial
                        {
                            SolicitudId =
                                solicitud.Id,

                            TipoEvento =
                                "PRESUPUESTO_DECLINADO",

                            Descripcion =
                                $"La etapa {detalleActual.NombreEtapa} declinó la aprobación presupuestal. Comentario: {comentario}",

                            UsuarioId =
                                usuarioActual.Id,

                            EstatusAnteriorId =
                                solicitud.EstatusId,

                            EstatusNuevoId =
                                solicitud.EstatusId,

                            FechaEvento =
                                ahora,

                            DireccionIp =
                                string.IsNullOrWhiteSpace(
                                    direccionIp
                                )
                                    ? null
                                    : direccionIp
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

                            finalizada = true,

                            aprobada = false,

                            firmada = true,

                            message =
                                "La aprobación presupuestal fue declinada y firmada correctamente."
                        }
                    );
                }


                // =================================================
                // APROBAR ETAPA ACTUAL
                // =================================================

                detalleActual.Estatus =
                    "Aprobada";

                detalleActual.EsActual =
                    false;

                detalleActual.Comentario =
                    comentario;

                detalleActual.FechaDecision =
                    ahora;


                AdqAprobacionPresupuestalDetalle? siguienteEtapa =
                    await _context
                        .AdqAprobacionesPresupuestalesDetalle
                        .Where(
                            x =>
                                x.AprobacionPresupuestalId ==
                                    aprobacion.Id
                                &&
                                !x.Eliminado
                                &&
                                x.Orden >
                                    detalleActual.Orden
                        )
                        .OrderBy(
                            x =>
                                x.Orden
                        )
                        .FirstOrDefaultAsync();


                // =================================================
                // EXISTE SIGUIENTE ETAPA
                // =================================================

                if (
                    siguienteEtapa !=
                    null
                )
                {
                    siguienteEtapa.Estatus =
                        "Pendiente";

                    siguienteEtapa.EsActual =
                        true;


                    aprobacion.Estatus =
                        "EnRevision";

                    aprobacion.UsuarioAprobadorId =
                        siguienteEtapa.UsuarioAprobadorId;

                    aprobacion.FechaRespuesta =
                        null;

                    aprobacion.ComentarioRespuesta =
                        null;


                    _context.AdqHistorial.Add(
                        new AdqHistorial
                        {
                            SolicitudId =
                                solicitud.Id,

                            TipoEvento =
                                "ETAPA_PRESUPUESTAL_APROBADA",

                            Descripcion =
                                $"La etapa {detalleActual.NombreEtapa} aprobó y firmó el presupuesto. El flujo continúa con {siguienteEtapa.NombreEtapa}.",

                            UsuarioId =
                                usuarioActual.Id,

                            EstatusAnteriorId =
                                solicitud.EstatusId,

                            EstatusNuevoId =
                                solicitud.EstatusId,

                            FechaEvento =
                                ahora,

                            DireccionIp =
                                string.IsNullOrWhiteSpace(
                                    direccionIp
                                )
                                    ? null
                                    : direccionIp
                        }
                    );


                    await _context
                        .SaveChangesAsync();


                    if (
                        !string.IsNullOrWhiteSpace(
                            siguienteEtapa.UsuarioAprobadorId
                        )
                    )
                    {
                        await CrearNotificacionAdquisicionesAsync(
                            new List<string>
                            {
                        siguienteEtapa.UsuarioAprobadorId
                            },
                            "Aprobación presupuestal pendiente",
                            $"La solicitud {solicitud.Folio} - {solicitud.Titulo} requiere tu aprobación presupuestal en la etapa {siguienteEtapa.NombreEtapa}.",
                            $"/ERP/Adquisiciones?openId={solicitud.Id}",
                            usuarioActual.Id
                        );
                    }


                    if (
                        usuariosObservadoresActivados.Count >
                        0
                    )
                    {
                        await CrearNotificacionAdquisicionesAsync(
                            usuariosObservadoresActivados,
                            "Seguimiento de aprobación presupuestal",
                            $"La solicitud {solicitud.Folio} - {solicitud.Titulo} fue aprobada y firmada por {detalleActual.NombreEtapa}. Has sido incluido como observador del proceso presupuestal.",
                            $"/ERP/Adquisiciones?openId={solicitud.Id}",
                            usuarioActual.Id
                        );
                    }


                    await transaccion
                        .CommitAsync();


                    return new JsonResult(
                        new
                        {
                            success = true,

                            finalizada = false,

                            aprobada = true,

                            firmada = true,

                            siguienteEtapa =
                                siguienteEtapa.NombreEtapa,

                            message =
                                $"Etapa firmada y aprobada. El flujo continúa con {siguienteEtapa.NombreEtapa}."
                        }
                    );
                }


                // =================================================
                // ÚLTIMA ETAPA:
                // VALIDAR APROBACIONES Y FIRMAS PREVIAS
                // =================================================

                List<int> idsEtapasPrevias =
                    await _context
                        .AdqAprobacionesPresupuestalesDetalle
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.AprobacionPresupuestalId ==
                                    aprobacion.Id
                                &&
                                !x.Eliminado
                                &&
                                x.Orden <
                                    detalleActual.Orden
                        )
                        .Select(
                            x =>
                                x.Id
                        )
                        .ToListAsync();


                int totalEtapasPrevias =
                    idsEtapasPrevias.Count;


                int totalEtapasPreviasAprobadas =
                    await _context
                        .AdqAprobacionesPresupuestalesDetalle
                        .AsNoTracking()
                        .CountAsync(
                            x =>
                                idsEtapasPrevias.Contains(
                                    x.Id
                                )
                                &&
                                x.Estatus ==
                                    "Aprobada"
                        );


                int totalFirmasPrevias =
                    await _context
                        .AdqFirmasAprobacionesPresupuestales
                        .AsNoTracking()
                        .Where(
                            x =>
                                idsEtapasPrevias.Contains(
                                    x.AprobacionPresupuestalDetalleId
                                )
                                &&
                                !x.Eliminado
                                &&
                                x.Decision ==
                                    "APROBAR"
                        )
                        .Select(
                            x =>
                                x.AprobacionPresupuestalDetalleId
                        )
                        .Distinct()
                        .CountAsync();


                if (
                    totalEtapasPreviasAprobadas !=
                        totalEtapasPrevias
                    ||
                    totalFirmasPrevias !=
                        totalEtapasPrevias
                )
                {
                    throw new InvalidOperationException(
                        "No es posible concluir el flujo porque existen etapas anteriores sin aprobación o sin evidencia de firma."
                    );
                }


                // =================================================
                // ÚLTIMA ETAPA APROBADA
                // =================================================

                aprobacion.Estatus =
                    "Aprobada";

                aprobacion.UsuarioAprobadorId =
                    usuarioActual.Id;

                aprobacion.FechaRespuesta =
                    ahora;

                aprobacion.ComentarioRespuesta =
                    comentario;


                int estatusAnterior =
                    solicitud.EstatusId;


                solicitud.EstatusId =
                    13;

                solicitud.FechaModificacion =
                    ahora;


                _context.AdqHistorial.Add(
                    new AdqHistorial
                    {
                        SolicitudId =
                            solicitud.Id,

                        TipoEvento =
                            "PRESUPUESTO_APROBADO",

                        Descripcion =
                            "El flujo de aprobación presupuestal fue aprobado y firmado en todas sus etapas.",

                        UsuarioId =
                            usuarioActual.Id,

                        EstatusAnteriorId =
                            estatusAnterior,

                        EstatusNuevoId =
                            13,

                        FechaEvento =
                            ahora,

                        DireccionIp =
                            string.IsNullOrWhiteSpace(
                                direccionIp
                            )
                                ? null
                                : direccionIp
                    }
                );


                await _context
                    .SaveChangesAsync();


                if (
                    usuariosObservadoresActivados.Count >
                    0
                )
                {
                    await CrearNotificacionAdquisicionesAsync(
                        usuariosObservadoresActivados,
                        "Seguimiento de aprobación presupuestal",
                        $"La solicitud {solicitud.Folio} - {solicitud.Titulo} fue aprobada y firmada por {detalleActual.NombreEtapa}. El flujo presupuestal ha concluido.",
                        $"/ERP/Adquisiciones?openId={solicitud.Id}",
                        usuarioActual.Id
                    );
                }


                await transaccion
                    .CommitAsync();


                return new JsonResult(
                    new
                    {
                        success = true,

                        finalizada = true,

                        aprobada = true,

                        firmada = true,

                        estatusId = 13,

                        message =
                            "El presupuesto fue aprobado y firmado correctamente en todas sus etapas."
                    }
                );
            }
            catch (
                Exception ex
            )
            {
                await transaccion
                    .RollbackAsync();


                /*
                 * Si la transacción falla, eliminamos únicamente
                 * el snapshot que acabamos de generar.
                 */
                if (
                    snapshotCreado
                    &&
                    System.IO.File.Exists(
                        rutaSnapshotFisica
                    )
                )
                {
                    try
                    {
                        System.IO.File.Delete(
                            rutaSnapshotFisica
                        );
                    }
                    catch (
                        Exception exArchivo
                    )
                    {
                        _logger.LogWarning(
                            exArchivo,
                            "No fue posible eliminar el snapshot de firma huérfano {RutaSnapshot}.",
                            rutaSnapshotFisica
                        );
                    }
                }


                _logger.LogError(
                    ex,
                    "Error al procesar la aprobación presupuestal firmada {DetalleId}.",
                    detalleId
                );


                return new JsonResult(
                    new
                    {
                        success = false,

                        message =
                            ex is InvalidOperationException
                                ? ex.Message
                                : "Ocurrió un error al procesar la aprobación presupuestal."
                    }
                )
                {
                    StatusCode =
                        ex is InvalidOperationException
                            ? StatusCodes.Status409Conflict
                            : StatusCodes.Status500InternalServerError
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
                                        "EnRevision"
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

                        TipoDocumentoSolicitud =
                            Input.TipoDocumentoSolicitud,

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

            if (
                ArchivosCotizacion == null
                ||
                ArchivosCotizacion.Count == 0
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
                int id
            )
        {
            AppUser? usuarioActual =
                await ObtenerUsuarioActualAsync();


            if (
                usuarioActual ==
                null
            )
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
                            x.Id ==
                                id
                            &&
                            !x.Eliminado
                    );


            if (
                !solicitudExiste
            )
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
                            x.Id ==
                                id
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
                            x.Id ==
                                id
                            &&
                            !x.Eliminado
                            &&
                            x.UsuarioAsignadoId ==
                                usuarioActual.Id
                    );


            // =====================================================
            // 5. OBSERVADOR PRESUPUESTAL ACTIVO
            // =====================================================

            bool esObservadorPresupuestal =
                await (
                    from observador
                        in _context
                            .AdqAprobacionesPresupuestalesObservadores
                            .AsNoTracking()

                    join aprobacionPresupuestal
                        in _context
                            .AdqAprobacionesPresupuestales
                            .AsNoTracking()
                        on observador.AprobacionPresupuestalId
                        equals aprobacionPresupuestal.Id

                    where
                        aprobacionPresupuestal.SolicitudId ==
                            id
                        &&
                        observador.UsuarioId ==
                            usuarioActual.Id
                        &&
                        observador.Activo
                        &&
                        !observador.Eliminado
                        &&
                        !aprobacionPresupuestal.Eliminado

                    select observador.Id
                )
                .AnyAsync();


            // =====================================================
            // 6. APROBADOR PRESUPUESTAL
            // =====================================================

            bool esAprobadorPresupuestal =
                await (
                    from detalle
                        in _context
                            .AdqAprobacionesPresupuestalesDetalle
                            .AsNoTracking()

                    join aprobacionPresupuestal
                        in _context
                            .AdqAprobacionesPresupuestales
                            .AsNoTracking()
                        on detalle.AprobacionPresupuestalId
                        equals aprobacionPresupuestal.Id

                    where
                        aprobacionPresupuestal.SolicitudId ==
                            id
                        &&
                        detalle.UsuarioAprobadorId ==
                            usuarioActual.Id
                        &&
                        !detalle.Eliminado
                        &&
                        !aprobacionPresupuestal.Eliminado

                    select detalle.Id
                )
                .AnyAsync();


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
                esAgenteAsignado
                ||
                esObservadorPresupuestal
                ||
                esAprobadorPresupuestal;


            if (
                !puedeConsultar
            )
            {
                _logger.LogWarning(
                    "Acceso denegado al detalle de solicitud. " +
                    "SolicitudId: {SolicitudId}, UsuarioId: {UsuarioId}, " +
                    "Propietario: {EsPropietario}, " +
                    "Aprobador: {EsAprobador}, " +
                    "Adquisiciones: {EsUsuarioAdquisiciones}, " +
                    "Asignado: {EsAgenteAsignado}, " +
                    "ObservadorPresupuestal: {EsObservadorPresupuestal}, " +
                    "AprobadorPresupuestal: {EsAprobadorPresupuestal}",
                    id,
                    usuarioActual.Id,
                    esPropietario,
                    esAprobador,
                    esUsuarioAdquisiciones,
                    esAgenteAsignado,
                    esObservadorPresupuestal,
                    esAprobadorPresupuestal
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
                            x.Id ==
                                id
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

                                x.TipoDocumentoSolicitud,

                                x.Justificacion,

                                x.FechaSolicitud,

                                x.AreaId,

                                Area =
                                    x.Area.Nombre,

                                x.EstatusId,

                                Estatus =
                                    x.Estatus.Nombre,


                                // =========================================
                                // SOLICITUD DE PAGO
                                // =========================================

                                PdfSolicitudPagoGenerado =
                                    _context.AdqSolicitudesPago
                                        .Any(
                                            solicitudPago =>
                                                solicitudPago.SolicitudId ==
                                                    x.Id
                                                &&
                                                !solicitudPago.Eliminado
                                                &&
                                                solicitudPago.PdfGenerado
                                        ),


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


            if (
                solicitud ==
                null
            )
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

            bool esAprobadorPresupuestal =
                await (
                    from detalle
                        in _context
                            .AdqAprobacionesPresupuestalesDetalle
                            .AsNoTracking()

                    join aprobacionPresupuestal
                        in _context
                            .AdqAprobacionesPresupuestales
                            .AsNoTracking()
                        on detalle.AprobacionPresupuestalId
                        equals aprobacionPresupuestal.Id

                    where
                        aprobacionPresupuestal.SolicitudId ==
                            solicitud.Id
                        &&
                        detalle.UsuarioAprobadorId ==
                            usuarioActual.Id
                        &&
                        !detalle.Eliminado
                        &&
                        !aprobacionPresupuestal.Eliminado

                    select detalle.Id
                )
                .AnyAsync();


            if (
                !esSolicitante
                &&
                !esAprobador
                &&
                !esUsuarioAdquisiciones
                &&
                !esAgenteAsignado
                &&
                !esAprobadorPresupuestal
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

        // =========================================================
        // CONSULTAR PERMISOS DE ADQUISICIONES
        // GET ?handler=PermisosUsuariosAdquisiciones
        // =========================================================

        public async Task<IActionResult>
            OnGetPermisosUsuariosAdquisicionesAsync()
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


            // =====================================================
            // SOLO ADMINISTRADORES DE ADQUISICIONES
            // =====================================================

            bool puedeAdministrar =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.PuedeAdministrar
                    );


            if (!puedeAdministrar)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para administrar los permisos de Adquisiciones."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // USUARIOS ACTIVOS
            // =====================================================

            List<AppUser> usuarios =
                await _userManager.Users
                    .AsNoTracking()
                    .Where(
                        x =>
                            !x.IsBanned
                    )
                    .OrderBy(
                        x =>
                            x.UserName
                    )
                    .ThenBy(
                        x =>
                            x.Email
                    )
                    .ToListAsync();


            // =====================================================
            // PERMISOS ACTUALES
            // =====================================================

            Dictionary<string, AdqPermisoUsuario>
                permisosPorUsuario =
                    await _context.AdqPermisosUsuarios
                        .AsNoTracking()
                        .ToDictionaryAsync(
                            x =>
                                x.UsuarioId
                        );


            List<object> resultado =
                new();


            foreach (
                AppUser usuario
                in usuarios
            )
            {
                permisosPorUsuario.TryGetValue(
                    usuario.Id,
                    out AdqPermisoUsuario? permiso
                );


                string nombre =
                    !string.IsNullOrWhiteSpace(
                        usuario.UserName
                    )
                        ? usuario.UserName
                        : usuario.Email
                            ??
                            "Usuario";


                resultado.Add(
                    new
                    {
                        id = usuario.Id,

                        nombre,

                        correo =
                            usuario.Email
                            ??
                            usuario.UserName
                            ??
                            string.Empty,

                        puedeVisualizar =
                            permiso?.PuedeVisualizar
                            ??
                            false,

                        puedeCrearSolicitud =
                            permiso?.PuedeCrearSolicitud
                            ??
                            false,

                        puedeGestionarSolicitudes =
                            permiso?.PuedeGestionarSolicitudes
                            ??
                            false,

                        puedeAprobar =
                            permiso?.PuedeAprobar
                            ??
                            false,

                        puedeAsignar =
                            permiso?.PuedeAsignar
                            ??
                            false,

                        puedeCotizar =
                            permiso?.PuedeCotizar
                            ??
                            false,

                        puedeGestionarProveedores =
                            permiso?.PuedeGestionarProveedores
                            ??
                            false,

                        puedeGenerarSolicitudPago =
                            permiso?.PuedeGenerarSolicitudPago
                            ??
                            false,

                        puedeVerReportes =
                            permiso?.PuedeVerReportes
                            ??
                            false,

                        puedeAprobarPresupuesto =
                            permiso?.PuedeAprobarPresupuesto
                            ??
                            false,

                        nivelPresupuestal =
                            permiso?.NivelPresupuestal,

                        puedeAdministrar =
                            permiso?.PuedeAdministrar
                            ??
                            false
                    }
                );
            }


            return new JsonResult(
                new
                {
                    success =
                        true,

                    data =
                        resultado
                }
            );
        }

        // =========================================================
        // GUARDAR PERMISOS DE ADQUISICIONES
        // POST ?handler=GuardarPermisosUsuariosAdquisiciones
        // =========================================================

        public async Task<IActionResult>
            OnPostGuardarPermisosUsuariosAdquisicionesAsync(
                [FromBody]
        GuardarPermisosAdquisicionesRequest request)
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


            // =====================================================
            // VALIDAR PERMISO DE ADMINISTRACIÓN
            // =====================================================

            bool puedeAdministrar =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioId ==
                                usuarioActual.Id
                            &&
                            x.PuedeAdministrar
                    );


            if (!puedeAdministrar)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No tienes permisos para administrar los permisos de Adquisiciones."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status403Forbidden
                };
            }


            // =====================================================
            // VALIDAR REQUEST
            // =====================================================

            if (
                request == null
                ||
                request.Permisos == null
                ||
                request.Permisos.Count == 0
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "No se recibieron permisos para guardar."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // VALIDAR USUARIOS REPETIDOS
            // =====================================================

            bool existenUsuariosDuplicados =
                request.Permisos
                    .Where(
                        x =>
                            !string.IsNullOrWhiteSpace(
                                x.UsuarioId
                            )
                    )
                    .GroupBy(
                        x =>
                            x.UsuarioId
                    )
                    .Any(
                        x =>
                            x.Count() >
                            1
                    );


            if (existenUsuariosDuplicados)
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Se detectaron usuarios duplicados en la configuración de permisos."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            // =====================================================
            // VALIDAR NIVELES PRESUPUESTALES
            // =====================================================

            foreach (
                PermisoAdquisicionesInput item
                in request.Permisos
            )
            {
                if (
                    string.IsNullOrWhiteSpace(
                        item.UsuarioId
                    )
                )
                {
                    return new JsonResult(
                        new
                        {
                            success = false,
                            message =
                                "Se recibió un usuario inválido."
                        }
                    )
                    {
                        StatusCode =
                            StatusCodes.Status400BadRequest
                    };
                }


                if (
                    item.PuedeAprobarPresupuesto
                    &&
                    (
                        !item.NivelPresupuestal.HasValue
                        ||
                        item.NivelPresupuestal.Value <
                            1
                        ||
                        item.NivelPresupuestal.Value >
                            4
                    )
                )
                {
                    return new JsonResult(
                        new
                        {
                            success = false,
                            message =
                                "Los usuarios que aprueban presupuesto deben tener un nivel presupuestal entre 1 y 4."
                        }
                    )
                    {
                        StatusCode =
                            StatusCodes.Status400BadRequest
                    };
                }
            }


            // =====================================================
            // VALIDAR QUE LOS USUARIOS EXISTAN
            // =====================================================

            List<string> idsUsuarios =
                request.Permisos
                    .Select(
                        x =>
                            x.UsuarioId.Trim()
                    )
                    .Distinct()
                    .ToList();


            List<string> usuariosExistentes =
                await _userManager.Users
                    .AsNoTracking()
                    .Where(
                        x =>
                            idsUsuarios.Contains(
                                x.Id
                            )
                            &&
                            !x.IsBanned
                    )
                    .Select(
                        x =>
                            x.Id
                    )
                    .ToListAsync();


            if (
                usuariosExistentes.Count !=
                idsUsuarios.Count
            )
            {
                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Uno o más usuarios no existen o se encuentran deshabilitados."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }


            DateTime ahora =
                DateTime.Now;


            await using var transaccion =
                await _context.Database
                    .BeginTransactionAsync();


            try
            {
                // =================================================
                // PERMISOS EXISTENTES
                // =================================================

                Dictionary<string, AdqPermisoUsuario>
                    permisosExistentes =
                        await _context.AdqPermisosUsuarios
                            .Where(
                                x =>
                                    idsUsuarios.Contains(
                                        x.UsuarioId
                                    )
                            )
                            .ToDictionaryAsync(
                                x =>
                                    x.UsuarioId
                            );


                int totalGuardados =
                    0;


                foreach (
                    PermisoAdquisicionesInput item
                    in request.Permisos
                )
                {
                    string usuarioId =
                        item.UsuarioId.Trim();


                    if (
                        !permisosExistentes.TryGetValue(
                            usuarioId,
                            out AdqPermisoUsuario? permiso
                        )
                    )
                    {
                        permiso =
                            new AdqPermisoUsuario
                            {
                                UsuarioId =
                                    usuarioId,

                                FechaCreacion =
                                    ahora
                            };


                        _context.AdqPermisosUsuarios.Add(
                            permiso
                        );


                        permisosExistentes[
                            usuarioId
                        ] =
                            permiso;
                    }


                    // =============================================
                    // PERMISOS GENERALES
                    // =============================================

                    permiso.PuedeVisualizar =
                        item.PuedeVisualizar;

                    permiso.PuedeCrearSolicitud =
                        item.PuedeCrearSolicitud;

                    permiso.PuedeGestionarSolicitudes =
                        item.PuedeGestionarSolicitudes;

                    permiso.PuedeAprobar =
                        item.PuedeAprobar;

                    permiso.PuedeAsignar =
                        item.PuedeAsignar;

                    permiso.PuedeCotizar =
                        item.PuedeCotizar;

                    permiso.PuedeGestionarProveedores =
                        item.PuedeGestionarProveedores;

                    permiso.PuedeGenerarSolicitudPago =
                        item.PuedeGenerarSolicitudPago;

                    permiso.PuedeVerReportes =
                        item.PuedeVerReportes;

                    permiso.PuedeAdministrar =
                        item.PuedeAdministrar;


                    // =============================================
                    // APROBACIÓN PRESUPUESTAL
                    // =============================================

                    permiso.PuedeAprobarPresupuesto =
                        item.PuedeAprobarPresupuesto;


                    permiso.NivelPresupuestal =
                        item.PuedeAprobarPresupuesto
                            ? item.NivelPresupuestal
                            : null;


                    // =============================================
                    // AUDITORÍA DE MODIFICACIÓN
                    // =============================================

                    permiso.FechaModificacion =
                        ahora;

                    permiso.UsuarioModificacionId =
                        usuarioActual.Id;


                    totalGuardados++;
                }


                await _context
                    .SaveChangesAsync();


                await transaccion
                    .CommitAsync();


                return new JsonResult(
                    new
                    {
                        success =
                            true,

                        message =
                            totalGuardados == 1
                                ? "Los permisos del usuario se guardaron correctamente."
                                : $"Se guardaron correctamente los permisos de {totalGuardados} usuarios.",

                        totalGuardados
                    }
                );
            }
            catch (Exception ex)
            {
                await transaccion
                    .RollbackAsync();


                _logger.LogError(
                    ex,
                    "Error al guardar permisos de Adquisiciones."
                );


                return new JsonResult(
                    new
                    {
                        success = false,
                        message =
                            "Ocurrió un error al guardar los permisos de Adquisiciones."
                    }
                )
                {
                    StatusCode =
                        StatusCodes.Status500InternalServerError
                };
            }
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


            await CargarHistorialAprobacionesAsync(
                usuarioActual
            );


            await CargarPermisosAdquisicionesAsync(
                usuarioActual
            );

            // =========================================================
            // VALIDAR SI EL USUARIO ES APROBADOR PRESUPUESTAL
            // =========================================================

            EsAprobadorPresupuestal =
                await _context
                    .AdqConfiguracionAprobacionPresupuestal
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.UsuarioResponsableId ==
                                usuarioActual.Id
                            &&
                            x.Activo
                            &&
                            !x.Eliminado
                    );

            // =========================================================
            // APROBACIONES PRESUPUESTALES PENDIENTES
            // =========================================================

            await CargarAprobacionesPresupuestalesPendientesAsync(
                usuarioActual
            );


            // =========================================================
            // SEGUIMIENTO PRESUPUESTAL - ASISTENTES / OBSERVADORES
            // =========================================================

            await CargarSeguimientosPresupuestalesAsync(
                usuarioActual
            );


            await CargarBandejaAdquisicionesAsync();

            // =========================================================
            // MIS ÓRDENES ASIGNADAS
            // =========================================================

            await CargarOrdenesAsignadasAsync(
                usuarioActual
            );


            // =========================================================
            // DASHBOARD GENERAL HU #5
            // =========================================================

            if (
                EsUsuarioAdquisiciones
                ||
                EsAgenteCompras
            )
            {
                await CargarDashboardAdquisicionesAsync();
            }


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
        // DASHBOARD GENERAL DE ADQUISICIONES - HU #5
        // =========================================================

        private async Task CargarDashboardAdquisicionesAsync()
        {
            List<AdqSolicitud> solicitudesDashboard =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .Include(
                        x =>
                            x.Estatus
                    )
                    .Where(
                        x =>
                            !x.Eliminado
                    )
                    .ToListAsync();


            DashboardTotalOrdenes =
                solicitudesDashboard.Count;


            DashboardPendientes =
                solicitudesDashboard.Count(
                    x =>
                        x.EstatusId == 2
                        ||
                        x.EstatusId == 3
                        ||
                        x.EstatusId == 4
                );


            DashboardEnCurso =
                solicitudesDashboard.Count(
                    x =>
                        x.EstatusId >= 5
                        &&
                        x.EstatusId <= 16
                        &&
                        x.EstatusId != 6
                        &&
                        x.EstatusId != 7
                );


            DashboardRechazadas =
                solicitudesDashboard.Count(
                    x =>
                        x.EstatusId == 6
                );


            DashboardCanceladas =
                solicitudesDashboard.Count(
                    x =>
                        x.EstatusId == 7
                );


            DashboardFinalizadas =
                solicitudesDashboard.Count(
                    x =>
                        x.EstatusId == 17
                );


            DashboardEnCotizacion =
                solicitudesDashboard.Count(
                    x =>
                        x.EstatusId == 9
                        ||
                        x.EstatusId == 10
                );


            DashboardEnAprobacionPresupuestal =
                solicitudesDashboard.Count(
                    x =>
                        x.EstatusId == 11
                        ||
                        x.EstatusId == 12
                        ||
                        x.EstatusId == 13
                );


            DashboardSolicitudesPago =
                await _context.AdqSolicitudesPago
                    .AsNoTracking()
                    .CountAsync(
                        x =>
                            !x.Eliminado
                            &&
                            x.PdfGenerado
                    );


            DashboardPorcentajeFinalizadas =
                DashboardTotalOrdenes >
                0
                    ? decimal.Round(
                        (
                            DashboardFinalizadas * 100m
                        )
                        /
                        DashboardTotalOrdenes,
                        2
                    )
                    : 0m;


            DashboardDistribucionEstatus =
                solicitudesDashboard
                    .Where(
                        x =>
                            x.Estatus != null
                    )
                    .GroupBy(
                        x =>
                            new
                            {
                                x.EstatusId,
                                Estatus =
                                    x.Estatus!.Nombre
                            }
                    )
                    .Select(
                        grupo =>
                            new DashboardEstatusDto
                            {
                                EstatusId =
                                    grupo.Key.EstatusId,

                                Estatus =
                                    grupo.Key.Estatus,

                                Total =
                                    grupo.Count(),

                                Porcentaje =
                                    DashboardTotalOrdenes >
                                    0
                                        ? decimal.Round(
                                            (
                                                grupo.Count() * 100m
                                            )
                                            /
                                            DashboardTotalOrdenes,
                                            2
                                        )
                                        : 0m
                            }
                    )
                    .OrderBy(
                        x =>
                            x.EstatusId
                    )
                    .ToList();
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

            Input.TipoDocumentoSolicitud =
                Input.TipoDocumentoSolicitud?
                    .Trim()
                ??
                string.Empty;


                        string[] tiposDocumentoPermitidos =
                        {
                "Factura",
                "Contrato",
                "Cotizaciones",
                "Otros"
            };


            string? tipoDocumentoNormalizado =
                tiposDocumentoPermitidos
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                x,
                                Input.TipoDocumentoSolicitud,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );


            if (
                tipoDocumentoNormalizado ==
                null
            )
            {
                ModelState.AddModelError(
                    "Input.TipoDocumentoSolicitud",
                    "El tipo de solicitud seleccionado no es válido."
                );
            }
            else
            {
                /*
                 * Conservamos siempre la escritura oficial:
                 * Factura / Contrato / Cotizaciones / Otros.
                 */
                Input.TipoDocumentoSolicitud =
                    tipoDocumentoNormalizado;
            }


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
        // VALIDAR TIPO DE SOLICITUD
        // =========================================================

        private void ValidarTipoDocumentoSolicitud()
        {
            string[] tiposPermitidos =
            {
        "Factura",
        "Contrato",
        "Cotizaciones",
        "Otros"
    };


            string? valorNormalizado =
                tiposPermitidos
                    .FirstOrDefault(
                        x =>
                            string.Equals(
                                x,
                                Input.TipoDocumentoSolicitud,
                                StringComparison.OrdinalIgnoreCase
                            )
                    );


            if (
                valorNormalizado ==
                null
            )
            {
                ModelState.AddModelError(
                    "Input.TipoDocumentoSolicitud",
                    "Debes seleccionar un tipo de solicitud válido."
                );

                return;
            }


            Input.TipoDocumentoSolicitud =
                valorNormalizado;
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
        // CONVERTIR FIRMA BASE64 A PNG
        // =========================================================

        private static byte[]
            ConvertirFirmaBase64APng(
                string? firmaBase64
            )
        {
            if (
                string.IsNullOrWhiteSpace(
                    firmaBase64
                )
            )
            {
                throw new InvalidOperationException(
                    "No se recibió la imagen de la firma."
                );
            }


            string contenido =
                firmaBase64.Trim();


            const string prefijoPng =
                "data:image/png;base64,";


            if (
                contenido.StartsWith(
                    "data:",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                if (
                    !contenido.StartsWith(
                        prefijoPng,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    throw new InvalidOperationException(
                        "La firma debe estar en formato PNG."
                    );
                }


                contenido =
                    contenido.Substring(
                        prefijoPng.Length
                    );
            }


            byte[] bytes;


            try
            {
                bytes =
                    Convert.FromBase64String(
                        contenido
                    );
            }
            catch (
                FormatException
            )
            {
                throw new InvalidOperationException(
                    "La imagen de la firma no contiene información Base64 válida."
                );
            }


            /*
             * Firma estándar de un archivo PNG:
             *
             * 89 50 4E 47 0D 0A 1A 0A
             */

            byte[] encabezadoPng =
            {
        0x89,
        0x50,
        0x4E,
        0x47,
        0x0D,
        0x0A,
        0x1A,
        0x0A
    };


            if (
                bytes.Length <
                encabezadoPng.Length
            )
            {
                throw new InvalidOperationException(
                    "El archivo de firma no es una imagen PNG válida."
                );
            }


            for (
                int indice = 0;
                indice <
                    encabezadoPng.Length;
                indice++
            )
            {
                if (
                    bytes[indice] !=
                    encabezadoPng[indice]
                )
                {
                    throw new InvalidOperationException(
                        "El archivo de firma no es una imagen PNG válida."
                    );
                }
            }


            return bytes;
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