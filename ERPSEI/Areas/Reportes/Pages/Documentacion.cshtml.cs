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
        private readonly IEmpleadoManager empleadoActivoFijoManager;
        //private readonly IOficinaManager oficinaActivoFijoManager;
        private readonly IStringLocalizer<DocumentacionModel> localizer;
        private readonly AppUserManager userManager;

        private readonly Data.ApplicationDbContext db;

        [BindProperty]
        public Documento DocumentosList { get; set; }

        [BindProperty]
        public InputFiltroModel InputFiltro { get; set; }

    public class InputFiltroModel
    {
        // Búsqueda por título / texto
        [DataType(DataType.Text)]
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
        [DataType(DataType.Text)]
        [StringLength(80, ErrorMessage = "FieldLength", MinimumLength = 2)]
        [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
        public string? PalabraClave { get; set; } = string.Empty;

        // Rango de fechas (por FechaCreacion de Documento)
        [Display(Name = "Fecha Creación Inicio")]
        [DataType(DataType.Date)]
        public DateTime? FechaCreacionInicio { get; set; }

        [Display(Name = "Fecha Creación Fin")]
        [DataType(DataType.Date)]
        public DateTime? FechaCreacionFin { get; set; }
    }

        public class EstatusDocumentoFiltroModel
        {
            public int? Id { get; set; }

            [StringLength(80)]
            public string? Nombre { get; set; }

            public bool? Activo { get; set; }

            public bool? EsPublicable { get; set; }
        }

        public class TipoDocumentoFiltroModel
        {
            public int? Id { get; set; }

            [StringLength(150)]
            public string? Nombre { get; set; }

            public bool? Activo { get; set; }
        }



    }
}
