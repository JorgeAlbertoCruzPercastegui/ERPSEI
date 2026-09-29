using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Metricas;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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

            [Display(Name = "RolField")]
            public List<string> RolIds { get; set; } = new();

            [Display(Name = "UserNameField")]
			public string NombreUsuario { get; set; } = string.Empty;

			[Display(Name = "EmployeeNameField")]
			public string NombreEmpleado { get; set; } = string.Empty;
		}

        private readonly AuditoriaContext _auditoriaContext;

        public UsuariosModel(
			AppUserManager usuarioManager,
			IEmpleadoManager empleadoManager,
			AppRoleManager roleManager,
			IStringLocalizer<UsuariosModel> stringLocalizer,
			ILogger<UsuariosModel> logger,
			ApplicationDbContext db,
            AuditoriaContext auditoriaContext
        )
		{
			_usuarioManager = usuarioManager;
			_empleadoManager = empleadoManager;
			_roleManager = roleManager;
			_strLocalizer = stringLocalizer;
			_logger = logger;
			_db = db;
            _auditoriaContext = auditoriaContext;

            InputUsuario = new UsuarioModel();
		}

        private bool EsCambioDeRolUsuario(EntityEntry entry)
        {
            string nombreEntidad = entry.Entity.GetType().Name.ToLower();

            return nombreEntidad.Contains("identityuserrole") ||
                   nombreEntidad.Contains("appuserrole") ||
                   entry.Metadata.ClrType == typeof(IdentityUserRole<string>);
        }

        private async Task<string> GetLista()
        {
            var resultados = new List<object>();

            foreach (AppUser u in _usuarioManager.Users.Where(u => !u.IsBanned))
            {
                if (await _usuarioManager.IsInRoleAsync(
                    u,
                    ServicesConfiguration.RolMaster))
                {
                    continue;
                }

                IList<string> rolesUsuario =
                    await _usuarioManager.GetRolesAsync(u);

                List<string> idRoles = new();
                List<string> nombreRoles = new();

                foreach (string nombreRol in rolesUsuario)
                {
                    AppRole? foundRole =
                        await _roleManager.GetByNameAsync(nombreRol);

                    if (foundRole == null)
                    {
                        continue;
                    }

                    // No exponemos estos roles en este módulo
                    if (foundRole.Name == ServicesConfiguration.RolMaster ||
                        foundRole.Name == ServicesConfiguration.RolCandidato)
                    {
                        continue;
                    }

                    idRoles.Add(foundRole.Id);

                    nombreRoles.Add(
                        foundRole.Name ??
                        string.Empty
                    );
                }

                if (nombreRoles.Count == 0)
                {
                    nombreRoles.Add(
                        _strLocalizer["EmptyRoleName"]
                    );
                }

                Empleado? emp =
                    await _empleadoManager.GetByIdAsync(
                        u.EmpleadoId ?? 0
                    );

                string nombreEmpleado =
                    emp != null
                        ? emp.NombreCompleto
                        : _strLocalizer["EmptyEmployeeName"];

                resultados.Add(new
                {
                    id = u.Id,

                    rolIds = idRoles,

                    rol = string.Join(
                        ", ",
                        nombreRoles
                    ),

                    nombreUsuario =
                        u.UserName ?? string.Empty,

                    nombreEmpleado =
                        nombreEmpleado
                });
            }

            return JsonConvert.SerializeObject(
                resultados
            );
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

        /*public async Task<JsonResult> OnPostSave()
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
		}*/

        public async Task<JsonResult> OnPostSave()
        {
            ServerResponse resp = new(true, _strLocalizer["SavedUnsuccessfully"]);

            if (!ModelState.IsValid)
            {
                resp.Errores = ModelState.Keys
                    .SelectMany(k => ModelState[k]?.Errors ?? [])
                    .Select(m => m.ErrorMessage)
                    .ToArray();

                return new JsonResult(resp);
            }

            if (InputUsuario.RolIds == null || InputUsuario.RolIds.Count == 0)
            {
                resp.Mensaje =
                    "Debe seleccionar al menos un rol.";

                return new JsonResult(resp);
            }

            try
            {
                await _db.Database.BeginTransactionAsync();

                _auditoriaContext.Activar("Usuarios", "Edición");

                await UpdateUser(InputUsuario);

                _auditoriaContext.Desactivar();

                await _db.Database.CommitTransactionAsync();

                resp.TieneError = false;
                resp.Mensaje = _strLocalizer["SavedSuccessfully"];
            }
            catch (Exception ex)
            {
                _auditoriaContext.Desactivar();

                await _db.Database.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        /*private async Task UpdateUser(UsuarioModel e)
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
		}*/

        private async Task UpdateUser(UsuarioModel e)
        {
            AppUser? usuario =
                await _usuarioManager
                    .FindByIdAsync(e.Id);

            if (usuario == null)
            {
                throw new Exception(
                    "El usuario no fue encontrado."
                );
            }


            // =====================================================
            // VALIDAR ROLES SELECCIONADOS
            // =====================================================

            if (e.RolIds == null ||
                e.RolIds.Count == 0)
            {
                throw new Exception(
                    "Debe seleccionar al menos un rol."
                );
            }


            // Evitar IDs repetidos
            List<string> idsSeleccionados =
                e.RolIds
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();


            List<string> rolesNuevos =
                new();


            foreach (string rolId in idsSeleccionados)
            {
                AppRole? rol =
                    await _roleManager
                        .FindByIdAsync(rolId);

                if (rol == null ||
                    string.IsNullOrWhiteSpace(
                        rol.Name))
                {
                    continue;
                }


                // Estos roles no deben poder asignarse
                // desde este módulo.
                if (rol.Name ==
                        ServicesConfiguration.RolMaster ||
                    rol.Name ==
                        ServicesConfiguration.RolCandidato)
                {
                    continue;
                }


                rolesNuevos.Add(
                    rol.Name
                );
            }


            if (rolesNuevos.Count == 0)
            {
                throw new Exception(
                    "No se recibió ningún rol válido."
                );
            }


            // =====================================================
            // ROLES ACTUALES
            // =====================================================

            IList<string> rolesActuales =
                await _usuarioManager
                    .GetRolesAsync(usuario);


            // No modificamos roles protegidos
            List<string> rolesActualesEditables =
                rolesActuales
                    .Where(x =>
                        !x.Equals(
                            ServicesConfiguration.RolMaster,
                            StringComparison.OrdinalIgnoreCase
                        ) &&
                        !x.Equals(
                            ServicesConfiguration.RolCandidato,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();


            // =====================================================
            // CALCULAR DIFERENCIAS
            // =====================================================

            List<string> rolesAgregar =
                rolesNuevos
                    .Except(
                        rolesActualesEditables,
                        StringComparer.OrdinalIgnoreCase
                    )
                    .ToList();


            List<string> rolesEliminar =
                rolesActualesEditables
                    .Except(
                        rolesNuevos,
                        StringComparer.OrdinalIgnoreCase
                    )
                    .ToList();


            // =====================================================
            // ELIMINAR ÚNICAMENTE LOS QUE YA NO ESTÁN MARCADOS
            // =====================================================

            if (rolesEliminar.Count > 0)
            {
                IdentityResult resultadoEliminar =
                    await _usuarioManager
                        .RemoveFromRolesAsync(
                            usuario,
                            rolesEliminar
                        );

                if (!resultadoEliminar.Succeeded)
                {
                    throw new Exception(
                        string.Join(
                            ". ",
                            resultadoEliminar.Errors
                                .Select(x =>
                                    x.Description)
                        )
                    );
                }
            }


            // =====================================================
            // AGREGAR ÚNICAMENTE LOS NUEVOS
            // =====================================================

            if (rolesAgregar.Count > 0)
            {
                IdentityResult resultadoAgregar =
                    await _usuarioManager
                        .AddToRolesAsync(
                            usuario,
                            rolesAgregar
                        );

                if (!resultadoAgregar.Succeeded)
                {
                    throw new Exception(
                        string.Join(
                            ". ",
                            resultadoAgregar.Errors
                                .Select(x =>
                                    x.Description)
                        )
                    );
                }
            }


            // =====================================================
            // AUDITORÍA
            // =====================================================

            string rolAnterior =
                rolesActualesEditables.Any()
                    ? string.Join(
                        ", ",
                        rolesActualesEditables
                            .OrderBy(x => x)
                    )
                    : "Sin rol";


            string rolNuevo =
                rolesNuevos.Any()
                    ? string.Join(
                        ", ",
                        rolesNuevos
                            .OrderBy(x => x)
                    )
                    : "Sin rol";


            bool rolesCambiaron =
                rolesAgregar.Count > 0 ||
                rolesEliminar.Count > 0;


            if (rolesCambiaron)
            {
                _db.IntranetAuditorias.Add(
                    new IntranetAuditoria
                    {
                        UsuarioEjecutorId =
                            User.FindFirst(
                                System.Security.Claims
                                    .ClaimTypes
                                    .NameIdentifier
                            )?.Value,

                        UsuarioEjecutor =
                            User.Identity?.Name,

                        Modulo =
                            "Usuarios",

                        Accion =
                            "Edición",

                        Entidad =
                            "Usuario",

                        RegistroId =
                            usuario.Id,

                        RegistroNombre =
                            usuario.UserName ??
                            "Sin usuario",

                        CampoModificado =
                            "Roles",

                        ValorAnterior =
                            rolAnterior,

                        ValorNuevo =
                            rolNuevo,

                        FechaHora =
                            DateTime.Now,

                        Ip =
                            HttpContext
                                .Connection
                                .RemoteIpAddress?
                                .ToString() == "::1"
                                    ? "127.0.0.1"
                                    : HttpContext
                                        .Connection
                                        .RemoteIpAddress?
                                        .ToString(),

                        UserAgent =
                            Request.Headers[
                                "User-Agent"
                            ].ToString()
                    }
                );

                await _db.SaveChangesAsync();
            }
        }
    }
}