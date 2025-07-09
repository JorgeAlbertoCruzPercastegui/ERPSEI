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
using ERPSEI.Areas.ERP.Pages;
using ERPSEI.Data.Entities.Empresas;

namespace ERPSEI.Areas.Reportes.Pages
{
    public class GeneradorContratoModel : ERPPageModel
    {
        private readonly IStringLocalizer<GeneradorContratoModel> stringLocalizer;
        private readonly ILogger<GeneradorContratoModel> logger;
        private readonly AppUserManager appUserManager;
        private readonly IEmpresaManager empresaManager;
        private readonly IEmpleadoManager empleadoActivoFijoManager;
        private readonly IStringLocalizer<GeneradorContratoModel> localizer;
        private readonly AppUserManager userManager;

        private readonly Data.ApplicationDbContext db;

        [BindProperty]
        public Empresa? EmpresasList { get; set; }

        [BindProperty]
        public InputFiltroModel InputFiltro { get; set; }

        public class InputFiltroModel
        {
            [Display(Name = "Tipo Contrato")]
            public int? TipoContratoId { get; set; }

            [Display(Name = "Prestador")]
            public int? PrestadorId { get; set; }

            [Display(Name = "Prestatario")]
            public int? PrestatarioId { get; set; }
        }
    }
}