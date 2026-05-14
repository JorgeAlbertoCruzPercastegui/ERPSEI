using ERPSEI.Areas.Catalogos.Pages.Auditoria;
using ERPSEI.Data;
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
	public class NivelesModel : PageModel
    {
		private readonly ApplicationDbContext _db;
		private readonly IEmpresaManager _empresaManager;
		private readonly IRWCatalogoManager<Nivel> _catalogoManager;
		private readonly IStringLocalizer<NivelesModel> _strLocalizer;
		private readonly ILogger<NivelesModel> _logger;
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

			[Display(Name = "OrdinalField")]
			[Required(ErrorMessage = "Required")]
			[RegularExpression(RegularExpressions.NumericFirstDigitNonZero, ErrorMessage = "Numeric")]
			public int Ordinal { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "PuedeFacturarField")]
			public bool PuedeFacturar { get; set; } = true;
		}

		public NivelesModel(
			ApplicationDbContext db,
			IEmpresaManager empresaManager,
			IRWCatalogoManager<Nivel> catalogoManager,
			IStringLocalizer<NivelesModel> stringLocalizer,
			ILogger<NivelesModel> logger,
            AuditoriaContext auditoriaContext
        )
		{
			Input = new InputModel();
			_db = db;
			_empresaManager = empresaManager;
			_catalogoManager = catalogoManager;
			_strLocalizer = stringLocalizer;
			_logger = logger;
            _auditoriaContext = auditoriaContext;
        }

		public async Task<JsonResult> OnGetNivelesList()
        {
			List<Nivel> niveles = await _catalogoManager.GetAllAsync();
			List<string> jsonNiveles = [];
			foreach (Nivel n in niveles)
            {
				jsonNiveles.Add(
					"{" +
						$"\"id\": {n.Id}," +
						$"\"nombre\": \"{n.Nombre}\", " +
						$"\"ordinal\": \"{n.Ordinal}\", " +
						$"\"puedeFacturar\": \"{n.PuedeFacturar}\"" +
					"}"
				);
            }

			string jsonResponse = $"[{string.Join(",", jsonNiveles)}]";

			return new JsonResult(jsonResponse);
		}

        /*public async Task<JsonResult> OnPostDeleteNiveles(string[] ids)
		{
			ServerResponse resp = new(true, _strLocalizer["DeletedUnsuccessfully"]);
			try
			{
				await _db.Database.BeginTransactionAsync();

				List<Nivel> niveles = await _catalogoManager.GetAllAsync();
				foreach (string id in ids)
				{
					if (!int.TryParse(id, out int sid)) { sid = 0; }
					Nivel? nivel = niveles.Where(o => o.Id == sid).FirstOrDefault();
					List<Empresa> empresas = await _empresaManager.GetAllAsync();
					empresas = empresas.Where(e => e.NivelId == sid).ToList();
					List<Empresa> empresasActivasRelacionadas = empresas.Where(e => e.Deshabilitado == 0).ToList();
					if (empresasActivasRelacionadas.Count > 0)
					{
						List<string> names = [];
						foreach (Empresa e in empresasActivasRelacionadas) { names.Add($"<i>{e.Id} - {e.RazonSocial}</i>"); }
						resp.TieneError = true;
						resp.Mensaje = $"{_strLocalizer["NivelIsRelated"]}<br/><br/><i>{nivel?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
						break;
					}
					else
					{
						foreach (Empresa empresa in empresas)
						{
							empresa.NivelId = null;
							await _empresaManager.UpdateAsync(empresa);
						}
						await _catalogoManager.DeleteByIdAsync(sid);

						resp.TieneError = false;
						resp.Mensaje = _strLocalizer["DeletedSuccessfully"];
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

        public async Task<JsonResult> OnPostDeleteNiveles(string[] ids)
        {
            ServerResponse resp = new(true, _strLocalizer["DeletedUnsuccessfully"]);

            try
            {
                await _db.Database.BeginTransactionAsync();

                _auditoriaContext.Activar("Niveles", "Eliminación");

                List<Nivel> niveles = await _catalogoManager.GetAllAsync();

                foreach (string id in ids)
                {
                    if (!int.TryParse(id, out int sid))
                    {
                        sid = 0;
                    }

                    Nivel? nivel = niveles
                        .Where(o => o.Id == sid)
                        .FirstOrDefault();

                    List<Empresa> empresas = await _empresaManager.GetAllAsync();

                    empresas = empresas
                        .Where(e => e.NivelId == sid)
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
                        resp.Mensaje = $"{_strLocalizer["NivelIsRelated"]}<br/><br/><i>{nivel?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
                        break;
                    }
                    else
                    {
                        foreach (Empresa empresa in empresas)
                        {
                            empresa.NivelId = null;
                            await _empresaManager.UpdateAsync(empresa);
                        }

                        await _catalogoManager.DeleteByIdAsync(sid);

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

        /*public async Task<JsonResult> OnPostSaveNivel()
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
					//Busca el nivel por nombre
					Nivel? nivel = await _catalogoManager.GetByNameAsync(Input.Nombre);

					if (nivel != null && nivel.Id != Input.Id)
					{
						resp.Mensaje = _strLocalizer["ErrorExistente"];
					}
					else
					{
						int id = 0;
						//Se busca por id.
						nivel = await _catalogoManager.GetByIdAsync(Input.Id);

						if (nivel != null) { id = nivel.Id; } else { nivel = new Nivel(); }

						nivel.Nombre = Input.Nombre;
						nivel.Ordinal = Input.Ordinal;
						nivel.PuedeFacturar = Input.PuedeFacturar;

						if (id >= 1)
						{
							//El registro ya existe, por lo que solo se actualiza.
							await _catalogoManager.UpdateAsync(nivel);
						}
						else
						{
							await _catalogoManager.CreateAsync(nivel);

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

        public async Task<JsonResult> OnPostSaveNivel()
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
                    Nivel? nivel = await _catalogoManager.GetByNameAsync(Input.Nombre);

                    if (nivel != null && nivel.Id != Input.Id)
                    {
                        resp.Mensaje = _strLocalizer["ErrorExistente"];
                    }
                    else
                    {
                        int id = 0;

                        nivel = await _catalogoManager.GetByIdAsync(Input.Id);

                        if (nivel != null)
                        {
                            id = nivel.Id;
                        }
                        else
                        {
                            nivel = new Nivel();
                        }

                        string accionAuditoria = id >= 1 ? "Edición" : "Alta";

                        _auditoriaContext.Activar("Niveles", accionAuditoria);

                        nivel.Nombre = Input.Nombre;
                        nivel.Ordinal = Input.Ordinal;
                        nivel.PuedeFacturar = Input.PuedeFacturar;

                        if (id >= 1)
                        {
                            await _catalogoManager.UpdateAsync(nivel);
                        }
                        else
                        {
                            await _catalogoManager.CreateAsync(nivel);
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
    }
}
