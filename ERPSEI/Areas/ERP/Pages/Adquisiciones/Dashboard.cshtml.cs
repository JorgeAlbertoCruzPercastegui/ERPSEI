using ERPSEI.Data;
using ERPSEI.Data.Entities.Adquisiciones;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Data.Entities.Usuarios;
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


        public DashboardModel(
            ApplicationDbContext context,
            AppUserManager userManager
        )
        {
            _context =
                context;

            _userManager =
                userManager;
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
        // GET
        // =========================================================

        public async Task<IActionResult>
            OnGetAsync()
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
            AdqPermisoUsuario? permiso =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x =>
                            x.UsuarioId ==
                            usuarioActual.Id
                    );


            if (
                permiso ==
                null
            )
            {
                EsUsuarioAdquisiciones =
                    false;

                EsAgenteCompras =
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


            EsAgenteCompras =
                permiso.PuedeCotizar
                ||
                permiso.PuedeAdministrar;
        }


        // =========================================================
        // DASHBOARD GENERAL
        // =========================================================

        private async Task CargarDashboardAsync()
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
    }
}