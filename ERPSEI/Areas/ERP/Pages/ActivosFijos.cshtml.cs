using ERPSEI.Data;
using ERPSEI.Data.Entities.ActivosFijos;
using ERPSEI.Data.Managers.ActivosFijos;
using ERPSEI.Data.Entities.Conciliaciones;
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
using static ERPSEI.Areas.Catalogos.Pages.GestionDeTalentoModel;
using static ERPSEI.Areas.ERP.Pages.ConciliacionesModel;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using ERPSEI.Requests;
using OfficeOpenXml;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using iText.Layout;
using MathNet.Numerics.Distributions;

namespace ERPSEI.Areas.ERP.Pages
{
    public class ActivosFijosModel : ERPPageModel
    {
        private readonly IStringLocalizer<ActivosFijosModel> stringLocalizer;
        private readonly ILogger<ActivosFijosModel> logger;
        private readonly AppUserManager appUserManager;
        private readonly IActivoFijoManager activoFijoManager;
        private readonly ICategoriaActivosFijosManager categoriaActivoFijoManager;
        private readonly ITipoActivosFijosManager tipoActivoFijoManager;
        private readonly IEmpleadoManager empleadoActivoFijoManager;
        private readonly IOficinaManager oficinaActivoFijoManager;
        private readonly IStringLocalizer<ActivosFijosModel> localizer;
        private readonly AppUserManager userManager;

        private readonly Data.ApplicationDbContext db;

        [BindProperty]
        public ActivoFijo? ActivosFijosList { get; set; }

        [BindProperty]
        public InputFiltroModel InputFiltro { get; set; }

        public class InputFiltroModel
        {
            [StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "PersonName")]
            public string? Folio { get; set; }

            [DataType(DataType.Text)]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Responsable { get; set; } = string.Empty;

            [Display(Name = "Categoria")]
            public int? CategoriaId { get; set; }

            [Display(Name = "Tipo")]
            public int? TipoId { get; set; }

            [Display(Name = "Fecha Compra Inicio")]
            [DataType(DataType.Date)]
            public DateTime? FechaCompraInicio { get; set; }

            [Display(Name = "Fecha Compra Fin")]
            [DataType(DataType.Date)]
            public DateTime? FechaCompraFin { get; set; }

            [DataType(DataType.Text)]
            //[StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.AlphanumNoSpace, ErrorMessage = "PersonName")]
            public string? Estatus { get; set; }
        }


        [BindProperty]
        public ActivoFijoTableModel InputActivosFijos { get; set; }

        public List<Empleado> empleados { get; set; } = new();

