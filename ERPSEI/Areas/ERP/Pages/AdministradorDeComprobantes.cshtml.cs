using ERPSEI.Data;
using ERPSEI.Data.Entities.Cuentas;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.AdministradorPolizas;
using ERPSEI.Data.Managers.Cuentas;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.SAT.cfdiv40;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Pages.Shared;
using ERPSEI.Requests;
using ERPSEI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NuGet.Packaging;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using WS_SAT_ConsultaEstatusCFDI;
using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Managers.Polizas;
using ERPSEI.Data.Managers.AdministradorPolizas;
using System.Text.RegularExpressions;

namespace ERPSEI.Areas.ERP.Pages
{
	[Authorize(Policy = "AccessPolicy")]
	public class AdministradorDeComprobantesModel(
			ApplicationDbContext db,
			AppUserManager userManager,
			IEmpresaManager empresaManager,
			IComprobanteEmisorManager comprobanteEmisorManager,
			IComprobanteReceptorManager comprobanteReceptorManager,
			IComprobanteManager comprobanteManager,
			ICuentaContableManager cuentaContableManager,
			IGruposPolizasManager gruposPolizas,
			IStringLocalizer<AdministradorDeComprobantesModel> localizer,
			ILogger<AdministradorDeComprobantesModel> logger,
			IEncriptacionAES encriptacionAES
		) : ERPPageModel
	{
		public class SinCuentasException : Exception
		{
			public SinCuentasException(string? message) : base(message)
			{
			}
		}

		public class RFCCuenta
		{
			public string Nombre { get; set; } = string.Empty;
			public string RFC { get; set; } = string.Empty;
			public string Cuenta { get; set; } = string.Empty;
		}

		public class ComprobanteIdCuentaId
		{
			public int Id { get; set; } 
			public int CuentaId { get; set; }
		}

		public class ComprobantesYCuentasPoliza
		{
			public Dictionary<Comprobante, CuentaContable> ComprobantesYCuentas { get; set; } = [];

			public Dictionary<string, CuentaContable> CuentasAuxiliares { get; set; } = [];
		}

		public enum TipoExportacion
		{
			PDF = 0,
			XML,
			Excel,
			PolizaIngresos,
			PolizaEgresos
		}

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

			[Display(Name = "EstatusField")]
			public int? EstatusId { get; set; }

			[Display(Name = "TipoField")]
			public int? TipoId { get; set; }

			[Display(Name = "EstatusContableField")]
			public int? EstatusContableId { get; set; }

			[Display(Name = "TipoComprobanteField")]
			public string? TipoComprobanteClave { get; set; }

			[Display(Name = "FormaPagoField")]
			public string? FormaPagoClave { get; set; }

			[Display(Name = "MetodoPagoField")]
			public string? MetodoPagoClave { get; set; }

			[Display(Name = "UsoCFDIField")]
			public string? UsoCFDIClave { get; set; }

			[Display(Name = "EmisorField")]
			public string? EmisorRFC { get; set; }

			[Display(Name = "ReceptorField")]
			public string? ReceptorRFC { get; set; }
		}

		[BindProperty]
		public CuentaContableModel InputCuentaContable { get; set; } = new CuentaContableModel();

		public class CuentaContableModel
		{
			[Display(Name = "SearchCuentaContableField")]
			public int? CuentaContableId { get; set; }
		}

		public IActionResult OnGet(int tipoId)
		{
			if (tipoId <= 0 || tipoId >= 3){ return RedirectToPage("/404"); }
			
			InputFiltro.TipoId = tipoId;
			return Page();

		}

		private string CreateJsonComprobantes(List<Comprobante> comprobantes)
		{
			List<string> jsonComprobantes = [];
			string jsonResponse;

			foreach (Comprobante c in comprobantes)
			{
				DateTime? fecha = c.Fecha == DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss") || string.IsNullOrEmpty(c.Fecha) ? null : DateTime.ParseExact(c.Fecha, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

				AppUser? usr = userManager.GetUserAsync(User).Result;
				string safeL = string.Empty;
				if (usr != null)
				{
					safeL = $"userId={usr.Id}&id={c.Id}&module=administradordecomprobantes";
					safeL = encriptacionAES.PlainTextToBase64AES(safeL);
				}

				jsonComprobantes.Add(
					"{" +
						$"\"id\": {c.Id}," +
						$"\"safeL\": \"{safeL}\"," +
						$"\"serie\": \"{c.Serie ?? "F"}\", " +
						$"\"folio\": \"{c.Folio ?? "0"}\", " +
						$"\"fecha\": \"{fecha:dd/MM/yyyy HH:mm:ss}\", " +
						$"\"fechaJS\": \"{fecha:yyyy-MM-dd HH:mm:ss}\", " +
						$"\"uuid\": \"{c.Complemento?.TimbreFiscalDigital?.UUID}\", " +
						$"\"formaPago\": \"{c.FormaPago}\", " +
						$"\"subtotal\": \"{c.SubTotal}\", " +
						$"\"descuento\": \"{c.Descuento}\", " +
						$"\"moneda\": \"{c.Moneda}\", " +
						$"\"tipoCambio\": {c.TipoCambio}, " +
						$"\"total\": \"{c.Total}\", " +
						$"\"tipoComprobante\": \"{c.TipoDeComprobante}\", " +
						$"\"metodoPago\": \"{c.MetodoPago}\", " +
						$"\"lugarExpedicion\": \"{c.LugarExpedicion}\", " +
						$"\"emisor\": \"{c.Emisor?.Rfc}\", " +
						$"\"receptor\": \"{c.Receptor?.Rfc}\", " +
						$"\"usoCFDI\": \"{c.Receptor?.UsoCFDI}\", " +
						$"\"cancelado\": \"{(c.Cancelado ?? false ? 1 : 0)}\", " +
						$"\"valido\": \"{(c.Valido ?? false ? 1 : 0)}\", " +
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
				filtro?.Mes,
				filtro?.EstatusId,
				filtro?.TipoId,
				filtro?.EstatusContableId,
				filtro?.TipoComprobanteClave,
				filtro?.FormaPagoClave,
				filtro?.MetodoPagoClave,
				filtro?.UsoCFDIClave,
				filtro?.EmisorRFC,
				filtro?.ReceptorRFC
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

		public async Task<JsonResult> OnPostGetEmisorSuggestion(string texto)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetEmisorSuggestion(texto);
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
				logger.LogError("{message}", ex.Message);
			}

			return new JsonResult(resp);
		}
		private async Task<string> GetEmisorSuggestion(string texto)
		{
			string jsonResponse;
			List<string> jsonResult = [];

			List<ComprobanteEmisor> emisores = await comprobanteEmisorManager.SearchEmisor(texto);
			emisores = [.. emisores.Take(20)];

			if (emisores != null)
			{
				foreach (ComprobanteEmisor e in emisores)
				{
					e.Nombre = JsonEscape(e.Nombre??string.Empty);

					jsonResult.Add($"{{" +
										$"\"id\": {e.Id}, " +
										$"\"value\": \"{e.Nombre}\", " +
										$"\"label\": \"{e.Rfc} - {e.Nombre}\", " +
										$"\"rfc\": \"{e.Rfc}\"" +
									$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsonResult)}]";

			return jsonResponse;
		}

		public async Task<JsonResult> OnPostGetReceptorSuggestion(string texto)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetReceptorSuggestion(texto);
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
				logger.LogError("{message}", ex.Message);
			}

