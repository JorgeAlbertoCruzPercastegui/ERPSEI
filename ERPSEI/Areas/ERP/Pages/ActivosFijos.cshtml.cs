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

namespace ERPSEI.Areas.ERP.Pages
{
    public class ActivosFijosModel : ERPPageModel
    {
        private readonly IStringLocalizer<ActivosFijosModel> stringLocalizer;
        private readonly ILogger<ActivosFijosModel> logger;
        private readonly AppUserManager appUserManager;
        private readonly IActivoFijoManager activoFijoManager;
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

        // ⬇️ Agrega esto aquí
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

            public int? Deshabilitado { get; set; } = 0;
        }

        public ActivosFijosModel(
            IStringLocalizer<ActivosFijosModel> _stringLocalizer,
            ILogger<ActivosFijosModel> _logger,
            AppUserManager _appUserManager,
            IStringLocalizer<ActivosFijosModel> _localizer,
            Data.ApplicationDbContext _db,
            IActivoFijoManager _activoFijoManager,
            AppUserManager _userManager
        )
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            appUserManager = _appUserManager;
            localizer = _localizer;
            db = _db;
            activoFijoManager = _activoFijoManager;
            userManager = _userManager;

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
                    fechaCompra = fechaCompra?.ToString("dd/MM/yyyy") ?? "-",
                    fechaCompraJS = fechaCompra?.ToString("yyyy-MM-dd") ?? "-",
                    fechaRenovacion = fechaRenovacion?.ToString("dd/MM/yyyy") ?? "-",
                    precio = a.Precio,
                    ubicacion = a.Ubicacion ?? "-",
                    linkFacturaCompra = a.LinkFacturaCompra ?? "-",
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

                // Claves foráneas
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
                    fechaCompra = a.FechaCompra?.ToString("dd/MM/yyyy") ?? "-",
                    fechaCompraJS = a.FechaCompra?.ToString("yyyy-MM-dd") ?? "-",
                    fechaRenovacion = a.FechaRenovacion?.ToString("dd/MM/yyyy") ?? "-",
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

    }
}

