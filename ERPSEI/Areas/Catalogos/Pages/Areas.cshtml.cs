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
    public class AreasModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmpleadoManager _empleadoManager;
        private readonly IRWCatalogoManager<Area> _areaManager;
		private readonly IStringLocalizer<AreasModel> _strLocalizer;
		private readonly ILogger<AreasModel> _logger;
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

		public AreasModel(
            ApplicationDbContext db,
            IEmpleadoManager empleadoManager,
            IRWCatalogoManager<Area> areaManager,
			IStringLocalizer<AreasModel> stringLocalizer,
            ILogger<AreasModel> logger,
			AuditoriaContext auditoriaContext
        )
		{
			Input = new InputModel();
            _db = db;
            _empleadoManager = empleadoManager;
            _areaManager = areaManager;
			_strLocalizer = stringLocalizer;
			_logger = logger;
            _auditoriaContext = auditoriaContext;
        }

		public JsonResult OnGetAreasList()
        {
			List<Area> areas = _areaManager.GetAllAsync().Result;

			return new JsonResult(areas);
		}

        /*public async Task<JsonResult> OnPostDeleteAreas(string[] ids)
		{
			ServerResponse resp = new(true, _strLocalizer["AreasDeletedUnsuccessfully"]);
			try
			{
                await _db.Database.BeginTransactionAsync();

                List<Area> areas = await _areaManager.GetAllAsync();
				foreach (string id in ids)
				{
					if (!int.TryParse(id, out int sid)) { sid = 0; }
					Area? area = areas.Where(p => p.Id == sid).FirstOrDefault();
                    List<Empleado> empleados = await _empleadoManager.GetAllAsync(null, null, null, null, null, sid, null, null, true);
                    List<Empleado> empleadosActivosRelacionados = empleados.Where(e => e.Deshabilitado == 0).ToList();
                    //Si existen empleados que tengan el registro asignado, se le notifica al usuario.
                    if (empleadosActivosRelacionados.Count > 0)
                    {
                        List<string> names = [];
                        foreach (Empleado e in empleadosActivosRelacionados) { names.Add($"<i>{e.Id} - {e.NombreCompleto}</i>"); }
                        resp.TieneError = true;
                        resp.Mensaje = $"{_strLocalizer["AreaIsRelated"]}<br/><br/><i>{area?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
                        break;
                    }
                    else
                    {
                        //En caso de no haber empleados con el registro asignado, procede a eliminar referencias y registro.
                        foreach (Empleado e in empleados)
                        {
                            e.AreaId = null;
                            await _empleadoManager.UpdateAsync(e);
                        }
                        await _areaManager.DeleteByIdAsync(sid);

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["AreasDeletedSuccessfully"];
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

        public async Task<JsonResult> OnPostDeleteAreas(string[] ids)
        {
            ServerResponse resp = new(true, _strLocalizer["AreasDeletedUnsuccessfully"]);

            try
            {
                await _db.Database.BeginTransactionAsync();

                _auditoriaContext.Activar("Áreas", "Eliminación");

                List<Area> areas = await _areaManager.GetAllAsync();

                foreach (string id in ids)
                {
                    if (!int.TryParse(id, out int sid))
                    {
                        sid = 0;
                    }

                    Area? area = areas.Where(p => p.Id == sid).FirstOrDefault();

                    List<Empleado> empleados = await _empleadoManager.GetAllAsync(
                        null, null, null, null, null, sid, null, null, true);

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
                        resp.Mensaje = $"{_strLocalizer["AreaIsRelated"]}<br/><br/><i>{area?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
                        break;
                    }
                    else
                    {
                        foreach (Empleado e in empleados)
                        {
                            e.AreaId = null;
                            await _empleadoManager.UpdateAsync(e);
                        }

                        await _areaManager.DeleteByIdAsync(sid);

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["AreasDeletedSuccessfully"];
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

        /*public async Task<JsonResult> OnPostSaveArea()
		{
			ServerResponse resp = new(true, _strLocalizer["AreaSavedUnsuccessfully"]);

			try
			{
				if (!ModelState.IsValid)
				{
					resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors ?? []).Select(m => m.ErrorMessage).ToArray();
				}
				else
				{
					//Se busca si ya existe un registro con el mismo nombre.
					Area? area = await _areaManager.GetByNameAsync(Input.Nombre);

					//Si ya existe un registro con el mismo nombre y los Id's no coinciden
					if (area != null && area.Id != Input.Id)
					{
						//Ya existe un elemento con el mismo nombre.
						resp.Mensaje = _strLocalizer["ErrorAreaExistente"];
					}
					else
					{
						int id = 0;
						//Busca el registro por Id
						area = await _areaManager.GetByIdAsync(Input.Id);

						//Si se encontró área, obtiene su Id del registro existente. De lo contrario, se crea uno nuevo.
						if (area != null) { id = area.Id; } else { area = new Area(); }

						area.Nombre = Input.Nombre;

						//Crea o actualiza el registro
						if(id >= 1)
						{
							await _areaManager.UpdateAsync(area);
						}
						else
						{
							await _areaManager.CreateAsync(area);
						}

						resp.TieneError = false;
						resp.Mensaje = _strLocalizer["AreaSavedSuccessfully"];
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}*/

        public async Task<JsonResult> OnPostSaveArea()
        {
            ServerResponse resp = new(true, _strLocalizer["AreaSavedUnsuccessfully"]);

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
                    Area? area = await _areaManager.GetByNameAsync(Input.Nombre);

                    if (area != null && area.Id != Input.Id)
                    {
                        resp.Mensaje = _strLocalizer["ErrorAreaExistente"];
                    }
                    else
                    {
                        int id = 0;

                        area = await _areaManager.GetByIdAsync(Input.Id);

                        if (area != null)
                        {
                            id = area.Id;
                        }
                        else
                        {
                            area = new Area();
                        }

                        string accionAuditoria = id >= 1
                            ? "Edición"
                            : "Alta";

                        _auditoriaContext.Activar("Áreas", accionAuditoria);

                        area.Nombre = Input.Nombre;

                        if (id >= 1)
                        {
                            await _areaManager.UpdateAsync(area);
                        }
                        else
                        {
                            await _areaManager.CreateAsync(area);
                        }

                        _auditoriaContext.Desactivar();

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["AreaSavedSuccessfully"];
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
    }
}
