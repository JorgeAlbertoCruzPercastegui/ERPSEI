using ERPSEI.Data;
using ERPSEI.Data.Entities.Documentos;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.SAT;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.AdministradorPolizas;
using ERPSEI.Data.Managers.Conciliaciones;
using ERPSEI.Data.Managers.Cuentas;
using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.Polizas;
using ERPSEI.Data.Managers.SAT;
using ERPSEI.Data.Managers.SAT.cfdiv40;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Email;
using ERPSEI.Pages.Shared;
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
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using ERPSEI.Requests;
using OfficeOpenXml;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using iText.Layout;
using MathNet.Numerics.Distributions;
using ERPSEI.Areas.ERP.Pages;
using ERPSEI.Data.Managers.Documentos;
using ERPSEI.Data.Managers.ActivosFijos;
using static ERPSEI.Areas.ERP.Pages.ActivosFijosModel;
using DocumentFormat.OpenXml.Wordprocessing;
using AnnoDataType = System.ComponentModel.DataAnnotations.DataType;


namespace ERPSEI.Areas.Reportes.Pages
{
    public class DocumentacionModel : ERPPageModel
    {
        private readonly IStringLocalizer<DocumentacionModel> stringLocalizer;
        private readonly ILogger<DocumentacionModel> logger;
        private readonly AppUserManager appUserManager;
        //private readonly IActivoFijoManager activoFijoManager;
        //private readonly ICategoriaActivosFijosManager categoriaActivoFijoManager;
        //private readonly ITipoActivosFijosManager tipoActivoFijoManager;
        private readonly IEmpleadoManager empleadoDocumentoManager;
        private readonly IDocumentoManager documentoManager;
        private readonly IEstatusDocumentoManager estatusDocumentoManager;
        private readonly ITipoDocumentoManager tipoDocumentoManager;
        //private readonly IOficinaManager oficinaActivoFijoManager;
        private readonly IStringLocalizer<DocumentacionModel> localizer;
        private readonly AppUserManager userManager;

        private readonly Data.ApplicationDbContext db;

        [BindProperty]
        public Documento? DocumentosList { get; set; }

        [BindProperty]
        public DcoumentacionTableModel InputDocumentos { get; set; }

        public class DcoumentacionTableModel
        {
            public int? Id { get; set; }

            [StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Titulo { get; set; }

            [DataType(AnnoDataType.Text)]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Descripcion { get; set; } = string.Empty;

            [DataType(AnnoDataType.Text)]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            public string? Responsable { get; set; }

            public int? CreadoPorId { get; set; }
            public int? ModificadoPorId { get; set; }

            [Display(Name = "Fecha Creación")]
            [DataType(AnnoDataType.Date)]
            public DateTime? FechaCreacion { get; set; }

            [Display(Name = "Fecha Modificación")]
            [DataType(AnnoDataType.Date)]
            public DateTime? FechaModificacion { get; set; }

            public int? Activo { get; set; } = 0;
        }

        [BindProperty]
        public InputFiltroModel? InputFiltro { get; set; }

