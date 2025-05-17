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
        public InputFiltroModel InputFiltro { get; set; }

        public class InputFiltroModel
        {
            //[StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 1)]
            //[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "PersonName")]
            public int? Folio { get; set; }

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
        }

        public ActivosFijosModel(
            IStringLocalizer<ActivosFijosModel> _stringLocalizer,
            ILogger<ActivosFijosModel> _logger,
            AppUserManager _appUserManager,
            IStringLocalizer<ActivosFijosModel> _localizer,
            Data.ApplicationDbContext _db
        )
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            appUserManager = _appUserManager;
            localizer = _localizer;
            db = _db;

            InputFiltro = new InputFiltroModel();
            InputActivosFijos = new ActivoFijoTableModel();
        }


        public async Task<JsonResult> OnPostFiltrar()
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
            try
            {
                if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
                {
                    resp.Datos = await GetActivosFijosList(InputFiltro); // usa el método adaptado que ya hicimos
                    resp.TieneError = false;
                    resp.Mensaje = localizer["ConsultadoSuccessfully"];
                }
                else
                {
                    resp.Mensaje = localizer["AccesoDenegado"];
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al filtrar activos fijos");
            }

            return new JsonResult(resp);
        }

        private async Task<string> GetDatosAdicionales(int idActivo)
        {
            string jsonResponse;

            ActivoFijo? a = await activoFijoManager.GetByIdAsync(idActivo)
                ?? throw new Exception($"No se encontró información del activo fijo con id {idActivo}");

            jsonResponse = "{" +
                $"\"id\": {a.Id}, " +
                $"\"folio\": \"{a.Folio}\", " +
                $"\"descripcion\": \"{a.Descripcion}\", " +
                $"\"responsable\": \"{a.Empleado?.NombreCompleto}\", " +
                $"\"categoria\": \"{a.Categoria?.Descripcion}\", " +
                $"\"tipo\": \"{a.Tipo?.Descripcion}\", " +
                $"\"fechaCompra\": \"{a.FechaCompra:dd/MM/yyyy}\", " +
                $"\"precio\": {a.Precio}, " +
                $"\"linkFacturaCompra\": \"{a.LinkFacturaCompra}\", " +
                $"\"comentarios\": \"{a.Comentarios}\" " +
            "}";

            return jsonResponse;
        }


        private async Task<string> GetActivosFijosList(InputFiltroModel? filtro = null)
        {
            string jsonResponse;
            List<string> jsonActivos = [];
            List<ActivoFijo> activos;

            if (filtro != null)
            {
                activos = await activoFijoManager.GetFilteredAsync(
                    filtro.Folio,
                    filtro.Responsable,
                    filtro.CategoriaId,
                    filtro.TipoId,
                    filtro.FechaCompraInicio,
                    filtro.FechaCompraFin
                );
            }
            else
            {
                activos = await activoFijoManager.GetAllAsync();
            }

            foreach (ActivoFijo a in activos)
            {
                DateTime? fecha = a.FechaCompra == DateTime.MinValue ? null : a.FechaCompra;

                jsonActivos.Add(
                    "{" +
                        $"\"id\": {a.Id}," +
                        $"\"folio\": \"{a.Folio}\", " +
                        $"\"descripcion\": \"{a.Descripcion}\", " +
                        $"\"responsable\": \"{a.Empleado?.NombreCompleto}\", " +
                        $"\"responsableId\": {a.EmpleadoId}, " +
                        $"\"categoria\": \"{a.Categoria?.Descripcion}\", " +
                        $"\"categoriaId\": {a.CategoriaId}, " +
                        $"\"tipo\": \"{a.Tipo?.Descripcion}\", " +
                        $"\"tipoId\": {a.TipoId}, " +
                        $"\"fechaCompra\": \"{fecha:dd/MM/yyyy}\", " +
                        $"\"fechaCompraJS\": \"{fecha:yyyy-MM-dd}\", " +
                        $"\"precio\": {a.Precio}, " +
                        $"\"linkFacturaCompra\": \"{a.LinkFacturaCompra}\"" +
                    "}"
                );
            }

            jsonResponse = $"[{string.Join(",", jsonActivos)}]";
            return jsonResponse;
        }

    }
}