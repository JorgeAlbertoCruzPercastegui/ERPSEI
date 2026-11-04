
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ERPSEI.Authorization
{
    public class AccessHandlerEmpresas
        : AuthorizationHandler<AccessRequirementEmpresas>
    {
        private readonly AppUserManager _userManager;
        private readonly AppRoleManager _roleManager;
        private readonly IAccesoModuloManager _accesoModuloManager;

        public AccessHandlerEmpresas(
            AppRoleManager roleManager,
            AppUserManager userManager,
            IAccesoModuloManager accesoModuloManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _accesoModuloManager = accesoModuloManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AccessRequirementEmpresas requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true)
                return;

            AppUser? usuario =
                await _userManager.GetUserAsync(context.User);

            if (usuario == null)
                return;

            IList<string> rolesUsuario =
                await _userManager.GetRolesAsync(usuario);

            bool esAdministradorEmpresas =
                rolesUsuario.Any(r =>
                    r.Equals(
                        "Administrador Empresas",
                        StringComparison.OrdinalIgnoreCase));

            bool esAdministradorGeneral =
                rolesUsuario.Any(r =>
                    r.Equals(
                        "Administrador",
                        StringComparison.OrdinalIgnoreCase) ||
                    r.Equals(
                        "Master",
                        StringComparison.OrdinalIgnoreCase));

            bool controlTotalEmpresas =
                esAdministradorEmpresas ||
                esAdministradorGeneral;

            List<AccesoModulo> accesosEmpresas = new();
            List<AccesoModulo> accesosBancos = new();

            foreach (string rol in rolesUsuario)
            {
                AppRole? rolEncontrado =
                    await _roleManager.GetByNameAsync(rol);

                if (rolEncontrado == null)
                    continue;

                List<AccesoModulo> accesosRol =
                    await _accesoModuloManager
                        .GetByRolIdAsync(rolEncontrado.Id);

                accesosEmpresas.AddRange(
                    accesosRol.Where(a =>
                        a.Modulo?.NombreNormalizado == "empresas"));

                accesosBancos.AddRange(
                    accesosRol.Where(a =>
                        a.Modulo?.NombreNormalizado == "bancos"));
            }

            // PERMISOS ACUMULATIVOS DEL MÓDULO EMPRESAS

            bool puedeTodo =
                controlTotalEmpresas ||
                accesosEmpresas.Any(a => a.PuedeTodo == 1);

            bool puedeConsultar =
                controlTotalEmpresas ||
                accesosEmpresas.Any(a =>
                    a.PuedeTodo == 1 ||
                    a.PuedeConsultar == 1);

            bool puedeEditar =
                controlTotalEmpresas ||
                accesosEmpresas.Any(a =>
                    a.PuedeTodo == 1 ||
                    a.PuedeEditar == 1);

            bool puedeEliminar =
                controlTotalEmpresas ||
                accesosEmpresas.Any(a =>
                    a.PuedeTodo == 1 ||
                    a.PuedeEliminar == 1);

            bool puedeAutorizar =
                controlTotalEmpresas ||
                accesosEmpresas.Any(a =>
                    a.PuedeTodo == 1 ||
                    a.PuedeAutorizar == 1);

            // PERMISOS ACUMULATIVOS DE BANCOS EN EMPRESAS

            bool puedeTodoBancos =
                controlTotalEmpresas ||
                accesosBancos.Any(a => a.PuedeTodo == 1);

            bool puedeConsultarBancos =
                controlTotalEmpresas ||
                accesosBancos.Any(a =>
                    a.PuedeTodo == 1 ||
                    a.PuedeConsultar == 1);

            bool puedeEditarBancos =
                controlTotalEmpresas ||
                accesosBancos.Any(a =>
                    a.PuedeTodo == 1 ||
                    a.PuedeEditar == 1);

            bool puedeEliminarBancos =
                controlTotalEmpresas ||
                accesosBancos.Any(a =>
                    a.PuedeTodo == 1 ||
                    a.PuedeEliminar == 1);

            bool puedeAutorizarBancos =
                controlTotalEmpresas ||
                accesosBancos.Any(a =>
                    a.PuedeTodo == 1 ||
                    a.PuedeAutorizar == 1);

            // VALIDAR ACCESO AL MÓDULO

            bool tieneAccesoEmpresas =
                puedeTodo ||
                puedeConsultar ||
                puedeEditar ||
                puedeEliminar ||
                puedeAutorizar;

            bool tieneAccesoBancos =
                puedeTodoBancos ||
                puedeConsultarBancos ||
                puedeEditarBancos ||
                puedeEliminarBancos ||
                puedeAutorizarBancos;

            if (!tieneAccesoEmpresas && !tieneAccesoBancos)
                return;

            // ACTUALIZAR CLAIMS DE LA SOLICITUD

            ClaimsIdentity? identity =
                context.User.Identities
                    .FirstOrDefault(i =>
                        i.IsAuthenticated &&
                        i is ClaimsIdentity);

            if (identity == null)
                return;

            string[] tiposPermisos =
            {
                "PuedeTodo",
                "PuedeConsultar",
                "PuedeEditar",
                "PuedeEliminar",
                "PuedeAutorizar",
                "PuedeTodoBancos",
                "PuedeConsultarBancos",
                "PuedeEditarBancos",
                "PuedeEliminarBancos",
                "PuedeAutorizarBancos"
            };

            foreach (Claim claim in identity.Claims
                .Where(c => tiposPermisos.Contains(c.Type))
                .ToList())
            {
                identity.RemoveClaim(claim);
            }

            List<Claim> nuevosClaims = new()
            {
                new Claim("PuedeTodo", puedeTodo ? "1" : "0"),
                new Claim("PuedeConsultar", puedeConsultar ? "1" : "0"),
                new Claim("PuedeEditar", puedeEditar ? "1" : "0"),
                new Claim("PuedeEliminar", puedeEliminar ? "1" : "0"),
                new Claim("PuedeAutorizar", puedeAutorizar ? "1" : "0"),

                new Claim("PuedeTodoBancos", puedeTodoBancos ? "1" : "0"),
                new Claim("PuedeConsultarBancos", puedeConsultarBancos ? "1" : "0"),
                new Claim("PuedeEditarBancos", puedeEditarBancos ? "1" : "0"),
                new Claim("PuedeEliminarBancos", puedeEliminarBancos ? "1" : "0"),
                new Claim("PuedeAutorizarBancos", puedeAutorizarBancos ? "1" : "0")
            };

            identity.AddClaims(nuevosClaims);

            context.Succeed(requirement);
        }
    }
}