        public class ActivoFijoTableModel
        {
            public int? Id { get; set; }
            [StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Folio { get; set; }

            [DataType(DataType.Text)]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Descripcion { get; set; } = string.Empty;

            [DataType(DataType.Text)]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            public string? Responsable { get; set; }

            public int? EmpleadoId { get; set; }

            [DataType(DataType.Text)]
            public string? Categoria { get; set; }

            [DataType(DataType.Text)]
            public string? Tipo { get; set; }

            [DataType(DataType.Text)]
            public string? Oficina { get; set; }

            [Required(ErrorMessage = "La Fecha Compra es obligatoria.")]
            [Display(Name = "Fecha Compra")]
            [DataType(DataType.Date)]
            public DateTime? FechaCompra { get; set; }

            [Required(ErrorMessage = "El Precio es obligatoria.")]
            public decimal? Precio { get; set; } = 0;

            [Display(Name = "Link Factura Compra")]
            [DataType(DataType.Url)]
            [StringLength(300, ErrorMessage = "La URL es demasiado larga")]
            public string? LinkFacturaCompra { get; set; } = string.Empty;

            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            [Display(Name = "Marca")]
            public string? Marca { get; set; } = string.Empty;

            [Required(ErrorMessage = "El Número de Serie es obligatoria.")]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            [Display(Name = "Número Serie")]
            public string? NumeroSerie { get; set; } = string.Empty;

            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [Display(Name = "Ubicación")]
            public string? Ubicacion { get; set; }

            [StringLength(150, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [Display(Name = "Comentarios")]
            public string? Comentarios { get; set; }

            [Display(Name = "Fecha Renovación")]
            [DataType(DataType.Date)]
            public DateTime? FechaRenovacion { get; set; }

            [Display(Name = "Cantidad")]
            public int? Cantidades { get; set; }

            public int? Deshabilitado { get; set; } = 0;
        }

        [BindProperty]
        public ImportarModel InputImportar { get; set; }
        public class ImportarModel
        {
            [Required(ErrorMessage = "Required")]
            public IFormFile? Plantilla { get; set; }
        }

        public ActivosFijosModel(
            IStringLocalizer<ActivosFijosModel> _stringLocalizer,
            ILogger<ActivosFijosModel> _logger,
            AppUserManager _appUserManager,
            IStringLocalizer<ActivosFijosModel> _localizer,
            Data.ApplicationDbContext _db,
            IActivoFijoManager _activoFijoManager,
            AppUserManager _userManager,
            ICategoriaActivosFijosManager categoriaManager,
            ITipoActivosFijosManager tipoManager,
            IEmpleadoManager empleadoManager,
            IOficinaManager oficinaManager
        )
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            appUserManager = _appUserManager;
            localizer = _localizer;
            db = _db;
            activoFijoManager = _activoFijoManager;
            userManager = _userManager;

            categoriaActivoFijoManager = categoriaManager;
            tipoActivoFijoManager = tipoManager;
            empleadoActivoFijoManager = empleadoManager;
            oficinaActivoFijoManager = oficinaManager;

            InputFiltro = new InputFiltroModel();
            InputActivosFijos = new ActivoFijoTableModel();
            ActivosFijosList = new ActivoFijo();
        }


        //Método para listar en json todos los activos fijos

        public async Task<JsonResult> OnGetActivosFijosList()
        {
            var activos = await activoFijoManager.GetAllAsync();
            activos = activos.Where(a => a.Deshabilitado != true).ToList();


            var jsonActivos = new List<object>();

            foreach (var a in activos)
            {
                DateTime? fechaCompra = a.FechaCompra == DateTime.MinValue ? null : a.FechaCompra;
                DateTime? fechaRenovacion = a.FechaRenovacion == DateTime.MinValue ? null : a.FechaRenovacion;

                jsonActivos.Add(new
                {
                    id = a.Id,
                    folio = a.Folio ?? "-",
                    descripcion = a.Descripcion ?? "-",
                    marca = a.Marca ?? "-",
                    numeroSerie = a.NumeroSerie ?? "-",
                    responsable = a.Empleado?.NombreCompleto ?? "-",
                    responsableId = a.EmpleadoId,
                    categoria = a.Categoria?.Descripcion ?? "-",
                    categoriaId = a.CategoriaId,
                    tipo = a.Tipo?.Descripcion ?? "-",
                    tipoId = a.TipoId,
                    oficina = a.Oficina?.Nombre ?? "",
                    oficinaId = a.OficinaId,
                    fechaCompra = fechaCompra?.ToString("dd/MM/yyyy") ?? "-",
                    fechaCompraJS = fechaCompra?.ToString("yyyy-MM-dd") ?? "-",
                    fechaRenovacion = fechaRenovacion?.ToString("dd/MM/yyyy") ?? "-",
                    precio = a.Precio,
                    ubicacion = a.Ubicacion ?? "-",
                    linkFacturaCompra = a.LinkFacturaCompra ?? "-",
                    cantidades = a.Cantidades,
                    comentarios = a.Comentarios ?? "-",
                    deshabilitado = a.Deshabilitado.ToString()
                });
            }

            return new JsonResult(jsonActivos);
        }

        public async Task<JsonResult> OnGetObtenerSiguienteFolioAsync()
        {
            // Buscar el folio más alto que comienza con 'AF' y tiene 4 dígitos numéricos
            var ultimoFolio = await db.ActivosFijos
                .Where(a => a.Folio.StartsWith("AF") && a.Folio.Length == 6)
                .OrderByDescending(a => a.Folio)
                .Select(a => a.Folio)
                .FirstOrDefaultAsync();

            int siguienteNumero = 1;

            if (!string.IsNullOrEmpty(ultimoFolio) && int.TryParse(ultimoFolio.Substring(2), out int ultimoNumero))
            {
                siguienteNumero = ultimoNumero + 1;
            }

            var siguienteFolio = $"AF{siguienteNumero.ToString("D4")}";

            return new JsonResult(new { folio = siguienteFolio });
        }

        public async Task<JsonResult> OnPostDeleteActivosFijos(string[] ids)
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

                    var activo = await db.ActivosFijos.FirstOrDefaultAsync(a => a.Id == intId);

                    if (activo == null)
                        continue;

                    activo.Deshabilitado = true;

                    // Guardar cambios por cada uno
                    db.ActivosFijos.Update(activo);
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

        public async Task<JsonResult> OnPostSaveActivoFijo(ActivoFijoTableModel input)
        {
            //ServerResponse resp = new(true, "No se pudo guardar el registro.");
            ServerResponse resp = new(true, localizer["ActualizadoAFUnsuccessfully"]);

            try
            {
                await db.Database.BeginTransactionAsync();

                if (input == null)
                {
                    resp.Mensaje = "No se recibieron datos para guardar.";
                    return new JsonResult(resp);
                }

                if (string.IsNullOrWhiteSpace(input.Folio) || string.IsNullOrWhiteSpace(input.Descripcion))
                {
                    resp.Mensaje = "El folio y la descripción son obligatorios.";
                    return new JsonResult(resp);
                }

                ActivoFijo activo;

                var empleado = await db.Empleados
                    .FirstOrDefaultAsync(e => e.NombreCompleto == input.Responsable);

                /*if (empleado == null)
                {
                    resp.Mensaje = $"No se encontró el responsable asignado: {input.Responsable}";
                    return new JsonResult(resp);
                }*/

                bool esNuevo = input.Id == null || input.Id == 0;

                if (esNuevo)
                {
                    int nextId = (await db.ActivosFijos.OrderByDescending(a => a.Id).Select(a => a.Id).FirstOrDefaultAsync()) + 1;

                    activo = new ActivoFijo
                    {
                        Id = nextId
                    };

                    db.ActivosFijos.Add(activo);
                }
                else
                {
                    activo = await db.ActivosFijos.FirstOrDefaultAsync(a => a.Id == input.Id);
                    if (activo == null)
                    {
                        resp.Mensaje = "El activo no fue encontrado.";
                        return new JsonResult(resp);
                    }
                }

                // Asignación de valores
                activo.Folio = input.Folio ?? "";
                activo.Descripcion = input.Descripcion ?? "";
                activo.Marca = input.Marca ?? "";
                activo.NumeroSerie = input.NumeroSerie ?? "";
                activo.Ubicacion = input.Ubicacion ?? "";
                activo.FechaCompra = input.FechaCompra;
                activo.Precio = input.Precio ?? 0;
                activo.LinkFacturaCompra = input.LinkFacturaCompra ?? "";
                activo.Comentarios = input.Comentarios ?? "";
                activo.FechaRenovacion = input.FechaRenovacion;
                activo.Cantidades = input.Cantidades ?? 0;

                // Claves foráneas
                int ofiId = 0;
                int.TryParse(input.Oficina, out ofiId);

                if (ofiId <= 0 || !await db.Oficinas.AnyAsync(o => o.Id == ofiId))
                {
                    resp.Mensaje = "La oficina seleccionada no existe en el catálogo.";
                    return new JsonResult(resp);
                }

                activo.OficinaId = ofiId;
                //activo.EmpleadoId = input.EmpleadoId ?? 0;
                activo.EmpleadoId = input.EmpleadoId ?? 0;
                activo.TipoId = int.TryParse(input.Tipo, out int tipoId) ? tipoId : 0;
                activo.CategoriaId = int.TryParse(input.Categoria, out int catId) ? catId : 0;

                await db.SaveChangesAsync();

                await db.Database.CommitTransactionAsync();

                resp.TieneError = false;
                resp.Mensaje = esNuevo ? "Activo creado correctamente." : "Activo actualizado correctamente.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al guardar el activo fijo.");
                await db.Database.RollbackTransactionAsync();
                resp.Mensaje = localizer["ActualizadoAFSuccessfully"];
                //resp.Mensaje = "Ocurrió un error al guardar el registro.";
            }

            return new JsonResult(resp);
        }

