using ERPSEI.Areas.Reportes.Pages;
using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Requests;
using ERPSEI.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Net.Mime;
using ERPSEI.Pages.Shared;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Managers.Conciliaciones;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Managers.SAT;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empleados;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
// Para Excel usando EPPlus
using OfficeOpenXml;
using OfficeOpenXml.Style;

// Para PDF usando iTextSharp
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.AspNetCore.Identity;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Areas.Catalogos.Pages;
using ERPSEI.Data.Entities.Reportes;
using ERPSEI.Data.Managers.Reportes;
using NPOI.SS.Formula.Functions;
using Microsoft.DotNet.MSIdentity.Shared;
using static ERPSEI.Areas.ERP.Pages.ConciliacionesModel;
using ERPSEI.Data.Entities.SAT;
using ERPSEI.Data.Managers.SAT;
using ERPSEI.Data.Migrations;
using ERPSEI.Data.Managers.SAT.cfdiv40;

namespace ERPSEI.Areas.ERP.Pages
{
    public class ConciliacionesModel : ERPPageModel
    {
        private readonly IStringLocalizer<ConciliacionesModel> stringLocalizer;
        private readonly ILogger<ConciliacionesModel> logger;
        //private readonly IRCatalogoManager<Banco> bancoManager;
        private readonly IBancoManager bancoManager;
        private readonly IConciliacionManager conciliacionManager;
        private readonly IConciliacionDetalleManager conciliacionDetalleManager;
        private readonly IConciliacionDetalleComprobanteManager conciliacionDetalleComprobanteManager;
        private readonly IConciliacionDetalleMovimientoManager conciliacionDetalleMovimientoManager;
        private readonly IClienteManager clienteManager;
        private readonly IMovimientoBancarioManager movimientoBancarioManager;
        private readonly IEmpresaManager empresaManager;
        private readonly IEmpleadoManager _empleadoManager;
        private readonly IComprobanteManager comprobanteManager;
        private readonly IStringLocalizer<ConciliacionesModel> localizer;

        private readonly Data.ApplicationDbContext db;

        [BindProperty]
        public InputFiltroModel InputFiltro { get; set; }

        public class InputFiltroModel
        {
            [Display(Name = "IdField")]
            //[StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 1)]
            //[RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "PersonName")]
            public int? Id { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "ClienteField")]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Cliente { get; set; } = string.Empty;

