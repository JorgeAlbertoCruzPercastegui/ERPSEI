using ERPSEI.Data;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using ERPSEI.Areas.Catalogos.Pages.Auditoria;
using ERPSEI.Data.Entities.Metricas;
using System.Security.Claims;

namespace ERPSEI.Areas.Catalogos.Pages
{
	[Authorize(Policy = "AccessPolicy")]
    public class RolesModel : PageModel
	{
		private readonly IAccesoModuloManager _accesoModuloManager;
		private readonly AppUserManager _userManager;
		private readonly IModuloManager _moduloManager;
		private readonly AppRoleManager _roleManager;
		private readonly IStringLocalizer<RolesModel> _strLocalizer;
		private readonly ILogger<RolesModel> _logger;
		private readonly ApplicationDbContext _db;
        private readonly AuditoriaContext _auditoriaContext;

        [BindProperty]
		public RolModel InputRol { get; set; }

		public class RolModel
		{
			public string Id { get; set; } = string.Empty;

			[Required(ErrorMessage = "Required")]
			[Display(Name = "FullNameField")]
			public string NombreRol { get; set; } = string.Empty;

			public ModuloModel?[] Modulos { get; set; } = [];
		}

		public class ModuloModel
		{
			public int? ModuloId { get; set; }
			public bool? PuedeTodo { get; set; } = false;
			public bool? PuedeConsultar { get; set; } = false;
			public bool? PuedeEditar { get; set; } = false;
			public bool? PuedeEliminar { get; set; } = false;
			public bool? PuedeAutorizar { get; set; } = false;
		}

		public RolesModel(
			IAccesoModuloManager accesoModuloManager,
			AppUserManager userManager,
			IModuloManager moduloManager,
			AppRoleManager roleManager,
			IStringLocalizer<RolesModel> stringLocalizer,
			ILogger<RolesModel> logger,
			ApplicationDbContext db,
            AuditoriaContext auditoriaContext
        )
		{ 
			_accesoModuloManager = accesoModuloManager;
			_userManager = userManager;
			_moduloManager = moduloManager;
			_roleManager = roleManager;
			_strLocalizer = stringLocalizer;
			_logger = logger;
			_db = db;
            _auditoriaContext = auditoriaContext;

            InputRol = new RolModel();
		}

		private async Task<string> GetLista()
		{
			string jsonResponse;
			List<string> jsonResultados = [];
			List<Modulo> modulos = await _moduloManager.GetAllAsync();

			foreach (AppRole r in _roleManager.Roles)
			{
				//Se obtiene un listado de accesos del rol.
				List<AccesoModulo> accesosRol = await _accesoModuloManager.GetByRolIdAsync(r.Id);
				List<string> jsonModulos = [];
                switch (r.Name)
                {
                    case ServicesConfiguration.RolMaster:
                    case ServicesConfiguration.RolUsuario:
                    case ServicesConfiguration.RolCandidato:
                    case ServicesConfiguration.RolAdministradorBancos:
                    case ServicesConfiguration.RolUsuarioBancos:
                    case ServicesConfiguration
        .RolUsuarioOperacionesInternas:

                        /*
                         * Roles internos del sistema.
                         * No se administran desde el catálogo general.
                         */
                        break;
                    default:
						foreach (Modulo m in modulos)
						{
							AccesoModulo? acceso = accesosRol.Where(a => a.Modulo?.NombreNormalizado == m.NombreNormalizado).FirstOrDefault();
							jsonModulos.Add(
								"{" +
									$"\"id\": \"{m.Id}\"," +
									$"\"nombre\": \"{m.NombreNormalizado}\"," +
									$"\"puedeTodo\": \"{acceso?.PuedeTodo == 1}\"," +
									$"\"puedeConsultar\": \"{acceso?.PuedeConsultar == 1}\"," +
									$"\"puedeEditar\": \"{acceso?.PuedeEditar == 1}\"," +
									$"\"puedeEliminar\": \"{acceso?.PuedeEliminar == 1}\"," +
									$"\"puedeAutorizar\": \"{acceso?.PuedeAutorizar == 1}\"" +
								"}"
							);
						}

						//El resto de roles se mostrará
						jsonResultados.Add(
							"{" +
								$"\"id\": \"{r.Id}\"," +
								$"\"rol\": \"{r.Name}\"," +
								$"\"modulos\": [" +
									string.Join(",", jsonModulos) +
								$"]" +
							"}"
						);
						break;
				}
			}

			jsonResponse = $"[{string.Join(",", jsonResultados)}]";

			return jsonResponse;
		}

