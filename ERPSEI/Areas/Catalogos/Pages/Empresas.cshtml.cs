using ERPSEI.Data;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.SAT.Catalogos;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Pages.Shared;
using ERPSEI.Requests;
using ERPSEI.Resources;
using ERPSEI.Utils;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Mime;
using System.Web;

namespace ERPSEI.Areas.Catalogos.Pages
{
	[Authorize(Policy = "AccessPolicy")]
	public class EmpresasModel(
			IEmpresaManager _empresaManager,
			IBancoEmpresaManager _bancoEmpresaManager,
			IArchivoEmpresaManager _archivoEmpresaManager,
			IPerfilManager _perfilManager,
			IRWCatalogoManager<Origen> _origenManager,
			IRWCatalogoManager<Nivel> _nivelManager,
			IActividadEconomicaEmpresaManager _actividadesEconomicasEmpresaManager,
			IRWCatalogoManager<ActividadEconomica> _actividadEconomicaManager,
			IStringLocalizer<EmpresasModel> _strLocalizer,
			ILogger<EmpresasModel> _logger,
			ApplicationDbContext _db,
			IEncriptacionAES _encriptacionAES,
			AppUserManager _userManager
		) : ERPPageModel
	{

		[BindProperty]
		public FiltroModel InputFiltro { get; set; } = new FiltroModel();

		public class FiltroModel
		{
			[Display(Name = "OrigenField")]
			public int? OrigenId { get; set; }

			[Display(Name = "NivelField")]
			public int? NivelId { get; set; }

            [Display(Name = "ActividadEconomicaField")]
            public int? ActividadEconomicaId { get; set; }

			[Display(Name = "RFCField")]
			public string? RFC {  get; set; }
        }

		[BindProperty]
		public EmpresaModel InputEmpresa { get; set; } = new EmpresaModel();

		public class EmpresaModel
		{
			public int Id { get; set; }

			[DataType(DataType.Text)]
			[StringLength(100, ErrorMessage = "FieldLength", MinimumLength = 2)]
			[RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "CompanyName")]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "RazonSocialField")]
			public string RazonSocial { get; set; } = string.Empty;

			[Display(Name = "OrigenField")]
			public int? OrigenId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "NivelField")]
			public int? NivelId { get; set; }

            [DataType(DataType.DateTime)]
            [Display(Name = "FechaConstitucionField")]
            public DateTime? FechaConstitucion { get; set; }

            [DataType(DataType.DateTime)]
			[Display(Name = "FechaInicioOperacionField")]
			public DateTime? FechaInicioOperacion { get; set; }

            [DataType(DataType.DateTime)]
            [Display(Name = "FechaInicioFacturacionField")]
            public DateTime? FechaInicioFacturacion { get; set; }

            [DataType(DataType.DateTime)]
            [Display(Name = "FechaInicioAsimiladosField")]
            public DateTime? FechaInicioAsimilados { get; set; }

            [DataType(DataType.Text)]
			[StringLength(13, ErrorMessage = "FieldLength", MinimumLength = 12)]
			[RegularExpression(RegularExpressions.AlphanumNoSpaceNoUnderscore, ErrorMessage = "AlphanumNoSpaceNoUnderscore")]
			[Display(Name = "RFCField")]
			public string? RFC { get; set; } = string.Empty;

			[DataType(DataType.MultilineText)]
			[StringLength(300, ErrorMessage = "FieldLength", MinimumLength = 1)]
			[Display(Name = "DomicilioFiscalField")]
			public string? DomicilioFiscal { get; set; } = string.Empty;

			[DataType(DataType.Text)]
			[StringLength(100, ErrorMessage = "FieldLength", MinimumLength = 2)]
			[RegularExpression(RegularExpressions.PersonName, ErrorMessage = "PersonName")]
			public string? Administrador { get; set; } = string.Empty;

			[DataType(DataType.Text)]
			[StringLength(100, ErrorMessage = "FieldLength", MinimumLength = 2)]
			[RegularExpression(RegularExpressions.PersonName, ErrorMessage = "PersonName")]
			[Display(Name = "AccionistaField")]
			public string? Accionista { get; set; } = string.Empty;

			[EmailAddress(ErrorMessage = "EmailFormat")]
			[DataType(DataType.EmailAddress)]
			[StringLength(55, ErrorMessage = "FieldLength", MinimumLength = 1)]
			[Display(Name = "CorreoGeneralField")]
			public string? CorreoGeneral { get; set; } = string.Empty;

			[EmailAddress(ErrorMessage = "EmailFormat")]
			[DataType(DataType.EmailAddress)]
			[StringLength(55, ErrorMessage = "FieldLength", MinimumLength = 1)]
			[Display(Name = "CorreoBancosField")]
			public string? CorreoBancos { get; set; } = string.Empty;

			[EmailAddress(ErrorMessage = "EmailFormat")]
			[DataType(DataType.EmailAddress)]
			[StringLength(55, ErrorMessage = "FieldLength", MinimumLength = 1)]
			[Display(Name = "CorreoFiscalField")]
			public string? CorreoFiscal { get; set; } = string.Empty;

            [EmailAddress(ErrorMessage = "EmailFormat")]
            [DataType(DataType.EmailAddress)]
			[StringLength(55, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [Display(Name = "CorreoFacturacionField")]
            public string? CorreoFacturacion { get; set; } = string.Empty;

            [Phone(ErrorMessage = "PhoneFormat")]
			[StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 10)]
			[DataType(DataType.PhoneNumber)]
			[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "NumericNoRestriction")]
			[Display(Name = "PhoneNumberField")]
			public string? Telefono { get; set; } = string.Empty;

			[Required(ErrorMessage = "Required")]
			[Display(Name = "PerfilField")]
			public int? PerfilId { get; set; }

			[Display(Name = "SearchEconomicActivityField")]
            public int? ActividadEconomicaId {  get; set; }
			public int?[] ActividadesEconomicas { get; set; } = [];

			[DataType(DataType.MultilineText)]
			[StringLength(5000, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [Display(Name = "ObjetoSocialField")]
            public string? ObjetoSocial {  get; set; } = string.Empty;

            public BancoModel?[] Bancos { get; set; } = [];

            public ArchivoModel?[] Archivos { get; set; } = [];

			[DataType(DataType.Password)]
			[Display(Name = "OldPasswordField")]
			public string? ArchivosSATOldPassword { get; set; } = string.Empty;

			[DataType(DataType.Password)]
			[Display(Name = "NewPasswordField")]
			public string? ArchivosSATNewPassword { get; set; } = string.Empty;

			[DataType(DataType.Password)]
			[Display(Name = "ConfirmNewPasswordField")]
			public string? ArchivosSATConfirmNewPassword { get; set; } = string.Empty;
		}

		public class BancoModel
		{
            public string? Banco { get; set; } = string.Empty;
            public string? Responsable { get; set; } = string.Empty;
            public string? Firmante { get; set; } = string.Empty;
			public decimal? Limite { get; set; } = 0m;
        }

		public class ArchivoModel
		{
			public string? Id { get; set; } = string.Empty;
			public string? Nombre { get; set; } = string.Empty;
			public int? TipoArchivoId { get; set; }
			public string? Extension { get; set; } = string.Empty;
			public string? ImgSrc { get; set; } = string.Empty;
		}

		[BindProperty]
		public ImportarModel InputImportar { get; set; } = new ImportarModel();
		public class ImportarModel
		{
			[Required(ErrorMessage = "Required")]
			public IFormFile? Plantilla { get; set; }
		}

        private async Task<string> GetDatosAdicionalesEmpresa(int idEmpresa)
        {
            string jsonResponse = string.Empty;
            Empresa? e = await _empresaManager.GetByIdWithAdicionalesAsync(idEmpresa);

            if (e == null) { return jsonResponse; }

			List<string> jsonActividades = GetListJsonActividades(e.ActividadesEconomicasEmpresa);

			List<string> jsonBancos = GetListJsonBancos(e.BancosEmpresa);

            List<string> jsonArchivos;
            List<SemiArchivoEmpresa> archivos = await _archivoEmpresaManager.GetFilesByEmpresaIdAsync(idEmpresa);
            jsonArchivos = GetListJsonArchivos(archivos);

            jsonResponse = $"{{" +
								$"\"actividadesEconomicas\": [{string.Join(",", jsonActividades)}], " +
								$"\"bancos\": [{string.Join(",", jsonBancos)}], " +
                                $"\"archivos\": [{string.Join(",", jsonArchivos)}] " +
                            $"}}";

            return jsonResponse;
        }
        private async Task<string> GetListaEmpresas(FiltroModel? filtro = null)
		{
			string nombreOrigen;
			string nombreNivel;
			string nombrePerfil;
			string jsonResponse;
			List<string> jsonEmpresas = [];
			List<Empresa> empresas;

			if(filtro != null)
			{
				empresas = await _empresaManager.GetAllAsync(
					filtro.OrigenId,
					filtro.NivelId,
					filtro.ActividadEconomicaId,
					filtro.RFC
				);
            }
			else
			{
				empresas = await _empresaManager.GetAllAsync();
			}

			foreach (Empresa e in empresas)
			{
				nombreOrigen = e.Origen != null ? e.Origen.Nombre : string.Empty;
				nombreNivel = e.Nivel != null ? e.Nivel.Nombre : string.Empty;
				nombrePerfil = e.Perfil != null ? e.Perfil.Nombre : string.Empty;

				e.ObjetoSocial = JsonEscape(e.ObjetoSocial??string.Empty);

				DateTime? fechaConstitucion = e.FechaConstitucion == DateTime.MinValue ? null : e.FechaConstitucion;
				DateTime? fechaInicioOperacion = e.FechaInicioOperacion == DateTime.MinValue ? null : e.FechaInicioOperacion;
				DateTime? fechaInicioFacturacion = e.FechaInicioFacturacion == DateTime.MinValue ? null : e.FechaInicioFacturacion;
				DateTime? fechaInicioAsimilados = e.FechaInicioAsimilados == DateTime.MinValue ? null : e.FechaInicioAsimilados;

				jsonEmpresas.Add(
					"{" +
						$"\"id\": {e.Id}," +
						$"\"razonSocial\": \"{e.RazonSocial}\", " +
                        $"\"perfilId\": \"{e.PerfilId}\", " +
						$"\"perfil\": \"{nombrePerfil}\", " +
						$"\"origenId\": \"{e.OrigenId}\", " +
						$"\"origen\": \"{nombreOrigen}\", " +
						$"\"nivelId\": \"{e.NivelId}\", " +
						$"\"nivel\": \"{nombreNivel}\", " +
						$"\"fechaConstitucion\": \"{fechaConstitucion:dd/MM/yyyy}\", " +
						$"\"fechaConstitucionJS\": \"{fechaConstitucion:yyyy-MM-dd}\", " +
						$"\"fechaInicioOperacion\": \"{fechaInicioOperacion:dd/MM/yyyy}\", " +
						$"\"fechaInicioOperacionJS\": \"{fechaInicioOperacion:yyyy-MM-dd}\", " +
						$"\"fechaInicioFacturacion\": \"{fechaInicioFacturacion:dd/MM/yyyy}\", " +
						$"\"fechaInicioFacturacionJS\": \"{fechaInicioFacturacion:yyyy-MM-dd}\", " +
						$"\"fechaInicioAsimilados\": \"{fechaInicioAsimilados:dd/MM/yyyy}\", " +
						$"\"fechaInicioAsimiladosJS\": \"{fechaInicioAsimilados:yyyy-MM-dd}\", " +
						$"\"rfc\": \"{e.RFC}\", " +
						$"\"domicilioFiscal\": \"{e.DomicilioFiscal}\", " +
                        $"\"administrador\": \"{e.Administrador}\", " +
						$"\"accionista\": \"{e.Accionista}\", " +
                        $"\"correoGeneral\": \"{e.CorreoGeneral}\", " +
						$"\"correoBancos\": \"{e.CorreoBancos}\", " +
						$"\"correoFiscal\": \"{e.CorreoFiscal}\", " +
						$"\"correoFacturacion\": \"{e.CorreoFacturacion}\", " +
						$"\"telefono\": \"{e.Telefono}\", " +
						$"\"objetoSocial\": \"{e.ObjetoSocial}\", " +
						$"\"hasPasswordSAT\": \"{e.PFESAT?.Length >= 1}\"" +
					"}"
				);
			}

			jsonResponse = $"[{string.Join(",", jsonEmpresas)}]";

			return jsonResponse;
		}
		private static string JsonEscape(string str)
		{
			return str.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
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
		private static List<string> GetListJsonBancos(ICollection<BancoEmpresa>? bancos)
        {
            List<string> jsonBancos = [];
            if (bancos != null)
            {
                foreach (BancoEmpresa b in bancos)
                {
                    jsonBancos.Add(
                        "{" +
                            $"\"id\": \"{b.Id}\"," +
                            $"\"banco\": \"{b.Banco}\"," +
                            $"\"responsable\": \"{b.Responsable}\"," +
                            $"\"firmante\": \"{b.Firmante}\", " +
							$"\"limite\": \"{b.Limite}\"" +
						"}"
                    );
                }
            }

            return jsonBancos;
        }
        private List<string> GetListJsonArchivos(ICollection<SemiArchivoEmpresa>? archivos)
		{
			List<string> jsonArchivos = [];
			string imgSrc = string.Empty;
			string htmlContainer = string.Empty;
			SemiArchivoEmpresa? a = null;
			string id = string.Empty;

			//ordena los archivos de la empresa de menor a mayor por tipoArchivoId
			List<SemiArchivoEmpresa> empresaFiles = [.. (from empresaFile in archivos
												   orderby empresaFile.TipoArchivoId ascending
												   select empresaFile)];

			//Recorre todos los tipos de archivos.
			foreach (FileTypes i in Enum.GetValues(typeof(FileTypes)))
			{
				id = Guid.NewGuid().ToString();
				a = empresaFiles.Where(f => f.TipoArchivoId == (int)i).FirstOrDefault();

				if (a != null)
				{
					//Si el tipo de archivo está en la lista de archivos de la empresa, lo usa.
					string b64 = Convert.ToBase64String(a.Archivo);
					bool isJPG = a.Extension == "jpg" || a.Extension == "jpeg";
					bool isPNG = a.Extension == "png";
					bool isPDF = a.Extension == "pdf";

					if (isPDF)
					{
						imgSrc = $"data:application/pdf;base64,{b64}";
						htmlContainer = $"<canvas id = '{id}' b64 = '{b64}' class = 'canvaspdf'></canvas>";
					}
					else if (isJPG || isPNG)
					{
						if (isJPG)
						{
							imgSrc = $"data:image/jpeg;base64,{b64}";
						}
						else if (isPNG)
						{
							imgSrc = $"data:image/png;base64,{b64}";
						}
						htmlContainer = $"<img id = '{id}' src = '{imgSrc}' style='max-height: 200px;'/>";
					}

					AppUser? usr = _userManager.GetUserAsync(User).Result;
					string safeL = string.Empty;
					if (usr != null) { 
						safeL = $"userId={usr.Id}&id={a.Id}&module=empresas";
						safeL = _encriptacionAES.PlainTextToBase64AES(safeL);
					}

					jsonArchivos.Add(
						"{" +
							$"\"id\": \"{a.Id}\"," +
							$"\"safeL\": \"{safeL}\"," +
							$"\"nombre\": \"{a.Nombre}\"," +
							$"\"tipoArchivoId\": {a.TipoArchivoId}," +
							$"\"extension\": \"{a.Extension}\"," +
							$"\"imgSrc\": \"{imgSrc}\"," +
							$"\"htmlContainer\": \"{htmlContainer}\"," +
							$"\"fileSize\": \"{a.FileSize}\"" +
						"}"
					);
				}
				else
				{
					//De lo contrario, agrega el archivo como vacío para poder ser llenado por el usuario.
					htmlContainer = "<i class='bi bi-file-image opacity-50' style='font-size:105px'></i>";
					jsonArchivos.Add(
						"{" +
							$"\"id\": \"{id}\"," +
							$"\"nombre\": \"\"," +
							$"\"tipoArchivoId\": {(int)i}," +
							$"\"extension\": \"\"," +
							$"\"imgSrc\": \"{imgSrc}\"," +
							$"\"htmlContainer\": \"{htmlContainer}\"," +
							$"\"fileSize\": \"0\"" +
						"}"
					);
				}
				
			}

			return jsonArchivos;
		}

		public ActionResult OnGetDownloadPlantilla()
		{
			if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
			{
				return File("/templates/PlantillaEmpresas.xlsx", MediaTypeNames.Application.Octet, "PlantillaEmpresas.xlsx");
			}
			else
			{
				return new EmptyResult();
			}
		}

        public async Task<JsonResult> OnPostDatosAdicionalesEmpresa(int idEmpresa)
        {
            ServerResponse resp = new(true, _strLocalizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetDatosAdicionalesEmpresa(idEmpresa);
					resp.TieneError = false;
					resp.Mensaje = _strLocalizer["ConsultadoSuccessfully"];
				}
				else
				{
					resp.Mensaje = _strLocalizer["AccesoDenegado"];
				}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }
        public async Task<JsonResult> OnPostFiltrarEmpresas()
		{
			ServerResponse resp = new(true, _strLocalizer["EmpresasFiltradasUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetListaEmpresas(InputFiltro);
					resp.TieneError = false;
					resp.Mensaje = _strLocalizer["EmpresasFiltradasSuccessfully"];
				}
				else
				{
					resp.Mensaje = _strLocalizer["AccesoDenegado"];
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}
		
		public async Task<JsonResult> OnPostDisableEmpresas(string[] ids)
		{
			ServerResponse resp = new(true, _strLocalizer["EmpresasDeletedUnsuccessfully"]);
			await _db.Database.BeginTransactionAsync();
			try
			{
				if (PuedeTodo || PuedeEliminar) 
				{
					foreach (string id in ids)
					{
						int intId = Convert.ToInt32(id);
						await _empresaManager.DisableByIdAsync(intId);
					}

					resp.TieneError = false;
					resp.Mensaje = _strLocalizer["EmpresasDeletedSuccessfully"];
				}
				else
				{
					resp.Mensaje = _strLocalizer["AccesoDenegado"];
				}

				await _db.Database.CommitTransactionAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				await _db.Database.RollbackTransactionAsync();
			}

			return new JsonResult(resp);
		}
		
		public async Task<JsonResult> OnPostSaveEmpresa()
		{
			ServerResponse resp = new(true, _strLocalizer["EmpresaSavedUnsuccessfully"]);

			if (PuedeTodo || PuedeEditar)
			{
				//Se remueve el campo Plantilla para que no sea validado ya que no pertenece a este proceso.
				ModelState.Remove("Plantilla");

				if (!ModelState.IsValid)
				{
					resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors ?? []).Select(m => m.ErrorMessage).ToArray();
					return new JsonResult(resp);
				}

				try
				{
					//Valida que no exista una empresa registrada con los mismos datos. En caso de haber, se deja el mensaje en resp.Mensajes para ser mostrado al usuario.
					string validacion = await ValidarSiExisteEmpresa(InputEmpresa, false);

					//Si la longitud del mensaje de respuesta es menor o igual a cero, se considera que no hubo errores anteriores.
					if ((validacion ?? string.Empty).Length <= 0)
					{
						//Procede a crear o actualizar la empresa.
						validacion = await CreateOrUpdateCompany(InputEmpresa);

						if ((validacion ?? string.Empty).Length <= 0)
						{
							resp.TieneError = false;
							resp.Mensaje = _strLocalizer["EmpresaSavedSuccessfully"];
						}
						else
						{
							resp.Mensaje = validacion;
						}
					}
					else
					{
						resp.Mensaje = validacion;
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex.Message);
				}
			}
			else
			{
				resp.Mensaje = _strLocalizer["AccesoDenegado"];
			}

			return new JsonResult(resp);
		}
		private async Task<string> ValidarSiExisteEmpresa(EmpresaModel emp, bool rfcAsKey)
		{
			List<Empresa> coincidences = [];
			List<Empresa> emps = await _empresaManager.GetAllAsync();

			if (rfcAsKey)
			{
				//Excluyo a la empresa con el RFC actual.
				emps = emps.Where(e => e.RFC != emp.RFC).ToList();
			}
			else
			{
				//Excluyo a la empresa con el Id actual.
				emps = emps.Where(e => e.Id != emp.Id).ToList();
			}

			//Valido que no exista empresa que tenga los mismos datos.
			coincidences = emps.Where(e => (e.RazonSocial ?? "").Length >= 1 && e.RazonSocial == emp.RazonSocial).ToList();
            if (coincidences.Count >= 1) { return $"{_strLocalizer["ErrorEmpresaExistenteA"]} {_strLocalizer["RazonSocial"]} {emp.RazonSocial}. {_strLocalizer["ErrorEmpresaExistenteB"]}."; }

			coincidences = emps.Where(e => (e.RFC ?? "").Length >= 1 && e.RFC == emp.RFC).ToList();
			if (coincidences.Count >= 1) { return $"{_strLocalizer["ErrorEmpresaExistenteA"]} {_strLocalizer["RFC"]} {emp.RFC}. {_strLocalizer["ErrorEmpresaExistenteB"]}."; }

			return string.Empty;
		}
		private async Task<string> CreateOrUpdateCompany(EmpresaModel e)
		{
			try
			{
				await _db.Database.BeginTransactionAsync();

				int idEmpresa = 0;

				//Se busca empresa por id
				Empresa? empresa = await _empresaManager.GetByIdAsync(e.Id);

				//Si se encontró empresa, obtiene su Id del registro existente. 
				if (empresa != null) { 
					idEmpresa = empresa.Id;

					//Valida la información de bancos
					foreach (BancoModel? b in e.Bancos)
					{
						if (b != null && InputEmpresa.NivelId != null)
						{
							//Obtiene la información del Nivel
							Nivel? n = await _nivelManager.GetByIdAsync(InputEmpresa.NivelId ?? 0);

							//Si el nivel puede facturar, entonces valida que el límite de todos los bancos sea mayor a cero.
							if (n != null && n.PuedeFacturar && b.Limite <= 0)
							{
								return _strLocalizer["LimiteNoPuedeSerCero", n.Nombre];
							}
						}
					}
				}
				else
				{
					//De lo contrario, busca la empresa por RFC.
					empresa = await _empresaManager.GetByRFCAsync(e.RFC ?? string.Empty);

					//Si se encontró empresa, obtiene su Id del registro existente. De lo contrario, crea una nueva empresa.
					if (empresa != null) { idEmpresa = empresa.Id; } else { empresa = new Empresa(); }
				}

				//Se valida el password SAT en caso de venir
				if (e.ArchivosSATConfirmNewPassword?.Length >= 1 || e.ArchivosSATConfirmNewPassword?.Length >= 1)
				{
					if (e.ArchivosSATNewPassword != e.ArchivosSATConfirmNewPassword)
					{
						//La contraseña nueva de la firma electrónica de la empresa no es igual a la confirmación de contraseña nueva.
						return _strLocalizer["NuevoPasswordSATNoCoincide"];
					}
					if ((e.ArchivosSATOldPassword ?? string.Empty) != _encriptacionAES.Base64AESToPlainText(empresa.PFESAT ?? string.Empty))
					{
						//La contraseña anterior de la firma electrónica de la empresa no coincide con la contraseña anterior introducida por el usuario.
						return _strLocalizer["AnteriorPasswordSATNoCoincide"];
					}
                    if (e.ArchivosSATNewPassword?.Length >= 1 && e.ArchivosSATConfirmNewPassword?.Length >= 1)
                    {
                        empresa.PFESAT = _encriptacionAES.PlainTextToBase64AES(e.ArchivosSATNewPassword ?? string.Empty);
                    }
                }

				//Llena los datos de la empresa.
				empresa.RazonSocial = e.RazonSocial;
				empresa.PerfilId = e.PerfilId;
				empresa.OrigenId = e.OrigenId;
				empresa.NivelId = e.NivelId;
				empresa.FechaConstitucion = e.FechaConstitucion;
				empresa.FechaInicioOperacion = e.FechaInicioOperacion;
				empresa.FechaInicioFacturacion = e.FechaInicioFacturacion;
				empresa.FechaInicioAsimilados = e.FechaInicioAsimilados;
				empresa.RFC = e.RFC ?? string.Empty;
				empresa.DomicilioFiscal = e.DomicilioFiscal ?? string.Empty;
				empresa.Administrador = e.Administrador ?? string.Empty;
				empresa.Accionista = e.Accionista ?? string.Empty;
				empresa.CorreoGeneral = e.CorreoGeneral;
				empresa.CorreoBancos = e.CorreoBancos;
				empresa.CorreoFiscal = e.CorreoFiscal;
				empresa.CorreoFacturacion = e.CorreoFacturacion;
				empresa.Telefono = e.Telefono;
				empresa.ObjetoSocial = e.ObjetoSocial;

                if (idEmpresa >= 1)
				{
                    //Si la empresa ya existía, la actualiza.
                    await _empresaManager.UpdateAsync(empresa);

					//Elimina las actividades económicas de la empresa.
					await _actividadesEconomicasEmpresaManager.DeleteByEmpresaIdAsync(idEmpresa);

					//Elimina los bancos de la empresa.
					await _bancoEmpresaManager.DeleteByEmpresaIdAsync(idEmpresa);

                    //Elimina los archivos de la empresa que requieran actualizarse.
                    foreach (ArchivoModel? a in e.Archivos){ await _archivoEmpresaManager.DeleteByIdAsync(a?.Id ?? string.Empty); } 
				}
				else
				{
                    //De lo contrario, crea a la empresa y obtiene su id.
                    idEmpresa = await _empresaManager.CreateAsync(empresa);
				}

                //Crea las actividades de la empresa
                foreach (int? id in e.ActividadesEconomicas)
                {
                    if(id.HasValue)
					{
						await _actividadesEconomicasEmpresaManager.CreateAsync(
							new ActividadEconomicaEmpresa() { ActividadEconomicaId = id, EmpresaId = idEmpresa }
						);
					}
                }

                //Crea los bancos de la empresa
                foreach (BancoModel? b in e.Bancos)
				{
					if(b != null)
					{
						await _bancoEmpresaManager.CreateAsync(
							new BancoEmpresa() { Banco = b.Banco??string.Empty, Responsable = b.Responsable??string.Empty, Firmante = b.Firmante??string.Empty, Limite = b.Limite??0m, EmpresaId = idEmpresa }
						);
					}
				}

                //Crea los archivos de la empresa
                foreach (ArchivoModel? a in e.Archivos)
				{
					if (a == null) { continue; }

					//Se usa la info para guardar el archivo.
					await _archivoEmpresaManager.CreateAsync(
						new ArchivoEmpresa() { 
							Archivo = (a.ImgSrc ?? string.Empty).Length >= 1 ? Convert.FromBase64String(a.ImgSrc ?? string.Empty) : [], 
							EmpresaId = idEmpresa, 
							Extension = a.Extension ?? string.Empty, 
							Nombre = a.Nombre ?? string.Empty, 
							TipoArchivoId = a.TipoArchivoId 
						}
					);
                }


				await _db.Database.CommitTransactionAsync();
			}
			catch (Exception)
			{
				await _db.Database.RollbackTransactionAsync();
				throw;
			}
			return string.Empty;
		}

		public async Task<JsonResult> OnPostImportarEmpresas()
		{
			ServerResponse resp = new(true, _strLocalizer["EmpresasImportadasUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeEditar) { 
					if (Request.Form.Files.Count >= 1)
					{
						//Se procesa el archivo excel.
						using Stream s = Request.Form.Files[0].OpenReadStream();
						using var reader = ExcelReaderFactory.CreateReader(s);
						DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration() { FilterSheet = (tableReader, sheetIndex) => sheetIndex == 0 });
						foreach (DataRow row in result.Tables[0].Rows)
						{
							//Omite el procesamiento del row de encabezado
							if (result.Tables[0].Rows.IndexOf(row) == 0)
							{
								resp.TieneError = false;
								resp.Mensaje = _strLocalizer["EmpresasImportadasSuccessfully"];
								continue;
							}

							string vmsg = await CreateCompanyFromExcelRow(row);

							//Si la longitud del mensaje de respuesta es mayor o igual a uno, se considera que hubo errores.
							if ((vmsg ?? "").Length >= 1)
							{
								resp.TieneError = true;
								resp.Mensaje = vmsg;
								break;
							}
							else
							{
								resp.TieneError = false;
								resp.Mensaje = _strLocalizer["EmpresasImportadasSuccessfully"];
							}
						}
					}
				}
				else
				{
					resp.Mensaje = _strLocalizer["AccesoDenegado"];
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				resp.TieneError = true;
				resp.Mensaje = _strLocalizer["EmpresasImportadasUnsuccessfully"];
			}

			return new JsonResult(resp);
		}
		private static string NormalizePhrase(string s)
		{
            string[] parts = s.Split(" ", StringSplitOptions.RemoveEmptyEntries);
			List<string> normalizedPhrase = [];

            foreach (string part in parts){ normalizedPhrase.Add(part.Trim()); }

            return string.Join(" ", normalizedPhrase);
		}
		private async Task<string> CreateCompanyFromExcelRow(DataRow row)
        {
			Perfil? perfil = await _perfilManager.GetByNameAsync(row[1].ToString()?.Trim() ?? string.Empty);
            Origen? origen = await _origenManager.GetByNameAsync(row[2].ToString()?.Trim() ?? string.Empty);
            Nivel? nivel = await _nivelManager.GetByNameAsync(row[3].ToString()?.Trim() ?? string.Empty);

			string actividadesEconomicas = row[17].ToString()?.Trim() ?? string.Empty;
			string[] actividades = actividadesEconomicas.Split((char)10, StringSplitOptions.RemoveEmptyEntries);

			List<int?> listActividades = [];
            foreach (string item in actividades)
            {
				string phrase = NormalizePhrase(item);
				ActividadEconomica? actividadEconomica = await _actividadEconomicaManager.GetByNameAsync(phrase);
				if(actividadEconomica != null) { listActividades.Add(actividadEconomica.Id); } else { _logger.LogDebug($"Actividad económica {phrase} no encontrada de la empresa {row[0].ToString()?.Trim() ?? string.Empty}"); }
            }


			_ = DateTime.TryParse(row[4].ToString()?.Trim(), out DateTime fc);
			_ = DateTime.TryParse(row[5].ToString()?.Trim(), out DateTime fio);
			_ = DateTime.TryParse(row[6].ToString()?.Trim(), out DateTime fif);
			_ = DateTime.TryParse(row[7].ToString()?.Trim(), out DateTime fia);

			EmpresaModel e = new() {
				RazonSocial = row[0].ToString()?.Trim() ?? string.Empty,
				PerfilId = perfil?.Id,
				OrigenId = origen?.Id,
				NivelId = nivel?.Id,
				FechaConstitucion = fc,
				FechaInicioOperacion = fio,
				FechaInicioFacturacion = fif,
				FechaInicioAsimilados = fia,
				RFC = row[8].ToString()?.Trim() ?? string.Empty,
				DomicilioFiscal = row[9].ToString()?.Trim() ?? string.Empty,
				Administrador = row[10].ToString()?.Trim() ?? string.Empty,
				Accionista = row[11].ToString()?.Trim() ?? string.Empty,
				CorreoGeneral = row[12].ToString()?.Trim() ?? string.Empty,
				CorreoBancos = row[13].ToString()?.Trim() ?? string.Empty,
				CorreoFiscal = row[14].ToString()?.Trim() ?? string.Empty,
				CorreoFacturacion = row[15].ToString()?.Trim() ?? string.Empty,
				Telefono = row[16].ToString()?.Trim() ?? string.Empty,
				ActividadesEconomicas = [.. listActividades],
                ObjetoSocial = row[18].ToString()?.Trim() ?? string.Empty
			};

			e.ObjetoSocial = JsonEscape(e.ObjetoSocial);

			List<BancoModel> bancos = [];
			//Si existe banco 1, agrega uno al listado.
			if ((row[19].ToString()?.Trim() ?? string.Empty).Length >= 1) { bancos.Add(new BancoModel() { Banco = row[19].ToString()?.Trim() ?? string.Empty, Responsable = row[20].ToString()?.Trim() ?? string.Empty, Firmante = row[21].ToString()?.Trim() ?? string.Empty }); }
			//Si existe banco 2, agrega uno al listado.
			if ((row[22].ToString()?.Trim() ?? string.Empty).Length >= 1) { bancos.Add(new BancoModel() { Banco = row[22].ToString()?.Trim() ?? string.Empty, Responsable = row[23].ToString()?.Trim() ?? string.Empty, Firmante = row[24].ToString()?.Trim() ?? string.Empty }); }
			//Si existe banco 3, agrega uno al listado.
			if ((row[25].ToString()?.Trim() ?? string.Empty).Length >= 1) { bancos.Add(new BancoModel() { Banco = row[25].ToString()?.Trim() ?? string.Empty, Responsable = row[26].ToString()?.Trim() ?? string.Empty, Firmante = row[27].ToString()?.Trim() ?? string.Empty }); }
			//Si existe banco 4, agrega uno al listado.
			if ((row[28].ToString()?.Trim() ?? string.Empty).Length >= 1) { bancos.Add(new BancoModel() { Banco = row[28].ToString()?.Trim() ?? string.Empty, Responsable = row[29].ToString()?.Trim() ?? string.Empty, Firmante = row[30].ToString()?.Trim() ?? string.Empty }); }
			//Si existe banco 5, agrega uno al listado.
			if ((row[31].ToString()?.Trim() ?? string.Empty).Length >= 1) { bancos.Add(new BancoModel() { Banco = row[31].ToString()?.Trim() ?? string.Empty, Responsable = row[32].ToString()?.Trim() ?? string.Empty, Firmante = row[33].ToString()?.Trim() ?? string.Empty }); }

			e.Bancos = [.. bancos];

			List<ArchivoModel> archivos = [];
			//Crea los archivos de la empresa.
			foreach (FileTypes i in Enum.GetValues(typeof(FileTypes)))
			{
				archivos.Add(new ArchivoModel() { Extension = "", ImgSrc = "", Nombre = "", TipoArchivoId = (int)i });
			}

			e.Archivos = [.. archivos];

			//Valida que no exista una empresa registrada con los mismos datos. En caso de haber, se deja el mensaje en resp.Mensajes para ser mostrado al usuario.
			string validationMsg = await ValidarSiExisteEmpresa(e, true);

			//Si la longitud del mensaje de respuesta es menor o igual a cero, se considera que no hubo errores anteriores.
			if ((validationMsg ?? "").Length <= 0)
			{
				//Procede a crear o actualizar la empresa.
				await CreateOrUpdateCompany(e);
			}

			return validationMsg ?? "";
		}

		public async Task<JsonResult> OnPostGetActividadesEconomicasSuggestion(string texto)
		{
			ServerResponse resp = new(true, _strLocalizer["ConsultadoUnsuccessfully"]);
			try
			{
				if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
				{
					resp.Datos = await GetActividadesEconomicasSuggestion(texto);
					resp.TieneError = false;
					resp.Mensaje = _strLocalizer["ConsultadoSuccessfully"];
				}
				else
				{
					resp.Mensaje = _strLocalizer["AccesoDenegado"];
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}
		private async Task<string> GetActividadesEconomicasSuggestion(string texto)
		{
			string jsonResponse;
			List<string> jsons = [];

			List<ActividadEconomica> actividades = await _actividadEconomicaManager.GetAllAsync();
			actividades = actividades.Where(e => e.Nombre.ToLowerInvariant().Contains(texto.ToLowerInvariant()) || e.Clave.ToLowerInvariant().Contains(texto.ToLowerInvariant())).Take(20).ToList();

			if (actividades != null)
			{
				foreach (ActividadEconomica a in actividades)
				{
					jsons.Add($"{{" +
										$"\"id\": {a.Id}, " +
										$"\"value\": \"{a.Nombre}\", " +
										$"\"label\": \"{a.Clave} - {a.Nombre}\", " +
										$"\"clave\": \"{a.Clave}\"" +
									$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsons)}]";

			return jsonResponse;
		}
	}
}