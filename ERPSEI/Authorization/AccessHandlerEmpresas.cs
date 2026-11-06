using ERPSEI.Data;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ERPSEI.Authorization
{
    public class AccessHandlerEmpresas : AuthorizationHandler<AccessRequirementEmpresas>
    {
        private readonly AppUserManager _users;
        private readonly AppRoleManager _roles;
        private readonly IAccesoModuloManager _access;
        private readonly ApplicationDbContext _db;

        public AccessHandlerEmpresas(AppRoleManager roles, AppUserManager users,
            IAccesoModuloManager access, ApplicationDbContext db)
        {
            _roles = roles;
            _users = users;
            _access = access;
            _db = db;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AccessRequirementEmpresas requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true) return;
            var usuario = await _users.GetUserAsync(context.User);
            if (usuario == null) return;
            var roles = await _users.GetRolesAsync(usuario);
            bool admin = roles.Any(r => r.Equals("Administrador", StringComparison.OrdinalIgnoreCase) || r.Equals("Master", StringComparison.OrdinalIgnoreCase));
            bool adminEmpresas = roles.Any(r => r.Equals("Administrador Empresas", StringComparison.OrdinalIgnoreCase));

            var empresas = new List<AccesoModulo>();
            var bancos = new List<AccesoModulo>();
            if (!adminEmpresas && !admin)
            {
                foreach (var role in roles)
                {
                    var found = await _roles.GetByNameAsync(role);
                    if (found == null) continue;
                    var grants = await _access.GetByRolIdAsync(found.Id);
                    empresas.AddRange(grants.Where(a => a.Modulo?.NombreNormalizado == "empresas"));
                    bancos.AddRange(grants.Where(a => a.Modulo?.NombreNormalizado == "bancos"));
                }
            }

            var overrideUsuario = adminEmpresas && !admin
                ? await _db.Set<EmpresaPermisoUsuarioHU2>().AsNoTracking().FirstOrDefaultAsync(x => x.UserId == usuario.Id)
                : null;

            // Antes de asignar permisos particulares, conservar acceso anterior.
            // Una vez existe configuración individual, reemplaza por completo el acceso del rol Administrador Empresas.
            bool legacy = adminEmpresas && overrideUsuario == null;
            // Crear o editar implica poder visualizar, incluso en configuraciones antiguas.
            bool ver = admin || legacy || (overrideUsuario?.PuedeVisualizar ?? false)
                || (overrideUsuario?.PuedeCrear ?? false)
                || (overrideUsuario?.PuedeEditar ?? false)
                || empresas.Any(a => a.PuedeTodo == 1 || a.PuedeConsultar == 1);
            bool crear = admin || legacy || (overrideUsuario?.PuedeCrear ?? false);
            bool editar = admin || legacy || (overrideUsuario?.PuedeEditar ?? false) || empresas.Any(a => a.PuedeTodo == 1 || a.PuedeEditar == 1);
            bool cargar = admin || legacy || (overrideUsuario?.PuedeCargarDocumentos ?? false);
            bool descargar = admin || legacy || (overrideUsuario?.PuedeDescargar ?? false);
            bool desactivar = admin || legacy || (overrideUsuario?.PuedeDesactivar ?? false) || empresas.Any(a => a.PuedeTodo == 1 || a.PuedeEliminar == 1);
            bool editarBancos = admin || legacy || (overrideUsuario?.PuedeEditarBancos ?? false) || bancos.Any(a => a.PuedeTodo == 1 || a.PuedeEditar == 1);
            bool verBancos = admin || legacy || editarBancos || bancos.Any(a => a.PuedeTodo == 1 || a.PuedeConsultar == 1);
            bool puedeTodo = admin || legacy || (!adminEmpresas && empresas.Any(a => a.PuedeTodo == 1));
            bool puedeTodoBancos = admin || legacy || (!adminEmpresas && bancos.Any(a => a.PuedeTodo == 1));

            if (!(ver || crear || editar || cargar || descargar || desactivar || editarBancos || verBancos)) return;
            var identity = context.User.Identities.OfType<ClaimsIdentity>().FirstOrDefault(i => i.IsAuthenticated);
            if (identity == null) return;
            var values = new Dictionary<string, bool>
            {
                ["PuedeTodo"] = puedeTodo,
                ["PuedeConsultar"] = ver,
                ["PuedeEditar"] = editar,
                ["PuedeEliminar"] = desactivar,
                ["PuedeAutorizar"] = admin || legacy || (!adminEmpresas && empresas.Any(a => a.PuedeTodo == 1 || a.PuedeAutorizar == 1)),
                ["PuedeTodoBancos"] = puedeTodoBancos,
                ["PuedeConsultarBancos"] = verBancos,
                ["PuedeEditarBancos"] = editarBancos,
                ["PuedeEliminarBancos"] = admin || legacy || (!adminEmpresas && bancos.Any(a => a.PuedeTodo == 1 || a.PuedeEliminar == 1)),
                ["PuedeAutorizarBancos"] = admin || legacy || (!adminEmpresas && bancos.Any(a => a.PuedeTodo == 1 || a.PuedeAutorizar == 1)),
                ["EmpresasPuedeCrear"] = crear,
                ["EmpresasPuedeCargar"] = cargar,
                ["EmpresasPuedeDescargar"] = descargar
            };
            foreach (var claim in identity.Claims.Where(c => values.ContainsKey(c.Type)).ToList()) identity.RemoveClaim(claim);
            identity.AddClaims(values.Select(kv => new Claim(kv.Key, kv.Value ? "1" : "0")));
            context.Succeed(requirement);
        }
    }
}
