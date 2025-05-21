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

namespace ERPSEI.Areas.ERP.Pages
{
    public class ActivosFijosModel : ERPPageModel
    {
        private readonly IStringLocalizer<ActivosFijosModel> stringLocalizer;
        private readonly ILogger<ActivosFijosModel> logger;
        private readonly AppUserManager appUserManager;
        private readonly IActivoFijoManager activoFijoManager;
        private readonly IStringLocalizer<ActivosFijosModel> localizer;

        private readonly Data.ApplicationDbContext db;

        [BindProperty]
        public ActivoFijo? ActivosFijosList { get; set; }
        public InputFiltroModel InputFiltro { get; set; }

        public class InputFiltroModel
        {
            //[StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 1)]
            //[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "PersonName")]
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

        public class ActivoFijoTableModel
        {
            public int? Id { get; set; }
            public string? Folio { get; set; }

            [DataType(DataType.Text)]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Descripcion { get; set; } = string.Empty;

            [DataType(DataType.Text)]
            [RegularExpression(RegularExpressions.AlphanumNoSpace, ErrorMessage = "PersonName")]
            public string? Responsable { get; set; }

            [DataType(DataType.Text)]
            [RegularExpression(RegularExpressions.AlphanumNoSpace, ErrorMessage = "PersonName")]
            public string? Categoria { get; set; }

            [DataType(DataType.Text)]
            [RegularExpression(RegularExpressions.AlphanumNoSpace, ErrorMessage = "PersonName")]
            public string? Tipo { get; set; }

            [Display(Name = "Fecha Compra")]
            [DataType(DataType.Date)]
            public DateTime? FechaCompra { get; set; }

            public decimal? Precio { get; set; }

            [Display(Name = "Link Factura Compra")]
            [DataType(DataType.Url)]
            [StringLength(300, ErrorMessage = "La URL es demasiado larga")]
            public string? LinkFacturaCompra { get; set; }

            [Display(Name = "Marca")]
            public string? Marca { get; set; }

            [Display(Name = "Número Serie")]
            public string? NumeroSerie { get; set; }

            [Display(Name = "Ubicación")]
            public string? Ubicacion { get; set; }

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
            IActivoFijoManager _activoFijoManager // <--- AÑADIR ESTO
        )
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            appUserManager = _appUserManager;
            localizer = _localizer;
            db = _db;
            activoFijoManager = _activoFijoManager; // <--- AÑADIR ESTA LÍNEA

            InputFiltro = new InputFiltroModel();
            InputActivosFijos = new ActivoFijoTableModel();
        }


        //Método para listar en json todos los activos fijos

        public async Task<JsonResult> OnGetActivosFijosList()
        {
            var activos = await activoFijoManager.GetAllAsync();

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


    }
}