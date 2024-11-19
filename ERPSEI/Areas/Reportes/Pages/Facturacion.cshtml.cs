using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.SAT.cfdiv40;
using ERPSEI.Pages.Shared;
using ERPSEI.Requests;
using ERPSEI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Areas.Reportes.Pages
{
	[Authorize(Policy = "AccessPolicy")]
	public class FacturacionModel(
			IEmpresaManager empresaManager,
			IComprobanteManager comprobanteManager,
			IStringLocalizer<FacturacionModel> localizer,
			ILogger<FacturacionModel> logger
		) : ERPPageModel
	{

		[BindProperty]
		public FiltroModel InputFiltro { get; set; } = new FiltroModel();

		public class FiltroModel
		{
			[Required(ErrorMessage = "Required")]
			[Display(Name = "EmpresaField")]
			public string? EmpresaRFC { get; set; }

			[Display(Name = "AnioField")]
			public string? Anio { get; set; }

			[Display(Name = "MesField")]
			public string? Mes { get; set; }
		}

		public IActionResult OnGet()
		{
			return Page();
		}

		private string CreateJsonComprobantes(List<Comprobante> comprobantes)
		{
			List<string> jsonComprobantes = [];
			string jsonResponse;

			foreach (Comprobante c in comprobantes)
			{
				jsonComprobantes.Add(
					"{" +
						$"\"id\": {c.Id}," +
						$"\"contabilizado\": \"{(c.Contabilizado ?? false ? 1 : 0)}\"" +
				"}"
				);
			}

			jsonResponse = $"[{string.Join(",", jsonComprobantes)}]";

			return jsonResponse;
		}

		public async Task<JsonResult> OnPostFiltrar()
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetComprobantesList(InputFiltro);
					resp.TieneError = false;
					resp.Mensaje = localizer["ConsultadoSuccessfully"];
				}
				else
				{
					resp.Mensaje = localizer["AccesoDenegado"];
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				logger.LogError("{message}", message);
			}

			return new JsonResult(resp);
		}
		private async Task<string> GetComprobantesList(FiltroModel? filtro = null)
		{
			string jsonResponse;
			List<Comprobante> comprobantes;

			comprobantes = await comprobanteManager.GetAllAsync(
				filtro?.EmpresaRFC,
				filtro?.Anio,
				filtro?.Mes
			);

			jsonResponse = CreateJsonComprobantes(comprobantes);

			return jsonResponse;
		}

		public async Task<JsonResult> OnPostGetEmpresaSuggestion(string texto)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetEmpresasSuggestion(texto);
					resp.TieneError = false;
					resp.Mensaje = localizer["ConsultadoSuccessfully"];
				}
				else
				{
					resp.Mensaje = localizer["AccesoDenegado"];
				}
			}
			catch (Exception ex)
			{
				logger.LogError(message: ex.Message);
			}

			return new JsonResult(resp);
		}
		private async Task<string> GetEmpresasSuggestion(string texto)
		{
			string jsonResponse;
			List<string> jsonResult = [];

			List<EmpresaBuscada> empresas = await empresaManager.SearchEmpresas(texto);

			if (empresas != null)
			{
				foreach (EmpresaBuscada e in empresas)
				{
					e.RazonSocial = JsonEscape(e.RazonSocial ?? string.Empty);

					jsonResult.Add($"{{" +
										$"\"id\": {e.Id}, " +
										$"\"value\": \"{e.RazonSocial}\", " +
										$"\"label\": \"{e.RFC} - {e.RazonSocial}\", " +
										$"\"rfc\": \"{e.RFC}\"" +
									$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsonResult)}]";

			return jsonResponse;
		}

		private static string JsonEscape(string str)
		{
			return str.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
		}
	}
}
