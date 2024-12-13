using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Empleados;
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

namespace ERPSEI.Areas.Catalogos.Pages
{
    [Authorize(Policy = "AccessPolicy")]
    public class UsuariosModel : PageModel
	{
		private readonly AppUserManager _usuarioManager;
		private readonly IEmpleadoManager _empleadoManager;
		private readonly AppRoleManager _roleManager;
		private readonly IStringLocalizer<UsuariosModel> _strLocalizer;
		private readonly ILogger<UsuariosModel> _logger;
		private readonly ApplicationDbContext _db;

		[BindProperty]
		public UsuarioModel InputUsuario { get; set; }

		public class UsuarioModel
		{
			public string Id { get; set; } = string.Empty;

			[Required(ErrorMessage = "Required")]
			[Display(Name = "RolField")]
			public string RolId { get; set; } = string.Empty;

			[Display(Name = "UserNameField")]
			public string NombreUsuario { get; set; } = string.Empty;

			[Display(Name = "EmployeeNameField")]
			public string NombreEmpleado { get; set; } = string.Empty;
		}

		public UsuariosModel(
			AppUserManager usuarioManager,
			IEmpleadoManager empleadoManager,
			AppRoleManager roleManager,
			IStringLocalizer<UsuariosModel> stringLocalizer,
			ILogger<UsuariosModel> logger,
			ApplicationDbContext db
		)
		{
			_usuarioManager = usuarioManager;
			_empleadoManager = empleadoManager;
			_roleManager = roleManager;
			_strLocalizer = stringLocalizer;
			_logger = logger;
			_db = db;

			InputUsuario = new UsuarioModel();
		}

		private async Task<string> GetLista()
		{
			string jsonResponse;
			List<string> jsonResultados = [];

			foreach (AppUser u in _usuarioManager.Users.Where(u => !u.IsBanned))
			{
				if(await _usuarioManager.IsInRoleAsync(u, ServicesConfiguration.RolMaster)) { continue; }

                IList<string> rolesUsuario = await _usuarioManager.GetRolesAsync(u);

                List<string> idRoles = [];
                List<string> nombreRoles = [];
                foreach (string r in rolesUsuario)
                {
                    AppRole? foundRole = await _roleManager.GetByNameAsync(r);
					idRoles.Add(foundRole?.Id ?? "0");
                    nombreRoles.Add(foundRole?.Name ?? string.Empty);
                }

				if (nombreRoles.Count <= 0) { nombreRoles.Add(_strLocalizer["EmptyRoleName"]); }

				Empleado? emp = await _empleadoManager.GetByIdAsync(u.EmpleadoId??0);
				string nombreEmpleado = emp != null ? emp.NombreCompleto : _strLocalizer["EmptyEmployeeName"];
				jsonResultados.Add(
					"{" +
						$"\"id\": \"{u.Id}\"," +
						$"\"rolId\": \"{string.Join(", ", idRoles)}\"," +
						$"\"rol\": \"{string.Join(", ", nombreRoles)}\"," +
						$"\"nombreUsuario\": \"{u.UserName}\"," +
						$"\"nombreEmpleado\": \"{nombreEmpleado}\"" +
					"}"
				);
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
		
		public async Task<JsonResult> OnPostSave()
		{
			ServerResponse resp = new(true, _strLocalizer["SavedUnsuccessfully"]);

			if (!ModelState.IsValid)
			{
				resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors ?? []).Select(m => m.ErrorMessage).ToArray();
				return new JsonResult(resp);
			}
			try
			{
				await _db.Database.BeginTransactionAsync();

				//Procede a actualizar el usuario.
				await UpdateUser(InputUsuario);

				await _db.Database.CommitTransactionAsync();

				resp.TieneError = false;
				resp.Mensaje = _strLocalizer["SavedSuccessfully"];
			}
			catch (Exception ex)
			{
				await _db.Database.RollbackTransactionAsync();
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}
		private async Task UpdateUser(UsuarioModel e)
		{
			//Se busca usuario por id
			AppUser? usuario = await _usuarioManager.FindByIdAsync(e.Id);
            AppRole? nuevoRol = await _roleManager.FindByIdAsync(e.RolId);

			//Si se encontró usuario, obtiene su Id del registro existente.
			if (usuario != null && nuevoRol != null) {
				//Obtiene los roles actuales del usuario.
                IList<string> rolesUsuario = await _usuarioManager.GetRolesAsync(usuario);

				//Se quitan todos los roles que tenía el usuario.
                foreach (string nombreRol in rolesUsuario){ await _usuarioManager.RemoveFromRoleAsync(usuario, nombreRol); }

                //Se establece el nuevo rol del usuario. Si no se encuentra el rol, entonces se usa el rol de usuario por default.
                await _usuarioManager.AddToRoleAsync(usuario, nuevoRol.Name ?? ServicesConfiguration.RolUsuario);
            }
		}
	}
}