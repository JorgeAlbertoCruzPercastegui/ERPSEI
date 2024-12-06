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
			IBancoEmpresaManager bancoEmpresaManager,
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
			public List<decimal> PorcentajeDisponible { get; set; } = [];
			public List<string> NivelesValues { get; set; } = [];
		}

		[BindProperty]
		public FiltroModel InputFiltro { get; set; } = new FiltroModel();

		public class FiltroModel
		{
			[Display(Name = "PerfilField")]
			public int? PerfilId { get; set; }

			[Display(Name = "EmpresaField")]
			public string? EmpresaRFC { get; set; }

			[Display(Name = "NivelField")]
			public int? NivelId { get; set; }

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
			Dictionary<Perfil, Dictionary<Empresa, List<Comprobante>>> perfilesEmpresasComprobantes = [];
			Perfil emptyPerfil = new() { Nombre = localizer["SinPerfilAsignado"] };

			perfiles = await perfilManager.GetAllAsync();
			empresas = await empresaManager.GetAllWithPerfilAndNivel();
			if (filtro?.PerfilId != null && filtro?.PerfilId >= 1) {
				empresas = [.. empresas.Where(e => e.PerfilId == filtro?.PerfilId)];
				perfiles = [.. perfiles.Where(p => p.Id == filtro.PerfilId)];
			}
			else if(filtro?.PerfilId == -1) { empresas = [.. empresas.Where(e => e.Perfil == null)]; }
			else
			{
				List<int> perfilesIds = [..perfiles.Select(p => p.Id)];
				empresas = [.. empresas.Where(e => perfilesIds.Contains(e.PerfilId??0))];
			}
			if (filtro?.EmpresaRFC != null) { empresas = [.. empresas.Where(e => e.RFC == filtro?.EmpresaRFC)]; }
			if (filtro?.NivelId != null) { empresas = [.. empresas.Where(e => e.NivelId == filtro?.NivelId)]; }

			if (empresas.Count <= 0) {
				return $"{{" +
					$"\"Perfiles\":" +
						$"{JsonConvert.SerializeObject(new GraphicDataModel())}," +
					$"\"Empresas\":" +
						$"{JsonConvert.SerializeObject(new GraphicDataModel())}" +
				$"}}";
			}

			comprobantes = await comprobanteManager.GetComprobantesGraficas(filtro?.Anio, filtro?.Mes);

			foreach (Empresa e in empresas)
            {
				if (!perfilesEmpresasComprobantes.ContainsKey(e.Perfil ?? emptyPerfil)) { perfilesEmpresasComprobantes.Add(e.Perfil ?? emptyPerfil, []); }
				if (!perfilesEmpresasComprobantes[e.Perfil ?? emptyPerfil].ContainsKey(e)) { perfilesEmpresasComprobantes[e.Perfil ?? emptyPerfil].Add(e, []); }

				perfilesEmpresasComprobantes[e.Perfil ?? emptyPerfil][e] = [.. from Comprobante c in comprobantes where c.Emisor?.Rfc == e.RFC select c]; 
				
			}

			jsonResponse = await CreateJsonComprobantes(perfilesEmpresasComprobantes, filtro?.Mes == null ? 12 : 1);

			return jsonResponse;
		}
		private async Task<string> CreateJsonComprobantes(Dictionary<Perfil, Dictionary<Empresa, List<Comprobante>>> comprobantes, int countMeses)
		{
			List<string> jsonComprobantes = [];
			string jsonResponse;

			GraphicDataModel datosPorPerfil = new();
			GraphicDataModel datosPorEmpresa = new();

			//Se ordenan los perfiles de manera ascendente por Id
			var perfilesOrdenados = comprobantes.OrderBy(c => c.Key.Id);

			foreach (KeyValuePair<Perfil, Dictionary<Empresa, List<Comprobante>>> perfil in perfilesOrdenados)
			{
				decimal acumuladoPUEEmpresas = 0m;
				decimal acumuladoPPDEmpresas = 0m;
				decimal acumuladoPrefacturadoEmpresas = 0m;
				decimal disponible = 0m;
				decimal LIMITE_FACTURACION_PERFIL = 0;
				decimal LIMITE_FACTURACION_EMPRESA = 0;

				List<KeyValuePair<int, decimal>> empresasIdTotales = [];

				LIMITE_FACTURACION_PERFIL = 0m;

                //Se obtiene el limite de facturación del perfil.
                foreach (KeyValuePair<Empresa, List<Comprobante>> empresa in perfil.Value)
                {
					decimal acumuladoComprobantes = 0m;
					LIMITE_FACTURACION_EMPRESA = 0m;

					foreach (Comprobante comprobante in empresa.Value){ acumuladoComprobantes += comprobante.Total; }

					//El límite de facturación mensual de la empresa será la sumatoria del límite de facturación mensual de todos sus bancos
					foreach(BancoEmpresa b in await bancoEmpresaManager.GetBancosByEmpresaIdAsync(empresa.Key.Id)){ LIMITE_FACTURACION_EMPRESA += b.Limite; }

					//Se multiplica el límite de facturación mensual por la cantidad de meses seleccionada dentro del rango del reporte.
					LIMITE_FACTURACION_EMPRESA = LIMITE_FACTURACION_EMPRESA * countMeses;

					LIMITE_FACTURACION_PERFIL += LIMITE_FACTURACION_EMPRESA;
				}

				//Se obtiene el porcentaje de disponibilidad para cada empresa.
				foreach (KeyValuePair<Empresa, List<Comprobante>> empresa in perfil.Value)
				{
                    decimal acumuladoComprobantes = 0m;
					decimal disp = 0m;
					LIMITE_FACTURACION_EMPRESA = 0m;

					foreach (Comprobante comprobante in empresa.Value){ acumuladoComprobantes += comprobante.Total; }

					//El límite de facturación mensual de la empresa será la sumatoria del límite de facturación mensual de todos sus bancos
					foreach(BancoEmpresa b in await bancoEmpresaManager.GetBancosByEmpresaIdAsync(empresa.Key.Id)){ LIMITE_FACTURACION_EMPRESA += b.Limite; }

					//Se multiplica el límite de facturación mensual por la cantidad de meses seleccionada dentro del rango del reporte.
					LIMITE_FACTURACION_EMPRESA = LIMITE_FACTURACION_EMPRESA * countMeses;

					disp = (LIMITE_FACTURACION_EMPRESA * 100) / (LIMITE_FACTURACION_PERFIL <= 0 ? 1 : LIMITE_FACTURACION_PERFIL);
					empresasIdTotales.Add(new(empresa.Key.Id, disp));
				}

                empresasIdTotales = [..empresasIdTotales.OrderByDescending(e => e.Value)];

				datosPorPerfil.LabelValues.Add(perfil.Key.Nombre);

				foreach (KeyValuePair<int, decimal> emp in empresasIdTotales)
                {
					decimal acumuladoPUEComprobantes = 0m;
					decimal acumuladoPPDComprobantes = 0m;
					decimal acumuladoPrefacturadoComprobantes = 0m;
					LIMITE_FACTURACION_EMPRESA = 0m;

					KeyValuePair<Empresa,List<Comprobante>> empresa = perfil.Value.Where(e => e.Key.Id == emp.Key).First();

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

					//El límite de facturación mensual de la empresa será la sumatoria del límite de facturación mensual de todos sus bancos
					foreach (BancoEmpresa b in await bancoEmpresaManager.GetBancosByEmpresaIdAsync(empresa.Key.Id)) { LIMITE_FACTURACION_EMPRESA += b.Limite; }

					//Se multiplica el límite de facturación mensual por la cantidad de meses seleccionada dentro del rango del reporte.
					LIMITE_FACTURACION_EMPRESA = LIMITE_FACTURACION_EMPRESA * countMeses;

					acumuladoPUEEmpresas += acumuladoPUEComprobantes;
					acumuladoPPDEmpresas += acumuladoPPDComprobantes;
					acumuladoPrefacturadoEmpresas += acumuladoPrefacturadoComprobantes;

					datosPorEmpresa.NivelesValues.Add(empresa.Key.Nivel?.Nombre ?? string.Empty);
					datosPorEmpresa.PUEValues.Add(acumuladoPUEComprobantes);
					datosPorEmpresa.PPDValues.Add(acumuladoPPDComprobantes);
					datosPorEmpresa.PrefacturadoValues.Add(acumuladoPrefacturadoComprobantes);
					datosPorEmpresa.FacturadoValues.Add(acumuladoPUEComprobantes + acumuladoPPDComprobantes + acumuladoPrefacturadoComprobantes);
					disponible = datosPorEmpresa.FacturadoValues.Last() < LIMITE_FACTURACION_EMPRESA ? LIMITE_FACTURACION_EMPRESA - datosPorEmpresa.FacturadoValues.Last() : 0;
					datosPorEmpresa.DisponibleValues.Add(disponible);
					datosPorEmpresa.PorcentajeDisponible.Add(Math.Round((LIMITE_FACTURACION_EMPRESA * 100) / (LIMITE_FACTURACION_PERFIL <= 0 ? 1 : LIMITE_FACTURACION_PERFIL)));
					datosPorEmpresa.ExcedenteValues.Add(datosPorEmpresa.FacturadoValues.Last() >= LIMITE_FACTURACION_EMPRESA ? datosPorEmpresa.FacturadoValues.Last() - LIMITE_FACTURACION_EMPRESA : 0);
				}

				datosPorPerfil.PUEValues.Add(acumuladoPUEEmpresas);
				datosPorPerfil.PPDValues.Add(acumuladoPPDEmpresas);
				datosPorPerfil.PrefacturadoValues.Add(acumuladoPrefacturadoEmpresas);
				datosPorPerfil.FacturadoValues.Add(acumuladoPUEEmpresas + acumuladoPPDEmpresas + acumuladoPrefacturadoEmpresas);
				disponible = datosPorPerfil.FacturadoValues.Last() < LIMITE_FACTURACION_PERFIL ? LIMITE_FACTURACION_PERFIL - datosPorPerfil.FacturadoValues.Last() : 0;
				datosPorPerfil.DisponibleValues.Add(disponible);
				datosPorPerfil.PorcentajeDisponible.Add(Math.Round((disponible * 100) / (LIMITE_FACTURACION_PERFIL <= 0 ? 1 : LIMITE_FACTURACION_PERFIL), 0));
				datosPorPerfil.ExcedenteValues.Add(datosPorPerfil.FacturadoValues.Last() >= LIMITE_FACTURACION_PERFIL ? datosPorPerfil.FacturadoValues.Last() - LIMITE_FACTURACION_PERFIL : 0);
			}

			jsonResponse = $"{{" +
					$"\"Perfiles\":" +
						$"{JsonConvert.SerializeObject(datosPorPerfil)}," +
					$"\"Empresas\":" +
						$"{JsonConvert.SerializeObject(datosPorEmpresa)}" +
				$"}}";

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
										$"\"rfc\": \"{e.RFC}\", " +
										$"\"perfil\": \"{e.Perfil}\", " +
										$"\"nivel\": \"{e.Nivel}\"" +
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
