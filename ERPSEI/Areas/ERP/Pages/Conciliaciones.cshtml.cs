using ERPSEI.Data.Entities.Conciliaciones;
using ERPSEI.Requests;
using ERPSEI.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Net.Mime;
using ERPSEI.Pages.Shared;
using ERPSEI.Data.Managers.Conciliaciones;
using ERPSEI.Data.Entities.SAT.cfdiv40;
using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Managers.SAT.cfdiv40;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SixLabors.ImageSharp.PixelFormats;
using iText.Commons.Actions.Contexts;
using ERPSEI.Data.Entities.Cuentas;
using ERPSEI.Data.Managers.Cuentas;
using Newtonsoft.Json;
using System.Text.Json;

namespace ERPSEI.Areas.ERP.Pages
{
    public class ConciliacionesModel : ERPPageModel
    {
        private readonly IStringLocalizer<ConciliacionesModel> stringLocalizer;
        private readonly ILogger<ConciliacionesModel> logger;
        private readonly IBancoManager bancoManager;
        private readonly IConciliacionManager conciliacionManager;
        private readonly IConciliacionDetalleManager conciliacionDetalleManager;
        private readonly IConciliacionDetalleComprobanteManager conciliacionDetalleComprobanteManager;
        private readonly IConciliacionDetalleMovimientoManager conciliacionDetalleMovimientoManager;
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

            [DataType(DataType.Text)]
            public string? rfc { get; set; } = string.Empty;

            [DataType(DataType.Text)]
            public string? UsuarioCreador { get; set; }

            [DataType(DataType.Text)]
            public string? UsuarioModificador { get; set; }

            public int BancoId { get; set; }
            public int EmpresaId { get; set; }
            public bool Finalizada { get; set; } = false;
            public List<MovimientoBancario> Movimientos { get; set; } = new List<MovimientoBancario>();
            public List<Comprobante> Comprobantes { get; set; } = new List<Comprobante>();

        }

        [BindProperty]
        public InputFiltroModelDComprobantes InputFiltroModalDComprobantes { get; set; }

        public class InputFiltroModelDComprobantes
        {
            [Display(Name = "FechaInicioModalDComprobantesField")]
            [Required(ErrorMessage = "Required")]
            public string? FechaInicioModalDComprobantes { get; set; }

            [Display(Name = "FechaFinModalDComprobantesField")]
            [Required(ErrorMessage = "Required")]
            public string? FechaFinModalDComprobantes { get; set; }

            [Display(Name = "ClienteIdField")]
            [Required(ErrorMessage = "Required")]
            public int? ClienteId { get; set; }
        }


        [BindProperty]
        public Conciliacion? ConciliacionesList { get; set; }
        public Banco BancoList { get; set; }

        public MovimientoBancario movimientoBancario { get; set; }
        public class MovimientoBancario
        {
            [Display(Name = "Fecha")]
            public DateTime? Fecha { get; set; }

            [Display(Name = "Descripción")]
            public string? Descripcion { get; set; }

            [Display(Name = "Importe")]
            public decimal? Importe { get; set; }

            [Display(Name = "Banco")]
            public string Banco { get; set; } = string.Empty;

            [Display(Name = "Conciliado")]
            public bool Conciliado { get; set; } = false;
        }

        public ConciliacionesModel(
            IStringLocalizer<ConciliacionesModel> _stringLocalizer,
            ILogger<ConciliacionesModel> _logger,
            IBancoManager _bancoManager,
            IConciliacionManager _conciliacionManager,
            IConciliacionDetalleManager _conciliacionDetalleManager,
            IConciliacionDetalleComprobanteManager _conciliacionDetalleComprobanteManager,
            IConciliacionDetalleMovimientoManager _conciliacionDetalleMovimientoManager,
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
            movimientoBancarioManager = _movimientoBancarioManager;
            empresaManager = _empresaManager;
            _empleadoManager = empleadoManager;
            comprobanteManager = _comprobanteManager;
            localizer = _localizer;
            db = _db;

            movimientoBancario = new MovimientoBancario();
            BancoList = new Banco();
            InputFiltro = new InputFiltroModel();
            InputFiltroModalDComprobantes = new InputFiltroModelDComprobantes();
            InputFiltroModalAgregar = new InputFiltroModelAgregar();
            ConciliacionesList = new Conciliacion();
        }

