using ERPSEI.Data;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.SAT;
using ERPSEI.Data.Entities.SAT.Catalogos;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.SAT;
using ERPSEI.Data.Managers.SAT.Catalogos;
using ERPSEI.Pages.Shared;
using ERPSEI.Requests;
using ERPSEI.Resources;
using ERPSEI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NuGet.Packaging;
using ServicioEDICOM;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using XSDToXML.Utils;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace ERPSEI.Areas.ERP.Pages
{
	[Authorize(Policy = "AccessPolicy")]
	public class PrefacturasModel(
			CFDi clienteEDICOM,
			ApplicationDbContext db,
			IAutorizacionesPrefactura autorizacionesPrefacturaManager,
			AppUserManager userManager,
			IPerfilManager perfilManager,
			IUnidadMedidaManager unidadMedidaManager,
			IProductoServicioManager productosServiciosManager,
			IEmpresaManager empresaManager,
			IPrefacturaManager prefacturaManager,
			IConceptoManager conceptoManager,
			ITasaOCuotaManager tasaOCuotaManager,
			IStringLocalizer<PrefacturasModel> localizer,
			ILogger<PrefacturasModel> logger,
			IConfiguration configuration,
			IArchivoEmpresaManager archivoEmpresaManager,
			IEncriptacionAES encriptacionAES
		) : ERPPageModel
	{

		[BindProperty]
		public FiltroModel InputFiltro { get; set; } = new FiltroModel();

		public class FiltroModel
		{
			[Display(Name = "SerieField")]
			public string? Serie { get; set; }

			[DataType(DataType.DateTime)]
			[Display(Name = "FechaInicioField")]
			public DateTime? FechaInicio { get; set; }

			[DataType(DataType.DateTime)]
			[Display(Name = "FechaFinField")]
			public DateTime? FechaFin { get; set; }

			[Display(Name = "MonedaField")]
			public int? MonedaId { get; set; }

			[Display(Name = "FormaPagoField")]
			public int? FormaPagoId { get; set; }

			[Display(Name = "MetodoPagoField")]
			public int? MetodoPagoId { get; set; }

			[Display(Name = "UsoCFDIField")]
			public int? UsoCFDIId { get; set; }

			[Display(Name = "UsuarioCreadorField")]
			public string? UsuarioCreadorId { get; set; }

			[Display(Name = "UsuarioTimbradorField")]
			public string? UsuarioTimbradorId { get; set; }
		}

		[BindProperty]
		public EmpresaModel InputEmpresa { get; set; } = new EmpresaModel();

		public class EmpresaModel()
		{
			[DisplayName("SearchCompanyField")]
			public string? TextoEmisor { get; set; } = string.Empty;

			[DisplayName("SearchCompanyField")]
			public string? TextoReceptor { get; set; } = string.Empty;
		}

		public ConceptoModel InputConceptos { get; set; } = new ConceptoModel();

		public class ConceptoModel
		{
			[Required(ErrorMessage = "Required")]
			[Display(Name = "CantidadField")]
			public int? Cantidad { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "UnidadField")]
			public int? UnidadId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "DescripcionField")]
			public string? Descripcion { get; set; } = string.Empty;

			[Required(ErrorMessage = "Required")]
			[Display(Name = "UnitarioField")]
			public decimal? Unitario { get; set; }

			[Display(Name = "DescuentoField")]
			public decimal? Descuento { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "ObjetoImpuestoField")]
			public int? ObjetoImpuestoId { get; set; }

			public TrasladoModel Traslado { get; set; } = new TrasladoModel();

			public RetencionModel Retencion { get; set; } = new RetencionModel();

			[Display(Name = "SearchProductServiceField")]
			public int? ProductoServicioId { get; set; }
		}

		public TrasladoModel InputTraslado { get; set; } = new TrasladoModel();
		public class TrasladoModel
		{
			[Required(ErrorMessage = "Required")]
			[Display(Name = "TrasladoField")]
			public int? TasaOCuotaId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "TrasladoField")]
			public decimal? Valor { get; set; }
		}

		public RetencionModel InputRetencion { get; set; } = new RetencionModel();
		public class RetencionModel
		{
			[Required(ErrorMessage = "Required")]
			[Display(Name = "RetencionField")]
			public int? TasaOCuotaId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "RetencionField")]
			public decimal? Valor { get; set; }
		}

		[BindProperty]
		public CFDIModel InputCFDI { get; set; } = new CFDIModel();

		public class CFDIModel
		{
			public int Id { get; set; }

			public int EmisorId { get; set; }

			public int ReceptorId { get; set; }

			[Required(ErrorMessage = "Required")]
			[RegularExpression(RegularExpressions.Alphabetic, ErrorMessage = "Alphabetic")]
			[Display(Name = "SerieField")]
			public string Serie { get; set; } = string.Empty;

			[Required(ErrorMessage = "Required")]
			[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "NumericNoRestriction")]
			[Display(Name = "FolioField")]
			public string Folio { get; set; } = string.Empty;

			[Required(ErrorMessage = "Required")]
			[Display(Name = "TipoComprobanteField")]
			public int TipoComprobanteId { get; set; }

			[Required(ErrorMessage = "Required")]
			[DataType(DataType.DateTime)]
			[Display(Name = "FechaField")]
			public DateTime Fecha { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "MonedaField")]
			public int MonedaId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "TipoCambioField")]
			public decimal TipoCambio { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "FormaPagoField")]
			public int FormaPagoId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "MetodoPagoField")]
			public int MetodoPagoId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "UsoCFDIField")]
			public int UsoCFDIId { get; set; }

			[Display(Name = "ExportacionField")]
			public int? ExportacionId { get; set; }

			[Display(Name = "NumeroOperacionField")]
			[RegularExpression(RegularExpressions.NumericFirstDigitNonZero, ErrorMessage = "Numeric")]
			public int? NumeroOperacion { get; set; }

			public bool RequiereAutorizacion { get; set; }

			public ConceptoModel?[] Conceptos { get; set; } = [];
		}

		public void OnGet()
		{
		}

		public async Task<JsonResult> OnPostDatosAdicionales(int idPrefactura)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetDatosAdicionales(idPrefactura);
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
		public async Task<JsonResult> OnPostFiltrar()
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetPrefacturasList(InputFiltro);
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
		private async Task<string> GetDatosAdicionales(int idPrefactura)
		{
			string jsonResponse;
			Prefactura? p = await prefacturaManager.GetByIdWithAdicionalesAsync(idPrefactura) ?? throw new Exception($"No se encontró información de la prefactura id {idPrefactura}");
			List<string> jsonConceptos;

			jsonConceptos = GetListJsonConceptos(p.Conceptos);

			jsonResponse = $"{{" +
								$"\"conceptos\": [{string.Join(",", jsonConceptos)}] " +
							$"}}";

			return jsonResponse;
		}
		private async Task<string> GetPrefacturasList(FiltroModel? filtro = null)
		{
			string nombreTipo;
			string nombreMoneda;
			string nombreForma;
			string nombreMetodo;
			string nombreUsoCFDI;
			string nombreExportacion;

			string jsonResponse;
			List<string> jsonPrefacturas = [];
			List<Prefactura> prefacturas;

			if (filtro != null)
			{
				prefacturas = await prefacturaManager.GetAllAsync(
					filtro.FechaInicio,
					filtro.FechaFin,
					filtro.Serie,
					filtro.MonedaId,
					filtro.FormaPagoId,
					filtro.MetodoPagoId,
					filtro.UsoCFDIId,
					filtro.UsuarioCreadorId,
					filtro.UsuarioTimbradorId,
					false
				);
			}
			else
			{
				prefacturas = await prefacturaManager.GetAllAsync();
			}

			foreach (Prefactura p in prefacturas)
			{
				nombreTipo = p.TipoComprobante != null ? p.TipoComprobante.Descripcion : "";
				nombreMoneda = p.Moneda != null ? p.Moneda.Descripcion : "";
				nombreForma = p.FormaPago != null ? p.FormaPago.Descripcion : "";
				nombreMetodo = p.MetodoPago != null ? p.MetodoPago.Descripcion : "";
				nombreUsoCFDI = p.UsoCFDI != null ? p.UsoCFDI.Descripcion : "";
				nombreExportacion = p.Exportacion != null ? p.Exportacion.Descripcion : "";

				DateTime? fecha = p.Fecha == DateTime.MinValue ? null : p.Fecha;

				List<string> listaAuth = GetListJsonAutorizaciones(p.Autorizaciones);

				AppUser? usr = userManager.GetUserAsync(User).Result;
				string safeL = string.Empty;
				if (usr != null)
				{
					safeL = $"userId={usr.Id}&id={p.Id}&module=prefacturas";
					safeL = encriptacionAES.PlainTextToBase64AES(safeL);
				}

				jsonPrefacturas.Add(
					"{" +
						$"\"id\": {p.Id}," +
						$"\"safeL\": \"{safeL}\"," +
						$"\"serie\": \"{p.Serie}\", " +
						$"\"folio\": \"{p.Folio}\", " +
						$"\"emisor\": \"{p.Emisor?.RazonSocial}\", " +
						$"\"emisorId\": {p.Emisor?.Id}, " +
						$"\"receptor\": \"{p.Receptor?.RazonSocial}\", " +
						$"\"receptorId\": {p.Receptor?.Id}, " +
						$"\"tipoComprobante\": \"{nombreTipo}\", " +
						$"\"tipoComprobanteId\": {p.TipoComprobanteId}, " +
						$"\"fecha\": \"{fecha:dd/MM/yyyy HH:mm:ss}\", " +
						$"\"fechaJS\": \"{fecha:yyyy-MM-dd HH:mm:ss}\", " +
						$"\"tipoCambio\": {p.TipoCambio}, " +
						$"\"moneda\": \"{nombreMoneda}\", " +
						$"\"monedaId\": {p.MonedaId}, " +
						$"\"formaPago\": \"{nombreForma}\", " +
						$"\"formaPagoId\": {p.FormaPagoId}, " +
						$"\"metodoPago\": \"{nombreMetodo}\", " +
						$"\"metodoPagoId\": {p.MetodoPagoId}, " +
						$"\"usoCFDI\": \"{nombreUsoCFDI}\", " +
						$"\"usoCFDIId\": {p.UsoCFDIId}, " +
						$"\"exportacion\": \"{nombreExportacion}\", " +
						$"\"exportacionId\": {p.ExportacionId}, " +
						$"\"numeroOperacion\": \"{p.NumeroOperacion}\", " +
						$"\"usuarioCreadorId\": \"{p.UsuarioCreadorId}\", " +
						$"\"usuarioTimbradorId\": \"{p.UsuarioTimbradorId}\", " +
						$"\"requiereAutorizacion\": \"{p.RequiereAutorizacion}\", " +
						$"\"estatus\": \"{p.Estatus?.Descripcion}\", " +
						$"\"estatusId\": \"{p.Estatus?.Id}\", " +
						$"\"autorizaciones\": [{string.Join(",", listaAuth)}], " +
						$"\"conceptos\": [] " +
					"}"
				);
			}

			jsonResponse = $"[{string.Join(",", jsonPrefacturas)}]";

			return jsonResponse;
		}
		private static List<string> GetListJsonAutorizaciones(ICollection<AutorizacionesPrefactura>? autorizacionesPrefacturas)
		{
			List<string> jsonAuth = [];
			if (autorizacionesPrefacturas != null)
			{
				foreach(AutorizacionesPrefactura aut in autorizacionesPrefacturas)
				{
					jsonAuth.Add($"{{" +
									$"\"id\": {aut.Id}, " +
									$"\"usuarioId\": {aut.UsuarioId} " +
								 $"}}");
				}
			}
			return jsonAuth;
		}
		private static List<string> GetListJsonConceptos(ICollection<Concepto>? conceptos)
		{
			decimal subtotal = 0,
				traslado = 0,
				retencion = 0,
				total = 0;
			string claveProdServ,
				nombreProdServ,
				nombreUnidad,
				nombreObjetoImpuesto;
			List<string> jsonConceptos = [];
			if (conceptos != null)
			{
				List<Concepto> concepts = [.. (from c in conceptos
										   orderby c.Id ascending
										   select c)];
				foreach (Concepto c in concepts)
				{
					claveProdServ = c.ProductoServicio != null ? c.ProductoServicio.Clave : string.Empty;
					nombreProdServ = c.ProductoServicio != null ? c.ProductoServicio.Descripcion : string.Empty;
					nombreUnidad = c.UnidadMedida != null ? $"{c.UnidadMedida.Clave} - {c.UnidadMedida.Nombre}" : string.Empty;
					nombreObjetoImpuesto = c.ObjetoImpuesto != null ? c.ObjetoImpuesto.Descripcion : string.Empty;

					subtotal = (c.Cantidad * c.PrecioUnitario) - c.Descuento;
					traslado = subtotal * c.TasaTraslado;
					retencion = subtotal * c.Retencion;
					total = subtotal + traslado - retencion;

					jsonConceptos.Add($"{{" +
										$"\"id\": {c.ProductoServicioId}, " +
										$"\"productoServicioId\": {c.ProductoServicioId}, " +
										$"\"productoServicio\": \"{nombreProdServ}\", " +
										$"\"objetoImpuestoId\": {c.ObjetoImpuestoId}, " +
										$"\"objetoImpuesto\": \"{nombreObjetoImpuesto}\", " +
										$"\"cantidad\": {c.Cantidad}, " +
										$"\"unidadId\": {c.UnidadMedidaId}, " +
										$"\"unidad\": \"{nombreUnidad}\", " +
										$"\"clave\": \"{claveProdServ}\", " +
										$"\"descripcion\": \"{c.Descripcion}\", " +
										$"\"unitario\": {c.PrecioUnitario}, " +
										$"\"descuento\": {c.Descuento}, " +
										$"\"traslado\": {{\"valor\": {c.Traslado} }}, " +
										$"\"retencion\": {{\"valor\": {c.Retencion} }}, " +
										$"\"subtotalCalculado\": {subtotal}, " +
										$"\"trasladoCalculado\": {traslado}, " +
										$"\"retencionCalculada\": {retencion}, " +
										$"\"totalCalculado\": {total} " +
									  $"}}");
				}
			}
			return jsonConceptos;
		}

		public async Task<JsonResult> OnPostSave()
		{
			ServerResponse resp = new(true, localizer["PrefacturaSavedUnsuccessfully"]);

			if (PuedeTodo || PuedeEditar)
			{
				if (!ModelState.IsValid)
				{
					resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors ?? []).Select(m => m.ErrorMessage).ToArray();
					return new JsonResult(resp);
				}
				try
				{
					string validacion = await ValidarSerieFolio(InputCFDI);

					//Si la longitud del mensaje de respuesta es menor o igual a cero, se considera que no hubo errores anteriores.
					if ((validacion ?? string.Empty).Length <= 0)
					{
						//Procede a crear o actualizar la prefactura.
						await CreateOrUpdatePrefactura(InputCFDI);

						resp.TieneError = false;
						resp.Mensaje = localizer["PrefacturaSavedSuccessfully"];
					}
					else
					{
						resp.Mensaje = validacion;
					}
				}
				catch (Exception ex)
				{
					logger.LogError(message: ex.Message);
				}
			}
			else
			{
				resp.Mensaje = localizer["AccesoDenegado"];
			}

			return new JsonResult(resp);
		}
		private async Task<string> ValidarSerieFolio(CFDIModel prefactura)
		{
			List<Prefactura> coincidences = [];
			List<Prefactura> prefs = await prefacturaManager.GetAllAsync();

			//Excluyo al empleado con el Id actual.
			prefs = prefs.Where(e => e.Id != prefactura.Id).ToList();

			//Valido que no exista prefactura con misma serie y folio.
			coincidences = prefs.Where(e => e.Deshabilitado == 0 && ($"{e.Serie}{e.Folio}" == $"{prefactura.Serie}{prefactura.Folio}")).ToList();
			if (coincidences.Count >= 1) { return $"{localizer["ErrorPrefacturaExistenteA"]} {prefactura.Serie}{prefactura.Folio}. {localizer["ErrorPrefacturaExistenteB"]}."; }

			return string.Empty;
		}
		private async Task CreateOrUpdatePrefactura(CFDIModel prefacturaModel)
		{
			try
			{
				await db.Database.BeginTransactionAsync();

				int idPrefactura = 0;
				int idEstatusSolicitada = 1;

				//Se busca prefactura por id
				Prefactura? prefactura = await prefacturaManager.GetByIdAsync(prefacturaModel.Id);

				//Si se encontró prefactura, obtiene su Id del registro existente. De lo contrario, se crea uno nuevo.
				if (prefactura != null) { idPrefactura = prefactura.Id; } else { prefactura = new Prefactura(); }

				//Llena los datos de la prefactura.
				prefactura.EmisorId = prefacturaModel.EmisorId;
				prefactura.ReceptorId = prefacturaModel.ReceptorId;
				prefactura.Fecha = prefacturaModel.Fecha;
				prefactura.Serie = prefacturaModel.Serie;
				prefactura.Folio = prefacturaModel.Folio;
				prefactura.TipoCambio = prefacturaModel.TipoCambio;
				prefactura.ExportacionId = prefacturaModel.ExportacionId;
				prefactura.FormaPagoId = prefacturaModel.FormaPagoId;
				prefactura.MetodoPagoId = prefacturaModel.MetodoPagoId;
				prefactura.UsoCFDIId = prefacturaModel.UsoCFDIId;
				prefactura.MonedaId = prefacturaModel.MonedaId;
				prefactura.NumeroOperacion = prefacturaModel.NumeroOperacion;
				prefactura.TipoCambio = prefacturaModel.TipoCambio;
				prefactura.TipoComprobanteId = prefacturaModel.TipoComprobanteId;
				prefactura.RequiereAutorizacion = prefacturaModel.RequiereAutorizacion;

				if (idPrefactura >= 1)
				{
					//Si la prefactura ya existía, la actualiza.
					await prefacturaManager.UpdateAsync(prefactura);

					//Elimina los conceptos de la prefactura.
					await conceptoManager.DeleteByPrefacturaIdAsync(idPrefactura);
				}
				else
				{
					//De lo contrario...

					//Se busca al usuario logeado
					AppUser? u = userManager.FindByNameAsync(User.Identity?.Name ?? "").Result;

					//Se establece al usuario que creó la prefactura.
					prefactura.UsuarioCreadorId = u?.Id;
					//Se establece el estatus de la prefactura en Solicitada
					prefactura.EstatusId = idEstatusSolicitada;

					//Crea al empleado y obtiene su id.
					idPrefactura = await prefacturaManager.CreateAsync(prefactura);
				}

				//Crea los conceptos de la prefactura
				foreach (ConceptoModel? c in prefacturaModel.Conceptos)
				{
					if (c == null) { continue; }

					int cantidad = c.Cantidad ?? 0;

					decimal precioUnitario = c.Unitario ?? 0,
						subtotal = cantidad * precioUnitario,
						tasaTraslado = c.Traslado?.Valor ?? 0,
						tasaRetencion = c.Retencion?.Valor ?? 0,
						traslado = subtotal * tasaTraslado,
						retencion = subtotal * tasaRetencion;

					//Se usa la info para guardar el concepto.
					await conceptoManager.CreateAsync(
						new Concepto()
						{
							ProductoServicioId = c.ProductoServicioId ?? 0,
							Cantidad = cantidad,
							PrecioUnitario = precioUnitario,
							Descuento = c.Descuento ?? 0,
							UnidadMedidaId = c.UnidadId ?? 0,
							Descripcion = c.Descripcion ?? string.Empty,
							ObjetoImpuestoId = c.ObjetoImpuestoId ?? 0,
							TasaTraslado = tasaTraslado,
							TasaRetencion = tasaRetencion,
							Traslado = traslado,
							Retencion = retencion,
							PrefacturaId = idPrefactura
						}
					);
				}

				await db.Database.CommitTransactionAsync();
			}
			catch (Exception)
			{
				await db.Database.RollbackTransactionAsync();
				throw;
			}
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
					List<string> jsonActividades = GetListJsonActividades(emp?.ActividadesEconomicasEmpresa);
					List<string> jsonProductosServicios = GetListJsonProductosServicios(prodServEmpresa);
					string serie = e.RFC != null ? e.RFC[..3] : string.Empty;
					List<Prefactura> prefacturas = await prefacturaManager.GetAllAsync(null, null, serie, null, null, null, null);
					prefacturas = [.. prefacturas.OrderByDescending(p => p.Id)];
					_ = int.TryParse(prefacturas.FirstOrDefault()?.Folio, out int proximoFolio);
					proximoFolio += 1;

					jsonEmpresas.Add($"{{" +
										$"\"id\": {e.Id}, " +
										$"\"value\": \"{e.RazonSocial}\", " +
										$"\"label\": \"{e.RFC} - {e.RazonSocial}\", " +
										$"\"rfc\": \"{e.RFC}\", " +
										$"\"proximoFolio\": \"{proximoFolio}\"," +
										$"\"razonSocial\": \"{e.RazonSocial}\", " +
										$"\"actividadesEconomicas\": [{string.Join(",", jsonActividades)}], " +
										$"\"objetoSocial\": \"{e.ObjetoSocial}\", " +
										$"\"origen\": \"{e.Origen}\", " +
										$"\"nivel\": {{" +
														$"\"nombre\": \"{e.Nivel}\", " +
														$"\"ordinal\": \"{e.Ordinal}\", " +
														$"\"puedeFacturar\": \"{e.PuedeFacturar}\"" +
													$"}}, " +
										$"\"perfil\": \"{e.Perfil}\", " +
										$"\"domicilioFiscal\": \"{e.DomicilioFiscal}\", " +
										$"\"productosServicios\": [{string.Join(",", jsonProductosServicios)}] " +
									$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsonEmpresas)}]";

			return jsonResponse;
		}
		private static List<string> GetListJsonProductosServicios(ICollection<ProductoServicioPerfil>? psp)
		{
			List<string> jsonProdServ = [];
			if (psp != null)
			{
				foreach (ProductoServicioPerfil a in psp)
				{
					if (a.ProductoServicio == null) { continue; }

					jsonProdServ.Add(
						"{" +
							$"\"id\": \"{a.ProductoServicio.Id}\"," +
							$"\"clave\": \"{a.ProductoServicio.Clave}\"" +
						"}"
					);
				}
			}

			return jsonProdServ;
		}
		private static List<string> GetListJsonActividades(ICollection<ActividadEconomicaEmpresa>? actividades)
		{
			List<string> jsonActividades = [];
			if (actividades != null)
			{
				foreach (ActividadEconomicaEmpresa a in actividades)
				{
					if (a.ActividadEconomica == null) { continue; }

					jsonActividades.Add(
						"{" +
							$"\"id\": \"{a.ActividadEconomica.Id}\"," +
							$"\"clave\": \"{a.ActividadEconomica.Clave}\"," +
							$"\"nombre\": \"{a.ActividadEconomica.Nombre}\"" +
						"}"
					);
				}
			}

			return jsonActividades;
		}
		private static string JsonEscape(string str)
		{
			return str.Replace("\n", "<br />").Replace("\r", "<br />").Replace("\t", "<br />");
		}

		public async Task<JsonResult> OnPostGetProductosServiciosSuggestion(string texto)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetProductosServiciosSuggestion(texto);
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
		private async Task<string> GetProductosServiciosSuggestion(string texto)
		{
			string jsonResponse;
			List<string> jsons = [];

			List<ProductoServicioBuscado> prodserv = await productosServiciosManager.SearchProductService(texto);

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

		public async Task<JsonResult> OnPostGetUsuariosSuggestion(string texto)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetUsuariosSuggestion(texto);
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
				logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}
		private async Task<string> GetUsuariosSuggestion(string texto)
		{
			string jsonResponse;
			List<string> jsonUsuarios = [];

			List<AppUser> usuarios = await userManager.SearchUsuarios(texto);

			if (usuarios != null)
			{
				foreach (AppUser u in usuarios)
				{
					string desc = u.Empleado?.NombreCompleto.Length >= 1 ? $"{u.Empleado?.NombreCompleto} - {u.UserName}" : $"{u.UserName}";
					jsonUsuarios.Add($"{{" +
										$"\"id\": \"{u.Id}\", " +
										$"\"value\": \"{desc}\", " +
										$"\"label\": \"{desc}\"" +
									$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsonUsuarios)}]";

			return jsonResponse;
		}

		public async Task<JsonResult> OnPostGetUnidadesSuggestion(string texto)
		{
			ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetUnidadesSuggestion(texto);
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
		private async Task<string> GetUnidadesSuggestion(string texto)
		{
			string jsonResponse;
			List<string> jsonUnidades = [];

			List<UnidadMedida> unidades = await unidadMedidaManager.SearchUnidades(texto);

			if (unidades != null)
			{
				foreach (UnidadMedida u in unidades)
				{
					if (u.Simbolo.Trim().Length >= 1)
					{
						jsonUnidades.Add($"{{" +
											$"\"id\": {u.Id}, " +
											$"\"value\": \"{u.Clave} - ({u.Simbolo}) {u.Nombre}\", " +
											$"\"label\": \"{u.Clave} - ({u.Simbolo}) {u.Nombre}\"" +
										$"}}");
					}
					else
					{
						jsonUnidades.Add($"{{" +
											$"\"id\": {u.Id}, " +
											$"\"value\": \"{u.Clave} - {u.Nombre}\", " +
											$"\"label\": \"{u.Clave} - {u.Nombre}\"" +
										$"}}");
					}
				}
			}

			jsonResponse = $"[{string.Join(",", jsonUnidades)}]";

			return jsonResponse;
		}

		public async Task<JsonResult> OnPostExportExcel(string[] ids)
		{
			ServerResponse resp = new(true, localizer["PrefacturasExportedUnsuccessfully"]);

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
								Prefactura? p = await prefacturaManager.GetByIdAsync(intId);
								if (p != null)
								{
									string clave = p.Serie + p.Folio.PadLeft(6, '0');
									foreach (Concepto c in p.Conceptos)
									{
										IRow row = sheet.CreateRow(rowIndex);
										//Clave
										CreateCell(row, 0, clave, cellStyle);
										//Cliente
										CreateCell(row, 1, p.ReceptorId.ToString(), cellStyle);
										//Fecha de elaboración
										CreateCell(row, 2, p.Fecha.ToString("dd/mm/yyyy"), cellStyle);
										//Su pedido
										CreateCell(row, 3, string.Empty, cellStyle);
										//Clave del artículo
										CreateCell(row, 4, c.ProductoServicio?.Clave ?? string.Empty, cellStyle);
										//Cantidad
										CreateCell(row, 5, c.Cantidad.ToString(), cellStyle);
										//Precio
										CreateCell(row, 6, c.PrecioUnitario.ToString(), cellStyle);
										//Desc. 1
										CreateCell(row, 7, string.Empty, cellStyle);
										//Desc. 2
										CreateCell(row, 8, string.Empty, cellStyle);
										//Desc. 3
										CreateCell(row, 9, string.Empty, cellStyle);
										//Clave de vendedor
										CreateCell(row, 10, string.Empty, cellStyle);
										//Comisión
										CreateCell(row, 11, string.Empty, cellStyle);
										//Clave de esquema de impuestos
										CreateCell(row, 12, string.Empty, cellStyle);
										//I.E.P.S.
										CreateCell(row, 13, GetIEPSConcepto(c, impuestosIEPS).ToString(), cellStyle);
										//Impuesto 2
										CreateCell(row, 14, string.Empty, cellStyle);
										//Impuesto 3
										CreateCell(row, 15, string.Empty, cellStyle);
										//I.V.A.
										CreateCell(row, 16, GetIVAConcepto(c, impuestosIVA).ToString(), cellStyle);
										//Impuesto 5
										CreateCell(row, 17, string.Empty, cellStyle);
										//Impuesto 6
										CreateCell(row, 18, string.Empty, cellStyle);
										//Impuesto 7
										CreateCell(row, 19, string.Empty, cellStyle);
										//Impuesto 8
										CreateCell(row, 20, string.Empty, cellStyle);
										//Método de pago
										CreateCell(row, 21, p.MetodoPago?.Clave ?? string.Empty, cellStyle);
										//Forma de Pago SAT
										CreateCell(row, 22, p.FormaPago?.Clave ?? string.Empty, cellStyle);
										//Uso CFDI
										CreateCell(row, 23, p.UsoCFDI?.Clave ?? string.Empty, cellStyle);
										//Clave SAT
										CreateCell(row, 24, c.ProductoServicio?.Clave ?? string.Empty, cellStyle);
										//Unidad SAT
										CreateCell(row, 25, c.UnidadMedida?.Clave ?? string.Empty, cellStyle);
										//Observaciones
										CreateCell(row, 26, c.Descripcion ?? string.Empty, cellStyle);
										//Observaciones de partida
										CreateCell(row, 27, string.Empty, cellStyle);
										//Fecha de entrega
										CreateCell(row, 28, string.Empty, cellStyle);
										//Fecha de vencimiento
										CreateCell(row, 29, string.Empty, cellStyle);
										//Descripcion
										CreateCell(row, 30, c.Descripcion ?? string.Empty, cellStyle);

										rowIndex++;
									}
								}
							}

							//Crea el archivo excel y lo exporta al usuario.
							using (var fileData = new FileStream("wwwroot/templates/Prefacturas.xls", FileMode.OpenOrCreate))
							{
								wb.Write(fileData);
							}

							wb.Close();
						}

						await db.Database.CommitTransactionAsync();

						resp.TieneError = false;
						resp.Mensaje = localizer["PrefacturasExportedSuccessfully"];
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

		public async Task<JsonResult> OnPostAutorizar(int idPrefactura)
		{
			ServerResponse resp = new(true, localizer["PrefacturaAuthorizedUnsuccessfully"]);

			try
			{
				if (PuedeTodo || PuedeAutorizar)
				{
					//Si el usuario tiene permisos de autorización...

					//Obtiene los datos de la prefactura
					Prefactura? p = await prefacturaManager.GetByIdAsync(idPrefactura);

                    int idEstatusSolicitada = 1;
					if(p != null && p.EstatusId == idEstatusSolicitada && p.RequiereAutorizacion)
					{
						//Si la prefactura existe y además el estatus de la prefactura es "Solicitada" y además requiere autorización...

						//Obtiene la información del usuario logueado.
						AppUser? user = await userManager.GetUserAsync(User);

						//Verifica si el usuario ya autorizó esta prefactura.
						bool foundUser = p.Autorizaciones?.Any(a => a.UsuarioId == user?.Id) ?? false;

						if (!foundUser) {
							//Si el usuario no ha autorizado...

							//Verifica si el usuario está permitido para autorizar prefacturas
							IConfigurationSection autorizadoresSection = configuration.GetSection("AutorizadoresPrefacturas");
							foundUser = autorizadoresSection.AsEnumerable().Any(a => a.Value == user?.Id);

							if (foundUser)
							{
								//Si el usuario está permitido para autorizar prefacturas, agrega su autorización a la base de datos.
								await autorizacionesPrefacturaManager.CreateAsync(new()
								{
									PrefacturaId = idPrefactura,
									UsuarioId = user?.Id ?? string.Empty,
									FechaHoraAutorizacion = DateTime.Now
								});

								resp.TieneError = false;
								resp.Mensaje = localizer["PrefacturaAuthorizedSuccessfully"];
							}
						}
					}
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
		
		public async Task<JsonResult> OnPostTimbrarMultiple(string[] ids)
		{
			ServerResponse resp = new(true, localizer["PrefacturasStampedUnsuccessfully"]);

			if (PuedeTodo || PuedeEditar)
			{
				try
				{
					await db.Database.BeginTransactionAsync();

					foreach (string id in ids)
					{
						_ = int.TryParse(id, out int idPrefactura);

						//Se timbra la prefactura
						ServerResponse respTimbre = new(true, localizer["FailedToStamp"] + $" {idPrefactura}");
						if (idPrefactura >= 1){ 
							respTimbre = await TimbrarPrefactura(idPrefactura);
							if (respTimbre.TieneError) {
								//TODO: Refactorizar método para que notifique errores por prefactura.
								resp.Errores.AddRange(respTimbre.Errores);
							}
						}
					}

					await db.Database.CommitTransactionAsync();

					resp.TieneError = false;
					resp.Mensaje = localizer["PrefacturasExportedSuccessfully"];
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
		public async Task<JsonResult> OnPostTimbrar(int idPrefactura)
		{
			ServerResponse resp = new(true, localizer["PrefacturaStampedUnsuccessfully"]);
			try
			{
				if(PuedeTodo || PuedeEditar)
				{
					//Se timbra la prefactura
					resp = await TimbrarPrefactura(idPrefactura);
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
		private async Task<ServerResponse> TimbrarPrefactura(int idPrefactura)
		{
			ServerResponse resp = new(true, localizer["PrefacturaStampedUnsuccessfully"]);
			try
			{
				//Obtiene los datos de la prefactura
				Prefactura? p = await prefacturaManager.GetByIdAsync(idPrefactura);

				if (p != null)
				{
					int anio = DateTime.Now.Year;
					int mes = DateTime.Now.Month;
					int dia = DateTime.Now.Day;
					string guid = $"pref_{idPrefactura}{DateTime.Now:yyyyMMddHHmmssfffffff}";

					string pathXML = $"wwwroot/cfdiv40/xml/{anio}/{mes}/{dia}/sin_timbre/",
						XMLFilePath = $"{pathXML}{guid}.xml",
						pathZip = $"wwwroot/cfdiv40/xml/{anio}/{mes}/{dia}/con_timbre/",
						ZipFilePath = $"{pathZip}{guid}.zip",
						pathXSLT = "Utils/cadenaoriginal_4_0.xslt",
						cadenaOriginal = string.Empty;

					SelloDigital sello = new();
					Comprobante cfdi;

					if (!Path.Exists(pathXML)) { Directory.CreateDirectory(pathXML); }
					if (!Path.Exists(pathZip)) { Directory.CreateDirectory(pathZip); }

					//Se obtienen los archivos del Emisor
					List <SemiArchivoEmpresa> archivos = await archivoEmpresaManager.GetFilesByEmpresaIdAsync(p.EmisorId);

					SemiArchivoEmpresa? saCER = archivos.Where(a => a.TipoArchivoId == (int)FileTypes.CER).FirstOrDefault(),
										saKEY = archivos.Where(a => a.TipoArchivoId == (int)FileTypes.KEY).FirstOrDefault();

                    if (saCER == null || saCER.FileSize <= 0) { throw new Exception(localizer["ArchivoCERMissing"]); }
                    if (saKEY == null || saKEY.FileSize <= 0) { throw new Exception(localizer["ArchivoKEYMissing"]); }

                    byte[] fileCER = archivoEmpresaManager.GetFileById(saCER.Id)?.Archivo ?? [],
						   fileKEY = archivoEmpresaManager.GetFileById(saKEY.Id)?.Archivo ?? [];

					if (fileCER == null || fileCER.Length <= 0) { throw new Exception(localizer["ArchivoCERMissing"]); }
					if (fileKEY == null || fileKEY.Length <= 0) { throw new Exception(localizer["ArchivoKEYMissing"]); }

					//Crea el CFDI a partir de los datos de la prefactura.
					cfdi = CrearCFDIFromPrefactura(p);
					if(cfdi == null) { throw new Exception(localizer["ErrorEstructuraCFDI"]); }

					//Establece el número del certificado, obtenido a partir del archivo CER
					cfdi.NoCertificado = ObtenerNumeroCertificado(fileCER);
					if (cfdi.NoCertificado.Length <= 0) { throw new Exception(localizer["ErrorNumeroCertificado"]); }

					//Crea el XML a partir del CFDI creado y lo guarda en una ruta física del servidor.
					await CrearXMLFromCFDIAsync(cfdi, XMLFilePath);
					if (!System.IO.File.Exists(XMLFilePath)) { throw new Exception(localizer["ErrorEstructuraXML"]); }

					//Se obtiene la cadena original a partir del xml creado y usando el archivo xslt del sat.
					cadenaOriginal = ObtenerCadenaOriginal(pathXSLT, XMLFilePath);
					if (cadenaOriginal.Length <= 0) { throw new Exception(localizer["ErrorCadenaOriginal"]); }

					//Se elimina el archivo XML sin sellar
					System.IO.File.Delete(XMLFilePath);

					//Se obtiene el certificado a partir del archivo CER
					cfdi.Certificado = sello.Certificado(fileCER);
					if (cfdi.Certificado.Length <= 0) { throw new Exception(localizer["ErrorCertificado"]); }

					//Se obtiene el sello a partir del archivo KEY usando la cadena original y la clave privada
					cfdi.Sello = sello.Sellar(cadenaOriginal, fileKEY, encriptacionAES.Base64AESToPlainText(p.Emisor?.PFESAT ?? string.Empty));
					if (cfdi.Sello.Length <= 0) { throw new Exception(localizer["ErrorSello"]); }

					//Se vuelve a crear el XML sellado.
					await CrearXMLFromCFDIAsync(cfdi, XMLFilePath);
					if (!System.IO.File.Exists(XMLFilePath)) { throw new Exception(localizer["ErrorEstructuraXML"]); }

					//Se obtienen los bytes de la cadena base64 generada a partir del XML
					byte[] fileCFDI = await CrearBytesXMLAsync(XMLFilePath);
					if (fileCFDI == null || fileCFDI.Length <= 0) { throw new Exception(localizer["ErrorBytesXMLCFDI"]); }

					//Se realiza el timbrado del comprobante
					byte[] zipComprobanteTimbrado = await TimbrarComprobanteEnEDICOMAsync(configuration["ERPSEI_EDICOM_USR"] ?? string.Empty, configuration["ERPSEI_EDICOM_PWD"] ?? string.Empty, fileCFDI);
					if (zipComprobanteTimbrado == null || zipComprobanteTimbrado.Length <= 0) { throw new Exception(localizer["ErrorTimbradoCFDI"]); }

					//Se guarda el comprobante timbrado
					await GuardarZipComprobante(ZipFilePath, zipComprobanteTimbrado);
					if (!System.IO.File.Exists(ZipFilePath)) { throw new Exception(localizer["ErrorGuardadoCFDI"]); }

					//Devuelve mensaje correcto de timbrado.
					resp.TieneError = false;
					resp.Errores = [];
					resp.Mensaje = localizer["PrefacturaStampedSuccessfully"];
				}
			}
			catch (Exception ex)
			{
				//Devuelve el error en el timbrado.
				resp.Errores = [..resp.Errores.Append(ex.Message)];
			}

			return resp;
		}
		private async Task GuardarZipComprobante(string pathZip, byte[] zipComprobanteTimbrado)
		{
			string b64Zip = Convert.ToBase64String(zipComprobanteTimbrado);
			byte[] zip = Convert.FromBase64String(b64Zip);
			using (FileStream f = new(pathZip, FileMode.OpenOrCreate))
			{
				await f.WriteAsync(zip);
				System.IO.Compression.ZipFile.ExtractToDirectory(f, Path.GetDirectoryName(pathZip) ?? string.Empty);
			}
		}
		private async Task<byte[]> TimbrarComprobanteEnEDICOMAsync(string user, string password, byte[] fileB64)
		{
			byte[] zipComprobanteTimbrado;

			getCfdiTestResponse resp = new();
			getCfdiTestRequest req = new() { user = user, password = password, file = fileB64 };
			resp = await clienteEDICOM.getCfdiTestAsync(req);
			zipComprobanteTimbrado = resp.getCfdiTestReturn;

			return zipComprobanteTimbrado;
		}
		private async Task<byte[]> CrearBytesXMLAsync(string pathXML)
		{
			byte[] fileCFDI;
			using (FileStream f = new(pathXML, FileMode.Open))
			{
				fileCFDI = new byte[f.Length];
				await f.ReadAsync(fileCFDI);
			}
			return fileCFDI;
		}
		private async Task CrearXMLFromCFDIAsync(Comprobante cfdi, string pathXML)
		{
			XmlSerializerNamespaces xmlsn = new();
			xmlsn.Add("cfdi", "http://www.sat.gob.mx/cfd/4");
			xmlsn.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
			xmlsn.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

			XmlSerializer xmlSerializer = new(typeof(Comprobante));

			string xml = string.Empty;

			using (StringWriterCustomEncoding sw = new(Encoding.UTF8))
			{
				using (XmlWriter xmlw = XmlWriter.Create(sw))
				{
					xmlSerializer.Serialize(xmlw, cfdi, xmlsn);
					xml = sw.ToString();
				}
			}

			await System.IO.File.WriteAllTextAsync(pathXML, xml);
		}
		private string ObtenerCadenaOriginal(string pathXSLT, string pathXML)
		{
			string cadenaOriginal = string.Empty;
			XslCompiledTransform transformador = new();
			XsltSettings settings = new(true, true);
			XmlUrlResolver resolver = new();
			transformador.Load(pathXSLT, settings, resolver);

			using (StringWriter sw = new())
			using (XmlWriter xmlw = XmlWriter.Create(sw, transformador.OutputSettings))
			{
				transformador.Transform(pathXML, xmlw);
				cadenaOriginal = sw.ToString();
			}

			return cadenaOriginal;
		}
		private string ObtenerNumeroCertificado(byte[] fileCER)
		{
			//Obtiene el número de certificado
			string noCertificado, x, y, z;
			SelloDigital.leerCER(fileCER, out x, out y, out z, out noCertificado);

			return noCertificado;
		}
		private string ObtenerCodigoPostal(string direccion)
		{
			return Regex.Match(direccion, "[0-9]{5}").Value;
		}
        private List<ComprobanteConcepto> CrearConceptosFromPrefactura(ICollection<Concepto> conceptos)
		{
			List<ComprobanteConcepto> lc = [];

			foreach (Concepto c in conceptos ?? [])
			{
                string umn = c.UnidadMedida?.Nombre ?? "0";
                lc.Add(new ComprobanteConcepto()
				{
					Impuestos = new()
					{
						Traslados = [new() {
							Base = 0.000001m,
							Importe = 0,
							ImporteSpecified = true,
							Impuesto = "002",
							TasaOCuota = c.TasaTraslado,
							TasaOCuotaSpecified = true,
							TipoFactor = "Tasa"
                        }],
						Retenciones = [new() {
                            Base = 0.000001m,
                            Importe = 0,
                            Impuesto = "002",
                            TasaOCuota = c.TasaRetencion,
                            TipoFactor = "Tasa"
                        }]
					},
					ClaveProdServ = c.ProductoServicio?.Clave ?? string.Empty,
					NoIdentificacion = c.ProductoServicio?.Clave ?? string.Empty,
					Cantidad = c.Cantidad,
					ClaveUnidad = c.UnidadMedida?.Clave ?? string.Empty,
					Descripcion = c.Descripcion ?? string.Empty,
					ValorUnitario = c.PrecioUnitario,
					Importe = c.Cantidad * c.PrecioUnitario,
					Descuento = c.Descuento,
					DescuentoSpecified = true,
					ObjetoImp = c.ObjetoImpuesto?.Clave ?? string.Empty,
				});
			}

			return lc;
		}
		private Comprobante CrearCFDIFromPrefactura(Prefactura p)
		{
			//Obtiene los datos de los conceptos
			List<ComprobanteConcepto> lc = CrearConceptosFromPrefactura(p.Conceptos);
			List<ComprobanteImpuestosTraslado> cit = [];
            List<ComprobanteImpuestosRetencion> cir = [];
			decimal totalTraslados = 0m;
            decimal totalRetenciones = 0m;
			decimal totalImportes = 0m;
			decimal totalDescuentos = 0m;
			string moneda = p.Moneda?.Clave ?? "MXN";

            foreach (ComprobanteConcepto cc in lc)
			{
				foreach(ComprobanteConceptoImpuestosTraslado ccit in cc.Impuestos?.Traslados ?? [])
				{
					cit.Add(new() { 
						Base = Math.Round(ccit.Base, 2),
						Importe = ccit.Importe,
						ImporteSpecified = ccit.ImporteSpecified,
						Impuesto = ccit.Impuesto,
						TasaOCuota = ccit.TasaOCuota,
						TasaOCuotaSpecified = ccit.TasaOCuotaSpecified,
						TipoFactor = ccit.TipoFactor
					});
					totalTraslados += ccit.Importe;
				}
				foreach(ComprobanteConceptoImpuestosRetencion ccir in cc.Impuestos?.Retenciones ?? [])
				{
					cir.Add(new() { 
						Importe = ccir.Importe,
						Impuesto = ccir.Impuesto
					});
					totalRetenciones += ccir.Importe;
                }

				totalImportes += cc.Importe;
				totalDescuentos += cc.Descuento;
            }

            return new()
			{
				Emisor = new()
				{
					Rfc = p.Emisor?.RFC ?? string.Empty,
					Nombre = p.Emisor?.RazonSocial ?? string.Empty,
					RegimenFiscal = p.Emisor?.RegimenFiscal?.Clave ?? string.Empty
				},
				Receptor = new()
				{
					Rfc = p.Receptor?.RFC ?? string.Empty,
					Nombre = p.Receptor?.RazonSocial ?? string.Empty,
					DomicilioFiscalReceptor = ObtenerCodigoPostal(p.Receptor?.DomicilioFiscal ?? string.Empty),
					RegimenFiscalReceptor = p.Receptor?.RegimenFiscal?.Clave ?? string.Empty,
					UsoCFDI = p.UsoCFDI?.Clave ?? string.Empty
                },
				Conceptos = [.. lc],
				Impuestos = new()
				{
					Retenciones = [.. cir],
					Traslados = [.. cit],
					TotalImpuestosRetenidos = totalRetenciones,
					TotalImpuestosRetenidosSpecified = true,
					TotalImpuestosTrasladados = totalTraslados,
					TotalImpuestosTrasladadosSpecified = true
				},
				Version = "4.0",
				Serie = p.Serie ?? string.Empty,
				Folio = p.Folio ?? string.Empty,
				Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"),
				Sello = string.Empty,
				FormaPago = p.FormaPago?.Clave ?? string.Empty,
				FormaPagoSpecified = true,
				NoCertificado = string.Empty,
				Certificado = string.Empty,
				SubTotal = Math.Round(totalImportes, p.Moneda?.Decimales ?? 2),
				Descuento = Math.Round(totalDescuentos, p.Moneda?.Decimales ?? 2),
                DescuentoSpecified = true,
				Moneda = moneda,
				TipoCambio = moneda == "MXN" ? 1 : p.TipoCambio,
				TipoCambioSpecified = true,
				Total = Math.Round(totalImportes - totalDescuentos + totalTraslados - totalRetenciones, p.Moneda?.Decimales ?? 2),
                TipoDeComprobante = p.TipoComprobante?.Clave ?? string.Empty,
				Exportacion = p.Exportacion?.Clave ?? string.Empty,
				MetodoPago = p.MetodoPago?.Clave ?? string.Empty,
				MetodoPagoSpecified = true,
				LugarExpedicion = ObtenerCodigoPostal(p.Emisor?.DomicilioFiscal ?? string.Empty)
            };
		}
	}
}
