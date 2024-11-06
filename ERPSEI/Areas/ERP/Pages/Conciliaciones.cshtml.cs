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
            public List<MovimientoBancario> Movimientos { get; set; } = new List<MovimientoBancario>();

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
            //IRCatalogoManager<Banco> _bancoManager,
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
                $"\"Deshabilitado\": \"{cons.Deshabilitado}\"" +
                "}");
            }
            string jsonResponse = $"[{string.Join(",", jsonConciliaciones)}]";
            return jsonResponse;
        }

        /*public async Task<JsonResult> OnPostGuardarMovimientos()
        {
            ServerResponse resp = new(true, stringLocalizer["MovimientoSavedUnsuccessfully"]);

            try
            {
                if (!ModelState.IsValid)
                {
                    resp.Errores = ModelState.Keys.SelectMany(k => ModelState[k]?.Errors ?? []).Select(m => m.ErrorMessage).ToArray();
                }
                else
                {
                    // Crear un nuevo registro de movimiento bancario
                    var nuevoMovimiento = new ERPSEI.Data.Entities.Conciliaciones.MovimientoBancario
                    {
                        Fecha = movimientoBancario.Fecha,             // Fecha importada del Excel
                        Descripcion = movimientoBancario.Descripcion, // Descripción importada del Excel
                        Importe = movimientoBancario.Importe,         // Importe (cargos) importado del Excel
                        Conciliado = false               // Inicialmente no conciliado
                    };

                    await db.MovimientosBancarios.AddAsync(nuevoMovimiento); // Agregar a la base de datos
                    await db.SaveChangesAsync(); // Guardar cambios

                    resp.TieneError = false;
                    resp.Mensaje = stringLocalizer["MovimientoSavedSuccessfully"];
                }
            }
            catch (Exception ex)
            {
                resp.TieneError = true;
                resp.Mensaje = stringLocalizer["MovimientoSavedUnsuccessfully"];
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> GuardarMovimientosImportados([FromBody] List<ERPSEI.Data.Entities.Conciliaciones.MovimientoBancario> movimientos)
        {
            ServerResponse resp = new(true, "Movimientos guardados sin éxito.");

            try
            {
                if (movimientos == null || movimientos.Count == 0)
                {
                    resp.Mensaje = "No se recibieron movimientos para guardar.";
                    return new JsonResult(resp);
                }

                foreach (var movimiento in movimientos)
                {
                    movimiento.Conciliado = false; // Ajuste necesario si aplica
                    db.MovimientosBancarios.Add(movimiento); // Agregar cada movimiento a la base de datos
                }

                await db.SaveChangesAsync();

                resp.TieneError = false;
                resp.Mensaje = "Movimientos guardados exitosamente.";
            }
            catch (Exception ex)
            {
                resp.Mensaje = "Ocurrió un error al guardar los movimientos.";
                logger.LogError(ex, "Error al guardar movimientos importados.");
            }

            return new JsonResult(resp);
        }*/

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
            DateTime? fechaI = DateTime.ParseExact(filtro?.FechaInicioModalDComprobantes?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm"), "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            DateTime? fechaF = DateTime.ParseExact(filtro?.FechaFinModalDComprobantes ?? DateTime.MinValue.ToString("yyyy-MM-ddTHH:mm"), "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture); ;

            // Aplicar los filtros de fechas en la llamada a GetAllAsync
            if (filtro != null)
            {
                comprobantes = await comprobanteManager.GetByDateRangeAsync(
                    fechaI,
                    fechaF);
            }
            else
            {
                // Si no hay filtros, obtener todos los comprobantes
                comprobantes = await comprobanteManager.GetAllAsync();
            }

            // Construir el JSON con objetos anónimos
            foreach (Comprobante comp in comprobantes)
            {
                //DateTime? fecha = comp.Fecha == DateTime.MinValue ? null : comp.Fecha;

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
                    EmpresaId = cliente.Id
                };

                await db.Conciliaciones.AddAsync(conciliacion);

                // Calcular el próximo ID para ConciliacionDetalle
                var lastDetalleConciliacion = await db.ConciliacionesDetalles.OrderByDescending(cd => cd.Id).FirstOrDefaultAsync();
                int nextDetalleId = (lastDetalleConciliacion?.Id ?? 0) + 1;

                // Crear un ConciliacionDetalle por cada movimiento en InputFiltroModalAgregar.Movimientos
                if (InputFiltroModalAgregar.Movimientos != null && InputFiltroModalAgregar.Movimientos.Any())
                {
                    int nextMovimientoId = await db.MovimientosBancarios.MaxAsync(m => (int?)m.Id) ?? 0;
                    int nextDetalleMovimientoId = await db.ConciliacionesDetallesMovimientos.MaxAsync(dm => (int?)dm.Id) ?? 0;

                    foreach (var mov in InputFiltroModalAgregar.Movimientos)
                    {
                        // Crear y guardar el movimiento bancario
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

                        // Crear un nuevo registro de ConciliacionDetalle para cada movimiento
                        var detalleConciliacion = new ConciliacionDetalle
                        {
                            Id = nextDetalleId++, // Incrementa el ID para cada nuevo detalle
                            ConciliacionId = conciliacion.Id,
                            Conciliacion = conciliacion,
                            ConciliacionesDetallesComprobantes = new List<ConciliacionDetalleComprobante>(),
                            ConciliacionesDetallesMovimientos = new List<ConciliacionDetalleMovimiento>()
                        };

                        // Agregar detalle de movimiento a ConciliacionDetalle
                        detalleConciliacion.ConciliacionesDetallesMovimientos.Add(new ConciliacionDetalleMovimiento
                        {
                            Id = ++nextDetalleMovimientoId, // Incrementa el ID para cada nuevo detalle de movimiento
                            MovimientoBancarioId = movimiento.Id,
                            ConciliacionDetalleId = detalleConciliacion.Id
                        });

                        await db.ConciliacionesDetalles.AddAsync(detalleConciliacion); // Guarda cada detalle de conciliación
                    }
                }

                await db.SaveChangesAsync();

                resp.TieneError = false;
                resp.Mensaje = stringLocalizer["ConciliacionCreatedSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                resp.TieneError = true;
                resp.Mensaje = stringLocalizer["ConciliacionSavedUnsuccessfully"];
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
                                        $"\"rfc\": \"{e.RFC}\""+
                                    $"}}");
                }
            }

            jsonResponse = $"[{string.Join(",", jsonEmpresas)}]";

            return jsonResponse;
        }
    }
}