        public async Task<JsonResult> OnGetExportarExcel(int id)
        {
            ServerResponse resp = new(true, localizer["ExportExcelUnsuccessfully"]);
            try
            {
                resp.Datos = await GetExportarExcel(id);
                resp.TieneError = false;
                resp.Mensaje = localizer["ExportExcelSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError("{message}", ex.Message);
                resp.Mensaje = ex.Message;
            }

            return new JsonResult(resp);
        }

        public async Task<List<object>> GetExportarExcel(int conciliacionId)
        {
            try
            {
                // Obtener la conciliación por ID
                var conciliacion = await conciliacionManager.GetByIdAsync(conciliacionId);

                // Crear una lista para almacenar los datos del Excel
                var datosExcel = new List<object>();

                // Recorrer los detalles de la conciliación y preparar los datos
                foreach (var detalle in conciliacion.DetallesConciliacion)
                {
                    foreach (var comprobante in detalle.ConciliacionesDetallesComprobantes)
                    {
                        // Obtener el TotalImpuestosTrasladados del comprobante
                        var totalImpuestosTrasladados = await conciliacionManager.GetTotalImpuestosTrasladadosAsync(comprobante.Comprobante?.Impuestos?.Id);

                        // Obtener los datos del receptor
                        var rfcReceptor = comprobante.Comprobante?.Receptor?.Rfc ?? "N/A";
                        var nombreReceptor = comprobante.Comprobante?.Receptor?.Nombre ?? "N/A";

                        foreach (var movimiento in detalle.ConciliacionesDetallesMovimientos)
                        {
                            datosExcel.Add(new
                            {
                                Cliente = conciliacion.Cliente?.RazonSocial ?? "Sin Cliente",
                                ComprobanteId = comprobante.Comprobante?.Id ?? 0,
                                Serie = comprobante.Comprobante?.Serie ?? "N/A",
                                Folio = comprobante.Comprobante?.Folio ?? "N/A",
                                Total = comprobante.Comprobante?.Total ?? 0,
                                MovimientoId = movimiento.MovimientoBancario?.Id ?? 0,
                                DescripcionMovimiento = movimiento.MovimientoBancario?.Descripcion ?? "N/A",
                                Cargos = movimiento.MovimientoBancario?.Importe ?? 0,
                                Fecha = comprobante.Comprobante?.Fecha ?? "N/A",
                                //Subtotal = comprobante.Comprobante?.SubTotal ?? 0,
                                TotalImpuestosTrasladados = totalImpuestosTrasladados,
                                RfcReceptor = rfcReceptor,
                                NombreReceptor = nombreReceptor
                            });
                        }
                    }
                }

                return datosExcel;
            }
            catch (Exception ex)
            {
                logger.LogError("{message}", ex.Message);
                return null;
            }
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
                    $"\"Finalizada\": \"{(cons.Finalizada ? "Finalizada" : "En progreso")}\", " +
                    $"\"Deshabilitado\": \"{cons.Deshabilitado}\"" +
                    "}");
            }