			return new JsonResult(resp);
		}
		private async Task<string> GetReceptorSuggestion(string texto)
		{
			string jsonResponse;
			List<string> jsonResult = [];

			List<ComprobanteReceptor> receptores = await comprobanteReceptorManager.SearchReceptor(texto);
			receptores = [.. receptores.Take(20)];

			if (receptores != null)
			{
				foreach (ComprobanteReceptor r in receptores)
				{
					r.Nombre = JsonEscape(r.Nombre ?? string.Empty);

					jsonResult.Add($"{{" +
										$"\"id\": {r.Id}, " +
										$"\"value\": \"{r.Nombre}\", " +
										$"\"label\": \"{r.Rfc} - {r.Nombre}\", " +
										$"\"rfc\": \"{r.Rfc}\"" +
									$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsonResult)}]";

			return jsonResponse;
		}

		public async Task<JsonResult> OnPostGetCuentasContablesSuggestion(string texto, string rfcreceptor)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetCuentasContablesSuggestion(texto, rfcreceptor);
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
				logger.LogError("{message}", ex.Message);
			}

			return new JsonResult(resp);
		}
		private async Task<string> GetCuentasContablesSuggestion(string texto, string rfcreceptor)
		{
			string jsonResponse;
			List<string> jsonResult = [];
			int tipoCuentaEgresosId = 1;
			int subtipoCuentaGastosId = 2;
			List<CuentaContable> cuentas = await cuentaContableManager.SearchCuentas(texto, rfcreceptor, tipoCuentaEgresosId, subtipoCuentaGastosId);

			if (cuentas != null)
			{
				foreach (CuentaContable r in cuentas)
				{
					r.Nombre = JsonEscape(r.Nombre ?? string.Empty);
					r.Cuenta = JsonEscape(r.Cuenta ?? string.Empty);
					r.RFC = JsonEscape(r.RFC ?? string.Empty);

					jsonResult.Add($"{{" +
										$"\"id\": {r.Id}, " +
										$"\"value\": \"{r.Cuenta}\", " +
										$"\"label\": \"{r.Cuenta} - {r.Nombre}\"" +
									$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsonResult)}]";

			return jsonResponse;
		}

		public async Task<JsonResult> OnPostSaveGrupoPoliza()
		{
			ServerResponse resp = new(true, localizer["PolicySavedUnsuccessfully"]);

			try
			{
					// Crear una nueva instancia de GrupoPoliza con los GUID correctos
					GrupoPoliza poliza = new GrupoPoliza
					{
						UsuarioCreadorId = Guid.Parse("d9cec426-9633-4319-8b97-e6d35fa2ac36").ToString(),
						UsuarioModificadorId = Guid.Parse("84fc78bb-f3a7-4719-a289-07361651d85e").ToString(),
						FechaHoraCreacion = DateTime.Now,
						FechaHoraModificacion = DateTime.Now,
						NumeroImpresion = 1,
						Deshabilitado = false
					};

					await gruposPolizas.CreateAsync(poliza);

					resp.TieneError = false;
					resp.Mensaje = localizer["PolicySavedSuccessfully"];
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				resp.Mensaje = localizer["AnErrorOccurred"];
			}

			return new JsonResult(resp);
		}

		private static string JsonEscape(string str)
		{
			return str.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
		}

		public async Task<ActionResult> OnPostExportCFDIS(string[] ids, int tipoExportado, string[]? cuentasClientesGuardables)
		{
			ServerResponse resp = new(true, localizer["ComprobantesExportedUnsuccessfully"]);
			if (PuedeTodo || PuedeConsultar || PuedeEditar)
			{
				try
				{
					switch (tipoExportado)
					{
						case (int)TipoExportacion.PDF:
							resp.TieneError = false;
							resp.Mensaje = localizer["ComprobantesExportedSuccessfully"];
							break;
						case (int)TipoExportacion.XML:
							resp.TieneError = false;
							resp.Mensaje = localizer["ComprobantesExportedSuccessfully"];
							break;
						case (int)TipoExportacion.Excel:
							resp.TieneError = false;
							resp.Mensaje = localizer["ComprobantesExportedSuccessfully"];
							break;
						case (int)TipoExportacion.PolizaIngresos:
							if (PuedeTodo || PuedeEditar)
							{
								// Llamar al método OnPostSaveGrupoPoliza y esperar su resultado
								var savePolizaResult = await OnPostSaveGrupoPoliza();

								// Verificar si hubo errores al guardar la póliza
								if (savePolizaResult.Value is ServerResponse savePolizaResp && !savePolizaResp.TieneError)
								{
									// Si guardar la póliza fue exitoso, proceder con la exportación
									resp.Datos = await CreateExcelPolizaIngresos(ids, cuentasClientesGuardables);
									resp.TieneError = false;
									resp.Mensaje = localizer["ComprobantesExportedSuccessfully"];
								}
								else
								{
									// Si hubo un error al guardar la póliza, asignar el mensaje de error
									//resp.Mensaje = savePolizaResp?.Mensaje ?? localizer["PolicySaveFailed"];
									resp.TieneError = true;
								}
							}
							else
							{
								resp.Mensaje = localizer["AccesoDenegado"];
								resp.TieneError = true;
							}
							break;

						case (int)TipoExportacion.PolizaEgresos:
							if (PuedeTodo || PuedeEditar)
							{
								resp.Datos = await CreateExcelPolizaEgresos(ids);
								resp.TieneError = false;
								resp.Mensaje = localizer["ComprobantesExportedSuccessfully"];
							}
							else
							{
								resp.Mensaje = localizer["AccesoDenegado"];
							}
							break;
						default:
							break;
					}
				}
				catch (SinCuentasException sinCuentasException)
				{
					resp.Mensaje = sinCuentasException.Message;
					resp.CodigoError = 1;
				}
				catch (Exception ex)
				{
					logger.LogError("{message}", ex.Message);
					resp.Mensaje = ex.Message;
				}
			}
			else
			{
				resp.Mensaje = localizer["AccesoDenegado"];
			}

			return new JsonResult(resp);
		}

		private static Task<string> CreateExcel() { return Task.FromResult(string.Empty); }
		private async Task<string> CreateExcelPolizaEgresos(string[] ids)
		{
			int rowIndex = 2;
			string? strTipoPoliza = "GASTOS";
			string? nombreArchivo = string.Empty;
			XSSFWorkbook? wb = (await CreateWorkbookEgresos()) ?? throw new Exception(localizer["NoWorkbookCreated"]);
			using (wb)
			{
				//Configura la primer hoja del archivo
				ISheet sheet = ConfigureFirstSheetPolizas(wb);

				//Crea el estilo de las celdas.
				XSSFCellStyle cellStyle = CreateCellStylePolizas(wb);

				Empresa? empresaReceptora = null;

				Dictionary<Comprobante, int> comprobantes = [];
				List<ComprobanteIdCuentaId>? elements = JsonConvert.DeserializeObject<List<ComprobanteIdCuentaId>>($"[{string.Join(",", ids)}]") ?? [];
				foreach (ComprobanteIdCuentaId cyc in elements)
				{
					Comprobante? comprobante = await comprobanteManager.GetByIdAsync(cyc.Id);
					if (comprobante != null)
					{
						empresaReceptora ??= await empresaManager.GetByRFCAsync(comprobante.Receptor?.Rfc ?? string.Empty);
						comprobante.FechaNET = DateTime.ParseExact(comprobante.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
						comprobantes.Add(comprobante, cyc.CuentaId);
					}
				}

				var comprobantesOrdenados = from entry in comprobantes orderby entry.Key.FechaNET ascending select entry;

				List<CuentaContable> ? cuentasContables = await cuentaContableManager.GetByIdEmpresaAsync(empresaReceptora?.Id ?? 0);
				cuentasContables = cuentasContables.Where(c => c.TipoId == 1).ToList();
				if (cuentasContables == null || cuentasContables.Count <= 0) { throw new Exception($"{localizer["SinCuentasContables"]} {localizer["PolizaNoCreada"]}"); }

				CuentaContable? cuentaIVAPorAcreditar = cuentasContables.Where(cuenta => cuenta.TipoId == 1 && cuenta.SubtipoId == 10).FirstOrDefault();
				CuentaContable? cuentaIVAAcreditable = cuentasContables.Where(cuenta => cuenta.TipoId == 1 && cuenta.SubtipoId == 9).FirstOrDefault();
				if (cuentaIVAPorAcreditar == null || cuentaIVAAcreditable == null) { throw new Exception($"{localizer["SinCuentasContablesIVAEgreso"]} {localizer["PolizaNoCreada"]}"); }

				CuentaContable? cuentaProveedor = null;
				CuentaContable? cuentaGasto = null;

				string conceptoString = string.Empty;
				foreach (KeyValuePair<Comprobante, int> kvp in comprobantesOrdenados)
				{
					conceptoString = $"PROVISION DE {strTipoPoliza} '{kvp.Key.Emisor?.Nombre}' {kvp.Key.Serie ?? "F"}-{kvp.Key.Folio}";
					cuentaProveedor = cuentasContables.Where(cuenta => cuenta.RFC == kvp.Key.Emisor?.Rfc).FirstOrDefault();
					if (cuentaProveedor == null) { throw new Exception($"{localizer["SinCuentaContableProveedor"]} {kvp.Key.Emisor?.Nombre}. {localizer["PolizaNoCreada"]}"); }

					cuentaGasto = cuentasContables.Where(cuenta => cuenta.Id == kvp.Value).FirstOrDefault();
					if (cuentaGasto == null) { throw new Exception($"{localizer["SinCuentaContableGasto"]} {localizer["PolizaNoCreada"]}"); }

					//Crea el row de encabezado de CFDI
					IRow hRow = sheet.CreateRow(rowIndex);
					//Tipo Pol
					CreateCell(hRow, 0, "Dr", cellStyle);
					//Placeholder
					CreateCell(hRow, 1, 1, cellStyle);
					//Concepto póliza
					CreateCell(hRow, 2, conceptoString, cellStyle);
					//Día fecha
					CreateCell(hRow, 3, kvp.Key.FechaNET.Day, cellStyle);

					//Crea el row del total de la factura
					IRow dRow = sheet.CreateRow(rowIndex + 1);
					//No. Cuenta
					CreateCell(dRow, 1, cuentaProveedor?.Cuenta ?? "0000-000-000", cellStyle);
					//Depto.
					CreateCell(dRow, 2, 0, cellStyle);
					//Concepto
					CreateCell(dRow, 3, conceptoString, cellStyle);
					//Placeholder
					CreateCell(dRow, 4, string.Empty, cellStyle);
					//Total
					CreateCell(dRow, 6, (double)kvp.Key.Total, cellStyle);

					//Crea el row del subtotal de la factura de CFDI
					IRow g1Row = sheet.CreateRow(rowIndex + 2);
					//No. Cuenta
					CreateCell(g1Row, 1, cuentaGasto?.Cuenta ?? "0000-000-000", cellStyle);
					//Depto.
					CreateCell(g1Row, 2, 0, cellStyle);
					//Concepto
					CreateCell(g1Row, 3, conceptoString, cellStyle);
					//Placeholder
					CreateCell(g1Row, 4, string.Empty, cellStyle);
					//Debe
					CreateCell(g1Row, 5, (double)kvp.Key.SubTotal, cellStyle);
					//Haber
					CreateCell(g1Row, 6, "", cellStyle);

					//Crea el row del IVA
					IRow g2Row = sheet.CreateRow(rowIndex + 3);
					//No. Cuenta
					CreateCell(g2Row, 1, cuentaIVAPorAcreditar?.Cuenta ?? "0000-000-000", cellStyle);
					//Depto.
					CreateCell(g2Row, 2, 0, cellStyle);
					//Concepto
					CreateCell(g2Row, 3, conceptoString, cellStyle);
					//Placeholder
					CreateCell(g1Row, 4, string.Empty, cellStyle);
					//Debe
					CreateCell(g2Row, 5, (double)(kvp.Key.Impuestos?.TotalImpuestosTrasladados ?? 0), cellStyle);
					//Haber
					CreateCell(g2Row, 6, "", cellStyle);

					//Crea el row de fin de partida
					IRow fRow = sheet.CreateRow(rowIndex + 4);
					//Fin
					CreateCell(fRow, 1, "FIN_PARTIDAS", cellStyle);

					//Avanza 5 lineas para poder iniciar una nueva póliza.
					rowIndex += 5;

					kvp.Key.Contabilizado = true;
				}

				//Crea el archivo excel y lo exporta al usuario.
				nombreArchivo = $"{Enum.GetName(typeof(TipoExportacion), TipoExportacion.PolizaEgresos)}_{DateTime.Now:yyyyMMddHHmmssfffffff}";
				using (var fileData = new FileStream($"wwwroot/templates/{nombreArchivo}.xlsx", FileMode.OpenOrCreate)) { wb.Write(fileData); }
				wb.Close();

				//Actualiza los comprobantes para que queden marcados con el flag "Contabilizado = true"
				await comprobanteManager.UpdateMultipleAsync([..comprobantes.Keys]);
			}

			return nombreArchivo;
		}
		private async Task<string> CreateExcelPolizaIngresos(string[] ids, string[]? cuentasClientesGuardables)
		{
			int rowIndex = 2;
			string? strTipoPoliza = "VENTA";
			string? nombreArchivo = string.Empty;
			XSSFWorkbook? wb = (wb = await CreateWorkbookIngresos()) ?? throw new Exception(localizer["NoWorkbookCreated"]);
			using (wb)
			{
				//Configura la primer hoja del archivo
				ISheet sheet = ConfigureFirstSheetPolizas(wb);

				//Crea el estilo de las celdas.
				XSSFCellStyle cellStyle = CreateCellStylePolizas(wb);

				//Obtiene los comprobantes seleccionados
				ComprobantesYCuentasPoliza ccp = await GetComprobantesYCuentasByIdForPolizaIngresos(ids, cuentasClientesGuardables);

				string conceptoString = string.Empty;
				CuentaContable? cuentaVenta = null;
				foreach (KeyValuePair<Comprobante, CuentaContable> comprobanteYCuenta in ccp.ComprobantesYCuentas)
				{
					conceptoString = $"PROVISION DE {strTipoPoliza} '{comprobanteYCuenta.Key.Receptor?.Nombre}' {comprobanteYCuenta.Key.Serie ?? "F"}-{comprobanteYCuenta.Key.Folio}";
					if (comprobanteYCuenta.Key.Impuestos != null && (comprobanteYCuenta.Key.Impuestos.Traslados?.Any(t => t.TasaOCuota == 0.16m) ?? false)) { cuentaVenta = ccp.CuentasAuxiliares["Ventas16"]; }
					else if (comprobanteYCuenta.Key.Impuestos != null && (comprobanteYCuenta.Key.Impuestos.Traslados?.Any(t => t.TasaOCuota == 0.0m) ?? false)) { cuentaVenta = ccp.CuentasAuxiliares["Ventas0"]; }
					else if (comprobanteYCuenta.Key.Impuestos == null) { cuentaVenta = ccp.CuentasAuxiliares["VentasExentas"]; }

					//Crea el row de encabezado de CFDI
					IRow hRow = sheet.CreateRow(rowIndex);
					//Tipo Pol
					CreateCell(hRow, 0, "Dr", cellStyle);
					//Placeholder
					CreateCell(hRow, 1, 1, cellStyle);
					//Concepto póliza
					CreateCell(hRow, 2, conceptoString, cellStyle);
					//Día fecha
					CreateCell(hRow, 3, comprobanteYCuenta.Key.FechaNET.Day, cellStyle);

					//Crea el row del total de la factura
					IRow dRow = sheet.CreateRow(rowIndex + 1);
					//No. Cuenta
					CreateCell(dRow, 1, comprobanteYCuenta.Value?.Cuenta ?? "0000-000-000", cellStyle);
					//Depto.
					CreateCell(dRow, 2, 0, cellStyle);
					//Concepto
					CreateCell(dRow, 3, conceptoString, cellStyle);
					//Placeholder
					CreateCell(dRow, 4, string.Empty, cellStyle);
					//Total
					CreateCell(dRow, 5, (double)comprobanteYCuenta.Key.Total, cellStyle);

					//Crea el row del subtotal de la factura de CFDI
					IRow g1Row = sheet.CreateRow(rowIndex + 2);
					//No. Cuenta
					CreateCell(g1Row, 1, cuentaVenta?.Cuenta ?? "0000-000-000", cellStyle);
					//Depto.
					CreateCell(g1Row, 2, 0, cellStyle);
					//Concepto
					CreateCell(g1Row, 3, conceptoString, cellStyle);
					//Placeholder
					CreateCell(g1Row, 4, string.Empty, cellStyle);
					//Debe
					CreateCell(g1Row, 5, "", cellStyle);
					//Haber
					CreateCell(g1Row, 6, (double)comprobanteYCuenta.Key.SubTotal, cellStyle);


					//Crea el row del IVA
					IRow g2Row = sheet.CreateRow(rowIndex + 3);
					//No. Cuenta
					CreateCell(g2Row, 1, ccp.CuentasAuxiliares["IVANoCobrado"]?.Cuenta ?? "0000-000-000", cellStyle);
					//Depto.
					CreateCell(g2Row, 2, 0, cellStyle);
					//Concepto
					CreateCell(g2Row, 3, conceptoString, cellStyle);
					//Placeholder
					CreateCell(g1Row, 4, string.Empty, cellStyle);
					//Debe
					CreateCell(g2Row, 5, "", cellStyle);
					//Haber
					CreateCell(g2Row, 6, (double)(comprobanteYCuenta.Key.Impuestos?.TotalImpuestosTrasladados ?? 0), cellStyle);

					//Crea el row de fin de partida
					IRow fRow = sheet.CreateRow(rowIndex + 4);
					//Fin
					CreateCell(fRow, 1, "FIN_PARTIDAS", cellStyle);

					//Avanza 5 lineas para poder iniciar una nueva póliza.
					rowIndex += 5;

					comprobanteYCuenta.Key.Contabilizado = true;
				}

				//Crea el archivo excel y lo exporta al usuario.
				nombreArchivo = $"{Enum.GetName(typeof(TipoExportacion), TipoExportacion.PolizaIngresos)}_{DateTime.Now:yyyyMMddHHmmssfffffff}";
				using (var fileData = new FileStream($"wwwroot/templates/{nombreArchivo}.xlsx", FileMode.OpenOrCreate)){ wb.Write(fileData); }
				wb.Close();

				//Actualiza los comprobantes para que queden marcados con el flag "Contabilizado = true"
				await comprobanteManager.UpdateMultipleAsync([..ccp.ComprobantesYCuentas.Keys]);
			}

			return nombreArchivo;
		}

		private static Task<XSSFWorkbook> CreateWorkbookExcel() {
			XSSFWorkbook workbook = new();
			XSSFFont myFont = (XSSFFont)workbook.CreateFont();
			myFont.FontHeightInPoints = 11;
			myFont.FontName = "Tahoma";

			// Define un borde
			XSSFCellStyle borderedCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
			borderedCellStyle.SetFont(myFont);
			borderedCellStyle.BorderLeft = BorderStyle.Medium;
			borderedCellStyle.BorderTop = BorderStyle.Medium;
			borderedCellStyle.BorderRight = BorderStyle.Medium;
			borderedCellStyle.BorderBottom = BorderStyle.Medium;
			borderedCellStyle.VerticalAlignment = VerticalAlignment.Center;

			ISheet Sheet = workbook.CreateSheet("Prefacturas");
			//Creat The Headers of the excel
			IRow HeaderRow = Sheet.CreateRow(0);

			//Create The Actual Cells
			CreateCell(HeaderRow, 0, "Clave", borderedCellStyle);
			CreateCell(HeaderRow, 1, "Cliente", borderedCellStyle);
			CreateCell(HeaderRow, 2, "Fecha de elaboración", borderedCellStyle);
			CreateCell(HeaderRow, 3, "Su pedido", borderedCellStyle);
			CreateCell(HeaderRow, 4, "Clave del artículo", borderedCellStyle);
			CreateCell(HeaderRow, 5, "Cantidad", borderedCellStyle);
			CreateCell(HeaderRow, 6, "Precio", borderedCellStyle);
			CreateCell(HeaderRow, 7, "Desc. 1", borderedCellStyle);
			CreateCell(HeaderRow, 8, "Desc. 2", borderedCellStyle);
			CreateCell(HeaderRow, 9, "Desc. 3", borderedCellStyle);
			CreateCell(HeaderRow, 10, "Clave de vendedor", borderedCellStyle);
			CreateCell(HeaderRow, 11, "Comisión", borderedCellStyle);
			CreateCell(HeaderRow, 12, "Clave de esquema de impuestos", borderedCellStyle);
			CreateCell(HeaderRow, 13, "I.E.P.S.", borderedCellStyle);
			CreateCell(HeaderRow, 14, "Impuesto 2", borderedCellStyle);
			CreateCell(HeaderRow, 15, "Impuesto 3", borderedCellStyle);
			CreateCell(HeaderRow, 16, "I.V.A.", borderedCellStyle);
			CreateCell(HeaderRow, 17, "Impuesto 5", borderedCellStyle);
			CreateCell(HeaderRow, 18, "Impuesto 6", borderedCellStyle);
			CreateCell(HeaderRow, 19, "Impuesto 7", borderedCellStyle);
			CreateCell(HeaderRow, 20, "Impuesto 8", borderedCellStyle);
			CreateCell(HeaderRow, 21, "Método de pago", borderedCellStyle);
			CreateCell(HeaderRow, 22, "Forma de Pago SAT", borderedCellStyle);
			CreateCell(HeaderRow, 23, "Uso CFDI", borderedCellStyle);
			CreateCell(HeaderRow, 24, "Clave SAT", borderedCellStyle);
			CreateCell(HeaderRow, 25, "Unidad SAT", borderedCellStyle);
			CreateCell(HeaderRow, 26, "Observaciones", borderedCellStyle);
			CreateCell(HeaderRow, 27, "Observaciones de partida", borderedCellStyle);
			CreateCell(HeaderRow, 28, "Fecha de entrega", borderedCellStyle);
			CreateCell(HeaderRow, 29, "Fecha de vencimiento", borderedCellStyle);
			CreateCell(HeaderRow, 30, "Descripcion", borderedCellStyle);

			return Task.FromResult(workbook);
		}
		private static Task<XSSFWorkbook> CreateWorkbookIngresos()
		{
			XSSFWorkbook workbook = new();
			XSSFFont myFont = (XSSFFont)workbook.CreateFont();
			myFont.FontHeightInPoints = 11;
			myFont.FontName = "Calibri";
			myFont.IsItalic = true;

			// Define un borde
			XSSFCellStyle FirstHeaderCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
			FirstHeaderCellStyle.SetFont(myFont);
			FirstHeaderCellStyle.BorderLeft = BorderStyle.Medium;
			FirstHeaderCellStyle.BorderTop = BorderStyle.Medium;
			FirstHeaderCellStyle.BorderRight = BorderStyle.Medium;
			FirstHeaderCellStyle.BorderBottom = BorderStyle.Medium;
			FirstHeaderCellStyle.VerticalAlignment = VerticalAlignment.Center;
			FirstHeaderCellStyle.Alignment = HorizontalAlignment.Center;

			XSSFCellStyle SecondHeaderCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
			SecondHeaderCellStyle.CloneStyleFrom(FirstHeaderCellStyle);
			SecondHeaderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
			SecondHeaderCellStyle.FillPattern = FillPattern.SolidForeground;

			XSSFCellStyle ThirdHeaderCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
			ThirdHeaderCellStyle.CloneStyleFrom(FirstHeaderCellStyle);
			ThirdHeaderCellStyle.Alignment = HorizontalAlignment.Right;
			ThirdHeaderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
			ThirdHeaderCellStyle.FillPattern = FillPattern.SolidForeground;

			XSSFCellStyle FourthHeaderCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
			FourthHeaderCellStyle.CloneStyleFrom(FirstHeaderCellStyle);
			FourthHeaderCellStyle.Alignment = HorizontalAlignment.Left;
			FourthHeaderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
			FourthHeaderCellStyle.FillPattern = FillPattern.SolidForeground;

			FirstHeaderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
			FirstHeaderCellStyle.FillPattern = FillPattern.SolidForeground;

			ISheet Sheet = workbook.CreateSheet("Comprobantes");

			//Crea los encabezados de la primer linea
			IRow FirstHeaderRow = Sheet.CreateRow(0);
			CreateCell(FirstHeaderRow, 0, "TipoPol", FirstHeaderCellStyle);
			CreateCell(FirstHeaderRow, 1, "Concepto póliza", FirstHeaderCellStyle);
			CreateCell(FirstHeaderRow, 2, string.Empty, FirstHeaderCellStyle);
			CreateCell(FirstHeaderRow, 3, "Póliza dinámica CFDI:", ThirdHeaderCellStyle);
			CreateCell(FirstHeaderRow, 4, "Venta", FourthHeaderCellStyle);
			CreateCell(FirstHeaderRow, 5, string.Empty, FirstHeaderCellStyle);
			CreateCell(FirstHeaderRow, 6, string.Empty, FirstHeaderCellStyle);
			CellRangeAddress regionA = new(0, 0, 1, 2);
			Sheet.AddMergedRegion(regionA);
			CellRangeAddress regionB = new(0, 0, 4, 6);
			Sheet.AddMergedRegion(regionB);

			//Crea los encabezados de la segunda linea
			IRow SecondHeaderRow = Sheet.CreateRow(1);
			CreateCell(SecondHeaderRow, 1, "No. Cuenta", SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 2, "Depto.", SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 3, "Concepto mov.", SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 4, string.Empty, SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 5, "Debe", SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 6, "Haber", SecondHeaderCellStyle);
			CellRangeAddress regionC = new(1, 1, 3, 4);
			Sheet.AddMergedRegion(regionC);

			return Task.FromResult(workbook);
		}
		private static Task<XSSFWorkbook> CreateWorkbookEgresos()
		{
			XSSFWorkbook workbook = new();
			XSSFFont myFont = (XSSFFont)workbook.CreateFont();
			myFont.FontHeightInPoints = 11;
			myFont.FontName = "Calibri";
			myFont.IsItalic = true;

			// Define un borde
			XSSFCellStyle FirstHeaderCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
			FirstHeaderCellStyle.SetFont(myFont);
			FirstHeaderCellStyle.BorderLeft = BorderStyle.Medium;
			FirstHeaderCellStyle.BorderTop = BorderStyle.Medium;
			FirstHeaderCellStyle.BorderRight = BorderStyle.Medium;
			FirstHeaderCellStyle.BorderBottom = BorderStyle.Medium;
			FirstHeaderCellStyle.VerticalAlignment = VerticalAlignment.Center;
			FirstHeaderCellStyle.Alignment = HorizontalAlignment.Center;

			XSSFCellStyle SecondHeaderCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
			SecondHeaderCellStyle.CloneStyleFrom(FirstHeaderCellStyle);
			SecondHeaderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightGreen.Index;
			SecondHeaderCellStyle.FillPattern = FillPattern.SolidForeground;

			XSSFCellStyle ThirdHeaderCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
			ThirdHeaderCellStyle.CloneStyleFrom(FirstHeaderCellStyle);
			ThirdHeaderCellStyle.Alignment = HorizontalAlignment.Right;
			ThirdHeaderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
			ThirdHeaderCellStyle.FillPattern = FillPattern.SolidForeground;

			XSSFCellStyle FourthHeaderCellStyle = (XSSFCellStyle)workbook.CreateCellStyle();
			FourthHeaderCellStyle.CloneStyleFrom(FirstHeaderCellStyle);
			FourthHeaderCellStyle.Alignment = HorizontalAlignment.Left;
			FourthHeaderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
			FourthHeaderCellStyle.FillPattern = FillPattern.SolidForeground;

			FirstHeaderCellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.PaleBlue.Index;
			FirstHeaderCellStyle.FillPattern = FillPattern.SolidForeground;

			ISheet Sheet = workbook.CreateSheet("Comprobantes");

			//Crea los encabezados de la primer linea
			IRow FirstHeaderRow = Sheet.CreateRow(0);
			CreateCell(FirstHeaderRow, 0, "TipoPol", FirstHeaderCellStyle);
			CreateCell(FirstHeaderRow, 1, "Concepto póliza", FirstHeaderCellStyle);
			CreateCell(FirstHeaderRow, 2, string.Empty, FirstHeaderCellStyle);
			CreateCell(FirstHeaderRow, 3, "Póliza dinámica CFDI:", ThirdHeaderCellStyle);
			CreateCell(FirstHeaderRow, 4, "Provisión gastos en general", FourthHeaderCellStyle);
			CreateCell(FirstHeaderRow, 5, string.Empty, FirstHeaderCellStyle);
			CreateCell(FirstHeaderRow, 6, string.Empty, FirstHeaderCellStyle);
			CellRangeAddress regionA = new(0, 0, 1, 2);
			Sheet.AddMergedRegion(regionA);
			CellRangeAddress regionB = new(0, 0, 4, 6);
			Sheet.AddMergedRegion(regionB);

			//Crea los encabezados de la segunda linea
			IRow SecondHeaderRow = Sheet.CreateRow(1);
			CreateCell(SecondHeaderRow, 1, "No. Cuenta", SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 2, "Depto.", SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 3, "Concepto mov.", SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 4, string.Empty, SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 5, "Debe", SecondHeaderCellStyle);
			CreateCell(SecondHeaderRow, 6, "Haber", SecondHeaderCellStyle);
			CellRangeAddress regionC = new(1, 1, 3, 4);
			Sheet.AddMergedRegion(regionC);

			return Task.FromResult(workbook);
		}

		private Dictionary<string, CuentaContable> GetCuentasContablesEmisor(List<CuentaContable> cuentasContables)
		{
			CuentaContable? cuentaVentas16 = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 3).FirstOrDefault();
			CuentaContable? cuentaVentas0 = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 5).FirstOrDefault();
			CuentaContable? cuentaVentasExentas = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 6).FirstOrDefault();
			if (cuentaVentas16 == null || cuentaVentas0 == null || cuentaVentasExentas == null) { throw new Exception($"{localizer["SinCuentasContablesVentas"]} {localizer["PolizaNoCreada"]}"); }

			CuentaContable? cuentaIVANoCobrado = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 7).FirstOrDefault();
			CuentaContable? cuentaIVACobrado = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 8).FirstOrDefault();
			if (cuentaIVANoCobrado == null || cuentaIVACobrado == null) { throw new Exception($"{localizer["SinCuentasContablesIVAIngreso"]} {localizer["PolizaNoCreada"]}"); }

			return new Dictionary<string, CuentaContable>([
				new KeyValuePair<string, CuentaContable>("Ventas16", cuentaVentas16),
				new KeyValuePair<string, CuentaContable>("Ventas0", cuentaVentas0),
				new KeyValuePair<string, CuentaContable>("VentasExentas", cuentaVentasExentas),
				new KeyValuePair<string, CuentaContable>("IVACobrado", cuentaIVACobrado),
				new KeyValuePair<string, CuentaContable>("IVANoCobrado", cuentaIVANoCobrado)
			]);
		}
		private async Task<ComprobantesYCuentasPoliza> GetComprobantesYCuentasByIdForPolizaIngresos(string[] ids, string[]? cuentasClientesGuardables)
		{
			List<string> errores = [];
			List<string> invalidos = [];
			List<string> cancelados = [];
			List<string> contabilizados = [];
			List<string> noEncontrados = [];
			List<string> sinCuenta = [];

			Dictionary<Comprobante, CuentaContable> comprobantesYCuentas = [];
			Dictionary<string, CuentaContable> cuentasAuxiliares = [];

			Empresa? empresaEmisora = null;

			List<CuentaContable>? cuentasContables = [];

			foreach (string id in ids)
			{
				int intId = Convert.ToInt32(id);
				Comprobante? comprobante = await comprobanteManager.GetByIdAsync(intId);
				if (comprobante != null)
				{
					if (empresaEmisora == null) 
					{
						//Se obtiene la empresa emisora con el primer comprobante. Todos los comprobantes son del mismo emisor.
						empresaEmisora = await empresaManager.GetByRFCAsync(comprobante.Emisor?.Rfc ?? string.Empty);

						//Si el usuario asigno cuentas contables para los clientes que no tenían, entonces primero guarda las cuentas ligadas a los clientes.
						if (cuentasClientesGuardables != null && cuentasClientesGuardables.Length >= 1)
						{
							List<RFCCuenta>? elements = JsonConvert.DeserializeObject<List<RFCCuenta>>($"[{string.Join(",", cuentasClientesGuardables)}]") ?? [];
							foreach (RFCCuenta rc in elements) { await cuentaContableManager.CreateAsync(new() { Cuenta = rc.Cuenta, Nombre = rc.Nombre, RFC = rc.RFC, EmpresaId = empresaEmisora?.Id, TipoId = 2, SubtipoId = 1 }); }
						}

						//Se obtienen las cuentas contables del emisor
						cuentasContables = await cuentaContableManager.GetByIdEmpresaAsync(empresaEmisora?.Id ?? 0);
						cuentasContables = cuentasContables.Where(c => c.TipoId == 2).ToList();
						if (cuentasContables == null || cuentasContables.Count <= 0) { throw new Exception($"{localizer["SinCuentasContables"]} {localizer["PolizaNoCreada"]}"); }

						//Se obtienen las cuentas contables del emisor de los comprobantes.
						cuentasAuxiliares = GetCuentasContablesEmisor(cuentasContables);
					}

					if (comprobante.Contabilizado ?? false) { contabilizados.Add(id); }
					if (comprobante.Cancelado ?? false) { cancelados.Add(id); }
					if (comprobante.Valido == false && comprobante.Cancelado == false) { invalidos.Add(id); }

					comprobante.FechaNET = DateTime.ParseExact(comprobante.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

					CuentaContable? cuentaCliente = cuentasContables.Where(cuenta => cuenta.SubtipoId == 1 && cuenta.RFC == comprobante.Receptor?.Rfc).FirstOrDefault();
					if (cuentaCliente == null)
					{
						if (!sinCuenta.Contains(comprobante.Receptor?.Nombre ?? string.Empty)) { sinCuenta.Add(comprobante.Receptor?.Nombre ?? string.Empty); }
					}
					else
					{
						comprobantesYCuentas.Add(comprobante, cuentaCliente);
					}
				}
				else
				{
					noEncontrados.Add(id);
				}
			}

			if (contabilizados.Count >= 1) { errores.Add(localizer["ComprobantesContabilizados", [contabilizados.Count, string.Join(", ", contabilizados)]]); }

			if (cancelados.Count >= 1) { errores.Add(localizer["ComprobantesCancelados", [contabilizados.Count, string.Join(", ", contabilizados)]]); }

			if (invalidos.Count >= 1) { errores.Add(localizer["ComprobantesInvalidos", [contabilizados.Count, string.Join(", ", contabilizados)]]); }

			if (noEncontrados.Count >= 1) { errores.Add(localizer["ComprobantesNoEncontrados", [noEncontrados.Count, string.Join(", ", noEncontrados)]]); }

			if (errores.Count >= 1) { throw new Exception(localizer["ErrorBuscandoComprobantes", [$" {string.Join(" ", errores)}"]]); }


			if (sinCuenta.Count >= 1) { throw new SinCuentasException(localizer["ComprobantesSinCuentaContable", [sinCuenta.Count, string.Join(", ", sinCuenta)]]); }

			comprobantesYCuentas = comprobantesYCuentas.OrderBy(c => c.Key.FechaNET).ToDictionary();

			return new() { ComprobantesYCuentas = comprobantesYCuentas, CuentasAuxiliares = cuentasAuxiliares };
		}
		private static ISheet ConfigureFirstSheetPolizas(XSSFWorkbook wb)
		{
			ISheet sheet = wb.GetSheetAt(0);
			sheet.SetColumnWidth(1, 20 * 256);
			sheet.SetColumnWidth(2, 70 * 256);
			sheet.SetColumnWidth(3, 70 * 256);
			sheet.SetColumnWidth(4, 10 * 256);
			sheet.SetColumnWidth(5, 10 * 256);

			return sheet;
		}
		private static XSSFCellStyle CreateCellStylePolizas(XSSFWorkbook wb)
		{
			XSSFCellStyle cellStyle = (XSSFCellStyle)wb.CreateCellStyle();
			XSSFFont myFont = (XSSFFont)wb.CreateFont();
			myFont.FontHeightInPoints = 11;
			myFont.FontName = "Calibri";
			cellStyle.SetFont(myFont);

			return cellStyle;
		}

		private static void CreateCell(IRow CurrentRow, int CellIndex, string Value, XSSFCellStyle Style)
		{
			ICell Cell = CurrentRow.CreateCell(CellIndex);
			Cell.SetCellValue(Value);
			Cell.CellStyle = Style;
		}
		private static void CreateCell(IRow CurrentRow, int CellIndex, int Value, XSSFCellStyle Style)
		{
			ICell Cell = CurrentRow.CreateCell(CellIndex);
			Cell.SetCellValue(Value);
			Cell.CellStyle = Style;
		}
		private static void CreateCell(IRow CurrentRow, int CellIndex, double Value, XSSFCellStyle Style)
		{
			ICell Cell = CurrentRow.CreateCell(CellIndex);
			Cell.SetCellValue(Value);
			Cell.CellStyle = Style;
		}

		public ActionResult OnGetDownloadExcel(string nombreArchivo)
		{
			try
			{
				byte[] bytes;
				FileStream fs = System.IO.File.OpenRead($"wwwroot/templates/{nombreArchivo}.xlsx");
				bytes = new byte[fs.Length];
				fs.Read(bytes, 0, bytes.Length);
				fs.Close();
				fs.Dispose();

				System.IO.File.Delete($"wwwroot/templates/{nombreArchivo}.xlsx");

				return File(bytes, MediaTypeNames.Application.Octet, $"{nombreArchivo}.xlsx");
			}
			catch (Exception ex)
			{
				logger.LogError("{message}", ex.Message);
			}

			return new EmptyResult();
		}
		
		public async Task<JsonResult> OnPostCancelarComprobante(string[] ids)
		{
			ServerResponse resp = new(true, localizer["ComprobantesCancelledUnsuccessfully"]);

			if (PuedeTodo || PuedeEliminar)
			{
				try
				{
					await db.Database.BeginTransactionAsync();

					foreach (string id in ids)
					{
						_ = int.TryParse(id, out int idComprobante);

						//Se timbra la prefactura
						ServerResponse respCancelacion = new(true, localizer["ComprobanteCancelledUnsuccessfully"] + $" {idComprobante}");
						if (idComprobante >= 1){ 
							respCancelacion = await CancelarComprobante(idComprobante);
							if (respCancelacion.TieneError) {
								//TODO: Refactorizar método para que notifique errores por prefactura.
								resp.Errores.AddRange(respCancelacion.Errores);
							}
						}
					}

					await db.Database.CommitTransactionAsync();

					resp.TieneError = false;
					resp.Mensaje = localizer["ComprobantesCancelledSuccessfully"];
				}
				catch (Exception ex)
				{
					string message = ex.Message;
					logger.LogError("{message}", message);
					resp.Mensaje = message;
					await db.Database.RollbackTransactionAsync();
				}
			}
			else
			{
				resp.Mensaje = localizer["AccesoDenegado"];
			}

			return new JsonResult(resp);
		}
		private async Task<ServerResponse> CancelarComprobante(int idComprobante)
		{
			ServerResponse resp = new(true, localizer["ComprobanteCancelledUnsuccessfully"]);
			try
			{
				//Obtiene los datos de la prefactura
				Comprobante? c = await comprobanteManager.GetByIdWithDescripcionesAsync(idComprobante);

				if (c != null)
				{
					//TODO: Proceso de cancelación de comprobantes

					//Devuelve mensaje correcto de timbrado.
					resp.TieneError = false;
					resp.Errores = [];
					resp.Mensaje = localizer["ComprobanteCancelledSuccessfully"];
				}
			}
			catch (Exception ex)
			{
				//Devuelve el error en el timbrado.
				resp.Errores = [..resp.Errores.Append(ex.Message)];
			}

			return resp;
		}

		public async Task<JsonResult> OnPostValidarComprobantes(string[] ids)
		{
			ServerResponse resp = new(true, localizer["ComprobantesValidatedUnsuccessfully"]);
			List<Comprobante> comprobantes = [];
			if (PuedeTodo || PuedeConsultar)
			{
				try
				{
					ConsultaCFDIServiceClient wsConsultaEstatus = new();
					foreach (string id in ids)
					{
						_ = int.TryParse(id, out int idComprobante);

						//Se timbra la prefactura
						ServerResponse respValidacion = new(true, localizer["ComprobanteValidatedUnsuccessfully"] + $" {idComprobante}");
						if (idComprobante >= 1)
						{
							//Obtiene los datos del comprobante
							Comprobante? c = await comprobanteManager.GetValidatableComprobanteByIdAsync(idComprobante);

							if (c != null){ 
								respValidacion = await ValidarComprobante(c, wsConsultaEstatus);
								if (!respValidacion.TieneError) { comprobantes.Add(c); }
							}

							if (respValidacion.TieneError){ resp.Errores.AddRange(respValidacion.Errores); }
						}
					}

					resp.Datos = CreateJsonComprobantes(comprobantes);
					resp.TieneError = false;
					resp.Mensaje = localizer["ComprobantesValidatedSuccessfully"];
				}
				catch (Exception ex)
				{
					string message = ex.Message;
					logger.LogError("{message}", message);
					resp.Mensaje = message;
				}
			}
			else
			{
				resp.Mensaje = localizer["AccesoDenegado"];
			}

			return new JsonResult(resp);
		}
		private async Task<ServerResponse> ValidarComprobante(Comprobante comprobante, ConsultaCFDIServiceClient wsConsultaEstatus)
		{
			ServerResponse resp = new(true, localizer["ComprobanteValidatedUnsuccessfully"]);
			try
			{
				string peticion = $"?re={comprobante.Emisor?.Rfc?.ToUpper()}&rr={comprobante.Receptor?.Rfc?.ToUpper()}&tt={comprobante.Total}&id={comprobante.Complemento?.TimbreFiscalDigital?.UUID?.ToUpper()}&fe={comprobante.Sello}";
				Acuse a = await wsConsultaEstatus.ConsultaAsync(peticion);
				if(a.CodigoEstatus.Contains("Comprobante obtenido satisfactoriamente", StringComparison.InvariantCultureIgnoreCase))
				{
					comprobante.Cancelado = a.Estado == "Cancelado";
					comprobante.Valido = a.Estado == "Vigente";

					//Actualiza su estatus en base de datos.
					await comprobanteManager.UpdateAsync(comprobante);

					//Devuelve mensaje correcto de validación.
					resp.TieneError = false;
					resp.Errores = [];
					resp.Mensaje = localizer["ComprobanteValidatedSuccessfully"];
				}
				else
				{
					resp.Mensaje = a.CodigoEstatus;
				}
			}
			catch (Exception ex)
			{
				//Devuelve el error en el timbrado.
				resp.Errores = [.. resp.Errores.Append(ex.Message)];
			}

			return resp;
		}

		public async Task<JsonResult> OnPostComprobantesWithConceptos(string[] ids)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			if (PuedeTodo || PuedeConsultar)
			{
				try
				{
					resp.Datos = await CreateJsonComprobantesWithConceptos(ids);
					resp.TieneError = false;
					resp.Mensaje = localizer["ConsultadoSuccessfully"];
				}
				catch (Exception ex)
				{
					logger.LogError("{message}", ex.Message);
					resp.Mensaje = ex.Message;
				}
			}
			else
			{
				resp.Mensaje = localizer["AccesoDenegado"];
			}

			return new JsonResult(resp);
		}
		private async Task<string> CreateJsonComprobantesWithConceptos(string[] ids)
		{
			List<string> jsonComprobantes = [];
			string jsonResponse;
			string jsonConceptos;

			foreach (string id in ids)
			{
				_ = int.TryParse(id, out int idComprobante);

				//Se obtiene el comprobante con sus conceptos
				if (idComprobante >= 1)
				{
					//Obtiene los datos del comprobante
					Comprobante? c = await comprobanteManager.GetWithConceptosByIdAsync(idComprobante);

					if (c != null)
					{
						jsonConceptos = CreateJsonConceptos(c.Conceptos ?? []);
						string serieFolio = $"{c.Serie ?? "F"}{c.Folio ?? "0"}";
						jsonComprobantes.Add(
							"{" +
								$"\"id\": {c.Id}," +
								$"\"serieFolio\": \"{serieFolio}\", " +
								$"\"total\": {c.Total}, " +
								$"\"rfcEmisor\": \"{c.Emisor?.Rfc}\", " +
								$"\"razonSocialEmisor\": \"{c.Emisor?.Rfc} - {c.Emisor?.Nombre}\", " +
								$"\"rfcReceptor\": \"{c.Receptor?.Rfc}\", " +
								$"\"razonSocialReceptor\": \"{c.Receptor?.Nombre}\", " +
								$"\"conceptos\": {jsonConceptos}" +
							"}"
						);
					}
				}
			}

			jsonResponse = $"[{string.Join(",", jsonComprobantes)}]";

			return jsonResponse;
		}
		private static string CreateJsonConceptos(List<ComprobanteConcepto> conceptos)
		{
			List<string> jsonConceptos = [];
			string jsonResponse;
			foreach(ComprobanteConcepto cc in conceptos)
			{
				cc.Descripcion = JsonEscape(cc.Descripcion);

				jsonConceptos.Add(
					"{" +
						$"\"id\": {cc.Id}," +
						$"\"claveProdServ\":\"{cc.ClaveProdServ}\"," +
						$"\"descripcion\":\"{cc.Descripcion}\"," +
						$"\"importe\": {cc.Importe}" +
					"}"
				);
			}

			jsonResponse = $"[{string.Join(",", jsonConceptos)}]";

			return jsonResponse;

		}

		public async Task<JsonResult> OnPostComprobantesWithReceptores(string[] ids)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			if (PuedeTodo || PuedeConsultar)
			{
				try
				{
					resp.Datos = await CreateJsonComprobantesWithReceptores(ids);
					resp.TieneError = false;
					resp.Mensaje = localizer["ConsultadoSuccessfully"];
				}
				catch (Exception ex)
				{
					logger.LogError("{message}", ex.Message);
					resp.Mensaje = ex.Message;
				}
			}
			else
			{
				resp.Mensaje = localizer["AccesoDenegado"];
			}

			return new JsonResult(resp);
		}
		private async Task<string> CreateJsonComprobantesWithReceptores(string[] ids)
		{
			List<string> jsonReceptores = [];
			List<string> receptores = [];

			string jsonResponse;

			foreach (string id in ids)
			{
				_ = int.TryParse(id, out int idComprobante);

				//Se obtiene el comprobante con sus conceptos
				if (idComprobante >= 1)
				{
					//Obtiene los datos del comprobante
					Comprobante? c = await comprobanteManager.GetWithReceptorByIdAsync(idComprobante);

					if (c != null && !string.IsNullOrEmpty(c.Receptor?.Rfc) && !receptores.Contains(c.Receptor.Rfc))
					{
						receptores.Add(c.Receptor.Rfc);
						jsonReceptores.Add(
							"{" +
								$"\"id\": {c.Receptor.Id}," +
								$"\"rfcReceptor\": \"{c.Receptor.Rfc}\", " +
								$"\"razonSocialReceptor\": \"{c.Receptor.Nombre}\", " +
								$"\"receptor\": \"{c.Receptor.Rfc} - {c.Receptor.Nombre}\"" +
							"}"
						);
					}
				}
			}

			jsonResponse = $"[{string.Join(",", jsonReceptores)}]";

			return jsonResponse;
		}
	}
}