            [DataType(DataType.Text)]
            [Display(Name = "UsuarioCreadorField")]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.AlphanumNoSpace, ErrorMessage = "PersonName")]
            public string? UsuarioCreador { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "UsuarioModificadorField")]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.AlphanumNoSpace, ErrorMessage = "PersonName")]
            public string? UsuarioModificador { get; set; }

            [Display(Name = "FechaElaboracionInicioField")]
            [Required(ErrorMessage = "Required")]
            [DataType(DataType.Date)]
            public DateTime? FechaElaboracionInicio { get; set; }

            [Display(Name = "FechaElaboracionFinField")]
            [Required(ErrorMessage = "Required")]
            [DataType(DataType.Date)]
            public DateTime? FechaElaboracionFin { get; set; }
        }

        [BindProperty]
        public InputFiltroModelAgregar InputFiltroModalAgregar { get; set; }
        public class InputFiltroModelAgregar
        {
            [Display(Name = "IdField")]
            //[StringLength(10, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.NumericNoRestriction, ErrorMessage = "PersonName")]
            public int? Id { get; set; }

            [Display(Name = "FechaElaboracionInicioField")]
            [Required(ErrorMessage = "Required")]
            [DataType(DataType.Date)]
            public DateTime? FechaElaboracionInicio { get; set; }

            [DataType(DataType.Text)]
            [Display(Name = "ClienteField")]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Cliente { get; set; } = string.Empty;

            [DataType(DataType.Text)]
            [Display(Name = "DescripcionField")]
            [StringLength(100, ErrorMessage = "FieldLength", MinimumLength = 1)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Descripcion { get; set; } = string.Empty;
        }

        [BindProperty]
        public InputFiltroModelDComprobantes InputFiltroModalDComprobantes { get; set; }

        public class InputFiltroModelDComprobantes
        {
            [Display(Name = "FechaInicioModalDComprobantesField")]
            [Required(ErrorMessage = "Required")]
            [DataType(DataType.Date)]
            public DateTime? FechaInicioModalDComprobantes { get; set; }

            [Display(Name = "FechaFinModalDComprobantesField")]
            [Required(ErrorMessage = "Required")]
            [DataType(DataType.Date)]
            public DateTime? FechaFinModalDComprobantes { get; set; }
        }

        [BindProperty]
        public Conciliacion? ConciliacionesList { get; set; }
        public Banco BancoList { get; set; }

        public ConciliacionesModel(
            IStringLocalizer<ConciliacionesModel> _stringLocalizer,
            ILogger<ConciliacionesModel> _logger,
            //IRCatalogoManager<Banco> _bancoManager,
            IBancoManager _bancoManager,
            IConciliacionManager _conciliacionManager,
            IConciliacionDetalleManager _conciliacionDetalleManager,
            IConciliacionDetalleComprobanteManager _conciliacionDetalleComprobanteManager,
            IConciliacionDetalleMovimientoManager _conciliacionDetalleMovimientoManager,
            IClienteManager _clienteManager,
            IMovimientoBancarioManager _movimientoBancarioManager,
            IEmpresaManager _empresaManager,
            IEmpleadoManager empleadoManager,
            IComprobanteManager _comprobanteManager,
            IStringLocalizer<ConciliacionesModel> _localizer,
            Data.ApplicationDbContext _db
        )
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            bancoManager = _bancoManager;
            conciliacionManager = _conciliacionManager;
            conciliacionDetalleManager = _conciliacionDetalleManager;
            conciliacionDetalleComprobanteManager = _conciliacionDetalleComprobanteManager;
            conciliacionDetalleMovimientoManager = _conciliacionDetalleMovimientoManager;
            clienteManager = _clienteManager;
            movimientoBancarioManager = _movimientoBancarioManager;
            empresaManager = _empresaManager;
            _empleadoManager = empleadoManager;
            comprobanteManager = _comprobanteManager;
            localizer = _localizer;
            db = _db;

            BancoList = new Banco();
            InputFiltro = new InputFiltroModel();
            InputFiltroModalDComprobantes = new InputFiltroModelDComprobantes();
            InputFiltroModalAgregar = new InputFiltroModelAgregar();
            ConciliacionesList = new Conciliacion();
        }

        public async Task<JsonResult> OnGetConciliacionesList()
        {
            List<string> jsonConciliaciones = new List<string>();
            List<Conciliacion> conciliaciones = await conciliacionManager.GetAllAsync();

            foreach (Conciliacion cons in conciliaciones)
            {
                string UsuarioCreador = "";
                string UsuarioModificador = "";

                if (cons.UsuarioCreador?.Empleado != null)
                {
                    UsuarioCreador = cons.UsuarioCreador.Empleado.NombreCompleto ?? string.Empty;
                }
                else 
                {
                    UsuarioCreador = cons.UsuarioCreador?.UserName ?? "-";
                }

                if (cons.UsuarioModificador?.Empleado != null)
                {
                    UsuarioModificador = cons.UsuarioModificador.Empleado.NombreCompleto ?? string.Empty;
                }
                else
                {
                    UsuarioModificador = cons.UsuarioModificador?.UserName ?? "-";
                }

                jsonConciliaciones.Add("{" +
                    $"\"id\": \"{cons.Id}\", " +
                    $"\"Fecha\": \"{cons.Fecha:dd/MM/yyyy HH:mm:ss}\", " +
                    $"\"FechaJS\": \"{cons.Fecha:yyyy-MM-dd HH:mm:ss}\", " +
                    $"\"Descripcion\": \"{cons.Descripcion}\", " +
                    $"\"Total\": \"{cons.Total}\", " +
                    $"\"BancoId\": \"{cons.BancoId}\", " +
                    $"\"Cliente\": \"{cons.Cliente?.RazonSocial}\", " +
                    $"\"EmpresaId\": \"{cons.EmpresaId}\", " +
                    $"\"UsuarioCreadorId\": \"{cons.UsuarioCreadorId}\", " +
                    $"\"UsuarioCreador\": \"{UsuarioCreador}\", " +
                    $"\"UsuarioModificadorId\": \"{cons.UsuarioModificadorId}\", " +
                    $"\"UsuarioModificador\": \"{UsuarioModificador}\", " +
                    $"\"Deshabilitado\": \"{cons.Deshabilitado}\"" +
                    "}");
            }

            string jsonResponse = $"[{string.Join(",", jsonConciliaciones)}]";
            return new JsonResult(jsonResponse);
        }

        public async Task<JsonResult> OnPostDeleteConciliaciones(string[] ids)
        {
            ServerResponse resp = new(true, stringLocalizer["ConciliacionesDeletedUnsuccessfully"]);
            try
            {
                await db.Database.BeginTransactionAsync();

                // Obtener las conciliaciones que coinciden con los ids proporcionados
                foreach (string id in ids)
                {
                    if (!int.TryParse(id, out int sid)) { sid = 0; }
                    Conciliacion? conciliacion = await conciliacionManager.GetByIdAsync(sid);

                    // Verificar si la conciliación es nula
                    if (conciliacion == null)
                    {
                        resp.TieneError = true;
                        resp.Mensaje = $"Conciliación con ID {sid} no encontrada.";
                        break;
                    }

                    // Marcar la conciliación como deshabilitada
                    conciliacion.Deshabilitado = true;
                    await conciliacionManager.UpdateAsync(conciliacion);
                }

                // Si hubo algún error, lanzar excepción para revertir la transacción
                //if (resp.TieneError) { throw new Exception(resp.Mensaje); }

                await db.Database.CommitTransactionAsync();
                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["ConciliacionesDeletedSuccessfully"];
            }
            catch (Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
                logger.LogError(ex.Message);
                resp.TieneError = true;
                resp.Mensaje = "Ocurrió un error al procesar la solicitud.";
            }

            return new JsonResult(resp);
        }


        public async Task<JsonResult> OnPostFiltrarConciliaciones()
        {
            // Inicializar la respuesta con mensaje de error por defecto
            ServerResponse resp = new(true, stringLocalizer["ConciliacionesFiltradosUnsuccessfully"]);

            try
            {
                resp.Datos = await GetConciliacionList(InputFiltro);
                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["ConciliacionesFiltradosSuccessfully"];
            }
            catch (Exception ex)
            {
                // Registrar el error en el log
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        private async Task<string> GetConciliacionList(InputFiltroModel? filtro = null)
        {
                List<object> jsonConciliaciones = new List<object>();
                List<Conciliacion> conciliaciones;

                // Aplicar los filtros de InputFiltro a la llamada a GetAllAsync
                if (filtro != null)
                {
                    conciliaciones = await conciliacionManager.GetAllAsync(
                        filtro.Id,
                        filtro.Cliente,
                        filtro.UsuarioCreador,
                        filtro.UsuarioModificador,
                        filtro.FechaElaboracionInicio,
                        filtro.FechaElaboracionFin
                    );
                }
                else
                {
                    // Si no hay filtros, obtener todos los registros
                    conciliaciones = await conciliacionManager.GetAllAsync();
                }

                // Construir el JSON con objetos anónimos
                foreach (Conciliacion cons in conciliaciones)
                {
                    string UsuarioCreador = cons.UsuarioCreador?.Empleado?.NombreCompleto ?? cons.UsuarioCreador?.UserName ?? "-";
                    string UsuarioModificador = cons.UsuarioModificador?.Empleado?.NombreCompleto ?? cons.UsuarioModificador?.UserName ?? "-";

                    jsonConciliaciones.Add("{" +
                    $"\"id\": \"{cons.Id}\", " +
                    $"\"Fecha\": \"{cons.Fecha:dd/MM/yyyy HH:mm:ss}\", " +
                    $"\"FechaJS\": \"{cons.Fecha:yyyy-MM-dd HH:mm:ss}\", " +
                    $"\"Descripcion\": \"{cons.Descripcion}\", " +
                    $"\"Total\": \"{cons.Total}\", " +
                    $"\"BancoId\": \"{cons.BancoId}\", " +
                    $"\"Cliente\": \"{cons.Cliente?.RazonSocial}\", " +
                    $"\"EmpresaId\": \"{cons.EmpresaId}\", " +
                    $"\"UsuarioCreadorId\": \"{cons.UsuarioCreadorId}\", " +
                    $"\"UsuarioCreador\": \"{UsuarioCreador}\", " +
                    $"\"UsuarioModificadorId\": \"{cons.UsuarioModificadorId}\", " +
                    $"\"UsuarioModificador\": \"{UsuarioModificador}\", " +
                    $"\"Deshabilitado\": \"{cons.Deshabilitado}\"" +
                    "}");
                }
                string jsonResponse = $"[{string.Join(",", jsonConciliaciones)}]";
                return jsonResponse;
        }

        /*public async Task<JsonResult> OnPostFiltrarComprobantesFechas()
        {
            // Inicializar la respuesta con mensaje de error por defecto
            ServerResponse resp = new(true, stringLocalizer["ComprobantesFiltradosUnsuccessfully"]);

            try
            {
                resp.Datos = await onGetConsultarComprobantes(InputFiltro);
                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["ComprobantesFiltradosSuccessfully"];
            }
            catch (Exception ex)
            {
                // Registrar el error en el log
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }*/

        /*public async Task<JsonResult> onGetConsultarComprobantes(InputFiltroModelDComprobantes? filtro = null)
        {
            try
            {
                List<object> jsonComprobantes = new List<object>();
                List<Comprobante> comprobantes;

                // Aplicar los filtros de InputFiltro a la llamada a GetAllAsync
                if (filtro != null)
                {
                    Comprobante = await comprobanteManager.GetAllAsync(
                        filtro.FechaInicioModalDComprobantes,
                        filtro.FechaFinModalDComprobantes,
                    );
                }
                else
                {
                    // Si no hay filtros, obtener todos los registros
                    conciliaciones = await conciliacionManager.GetAllAsync();
                }

                // Construir el JSON con objetos anónimos
                foreach (Conciliacion cons in conciliaciones)
                {
                    string UsuarioCreador = cons.UsuarioCreador?.Empleado?.NombreCompleto ?? cons.UsuarioCreador?.UserName ?? "-";
                    string UsuarioModificador = cons.UsuarioModificador?.Empleado?.NombreCompleto ?? cons.UsuarioModificador?.UserName ?? "-";

                    jsonConciliaciones.Add(new
                    {
                        id = cons.Id,
                        Fecha = cons.Fecha,
                        Descripcion = cons.Descripcion,
                        Total = cons.Total,
                        BancoId = cons.BancoId,
                        Cliente = cons.Cliente?.RazonSocial,
                        EmpresaId = cons.EmpresaId,
                        UsuarioCreadorId = cons.UsuarioCreadorId,
                        UsuarioCreador = UsuarioCreador,
                        UsuarioModificadorId = cons.UsuarioModificadorId,
                        UsuarioModificador = UsuarioModificador,
                        Deshabilitado = cons.Deshabilitado
                    });
                }

                // Retornar el JSON sin errores y con Datos como un array de objetos
                return new JsonResult(new { TieneError = false, Mensaje = "Operación exitosa", Datos = jsonConciliaciones });
            }
            catch (Exception ex)
            {
                // Registrar el error
                logger.LogError(ex, "Error al obtener la lista de conciliaciones.");

                // Retornar un mensaje de error
                return new JsonResult(new { TieneError = true, Mensaje = "Ocurrió un error al procesar la solicitud.", Datos = new List<object>() });
            }
        }*/

        public async Task<JsonResult> OnGetMovimientosList()
        {
            ServerResponse resp = new(true, stringLocalizer["AsistenciaSavedUnsuccessfully"]);

            try
            {
                //var bancos = await bancoManager.GetAllAsync();
                //resp = new ServerResponse(true, "Bancos recuperados correctamente", bancos);
            }
            catch (Exception ex)
            {
                //logger.LogError(ex.Message);
                //resp = new ServerResponse(false, "Error al recuperar los bancos");
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> OnPostSaveConciliacion()
        {
            ServerResponse resp = new(true, stringLocalizer["AsistenciaSavedUnsuccessfully"]);

            try
            {
                // Lógica para guardar la conciliación
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }
        public ActionResult OnGetDownloadPlantilla()
        {
            return File("/templates/PlantillaMovimientosBancarios.xlsx", MediaTypeNames.Application.Octet, "PlantillaMovimientosBancarios.xlsx");
        }

        public async Task<JsonResult> OnPostGetClientesEmpresasSuggestion(string texto)
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
            try
            {
                /*if (PuedeTodo || PuedeConsultar || PuedeEditar || PuedeEliminar)
                {*/
                    resp.Datos = await GetClientesEmpresasSuggestion(texto);
                    resp.TieneError = false;
                    resp.Mensaje = localizer["ConsultadoSuccessfully"];
                /*}
                else
                {
                    resp.Mensaje = localizer["AccesoDenegado"];
                }*/
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }
        private async Task<string> GetClientesEmpresasSuggestion(string texto)
        {
            string jsonResponse;
            List<string> jsonEmpresas = [];

            List<EmpresaBuscada> empresas = await empresaManager.SearchEmpresas(texto);

            if (empresas != null)
            {
                foreach (EmpresaBuscada e in empresas)
                {
                    string desc = $"{e.RFC} - {e.RazonSocial}";
                    jsonEmpresas.Add($"{{" +
                                        $"\"id\": \"{e.Id}\", " +
                                        $"\"value\": \"{desc}\", " +
                                        $"\"label\": \"{desc}\"" +
                                    $"}}");
                }
            }

            jsonResponse = $"[{string.Join(",", jsonEmpresas)}]";

            return jsonResponse;
        }
        /*private async Task<string> GetClientesEmpresas2Suggestion(string texto)
        {
            string jsonResponse;
            List<string> jsonClientes = [];

            List<ClienteBuscado> clientes = await clienteManager.SearchClientes(texto);

            if (clientes != null)
            {
                foreach (ClienteBuscado e in clientes)
                {
                    string desc = $"{e.NombreCliente} - {e.RazonSocial}";
                    jsonClientes.Add($"{{" +
                                        $"\"id\": \"{e.Id}\", " +
                                        $"\"value\": \"{desc}\", " +
                                        $"\"label\": \"{desc}\"" +
                                    $"}}");
                }
            }

            jsonResponse = $"[{string.Join(",", jsonClientes)}]";

            return jsonResponse;
        }*/

    }
}
