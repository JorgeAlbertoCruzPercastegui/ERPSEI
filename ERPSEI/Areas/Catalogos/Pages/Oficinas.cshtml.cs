using ERPSEI.Areas.Catalogos.Pages.Auditoria;
using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Metricas;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Requests;
using ERPSEI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Areas.Catalogos.Pages
{
	[Authorize(Policy = "AccessPolicy")]
	public class OficinasModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmpleadoManager _empleadoManager;
        private readonly IRWCatalogoManager<Oficina> _oficinaManager;
		private readonly IStringLocalizer<OficinasModel> _strLocalizer;
		private readonly ILogger<OficinasModel> _logger;
        private readonly AuditoriaContext _auditoriaContext;

        [BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Display(Name = "Id")]
			public int Id { get; set; }

			[StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 1)]
			[RegularExpression(RegularExpressions.AlphanumSpace, ErrorMessage = "AlphanumSpace")]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "NameField")]
			public string Nombre { get; set; } = string.Empty;
		}

		public OficinasModel(
            ApplicationDbContext db,
            IEmpleadoManager empleadoManager,
            IRWCatalogoManager<Oficina> oficinaManage,
			IStringLocalizer<OficinasModel> stringLocalizer,
			ILogger<OficinasModel> logger,
            AuditoriaContext auditoriaContext
        )
		{
			Input = new InputModel();
            _db = db;
            _empleadoManager = empleadoManager;
            _oficinaManager = oficinaManage;
			_strLocalizer = stringLocalizer;
			_logger = logger;
            _auditoriaContext = auditoriaContext;
        }

		public JsonResult OnGetOficinasList()
		{
			List<Oficina> oficinas = _oficinaManager.GetAllAsync().Result;

			return new JsonResult(oficinas);
		}

        /*public async Task<JsonResult> OnPostDeleteOficinas(string[] ids)
		{
			ServerResponse resp = new(true, _strLocalizer["OfficesDeletedUnsuccessfully"]);
			try
			{
                await _db.Database.BeginTransactionAsync();

                List<Oficina> oficinas = await _oficinaManager.GetAllAsync();
                foreach (string id in ids)
                {
					if (!int.TryParse(id, out int sid)) { sid = 0; }
					Oficina? oficina = oficinas.Where(p => p.Id == sid).FirstOrDefault();
                    List<Empleado> empleados = await _empleadoManager.GetAllAsync(null, null, null, null, null, null, null, sid, true);
                    List<Empleado> empleadosActivosRelacionados = empleados.Where(e => e.Deshabilitado == 0).ToList();
                    //Si existen empleados que tengan el registro asignado, se le notifica al usuario.
                    if (empleadosActivosRelacionados.Count > 0)
                    {
                        List<string> names = [];
                        foreach (Empleado e in empleadosActivosRelacionados) { names.Add($"<i>{e.Id} - {e.NombreCompleto}</i>"); }
                        resp.TieneError = true;
                        resp.Mensaje = $"{_strLocalizer["OfficeIsRelated"]}<br/><br/><i>{oficina?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
                        break;
                    }
                    else
                    {
                        //En caso de no haber empleados con el registro asignado, procede a eliminar referencias y registro.
                        foreach (Empleado e in empleados)
                        {
                            e.OficinaId = null;
                            await _empleadoManager.UpdateAsync(e);
                        }
                        await _oficinaManager.DeleteByIdAsync(sid);

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["OfficesDeletedSuccessfully"];
                    }
                }

                if (resp.TieneError) { throw new Exception(resp.Mensaje); }

                await _db.Database.CommitTransactionAsync();
            }
			catch (Exception ex)
			{
                await _db.Database.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}*/

        public async Task<JsonResult> OnPostDeleteOficinas(string[] ids)
        {
            ServerResponse resp = new(true, _strLocalizer["OfficesDeletedUnsuccessfully"]);

            try
            {
                await _db.Database.BeginTransactionAsync();

                _auditoriaContext.Activar("Oficinas", "Eliminación");

                List<Oficina> oficinas = await _oficinaManager.GetAllAsync();

                foreach (string id in ids)
                {
                    if (!int.TryParse(id, out int sid))
                    {
                        sid = 0;
                    }

                    Oficina? oficina = oficinas
                        .Where(p => p.Id == sid)
                        .FirstOrDefault();

                    List<Empleado> empleados = await _empleadoManager.GetAllAsync(
                        null, null, null, null, null, null, null, sid, true);

                    List<Empleado> empleadosActivosRelacionados = empleados
                        .Where(e => e.Deshabilitado == 0)
                        .ToList();

                    if (empleadosActivosRelacionados.Count > 0)
                    {
                        List<string> names = [];

                        foreach (Empleado e in empleadosActivosRelacionados)
                        {
                            names.Add($"<i>{e.Id} - {e.NombreCompleto}</i>");
                        }

                        resp.TieneError = true;
                        resp.Mensaje = $"{_strLocalizer["OfficeIsRelated"]}<br/><br/><i>{oficina?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
                        break;
                    }
                    else
                    {
                        foreach (Empleado e in empleados)
                        {
                            e.OficinaId = null;
                            await _empleadoManager.UpdateAsync(e);
                        }

                        await _oficinaManager.DeleteByIdAsync(sid);

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["OfficesDeletedSuccessfully"];
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

                await _db.Database.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }


        public async Task<JsonResult> OnPostSaveOficina()
        {
            ServerResponse resp = new(true, _strLocalizer["OfficeSavedUnsuccessfully"]);

            try
            {
                if (!ModelState.IsValid)
                {
                    resp.Errores = ModelState.Keys
                        .SelectMany(k => ModelState[k]?.Errors ?? [])
                        .Select(m => m.ErrorMessage)
                        .ToArray();
                }
                else
                {
                    Oficina? oficina = await _oficinaManager.GetByNameAsync(Input.Nombre);

                    if (oficina != null && oficina.Id != Input.Id)
                    {
                        resp.Mensaje = _strLocalizer["ErrorOficinaExistente"];
                    }
                    else
                    {
                        int id = 0;

                        oficina = await _oficinaManager.GetByIdAsync(Input.Id);

                        if (oficina != null)
                        {
                            id = oficina.Id;
                        }
                        else
                        {
                            oficina = new Oficina();
                        }

                        string accionAuditoria = id >= 1
                            ? "Edición"
                            : "Alta";

                        _auditoriaContext.Activar("Oficinas", accionAuditoria);

                        oficina.Nombre = Input.Nombre;

                        if (id >= 1)
                        {
                            await _oficinaManager.UpdateAsync(oficina);
                        }
                        else
                        {
                            await _oficinaManager.CreateAsync(oficina);
                        }

                        _auditoriaContext.Desactivar();

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["OfficeSavedSuccessfully"];
                    }
                }
            }
            catch (Exception ex)
            {
                _auditoriaContext.Desactivar();
                _logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        /*public async Task<JsonResult> OnPostSaveOficina()
		{
			ServerResponse resp = new(true, _strLocalizer["OfficeSavedUnsuccessfully"]);

			try
			{
				if (!ModelState.IsValid)
				{
					resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors ?? []).Select(m => m.ErrorMessage).ToArray();
				}
				else
				{
					Oficina? oficina = await _oficinaManager.GetByNameAsync(Input.Nombre);

					if (oficina != null && oficina.Id != Input.Id)
					{
						resp.Mensaje = _strLocalizer["ErrorOficinaExistente"];
					}
					else
					{
						int id = 0;
						//Se busca por id.
						oficina = await _oficinaManager.GetByIdAsync(Input.Id);

						if (oficina != null) { id = oficina.Id; } else { oficina = new Oficina(); }

						oficina.Nombre = Input.Nombre;

						if(id >= 1)
						{
							await _oficinaManager.UpdateAsync(oficina);
						}
						else
						{
							await _oficinaManager.CreateAsync(oficina);

						}

						resp.TieneError = false;
						resp.Mensaje = _strLocalizer["OfficeSavedSuccessfully"];
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}*/

    }
}
