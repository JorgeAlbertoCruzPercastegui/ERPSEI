using ERPSEI.Areas.Catalogos.Pages.Auditoria;
using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.Metricas;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empresas;
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
    public class OrigenesModel : PageModel
    {
		private readonly ApplicationDbContext _db;
		private readonly IEmpresaManager _empresaManager;
		private readonly IRWCatalogoManager<Origen> _origenManager;
		private readonly IStringLocalizer<OrigenesModel> _strLocalizer;
		private readonly ILogger<OrigenesModel> _logger;
        private readonly AuditoriaContext _auditoriaContext;

        [BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Display(Name = "Id")]
			public int Id { get; set; }

			[StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 1)]
			[RegularExpression(RegularExpressions.AlphanumSpaceParenthesis, ErrorMessage = "AlphanumSpaceParenthesis")]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "NameField")]
			public string Nombre { get; set; } = string.Empty;
		}

		public OrigenesModel(
			ApplicationDbContext db,
			IEmpresaManager empresaManager,
			IRWCatalogoManager<Origen> origenManager,
			IStringLocalizer<OrigenesModel> stringLocalizer,
			ILogger<OrigenesModel> logger,
            AuditoriaContext auditoriaContext
        )
		{
			Input = new InputModel();
			_db = db;
			_empresaManager = empresaManager;
			_origenManager = origenManager;
			_strLocalizer = stringLocalizer;
			_logger = logger;
            _auditoriaContext = auditoriaContext;
        }

		public JsonResult OnGetOrigenesList()
        {
			List<Origen> origenes = _origenManager.GetAllAsync().Result;

			return new JsonResult(origenes);
		}

        /*public async Task<JsonResult> OnPostDeleteOrigenes(string[] ids)
		{
			ServerResponse resp = new(true, _strLocalizer["DeletedUnsuccessfully"]);
			try
			{
				await _db.Database.BeginTransactionAsync();

				List<Origen> origenes = await _origenManager.GetAllAsync();
                foreach (string id in ids)
                {
					if (!int.TryParse(id, out int sid)) { sid = 0; }
					Origen? origen = origenes.Where(o => o.Id == sid).FirstOrDefault();
					List<Empresa> empresas = await _empresaManager.GetAllAsync();
					empresas = empresas.Where(e => e.OrigenId == sid).ToList();
					List<Empresa> empresasActivasRelacionadas = empresas.Where(e => e.Deshabilitado == 0).ToList();
					if(empresasActivasRelacionadas.Count > 0)
					{
						List<string> names = [];
						foreach (Empresa e in empresasActivasRelacionadas) { names.Add($"<i>{e.Id} - {e.RazonSocial}</i>"); }
						resp.TieneError = true;
						resp.Mensaje = $"{_strLocalizer["OrigenIsRelated"]}<br/><br/><i>{origen?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
						break;
					}
					else
					{
                        foreach (Empresa empresa in empresas)
                        {
							empresa.OrigenId = null;
							await _empresaManager.UpdateAsync(empresa);
                        }
                        await _origenManager.DeleteByIdAsync(sid);

						resp.TieneError = false;
						resp.Mensaje = _strLocalizer["DeletedSuccessfully"];
					}
                }

				if(resp.TieneError) { throw new Exception(resp.Mensaje); }

				await _db.Database.CommitTransactionAsync();
			}
			catch (Exception ex)
			{
				await _db.Database.RollbackTransactionAsync();
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}*/

        public async Task<JsonResult> OnPostDeleteOrigenes(string[] ids)
        {
            ServerResponse resp = new(true, _strLocalizer["DeletedUnsuccessfully"]);

            try
            {
                await _db.Database.BeginTransactionAsync();

                _auditoriaContext.Activar("Orígenes", "Eliminación");

                List<Origen> origenes = await _origenManager.GetAllAsync();

                foreach (string id in ids)
                {
                    if (!int.TryParse(id, out int sid))
                    {
                        sid = 0;
                    }

                    Origen? origen = origenes
                        .Where(o => o.Id == sid)
                        .FirstOrDefault();

                    List<Empresa> empresas = await _empresaManager.GetAllAsync();

                    empresas = empresas
                        .Where(e => e.OrigenId == sid)
                        .ToList();

                    List<Empresa> empresasActivasRelacionadas = empresas
                        .Where(e => e.Deshabilitado == 0)
                        .ToList();

                    if (empresasActivasRelacionadas.Count > 0)
                    {
                        List<string> names = [];

                        foreach (Empresa e in empresasActivasRelacionadas)
                        {
                            names.Add($"<i>{e.Id} - {e.RazonSocial}</i>");
                        }

                        resp.TieneError = true;

                        resp.Mensaje = $"{_strLocalizer["OrigenIsRelated"]}<br/><br/><i>{origen?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";

                        break;
                    }
                    else
                    {
                        foreach (Empresa empresa in empresas)
                        {
                            empresa.OrigenId = null;
                            await _empresaManager.UpdateAsync(empresa);
                        }

                        await _origenManager.DeleteByIdAsync(sid);

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["DeletedSuccessfully"];
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

        public async Task<JsonResult> OnPostSaveOrigen()
        {
            ServerResponse resp = new(true, _strLocalizer["SavedUnsuccessfully"]);

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
                    Origen? origen = await _origenManager.GetByNameAsync(Input.Nombre);

                    if (origen != null && origen.Id != Input.Id)
                    {
                        resp.Mensaje = _strLocalizer["ErrorExistente"];
                    }
                    else
                    {
                        int id = 0;

                        origen = await _origenManager.GetByIdAsync(Input.Id);

                        if (origen != null)
                        {
                            id = origen.Id;
                        }
                        else
                        {
                            origen = new Origen();
                        }

                        string accionAuditoria = id >= 1
                            ? "Edición"
                            : "Alta";

                        _auditoriaContext.Activar("Orígenes", accionAuditoria);

                        origen.Nombre = Input.Nombre;

                        if (id >= 1)
                        {
                            await _origenManager.UpdateAsync(origen);
                        }
                        else
                        {
                            await _origenManager.CreateAsync(origen);
                        }

                        _auditoriaContext.Desactivar();

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["SavedSuccessfully"];
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

        /*public async Task<JsonResult> OnPostSaveOrigen()
		{
			ServerResponse resp = new(true, _strLocalizer["SavedUnsuccessfully"]);

			try
			{
				if (!ModelState.IsValid)
				{
					resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors ?? []).Select(m => m.ErrorMessage).ToArray();
				}
				else
				{
					//Busca el origen por nombre
					Origen? origen = await _origenManager.GetByNameAsync(Input.Nombre);

					if (origen != null && origen.Id != Input.Id)
					{
						resp.Mensaje = _strLocalizer["ErrorExistente"];
					}
					else
					{
						int id = 0;
						//Se busca por id.
						origen = await _origenManager.GetByIdAsync(Input.Id);

						if (origen != null) { id = origen.Id; } else { origen = new Origen(); }

						origen.Nombre = Input.Nombre;

						if (id >= 1)
						{
							await _origenManager.UpdateAsync(origen);
						}
						else
						{
							await _origenManager.CreateAsync(origen);

						}

						resp.TieneError = false;
						resp.Mensaje = _strLocalizer["SavedSuccessfully"];
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
