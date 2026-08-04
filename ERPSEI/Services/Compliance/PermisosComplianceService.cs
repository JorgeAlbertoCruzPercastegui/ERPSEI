using ERPSEI.Data;
using ERPSEI.Data.Entities.ExpedientesBancarios;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERPSEI.Services.Compliance
{
    public class PermisosComplianceService :
        IPermisosComplianceService
    {
        private readonly ApplicationDbContext _context;

        public PermisosComplianceService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PermisosComplianceResultado>
    ObtenerPermisosAsync(
        ClaimsPrincipal user)
        {
            if (
                user == null ||
                user.Identity?.IsAuthenticated != true
            )
            {
                return PermisosComplianceResultado
                    .SinAcceso();
            }

            bool esAdministrador =
                user.IsInRole(
                    ServicesConfiguration.RolMaster
                ) ||
                user.IsInRole(
                    ServicesConfiguration.RolAdministrador
                ) ||
                user.IsInRole(
                    ServicesConfiguration.RolAdministradorBancos
                );

            if (esAdministrador)
            {
                return PermisosComplianceResultado
                    .AccesoTotal();
            }

            bool tieneRolCompliance =
                user.IsInRole(
                    ServicesConfiguration.RolUsuarioBancos
                ) ||
                user.IsInRole(
                    ServicesConfiguration
                        .RolUsuarioOperacionesInternas
                );

            if (!tieneRolCompliance)
            {
                return PermisosComplianceResultado
                    .SinAcceso();
            }

            string? usuarioId =
                user.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            if (string.IsNullOrWhiteSpace(usuarioId))
            {
                return PermisosComplianceResultado
                    .SinAcceso();
            }

            EbPermisoComplianceUsuario? permiso =
                await _context
                    .EbPermisosComplianceUsuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.UsuarioId == usuarioId
                    );

            /*
 * Cuando el usuario recibe por primera vez el rol
 * Usuario Bancos o Usuario Operaciones Internas,
 * su permiso inicial es únicamente Visualizar.
 */
            if (permiso == null)
            {
                return new PermisosComplianceResultado
                {
                    TieneAccesoModulo = true,
                    EsAdministrador = false,
                    PuedeAdministrarPermisos = false,
                    PuedeVisualizar = true,
                    PuedeCrearCargar = false,
                    PuedeModificar = false,
                    PuedeEliminar = false,
                    PuedeDescargar = false
                };
            }

            /*
             * Cuando ya existe un registro, se respetan
             * exactamente los permisos guardados.
             */
            return new PermisosComplianceResultado
            {
                TieneAccesoModulo = true,
                EsAdministrador = false,
                PuedeAdministrarPermisos = false,

                PuedeVisualizar =
                    permiso.PuedeVisualizar,

                PuedeCrearCargar =
                    permiso.PuedeCrearCargar,

                PuedeModificar =
                    permiso.PuedeModificar,

                PuedeEliminar =
                    permiso.PuedeEliminar,

                PuedeDescargar =
                    permiso.PuedeDescargar
            };
        }

        public async Task<bool>
            TieneAccesoModuloAsync(
                ClaimsPrincipal usuario)
        {
            PermisosComplianceResultado permisos =
                await ObtenerPermisosAsync(
                    usuario
                );

            return permisos.TieneAccesoModulo;
        }

        public Task<bool> EsAdministradorAsync(
            ClaimsPrincipal usuario)
        {
            return Task.FromResult(
                EsRolAdministrador(usuario)
            );
        }

        public Task<bool>
            PuedeAdministrarPermisosAsync(
                ClaimsPrincipal usuario)
        {
            return Task.FromResult(
                EsRolAdministrador(usuario)
            );
        }

        public async Task<bool>
            PuedeVisualizarAsync(
                ClaimsPrincipal usuario)
        {
            PermisosComplianceResultado permisos =
                await ObtenerPermisosAsync(
                    usuario
                );

            return permisos.PuedeVisualizar;
        }

        public async Task<bool>
            PuedeCrearCargarAsync(
                ClaimsPrincipal usuario)
        {
            PermisosComplianceResultado permisos =
                await ObtenerPermisosAsync(
                    usuario
                );

            return permisos.PuedeCrearCargar;
        }

        public async Task<bool>
            PuedeModificarAsync(
                ClaimsPrincipal usuario)
        {
            PermisosComplianceResultado permisos =
                await ObtenerPermisosAsync(
                    usuario
                );

            return permisos.PuedeModificar;
        }

        public async Task<bool>
            PuedeEliminarAsync(
                ClaimsPrincipal usuario)
        {
            PermisosComplianceResultado permisos =
                await ObtenerPermisosAsync(
                    usuario
                );

            return permisos.PuedeEliminar;
        }

        public async Task<bool>
            PuedeDescargarAsync(
                ClaimsPrincipal usuario)
        {
            PermisosComplianceResultado permisos =
                await ObtenerPermisosAsync(
                    usuario
                );

            return permisos.PuedeDescargar;
        }

        private static bool EsRolAdministrador(
            ClaimsPrincipal usuario)
        {
            return
                usuario.IsInRole(
                    ServicesConfiguration.RolMaster
                ) ||
                usuario.IsInRole(
                    ServicesConfiguration
                        .RolAdministrador
                ) ||
                usuario.IsInRole(
                    ServicesConfiguration
                        .RolAdministradorBancos
                );
        }
    }
}