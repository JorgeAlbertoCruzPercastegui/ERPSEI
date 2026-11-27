using ERPSEI.Data;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERPSEI.Services.Adquisiciones
{
    public sealed class AdquisicionesAlcanceService :
        IAdquisicionesAlcanceService
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<AppUser> _userManager;

        public AdquisicionesAlcanceService(
            ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================================================
        // OBTENER ALCANCE DEL USUARIO AUTENTICADO
        // =========================================================

        public async Task<AdquisicionesAlcance> ObtenerAlcanceAsync(
            ClaimsPrincipal principal,
            CancellationToken cancellationToken = default)
        {
            if (principal.Identity?.IsAuthenticated != true)
            {
                return new AdquisicionesAlcance();
            }

            string? usuarioId =
                _userManager.GetUserId(principal);

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                return new AdquisicionesAlcance();
            }

            // =====================================================
            // VALIDAR USUARIO ACTIVO
            // =====================================================

            bool usuarioValido =
                await _context.Users
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.Id == usuarioId &&
                            !x.IsBanned,
                        cancellationToken
                    );

            if (!usuarioValido)
            {
                return new AdquisicionesAlcance();
            }

            // =====================================================
            // CONSULTAR ROLES VIGENTES DESDE BASE DE DATOS
            // =====================================================

            var rolesUsuario =
                await (
                    from ur in _context.UserRoles.AsNoTracking()
                    join r in _context.Roles.AsNoTracking()
                        on ur.RoleId equals r.Id
                    where ur.UserId == usuarioId
                    select new
                    {
                        r.Id,
                        r.NormalizedName
                    }
                )
                .ToListAsync(cancellationToken);

            bool esAdministradorSistema =
                rolesUsuario.Any(
                    x => x.NormalizedName == "ADMINISTRADOR"
                );

            bool esAdministradorAdquisiciones =
                rolesUsuario.Any(
                    x => x.NormalizedName ==
                        "ADMINISTRADOR ADQUISICIONES"
                );

            bool esUsuarioAdquisiciones =
                rolesUsuario.Any(
                    x => x.NormalizedName ==
                        "USUARIO ADQUISICIONES"
                );

            bool esAprobadorAdquisiciones =
                rolesUsuario.Any(
                    x => x.NormalizedName ==
                        "APROBADOR ADQUISICIONES"
                );

            // =====================================================
            // OBTENER IDENTIFICADORES DE ROLES
            // =====================================================

            var rolesIds =
                rolesUsuario
                    .Select(x => x.Id)
                    .Distinct()
                    .ToList();

            // =====================================================
            // OBTENER ÁREAS AUTORIZADAS
            // =====================================================

            List<int> areasAutorizadas = new();

            if (rolesIds.Count > 0)
            {
                areasAutorizadas =
                    await _context.AdqRolesAreas
                        .AsNoTracking()
                        .Where(
                            x =>
                                x.Activo &&
                                !x.Eliminado &&
                                rolesIds.Contains(x.RolId)
                        )
                        .Select(x => x.AreaId)
                        .Distinct()
                        .OrderBy(x => x)
                        .ToListAsync(cancellationToken);
            }

            // =====================================================
            // CONSTRUIR RESULTADO
            // =====================================================

            return new AdquisicionesAlcance
            {
                UsuarioId = usuarioId,

                UsuarioValido = true,

                EsAdministradorSistema =
                    esAdministradorSistema,

                EsAdministradorAdquisiciones =
                    esAdministradorAdquisiciones,

                EsUsuarioAdquisiciones =
                    esUsuarioAdquisiciones,

                EsAprobadorAdquisiciones =
                    esAprobadorAdquisiciones,

                AreasAutorizadas =
                    areasAutorizadas.AsReadOnly()
            };
        }

        // =========================================================
        // VALIDAR ALCANCE SOBRE UN ÁREA
        // =========================================================

        public async Task<bool> TieneAlcanceAreaAsync(
            ClaimsPrincipal principal,
            int areaId,
            CancellationToken cancellationToken = default)
        {
            if (areaId <= 0)
            {
                return false;
            }

            AdquisicionesAlcance alcance =
                await ObtenerAlcanceAsync(
                    principal,
                    cancellationToken
                );

            return alcance.TieneAlcanceArea(areaId);
        }

        // =========================================================
        // HU #6 - VALIDAR ACCESO INDIVIDUAL A UNA SOLICITUD
        // =========================================================

        public async Task<AdquisicionesAccesoSolicitud>
            ObtenerAccesoSolicitudAsync(
                ClaimsPrincipal principal,
                int solicitudId,
                CancellationToken cancellationToken = default)
        {
            if (solicitudId <= 0)
            {
                return new AdquisicionesAccesoSolicitud();
            }

            // =====================================================
            // OBTENER ALCANCE GENERAL
            // =====================================================

            AdquisicionesAlcance alcance =
                await ObtenerAlcanceAsync(
                    principal,
                    cancellationToken
                );

            if (!alcance.UsuarioValido)
            {
                return new AdquisicionesAccesoSolicitud
                {
                    SolicitudId = solicitudId
                };
            }

            string usuarioId =
                alcance.UsuarioId;

            // =====================================================
            // CONSULTAR SOLICITUD
            // =====================================================

            var solicitud =
                await _context.AdqSolicitudes
                    .AsNoTracking()
                    .Where(
                        x =>
                            x.Id == solicitudId &&
                            !x.Eliminado
                    )
                    .Select(
                        x => new
                        {
                            x.Id,
                            x.AreaId,
                            x.UsuarioSolicitanteId,
                            x.UsuarioAsignadoId
                        }
                    )
                    .FirstOrDefaultAsync(
                        cancellationToken
                    );

            if (solicitud == null)
            {
                return new AdquisicionesAccesoSolicitud
                {
                    SolicitudId = solicitudId,
                    UsuarioValido = true
                };
            }

            // =====================================================
            // VALIDAR ALCANCE DEL ÁREA
            // =====================================================

            bool tieneAlcanceArea =
                alcance.AreasAutorizadas.Contains(
                    solicitud.AreaId
                );

            // =====================================================
            // VALIDAR SOLICITANTE
            // =====================================================

            bool esSolicitante =
                solicitud.UsuarioSolicitanteId == usuarioId;

            // =====================================================
            // VALIDAR GERENTE APROBADOR
            // =====================================================

            bool esGerenteAprobador =
                await _context.AdqAprobaciones
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId == solicitudId &&
                            x.UsuarioAprobadorId == usuarioId,
                        cancellationToken
                    );

            // =====================================================
            // VALIDAR AGENTE ASIGNADO
            // =====================================================

            bool esAgenteAsignado =
                solicitud.UsuarioAsignadoId == usuarioId
                &&
                await _context.AdqAsignaciones
                    .AsNoTracking()
                    .AnyAsync(
                        x =>
                            x.SolicitudId == solicitudId &&
                            x.UsuarioAsignadoId == usuarioId &&
                            x.Activa,
                        cancellationToken
                    );

            // =====================================================
            // VALIDAR APROBADOR PRESUPUESTAL
            // =====================================================

            bool esAprobadorPresupuestal =
                await (
                    from detalle in
                        _context.AdqAprobacionesPresupuestalesDetalle
                            .AsNoTracking()

                    join aprobacion in
                        _context.AdqAprobacionesPresupuestales
                            .AsNoTracking()

                        on detalle.AprobacionPresupuestalId
                        equals aprobacion.Id

                    where
                        aprobacion.SolicitudId == solicitudId &&
                        detalle.UsuarioAprobadorId == usuarioId &&
                        !detalle.Eliminado &&
                        !aprobacion.Eliminado

                    select detalle.Id
                )
                .AnyAsync(
                    cancellationToken
                );

            // =====================================================
            // VALIDAR OBSERVADOR PRESUPUESTAL
            // =====================================================

            bool esObservadorPresupuestal =
                await (
                    from observador in
                        _context.AdqAprobacionesPresupuestalesObservadores
                            .AsNoTracking()

                    join aprobacion in
                        _context.AdqAprobacionesPresupuestales
                            .AsNoTracking()

                        on observador.AprobacionPresupuestalId
                        equals aprobacion.Id

                    where
                        aprobacion.SolicitudId == solicitudId &&
                        observador.UsuarioId == usuarioId &&
                        observador.Activo &&
                        !observador.Eliminado &&
                        !aprobacion.Eliminado

                    select observador.Id
                )
                .AnyAsync(
                    cancellationToken
                );

            // =====================================================
            // OBTENER PERMISOS ABAC
            // =====================================================

            var permiso =
                await _context.AdqPermisosUsuarios
                    .AsNoTracking()
                    .Where(
                        x => x.UsuarioId == usuarioId
                    )
                    .Select(
                        x => new
                        {
                            x.PuedeVisualizar,
                            x.PuedeCrear,
                            x.PuedeEditar,
                            x.PuedeEliminar,
                            x.PuedeDescargar,
                            x.PuedeTodo
                        }
                    )
                    .FirstOrDefaultAsync(
                        cancellationToken
                    );

            bool puedeTodo =
                alcance.EsAdministradorSistema ||
                permiso?.PuedeTodo == true;

            // =====================================================
            // CONSTRUIR RESULTADO DE SEGURIDAD
            // =====================================================

            return new AdquisicionesAccesoSolicitud
            {
                SolicitudId =
                    solicitudId,

                SolicitudExiste =
                    true,

                UsuarioValido =
                    true,

                TieneAlcanceGlobal =
                    alcance.TieneAlcanceGlobal,

                TieneAlcanceArea =
                    tieneAlcanceArea,

                EsSolicitante =
                    esSolicitante,

                EsGerenteAprobador =
                    esGerenteAprobador,

                EsAgenteAsignado =
                    esAgenteAsignado,

                EsAprobadorPresupuestal =
                    esAprobadorPresupuestal,

                EsObservadorPresupuestal =
                    esObservadorPresupuestal,

                AbacPuedeVisualizar =
                    puedeTodo ||
                    permiso?.PuedeVisualizar == true,

                AbacPuedeCrear =
                    puedeTodo ||
                    permiso?.PuedeCrear == true,

                AbacPuedeEditar =
                    puedeTodo ||
                    permiso?.PuedeEditar == true,

                AbacPuedeEliminar =
                    puedeTodo ||
                    permiso?.PuedeEliminar == true,

                AbacPuedeDescargar =
                    puedeTodo ||
                    permiso?.PuedeDescargar == true
            };
        }

        // =========================================================
        // HU #6 - VALIDAR CONSULTA DE SOLICITUD
        // =========================================================

        public async Task<bool> PuedeConsultarSolicitudAsync(
            ClaimsPrincipal principal,
            int solicitudId,
            CancellationToken cancellationToken = default)
        {
            var acceso =
                await ObtenerAccesoSolicitudAsync(
                    principal,
                    solicitudId,
                    cancellationToken
                );

            return acceso.PuedeConsultar;
        }

        // =========================================================
        // HU #6 - VALIDAR GESTIÓN DE SOLICITUD
        // =========================================================

        public async Task<bool> PuedeGestionarSolicitudAsync(
            ClaimsPrincipal principal,
            int solicitudId,
            CancellationToken cancellationToken = default)
        {
            var acceso =
                await ObtenerAccesoSolicitudAsync(
                    principal,
                    solicitudId,
                    cancellationToken
                );

            return acceso.PuedeGestionar;
        }
    }
}