    public class InputFiltroModel
    {
        // Búsqueda por título / texto
        [DataType(AnnoDataType.Text)]
        [StringLength(250, ErrorMessage = "FieldLength", MinimumLength = 3)]
        [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
        public string? Titulo { get; set; } = string.Empty;

        // Área (catálogo existente)
        [Display(Name = "Área")]
        public int? AreaId { get; set; } // 0 o null = todos

        // TipoDocumento (Manuales, Procedimientos, etc.)
        [Display(Name = "Tipo Documento")]
        public int? TipoDocumentoId { get; set; } // 0 o null = todos

        // EstatusDocumento (Vigente, Obsoleto, En Revisión)
        [Display(Name = "Estatus Documento")]
        public int? EstatusDocumentoId { get; set; } // 0 o null = todos

        // Palabra clave (tabla DocumentoPalabrasClave)
        [Display(Name = "Palabra clave")]
        [DataType(AnnoDataType.Text)]
        [StringLength(80, ErrorMessage = "FieldLength", MinimumLength = 2)]
        [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
        public string? PalabraClave { get; set; } = string.Empty;

        // Rango de fechas (por FechaCreacion de Documento)
        [Display(Name = "Fecha Creación Inicio")]
        [DataType(AnnoDataType.Date)]
        public DateTime? FechaCreacionInicio { get; set; }

        [Display(Name = "Fecha Creación Fin")]
        [DataType(AnnoDataType.Date)]
        public DateTime? FechaCreacionFin { get; set; }
    }

        [BindProperty]
        public EstatusDocumentoFiltroModel? InputFiltroEstatusDocumento { get; set; }
        public class EstatusDocumentoFiltroModel
        {
            public int? Id { get; set; }

            [StringLength(80)]
            public string? Nombre { get; set; }

            public bool? Activo { get; set; }

            public bool? EsPublicable { get; set; }
        }

        [BindProperty]
        public TipoDocumentoFiltroModel? InputFiltroTipoDocumento { get; set; }
        public class TipoDocumentoFiltroModel
        {
            public int? Id { get; set; }

            [StringLength(150)]
            public string? Nombre { get; set; }

            public bool? Activo { get; set; }
        }

        public DocumentacionModel
        (
        IStringLocalizer<DocumentacionModel> _stringLocalizer,
        ILogger<DocumentacionModel> _logger,
        AppUserManager _appUserManager,
        IStringLocalizer<DocumentacionModel> _localizer,
        Data.ApplicationDbContext _db,
        IDocumentoManager _documentoManager,
        IEmpleadoManager empleadoManager,
        IEstatusDocumentoManager _estatusDocumentoManager,
        ITipoDocumentoManager _tipoDocumentoManager,
        AppUserManager _userManager
        )
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            appUserManager = _appUserManager;
            localizer = _localizer;
            db = _db;

            documentoManager = _documentoManager;
            empleadoDocumentoManager = empleadoManager;
            estatusDocumentoManager = _estatusDocumentoManager;
            tipoDocumentoManager = _tipoDocumentoManager;
            userManager = _userManager;

            InputFiltro = new InputFiltroModel();
            InputFiltroEstatusDocumento = new EstatusDocumentoFiltroModel();
            InputFiltroTipoDocumento = new TipoDocumentoFiltroModel();
            InputDocumentos = new DcoumentacionTableModel();

        }

        //Método para listar en json todos los documentos

        public async Task<JsonResult> OnGetDocumentosList()
        {
            var documentos = await documentoManager.GetAllAsync();
            documentos = documentos.Where(a => a.Activo == true).ToList();


            var jsonActivos = new List<object>();

            foreach (var a in documentos)
            {
                DateTime? fechaCreacion = a.FechaCreacion == DateTime.MinValue ? null : a.FechaCreacion;
                DateTime? fechaModificacion = a.FechaModificacion == DateTime.MinValue ? null : a.FechaModificacion;

                jsonActivos.Add(new
                {
                    id = a.Id,
                    areaId = a.AreaId,
                    area = a.Area != null ? a.Area.Nombre : "-",
                    tipoDocumentoId = a.TipoDocumentoId,
                    tipoDocumento = a.TipoDocumento != null ? a.TipoDocumento?.Nombre : "-",
                    titulo = a.Titulo ?? "-",
                    descripcion = a.Descripcion ?? "-",
                    activo = a.Activo,
                    creadoPorId = a.CreadoPorId,
                    modificadoPorId = a.ModificadoPorId
                });
            }

            return new JsonResult(jsonActivos);
        }

        public async Task<JsonResult> OnPostDeleteDocumentacion(string[] ids)
        {
            //ServerResponse resp = new(true, "No se pudieron dar de baja los registros.");
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);

            try
            {
                await db.Database.BeginTransactionAsync();

                foreach (string id in ids)
                {
                    if (!int.TryParse(id, out int intId))
                        continue;

                    var doc = await db.Documentos.FirstOrDefaultAsync(a => a.Id == intId);

                    if (doc == null)
                        continue;

                    doc.Activo = false;

                    // Guardar cambios por cada uno
                    db.Documentos.Update(doc);
                }

                await db.SaveChangesAsync();
                await db.Database.CommitTransactionAsync();

                resp.TieneError = false;
                //resp.Mensaje = "Registros dados de baja correctamente.";
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
                logger.LogError(ex, "Error al dar de baja activos fijos");
                resp.Mensaje = "Ocurrió un error al dar de baja los registros.";
            }

            return new JsonResult(resp);
        }


    }
}
