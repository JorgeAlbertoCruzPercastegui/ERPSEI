using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Email;
using ERPSEI.Requests;
using ERPSEI.Resources;
using ERPSEI.Utils;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Mime;
using System.Text;
using System.Web;
using ERPSEI.Data.Entities.Metricas;

namespace ERPSEI.Areas.Catalogos.Pages
{
	[Authorize(Policy = "AccessPolicy")]
	public class GestionDeTalentoModel : PageModel
	{
		private readonly IUserStore<AppUser> _userStore;
		private readonly IUserEmailStore<AppUser> _emailStore;
		private readonly AppUserManager _userManager;
		private readonly IEmpleadoManager _empleadoManager;
		private readonly IRWCatalogoManager<Area> _areaManager;
		private readonly IRWCatalogoManager<Subarea> _subareaManager;
		private readonly IRWCatalogoManager<Puesto> _puestoManager;
		private readonly IRWCatalogoManager<Oficina> _oficinaManager;

		private readonly IRCatalogoManager<Genero> _generoManager;
		private readonly IRCatalogoManager<EstadoCivil> _estadoCivilManager;
		private readonly IContactoEmergenciaManager _contactoEmergenciaManager;
		private readonly IArchivoEmpleadoManager _archivoEmpleadoManager;
		private readonly IStringLocalizer<GestionDeTalentoModel> _strLocalizer;
		private readonly ILogger<GestionDeTalentoModel> _logger;
		private readonly ApplicationDbContext _db;

		private readonly IEncriptacionAES _encriptacionAES;

		private readonly IEmailSender _emailSender;

		[BindProperty]
		public FiltroModel InputFiltro { get; set; }

		public class FiltroModel
		{
			[DataType(DataType.DateTime)]
			[Display(Name = "FechaIngresoInicioField")]
			public DateTime? FechaIngresoInicio { get; set; }

			[DataType(DataType.DateTime)]
			[Display(Name = "FechaIngresoFinField")]
			public DateTime? FechaIngresoFin { get; set; }

			[DataType(DataType.DateTime)]
			[Display(Name = "FechaNacimientoInicioField")]
			public DateTime? FechaNacimientoInicio { get; set; }

			[DataType(DataType.DateTime)]
			[Display(Name = "FechaNacimientoFinField")]
			public DateTime? FechaNacimientoFin { get; set; }

			[Display(Name = "PuestoField")]
			public int? PuestoId { get; set; }

			[Display(Name = "AreaField")]
			public int? AreaId { get; set; }

			[Display(Name = "SubareaField")]
			public int? SubareaId { get; set; }

			[Display(Name = "OficinaField")]
			public int? OficinaId { get; set; }
		}

		[BindProperty]
		public EmpleadoModel InputEmpleado { get; set; }

		public class EmpleadoModel
		{
			public int Id { get; set; }

			public IFormFile? ProfilePicture { get; set; }

			[Required(ErrorMessage = "Required")]
			[StringLength(30, ErrorMessage = "FieldLength", MinimumLength = 2)]
			[RegularExpression(RegularExpressions.PersonName, ErrorMessage = "PersonName")]
			[DataType(DataType.Text)]
			[Display(Name = "NameField")]
			public string Nombre { get; set; } = string.Empty;

			[StringLength(15, ErrorMessage = "FieldLength", MinimumLength = 2)]
			[RegularExpression(RegularExpressions.PersonName, ErrorMessage = "PersonName")]
			[DataType(DataType.Text)]
			[Display(Name = "PreferredNameField")]
			public string? NombrePreferido { get; set; } = string.Empty;

			[Required(ErrorMessage = "Required")]
			[StringLength(30, ErrorMessage = "FieldLength", MinimumLength = 2)]
			[RegularExpression(RegularExpressions.PersonName, ErrorMessage = "PersonName")]
			[DataType(DataType.Text)]
			[Display(Name = "FathersLastNameField")]
			public string ApellidoPaterno { get; set; } = string.Empty;

			[Required(ErrorMessage = "Required")]
			[StringLength(30, ErrorMessage = "FieldLength", MinimumLength = 2)]
			[RegularExpression(RegularExpressions.PersonName, ErrorMessage = "PersonName")]
			[DataType(DataType.Text)]
			[Display(Name = "MothersLastNameField")]
			public string ApellidoMaterno { get; set; } = string.Empty;

			[DataType(DataType.DateTime)]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "FechaNacimientoField")]
			public DateTime FechaNacimiento { get; set; }

