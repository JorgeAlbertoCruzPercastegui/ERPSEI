using ERPSEI.Data;
using ERPSEI.Data.Entities.Cuentas;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Entities.Usuarios;
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
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using NuGet.Packaging;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using WS_SAT_ConsultaEstatusCFDI;

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
			IStringLocalizer<AdministradorDeComprobantesModel> localizer,
			ILogger<AdministradorDeComprobantesModel> logger,
			IEncriptacionAES encriptacionAES
		) : ERPPageModel
	{

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
						$"\"serie\": \"{c.Serie}\", " +
						$"\"folio\": \"{c.Folio}\", " +
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

			if (filtro != null)
			{
				comprobantes = await comprobanteManager.GetAllAsync(
					filtro.EmpresaRFC,
					filtro.Anio,
					filtro.Mes,
					filtro.EstatusId,
					filtro.TipoId,
					filtro.EstatusContableId,
					filtro.TipoComprobanteClave,
					filtro.FormaPagoClave,
					filtro.MetodoPagoClave,
					filtro.UsoCFDIClave,
					filtro.EmisorRFC,
					filtro.ReceptorRFC
				);
			}
			else
			{
				comprobantes = await comprobanteManager.GetAllAsync();
			}

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

		private static string JsonEscape(string str)
		{
			return str.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\"", "\\\"");
		}

		public async Task<ActionResult> OnPostExportCFDIS(string[] ids, int tipoExportado)
		{
			ServerResponse resp = new(true, localizer["ComprobantesExportedUnsuccessfully"]);
			if (PuedeTodo || PuedeConsultar)
			{
				try
				{
					switch (tipoExportado)
					{
						case (int)TipoExportacion.PDF:
							break;
						case (int)TipoExportacion.XML:
							break;
						case (int)TipoExportacion.Excel:
							break;
						case (int)TipoExportacion.PolizaIngresos:
							resp.Datos = await CreateWorkbook(ids, TipoExportacion.PolizaIngresos, comprobanteManager, cuentaContableManager, empresaManager);
							break;
						case (int)TipoExportacion.PolizaEgresos:
							break;
						default:
							break;
					}

					
					resp.TieneError = false;
					resp.Mensaje = localizer["ComprobantesExportedSuccessfully"];
				}
				catch (Exception ex)
				{
					logger.LogError(message: ex.Message);
					resp.Mensaje = ex.Message;
				}
			}
			else
			{
				resp.Mensaje = localizer["AccesoDenegado"];
			}

			return new JsonResult(resp);
		}

		private async static Task<string> CreateWorkbook(string[] ids, TipoExportacion tipoExportacion, IComprobanteManager cmgr, ICuentaContableManager ccmgr, IEmpresaManager emgr)
		{
			int rowIndex = 2;
			string? strTipoPoliza = string.Empty;
			XSSFWorkbook? wb = null;
			string? nombreArchivo = string.Empty;
			switch (tipoExportacion)
			{
				case TipoExportacion.Excel:
					break;
				case TipoExportacion.PolizaIngresos:
					strTipoPoliza = "VENTA";
					wb = await CreateExcelPolizaIngresos();
					break;
				case TipoExportacion.PolizaEgresos:
					strTipoPoliza = "GASTO";
					//wb = await CreateExcelPolizaEgresos();
					break;
				default:
					break;
			}
			if (wb == null) { throw new Exception("No workbook created"); }
			using (wb)
			{
				//Obtiene la primer hoja del archivo
				ISheet sheet = wb.GetSheetAt(0);
				sheet.SetColumnWidth(1, 20 * 256);
				sheet.SetColumnWidth(2, 70 * 256);
				sheet.SetColumnWidth(3, 70 * 256);
				sheet.SetColumnWidth(4, 10 * 256);
				sheet.SetColumnWidth(5, 10 * 256);

				//Crea el estilo de las celdas.
				XSSFCellStyle cellStyle = (XSSFCellStyle)wb.CreateCellStyle();
				XSSFFont myFont = (XSSFFont)wb.CreateFont();
				myFont.FontHeightInPoints = 11;
				myFont.FontName = "Calibri";
				cellStyle.SetFont(myFont);

				Empresa? empresaEmisora = null;
				List<Comprobante> comprobantes = [];
                foreach (string id in ids)
                {
					int intId = Convert.ToInt32(id);
					Comprobante? comprobante = await cmgr.GetByIdAsync(intId);
					if (comprobante != null) 
					{
						empresaEmisora ??= await emgr.GetByRFCAsync(comprobante.Emisor?.Rfc ?? string.Empty);
						comprobante.FechaNET = DateTime.ParseExact(comprobante.Fecha ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm:ss"), "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
						comprobantes.Add(comprobante);
					}
				}

				comprobantes = [..comprobantes.OrderBy(c => c.FechaNET)];

				List<CuentaContable>? cuentasContables = await ccmgr.GetByIdEmpresaAsync(empresaEmisora?.Id ?? 0);
				switch (tipoExportacion)
				{
					case TipoExportacion.Excel:
						break;
					case TipoExportacion.PolizaIngresos:
						cuentasContables = cuentasContables.Where(c => c.TipoId == 2).ToList();
						break;
					case TipoExportacion.PolizaEgresos:
						cuentasContables = cuentasContables.Where(c => c.TipoId == 1).ToList();
						break;
					default:
						break;
				}
				CuentaContable? cuentaVentas16 = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 3).FirstOrDefault();
				CuentaContable? cuentaVentas0 = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 5).FirstOrDefault();
				CuentaContable? cuentaVentasExentas = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 6).FirstOrDefault();
				CuentaContable? cuentaIVANoCobrado = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 7).FirstOrDefault();
				CuentaContable? cuentaIVACobrado = cuentasContables.Where(cuenta => cuenta.TipoId == 2 && cuenta.SubtipoId == 8).FirstOrDefault();
				CuentaContable? cuentaCliente = null;
				CuentaContable? cuentaVenta = null;

				string conceptoString = string.Empty;
				foreach (Comprobante comprobante in comprobantes)
				{
					conceptoString = $"PROVISION DE {strTipoPoliza} '{comprobante.Receptor?.Nombre}' {comprobante.Serie ?? "F"}-{comprobante.Folio}";
					cuentaCliente = cuentasContables.Where(cuenta => cuenta.RFC == comprobante.Receptor?.Rfc).FirstOrDefault();
					if (comprobante.Impuestos != null && (comprobante.Impuestos.Traslados?.Any(t => t.TasaOCuota == 0.16m) ?? false)) { cuentaVenta = cuentaVentas16; }
					else if (comprobante.Impuestos != null && (comprobante.Impuestos.Traslados?.Any(t => t.TasaOCuota == 0.0m) ?? false)) { cuentaVenta = cuentaVentas0; }
					else if (comprobante.Impuestos == null) { cuentaVenta = cuentaVentasExentas; }

					//Crea el row de encabezado de CFDI
					IRow hRow = sheet.CreateRow(rowIndex);
					//Tipo Pol
					CreateCell(hRow, 0, "Dr", cellStyle);
					//Placeholder
					CreateCell(hRow, 1, 1, cellStyle);
					//Concepto póliza
					CreateCell(hRow, 2, conceptoString, cellStyle);
					//Día fecha
					CreateCell(hRow, 3, comprobante.FechaNET.Day, cellStyle);

					//Crea el row del total de la factura
					IRow dRow = sheet.CreateRow(rowIndex + 1);
					//No. Cuenta
					CreateCell(dRow, 1, cuentaCliente?.Cuenta ?? "0000-000-000", cellStyle);
					//Depto.
					CreateCell(dRow, 2, 0, cellStyle);
					//Concepto
					CreateCell(dRow, 3, conceptoString, cellStyle);
					//Placeholder
					CreateCell(dRow, 4, string.Empty, cellStyle);
					//Total
					CreateCell(dRow, 5, (double)comprobante.Total, cellStyle);

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
					if (tipoExportacion == TipoExportacion.PolizaIngresos)
					{
						//Debe
						CreateCell(g1Row, 5, "", cellStyle);
						//Haber
						CreateCell(g1Row, 6, (double)comprobante.SubTotal, cellStyle);
					}
					else
					{
						//Debe
						CreateCell(g1Row, 5, (double)comprobante.SubTotal, cellStyle);
						//Haber
						CreateCell(g1Row, 6, "", cellStyle);
					}

					//Crea el row del IVA
					IRow g2Row = sheet.CreateRow(rowIndex + 3);
					//No. Cuenta
					CreateCell(g2Row, 1, cuentaIVANoCobrado?.Cuenta ?? "0000-000-000", cellStyle);
					//Depto.
					CreateCell(g2Row, 2, 0, cellStyle);
					//Concepto
					CreateCell(g2Row, 3, conceptoString, cellStyle);
					//Placeholder
					CreateCell(g1Row, 4, string.Empty, cellStyle);
					if (tipoExportacion == TipoExportacion.PolizaIngresos)
					{
						//Debe
						CreateCell(g2Row, 5, "", cellStyle);
						//Haber
						CreateCell(g2Row, 6, (double)(comprobante.Impuestos?.TotalImpuestosTrasladados ?? 0), cellStyle);
					}
					else
					{
						//Debe
						CreateCell(g2Row, 5, (double)(comprobante.Impuestos?.TotalImpuestosTrasladados ?? 0), cellStyle);
						//Haber
						CreateCell(g2Row, 6, "", cellStyle);
					}

					//Crea el row de fin de partida
					IRow fRow = sheet.CreateRow(rowIndex + 4);
					//Fin
					CreateCell(fRow, 1, "FIN_PARTIDAS", cellStyle);

					//Avanza 5 lineas para poder iniciar una nueva póliza.
					rowIndex += 5;

					comprobante.Contabilizado = true;
				}

				//Crea el archivo excel y lo exporta al usuario.
				nombreArchivo = $"{Enum.GetName(typeof(TipoExportacion), tipoExportacion)}_{DateTime.Now:yyyyMMddHHmmssfffffff}";
				using (var fileData = new FileStream($"wwwroot/templates/{nombreArchivo}.xlsx", FileMode.OpenOrCreate)){ wb.Write(fileData); }
				wb.Close();

				//Actualiza los comprobantes para que queden marcados con el flag "Contabilizado = true"
				await cmgr.UpdateMultipleAsync(comprobantes);
			}

			return nombreArchivo;
		}
		private static Task<XSSFWorkbook> CreateExcel()
		{
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
		private static Task<XSSFWorkbook> CreateExcelPolizaIngresos()
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

			if (PuedeTodo || PuedeEditar || PuedeEliminar)
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
				Comprobante? c = await comprobanteManager.GetByIdAsync(idComprobante);

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
			if (PuedeTodo || PuedeEditar)
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
	}
}
