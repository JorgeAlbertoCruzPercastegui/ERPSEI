using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ERPSEI.Authorization
{
	public class AccessHandlerEmpresas : AuthorizationHandler<AccessRequirementEmpresas>
	{
		readonly AppRoleManager _roleManager;
		readonly AppUserManager _userManager;

		public AccessHandlerEmpresas(AppRoleManager roleManager, AppUserManager userManager) 
		{ 
			_roleManager = roleManager;
			_userManager = userManager;
		}

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AccessRequirementEmpresas requirement)
		{
            AppUser? usr = await _userManager.GetUserAsync(context.User);
            IList<string> rolesUsuario = usr != null ? await _userManager.GetRolesAsync(usr) : [];
            List<AccesoModulo> accesos = [];
			List<AccesoModulo> accesosBancos = [];
			foreach (string rol in rolesUsuario)
            {
				AppRole? foundRole = await _roleManager.GetByNameAsync(rol);
				accesos.AddRange(foundRole?.Accesos.Where(acceso => acceso.Modulo?.NombreNormalizado == "empresas" && (acceso.PuedeTodo == 1 || acceso.PuedeConsultar == 1 || acceso.PuedeEditar == 1 || acceso.PuedeEliminar == 1 || acceso.PuedeAutorizar == 1)) ?? []);
				accesosBancos.AddRange(foundRole?.Accesos.Where(acceso => acceso.Modulo?.NombreNormalizado == "bancos" && (acceso.PuedeTodo == 1 || acceso.PuedeConsultar == 1 || acceso.PuedeEditar == 1 || acceso.PuedeEliminar == 1 || acceso.PuedeAutorizar == 1)) ?? []);
			}

            var identity = context.User.Identity as ClaimsIdentity;

			//Elimina las claims de acceso que puedan existir de otros módulos.
			List<Claim> claims = (from c in context.User.Claims
                                  where
                                  c.Type == "PuedeTodo" ||
                                  c.Type == "PuedeConsultar" ||
                                  c.Type == "PuedeEditar" ||
                                  c.Type == "PuedeEliminar" ||
                                  c.Type == "PuedeAutorizar" ||
								  c.Type == "PuedeTodoBancos" ||
								  c.Type == "PuedeConsultarBancos" ||
								  c.Type == "PuedeEditarBancos" ||
								  c.Type == "PuedeEliminarBancos" ||
								  c.Type == "PuedeAutorizarBancos"
								  select c).ToList();
            foreach (Claim claim in claims) { identity?.RemoveClaim(claim); }

			//Si se encontraron accesos para el módulo visitado
			if (accesos.Count >= 1 || accesosBancos.Count >= 1)
			{
                //Crea un nuevo set de claims de acceso para el módulo.
                claims = [
					new Claim("PuedeTodo", accesos.Where(a => a.PuedeTodo == 1).ToList().Count.ToString()),
                    new Claim("PuedeConsultar", accesos.Where(a => a.PuedeConsultar == 1).ToList().Count.ToString()),
                    new Claim("PuedeEditar", accesos.Where(a => a.PuedeEditar == 1).ToList().Count.ToString()),
                    new Claim("PuedeEliminar", accesos.Where(a => a.PuedeEliminar == 1).ToList().Count.ToString()),
                    new Claim("PuedeAutorizar", accesos.Where(a => a.PuedeAutorizar == 1).ToList().Count.ToString()),
					new Claim("PuedeTodoBancos", accesosBancos.Where(a => a.PuedeTodo == 1).ToList().Count.ToString()),
					new Claim("PuedeConsultarBancos", accesosBancos.Where(a => a.PuedeConsultar == 1).ToList().Count.ToString()),
					new Claim("PuedeEditarBancos", accesosBancos.Where(a => a.PuedeEditar == 1).ToList().Count.ToString()),
					new Claim("PuedeEliminarBancos", accesosBancos.Where(a => a.PuedeEliminar == 1).ToList().Count.ToString()),
					new Claim("PuedeAutorizarBancos", accesosBancos.Where(a => a.PuedeAutorizar == 1).ToList().Count.ToString())
				];

                //Agrega los claims de accesos al identity del usuario.
                identity?.AddClaims(claims);

                //Confirma el acceso al módulo.
                context.Succeed(requirement);
			}
		}
	}
}