		public async Task<JsonResult> OnPostFiltrar()
		{
			ServerResponse resp = new(true, _strLocalizer["FiltroUnsuccessfully"]);
			try
			{
				resp.Datos = await GetLista();
				resp.TieneError = false;
				resp.Mensaje = _strLocalizer["FiltroSuccessfully"];
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}

        /*public async Task<JsonResult> OnPostDeleteRoles(string[] ids)
		{
			ServerResponse resp = new(true, _strLocalizer["RolDeletedUnsuccessfully"]);
			try
			{
				await _db.Database.BeginTransactionAsync();

				foreach (string id in ids)
				{
					//Se obtiene el rol que se está intentando dar de baja
					AppRole? rol = await _roleManager.GetByIdAsync(id);
					//Se filtran todos los usuarios para obtener los que tienen asignado el rol que se intenta dar de baja
					IList<AppUser> users = await _userManager.GetUsersInRoleAsync(rol?.Name??string.Empty);
					//Se filtran todos los usuarios para obtener los que no están dados de baja
					List<AppUser> usuariosActivosRelacionados = users.Where(u => !u.IsBanned).ToList();
					//Si existen usuarios que tengan el rol asignado, se le notifica al usuario.
					if (usuariosActivosRelacionados.Count > 0)
					{
						List<string> names = [];
						foreach (AppUser u in usuariosActivosRelacionados) { names.Add($"<i>{u.UserName}</i>"); }
						resp.TieneError = true;
						resp.Mensaje = $"{_strLocalizer["RolIsRelated"]}<br/><br/><i>{rol?.Name}</i><br/><br/>{string.Join("<br/>", names)}";
						break;
					}
					else
					{
						//Procede a eliminar los accesos a módulos que se le hayan asignado al rol.
						await _accesoModuloManager.DeleteByRolIdAsync(rol?.Id??string.Empty);

						//En caso de no haber usuarios con el rol, procede a eliminar registro.
						if (rol != null) { await _roleManager.DeleteAsync(rol); }

						resp.TieneError = false;
						resp.Mensaje = _strLocalizer["RolDeletedSuccessfully"];
					}
				}

				if (resp.TieneError) { throw new Exception(resp.Mensaje); }

				await _db.Database.CommitTransactionAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				await _db.Database.RollbackTransactionAsync();
			}

			return new JsonResult(resp);
		}
		*/

        public async Task<JsonResult> OnPostDeleteRoles(string[] ids)
        {
            ServerResponse resp = new(true, _strLocalizer["RolDeletedUnsuccessfully"]);

            try
            {
                await _db.Database.BeginTransactionAsync();

                _auditoriaContext.Activar("Roles", "Eliminación");

                foreach (string id in ids)
                {
                    AppRole? rol = await _roleManager.GetByIdAsync(id);

                    IList<AppUser> users = await _userManager.GetUsersInRoleAsync(rol?.Name ?? string.Empty);

                    List<AppUser> usuariosActivosRelacionados = users
                        .Where(u => !u.IsBanned)
                        .ToList();

                    if (usuariosActivosRelacionados.Count > 0)
                    {
                        List<string> names = [];

                        foreach (AppUser u in usuariosActivosRelacionados)
                        {
                            names.Add($"<i>{u.UserName}</i>");
                        }

                        resp.TieneError = true;
                        resp.Mensaje = $"{_strLocalizer["RolIsRelated"]}<br/><br/><i>{rol?.Name}</i><br/><br/>{string.Join("<br/>", names)}";
                        break;
                    }
                    else
                    {
                        await _accesoModuloManager.DeleteByRolIdAsync(rol?.Id ?? string.Empty);

                        if (rol != null)
                        {
                            await _roleManager.DeleteAsync(rol);
                        }

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["RolDeletedSuccessfully"];
                    }
                }

                if (resp.TieneError)
                {
                    throw new Exception(resp.Mensaje);
                }

                _auditoriaContext.Desactivar();

                await _db.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                _auditoriaContext.Desactivar();

                _logger.LogError(ex.Message);
                await _db.Database.RollbackTransactionAsync();
            }

            return new JsonResult(resp);
        }

        /*public async Task<JsonResult> OnPostSave()
		{
			ServerResponse resp = new(true, _strLocalizer["RolSavedUnsuccessfully"]);

			if (!ModelState.IsValid)
			{
				resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors ?? []).Select(m => m.ErrorMessage).ToArray();
				return new JsonResult(resp);
			}
			try
			{
				//Se busca si ya existe un registro con el mismo nombre.
				AppRole? rol = await _roleManager.GetByNameAsync(InputRol.NombreRol);

				//Si ya existe un registro con el mismo nombre y los Id's no coinciden
				if (rol != null && rol.Id != InputRol.Id)
				{
					//Ya existe un elemento con el mismo nombre.
					resp.Mensaje = _strLocalizer["ErrorRolExistente"];
				}
				else
				{
					//Crea o actualiza el registro
					await CreateOrUpdateRole(InputRol);

					resp.TieneError = false;
					resp.Mensaje = _strLocalizer["RolSavedSuccessfully"];
				}
			}
			catch (Exception ex)
			{
				await _db.Database.RollbackTransactionAsync();
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}*/

        public async Task<JsonResult> OnPostSave()
        {
            ServerResponse resp = new(true, _strLocalizer["RolSavedUnsuccessfully"]);

            if (!ModelState.IsValid)
            {
                resp.Errores = ModelState.Keys
                    .SelectMany(k => ModelState[k]?.Errors ?? [])
                    .Select(m => m.ErrorMessage)
                    .ToArray();

                return new JsonResult(resp);
            }

            try
            {
                AppRole? rol = await _roleManager.GetByNameAsync(InputRol.NombreRol);

                if (rol != null && rol.Id != InputRol.Id)
                {
                    resp.Mensaje = _strLocalizer["ErrorRolExistente"];
                }
                else
                {
                    AppRole? rolActual = await _roleManager.GetByIdAsync(InputRol.Id);

                    string accionAuditoria = rolActual == null
                        ? "Alta"
                        : "Edición";

                    _auditoriaContext.Activar("Roles", accionAuditoria);

                    await CreateOrUpdateRole(InputRol);

                    _auditoriaContext.Desactivar();

                    if (accionAuditoria == "Edición")
                    {
                        await RegistrarAuditoriaPermisosRolAsync(
                            InputRol.Id,
                            InputRol.NombreRol
                        );
                    }

                    resp.TieneError = false;
                    resp.Mensaje = _strLocalizer["RolSavedSuccessfully"];
                }
            }
            catch (Exception ex)
            {
                _auditoriaContext.Desactivar();
                await _db.Database.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        private async Task RegistrarAuditoriaPermisosRolAsync(string rolId, string nombreRol)
        {
            _db.IntranetAuditorias.Add(new IntranetAuditoria
            {
                UsuarioEjecutorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                UsuarioEjecutor = User.Identity?.Name,

                Modulo = "Roles",
                Accion = "Edición",
                Entidad = "Rol",
                RegistroId = rolId,
                RegistroNombre = nombreRol,

                CampoModificado = "Permisos",
                ValorAnterior = "Configuración anterior",
                ValorNuevo = "Permisos actualizados",

                FechaHora = DateTime.Now,

                Ip = HttpContext.Connection.RemoteIpAddress?.ToString() == "::1"
                    ? "127.0.0.1"
                    : HttpContext.Connection.RemoteIpAddress?.ToString(),

                UserAgent = Request.Headers["User-Agent"].ToString()
            });

            await _db.SaveChangesAsync();
        }

        private async Task CreateOrUpdateRole(RolModel e)
		{
			try
			{
				await _db.Database.BeginTransactionAsync();

				string idRol = "";
				//Busca el registro por Id
				AppRole? rol = await _roleManager.GetByIdAsync(e.Id);

				//Si se encontró rol, obtiene su Id del registro existente. De lo contrario, se crea uno nuevo.
				if (rol != null) { idRol = rol.Id; } else { rol = new AppRole(); }

				rol.Name = e.NombreRol;

				if (idRol.Length >= 1)
				{
					//El registro ya existe, por lo que solo se actualiza.
					await _roleManager.UpdateAsync(rol);

					//Elimina los accesos del rol.
					await _accesoModuloManager.DeleteByRolIdAsync(idRol);
				}
				else
				{
					//De lo contrario, crea a la empresa y obtiene su id.
					IdentityResult result = await _roleManager.CreateAsync(rol);
					if(result.Succeeded)
					{
						idRol = rol?.Id??"";
					}
				}

				//Crea los accesos del rol
				foreach (ModuloModel? acceso in e.Modulos)
				{
					if (acceso != null)
					{
						await _accesoModuloManager.CreateAsync(
							new AccesoModulo() { 
								RolId = idRol,
								ModuloId = acceso.ModuloId, 
								PuedeTodo = acceso.PuedeTodo ?? false ? 1 : 0, 
								PuedeConsultar = acceso.PuedeConsultar ?? false ? 1 : 0, 
								PuedeEditar = acceso.PuedeEditar ?? false ? 1 : 0, 
								PuedeEliminar = acceso.PuedeEliminar ?? false ? 1 : 0, 
								PuedeAutorizar = acceso.PuedeAutorizar ?? false ? 1 : 0 
							}
						);
					}
				}

				await _db.Database.CommitTransactionAsync();
			}
			catch (Exception)
			{
				await _db.Database.RollbackTransactionAsync();
				throw;
			}
		}
	}
}