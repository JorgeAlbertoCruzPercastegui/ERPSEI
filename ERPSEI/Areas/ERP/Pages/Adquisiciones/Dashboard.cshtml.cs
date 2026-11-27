using ERPSEI.Data;
using ERPSEI.Services.Adquisiciones;
using ERPSEI.Data.Entities.Adquisiciones;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.ERP.Pages.Adquisiciones
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly AppUserManager _userManager;
        private readonly IAdquisicionesAlcanceService _alcanceService;
        private AdquisicionesAlcance _alcance = new();
        public bool MostrarControlPagosHu6 { get; private set; }
        public bool DescargarControlPagosHu6 { get; private set; }
        public List<DashboardOrdenDto> PagosVerificadosHu6 { get; private set; } = new();
        public int PagosNoVerificadosHu6 { get; private set; }



        public DashboardModel(
            ApplicationDbContext context,
            AppUserManager userManager,
            IAdquisicionesAlcanceService alcanceService
        )
        {
            _context =
                context;

            _userManager =
                userManager;
            _alcanceService = alcanceService;
        }


        // =========================================================
        // PERMISOS
        // =========================================================

        public bool EsUsuarioAdquisiciones
        {
            get;
            private set;
        }


        public bool EsAgenteCompras
        {
            get;
            private set;
        }


        // =========================================================
        // KPIs HU #5
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


        // =========================================================
        // DISTRIBUCIÓN POR ESTATUS
        // =========================================================

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
        // ÓRDENES
        // =========================================================

        public class DashboardOrdenDto
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


            public int AreaId
            {
                get;
                set;
            }


            public string Area
            {
                get;
                set;
            } = string.Empty;


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


            public DateTime FechaSolicitud
            {
                get;
                set;
            }


            public bool TieneSolicitudPago
            {
                get;
                set;
            }
        }


        public List<DashboardOrdenDto> DashboardOrdenes
        {
            get;
            private set;
        } = new();


        // =========================================================
        // ÁREAS SOLICITANTES
        // =========================================================

        public class DashboardAreaDto
        {
            public int AreaId
            {
                get;
                set;
            }


            public string Area
            {
                get;
                set;
            } = string.Empty;


            public int Total
            {
                get;
                set;
            }
        }


        public List<DashboardAreaDto> DashboardAreas
        {
            get;
            private set;
        } = new();


        // =========================================================
        // PROVEEDORES
        // =========================================================

        public class DashboardProveedorDto
        {
            public int? ProveedorId
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


            public string? ContactoProveedor
            {
                get;
                set;
            }


            public string? EmailProveedor
            {
                get;
                set;
            }


            public string? TelefonoProveedor
            {
                get;
                set;
            }


            public int TotalCotizaciones
            {
                get;
                set;
            }


            public decimal MontoCotizado
            {
                get;
                set;
            }


            public DateTime UltimaCotizacion
            {
                get;
                set;
            }
        }


        public List<DashboardProveedorDto> DashboardProveedores
        {
            get;
            private set;
        } = new();


        // =========================================================
        // GET
        // =========================================================

        public async Task<IActionResult> OnGetAsync()
        {
            AppUser? usuarioActual =
                await _userManager.GetUserAsync(
                    User
                );


            if (
                usuarioActual ==
                null
            )
            {
                return Challenge();
            }


            await CargarPermisosAsync(
                usuarioActual
            );


            if (
                !EsUsuarioAdquisiciones
                &&
                !EsAgenteCompras
            )
            {
                return Forbid();
            }


            await CargarDashboardAsync();


            return Page();
        }

        // =========================================================
        // PERMISOS
        // =========================================================

        private async Task CargarPermisosAsync(
            AppUser usuarioActual
        )
        {
            _alcance = await _alcanceService.ObtenerAlcanceAsync(User, HttpContext.RequestAborted);
            if (!_alcance.UsuarioValido || _alcance.UsuarioId != usuarioActual.Id) return;
            var permiso = await _context.AdqPermisosUsuarios.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UsuarioId == usuarioActual.Id, HttpContext.RequestAborted);
            bool visualizar = _alcance.EsAdministradorSistema || permiso?.PuedeTodo == true || permiso?.PuedeVisualizar == true;
            EsUsuarioAdquisiciones = visualizar && (_alcance.TieneAlcanceGlobal || _alcance.EsGestorArea);
            EsAgenteCompras = visualizar && _alcance.EsUsuarioAdquisiciones;
            MostrarControlPagosHu6 = visualizar && _alcance.TieneAlcanceGlobal;
            DescargarControlPagosHu6 = MostrarControlPagosHu6 && (_alcance.EsAdministradorSistema || permiso?.PuedeTodo == true || permiso?.PuedeDescargar == true);


        }


        // =========================================================
        // DASHBOARD GENERAL
        // =========================================================

        private async Task CargarDashboardAsync()
        {
            // =====================================================
            // SOLICITUDES
            // =====================================================

            var consulta = _context.AdqSolicitudes.AsNoTracking().Where(x => !x.Eliminado);
            if (!_alcance.TieneAlcanceGlobal)
            {
                var areas = _alcance.AreasAutorizadas.ToArray();
                var usuarioId = _alcance.UsuarioId;
                bool agente = _alcance.EsUsuarioAdquisiciones;
                consulta = consulta.Where(x => areas.Contains(x.AreaId) ||
                    (agente && x.UsuarioAsignadoId == usuarioId && _context.AdqAsignaciones.Any(a =>
                        a.SolicitudId == x.Id && a.UsuarioAsignadoId == usuarioId && a.Activa)));
            }
            List<AdqSolicitud> solicitudesDashboard = await consulta.Include(x => x.Estatus).Include(x => x.Area)
                .ToListAsync(HttpContext.RequestAborted);
            var idsAutorizados = consulta.Select(x => x.Id);
            var pagosAutorizados = _context.AdqSolicitudesPago.AsNoTracking()
                .Where(x => !x.Eliminado && x.PdfGenerado && idsAutorizados.Contains(x.SolicitudId));
            List<int> solicitudesConPago = await pagosAutorizados.Select(x => x.SolicitudId).Distinct()
                .ToListAsync(HttpContext.RequestAborted);

            if (MostrarControlPagosHu6)
            {
                foreach (var orden in solicitudesDashboard.Where(x => solicitudesConPago.Contains(x.Id)))
                {
                    var pdf = await AdquisicionesIntegridadPago.ObtenerPdfValidadoAsync(_context, orden.Id, HttpContext.RequestAborted);
                    if (pdf == null) { PagosNoVerificadosHu6++; continue; }
                    PagosVerificadosHu6.Add(new DashboardOrdenDto
                    {
                        Id = orden.Id,
                        Folio = orden.Folio,
                        Titulo = orden.Titulo,
                        AreaId = orden.AreaId,
                        Area = orden.Area.Nombre,
                        EstatusId = orden.EstatusId,
                        Estatus = orden.Estatus.Nombre,
                        FechaSolicitud = orden.FechaSolicitud,
                        TieneSolicitudPago = true
                    });
                }
            }

            // =====================================================
            // KPIs
            // =====================================================

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


            DashboardSolicitudesPago = await pagosAutorizados.CountAsync(HttpContext.RequestAborted);

            DashboardPorcentajeFinalizadas =
                DashboardTotalOrdenes >
                0
                    ? decimal.Round(
                        (
                            DashboardFinalizadas
                            *
                            100m
                        )
                        /
                        DashboardTotalOrdenes,
                        2
                    )
                    : 0m;


            // =====================================================
            // DISTRIBUCIÓN POR ESTATUS
            // =====================================================

            DashboardDistribucionEstatus =
                solicitudesDashboard
                    .Where(
                        x =>
                            x.Estatus !=
                            null
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
                                                grupo.Count()
                                                *
                                                100m
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


            // =====================================================
            // ÓRDENES PARA EL EXPLORADOR
            // =====================================================

            DashboardOrdenes =
                solicitudesDashboard
                    .OrderByDescending(
                        x =>
                            x.FechaSolicitud
                    )
                    .Select(
                        x =>
                            new DashboardOrdenDto
                            {
                                Id =
                                    x.Id,

                                Folio =
                                    x.Folio,

                                Titulo =
                                    x.Titulo,

                                AreaId =
                                    x.AreaId,

                                Area =
                                    x.Area?.Nombre
                                    ??
                                    "Sin área",

                                EstatusId =
                                    x.EstatusId,

                                Estatus =
                                    x.Estatus?.Nombre
                                    ??
                                    "Sin estatus",

                                FechaSolicitud =
                                    x.FechaSolicitud,

                                TieneSolicitudPago =
                                    solicitudesConPago.Contains(
                                        x.Id
                                    )
                            }
                    )
                    .ToList();


            // =====================================================
            // CLASIFICACIÓN POR ÁREA
            // =====================================================

            DashboardAreas =
                solicitudesDashboard
                    .GroupBy(
                        x =>
                            new
                            {
                                x.AreaId,

                                Area =
                                    x.Area?.Nombre
                                    ??
                                    "Sin área"
                            }
                    )
                    .Select(
                        grupo =>
                            new DashboardAreaDto
                            {
                                AreaId =
                                    grupo.Key.AreaId,

                                Area =
                                    grupo.Key.Area,

                                Total =
                                    grupo.Count()
                            }
                    )
                    .OrderBy(
                        x =>
                            x.Area
                    )
                    .ToList();


            // =====================================================
            // DIRECTORIO DE PROVEEDORES
            // =====================================================

            List<AdqCotizacion> cotizacionesProveedores =
                await _context.AdqCotizaciones
                    .AsNoTracking()
                    .Where(
                        x =>
                            !x.Eliminado
                            &&
                            idsAutorizados.Contains(x.SolicitudId) &&
                            !string.IsNullOrWhiteSpace(
                                x.NombreProveedor
                            )
                    )
                    .ToListAsync();


            DashboardProveedores =
                cotizacionesProveedores
                    .GroupBy(
                        x =>
                            new
                            {
                                x.ProveedorId,

                                NombreProveedor =
                                    x.NombreProveedor
                                        .Trim(),

                                RfcProveedor =
                                    string.IsNullOrWhiteSpace(
                                        x.RfcProveedor
                                    )
                                        ? null
                                        : x.RfcProveedor.Trim(),

                                EmailProveedor =
                                    string.IsNullOrWhiteSpace(
                                        x.EmailProveedor
                                    )
                                        ? null
                                        : x.EmailProveedor.Trim()
                            }
                    )
                    .Select(
                        grupo =>
                        {
                            AdqCotizacion ultima =
                                grupo
                                    .OrderByDescending(
                                        x =>
                                            x.FechaCreacion
                                    )
                                    .First();


                            return new DashboardProveedorDto
                            {
                                ProveedorId =
                                    grupo.Key.ProveedorId,

                                NombreProveedor =
                                    grupo.Key.NombreProveedor,

                                RfcProveedor =
                                    grupo.Key.RfcProveedor,

                                ContactoProveedor =
                                    ultima.ContactoProveedor,

                                EmailProveedor =
                                    ultima.EmailProveedor,

                                TelefonoProveedor =
                                    ultima.TelefonoProveedor,

                                TotalCotizaciones =
                                    grupo.Count(),

                                MontoCotizado =
                                    grupo.Sum(
                                        x =>
                                            x.Total
                                    ),

                                UltimaCotizacion =
                                    grupo.Max(
                                        x =>
                                            x.FechaCreacion
                                    )
                            };
                        }
                    )
                    .OrderBy(
                        x =>
                            x.NombreProveedor
                    )
                    .ToList();
        }
    }
}