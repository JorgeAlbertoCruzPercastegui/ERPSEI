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
using static ERPSEI.Areas.Reportes.Pages.DocumentacionModel;


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
            public int? AreaId { get; set; }
            public int? TipoDocumentoId { get; set; }
            public int? EstatusDocumentoId { get; set; }

            /*[DataType(AnnoDataType.Text)]
            public string? Area { get; set; }

            [DataType(AnnoDataType.Text)]
            public string? TipoDocumento { get; set; }

            [DataType(AnnoDataType.Text)]
            public string? EstatusDocumento { get; set; }*/

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

            public int? Activo { get; set; } = 1;

            [DataType(AnnoDataType.Text)]
            public string? Ubicacion { get; set; }

            public IFormFile? Archivo { get; set; }

            [DataType(AnnoDataType.Text)]
            public string? Observaciones { get; set; }
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

            // Solo activos
            documentos = documentos.Where(d => d.Activo).ToList();

            var jsonActivos = new List<object>();

            foreach (var d in documentos)
            {
                DateTime? fechaCreacion = d.FechaCreacion == DateTime.MinValue ? null : d.FechaCreacion;
                DateTime? fechaModificacion = d.FechaModificacion == DateTime.MinValue ? null : d.FechaModificacion;

                jsonActivos.Add(new
                {
                    id = d.Id,
                    areaId = d.AreaId,
                    area = d.Area != null ? d.Area.Nombre : "-",
                    tipoDocumentoId = d.TipoDocumentoId,
                    tipoDocumento = d.TipoDocumento != null ? d.TipoDocumento.Nombre : "-",
                    estatusDocumentoId = d.EstatusDocumentoId,
                    estatusDocumento = d.EstatusDocumento != null ? d.EstatusDocumento.Nombre : "-",
                    titulo = d.Titulo ?? "-",
                    descripcion = d.Descripcion ?? "-",
                    responsable = d.Responsable ?? "-",
                    ubicacion = d.Ubicacion ?? "-",
                    observaciones = d.Observaciones ?? "-",
                    nombreArchivo = d.NombreArchivo ?? "-",
                    rutaArchivo = d.RutaArchivo ?? "-",
                    activo = d.Activo,
                    fechaCreacion,
                    fechaModificacion,
                    creadoPorId = d.CreadoPorId,
                    modificadoPorId = d.ModificadoPorId
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

        public async Task<JsonResult> OnPostSaveDocumento(DcoumentacionTableModel input)
        {
            ServerResponse resp = new(true, "No se pudo guardar el documento.");

            try
            {
                await db.Database.BeginTransactionAsync();

                if (input == null)
                {
                    resp.Mensaje = "No se recibieron datos para guardar.";
                    return new JsonResult(resp);
                }

                if (string.IsNullOrWhiteSpace(input.Titulo))
                {
                    resp.Mensaje = "El título es obligatorio.";
                    return new JsonResult(resp);
                }

                if (!input.AreaId.HasValue || input.AreaId.Value <= 0)
                {
                    resp.Mensaje = "El área es obligatoria.";
                    return new JsonResult(resp);
                }

                if (!input.TipoDocumentoId.HasValue || input.TipoDocumentoId.Value <= 0)
                {
                    resp.Mensaje = "El tipo de documento es obligatorio.";
                    return new JsonResult(resp);
                }

                if (!input.EstatusDocumentoId.HasValue || input.EstatusDocumentoId.Value <= 0)
                {
                    resp.Mensaje = "El estatus del documento es obligatorio.";
                    return new JsonResult(resp);
                }

                if (!await db.Areas.AnyAsync(a => a.Id == input.AreaId.Value))
                {
                    resp.Mensaje = "El área seleccionada no existe en el catálogo.";
                    return new JsonResult(resp);
                }

                if (!await db.TiposDocumento.AnyAsync(t => t.Id == input.TipoDocumentoId.Value))
                {
                    resp.Mensaje = "El tipo de documento seleccionado no existe en el catálogo.";
                    return new JsonResult(resp);
                }

                if (!await db.DocumentosEstatus.AnyAsync(e => e.Id == input.EstatusDocumentoId.Value))
                {
                    resp.Mensaje = "El estatus seleccionado no existe en el catálogo.";
                    return new JsonResult(resp);
                }

                bool esNuevo = !input.Id.HasValue || input.Id.Value == 0;

                Documento doc;

                if (esNuevo)
                {
                    doc = new Documento
                    {
                        FechaCreacion = DateTime.Now,
                        Activo = true
                    };

                    // Auditoría (si realmente manejas userId string, cambia tu TableModel a string)
                    doc.CreadoPorId = input.CreadoPorId?.ToString();

                    db.Documentos.Add(doc);
                }
                else
                {
                    doc = await db.Documentos.FirstOrDefaultAsync(d => d.Id == input.Id.Value);
                    if (doc == null)
                    {
                        resp.Mensaje = "El documento no fue encontrado.";
                        return new JsonResult(resp);
                    }

                    doc.ModificadoPorId = input.ModificadoPorId?.ToString();
                    doc.FechaModificacion = DateTime.Now;
                }

                // ✅ Asignación de FKs (directo por ID)
                doc.AreaId = input.AreaId.Value;
                doc.TipoDocumentoId = input.TipoDocumentoId.Value;
                doc.EstatusDocumentoId = input.EstatusDocumentoId.Value;

                // ✅ Asignación de valores
                doc.Titulo = input.Titulo.Trim();
                doc.Descripcion = string.IsNullOrWhiteSpace(input.Descripcion) ? null : input.Descripcion.Trim();
                doc.Responsable = string.IsNullOrWhiteSpace(input.Responsable) ? null : input.Responsable.Trim();
                doc.Ubicacion = string.IsNullOrWhiteSpace(input.Ubicacion) ? null : input.Ubicacion.Trim();
                doc.Observaciones = string.IsNullOrWhiteSpace(input.Observaciones) ? null : input.Observaciones.Trim();

                // ✅ Activo (si lo mandas; si no, respeta lo que ya tenga)
                if (input.Activo.HasValue)
                    doc.Activo = input.Activo.Value == 1;

                // ✅ Archivo (si viene)
                if (input.Archivo != null && input.Archivo.Length > 0)
                {
                    // Validación básica (PDF)
                    var ext = Path.GetExtension(input.Archivo.FileName);
                    if (!string.Equals(ext, ".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        resp.Mensaje = "Solo se permiten archivos PDF.";
                        return new JsonResult(resp);
                    }

                    var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "documentos");
                    if (!Directory.Exists(uploadsRoot))
                        Directory.CreateDirectory(uploadsRoot);

                    var originalName = Path.GetFileName(input.Archivo.FileName);
                    var safeFileName = $"{Guid.NewGuid():N}{ext}";
                    var fullPath = Path.Combine(uploadsRoot, safeFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await input.Archivo.CopyToAsync(stream);
                    }

                    doc.NombreArchivo = originalName;
                    doc.RutaArchivo = $"/documentos/{safeFileName}";
                }

                await db.SaveChangesAsync();
                await db.Database.CommitTransactionAsync();

                resp.TieneError = false;
                resp.Mensaje = esNuevo ? "Documento creado correctamente." : "Documento actualizado correctamente.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al guardar el documento.");
                await db.Database.RollbackTransactionAsync();

                resp.TieneError = true;
                resp.Mensaje = "Ocurrió un error al guardar el documento.";
            }

            return new JsonResult(resp);
        }






    }
}
