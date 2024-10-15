using ERPSEI.Data;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.SAT;
using ERPSEI.Data.Entities.SAT.Catalogos;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.SAT.Catalogos;
using ERPSEI.Data.Managers.SAT.cfdiv40;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Pages.Shared;
using ERPSEI.Requests;
using ERPSEI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NuGet.Packaging;
using ServicioEDICOM;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace ERPSEI.Areas.ERP.Pages
{
	[Authorize(Policy = "AccessPolicy")]
	public class AdministradorDeComprobantesModel(
			CFDi clienteEDICOM,
			ApplicationDbContext db,
			AppUserManager userManager,
			IRWCatalogoManager<Perfil> perfilManager,
			IEmpresaManager empresaManager,
			IComprobanteManager comprobanteManager,
			ITasaOCuotaManager tasaOCuotaManager,
			IStringLocalizer<PrefacturasModel> localizer,
			ILogger<PrefacturasModel> logger,
			IEncriptacionAES encriptacionAES
		) : ERPPageModel
	{

		[BindProperty]
		public FiltroModel InputFiltro { get; set; } = new FiltroModel();

		public class FiltroModel
		{
			[DataType(DataType.Text)]
			[Display(Name = "PeriodoField")]
			public string? Periodo { get; set; }

			[Display(Name = "EstatusField")]
			public int? EstatusId { get; set; }

			[Display(Name = "TipoField")]
			public int? TipoId { get; set; }

			[Display(Name = "FormaPagoField")]
			public int? FormaPagoId { get; set; }

			[Display(Name = "MetodoPagoField")]
			public int? MetodoPagoId { get; set; }

			[Display(Name = "UsoCFDIField")]
			public int? UsoCFDIId { get; set; }

			[Display(Name = "EmisorField")]
			public int? EmisorId { get; set; }

			[Display(Name = "ReceptorField")]
			public int? ReceptorId { get; set; }
		}

		public void OnGet()
		{
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
			List<string> jsonComprobantes = [];
			List<Comprobante> comprobantes = [];

			if (filtro != null)
			{
				comprobantes = await comprobanteManager.GetAllAsync(
					filtro.Periodo,
					filtro.EstatusId,
					filtro.TipoId,
					filtro.FormaPagoId,
					filtro.MetodoPagoId,
					filtro.UsoCFDIId,
					filtro.EmisorId,
					filtro.ReceptorId
				);
			}
			else
			{
				comprobantes = await comprobanteManager.GetAllAsync();
			}

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
						$"\"emisor\": \"{c.Emisor?.Nombre}\", " +
						$"\"emisorId\": \"{c.Emisor?.Id}\", " +
						$"\"receptor\": \"{c.Receptor?.Nombre}\", " +
						$"\"receptorId\": \"{c.Receptor?.Id}\", " +
						$"\"tipoComprobante\": \"{c.TipoDeComprobante}\", " +
						$"\"fecha\": \"{fecha:dd/MM/yyyy HH:mm:ss}\", " +
						$"\"fechaJS\": \"{fecha:yyyy-MM-dd HH:mm:ss}\", " +
						$"\"tipoCambio\": {c.TipoCambio}, " +
						$"\"moneda\": \"{c.Moneda}\", " +
						$"\"formaPago\": \"{c.FormaPago}\", " +
						$"\"metodoPago\": \"{c.MetodoPago}\", " +
						$"\"usoCFDI\": \"{c.Receptor?.UsoCFDI}\", " +
						$"\"exportacion\": \"{c.Exportacion}\", " +
						$"\"estatus\": \"\"" +
					"}"
				);
			}

			jsonResponse = $"[{string.Join(",", jsonComprobantes)}]";

			return jsonResponse;
		}

		public async Task<JsonResult> OnPostGetEmpresaSuggestion(string texto, string idempresa)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					if (!int.TryParse(idempresa, out int idEmp)) { idEmp = 0; }

					resp.Datos = await GetEmpresasSuggestion(texto, idEmp);
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
		private async Task<string> GetEmpresasSuggestion(string texto, int idempresa)
		{
			string jsonResponse;
			List<string> jsonEmpresas = [];

			List<EmpresaBuscada> empresas = await empresaManager.SearchEmpresas(texto);

			if (empresas != null)
			{
				foreach (EmpresaBuscada e in empresas)
				{
					Empresa? emp = await empresaManager.GetByIdWithAdicionalesAsync(e.Id);
					List<Perfil> perfiles = await perfilManager.GetAllAsync();
					perfiles = perfiles.Where(p => p.Id == emp?.PerfilId).ToList();
					Perfil? perfilEmpresa = perfiles != null && perfiles.Count >= 1 ? perfiles.First() : null;

					List<ProductoServicioPerfil> prodServEmpresa = [];
					if (perfilEmpresa != null) { prodServEmpresa = [.. perfilEmpresa.ProductosServiciosPerfil]; }

					//Si viene establecido el id empresa, omite el elemento con ese id.
					if (idempresa >= 1 && e.Id == idempresa) { continue; }

					e.ObjetoSocial = JsonEscape(e.ObjetoSocial ?? string.Empty);
					List<Comprobante> comprobantes = await comprobanteManager.GetAllAsync();
					comprobantes = [.. comprobantes.OrderByDescending(p => p.Id)];

					jsonEmpresas.Add($"{{" +
										$"\"id\": {e.Id}, " +
										$"\"value\": \"{e.RazonSocial}\", " +
										$"\"label\": \"{e.RFC} - {e.RazonSocial}\", " +
										$"\"rfc\": \"{e.RFC}\", " +
										$"\"razonSocial\": \"{e.RazonSocial}\", " +
										$"\"objetoSocial\": \"{e.ObjetoSocial}\", " +
										$"\"origen\": \"{e.Origen}\", " +
										$"\"nivel\": {{" +
														$"\"nombre\": \"{e.Nivel}\", " +
														$"\"ordinal\": \"{e.Ordinal}\", " +
														$"\"puedeFacturar\": \"{e.PuedeFacturar}\"" +
													$"}}, " +
										$"\"perfil\": \"{e.Perfil}\", " +
										$"\"domicilioFiscal\": \"{e.DomicilioFiscal}\"" +
									$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsonEmpresas)}]";

			return jsonResponse;
		}
		private static string JsonEscape(string str)
		{
			return str.Replace("\n", "<br />").Replace("\r", "<br />").Replace("\t", "<br />");
		}

		public async Task<JsonResult> OnPostExportExcel(string[] ids)
		{
			ServerResponse resp = new(true, localizer["ComprobantesExportedUnsuccessfully"]);

			if (PuedeTodo || PuedeConsultar)
			{
				try
				{
					await db.Database.BeginTransactionAsync();

						//El llenado de datos comienza en la fila 1 del archivo ya que la fila 0 es el encabezado que se crea junto con el excel.
						int rowIndex = 1;
						List<TasaOCuota> impuestos = await tasaOCuotaManager.GetAllAsync();
						List<TasaOCuota> impuestosIEPS = impuestos.Where(t => t.ImpuestoId == 3).ToList();
						List<TasaOCuota> impuestosIVA = impuestos.Where(t => t.ImpuestoId == 2).ToList();

						//Crea el archivo Excel
						using (HSSFWorkbook wb = await CreateExcel())
						{
							//Obtiene la primer hoja del archivo
							ISheet sheet = wb.GetSheetAt(0);
							//Crea el estilo de las celdas.
							HSSFCellStyle cellStyle = (HSSFCellStyle)wb.CreateCellStyle();

							foreach (string id in ids)
							{
								int intId = Convert.ToInt32(id);
								Comprobante? p = await comprobanteManager.GetByIdAsync(intId);
								if (p != null)
								{
									//TODO: Crear los rows del excel en base a cada concepto.
								}
							}

							//Crea el archivo excel y lo exporta al usuario.
							using (var fileData = new FileStream("wwwroot/templates/Comprobantes.xls", FileMode.OpenOrCreate))
							{
								wb.Write(fileData);
							}

							wb.Close();
						}

						await db.Database.CommitTransactionAsync();

						resp.TieneError = false;
						resp.Mensaje = localizer["ComprobantesExportedSuccessfully"];
				}
				catch (Exception ex)
				{
					logger.LogError(message: ex.Message);
					resp.Mensaje = ex.Message;
					await db.Database.RollbackTransactionAsync();
				}
			}
			else
			{
				resp.Mensaje = localizer["AccesoDenegado"];
			}

			return new JsonResult(resp);
		}
		private static Task<HSSFWorkbook> CreateExcel()
		{
			HSSFWorkbook workbook = new();
			HSSFFont myFont = (HSSFFont)workbook.CreateFont();
			myFont.FontHeightInPoints = 11;
			myFont.FontName = "Tahoma";

			// Define un borde
			HSSFCellStyle borderedCellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
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
		private static void CreateCell(IRow CurrentRow, int CellIndex, string Value, HSSFCellStyle Style)
		{
			ICell Cell = CurrentRow.CreateCell(CellIndex);
			Cell.SetCellValue(Value);
			Cell.CellStyle = Style;
		}
		private static decimal GetIEPSConcepto(Concepto c, List<TasaOCuota> impuestos)
		{
			decimal total = 0;
			if (c.ObjetoImpuestoId >= 2)
			{
				//Obtiene el valor total del IEPS
				foreach (TasaOCuota t in impuestos)
				{
					if (c.TasaTraslado == (decimal)t.ValorMaximo)
					{
						total += c.Traslado;
						break;
					}
					else if (c.TasaRetencion == (decimal)t.ValorMaximo)
					{
						total += c.Retencion;
						break;
					}
				}
			}
			return total;
		}
		private static decimal GetIVAConcepto(Concepto c, List<TasaOCuota> impuestos)
		{
			decimal total = 0;
			if (c.ObjetoImpuestoId >= 2)
			{
				//Obtiene el valor total del IVA
				foreach (TasaOCuota t in impuestos)
				{
					if (c.TasaTraslado == (decimal)t.ValorMaximo)
					{
						total += c.Traslado;
						break;
					}
					else if (c.TasaRetencion == (decimal)t.ValorMaximo)
					{
						total += c.Retencion;
						break;
					}
				}
			}
			return total;
		}

		public ActionResult OnGetDownloadExcel()
		{
			if (PuedeTodo || PuedeConsultar)
			{
				return File("/templates/Prefacturas.xls", MediaTypeNames.Application.Octet, "Prefacturas.xls");
			}
			else
			{
				return new EmptyResult();
			}
		}
		
		public async Task<JsonResult> OnPostCancelarMultiple(string[] ids)
		{
			ServerResponse resp = new(true, localizer["ComprobantesCancelledUnsuccessfully"]);

			if (PuedeTodo || PuedeEditar)
			{
				try
				{
					await db.Database.BeginTransactionAsync();

					foreach (string id in ids)
					{
						_ = int.TryParse(id, out int idComprobante);

						//Se timbra la prefactura
						ServerResponse respCancelacion = new(true, localizer["FailedToCancel"] + $" {idComprobante}");
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
		public async Task<JsonResult> OnPostCancelar(int idComprobante)
		{
			ServerResponse resp = new(true, localizer["ComprobanteCancelledUnsuccessfully"]);
			try
			{
				if(PuedeTodo || PuedeEditar)
				{
					//Se timbra la prefactura
					resp = await CancelarComprobante(idComprobante);
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
	}
}