			[DataType(DataType.DateTime)]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "FechaIngresoField")]
			public DateTime FechaIngreso { get; set; }

			[Required(ErrorMessage = "Required")]
			[DataType(DataType.MultilineText)]
			[StringLength(300, ErrorMessage = "FieldLength", MinimumLength = 1)]
			[Display(Name = "DireccionField")]
			public string? Direccion { get; set; } = string.Empty;

			[Phone(ErrorMessage = "PhoneFormat")]
			[StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 10)]
			[DataType(DataType.PhoneNumber)]
			[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "NumericNoRestriction")]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "PhoneNumberField")]
			public string Telefono { get; set; } = string.Empty;

			[EmailAddress(ErrorMessage = "EmailFormat")]
			[DataType(DataType.EmailAddress)]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "CorreoEmpresarialField")]
			public string Email { get; set; } = string.Empty;

			[DataType(DataType.Text)]
			[StringLength(18, ErrorMessage = "FieldLength", MinimumLength = 18)]
			[RegularExpression(RegularExpressions.AlphanumNoSpaceNoUnderscore, ErrorMessage = "AlphanumNoSpaceNoUnderscore")]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "CURPField")]
			public string CURP { get; set; } = string.Empty;

			[DataType(DataType.Text)]
			[StringLength(13, ErrorMessage = "FieldLength", MinimumLength = 13)]
			[RegularExpression(RegularExpressions.AlphanumNoSpaceNoUnderscore, ErrorMessage = "AlphanumNoSpaceNoUnderscore")]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "RFCField")]
			public string RFC { get; set; } = string.Empty;

			[DataType(DataType.Text)]
			[StringLength(11, ErrorMessage = "FieldLength", MinimumLength = 9)]
			[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "Numeric")]
			[Required(ErrorMessage = "Required")]
			[Display(Name = "NSSField")]
			public string NSS { get; set; } = string.Empty;

			[Display(Name = "HorarioField")]
			public int? HorarioId { get; set; }

			[Display(Name = "GeneroField")]
			public int? GeneroId { get; set; }

			[Display(Name = "EstadoCivilField")]
			public int? EstadoCivilId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "PuestoField")]
			public int PuestoId { get; set; }

			[Required(ErrorMessage = "Required")]
			[Display(Name = "AreaField")]
			public int AreaId { get; set; }

			[Display(Name = "SubareaField")]
			public int? SubareaId { get; set; }

			[Display(Name = "OficinaField")]
			public int? OficinaId { get; set; }

			[Display(Name = "JefeField")]
			public int? JefeId { get; set; }

			[DataType(DataType.Text)]
			[StringLength(60, ErrorMessage = "FieldLength", MinimumLength = 2)]
			[RegularExpression(RegularExpressions.PersonName, ErrorMessage = "PersonName")]
			[Display(Name = "NameField")]
			public string? NombreContacto1 { get; set; } = string.Empty;

			[Phone(ErrorMessage = "PhoneFormat")]
			[StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 10)]
			[DataType(DataType.PhoneNumber)]
			[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "NumericNoRestriction")]
			[Display(Name = "PhoneNumberField")]
			public string? TelefonoContacto1 { get; set; } = string.Empty;

			[DataType(DataType.Text)]
			[StringLength(60, ErrorMessage = "FieldLength", MinimumLength = 2)]
			[RegularExpression(RegularExpressions.PersonName, ErrorMessage = "PersonName")]
			[Display(Name = "NameField")]
			public string? NombreContacto2 { get; set; } = string.Empty;

			[Phone(ErrorMessage = "PhoneFormat")]
			[StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 10)]
			[DataType(DataType.PhoneNumber)]
			[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "NumericNoRestriction")]
			[Display(Name = "PhoneNumberField")]
			public string? TelefonoContacto2 { get; set; } = string.Empty;

			public ArchivoModel?[] Archivos { get; set; } = [];
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
		public ImportarModel InputImportar { get; set; }
		public class ImportarModel
		{
			[Required(ErrorMessage = "Required")]
			public IFormFile? Plantilla { get; set; }
		}

        private readonly AuditoriaContext _auditoriaContext;

        public GestionDeTalentoModel(
			IUserStore<AppUser> store,
			AppUserManager userManager,
			IEmpleadoManager empleadoManager,
			IRWCatalogoManager<Area> areaManager,
			IRWCatalogoManager<Subarea> subareaManager,
			IRWCatalogoManager<Puesto> puestoManager,
			IRWCatalogoManager<Oficina> oficinaManager,
			IRCatalogoManager<Genero> generoManager,
			IRCatalogoManager<EstadoCivil> estadoCivilManager,
			IContactoEmergenciaManager contactoEmergenciaManager,
			IArchivoEmpleadoManager archivoEmpleadoManager,
			IStringLocalizer<GestionDeTalentoModel> stringLocalizer,
			ILogger<GestionDeTalentoModel> logger,
			ApplicationDbContext db,
			IEmailSender emailSender,
			IEncriptacionAES encriptacionAES,
            AuditoriaContext auditoriaContext
        )
		{
			_userStore = store;
			_userManager = userManager;
			_emailStore = GetEmailStore();
			_empleadoManager = empleadoManager;
			_areaManager = areaManager;
			_subareaManager = subareaManager;
			_puestoManager = puestoManager;
			_oficinaManager = oficinaManager;
			_generoManager = generoManager;
			_estadoCivilManager = estadoCivilManager;
			_contactoEmergenciaManager = contactoEmergenciaManager;
			_archivoEmpleadoManager = archivoEmpleadoManager;
			_strLocalizer = stringLocalizer;
			_logger = logger;
			_db = db;
			_emailSender = emailSender;
			_encriptacionAES = encriptacionAES;
            _auditoriaContext = auditoriaContext;

            InputFiltro = new FiltroModel();
			InputEmpleado = new EmpleadoModel();
			InputImportar = new ImportarModel();
		}

		private IUserEmailStore<AppUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("La UI default requiere almacenar un usuario con correo electrónico.");
			}
			return (IUserEmailStore<AppUser>)_userStore;
		}


		private async Task<string> GetDatosAdicionalesEmpleado(int idEmpleado)
		{
			string jsonResponse;
			string nombreJefe;
			Empleado? e = await _empleadoManager.GetByIdWithAdicionalesAsync(idEmpleado) ?? throw new Exception($"No se encontró información del empleado id {idEmpleado}");
			Empleado? jefe = e.JefeId != null ? await _empleadoManager.GetByIdAsync((int)e.JefeId) : null;
			List<SemiArchivoEmpleado> archivos = await _archivoEmpleadoManager.GetFilesByEmpleadoIdAsync(idEmpleado);
            foreach (SemiArchivoEmpleado a in archivos)
            {
				if(a.TipoArchivoId == (int)FileTypes.ImagenPerfil)
				{
					ArchivoEmpleado? ae = _archivoEmpleadoManager.GetFileById(a.Id);
					if (ae != null) { a.Archivo = ae.Archivo; }
					break;   
				}
            }
            List<string> jsonArchivos;
			List<string> jsonContactosEmergencia;

			nombreJefe = jefe != null ? jefe.NombreCompleto : "";

			jsonContactosEmergencia = GetListJsonContactosEmergencia(e.ContactosEmergencia);

			jsonArchivos = GetListJsonArchivos(archivos);

			jsonResponse = $"{{" +
								$"\"jefeId\": {e.JefeId ?? 0}, " +
								$"\"jefe\": \"{nombreJefe}\", " +
								$"\"contactosEmergencia\": [{string.Join(",", jsonContactosEmergencia)}], " +
								$"\"archivos\": [{string.Join(",", jsonArchivos)}] " +
							$"}}";

			return jsonResponse;
		}
		private async Task<string> GetTalentList(FiltroModel? filtro = null)
		{
			string nombreHorario;
			string nombreArea;
			string nombreSubarea;
			string nombrePuesto;
			string nombreOficina;
			string nombreGenero;
			string nombreEstadoCivil;
			string jsonResponse;
			List<string> jsonEmpleados = [];
			List<Empleado> empleados;

			if (filtro != null)
			{
				empleados = await _empleadoManager.GetAllAsync(
					filtro.FechaIngresoInicio, 
					filtro.FechaIngresoFin, 
					filtro.FechaNacimientoInicio, 
					filtro.FechaNacimientoFin,
					filtro.PuestoId,
					filtro.AreaId,
					filtro.SubareaId,
					filtro.OficinaId
				);
			}
			else
			{
				empleados = await _empleadoManager.GetAllAsync();
			}

			foreach (Empleado e in empleados)
			{
				nombreHorario = e.Horario != null ? e.Horario.Descripcion : "";
				nombreArea = e.Area != null ? e.Area.Nombre : "";
				nombreSubarea = e.Subarea != null ? e.Subarea.Nombre : "";
				nombrePuesto = e.Puesto != null ? e.Puesto.Nombre : "";
				nombreOficina = e.Oficina != null ? e.Oficina.Nombre : "";
				nombreGenero = e.Genero != null ? e.Genero.Nombre : "";
				nombreEstadoCivil = e.EstadoCivil != null ? e.EstadoCivil.Nombre : "";
				AppUser? usuario = e.UserId != null && e.UserId.Length >= 1 ? await _userManager.FindByIdAsync(e.UserId) : null;
				bool usuarioConfirmado = usuario != null && usuario.EmailConfirmed;

				DateTime? fechaIngreso = e.FechaIngreso == DateTime.MinValue ? null : e.FechaIngreso;
				DateTime? fechaNacimiento = e.FechaNacimiento == DateTime.MinValue ? null : e.FechaNacimiento;

				jsonEmpleados.Add(
					"{" +
						$"\"id\": {e.Id}," +
						$"\"nombre\": \"{e.Nombre}\", " +
						$"\"nombrePreferido\": \"{e.NombrePreferido}\", " +
						$"\"apellidoPaterno\": \"{e.ApellidoPaterno}\", " +
						$"\"apellidoMaterno\": \"{e.ApellidoMaterno}\", " +
						$"\"nombreCompleto\": \"{e.NombreCompleto}\", " +
						$"\"fechaIngreso\": \"{fechaIngreso:dd/MM/yyyy}\", " +
						$"\"fechaIngresoJS\": \"{fechaIngreso:yyyy-MM-dd}\", " +
						$"\"fechaNacimiento\": \"{fechaNacimiento:dd/MM/yyyy}\", " +
						$"\"fechaNacimientoJS\": \"{fechaNacimiento:yyyy-MM-dd}\", " +
						$"\"direccion\": \"{e.Direccion.Trim()}\", " +
						$"\"telefono\": \"{e.Telefono}\", " +
						$"\"email\": \"{e.Email}\", " +
						$"\"generoId\": {e.GeneroId ?? 0}, " +
						$"\"genero\": \"{nombreGenero}\", " +
						$"\"subareaId\": {e.SubareaId ?? 0}, " +
						$"\"subarea\": \"{nombreSubarea}\", " +
						$"\"oficinaId\": {e.OficinaId ?? 0}, " +
						$"\"oficina\": \"{nombreOficina}\", " +
						$"\"puestoId\": {e.PuestoId ?? 0}, " +
						$"\"puesto\": \"{nombrePuesto}\", " +
						$"\"areaId\": {e.AreaId ?? 0}, " +
						$"\"area\": \"{nombreArea}\", " +
						$"\"estadoCivilId\": {e.EstadoCivilId ?? 0}, " +
						$"\"estadoCivil\": \"{nombreEstadoCivil}\", " +
						$"\"jefeId\": {e.JefeId ?? 0}, " +
						$"\"jefe\": \"\", " +
						$"\"curp\": \"{e.CURP}\", " +
						$"\"rfc\": \"{e.RFC}\", " +
						$"\"nss\": \"{e.NSS}\", " +
						$"\"horarioId\": {e.HorarioId ?? 0}, " +
						$"\"horario\": \"{nombreHorario}\", " +
						$"\"usuarioId\": \"{e.UserId}\", " +
						$"\"usuarioValido\": \"{(usuarioConfirmado ? "1" : "0")}\", " +
						$"\"contactosEmergencia\": [], " +
						$"\"archivos\": [] " +
					"}"
				);
			}

			jsonResponse = $"[{string.Join(",", jsonEmpleados)}]";

			return jsonResponse;
		}
		private static List<string> GetListJsonContactosEmergencia(ICollection<ContactoEmergencia>? contactos)
		{
			List<string> jsonContactosEmergencia = [];
			if (contactos != null)
			{
				List<ContactoEmergencia> contacts = [.. (from c in contactos
													  orderby c.Id ascending
													  select c)];
				foreach (ContactoEmergencia c in contacts)
				{
					jsonContactosEmergencia.Add($"{{\"nombre\": \"{c.Nombre}\", \"telefono\": \"{c.Telefono}\"}}");
				}
			}
			return jsonContactosEmergencia;
		}
		private List<string> GetListJsonArchivos(ICollection<SemiArchivoEmpleado>? archivos)
		{
			List<string> jsonArchivos = [];
			if (archivos != null)
			{
				//Si el usuario ya tiene archivos, se llena el arreglo de datos a partir de ellos.					
				List<SemiArchivoEmpleado> userFiles = [.. (from userFile in archivos
												   orderby userFile.TipoArchivoId ascending
												   select userFile)];

				foreach (SemiArchivoEmpleado a in userFiles)
				{
					string htmlContainer = string.Empty;
					string imgSrc = string.Empty;
					string id = Guid.NewGuid().ToString();
					//Si el archivo tiene contenido
					if (a.FileSize >= 1)
					{
						//Asigna la información del archivo al arreglo de datos.
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
					}

					AppUser? usr = _userManager.GetUserAsync(User).Result;
					string safeL = string.Empty;
					if (usr != null)
					{
						safeL = $"userId={usr.Id}&id={a.Id}&module=gestiondetalento";
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
			}
			else
			{
				//Si el usuario no tiene archivos, se llena el arreglo de datos a partir del enum.
				foreach (FileTypes i in Enum.GetValues(typeof(FileTypes)))
				{
					string imgSrc = string.Empty;
					string htmlContainer = string.Empty;
					//Si el tipo de archivo es Imagen de perfil, entonces agrega valor default.
					if ((int)i == 0) { imgSrc = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/img/default_profile_pic.png"; }
					if ((int)i != 0) { htmlContainer = "<i class='bi bi-file-image opacity-50' style='font-size:105px'></i>"; }
					jsonArchivos.Add(
						"{" +
							$"\"id\": \"{Guid.NewGuid()}\"," +
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
			return File("/templates/PlantillaEmpleados.xlsx", MediaTypeNames.Application.Octet, "PlantillaEmpleados.xlsx");
		}

		public async Task<JsonResult> OnPostDatosAdicionalesEmpleado(int idEmpleado)
		{
			ServerResponse resp = new(true, _strLocalizer["EmpleadoConsultadoUnsuccessfully"]);
			try
			{
				resp.Datos = await GetDatosAdicionalesEmpleado(idEmpleado);
				resp.TieneError = false;
				resp.Mensaje = _strLocalizer["EmpleadoConsultadoSuccessfully"];
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}
		public async Task<JsonResult> OnPostFiltrarEmpleados()
		{
			ServerResponse resp = new(true, _strLocalizer["EmpleadosFiltradosUnsuccessfully"]);
			try
			{
				resp.Datos = await GetTalentList(InputFiltro);
				resp.TieneError = false;
				resp.Mensaje = _strLocalizer["EmpleadosFiltradosSuccessfully"];
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}

        /*public async Task<JsonResult> OnPostDisableEmpleados(string[] ids)
		{
			ServerResponse resp = new(true, _strLocalizer["EmpleadosDisabledUnsuccessfully"]);
			try
			{
				await _db.Database.BeginTransactionAsync();

                foreach (string id in ids)
                {
					int intId = Convert.ToInt32(id);
					//Se obtienen todos los empleados, incluídos los dados de baja
                    List<Empleado> empleados = await _empleadoManager.GetAllAsync(null, null, null, null, null, null, null, null, true);
					//Se obtiene el empleado que se está intentando dar de baja
                    Empleado? empleado = empleados.Where(p => p.Id == intId).FirstOrDefault();
					//Se filtran todos los empleados para obtener los que tienen asignado como jefe al empleado que se intenta dar de baja
					empleados = empleados.Where(e => e.JefeId == intId).ToList();
					//Se filtran todos los empleados para obtener los que no están dados de baja
                    List<Empleado> empleadosActivosRelacionados = empleados.Where(e => e.Deshabilitado == 0).ToList();
					//Si existen empleados que tengan el registro asignado, se le notifica al usuario.
					if (empleadosActivosRelacionados.Count > 0)
					{
                        List<string> names = [];
                        foreach (Empleado e in empleadosActivosRelacionados) { names.Add($"<i>{e.Id} - {e.NombreCompleto}</i>"); }
                        resp.TieneError = true;
                        resp.Mensaje = $"{_strLocalizer["EmpleadoIsRelated"]}<br/><br/><i>{empleado?.NombreCompleto}</i><br/><br/>{string.Join("<br/>", names)}";
                        break;
                    }
					else
					{
                        //En caso de no haber empleados con el registro asignado, procede a eliminar referencias y registro.
                        foreach (Empleado e in empleados)
                        {
                            e.JefeId = null;
                            await _empleadoManager.UpdateAsync(e);
                        }
                        //Deshabilita el empleado y por ende, el usuario relacionado.
                        await _empleadoManager.DisableByIdAsync(intId);

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["EmpleadosDisabledSuccessfully"];
                    }
                }

                if (resp.TieneError) { throw new Exception(resp.Mensaje); }

                await _db.Database.CommitTransactionAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				await _db.Database.RollbackTransactionAsync();
			}

			return new JsonResult(resp);
		}*/

        public async Task<JsonResult> OnPostDisableEmpleados(string[] ids)
        {
            ServerResponse resp = new(true, _strLocalizer["EmpleadosDisabledUnsuccessfully"]);

            try
            {
                await _db.Database.BeginTransactionAsync();

                _auditoriaContext.Activar("Gestión de Talento", "Eliminación");

                foreach (string id in ids)
                {
                    int intId = Convert.ToInt32(id);

                    List<Empleado> empleados = await _empleadoManager.GetAllAsync(null, null, null, null, null, null, null, null, true);

                    Empleado? empleado = empleados.Where(p => p.Id == intId).FirstOrDefault();

                    empleados = empleados.Where(e => e.JefeId == intId).ToList();

                    List<Empleado> empleadosActivosRelacionados = empleados
                        .Where(e => e.Deshabilitado == 0)
                        .ToList();

                    if (empleadosActivosRelacionados.Count > 0)
                    {
                        List<string> names = [];

                        foreach (Empleado e in empleadosActivosRelacionados)
                        {
                            names.Add($"<i>{e.Id} - {e.NombreCompleto}</i>");
                        }

                        resp.TieneError = true;
                        resp.Mensaje = $"{_strLocalizer["EmpleadoIsRelated"]}<br/><br/><i>{empleado?.NombreCompleto}</i><br/><br/>{string.Join("<br/>", names)}";
                        break;
                    }
                    else
                    {
                        foreach (Empleado e in empleados)
                        {
                            e.JefeId = null;
                            await _empleadoManager.UpdateAsync(e);
                        }

                        await _empleadoManager.DisableByIdAsync(intId);

                        resp.TieneError = false;
                        resp.Mensaje = _strLocalizer["EmpleadosDisabledSuccessfully"];
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

                _logger.LogError(ex.Message);
                await _db.Database.RollbackTransactionAsync();
            }

            return new JsonResult(resp);
        }

        /*public async Task<JsonResult> OnPostSaveEmpleado()
		{
			ServerResponse resp = new(true, _strLocalizer["EmpleadoSavedUnsuccessfully"]);

			//Se remueve el campo Plantilla para que no sea validado ya que no pertenece a este proceso.
			ModelState.Remove("Plantilla");

			if (!ModelState.IsValid)
			{
				resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors??[]).Select(m => m.ErrorMessage).ToArray();
				return new JsonResult(resp);
			}
			try
			{
				//Valida que no exista un empleado registrado con los mismos datos. En caso de haber, se deja el mensaje en resp.Mensajes para ser mostrado al usuario.
				string validacion = await ValidarSiExisteEmpleado(InputEmpleado, false);

				//Si la longitud del mensaje de respuesta es menor o igual a cero, se considera que no hubo errores anteriores.
				if ((validacion ?? string.Empty).Length <= 0)
				{
					//Procede a crear o actualizar el empleado.
					await CreateOrUpdateEmployee(InputEmpleado);

					resp.TieneError = false;
					resp.Mensaje = _strLocalizer["EmpleadoSavedSuccessfully"];
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

			return new JsonResult(resp);
		}*/

        public async Task<JsonResult> OnPostSaveEmpleado()
        {
            ServerResponse resp = new(true, _strLocalizer["EmpleadoSavedUnsuccessfully"]);

            ModelState.Remove("Plantilla");

            if (!ModelState.IsValid)
            {
                resp.Errores = ModelState.Keys
                    .SelectMany(k => ModelState[k]?.Errors ?? [])
                    .Select(m => m.ErrorMessage)
                    .ToArray();

                return new JsonResult(resp);
            }

            try
            {
                string validacion = await ValidarSiExisteEmpleado(InputEmpleado, false);

                if ((validacion ?? string.Empty).Length <= 0)
                {
                    string accionAuditoria = InputEmpleado.Id >= 1
                        ? "Edición"
                        : "Alta";

                    _auditoriaContext.Activar("Gestión de Talento", accionAuditoria);

                    await CreateOrUpdateEmployee(InputEmpleado);

                    _auditoriaContext.Desactivar();

                    resp.TieneError = false;
                    resp.Mensaje = _strLocalizer["EmpleadoSavedSuccessfully"];
                }
                else
                {
                    resp.Mensaje = validacion;
                }
            }
            catch (Exception ex)
            {
                _auditoriaContext.Desactivar();
                _logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        private async Task<string> ValidarSiExisteEmpleado(EmpleadoModel emp, bool curpAsKey)
		{
			List<Empleado> coincidences = [];
			List<Empleado> emps = await _empleadoManager.GetAllAsync();

			if (curpAsKey)
			{
				//Excluyo al empleado con el CURP actual.
				emps = emps.Where(e => e.CURP != emp.CURP).ToList();
			}
			else
			{
				//Excluyo al empleado con el Id actual.
				emps = emps.Where(e => e.Id != emp.Id).ToList();
			}

            //Valido que no exista empleado que tenga los mismos datos.
            coincidences = emps.Where(e => e.Deshabilitado == 0 && (e.NombreCompleto ?? "").Length >= 1 && e.NombreCompleto == $"{emp.Nombre} {emp.ApellidoPaterno} {emp.ApellidoMaterno}").ToList();
            if (coincidences.Count >= 1) { return $"{_strLocalizer["ErrorEmpleadoExistenteA"]} {_strLocalizer["Nombre"]} {emp.Nombre} {emp.ApellidoPaterno} {emp.ApellidoMaterno}. {_strLocalizer["ErrorEmpleadoExistenteB"]}."; }

            coincidences = emps.Where(e => e.Deshabilitado == 0 && (e.Email ?? "").Length >= 1 && e.Email == emp.Email).ToList();
            if (coincidences.Count >= 1) { return $"{_strLocalizer["ErrorEmpleadoExistenteA"]} {_strLocalizer["Correo"]} {emp.Email}. {_strLocalizer["ErrorEmpleadoExistenteB"]}."; }

            coincidences = emps.Where(e => e.Deshabilitado == 0 && (e.CURP ?? "").Length >= 1 && e.CURP == emp.CURP).ToList();
            if (coincidences.Count >= 1) { return $"{_strLocalizer["ErrorEmpleadoExistenteA"]} {_strLocalizer["CURP"]} {emp.CURP}. {_strLocalizer["ErrorEmpleadoExistenteB"]}."; }

            coincidences = emps.Where(e => e.Deshabilitado == 0 && (e.RFC ?? "").Length >= 1 && e.RFC == emp.RFC).ToList();
            if (coincidences.Count >= 1) { return $"{_strLocalizer["ErrorEmpleadoExistenteA"]} {_strLocalizer["RFC"]} {emp.RFC}. {_strLocalizer["ErrorEmpleadoExistenteB"]}."; }

            coincidences = emps.Where(e => e.Deshabilitado == 0 && (e.NSS ?? "").Length >= 1 && e.NSS == emp.NSS).ToList();
            if (coincidences.Count >= 1) { return $"{_strLocalizer["ErrorEmpleadoExistenteA"]} {_strLocalizer["NSS"]} {emp.NSS}. {_strLocalizer["ErrorEmpleadoExistenteB"]}."; }

            return string.Empty;
		}
		private async Task CreateOrUpdateEmployee(EmpleadoModel e)
		{
			try
			{
				await _db.Database.BeginTransactionAsync();

				int idEmpleado = 0;

				//Se busca empleado por id
				Empleado? empleado = await _empleadoManager.GetByIdAsync(e.Id);
				//Si no se encontró empleado por id, se busca el empleado por su CURP. 
				empleado ??= await _empleadoManager.GetByCURPAsync(e.CURP ?? string.Empty);

				//Si se encontró empleado, obtiene su Id del registro existente. De lo contrario, se crea uno nuevo.
				if (empleado != null) { idEmpleado = empleado.Id; } else { empleado = new Empleado(); }

				//Llena los datos del empleado.
				empleado.ApellidoMaterno = e.ApellidoMaterno;
				empleado.ApellidoPaterno = e.ApellidoPaterno;
				empleado.AreaId = e.AreaId;
				empleado.Direccion = e.Direccion ?? string.Empty;
				empleado.Email = e.Email;
				empleado.EstadoCivilId = e.EstadoCivilId;
				empleado.FechaIngreso = e.FechaIngreso;
				empleado.FechaNacimiento = e.FechaNacimiento;
				empleado.GeneroId = e.GeneroId;
				empleado.JefeId = e.JefeId;
				empleado.NombreCompleto = $"{e.Nombre} {e.ApellidoPaterno} {e.ApellidoMaterno}";
				empleado.OficinaId = e.OficinaId;
				empleado.Nombre = e.Nombre;
				empleado.NombrePreferido = e.NombrePreferido ?? string.Empty;
				empleado.PuestoId = e.PuestoId;
				empleado.SubareaId = e.SubareaId;
				empleado.Telefono = e.Telefono;
				empleado.CURP = e.CURP ?? string.Empty;
				empleado.RFC = e.RFC ?? string.Empty;
				empleado.NSS = e.NSS ?? string.Empty;
				empleado.HorarioId = e.HorarioId >= 1 ? e.HorarioId : null;
				empleado.CalcularAsistencia = e.HorarioId >= 1;

                if (idEmpleado >= 1)
				{
					//Si el empleado ya existía, lo actualiza.
					await _empleadoManager.UpdateAsync(empleado);

					//Elimina los contactos del empleado.
					await _contactoEmergenciaManager.DeleteByEmpleadoIdAsync(idEmpleado);

                    //Elimina los archivos del usuario que requieran actualizarse.
                    foreach (ArchivoModel? a in e.Archivos) { await _archivoEmpleadoManager.DeleteByIdAsync(a?.Id ?? string.Empty); }
                }
				else
				{
                    //De lo contrario, crea al empleado y obtiene su id.
                    idEmpleado = await _empleadoManager.CreateAsync(empleado);
				}

				//Crea dos nuevos contactos para el empleado.
				await _contactoEmergenciaManager.CreateAsync(
					new ContactoEmergencia() { Nombre = e.NombreContacto1 ?? string.Empty, Telefono = e.TelefonoContacto1 ?? string.Empty, EmpleadoId = idEmpleado }
				);
				await _contactoEmergenciaManager.CreateAsync(
					new ContactoEmergencia() { Nombre = e.NombreContacto2 ?? string.Empty, Telefono = e.TelefonoContacto2 ?? string.Empty, EmpleadoId = idEmpleado }
				);

                //Crea los archivos del usuario
                foreach (ArchivoModel? a in e.Archivos)
                {
                    if (a == null) { continue; }

                    //Se usa la info para guardar el archivo.
                    await _archivoEmpleadoManager.CreateAsync(
						new ArchivoEmpleado()
                        {
                            Archivo = (a.ImgSrc ?? string.Empty).Length >= 1 ? Convert.FromBase64String(a.ImgSrc ?? string.Empty) : [],
                            EmpleadoId = idEmpleado,
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
		}

		public async Task<JsonResult> OnPostImportarEmpleados()
		{
			ServerResponse resp = new(true, _strLocalizer["EmpleadosImportadosUnsuccessfully"]);
			try
			{
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
							resp.Mensaje = _strLocalizer["EmpleadosImportadosSuccessfully"];
							continue;
						}

						string vmsg = await CreateEmployeeFromExcelRow(row);

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
							resp.Mensaje = _strLocalizer["EmpleadosImportadosSuccessfully"];
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				resp.TieneError = true;
				resp.Mensaje = _strLocalizer["EmpleadosImportadosUnsuccessfully"];
			}

			return new JsonResult(resp);
		}
		private async Task<string> CreateEmployeeFromExcelRow(DataRow row)
		{
			Genero? genero = await _generoManager.GetByNameAsync(row[5].ToString()?.Trim() ?? string.Empty);
			EstadoCivil? estadoCivil = await _estadoCivilManager.GetByNameAsync(row[6].ToString()?.Trim() ?? string.Empty);
			Puesto? puesto = await _puestoManager.GetByNameAsync(row[8].ToString()?.Trim() ?? string.Empty);
			Area? area = await _areaManager.GetByNameAsync(row[9].ToString()?.Trim() ?? string.Empty);
			Subarea? subarea = await _subareaManager.GetByNameAsync(row[10].ToString()?.Trim() ?? string.Empty);
			Oficina? oficina = await _oficinaManager.GetByNameAsync(row[11].ToString()?.Trim() ?? string.Empty);
			Empleado? jefe = await _empleadoManager.GetByNameAsync(row[12].ToString()?.Trim() ?? string.Empty);

			_ = DateTime.TryParse(row[3].ToString(), out DateTime fn);
			_ = DateTime.TryParse(row[13].ToString(), out DateTime fi);

			EmpleadoModel e = new() {
				Nombre = row[0].ToString()?.Trim() ?? string.Empty,
				ApellidoPaterno = row[1].ToString()?.Trim() ?? string.Empty,
				ApellidoMaterno = row[2].ToString()?.Trim() ?? string.Empty,
				FechaNacimiento = fn,
				Telefono = row[4].ToString()?.Trim() ?? string.Empty,
				GeneroId = genero?.Id,
				EstadoCivilId = estadoCivil?.Id,
				Direccion = row[7].ToString()?.Trim() ?? string.Empty,
				PuestoId = puesto != null ? puesto.Id : 0,
				AreaId = area != null ? area.Id : 0,
				SubareaId = subarea?.Id,
				OficinaId = oficina?.Id,
				JefeId = jefe?.Id,
				FechaIngreso = fi,
				Email = row[14].ToString()?.Trim() ?? string.Empty,
				NombreContacto1 = row[15].ToString()?.Trim() ?? string.Empty,
				TelefonoContacto1 = row[16].ToString()?.Trim() ?? string.Empty,
				NombreContacto2 = row[17].ToString()?.Trim() ?? string.Empty,
				TelefonoContacto2 = row[18].ToString()?.Trim() ?? string.Empty,
				CURP = row[19].ToString()?.Trim() ?? string.Empty,
				RFC = row[20].ToString()?.Trim() ?? string.Empty,
				NSS = row[21].ToString()?.Trim() ?? string.Empty,
				HorarioId = 1,
				NombrePreferido = row[22].ToString()?.Trim() ?? string.Empty,
			};

			List<ArchivoModel> archivos = [];
			//Crea los archivos del usuario.
			foreach (FileTypes i in Enum.GetValues(typeof(FileTypes)))
			{
				archivos.Add(new ArchivoModel() { Extension = "", ImgSrc = "", Nombre = "", TipoArchivoId = (int)i });
			}

			e.Archivos = [.. archivos];

			//Valida que no exista un empleado registrado con los mismos datos. En caso de haber, se deja el mensaje en resp.Mensajes para ser mostrado al usuario.
			string validationMsg = await ValidarSiExisteEmpleado(e, true);

			//Si la longitud del mensaje de respuesta es menor o igual a cero, se considera que no hubo errores anteriores.
			if ((validationMsg ?? "").Length <= 0)
			{
				//Procede a crear o actualizar el empleado.
				await CreateOrUpdateEmployee(e);
			}

			return validationMsg ?? "";
		}

		private AppUser CreateUser()
		{
			try
			{
				return Activator.CreateInstance<AppUser>();
			}
			catch
			{
				throw new InvalidOperationException($"No se puede crear una instancia de '{nameof(AppUser)}'. " +
					$"Asegurese que '{nameof(AppUser)}' no es una clase abstracta y tiene un constructor sin parámetros, o alternativamente " +
					$"sobrecargue la página de registro en /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}
        public async Task<IActionResult> OnPostInvitarEmpleado(int id)
        {
            ServerResponse resp = new(true, _strLocalizer["EmpleadoInvitadoUnsuccessfully"]);

            try
            {
                Empleado? emp = await _empleadoManager.GetByIdAsync(id);
                if (emp == null)
                {
                    resp.Mensaje = "No se encontró el empleado.";
                    _auditoriaContext.Desactivar();
                    return new JsonResult(resp);
                }

                string password = _userManager.GenerateRandomPassword(10);
                AppUser? user = null;

                if (string.IsNullOrWhiteSpace(emp.UserId))
                {
                    user = CreateUser();
                    user.Email = emp.Email;
                    user.EmpleadoId = emp.Id;
                    user.NormalizedEmail = emp.Email.ToUpper();
                    user.NormalizedUserName = emp.Email.ToUpper();
                    user.PasswordResetNeeded = true;
                    user.IsPreregisterAuthorized = true;
                    user.PhoneNumber = emp.Telefono;
                    user.UserName = emp.Email;

                    await _userStore.SetUserNameAsync(user, emp.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, emp.Email, CancellationToken.None);

                    var result = await _userManager.CreateAsync(user, password);

                    if (!result.Succeeded)
                    {
                        resp.Mensaje = string.Join(" | ", result.Errors.Select(e => e.Description));
                        _auditoriaContext.Desactivar();
                        return new JsonResult(resp);
                    }

                    var roleResult = await _userManager.AddToRoleAsync(user, ServicesConfiguration.RolUsuario);
                    if (!roleResult.Succeeded)
                    {
                        resp.Mensaje = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
                        _auditoriaContext.Desactivar();
                        return new JsonResult(resp);
                    }

                    emp.UserId = user.Id;
                    _auditoriaContext.Activar("Gestión de Talento", "Invitación");
                    await _empleadoManager.UpdateAsync(emp);
                }
                else
                {
                    user = await _userManager.FindByIdAsync(emp.UserId ?? "");
                    if (user == null)
                    {
                        resp.Mensaje = "No se encontró el usuario relacionado al empleado.";
                        _auditoriaContext.Desactivar();
                        return new JsonResult(resp);
                    }

                    var removePwd = await _userManager.RemovePasswordAsync(user);
                    if (!removePwd.Succeeded)
                    {
                        resp.Mensaje = string.Join(" | ", removePwd.Errors.Select(e => e.Description));
                        _auditoriaContext.Desactivar();
                        return new JsonResult(resp);
                    }

                    var addPwd = await _userManager.AddPasswordAsync(user, password);
                    if (!addPwd.Succeeded)
                    {
                        resp.Mensaje = string.Join(" | ", addPwd.Errors.Select(e => e.Description));
                        _auditoriaContext.Desactivar();
                        return new JsonResult(resp);
                    }
                }

                string userCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                userCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userCode));

                string userURL = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = userCode },
                    protocol: Request.Scheme) ?? string.Empty;

                string emailBody =
                    $"{_strLocalizer["EmailBodyFP"]} <a href='{userURL}'>{_strLocalizer["EmailBodySP"]}</a>" +
                    $"<br /><br />" +
                    $"{_strLocalizer["EmailBodyTP"]}" +
                    $"<br /><br />" +
                    $"User: {emp.Email}" +
                    $"<br />" +
                    $"Password: {password}" +
                    $"<br /><br />" +
                    $"{_strLocalizer["EmailBodyFOP"]}";

                await _emailSender.SendEmailAsync(emp.Email, _strLocalizer["EmailSubject"], emailBody);

                resp.TieneError = false;
                resp.Mensaje = _strLocalizer["EmpleadoInvitadoSuccessfully"];
                _auditoriaContext.Desactivar();
                return new JsonResult(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al invitar empleado");
                resp.Mensaje = ex.ToString();
                _auditoriaContext.Desactivar();
                return new JsonResult(resp);
            }
        }

        public async Task<JsonResult> OnPostGetEmpleadosSuggestion(string texto)
		{
			ServerResponse resp = new(true, _strLocalizer["ConsultadoUnsuccessfully"]);
			try
			{
				resp.Datos = await GetEmpleadosSuggestion(texto);
				resp.TieneError = false;
				resp.Mensaje = _strLocalizer["ConsultadoSuccessfully"];
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
			}

			return new JsonResult(resp);
		}
		private async Task<string> GetEmpleadosSuggestion(string texto)
		{
			string jsonResponse;
			List<string> jsons = [];

			List<Empleado> empleados = await _empleadoManager.GetAllAsync();
			empleados = empleados.Where(e => e.NombreCompleto.ToLowerInvariant().Contains(texto.ToLowerInvariant())).Take(20).ToList();

			if (empleados != null)
			{
				foreach (Empleado e in empleados)
				{
					jsons.Add($"{{" +
									$"\"id\": {e.Id}, " +
									$"\"value\": \"{e.NombreCompleto}\", " +
									$"\"label\": \"{e.NombreCompleto}\" " +
								$"}}");
				}
			}

			jsonResponse = $"[{string.Join(",", jsons)}]";

			return jsonResponse;
		}
	}
}