            string jsonResponse = $"[{string.Join(",", jsonConciliaciones)}]";
            return new JsonResult(jsonResponse);
        }

        public async Task<JsonResult> OnGetFinalizarConciliaciones(int id)
        {
            ServerResponse resp = new(true, stringLocalizer["ConciliacionFinalizadaUnsuccessfully"]);
            try
            {
                await db.Database.BeginTransactionAsync();

                // Obtener la conciliación por ID
                Conciliacion? conciliacion = await conciliacionManager.GetByIdAsync(id);

                // Verificar si la conciliación es nula
                if (conciliacion == null)
                {
                    resp.TieneError = true;
                    resp.Mensaje = stringLocalizer["ConciliacionFinalizadaUnSuccessfully"];
                    return new JsonResult(resp);
                }

                // Marcar la conciliación como finalizada
                conciliacion.Finalizada = true;
                await conciliacionManager.UpdateAsync(conciliacion);

                await db.Database.CommitTransactionAsync();

                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["ConciliacionFinalizadaSuccessfully"];
            }
            catch (Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
                logger.LogError(ex, stringLocalizer["ConciliacionErrorfinalizando"]);
                resp.TieneError = true;
                resp.Mensaje = ex.Message;
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> OnGetComprobantesList()
        {
            List<string> jsonComprobantes = new List<string>();
            List<Comprobante> comprobantes = await comprobanteManager.GetAllAsync();

            foreach (Comprobante comp in comprobantes)
            {
                // Inicializa el UUID como vacío o nulo por defecto
                string uuid = string.Empty;

                // Verifica si Complemento y TimbreFiscalDigital no son nulos antes de acceder a UUID
                if (comp.Complemento != null && comp.Complemento.TimbreFiscalDigital != null)
                {
                    uuid = comp.Complemento.TimbreFiscalDigital.UUID ?? string.Empty;
                }

                // Construir el JSON con el UUID y los demás campos
                jsonComprobantes.Add("{" +
                    $"\"Id\": \"{comp.Id}\", " +
                    $"\"Serie\": \"{comp.Serie}\", " +
                    $"\"Folio\": \"{comp.Folio}\", " +
                    $"\"Fecha\": \"{comp.Fecha}\", " +
                    $"\"UUID\": \"{uuid}\", " +
                    $"\"Total\": \"{comp.Total}\"" +
                    "}");
            }

            string jsonResponse = $"[{string.Join(",", jsonComprobantes)}]";
            return new JsonResult(jsonResponse);
        }

        public async Task<JsonResult> OnGetComprobantesMovimientosList(int id)
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
                try
                {
                    resp.Datos = await CreateJsonComprobantesMovimientosList(id);
                    resp.TieneError = false;
                    resp.Mensaje = localizer["ConsultadoSuccessfully"];
                }
                catch (Exception ex)
                {
                    logger.LogError("{message}", ex.Message);
                    resp.Mensaje = ex.Message;
                }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> CreateJsonComprobantesMovimientosList(int id)
        {
            ServerResponse resp = new(true, stringLocalizer["ComprobantesFiltradosUnsuccessfully"]);

            try
            {
                string jsonConciliacion = string.Empty;
                Conciliacion? c = await conciliacionManager.GetByIdAsync(id);
                string jsonDetalles = string.Empty;

                // Obtenemos los detalles y les agregamos un identificador único
                List<string> detallesConId = new();
                int contadorId = 1;

                foreach (var detalle in c.DetallesConciliacion)
                {
                    // Generar JSON para comprobantes y movimientos
                    string jsonComprobantes = CreateJsonComprobantes([.. detalle.ConciliacionesDetallesComprobantes]);
                    string jsonMovimientos = CreateJsonMovimientos([.. detalle.ConciliacionesDetallesMovimientos]);
                    string jsonResultadosComprobantes = CreateJsonResultados([.. detalle.ConciliacionesDetallesComprobantes]);
                    string jsonResultadosMovimientos = CreateJsonResultadosMovimientos([.. detalle.ConciliacionesDetallesMovimientos]);

                    // Agregar los detalles al JSON
                    detallesConId.Add(
                        "{" +
                            $"\"id\": \"{contadorId++}\"," +
                            $"\"detallesComprobantes\": {jsonComprobantes}," +
                            $"\"detallesMovimientos\": {jsonMovimientos}," +
                            $"\"resultadosComprobantes\": {jsonResultadosComprobantes}," +
                            $"\"resultadosMovimientos\": {jsonResultadosMovimientos}" +
                        "}"
                    );
                }

                jsonDetalles = $"[{string.Join(",", detallesConId)}]";

                // Construimos el JSON final para la respuesta
                jsonConciliacion = "{" +
                    $"\"detalles\": {jsonDetalles}" +
                    "}";

                resp.Mensaje = stringLocalizer["ComprobantesFiltradosSuccessfully"];
                resp.Datos = jsonConciliacion;
            }
            catch (Exception ex)
            {
                logger.LogError("{message}", ex.Message);
                resp.Mensaje = ex.Message;
            }

            return new JsonResult(resp);
        }

        private static string CreateJsonDetalles(List<ConciliacionDetalle> detalles)
        {
            List<string> jsonDetalles = [];
            string jsonComprobantes = string.Empty;
            string jsonMovimientos = string.Empty;
            string jsonResponse;
            foreach (ConciliacionDetalle cc in detalles)
            {
                jsonComprobantes = CreateJsonComprobantes([.. cc.ConciliacionesDetallesComprobantes]);
                jsonMovimientos = CreateJsonMovimientos([.. cc.ConciliacionesDetallesMovimientos]);

                jsonDetalles.Add(
                    "{" +
                        $"\"detallesComprobantes\":{jsonComprobantes}," +
                        $"\"detallesMovimientos\":{jsonMovimientos}" +
                    "}");
            }

            jsonResponse = $"[{string.Join(",", jsonDetalles)}]";

            return jsonResponse;

        }

        private static string CreateJsonComprobantes(List<ConciliacionDetalleComprobante> detalles)
        {
            List<string> jsonDetalles = [];
            string jsonResponse;
            foreach (ConciliacionDetalleComprobante cc in detalles)
            {
                string uuid = string.Empty;
                if (cc.Comprobante?.Complemento?.TimbreFiscalDigital != null)
                {
                    uuid = cc.Comprobante.Complemento.TimbreFiscalDigital.UUID ?? "-";
                }

                jsonDetalles.Add("{" +
                    $"\"Id\": {cc.Comprobante?.Id}, " +
                    $"\"Serie\": \"{cc.Comprobante?.Serie}\", " +
                    $"\"Folio\": \"{cc.Comprobante?.Folio}\", " +
                    $"\"Fecha\": \"{cc.Comprobante?.Fecha:dd/MM/yyyy HH:mm:ss}\", " +
                    $"\"FechaJS\": \"{cc.Comprobante?.Fecha:yyyy-MM-dd HH:mm:ss}\", " +
                    $"\"UUID\": \"{uuid}\", " +
                    $"\"Total\": \"{cc.Comprobante?.Total}\"" +
                    "}");

            }

            jsonResponse = $"[{string.Join(",", jsonDetalles)}]";

            return jsonResponse;
        }

        private static string CreateJsonMovimientos(List<ConciliacionDetalleMovimiento> detalles)
        {
            List<string> jsonDetalles = [];
            string jsonResponse;
            foreach (ConciliacionDetalleMovimiento cc in detalles)
            {

                jsonDetalles.Add("{" +
                    $"\"Id\":\"{cc.MovimientoBancario?.Id}\"," +
                    $"\"Fecha\":\"{cc.MovimientoBancario?.Fecha:yyyy-MM-dd HH:mm:ss}\"," +
                    $"\"Descripción\":\"{cc.MovimientoBancario?.Descripcion}\"," +
                    $"\"Cargos\":\"{cc.MovimientoBancario?.Importe}\"" +
                "}");
            }

            jsonResponse = $"[{string.Join(",", jsonDetalles)}]";

            return jsonResponse;
        }

        private static string CreateJsonResultados(List<ConciliacionDetalleComprobante> detalles)
        {
            List<string> jsonDetalles = [];
            string jsonResponse;
            foreach (var cc in detalles)
            {
                jsonDetalles.Add("{" +
                    $"\"Id\": {cc.Comprobante?.Id}, " +
                    $"\"Serie\": \"{cc.Comprobante?.Serie}\", " +
                    $"\"Folio\": \"{cc.Comprobante?.Folio}\", " +
                    $"\"Fecha\": \"{cc.Comprobante?.Fecha:dd/MM/yyyy HH:mm:ss}\", " +
                    $"\"Total\": \"{cc.Comprobante?.Total}\", " +
                    $"\"Similitud\": \"100.00%\"" +
                "}");
            }

            jsonResponse = $"[{string.Join(",", jsonDetalles)}]";

            return jsonResponse;
        }

        private static string CreateJsonResultadosMovimientos(List<ConciliacionDetalleMovimiento> detalles)
        {
            List<string> jsonDetalles = [];
            string jsonResponse;

            foreach (var cc in detalles)
            {
                // Extraer información del movimiento bancario
                var movimiento = cc.MovimientoBancario;
                if (movimiento != null)
                {
                    jsonDetalles.Add("{" +
                        $"\"Id\": \"{movimiento.Id}\", " +
                        $"\"Fecha\": \"{movimiento.Fecha:dd/MM/yyyy HH:mm:ss}\", " +
                        $"\"Descripcion\": \"{movimiento.Descripcion ?? "Sin descripción"}\", " +
                        $"\"Banco\": \"{movimiento.Conciliacion?.BancoId}\", " +
                        $"\"Total\": \"{movimiento.Importe}\", " +
                        $"\"Similitud\": \"100.00%\"" +
                    "}");
                }
            }

            jsonResponse = $"[{string.Join(",", jsonDetalles)}]";

            return jsonResponse;
        }

        public async Task<JsonResult> OnGetProcessedConciliacionList(int id)
        {
            ServerResponse resp = new(true, stringLocalizer["ComprobantesFiltradosUnsuccessfully"]);
            try
            {
                Conciliacion? c = await conciliacionManager.GetByIdAsync(id);
                if (c == null)
                {
                    resp.Mensaje = stringLocalizer["ConciliacionSuccessfully"];
                    return new JsonResult(resp);
                }

                List<object> comprobantes = new();
                List<object> movimientos = new();
                List<object> conciliaciones = new();
                HashSet<int> idsConciliados = new();
                Dictionary<int, object> mapaMovimientos = new();

                // Procesar detalles de movimientos
                foreach (var detalle in c.DetallesConciliacion)
                {
                    foreach (var movimiento in detalle.ConciliacionesDetallesMovimientos)
                    {
                        // Extraer el Banco desde la tabla Conciliaciones
                        string? banco = detalle.Conciliacion?.Banco?.Nombre ?? "Banco no especificado";

                        var movimientoData = new
                        {
                            Id = movimiento.Id,
                            Fecha = movimiento.MovimientoBancario.Fecha?.ToString("yyyy-MM-dd"),
                            Descripcion = movimiento.MovimientoBancario.Descripcion ?? "Sin descripción",
                            Cargos = movimiento.MovimientoBancario.Importe ?? 0,
                            Abonos = 0,
                            Banco = banco,
                            bloqueado = true
                        };
                        mapaMovimientos[movimiento.Id] = movimientoData;
                        movimientos.Add(movimientoData);
                    }
                }

                // Procesar detalles de comprobantes y asociar movimientos
                foreach (var detalle in c.DetallesConciliacion)
                {
                    foreach (var comprobante in detalle.ConciliacionesDetallesComprobantes)
                    {
                        if (idsConciliados.Contains(comprobante.Id)) continue;
                        idsConciliados.Add(comprobante.Id);

                        // Buscar movimientos asociados
                        List<object> movimientosConciliados = new();
                        foreach (var mov in detalle.ConciliacionesDetallesMovimientos)
                        {
                            if (mapaMovimientos.ContainsKey(mov.Id))
                            {
                                movimientosConciliados.Add(mapaMovimientos[mov.Id]);
                            }
                        }

                        string? banco = detalle.Conciliacion?.Banco?.Nombre ?? "Banco no especificado";

                        // Crear objeto de comprobante
                        var comprobanteData = new
                        {
                            Id = comprobante.Id,
                            Serie = comprobante.Comprobante.Serie,
                            Folio = comprobante.Comprobante.Folio,
                            Fecha = comprobante.Comprobante.Fecha?.ToString(),
                            Banco = banco,
                            UUID = comprobante.Comprobante.Complemento?.TimbreFiscalDigital?.UUID ?? "UUID no disponible",
                            Receptor = comprobante.Comprobante.Receptor?.Nombre ?? "Receptor no especificado",
                            Total = comprobante.Comprobante.Total,
                            movimientosConciliados,
                            bloqueado = true
                        };
                        comprobantes.Add(comprobanteData);

                        // Agregar a conciliaciones para tableResult
                        conciliaciones.Add(comprobanteData);
                    }
                }

                // Preparar la respuesta
                var resultData = new
                {
                    comprobantes,
                    movimientos,
                    conciliaciones
                };

                resp.Mensaje = stringLocalizer["ComprobantesFiltradosSuccessfully"];
                resp.Datos = JsonConvert.SerializeObject(resultData);
                resp.TieneError = false;
            }
            catch (Exception ex)
            {
                logger.LogError("{message}", ex.Message);
                resp.Mensaje = ex.Message;
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> OnGetProcessedConciliacionEditList(int id)
        {
            ServerResponse resp = new(true, stringLocalizer["ConciliacionFiltradosUnsuccessfully"]);
            try
            {
                Conciliacion? c = await conciliacionManager.GetByIdAsync(id);
                if (c == null)
                {
                    resp.Mensaje = stringLocalizer["ConciliacionSuccessfully"];
                    return new JsonResult(resp);
                }

                List<object> comprobantes = new();
                List<object> movimientos = new();
                List<object> conciliaciones = new();
                HashSet<int> idsConciliados = new();
                Dictionary<int, object> mapaMovimientos = new();

                // Procesar detalles de movimientos
                foreach (var detalle in c.DetallesConciliacion)
                {
                    foreach (var movimiento in detalle.ConciliacionesDetallesMovimientos)
                    {
                        // Extraer el Banco desde la tabla Conciliaciones
                        string? banco = detalle.Conciliacion?.Banco?.Nombre ?? "Banco no especificado";

                        var movimientoData = new
                        {
                            Id = movimiento.Id,
                            Fecha = movimiento.MovimientoBancario.Fecha?.ToString("yyyy-MM-dd"),
                            Descripcion = movimiento.MovimientoBancario.Descripcion ?? "Sin descripción",
                            Cargos = movimiento.MovimientoBancario.Importe ?? 0,
                            Abonos = 0,
                            Banco = banco,
                            bloqueado = true
                        };
                        mapaMovimientos[movimiento.Id] = movimientoData;
                        movimientos.Add(movimientoData);
                    }
                }

                // Procesar detalles de comprobantes y asociar movimientos
                foreach (var detalle in c.DetallesConciliacion)
                {
                    foreach (var comprobante in detalle.ConciliacionesDetallesComprobantes)
                    {
                        if (idsConciliados.Contains(comprobante.Id)) continue;
                        idsConciliados.Add(comprobante.Id);

                        // Buscar movimientos asociados
                        List<object> movimientosConciliados = new();
                        foreach (var mov in detalle.ConciliacionesDetallesMovimientos)
                        {
                            if (mapaMovimientos.ContainsKey(mov.Id))
                            {
                                movimientosConciliados.Add(mapaMovimientos[mov.Id]);
                            }
                        }

                        string? banco = detalle.Conciliacion?.Banco?.Nombre ?? "Banco no especificado";

                        // Crear objeto de comprobante
                        var comprobanteData = new
                        {
                            Id = comprobante.Id,
                            Serie = comprobante.Comprobante.Serie,
                            Folio = comprobante.Comprobante.Folio,
                            Fecha = comprobante.Comprobante.Fecha?.ToString(),
                            Banco = banco,
                            UUID = comprobante.Comprobante.Complemento?.TimbreFiscalDigital?.UUID ?? "UUID no disponible",
                            Receptor = comprobante.Comprobante.Receptor?.Nombre ?? "Receptor no especificado",
                            Total = comprobante.Comprobante.Total,
                            movimientosConciliados
                        };
                        comprobantes.Add(comprobanteData);

                        // Agregar a conciliaciones para tableResult
                        conciliaciones.Add(comprobanteData);
                    }
                }

                // Preparar la respuesta
                var resultData = new
                {
                    comprobantes,
                    movimientos,
                    conciliaciones
                };

                resp.Mensaje = stringLocalizer["ComprobantesFiltradosSuccessfully"];
                resp.Datos = JsonConvert.SerializeObject(resultData);
                resp.TieneError = false;
            }
            catch (Exception ex)
            {
                logger.LogError("{message}", ex.Message);
                resp.Mensaje = ex.Message;
            }

            return new JsonResult(resp);
        }


        public async Task<JsonResult> OnPostComprobantesListEmpresas()
        {
            // Inicializar la respuesta con mensaje de error por defecto
            ServerResponse resp = new(true, stringLocalizer["ComprobantesFiltradosUnsuccessfully"]);

            try
            {
                resp.Datos = await GetComprobantesListEmpresas(InputFiltroModalAgregar.rfc);
                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["ComprobantesFiltradosSuccessfully"];
            }
            catch (Exception ex)
            {
                // Registrar el error en el log
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        private async Task<string> GetComprobantesListEmpresas(string? rfc = null)
        {
            List<string> jsonComprobantes = new List<string>();
            List<Comprobante> comprobantes;

            if (!string.IsNullOrEmpty(rfc))
            {
                // Filtra los comprobantes según el RFC filtrado
                comprobantes = await comprobanteManager.GetByRFCAsync(rfc);
            }
            else
            {
                // Si no se proporciona RFC, obtiene todos los comprobantes
                comprobantes = await comprobanteManager.GetAllAsync();
            }

            foreach (Comprobante comp in comprobantes)
            {
                string uuid = string.Empty;

                if (comp.Complemento != null && comp.Complemento.TimbreFiscalDigital != null)
                {
                    uuid = comp.Complemento.TimbreFiscalDigital.UUID ?? string.Empty;
                }

                jsonComprobantes.Add("{" +
                    $"\"Id\": \"{comp.Id}\", " +
                    $"\"Serie\": \"{comp.Serie}\", " +
                    $"\"Folio\": \"{comp.Folio}\", " +
                    $"\"Fecha\": \"{comp.Fecha:dd/MM/yyyy HH:mm:ss}\", " +
                    $"\"FechaJS\": \"{comp.Fecha:yyyy-MM-dd HH:mm:ss}\", " +
                    $"\"UUID\": \"{uuid}\", " +
                    $"\"Total\": \"{comp.Total}\"" +
                    "}");
            }

            string jsonResponse = $"[{string.Join(",", jsonComprobantes)}]";
            return jsonResponse;
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
                $"\"Finalizada\": \"{(cons.Finalizada ? "Finalizada" : "En progreso")}\", " +
                $"\"Deshabilitado\": \"{cons.Deshabilitado}\"" +
                "}");
            }
            string jsonResponse = $"[{string.Join(",", jsonConciliaciones)}]";
            return jsonResponse;
        }

        public async Task<JsonResult> OnPostFiltrarComprobantesFechas()
        {
            // Inicializar la respuesta con un mensaje de error por defecto
            ServerResponse resp = new(true, stringLocalizer["ComprobantesFiltradosUnsuccessfully"]);

            try
            {
                // Obtener los comprobantes filtrados por fechas
                resp.Datos = await GetConsultarComprobantes(InputFiltroModalDComprobantes);
                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["ComprobantesFiltradosSuccessfully"];
            }
            catch (Exception ex)
            {
                // Registrar el error en el log
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        private async Task<string> GetConsultarComprobantes(InputFiltroModelDComprobantes? filtro = null)
        {
            List<object> jsonComprobantes = new List<object>();
            List<Comprobante> comprobantes;
            DateTime? fechaI = DateTime.ParseExact(filtro?.FechaInicioModalDComprobantes ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm"), "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            DateTime? fechaF = DateTime.ParseExact(filtro?.FechaFinModalDComprobantes ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm"), "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture); ;

            // Aplicar los filtros de fechas en la llamada a GetAllAsync
            if (filtro != null)
            {
                comprobantes = await comprobanteManager.GetByDateRangeAsync(
                    fechaI,
                    fechaF
                    //filtro.ClienteId
                    );
            }
            else
            {
                // Si no hay filtros, obtener todos los comprobantes
                comprobantes = await comprobanteManager.GetAllAsync();
            }

            foreach (Comprobante comp in comprobantes)
            {
                // Si el complemento o TimbreFiscalDigital no existe, devolver valores por defecto
                string uuid = comp.Complemento?.TimbreFiscalDigital?.UUID ?? "-";

                // Añadir el comprobante a la lista en formato JSON
                jsonComprobantes.Add("{" +
                    $"\"Id\": \"{comp.Id}\", " +
                    $"\"Serie\": \"{comp.Serie}\", " +
                    $"\"Folio\": \"{comp.Folio}\", " +
                    $"\"Fecha\": \"{comp.Fecha:dd/MM/yyyy HH:mm:ss}\", " +
                    $"\"FechaJS\": \"{comp.Fecha:yyyy-MM-dd HH:mm:ss}\", " +
                    $"\"UUID\": \"{uuid}\", " +
                    $"\"Total\": \"{comp.Total}\"" +
                    "}");
            }

            // Convertir la lista de comprobantes a un JSON string
            string jsonResponse = $"[{string.Join(",", jsonComprobantes)}]";
            return jsonResponse;
        }

        public async Task<JsonResult> OnGetMovimientosList()
        {
            ServerResponse resp = new(true, stringLocalizer["BankingMovementSavedUnsuccessfully"]);

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
            ServerResponse resp = new(false, stringLocalizer["ConciliacionSavedUnsuccessfully"]);

            try
            {
                await db.Database.BeginTransactionAsync();

                var cliente = await db.Clientes.FirstOrDefaultAsync(c => c.RazonSocial == InputFiltroModalAgregar.Cliente);
                if (cliente == null)
                {
                    resp.TieneError = true;
                    resp.Mensaje = stringLocalizer["ClienteNotFound"];
                    return new JsonResult(resp);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var usuarioCreador = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (usuarioCreador == null)
                {
                    resp.TieneError = true;
                    resp.Mensaje = stringLocalizer["UserNotFound"];
                    return new JsonResult(resp);
                }

                // Calcular el próximo ID para Conciliacion
                var lastConciliacion = await db.Conciliaciones.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
                int nextConciliacionId = (lastConciliacion?.Id ?? 0) + 1;

                // Calcular el total sumando los totales de todos los comprobantes conciliados
                decimal totalConciliacion = InputFiltroModalAgregar.Comprobantes?.Sum(comp => comp.Total) ?? 0;

                // Crear el registro de Conciliacion
                Conciliacion conciliacion = new Conciliacion
                {
                    Id = nextConciliacionId,
                    Fecha = InputFiltroModalAgregar.FechaElaboracionInicio,
                    ClienteId = cliente.Id,
                    Descripcion = InputFiltroModalAgregar.Descripcion,
                    UsuarioCreador = usuarioCreador,
                    UsuarioModificador = usuarioCreador,
                    BancoId = InputFiltroModalAgregar.BancoId,
                    EmpresaId = cliente.Id,
                    Finalizada = InputFiltroModalAgregar.Finalizada,
                    Total = totalConciliacion
                };

                await db.Conciliaciones.AddAsync(conciliacion);
                await db.SaveChangesAsync(); // Guardar para asegurar que ConciliacionId esté disponible

                // Obtener los IDs iniciales para ConciliacionDetalle y ConciliacionDetalleComprobante
                var lastDetalleConciliacion = await db.ConciliacionesDetalles.OrderByDescending(cd => cd.Id).FirstOrDefaultAsync();
                int nextDetalleId = (lastDetalleConciliacion?.Id ?? 0) + 1;
                int nextComprobanteId = await db.ConciliacionesDetallesComprobantes.MaxAsync(dc => (int?)dc.Id) ?? 0;

                // Crear un registro en ConciliacionDetalle para cada comprobante seleccionado
                List<int> detalleConciliacionIds = new List<int>();
                if (InputFiltroModalAgregar.Comprobantes != null && InputFiltroModalAgregar.Comprobantes.Any())
                {
                    foreach (var comp in InputFiltroModalAgregar.Comprobantes)
                    {
                        // Crear un nuevo detalle de conciliación para cada comprobante
                        var detalleConciliacion = new ConciliacionDetalle
                        {
                            Id = nextDetalleId++,  // Incrementa el ID para cada detalle
                            ConciliacionId = conciliacion.Id, // Asocia el mismo ConciliacionId para cada registro
                            Conciliacion = conciliacion,
                            ConciliacionesDetallesComprobantes = new List<ConciliacionDetalleComprobante>(),
                            ConciliacionesDetallesMovimientos = new List<ConciliacionDetalleMovimiento>()
                        };

                        // Agregar el comprobante a ConciliacionDetalleComprobante
                        detalleConciliacion.ConciliacionesDetallesComprobantes.Add(new ConciliacionDetalleComprobante
                        {
                            Id = ++nextComprobanteId,  // Incrementa el ID para cada comprobante
                            ConciliacionDetalleId = detalleConciliacion.Id,
                            ComprobanteId = comp.Id
                        });

                        // Guardar cada ConciliacionDetalle individualmente para asegurar su disponibilidad
                        await db.ConciliacionesDetalles.AddAsync(detalleConciliacion);

                        // Guardar el ID del detalle de conciliación recién creado
                        detalleConciliacionIds.Add(detalleConciliacion.Id);
                    }

                    await db.SaveChangesAsync(); // Guardar cambios para asegurar que todos los detalles estén en la base de datos
                }

                // Agregar los movimientos bancarios distribuidos entre los ConciliacionDetalleIds creados
                var nextMovimientoId = await db.MovimientosBancarios.MaxAsync(m => (int?)m.Id) ?? 0;
                var nextDetalleMovimientoId = await db.ConciliacionesDetallesMovimientos.MaxAsync(dm => (int?)dm.Id) ?? 0;

                if (InputFiltroModalAgregar.Movimientos != null && InputFiltroModalAgregar.Movimientos.Any())
                {
                    int index = 0;
                    foreach (var mov in InputFiltroModalAgregar.Movimientos)
                    {
                        var movimiento = new ERPSEI.Data.Entities.Conciliaciones.MovimientoBancario
                        {
                            Id = ++nextMovimientoId,
                            Fecha = mov.Fecha,
                            Descripcion = mov.Descripcion,
                            Importe = mov.Importe,
                            Conciliado = mov.Conciliado,
                            Conciliacion = conciliacion
                        };

                        await db.MovimientosBancarios.AddAsync(movimiento);

                        // Asocia el movimiento con el correspondiente ConciliacionDetalleId
                        var detalleMovimiento = new ConciliacionDetalleMovimiento
                        {
                            Id = ++nextDetalleMovimientoId,
                            MovimientoBancarioId = movimiento.Id,
                            ConciliacionDetalleId = detalleConciliacionIds[index]  // Asociar al ConciliacionDetalleId correspondiente
                        };

                        await db.ConciliacionesDetallesMovimientos.AddAsync(detalleMovimiento);

                        // Incrementa el índice y reinicia si es necesario para distribuir entre los IDs disponibles
                        index = (index + 1) % detalleConciliacionIds.Count;
                    }
                }

                await db.SaveChangesAsync();

                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["ConciliacionCreatedSuccessfully"];

                await db.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                resp.TieneError = true;
                resp.Mensaje = stringLocalizer["ConciliacionSavedUnsuccessfully"];
                await db.Database.RollbackTransactionAsync();
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
                resp.Datos = await GetClientesEmpresasSuggestion(texto);
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
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
                                        $"\"value\": \"{e.RazonSocial}\", " +
                                        $"\"label\": \"{desc}\", " +
                                        $"\"rfc\": \"{e.RFC}\"" +
                                    $"}}");
                }
            }

            jsonResponse = $"[{string.Join(",", jsonEmpresas)}]";

            return jsonResponse;
        }
    }
}
