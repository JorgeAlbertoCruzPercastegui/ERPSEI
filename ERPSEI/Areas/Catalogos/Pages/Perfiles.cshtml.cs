using ERPSEI.Data;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.SAT.Catalogos;
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
	public class PerfilesModel : PageModel
    {
		private readonly IProductoServicioManager _productosServiciosManager;
		private readonly IProductoServicioPerfilManager _productosServiciosPerfilManager;
		private readonly IPerfilManager _catalogoManager;
		private readonly IStringLocalizer<PerfilesModel> _strLocalizer;
		private readonly ILogger<PerfilesModel> _logger;
		private readonly ApplicationDbContext _db;
		private readonly IEmpresaManager _empresaManager;

		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Display(Name = "Id")]
			public int Id { get; set; }

			[StringLength(100, ErrorMessage = "FieldLength", MinimumLength = 1)]
			[RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "AlphanumSpace")]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "NameField")]
			public string Nombre { get; set; } = string.Empty;

			[Display(Name = "SearchProductServiceField")]
			public int? ProductoServicioId { get; set; }
			public int?[] ProductosServicios { get; set; } = [];
		}

		public PerfilesModel(
			IEmpresaManager empresaManager,
			IProductoServicioManager productosServiciosManager,
			IProductoServicioPerfilManager productosServiciosPerfilManager,
			IPerfilManager catalogoManager,
			IStringLocalizer<PerfilesModel> stringLocalizer,
			ILogger<PerfilesModel> logger,
			ApplicationDbContext db
		)
		{
			Input = new InputModel();
			_empresaManager = empresaManager;
			_productosServiciosManager = productosServiciosManager;
			_productosServiciosPerfilManager = productosServiciosPerfilManager;
			_catalogoManager = catalogoManager;
			_strLocalizer = stringLocalizer;
			_logger = logger;
			_db = db;
		}

		public JsonResult OnGetListAll()
        {

			string jsonResponse;
			List<string> jsonPerfiles = [];
			List<Perfil> perfiles = _catalogoManager.GetAllAsync().Result;

			foreach ( Perfil p in perfiles)
			{
				List<string> jsonProdServ = GetListJsonProductosServicios(p.ProductosServiciosPerfil);
				jsonPerfiles.Add(
									"{" +
										$"\"id\": {p.Id}," +
										$"\"nombre\": \"{p.Nombre}\", " +
										$"\"productosServicios\": [{string.Join(",", jsonProdServ)}]" +
									"}"
								);
			}

			jsonResponse = $"[{string.Join(",", jsonPerfiles)}]";

			return new JsonResult(jsonResponse);
		}
		private static List<string> GetListJsonProductosServicios(ICollection<ProductoServicioPerfil>? productosServicios)
		{
			List<string> jsonProdServ = [];
			if (productosServicios != null)
			{
				foreach (ProductoServicioPerfil a in productosServicios)
				{
					if (a.ProductoServicio == null) { continue; }

					jsonProdServ.Add(
						"{" +
							$"\"id\": \"{a.ProductoServicio.Id}\"," +
							$"\"clave\": \"{a.ProductoServicio.Clave}\"," +
							$"\"descripcion\": \"{a.ProductoServicio.Descripcion}\"" +
						"}"
					);
				}
			}

			return jsonProdServ;
		}

		public async Task<JsonResult> OnPostDelete(string[] ids)
		{
			ServerResponse resp = new(true, _strLocalizer["DeletedUnsuccessfully"]);
			try
			{
				await _db.Database.BeginTransactionAsync();

				foreach (string id in ids)
				{
					if (!int.TryParse(id, out int sid)) { sid = 0; }
					Perfil? perfil = await _catalogoManager.GetByIdAsync(sid);
					List<Empresa> empresas = await _empresaManager.GetAllAsync();
					empresas = empresas.Where(e => e.PerfilId == sid).ToList();
					List<Empresa> empresasActivasRelacionadas = empresas.Where(e => e.Deshabilitado == 0).ToList();
					if (empresasActivasRelacionadas.Count > 0)
					{
						List<string> names = [];
						foreach (Empresa e in empresasActivasRelacionadas) { names.Add($"<i>{e.Id} - {e.RazonSocial}</i>"); }
						resp.TieneError = true;
						resp.Mensaje = $"{_strLocalizer["PerfilIsRelated"]}<br/><br/><i>{perfil?.Nombre}</i><br/><br/>{string.Join("<br/>", names)}";
						break;
					}
					else
					{
						foreach (Empresa empresa in empresas)
						{
							empresa.PerfilId = null;
							await _empresaManager.UpdateAsync(empresa);
						}

						
						if(perfil != null)
						{
							foreach (ProductoServicioPerfil psp in perfil.ProductosServiciosPerfil)
							{
								_db.Remove(psp);
							}
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
		}

		public async Task<JsonResult> OnPostSave()
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
					//Se busca si ya existe un registro con el mismo nombre.
					Perfil? perfil = await _catalogoManager.GetByNameAsync(Input.Nombre);

					//Si ya existe un registro con el mismo nombre y los Id's no coinciden
					if (perfil != null && perfil.Id != Input.Id)
					{
						//Ya existe un elemento con el mismo nombre.
						resp.Mensaje = _strLocalizer["ErrorExistente"];
					}
					else
					{
						//Crea o actualiza el registro
						await CreateOrUpdateProfile(Input);

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
		}
		private async Task CreateOrUpdateProfile(InputModel p)
		{
			try
			{
				await _db.Database.BeginTransactionAsync();

				int idPerfil = 0;
				//Busca el registro por Id
				Perfil? perfil = await _catalogoManager.GetByIdAsync(p.Id);

				//Si se encontró perfil, obtiene su Id del registro existente. De lo contrario, se crea uno nuevo.
				if (perfil != null) { idPerfil = perfil.Id; } else { perfil = new Perfil(); }

				perfil.Nombre = p.Nombre;

				if (idPerfil >= 1)
				{
					//El registro ya existe, por lo que solo se actualiza.
					await _catalogoManager.UpdateAsync(perfil);

					//Elimina los productos y servicios del perfil.
					await _productosServiciosPerfilManager.DeleteByPerfilIdAsync(idPerfil);
				}
				else
				{
					//De lo contrario, crea a la empresa y obtiene su id.
					idPerfil = await _catalogoManager.CreateAsync(perfil);
				}

				//Crea los productos y servicios del perfil
				foreach (int? id in p.ProductosServicios)
				{
					if (id != null)
					{
						await _productosServiciosPerfilManager.CreateAsync(
							new ProductoServicioPerfil() { ProductoServicioId = id, PerfilId = idPerfil }
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

		public async Task<JsonResult> OnPostGetProductosServiciosSuggestion(string texto)
		{
			ServerResponse resp = new(true, _strLocalizer["ConsultadoUnsuccessfully"]);
			try
			{
				resp.Datos = await GetProductosServiciosSuggestion(texto);
				resp.TieneError = false;
				resp.Mensaje = _strLocalizer["ConsultadoSuccessfully"];
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}
		private async Task<string> GetProductosServiciosSuggestion(string texto)
		{
			string jsonResponse;
			List<string> jsons = [];

			List<ProductoServicioBuscado> prodserv = await _productosServiciosManager.SearchProductService(texto);

			if (prodserv != null)
			{
				foreach (ProductoServicioBuscado a in prodserv)
				{
					string additional = a.PalabrasSimilares.Length >= 1 ? $" ({a.PalabrasSimilares})" : string.Empty;
					jsons.Add($"{{" +
									$"\"id\": {a.Id}, " +
									$"\"value\": \"{a.Descripcion}\", " +
									$"\"label\": \"{a.Clave} - {a.Descripcion}{additional}\", " +
									$"\"clave\": \"{a.Clave}\"" +
								$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsons)}]";

			return jsonResponse;
		}
	}
}