        public async Task OnGetAsync()
        {
            empleados = await db.Empleados
                .OrderBy(e => e.NombreCompleto)
                .ToListAsync();
        }

        public async Task<JsonResult> OnPostFiltrarActivosFijos()
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);

            try
            {
                var activos = await activoFijoManager.GetAllAsync(InputFiltro);

                var result = activos.Select(a => new {
                    id = a.Id,
                    folio = a.Folio ?? "-",
                    descripcion = a.Descripcion ?? "-",
                    marca = a.Marca ?? "-",
                    numeroSerie = a.NumeroSerie ?? "-",
                    responsable = a.Empleado?.NombreCompleto ?? "-",
                    responsableId = a.EmpleadoId,
                    categoria = a.Categoria?.Descripcion ?? "-",
                    categoriaId = a.CategoriaId,
                    tipo = a.Tipo?.Descripcion ?? "-",
                    tipoId = a.TipoId,
                    oficina = a.Oficina?.Nombre ?? "-",
                    oficinaId = a.OficinaId,
                    fechaCompra = a.FechaCompra?.ToString("dd/MM/yyyy") ?? "-",
                    fechaCompraJS = a.FechaCompra?.ToString("yyyy-MM-dd") ?? "-",
                    fechaRenovacion = a.FechaRenovacion?.ToString("dd/MM/yyyy") ?? "-",
                    cantidades = a.Cantidades,
                    precio = a.Precio,
                    ubicacion = a.Ubicacion ?? "-",
                    linkFacturaCompra = a.LinkFacturaCompra ?? "-",
                    comentarios = a.Comentarios ?? "-",
                    deshabilitado = a.Deshabilitado
                }).ToList();

                resp.Datos = result;
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al filtrar activos fijos");
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> OnPostGetUsuariosSuggestion(string texto)
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
            try
            {
                    resp.Datos = await GetUsuariosSuggestion(texto);
                    resp.TieneError = false;
                    resp.Mensaje = localizer["ConsultadoSuccessfully"];
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
                    string desc = u.Empleado?.NombreCompleto.Length >= 1 ? $"{u.Empleado?.NombreCompleto}" : $"{u.UserName}";
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

        public ActionResult OnGetDownloadPlantilla()
        {
            return File("/templates/PlantillaActivosFijos.xlsx", MediaTypeNames.Application.Octet, "PlantillaActivosFijos.xlsx");
        }

        public async Task<IActionResult> OnGetExportarActivosFijos()
        {
            var activos = await activoFijoManager.GetAllAsync();

            foreach (var a in activos)
            {
                logger.LogInformation(
                    "Activo {Id} | OficinaId: {OficinaId} | OficinaNombre: {OficinaNombre}",
                    a.Id,
                    a.OficinaId,
                    a.Oficina?.Nombre
                );
            }


            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Activos Fijos");

            // Estilo de encabezado
            ICellStyle headerStyle = workbook.CreateCellStyle();
            IFont headerFont = workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);
            headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            headerStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            headerStyle.FillPattern = FillPattern.SolidForeground;
            headerStyle.BorderBottom = BorderStyle.Thin;
            headerStyle.BorderTop = BorderStyle.Thin;
            headerStyle.BorderLeft = BorderStyle.Thin;
            headerStyle.BorderRight = BorderStyle.Thin;

            // Estilo para datos
            ICellStyle dataStyle = workbook.CreateCellStyle();
            dataStyle.BorderBottom = BorderStyle.Thin;
            dataStyle.BorderTop = BorderStyle.Thin;
            dataStyle.BorderLeft = BorderStyle.Thin;
            dataStyle.BorderRight = BorderStyle.Thin;

            // Encabezados
            var headers = new[] {
        "Id", "Folio", "Descripción", "Responsable", "Categoría", "Tipo",
        "Fecha Compra","Cantidad", "Precio", "Oficina", "Número Serie", "Link Factura", "Comentarios"
    };

            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = headerRow.CreateCell(i);
                cell.SetCellValue(headers[i]);
                cell.CellStyle = headerStyle;
            }

            // Contenido
            for (int i = 0; i < activos.Count; i++)
            {
                var a = activos[i];
                IRow row = sheet.CreateRow(i + 1);

                row.CreateCell(0).SetCellValue(a.Id);
                row.CreateCell(1).SetCellValue(a.Folio ?? "-");
                row.CreateCell(2).SetCellValue(a.Descripcion ?? "-");
                row.CreateCell(3).SetCellValue(a.Empleado?.NombreCompleto ?? "-");
                row.CreateCell(4).SetCellValue(a.Categoria?.Descripcion ?? "-");
                row.CreateCell(5).SetCellValue(a.Tipo?.Descripcion ?? "-");
                row.CreateCell(6).SetCellValue(a.FechaCompra?.ToString("dd/MM/yyyy") ?? "-");
                row.CreateCell(7).SetCellValue(Convert.ToInt32(a.Cantidades));
                row.CreateCell(8).SetCellValue(Convert.ToDouble(a.Precio));
                row.CreateCell(9).SetCellValue(a.Oficina?.Nombre ?? "-");
                row.CreateCell(10).SetCellValue(a.NumeroSerie ?? "-");
                row.CreateCell(11).SetCellValue(a.LinkFacturaCompra ?? "-");
                row.CreateCell(12).SetCellValue(a.Comentarios ?? "-");

                // Aplica estilo a cada celda
                for (int j = 0; j < headers.Length; j++)
                {
                    row.GetCell(j).CellStyle = dataStyle;
                }
            }

            // Autoajustar ancho de columnas
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            // Exportar archivo
            byte[] fileBytes;
            using (var exportData = new MemoryStream())
            {
                workbook.Write(exportData);
                fileBytes = exportData.ToArray();
            }

            string fileName = $"ActivosFijos_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private async Task<string> ValidarSiExisteActivoFijo(ActivoFijoTableModel af)
        {
            List<ActivoFijo> activos = await activoFijoManager.GetAllAsync();

            // Si el Id ya existe, se excluye de la comparación
            activos = activos.Where(a => a.Id != af.Id).ToList();

            // Validación por Folio
            if (activos.Any(a => !a.Deshabilitado && (a.Folio ?? "") == af.Folio))
                return $"El folio '{af.Folio}' ya está registrado en otro activo.";

            // Validación por Número de Serie
            if (activos.Any(a => !a.Deshabilitado && (a.NumeroSerie ?? "") == af.NumeroSerie))
                return $"El número de serie '{af.NumeroSerie}' ya está registrado en otro activo.";

            return string.Empty;
        }

        private async Task CreateOrUpdateActivoFijo(ActivoFijoTableModel af)
        {
            try
            {
                await db.Database.BeginTransactionAsync();

                int idActivo = 0;
                ActivoFijo? activo = await activoFijoManager.GetByIdAsync(af.Id ?? 0);

                if (activo != null)
                {
                    idActivo = activo.Id;
                }
                else
                {
                    activo = new ActivoFijo();
                }

                // Asignar propiedades
                activo.Folio = af.Folio ?? "";
                activo.Marca = af.Marca ?? "";
                activo.NumeroSerie = af.NumeroSerie ?? "";
                activo.Descripcion = af.Descripcion ?? "";
                //activo.Ubicacion = af.Ubicacion ?? "";
                activo.FechaCompra = af.FechaCompra;
                activo.Precio = af.Precio ?? 0;
                activo.Comentarios = af.Comentarios ?? "";
                activo.FechaRenovacion = af.FechaRenovacion;
                activo.EmpleadoId = af.EmpleadoId ?? 0;
                activo.CategoriaId = int.TryParse(af.Categoria, out var catId) ? catId : 0;
                activo.TipoId = int.TryParse(af.Tipo, out var tipoId) ? tipoId : 0;
                activo.OficinaId = int.TryParse(af.Oficina, out var ofiId) ? ofiId : 0;
                activo.LinkFacturaCompra = af.LinkFacturaCompra ?? "";
                activo.Deshabilitado = false;

                // Crear o actualizar según corresponda
                if (idActivo > 0)
                    await activoFijoManager.UpdateFromExcelAsync(activo);
                else
                    await activoFijoManager.CreateFromExcelAsync(activo);

                await db.Database.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await db.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<JsonResult> OnPostImportarActivosFijos()
        {
            ServerResponse resp = new(true, localizer["ActivosFijosImportadosUnsuccessfully"]);

            try
            {
                if (Request.Form.Files.Count >= 1)
                {
                    using Stream s = Request.Form.Files[0].OpenReadStream();
                    using var reader = ExcelReaderFactory.CreateReader(s);
                    DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        FilterSheet = (tableReader, sheetIndex) => sheetIndex == 0
                    });

                    foreach (DataRow row in result.Tables[0].Rows)
                    {
                        if (result.Tables[0].Rows.IndexOf(row) == 0)
                        {
                            resp.TieneError = false;
                            resp.Mensaje = localizer["ActivosFijosImportadosSuccessfully"];
                            continue;
                        }

                        string vmsg = await CreateActivoFijoFromExcelRow(row);

                        if (!string.IsNullOrEmpty(vmsg))
                        {
                            resp.TieneError = true;
                            resp.Mensaje = vmsg;
                            break;
                        }

                        resp.TieneError = false;
                        resp.Mensaje = localizer["ActivosFijosImportadosSuccessfully"];
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                resp.TieneError = true;
                resp.Mensaje = localizer["ActivosFijosImportadosUnsuccessfully"];
            }

            return new JsonResult(resp);
        }

        private async Task<string> CreateActivoFijoFromExcelRow(DataRow row)
        {
            // Conversión de fechas y números
            _ = DateTime.TryParse(row[5]?.ToString(), out DateTime fechaCompra);
            _ = DateTime.TryParse(row[8]?.ToString(), out DateTime fechaRenovacion);
            _ = decimal.TryParse(row[6]?.ToString(), out decimal precio);
            _ = int.TryParse(row[4]?.ToString(), out int cantidades);


            // Obtener nombres desde el Excel
            string folio = row[0]?.ToString()?.Trim() ?? "";
            string marca = row[1]?.ToString()?.Trim() ?? "";
            string numeroSerie = row[2]?.ToString()?.Trim() ?? "";
            string descripcion = row[3]?.ToString()?.Trim() ?? "";
            string responsableNombre = row[9]?.ToString()?.Trim() ?? "";
            string categoriaNombre = row[10]?.ToString()?.Trim() ?? "";
            string tipoNombre = row[11]?.ToString()?.Trim() ?? "";
            string linkFactura = row[12]?.ToString()?.Trim() ?? "";
            string comentarios = row[7]?.ToString()?.Trim() ?? "";
            string oficinaNombre = row[13]?.ToString()?.Trim() ?? "";
            //string ubicacion = row[4]?.ToString()?.Trim() ?? "";

            // Validación de existencia en catálogos
            var categoria = await categoriaActivoFijoManager.GetByNameAsync(categoriaNombre);
            if (categoria == null)
                return $"La categoría '{categoriaNombre}' no existe en el catálogo.";

            var tipo = await tipoActivoFijoManager.GetByNameAsync(tipoNombre);
            if (tipo == null)
                return $"El tipo '{tipoNombre}' no existe en el catálogo.";

            var oficina = await oficinaActivoFijoManager.GetByNameAsync(oficinaNombre);
            if (oficina == null)
                return $"La oficina '{oficinaNombre}' no existe en el catálogo.";

            var responsable = await empleadoActivoFijoManager.GetByNameAsync(responsableNombre);
            if (responsable == null)
                return $"El responsable '{responsableNombre}' no existe en el catálogo.";

            // Crear modelo
            ActivoFijoTableModel af = new()
            {
                Folio = folio,
                Marca = marca,
                NumeroSerie = numeroSerie,
                Descripcion = descripcion,
                EmpleadoId = responsable.Id,
                Categoria = categoria.Id.ToString(),
                Tipo = tipo.Id.ToString(),
                Oficina = oficina.Id.ToString(),
                FechaCompra = fechaCompra,
                Precio = precio,
                Cantidades = cantidades,
                LinkFacturaCompra = linkFactura,
                Comentarios = comentarios,
                FechaRenovacion = fechaRenovacion == DateTime.MinValue ? null : fechaRenovacion
                //Ubicacion = ubicacion
            };

            // Validar duplicados por folio o serie
            string validationMsg = await ValidarSiExisteActivoFijo(af);
            if (string.IsNullOrEmpty(validationMsg))
            {
                await CreateOrUpdateActivoFijo(af);
            }

            return validationMsg ?? "";
        }
    }
}

