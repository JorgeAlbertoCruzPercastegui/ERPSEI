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
using DocumentFormat.OpenXml.Spreadsheet;
using System.Security.Claims;



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
        public DcoumentacionVersionTableModel InputDocumentosVersion { get; set; }

        public class DcoumentacionVersionTableModel
        {
            public int? Id { get; set; }

            public int? DocumentoId { get; set; }

            [Required]
            [StringLength(20)]
            public string Version { get; set; } = "1.0";

            [Required]
            public int EstatusDocumentoId { get; set; }

            [DataType(AnnoDataType.Date)]
            public DateTime? FechaPublicacion { get; set; }

            [StringLength(1000)]
            public string? Comentarios { get; set; }

            public IFormFile? Archivo { get; set; }

            public string? NombreArchivo { get; set; }
            public string? RutaArchivo { get; set; }
            public string? MimeType { get; set; }
            public long? TamanoBytes { get; set; }

            public bool EsActual { get; set; } = true;
            public bool Activo { get; set; } = true;

            public string? CreadoPorId { get; set; }

            [DataType(AnnoDataType.Date)]
            public DateTime FechaCreacion { get; set; } = DateTime.Now;
        }


        [BindProperty]
        public DcoumentacionTableModel InputDocumentos { get; set; }

        public class DcoumentacionTableModel
        {
            public int? Id { get; set; }
            public int? AreaId { get; set; }
            public int? TipoDocumentoId { get; set; }
            public int? EstatusDocumentoId { get; set; }

            [StringLength(100, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Titulo { get; set; }

            [DataType(AnnoDataType.Text)]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            [Display(Name = "Conexión")]
            public string? Descripcion { get; set; } = string.Empty;

            [DataType(AnnoDataType.Text)]
            //[StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
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
            public string? NombreArchivo { get; set; }

            [Display(Name = "Código")]
            [DataType(AnnoDataType.Text)]
            public string? Ubicacion { get; set; }

            public IFormFile? Archivo { get; set; }

            [DataType(AnnoDataType.Text)]
            public string? Observaciones { get; set; }

            [DataType(AnnoDataType.Text)]
            public string? RutaArchivo { get; set; }
        }

        [BindProperty]
        public InputFiltroModel? InputFiltro { get; set; }

    public class InputFiltroModel
    {
        [DataType(AnnoDataType.Text)]
        [StringLength(250, ErrorMessage = "FieldLength", MinimumLength = 3)]
        [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
        public string? Titulo { get; set; } = string.Empty;

        [Display(Name = "Área")]
        public int? AreaId { get; set; }

        [Display(Name = "Tipo Documento")]
        public int? TipoDocumentoId { get; set; } 

        [Display(Name = "Estatus Documento")]
        public int? EstatusDocumentoId { get; set; }

        [Display(Name = "Palabra clave")]
        [DataType(AnnoDataType.Text)]
        [StringLength(80, ErrorMessage = "FieldLength", MinimumLength = 2)]
        [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
        public string? PalabraClave { get; set; } = string.Empty;

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

        public enum RolAutorizacion
        {
            DIRECTIVOS = 1,
            REINGENIERIA = 2,
            GERENTE = 3
        }

        public enum EstadoAutorizacion
        {
            NA = 0,
            PENDIENTE = 1,
            APROBADO = 2,
            RECHAZADO = 3
        }

        public class AutorizacionesDto
        {
            public string Resumen { get; set; } = "PENDIENTE";
            public AutorizacionItemDto Directivos { get; set; } = new();
            public AutorizacionItemDto Reingenieria { get; set; } = new();
            public AutorizacionItemDto Gerente { get; set; } = new();
            public PermisosAutorizacionDto Permisos { get; set; } = new();
        }

        public class AutorizacionItemDto
        {
            public string Estado { get; set; } = "PENDIENTE";
            public string? Por { get; set; }
            public string? Fecha { get; set; }
            public string? Comentario { get; set; }
        }

        public class PermisosAutorizacionDto
        {
            public bool PuedeAutorizarDirectivos { get; set; }
            public bool PuedeAutorizarReingenieria { get; set; }
            public bool PuedeAutorizarGerente { get; set; }
        }

        public class AutorizarRequest
        {
            public int DocumentoId { get; set; }
            public string Rol { get; set; } = "";
            public bool Aprobar { get; set; }
            public string? Comentario { get; set; }
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

        /*public async Task<JsonResult> OnPostSaveDocumento(DcoumentacionTableModel input)
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
                        FechaModificacion = null,
                        Activo = true
                    };

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

                doc.AreaId = input.AreaId.Value;
                doc.TipoDocumentoId = input.TipoDocumentoId.Value;
                doc.EstatusDocumentoId = input.EstatusDocumentoId.Value;

                doc.Titulo = input.Titulo.Trim();
                doc.Descripcion = string.IsNullOrWhiteSpace(input.Descripcion) ? null : input.Descripcion.Trim();
                doc.Responsable = string.IsNullOrWhiteSpace(input.Responsable) ? null : input.Responsable.Trim();
                doc.Observaciones = string.IsNullOrWhiteSpace(input.Observaciones) ? null : input.Observaciones.Trim();

                doc.NombreArchivo = string.IsNullOrWhiteSpace(input.NombreArchivo) ? null : input.NombreArchivo.Trim();
                doc.RutaArchivo = string.IsNullOrWhiteSpace(input.RutaArchivo) ? null : input.RutaArchivo.Trim();
                doc.Ubicacion = string.IsNullOrWhiteSpace(input.Ubicacion) ? null : input.Ubicacion.Trim();

                if (input.Activo.HasValue)
                    doc.Activo = input.Activo.Value == 1;

                if (input.Archivo != null && input.Archivo.Length > 0)
                {
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

                    if (string.IsNullOrWhiteSpace(doc.NombreArchivo))
                        doc.NombreArchivo = originalName;

                    if (string.IsNullOrWhiteSpace(doc.RutaArchivo))
                        doc.RutaArchivo = $"/documentos/{safeFileName}";
                }

                await db.SaveChangesAsync();
                await db.Database.CommitTransactionAsync();

                resp.TieneError = false;
                resp.Mensaje = esNuevo ? "Documento creado correctamente." : "Documento actualizado correctamente.";

                return new JsonResult(new
                {
                    resp.TieneError,
                    resp.Mensaje,
                    id = doc.Id,
                    fechaCreacion = doc.FechaCreacion,
                    fechaModificacion = doc.FechaModificacion,
                    nombreArchivo = doc.NombreArchivo,
                    rutaArchivo = doc.RutaArchivo,
                    ubicacion = doc.Ubicacion
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al guardar el documento.");
                await db.Database.RollbackTransactionAsync();

                resp.TieneError = true;
                resp.Mensaje = "Ocurrió un error al guardar el documento.";
                return new JsonResult(resp);
            }
        }*/

        public async Task<JsonResult> OnGetSiguienteCodigoDocumento(int tipoDocumentoId, int areaId)
        {
            var resp = new ServerResponse(true, "No se pudo generar el código.");

            var area = await db.Areas.AsNoTracking().FirstOrDefaultAsync(a => a.Id == areaId);
            if (area == null) return new JsonResult(resp);

            string prefijo = tipoDocumentoId switch
            {
                1 => "MAN",
                2 => "PRO",
                3 => "POL",
                4 => "REG",
                5 => "FOR",
                6 => "DIA",
                _ => ""
            };

            if (string.IsNullOrWhiteSpace(prefijo))
            {
                resp.Mensaje = "Tipo de documento no válido para generar código.";
                return new JsonResult(resp);
            }

            string siglasArea = ObtenerSiglas3(area.Nombre);
            string baseCode = $"{prefijo}-{siglasArea}-";

            var lastCode = await db.Documentos.AsNoTracking()
                .Where(d => d.AreaId == areaId && d.TipoDocumentoId == tipoDocumentoId && d.NombreArchivo.StartsWith(baseCode))
                .OrderByDescending(d => d.Id)
                .Select(d => d.NombreArchivo)
                .FirstOrDefaultAsync();

            int next = 1;
            if (!string.IsNullOrWhiteSpace(lastCode))
            {
                var parts = lastCode.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int n))
                    next = n + 1;
            }

            string codigo = $"{baseCode}{next:00}";

            resp.TieneError = false;
            resp.Mensaje = "OK";
            resp.Datos = codigo;
            return new JsonResult(resp);
        }

        private static string ObtenerSiglas3(string? nombreArea)
        {
            if (string.IsNullOrWhiteSpace(nombreArea)) return "XXX";

            var words = nombreArea.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 1)
            {
                var clean = new string(words[0].Where(char.IsLetterOrDigit).ToArray()).ToUpper();
                return clean.Length >= 3 ? clean.Substring(0, 3) : clean.PadRight(3, 'X');
            }

            var initials = string.Concat(words.Select(w => char.ToUpper(w[0])));
            return initials.Length >= 3 ? initials.Substring(0, 3) : initials.PadRight(3, 'X');
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

                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    resp.Mensaje = "No se pudo identificar al usuario logeado.";
                    return new JsonResult(resp);
                }

                var codigo = string.IsNullOrWhiteSpace(input.NombreArchivo) ? null : input.NombreArchivo.Trim();

                bool esNuevo = !input.Id.HasValue || input.Id.Value == 0;

                Documento doc;

                if (esNuevo)
                {
                    doc = new Documento
                    {
                        FechaCreacion = DateTime.Now,
                        FechaModificacion = null,
                        Activo = true,
                        CreadoPorId = userId
                    };

                    db.Documentos.Add(doc);
                }
                else
                {
                    doc = await db.Documentos
                        .Include(d => d.Versiones)
                        .FirstOrDefaultAsync(d => d.Id == input.Id.Value);

                    if (doc == null)
                    {
                        resp.Mensaje = "El documento no fue encontrado.";
                        return new JsonResult(resp);
                    }

                    doc.ModificadoPorId = userId;
                    doc.FechaModificacion = DateTime.Now;
                }

                doc.AreaId = input.AreaId.Value;
                doc.TipoDocumentoId = input.TipoDocumentoId.Value;
                doc.EstatusDocumentoId = input.EstatusDocumentoId.Value;

                doc.Titulo = input.Titulo.Trim();
                doc.Descripcion = string.IsNullOrWhiteSpace(input.Descripcion) ? null : input.Descripcion.Trim();
                doc.Responsable = string.IsNullOrWhiteSpace(input.Responsable) ? null : input.Responsable.Trim();
                doc.Observaciones = string.IsNullOrWhiteSpace(input.Observaciones) ? null : input.Observaciones.Trim();

                if (input.Activo.HasValue)
                    doc.Activo = input.Activo.Value == 1;

                if (!string.IsNullOrWhiteSpace(codigo))
                    doc.NombreArchivo = codigo;

                await db.SaveChangesAsync();

                // ==========================================================
                // ✅ NUEVO: crear 3 registros iniciales en DocumentosAutorizaciones
                //     (solo para documentos nuevos; si ya existieran no duplica)
                // ==========================================================
                if (esNuevo)
                {
                    var existenAuth = await db.DocumentosAutorizaciones
                        .AnyAsync(a => a.DocumentoId == doc.Id);

                    if (!existenAuth)
                    {
                        db.DocumentosAutorizaciones.AddRange(
                            new DocumentoAutorizacion
                            {
                                DocumentoId = doc.Id,
                                Rol = RolAutorizacion.DIRECTIVOS,
                                Estado = "PENDIENTE",
                                Activo = true,
                                FechaCreacion = DateTime.Now
                            },
                            new DocumentoAutorizacion
                            {
                                DocumentoId = doc.Id,
                                Rol = RolAutorizacion.REINGENIERIA,
                                Estado = "PENDIENTE",
                                Activo = true,
                                FechaCreacion = DateTime.Now
                            },
                            new DocumentoAutorizacion
                            {
                                DocumentoId = doc.Id,
                                Rol = RolAutorizacion.GERENTE,
                                Estado = "PENDIENTE",
                                Activo = true,
                                FechaCreacion = DateTime.Now
                            }
                        );

                        await db.SaveChangesAsync();
                    }
                }

                DocumentoVersion? versionCreada = null;

                if (input.Archivo != null && input.Archivo.Length > 0)
                {
                    var ext = Path.GetExtension(input.Archivo.FileName);
                    if (!string.Equals(ext, ".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        resp.Mensaje = "Solo se permiten archivos PDF.";
                        return new JsonResult(resp);
                    }

                    await db.DocumentosVersion
                        .Where(v => v.DocumentoId == doc.Id && v.EsActual)
                        .ExecuteUpdateAsync(setters => setters.SetProperty(v => v.EsActual, false));

                    string nextVersion = "1.0";
                    if (!esNuevo)
                    {
                        var last = await db.DocumentosVersion
                            .Where(v => v.DocumentoId == doc.Id)
                            .OrderByDescending(v => v.Id)
                            .Select(v => v.Version)
                            .FirstOrDefaultAsync();

                        nextVersion = CalcularSiguienteVersion(last);
                    }

                    var uploadsRoot = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "documentos",
                        doc.Id.ToString()
                    );

                    if (!Directory.Exists(uploadsRoot))
                        Directory.CreateDirectory(uploadsRoot);

                    var safeFileName = $"{Guid.NewGuid():N}{ext}";
                    var fullPath = Path.Combine(uploadsRoot, safeFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await input.Archivo.CopyToAsync(stream);
                    }

                    var rutaPublica = $"/documentos/{doc.Id}/{safeFileName}";

                    var codigoFinal = !string.IsNullOrWhiteSpace(codigo)
                        ? codigo
                        : (!string.IsNullOrWhiteSpace(doc.NombreArchivo) ? doc.NombreArchivo : Path.GetFileNameWithoutExtension(input.Archivo.FileName));

                    versionCreada = new DocumentoVersion
                    {
                        DocumentoId = doc.Id,
                        Version = nextVersion,
                        EstatusDocumentoId = doc.EstatusDocumentoId,
                        FechaPublicacion = null,
                        Comentarios = null,

                        NombreArchivo = codigoFinal,
                        RutaArchivo = rutaPublica,
                        MimeType = input.Archivo.ContentType,
                        TamanoBytes = input.Archivo.Length,

                        EsActual = true,
                        Activo = true,
                        CreadoPorId = userId,
                        FechaCreacion = DateTime.Now
                    };

                    db.DocumentosVersion.Add(versionCreada);

                    doc.RutaArchivo = rutaPublica;
                    doc.Ubicacion = rutaPublica;
                    doc.NombreArchivo = codigoFinal;

                    await db.SaveChangesAsync();
                }

                await db.Database.CommitTransactionAsync();

                resp.TieneError = false;
                resp.Mensaje = esNuevo ? "Documento creado correctamente." : "Documento actualizado correctamente.";

                return new JsonResult(new
                {
                    resp.TieneError,
                    resp.Mensaje,
                    id = doc.Id,
                    fechaCreacion = doc.FechaCreacion,
                    fechaModificacion = doc.FechaModificacion,
                    creadoPorId = doc.CreadoPorId,
                    modificadoPorId = doc.ModificadoPorId,

                    nombreArchivo = doc.NombreArchivo,
                    rutaArchivo = doc.RutaArchivo,
                    ubicacion = doc.Ubicacion,

                    version = versionCreada?.Version,
                    versionNombreArchivo = versionCreada?.NombreArchivo,
                    versionRutaArchivo = versionCreada?.RutaArchivo,
                    versionMimeType = versionCreada?.MimeType,
                    versionTamanoBytes = versionCreada?.TamanoBytes
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al guardar el documento.");
                await db.Database.RollbackTransactionAsync();

                resp.TieneError = true;
                resp.Mensaje = "Ocurrió un error al guardar el documento.";
                return new JsonResult(resp);
            }
        }


        private static string CalcularSiguienteVersion(string? lastVersion)
        {
            if (string.IsNullOrWhiteSpace(lastVersion)) return "1.0";

            var parts = lastVersion.Split('.');
            if (parts.Length != 2) return "1.0";

            if (!int.TryParse(parts[0], out var major)) return "1.0";
            if (!int.TryParse(parts[1], out var minor)) return "1.0";

            minor += 1;
            return $"{major}.{minor}";
        }

        public async Task<JsonResult> OnGetResponsablesByArea(int areaId)
        {
            ServerResponse resp = new(true, "No se pudieron consultar los responsables.");

            try
            {
                if (areaId <= 0)
                {
                    resp.Mensaje = "Área inválida.";
                    return new JsonResult(resp);
                }

                var responsables = await db.Empleados
                    .Where(e => e.AreaId == areaId && e.Deshabilitado == 0)
                    .OrderBy(e => e.NombreCompleto)
                    .Select(e => new
                    {
                        id = e.Id,
                        nombre = e.NombreCompleto
                    })
                    .ToListAsync();

                resp.TieneError = false;
                resp.Mensaje = "OK";
                resp.Datos = responsables;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar responsables por área.");
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> OnGetAutorizaciones(int documentoId)
        {
            var resp = new ServerResponse(true, "No se pudieron consultar las autorizaciones.");

            try
            {
                if (documentoId <= 0)
                    return new JsonResult(resp);

                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    resp.Mensaje = "Usuario no identificado.";
                    return new JsonResult(resp);
                }

                // ✅ Usuario + roles (para debug)
                var user = await userManager.GetUserAsync(User);
                var roles = (user != null)
                    ? await userManager.GetRolesAsync(user)
                    : new List<string>();

                // ✅ Admin si tiene cualquiera de estos roles (ajusta a los reales)
                bool esAdmin =
                    roles.Any(r =>
                        r.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ||
                        r.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                        r.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase)
                    );

                bool puedeDirectivos =
                    esAdmin ||
                    User.IsInRole("DirectorGeneral") ||
                    User.IsInRole("Director") ||
                    User.IsInRole("Socio");

                bool puedeReingenieria =
                    esAdmin ||
                    User.IsInRole("ReingenieriaProcesos") ||
                    User.IsInRole("Reingenieria");

                bool puedeGerente =
                    esAdmin ||
                    User.IsInRole("Gerente") ||
                    User.IsInRole("GerenteArea");

                var auth = await db.DocumentosAutorizaciones
                    .Include(x => x.AutorizadoPor)
                    .Where(a => a.DocumentoId == documentoId && a.Activo)
                    .ToListAsync();

                AutorizacionItemDto Map(RolAutorizacion rol)
                {
                    var item = auth.FirstOrDefault(x => x.Rol == rol);

                    if (item == null)
                        return new AutorizacionItemDto { Estado = "PENDIENTE" };

                    return new AutorizacionItemDto
                    {
                        Estado = item.Estado,
                        Por = item.AutorizadoPor?.UserName,
                        Fecha = item.Fecha?.ToString("yyyy-MM-dd"),
                        Comentario = item.Comentario
                    };
                }

                var dto = new AutorizacionesDto
                {
                    Directivos = Map(RolAutorizacion.DIRECTIVOS),
                    Reingenieria = Map(RolAutorizacion.REINGENIERIA),
                    Gerente = Map(RolAutorizacion.GERENTE),
                    Permisos = new PermisosAutorizacionDto
                    {
                        PuedeAutorizarDirectivos = puedeDirectivos,
                        PuedeAutorizarReingenieria = puedeReingenieria,
                        PuedeAutorizarGerente = puedeGerente
                    }
                };

                var estados = new[]
                {
            dto.Directivos.Estado,
            dto.Reingenieria.Estado,
            dto.Gerente.Estado
        }
                .Select(x => (x ?? "").ToUpper())
                .ToList();

                dto.Resumen =
                    estados.Contains("RECHAZADO") ? "RECHAZADO" :
                    estados.All(x => x == "APROBADO") ? "APROBADO" :
                    "PENDIENTE";

                resp.TieneError = false;
                resp.Mensaje = "OK";

                resp.Datos = new
                {
                    dto.Resumen,
                    dto.Directivos,
                    dto.Reingenieria,
                    dto.Gerente,
                    dto.Permisos,

                    RolesUsuario = roles
                };

                return new JsonResult(resp);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar autorizaciones.");
                return new JsonResult(resp);
            }
        }


        public async Task<JsonResult> OnPostAutorizar(AutorizarRequest input)
        {
            var resp = new ServerResponse(true, "No se pudo registrar la autorización.");

            try
            {
                if (input == null || input.DocumentoId <= 0 || string.IsNullOrWhiteSpace(input.Rol))
                    return new JsonResult(resp);

                var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(userId))
                {
                    resp.Mensaje = "Usuario no identificado.";
                    return new JsonResult(resp);
                }

                var user = await userManager.GetUserAsync(User);
                var roles = (user != null)
                    ? await userManager.GetRolesAsync(user)
                    : new List<string>();

                bool esAdmin =
                    roles.Any(r =>
                        r.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ||
                        r.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                        r.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase)
                    );

                string rol = input.Rol.Trim().ToUpper();

                bool autorizado = esAdmin || rol switch
                {
                    "DIRECTIVOS" =>
                        User.IsInRole("DirectorGeneral") ||
                        User.IsInRole("Director") ||
                        User.IsInRole("Socio"),

                    "REINGENIERIA" =>
                        User.IsInRole("ReingenieriaProcesos") ||
                        User.IsInRole("Reingenieria"),

                    "GERENTE" =>
                        User.IsInRole("Gerente") ||
                        User.IsInRole("GerenteArea"),

                    _ => false
                };

                if (!autorizado)
                {
                    resp.Mensaje = "No tienes permisos para autorizar en este nivel.";
                    return new JsonResult(resp);
                }

                RolAutorizacion rolEnum = Enum.Parse<RolAutorizacion>(rol);

                var registro = await db.DocumentosAutorizaciones
                    .FirstOrDefaultAsync(a =>
                        a.DocumentoId == input.DocumentoId &&
                        a.Rol == rolEnum);

                if (registro == null)
                {
                    registro = new DocumentoAutorizacion
                    {
                        DocumentoId = input.DocumentoId,
                        Rol = rolEnum,
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    };
                    db.DocumentosAutorizaciones.Add(registro);
                }

                registro.Estado = input.Aprobar ? "APROBADO" : "RECHAZADO";
                registro.Fecha = DateTime.Now;
                registro.AutorizadoPorId = userId;
                registro.Comentario = input.Comentario;

                await db.SaveChangesAsync();

                resp.TieneError = false;
                resp.Mensaje = input.Aprobar
                    ? "Autorizado correctamente."
                    : "Rechazado correctamente.";

                return new JsonResult(resp);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al autorizar documento.");
                return new JsonResult(resp);
            }
        }



        public async Task<JsonResult> OnPostFiltrarDocumentos()
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);

            try
            {
                var documentos = await documentoManager.GetAllAsync(InputFiltro);

                documentos = documentos.Where(d => d.Activo).ToList();

                var result = documentos.Select(d => new
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

                    creadoPorId = d.CreadoPorId ?? "-",
                    modificadoPorId = d.ModificadoPorId ?? "-",

                    fechaCreacion = d.FechaCreacion == DateTime.MinValue ? (DateTime?)null : d.FechaCreacion,
                    fechaCreacionJS = d.FechaCreacion == DateTime.MinValue ? "" : d.FechaCreacion.ToString("yyyy-MM-dd"),

                    fechaModificacion = d.FechaModificacion.HasValue ? d.FechaModificacion : null,
                    fechaModificacionJS = d.FechaModificacion.HasValue ? d.FechaModificacion.Value.ToString("yyyy-MM-dd") : "",

                    activo = d.Activo
                }).ToList();

                resp.Datos = result;
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Rango de fechas inválido al filtrar documentos.");

                resp.TieneError = true;
                resp.Mensaje = "La fecha fin no puede ser menor que la fecha inicio. Selecciona un rango correcto.";
                resp.Datos = null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al filtrar documentos");

                resp.TieneError = true;
                resp.Mensaje = localizer["ConsultadoUnsuccessfully"];
                resp.Datos = null;
            }

            return new JsonResult(resp);
        }

    }
}
