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
    public class PuestosModel : PageModel
    {
		private readonly ApplicationDbContext _db;
		private readonly IEmpleadoManager _empleadoManager;
		private readonly IRWCatalogoManager<Puesto> _puestoManager;
		private readonly IStringLocalizer<PuestosModel> _strLocalizer;
		private readonly ILogger<PuestosModel> _logger;
        private readonly AuditoriaContext _auditoriaContext;

        [BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Display(Name = "Id")]
			public int Id { get; set; }

			[Display(Name = "NameField")]
			public string Nombre { get; set; } = string.Empty;
		}

		public PuestosModel(
			ApplicationDbContext db,
			IEmpleadoManager empleadoManager,	
			IRWCatalogoManager<Puesto> puestoManager,
			IStringLocalizer<PuestosModel> stringLocalizer,
			ILogger<PuestosModel> logger,
            AuditoriaContext auditoriaContext
        )
		{
			Input = new InputModel();
			_db = db;
			_empleadoManager = empleadoManager;
			_puestoManager = puestoManager;
			_strLocalizer = stringLocalizer;
			_logger = logger;
            _auditoriaContext = auditoriaContext;
        }

		public JsonResult OnGetPuestosList()
		{
			List<Puesto> puestos = _puestoManager.GetAllAsync().Result;

			return new JsonResult(puestos);
		}

        /*public async Task<JsonResult> OnPostDeletePuestos(string[] ids)
		{
			ServerResponse resp = new(true, _strLocalizer["PositionsDeletedUnsuccessfully"]);
			try
			{
                await _db.Database.BeginTransactionAsync();

				List<Puesto> puestos = await _puestoManager.GetAllAsync();
                foreach (string id in ids)
                {
					if (!int.TryParse(id, out int sid)) { sid = 0; }
					Puesto? puesto = puestos.Where(p => p.Id == sid).FirstOrDefault();
                    List<Empleado> empleados = await _empleadoManager.GetAllAsync(null, null, null, null, sid, null, null ,null, true);
					List<Empleado> empleadosActivosRelacionados = empleados.Where(e => e.Deshabilitado == 0).ToList();
                    //Si existen empleados que tengan el registro asignado, se le notifica al usuario.
                    if (empleadosActivosRelacionados.Count > 0)
					{
						List<string> names = [];
                        foreach (Empleado e in empleadosActivosRelacionados){ names.Add($"<i>{e.Id} - {e.NombreCompleto}</i>"); }
                        resp.TieneError = true;
                        resp.Mensaje = $"{_strLocalizer["PositionIsRelated"]}<br/><br/><i>{puesto?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
						break;
                    }
					else
					{
						//En caso de no haber empleados con el registro asignado, procede a eliminar referencias y registro.
                        foreach (Empleado e in empleados)
                        {
                            e.PuestoId = null;
                            await _empleadoManager.UpdateAsync(e);
                        }
                        await _puestoManager.DeleteByIdAsync(sid);

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["PositionsDeletedSuccessfully"];
                    }
                }

				if(resp.TieneError){ throw new Exception(resp.Mensaje); }

                await _db.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _db.Database.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
		}*/

        public async Task<JsonResult> OnPostDeletePuestos(string[] ids)
        {
            ServerResponse resp = new(true, _strLocalizer["PositionsDeletedUnsuccessfully"]);

            try
            {
                await _db.Database.BeginTransactionAsync();

                _auditoriaContext.Activar("Puestos", "Eliminación");

                List<Puesto> puestos = await _puestoManager.GetAllAsync();

                foreach (string id in ids)
                {
                    if (!int.TryParse(id, out int sid))
                    {
                        sid = 0;
                    }

                    Puesto? puesto = puestos.Where(p => p.Id == sid).FirstOrDefault();

                    List<Empleado> empleados = await _empleadoManager.GetAllAsync(
                        null, null, null, null, sid, null, null, null, true);

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
                        resp.Mensaje = $"{_strLocalizer["PositionIsRelated"]}<br/><br/><i>{puesto?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
                        break;
                    }
                    else
                    {
                        foreach (Empleado e in empleados)
                        {
                            e.PuestoId = null;
                            await _empleadoManager.UpdateAsync(e);
                        }

                        await _puestoManager.DeleteByIdAsync(sid);

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["PositionsDeletedSuccessfully"];
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

        /*public async Task<JsonResult> OnPostSavePuesto()
		{
			ServerResponse resp = new(true, _strLocalizer["PositionSavedUnsuccessfully"]);

			try
			{
				if (!ModelState.IsValid)
				{
					resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors ?? []).Select(m => m.ErrorMessage).ToArray();
				}
				else
				{
					Puesto? puesto = await _puestoManager.GetByNameAsync(Input.Nombre);

					if (puesto != null && puesto.Id != Input.Id)
					{
						resp.Mensaje = _strLocalizer["ErrorPuestoExistente"];
					}
					else
					{
						int id = 0;
						puesto = await _puestoManager.GetByIdAsync(Input.Id);

						if (puesto != null) { id = puesto.Id; } else { puesto = new Puesto(); }

						puesto.Nombre = Input.Nombre;

						if(id >= 1)
						{
							await _puestoManager.UpdateAsync(puesto);
						}
						else
						{
							await _puestoManager.CreateAsync(puesto);

						}

						resp.TieneError = false;
						resp.Mensaje = _strLocalizer["PositionSavedSuccessfully"];
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}*/
        public async Task<JsonResult> OnPostSavePuesto()
        {
            ServerResponse resp = new(true, _strLocalizer["PositionSavedUnsuccessfully"]);

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
                    Puesto? puesto = await _puestoManager.GetByNameAsync(Input.Nombre);

                    if (puesto != null && puesto.Id != Input.Id)
                    {
                        resp.Mensaje = _strLocalizer["ErrorPuestoExistente"];
                    }
                    else
                    {
                        int id = 0;
                        puesto = await _puestoManager.GetByIdAsync(Input.Id);

                        if (puesto != null)
                        {
                            id = puesto.Id;
                        }
                        else
                        {
                            puesto = new Puesto();
                        }

                        string accionAuditoria = id >= 1 ? "Edición" : "Alta";

                        _auditoriaContext.Activar("Puestos", accionAuditoria);

                        puesto.Nombre = Input.Nombre;

                        if (id >= 1)
                        {
                            await _puestoManager.UpdateAsync(puesto);
                        }
                        else
                        {
                            await _puestoManager.CreateAsync(puesto);
                        }

                        _auditoriaContext.Desactivar();

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["PositionSavedSuccessfully"];
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
