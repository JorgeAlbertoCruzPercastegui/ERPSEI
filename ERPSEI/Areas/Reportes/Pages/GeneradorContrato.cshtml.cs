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
using ERPSEI.Data.Entities.TipoContratos;
using ERPSEI.Data.Managers.TipoContratos;

namespace ERPSEI.Areas.Reportes.Pages
{
    public class GeneradorContratoModel : ERPPageModel
    {
        private readonly IStringLocalizer<GeneradorContratoModel> stringLocalizer;
        private readonly ILogger<GeneradorContratoModel> logger;
        private readonly AppUserManager appUserManager;
        private readonly IEmpresaManager empresaManager;
        private readonly ITipoContratosManager tipoContratosManager;
        private readonly IEmpresaContratosManager empresaContratosManager;
        private readonly IClienteContratosManager clienteContratosManager;
        private readonly IEmpleadoManager empleadoActivoFijoManager;
        private readonly IStringLocalizer<GeneradorContratoModel> localizer;
        private readonly AppUserManager userManager;

        private readonly Data.ApplicationDbContext db;

        [BindProperty]
        public Empresa? EmpresasList { get; set; }

        [BindProperty]
        public EmpresaContrato? EmpresaContratosList { get; set; }

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

        public class EmpresaContratoTableModel
        {
            public int? Id { get; set; }

            [Required(ErrorMessage = "La razón social es obligatoria.")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Longitud inválida.")]
            public string? RazonSocial { get; set; }

            [StringLength(200)]
            [Display(Name = "Domicilio Fiscal")]
            public string? DomicilioFiscal { get; set; }

            [Required(ErrorMessage = "El RFC es obligatorio.")]
            [StringLength(13, MinimumLength = 12, ErrorMessage = "RFC inválido.")]
            public string? RFC { get; set; }

            [Display(Name = "N° de Notario")]
            public int? NoNotario { get; set; }

            [StringLength(100)]
            public string? Notario { get; set; }

            [StringLength(100)]
            [Display(Name = "Representante Legal")]
            public string? RepresentanteLegal { get; set; }

            [EmailAddress(ErrorMessage = "Correo inválido")]
            [StringLength(100)]
            public string? Email { get; set; }

            [Url(ErrorMessage = "URL inválida")]
            [Display(Name = "Página Web")]
            public string? PaginaWeb { get; set; }

            [Display(Name = "Fecha de Constitución")]
            [DataType(DataType.Date)]
            public DateTime? FechaConstitucion { get; set; }

            [Display(Name = "Deshabilitado")]
            public bool Deshabilitado { get; set; }

            [Display(Name = "Tipo de Contrato")]
            public int? TipoContratoId { get; set; }
        }


        public GeneradorContratoModel(
            IStringLocalizer<GeneradorContratoModel> _stringLocalizer,
            ILogger<GeneradorContratoModel> _logger,
            AppUserManager _appUserManager,
            IStringLocalizer<GeneradorContratoModel> _localizer,
            Data.ApplicationDbContext _db,
            AppUserManager _userManager,
            ITipoContratosManager _tipoContratosManager,
            IEmpresaContratosManager _empresaContratosManager,
            IClienteContratosManager _clienteContratosManager
            ) 
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            appUserManager = _appUserManager;
            localizer = _localizer;
            db = _db;
            userManager = _userManager;

            tipoContratosManager = _tipoContratosManager;
            empresaContratosManager = _empresaContratosManager;
            clienteContratosManager = _clienteContratosManager;

            EmpresaContratosList = new EmpresaContrato();
        }

        public async Task<JsonResult> OnGetEmpresaContratosList()
        {
            var empresas = await empresaContratosManager.GetAllAsync();
            empresas = empresas.Where(e => !e.Deshabilitado).ToList();

            var jsonEmpresas = new List<object>();

            foreach (var e in empresas)
            {
                jsonEmpresas.Add(new
                {
                    id = e.Id,
                    razonSocial = e.RazonSocial ?? "-",
                    domicilioFiscal = e.DomicilioFiscal ?? "-",
                    rfc = e.RFC ?? "-",
                    noNotario = e.NoNotario?.ToString() ?? "-",
                    notario = e.Notario ?? "-",
                    representanteLegal = e.RepresentanteLegal ?? "-",
                    email = e.Email ?? "-",
                    paginaWeb = e.PaginaWeb ?? "-",
                    fechaConstitucion = e.FechaConstitucion?.ToString("dd/MM/yyyy") ?? "-",
                    fechaConstitucionJS = e.FechaConstitucion?.ToString("yyyy-MM-dd") ?? "-",
                    tipoContrato = e.TipoContrato?.Nombre ?? "-",
                    tipoContratoId = e.TipoContratoId,
                    deshabilitado = e.Deshabilitado.ToString()
                });
            }

            return new JsonResult(jsonEmpresas);
        }

        public async Task<JsonResult> OnPostDeleteEmpresaContratos(string[] ids)
        {
            var resp = new ServerResponse(true, localizer["ConsultadoUnsuccessfully"]);

            try
            {
                await db.Database.BeginTransactionAsync();

                foreach (string id in ids)
                {
                    if (!int.TryParse(id, out int intId))
                        continue;

                    var empresa = await db.EmpresaContratos.FirstOrDefaultAsync(e => e.Id == intId);

                    if (empresa == null)
                        continue;

                    empresa.Deshabilitado = true;

                    db.EmpresaContratos.Update(empresa);
                }

                await db.SaveChangesAsync();
                await db.Database.CommitTransactionAsync();

                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
                logger.LogError(ex, "Error al dar de baja empresa contratos");
                resp.Mensaje = "Ocurrió un error al dar de baja los registros.";
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> OnGetClientesPorEmpresa(int id)
        {
            var clientes = await clienteContratosManager.GetByEmpresaContratoIdAsync(id);

            var result = clientes.Select(c => new
            {
                id = c.Id,
                nombre = c.RazonSocial ?? "-",
                rfc = c.RFC ?? "-",
                domicilioFiscal = c.DomicilioFiscal ?? "-",
                representanteLegal = c.RepresentanteLegal ?? "-",
                noNotario = c.NoNotario ?? 0,
                notario = c.Notario ?? "-"
            });

            return new JsonResult(result);
        }

    }
}