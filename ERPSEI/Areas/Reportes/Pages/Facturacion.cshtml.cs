using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.SAT.cfdiv40;
using ERPSEI.Pages.Shared;
using ERPSEI.Requests;
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
			IPerfilManager perfilManager,
			IStringLocalizer<FacturacionModel> localizer,
			ILogger<FacturacionModel> logger
		) : ERPPageModel
	{
		public class GraphicDataModel
		{
			public List<string> LabelValues { get; set; } = [];
			public List<decimal> PUEValues { get; set; } = [];
			public List<decimal> PPDValues { get; set; } = [];
			public List<decimal> PrefacturadoValues { get; set; } = [];
			public List<decimal> FacturadoValues { get; set; } = [];
			public List<decimal> DisponibleValues { get; set; } = [];
			public List<decimal> ExcedenteValues { get; set; } = [];
		}

		[BindProperty]
		public FiltroModel InputFiltro { get; set; } = new FiltroModel();

		public class FiltroModel
		{
			[Display(Name = "PerfilField")]
			public int? PerfilId { get; set; }

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
			List<Comprobante> comprobantes = [];
			List<Perfil> perfiles;
			List<Empresa> empresas;
			List<Empresa> empresasFinales = [];
			Dictionary<Perfil, Dictionary<Empresa, List<Comprobante>>> perfilesEmpresasComprobantes = [];

			perfiles = await perfilManager.GetAllAsync();
			if (filtro?.PerfilId != null) { perfiles = [.. perfiles.Where(p => p.Id == filtro.PerfilId)]; }

			empresas = await empresaManager.GetAllWithPerfil();
			if (filtro?.EmpresaRFC != null) { empresasFinales = [.. empresas.Where(e => e.RFC == filtro.EmpresaRFC)]; }

			foreach (Perfil p in perfiles)
			{
				empresasFinales.AddRange([.. empresas.Where(e => e.PerfilId == p.Id)]);
			}

            foreach (Empresa e in empresasFinales)
            {
				comprobantes = await comprobanteManager.GetComprobantesGraficas(
					e.RFC,
					filtro?.Anio,
					filtro?.Mes
				);

				if (e.Perfil != null) {
					if (!perfilesEmpresasComprobantes.ContainsKey(e.Perfil)) { perfilesEmpresasComprobantes.Add(e.Perfil, []); }
					if (!perfilesEmpresasComprobantes[e.Perfil].ContainsKey(e)) { perfilesEmpresasComprobantes[e.Perfil].Add(e, []); }

					perfilesEmpresasComprobantes[e.Perfil][e] = comprobantes; 
				}
			}

			jsonResponse = CreateJsonComprobantes(perfilesEmpresasComprobantes);

			return jsonResponse;
		}
		private static string CreateJsonComprobantes(Dictionary<Perfil, Dictionary<Empresa, List<Comprobante>>> comprobantes)
		{
			List<string> jsonComprobantes = [];
			string jsonResponse;

			decimal LIMITE_FACTURACION = 300000000;
			GraphicDataModel datosPorPerfil = new();
			GraphicDataModel datosPorEmpresa = new();

			//Se ordenan los perfiles de manera ascendente por Id
			var perfilesOrdenados = comprobantes.OrderBy(c => c.Key.Id);

			foreach (KeyValuePair<Perfil, Dictionary<Empresa, List<Comprobante>>> perfil in perfilesOrdenados)
			{
				decimal acumuladoPUEEmpresas = 0m;
				decimal acumuladoPPDEmpresas = 0m;
				decimal acumuladoPrefacturadoEmpresas = 0m;

				//Se ordenan las empresas de manera ascendente por Id
				var empresasOrdenadas = perfil.Value.OrderBy(p => p.Key.Id);

				datosPorPerfil.LabelValues.Add(perfil.Key.Nombre);

				foreach (KeyValuePair<Empresa, List<Comprobante>> empresa in empresasOrdenadas)
                {
					decimal acumuladoPUEComprobantes = 0m;
					decimal acumuladoPPDComprobantes = 0m;
					decimal acumuladoPrefacturadoComprobantes = 0m;

					datosPorEmpresa.LabelValues.Add(empresa.Key.RazonSocial);

                    foreach (Comprobante comprobante in empresa.Value)
                    {
						switch (comprobante.MetodoPago) 
						{
							case "PUE":
								acumuladoPUEComprobantes += comprobante.Total;
								break;
							case "PPD":
								acumuladoPPDComprobantes += comprobante.Total;
								break;
						}
                    }

					acumuladoPUEEmpresas += acumuladoPUEComprobantes;
					acumuladoPPDEmpresas += acumuladoPPDComprobantes;
					acumuladoPrefacturadoEmpresas += acumuladoPrefacturadoComprobantes;

					datosPorEmpresa.PUEValues.Add(acumuladoPUEComprobantes);
					datosPorEmpresa.PPDValues.Add(acumuladoPPDComprobantes);
					datosPorEmpresa.PrefacturadoValues.Add(acumuladoPrefacturadoComprobantes);
					datosPorEmpresa.FacturadoValues.Add(acumuladoPUEComprobantes + acumuladoPPDComprobantes + acumuladoPrefacturadoComprobantes);
					datosPorEmpresa.DisponibleValues.Add(datosPorEmpresa.FacturadoValues.Last() < LIMITE_FACTURACION ? LIMITE_FACTURACION - datosPorEmpresa.FacturadoValues.Last() : 0);
					datosPorEmpresa.ExcedenteValues.Add(datosPorEmpresa.FacturadoValues.Last() >= LIMITE_FACTURACION ? datosPorEmpresa.FacturadoValues.Last() - LIMITE_FACTURACION : 0);
				}

				datosPorPerfil.PUEValues.Add(acumuladoPUEEmpresas);
				datosPorPerfil.PPDValues.Add(acumuladoPPDEmpresas);
				datosPorPerfil.PrefacturadoValues.Add(acumuladoPrefacturadoEmpresas);
				datosPorPerfil.FacturadoValues.Add(acumuladoPUEEmpresas + acumuladoPPDEmpresas + acumuladoPrefacturadoEmpresas);
				datosPorPerfil.DisponibleValues.Add(datosPorPerfil.FacturadoValues.Last() < LIMITE_FACTURACION ? LIMITE_FACTURACION - datosPorPerfil.FacturadoValues.Last() : 0);
				datosPorPerfil.ExcedenteValues.Add(datosPorPerfil.FacturadoValues.Last() >= LIMITE_FACTURACION ? datosPorPerfil.FacturadoValues.Last() - LIMITE_FACTURACION : 0);
			}

			jsonResponse = $"{{" +
					$"\"Perfiles\":" +
						$"{JsonConvert.SerializeObject(datosPorPerfil)}," +
					$"\"Empresas\":" +
						$"{JsonConvert.SerializeObject(datosPorEmpresa)}" +
				$"}}";

			return jsonResponse;
		}

		public async Task<JsonResult> OnGetEmpresaSuggestion(string texto)
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
