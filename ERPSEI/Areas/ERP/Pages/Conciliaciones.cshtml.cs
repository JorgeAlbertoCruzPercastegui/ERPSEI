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
using Microsoft.AspNetCore.Identity;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Data.Entities.Polizas;
using ERPSEI.Data.Managers.Polizas;
using ERPSEI.Data.Managers.AdministradorPolizas;

namespace ERPSEI.Areas.ERP.Pages
{
    public class ConciliacionesModel : ERPPageModel
    {
        private readonly IStringLocalizer<ConciliacionesModel> stringLocalizer;
        private readonly ILogger<ConciliacionesModel> logger;
        private readonly IBancoManager bancoManager;
        private readonly IConciliacionManager conciliacionManager;
        private readonly ICuentaContableManager cuentaContableManager;
        private readonly ICuentaContableManager cuentaContableSubtipoManager;
        private readonly IConciliacionDetalleManager conciliacionDetalleManager;
        private readonly IConciliacionDetalleComprobanteManager conciliacionDetalleComprobanteManager;
        private readonly IConciliacionDetalleMovimientoManager conciliacionDetalleMovimientoManager;
        private readonly IMovimientoBancarioManager movimientoBancarioManager;
        private readonly IEmpresaManager empresaManager;
        private readonly IEmpleadoManager _empleadoManager;
        private readonly AppUserManager appUserManager;
        private readonly IPolizasTipos polizasTipos;
        private readonly IPolizasManager polizasManager;
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
            ICuentaContableManager _cuentaContableManager,
            IConciliacionDetalleManager _conciliacionDetalleManager,
            IConciliacionDetalleComprobanteManager _conciliacionDetalleComprobanteManager,
            IConciliacionDetalleMovimientoManager _conciliacionDetalleMovimientoManager,
            IMovimientoBancarioManager _movimientoBancarioManager,
            IEmpresaManager _empresaManager,
            IEmpleadoManager empleadoManager,
            AppUserManager _appUserManager,
            IPolizasTipos _polizasTipos,
            IPolizasManager _polizasManager,
            IComprobanteManager _comprobanteManager,
            IStringLocalizer<ConciliacionesModel> _localizer,
            Data.ApplicationDbContext _db
        )
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            bancoManager = _bancoManager;
            conciliacionManager = _conciliacionManager;
            cuentaContableManager = _cuentaContableManager;
            conciliacionDetalleManager = _conciliacionDetalleManager;
            conciliacionDetalleComprobanteManager = _conciliacionDetalleComprobanteManager;
            conciliacionDetalleMovimientoManager = _conciliacionDetalleMovimientoManager;
            movimientoBancarioManager = _movimientoBancarioManager;
            empresaManager = _empresaManager;
            _empleadoManager = empleadoManager;
            appUserManager = _appUserManager;
            polizasTipos = _polizasTipos;
            polizasManager = _polizasManager;
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

        public async Task<JsonResult> OnGetExportarExcel(int id, string cuentaBancariaSeleccionada)
        {
            ServerResponse resp = new(true, localizer["ExportExcelUnsuccessfully"]);
            try
            {
                var usuarioActual = HttpContext?.User.Identity?.Name;
                var user = await appUserManager.FindByNameAsync(usuarioActual);
                var nombreUsuario = user?.UserName ?? "Usuario Desconocido";
                var idUser = user?.Id;

                // Llamar a CrearGrupoPoliza solo una vez
                var registroCreado = await CrearGrupoPoliza(idUser, nombreUsuario);

                if (!registroCreado)
                {
                    resp.Mensaje = "Error al guardar la información en la base de datos.";
                    return new JsonResult(resp);
                }

                // Pasar la cuenta bancaria seleccionada al método GetExportarExcel
                resp.Datos = await GetExportarExcel(id, HttpContext, cuentaBancariaSeleccionada);
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

        public async Task<List<object>> GetExportarExcel(int conciliacionId, HttpContext httpContext, string cuentaBancariaSeleccionada)
        {
            try
            {
                // Obtener la conciliación por ID 
                var conciliacion = await conciliacionManager.GetByIdAsync(conciliacionId);

                // Crear una lista para almacenar los datos del Excel
                var datosExcel = new List<object>();

                // Obtener el nombre del usuario actual desde AspNetUsers
                var usuarioActual = httpContext?.User.Identity?.Name;
                var user = await appUserManager.FindByNameAsync(usuarioActual);
                var nombreUsuario = user?.UserName ?? "Usuario Desconocido";
                var idUser = user?.Id;

                // Obtener la lista de PolizaTipos
                var polizaTipos = await ObtenerPolizaTiposAsync();

                // Crear el registro en GrupoPoliza y obtener su Id
                var idGrupoPoliza = await ObtenerGrupoPolizaId(idUser);

                // Verificar si se creó correctamente el registro
                if (idGrupoPoliza == null)
                {
                    throw new Exception("No se pudo crear el registro de GrupoPoliza.");
                }

                // Recorrer los detalles de la conciliación y preparar los datos
                foreach (var detalle in conciliacion.DetallesConciliacion)
                {
                    foreach (var comprobante in detalle.ConciliacionesDetallesComprobantes)
                    {
                        // Obtener el TotalImpuestosTrasladados del comprobante
                        var totalImpuestosTrasladados = await conciliacionManager.GetTotalImpuestosTrasladadosAsync(comprobante.Comprobante?.Impuestos?.Id);

                        // Obtener la lista de VPolizas
                        var vPolizas = await ObtenerVPolizasAsync();

                        // Obtener los datos del receptor
                        var rfcReceptor = comprobante.Comprobante?.Receptor?.Rfc ?? "N/A";
                        var rfcEmisor = comprobante.Comprobante?.Emisor?.Rfc ?? "N/A";
                        var nombreEmisor = comprobante.Comprobante?.Emisor?.Nombre ?? "N/A";

                        // Capturar y utilizar la cuenta bancaria seleccionada
                        var cuentaBancariaS = string.IsNullOrEmpty(cuentaBancariaSeleccionada) ? "Cuenta no asignada" : cuentaBancariaSeleccionada;

                        var nombreReceptor = comprobante.Comprobante?.Receptor?.Nombre ?? "N/A";
                        var tipoDeComprobante = comprobante.Comprobante?.TipoDeComprobante ?? "N/A";

                        // Mapear TipoDeComprobante a descripción de PolizaTipo
                        string descripcionPolizaTipo = tipoDeComprobante switch
                        {
                            "I" => "Ingreso",
                            "E" => "Egreso",
                            "B" => "Bancos",
                            _ => "N/A"
                        };

                        // Buscar el Id de PolizaTipo correspondiente a la descripción
                        var polizaTipoId = polizaTipos
                            .FirstOrDefault(pt => pt.Descripcion == descripcionPolizaTipo)?.Id ?? 0;

                        var EmisorId = comprobante.Comprobante.Emisor.Id;
                        var empresas = await empresaManager.GetByRFCAsync(rfcEmisor);
                        //var cuentasContables = await cuentaContableManager.GetByIdAsync(1708);
                        var cuentasContables = await cuentaContableManager.GetFilteredAsync(empresas.Id, 1, 2, rfcReceptor);

                        //Obtener cuentas bancarias
                        List<CuentaContable>? cuentasContablesBanc = await cuentaContableManager.GetByIdEmpresaAsync(empresas?.Id ?? 0);
                        cuentasContablesBanc = cuentasContablesBanc.Where(c => c.TipoId == 3).ToList();

                        CuentaContable? cuentaBancaria = cuentasContablesBanc.Where(cuenta => cuenta.TipoId == 3 && cuenta.SubtipoId == 19).FirstOrDefault();


                        foreach (var movimiento in detalle.ConciliacionesDetallesMovimientos)
                        {
                            // Definir fecha y concepto para la póliza
                            var fechaString = comprobante.Comprobante?.Fecha; // Suponiendo que este valor es un string
                            DateTime fechaHora;

                            // Intentar convertir el string a DateTime de manera segura
                            if (!DateTime.TryParse(fechaString, out fechaHora))
                            {
                                // Si la conversión falla, asignar la fecha y hora actual como valor predeterminado
                                fechaHora = DateTime.Now;
                            }

                            var concepto = $"INGRESOS {nombreReceptor} {comprobante.Comprobante?.Serie ?? "N/A"}-F-{comprobante.Comprobante?.Folio ?? "N/A"}";

                            // Llamar al método para crear la póliza en la base de datos
                            await CrearPoliza(idUser, idGrupoPoliza, polizaTipoId, fechaHora, concepto);

                            // Obtener el ID de la póliza generada
                            var polizaId = await db.VPolizas
                                .Where(p => p.GrupoId == idGrupoPoliza && p.TipoId == polizaTipoId && p.FechaHora == fechaHora)
                                .Select(p => p.Id)
                                .FirstOrDefaultAsync();

                            if (polizaId == 0)
                            {
                                throw new Exception("No se pudo obtener el ID de la póliza creada.");
                            }

                            // Concepto Detalle para las filas del Excel
                            //var conceptoDetalle = $"{nombreReceptor} {comprobante.Comprobante?.Serie ?? "N/A"}-F-{comprobante.Comprobante?.Folio ?? "N/A"}";

                            // Concepto Detalle para las filas del Excel repetido 4 veces, separado por espacios
                            var conceptoDetalle = string.Join(" ", Enumerable.Repeat($"{nombreReceptor} {comprobante.Comprobante?.Serie ?? "N/A"}-F-{comprobante.Comprobante?.Folio ?? "N/A"}", 4));


                            decimal debe = movimiento.MovimientoBancario?.Importe ?? 0;
                            decimal debeImp = totalImpuestosTrasladados;
                            decimal totalDebe = debe + debeImp;

                            decimal haber = movimiento.MovimientoBancario?.Importe ?? 0;
                            decimal haberImp = totalImpuestosTrasladados;
                            decimal totalHaber = haber + haberImp;

                            // Llamar al método CrearPolizaDetalle
                            await CrearPolizaDetalle(
                                polizaId,               // El ID de la póliza generada
                                cuentaId: 2438,         // El ID de la cuenta (en este caso 2438 como mencionaste)
                                concepto: conceptoDetalle,
                                debe: totalDebe,
                                haber: totalHaber
                            );

                            datosExcel.Add(new
                                    {
                                        Cliente = conciliacion.Cliente?.RazonSocial ?? "Sin Cliente",
                                        ComprobanteId = comprobante.Comprobante?.Id ?? 0,
                                        Serie = comprobante.Comprobante?.Serie ?? "N/A",
                                        Folio = comprobante.Comprobante?.Folio ?? "N/A",
                                        EmpresaID = empresas?.Id,
                                        NombreEmpresa = empresas?.RazonSocial,
                                        RFCEmpresa = empresas?.RFC,
                                        CuentaBancariaSeleccionada = cuentaBancariaSeleccionada,
                                        CBS = cuentaBancariaS,
                                        Total = comprobante.Comprobante?.Total ?? 0,
                                        MovimientoId = movimiento.MovimientoBancario?.Id ?? 0,
                                        DescripcionMovimiento = movimiento.MovimientoBancario?.Descripcion ?? "N/A",
                                        Cargos = movimiento.MovimientoBancario?.Importe ?? 0,
                                        Fecha = comprobante.Comprobante?.Fecha ?? "N/A",
                                        TotalImpuestosTrasladados = totalImpuestosTrasladados,
                                        CuentaContable = string.Join(", ", cuentasContables),
                                        RfcReceptor = rfcReceptor,
                                        NombreReceptor = nombreReceptor,
                                        RfcEmisor = rfcEmisor,
                                        NombreEmisor = nombreEmisor,
                                        CuentaBancariaOption = string.Join(", ", cuentasContablesBanc.Select(c => $"{c.Cuenta} ({c.Nombre})")),
                                        CuentaBancariaVista = string.Join(", ", cuentasContablesBanc.Select(c => $"{c.Cuenta} ({c.Nombre})")),
                                        CuentaBancariaExcel = string.Join(", ", cuentasContablesBanc.Select(c => c.Cuenta)),
                                        

                                        // Información del usuario logueado para GrupoPoliza
                                        IdGrupoPoliza = idGrupoPoliza,
                                        UsuarioLogueado = nombreUsuario,
                                        IdUsuarioCreador = idUser,
                                        IdUsuarioModificador = idUser,
                                        FechaHoraCreacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        FechaHoraModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                        NumeroImpresion = 1,
                                        Deshabilitado = 0,

                                        // Información de PolizaTipo
                                        PolizaTipos = polizaTipos.Select(pt => new
                                        {
                                             pt.Id,
                                             pt.Descripcion,
                                             pt.Deshabilitado
                                        }),
                                        
                                        //Informacion de la PolizaTipo del registro seleccionado

                                        //Información Poliza
                                        VPolizas = vPolizas.Select(vp => new
                                        {
                                            vp.Id,
                                            vp.GrupoId,
                                            vp.TipoId,
                                            vp.FechaHora,
                                            vp.Concepto
                                        }),

                                        //Informacion de la Poliza del registro seleccionado
                                        PolizaGrupoId = idGrupoPoliza,
                                        PolizaTipoId = polizaTipoId,
                                        TipoDComprobante = tipoDeComprobante,
                                        TipoDeComprobante = comprobante.Comprobante?.TipoDeComprobante ?? "N/A",
                                        PolizaTipoDescripcion = polizaTipos.Select(ptd => new 
                                        { 
                                        ptd.Id,
                                        ptd.Descripcion,
                                        ptd.Deshabilitado
                                        }),

                                        GrupoId = idGrupoPoliza,
                                        TipoId = polizaTipoId,
                                        FechaHora = comprobante.Comprobante?.Fecha ?? "N/A",
                                        Concepto = nombreReceptor + ' ' + comprobante.Comprobante?.Serie ?? "N/A" + ' ' + comprobante.Comprobante?.Folio ?? "N/A" 

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

        public async Task<bool> CrearPolizaDetalle(int polizaId, int cuentaId, string concepto, decimal debe, decimal haber)
        {
            try
            {
                // Verificar si ya existe un registro similar en PolizaDetalle
                var existeDetalle = await db.PolizasDetalles
                    .AnyAsync(pd => pd.PolizaId == polizaId && pd.CuentaId == cuentaId && pd.Concepto == concepto);

                if (existeDetalle)
                {
                    logger.LogWarning("Ya existe un registro de PolizaDetalle con el mismo PolizaId, CuentaId y Concepto.");
                    return true; // Evitar duplicar el registro
                }

                // Generar un nuevo Id basado en el valor máximo actual
                var nuevoId = (await db.PolizasDetalles.MaxAsync(pd => (int?)pd.Id) ?? 0) + 1;

                // Crear una nueva instancia de PolizaDetalle
                var polizaDetalle = new PolizaDetalle
                {
                    Id = nuevoId, // Asignar el nuevo ID generado
                    PolizaId = polizaId,
                    CuentaId = cuentaId,
                    Concepto = concepto,
                    Debe = debe,
                    Haber = haber
                };

                // Agregar el nuevo registro al contexto
                db.PolizasDetalles.Add(polizaDetalle);

                // Guardar los cambios en la base de datos
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("Error al crear PolizaDetalle: {message}", ex.Message);
                return false;
            }
        }


        public async Task<bool> CrearPoliza(string usuarioId, int? grupoId, int tipoId, DateTime fechaHora, string concepto)
        {
            try
            {
                // Verificar si ya existe una póliza con los mismos valores de GrupoId, TipoId, y FechaHora
                var existePoliza = await db.VPolizas
                    .AnyAsync(p => p.GrupoId == grupoId && p.TipoId == tipoId && p.FechaHora == fechaHora);

                if (existePoliza)
                {
                    logger.LogWarning("Ya existe un registro de Poliza con el mismo GrupoId, TipoId y FechaHora.");
                    return true; // Evitar duplicar el registro
                }

                // Generar un nuevo Id basado en el valor máximo actual en la tabla
                var nuevoId = (await db.VPolizas.MaxAsync(p => (int?)p.Id) ?? 0) + 1;

                // Crear una nueva instancia de Poliza
                var poliza = new VPoliza
                {
                    Id = nuevoId,
                    GrupoId = grupoId,
                    TipoId = tipoId,
                    FechaHora = fechaHora,
                    Concepto = concepto
                };

                // Agregar el nuevo registro al contexto
                db.VPolizas.Add(poliza);

                // Guardar los cambios en la base de datos
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("Error al crear Poliza: {message}", ex.Message);
                return false;
            }
        }

        public async Task<bool> CrearGrupoPoliza(string usuarioId, string nombreUsuario)
        {
            try
            {
                // Verificar si ya existe un registro con el mismo UsuarioCreadorId y fecha actual
                var hoy = DateTime.Now.Date;
                var existePoliza = await db.GruposPolizas
                    .AnyAsync(g => g.UsuarioCreadorId == usuarioId && g.FechaHoraCreacion.HasValue && g.FechaHoraCreacion.Value.Date == hoy);

                if (existePoliza)
                {
                    logger.LogWarning("Ya existe un registro de GrupoPoliza para el usuario actual en la fecha de hoy.");
                    return true; // Evitar duplicar el registro
                }

                // Crear una nueva instancia de GrupoPoliza
                var grupoPoliza = new GrupoPoliza
                {
                    Id = await GenerarNuevoIdAsync(),
                    UsuarioCreadorId = usuarioId,
                    UsuarioModificadorId = usuarioId,
                    FechaHoraCreacion = DateTime.Now,
                    FechaHoraModificacion = DateTime.Now,
                    NumeroImpresion = 1,
                    Deshabilitado = false
                };

                // Agregar el nuevo registro al contexto
                db.GruposPolizas.Add(grupoPoliza);

                // Guardar los cambios en la base de datos
                await db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError("{message}", ex.Message);
                return false;
            }
        }

        public async Task<int?> ObtenerGrupoPolizaId(string usuarioId)
        {
            try
            {
                // Verificar si ya existe un registro con el mismo UsuarioCreadorId y fecha actual
                var hoy = DateTime.Now.Date;
                var grupoPolizaExistente = await db.GruposPolizas
                    .FirstOrDefaultAsync(g => g.UsuarioCreadorId == usuarioId && g.FechaHoraCreacion.HasValue && g.FechaHoraCreacion.Value.Date == hoy);

                if (grupoPolizaExistente != null)
                {
                    logger.LogWarning("Ya existe un registro de GrupoPoliza para el usuario actual en la fecha de hoy.");
                    return grupoPolizaExistente.Id; // Devolver el Id del registro existente
                }

                // Crear un nuevo registro de GrupoPoliza si no existe uno para hoy
                var grupoPoliza = new GrupoPoliza
                {
                    Id = await GenerarNuevoIdAsync(),
                    UsuarioCreadorId = usuarioId,
                    UsuarioModificadorId = usuarioId,
                    FechaHoraCreacion = DateTime.Now,
                    FechaHoraModificacion = DateTime.Now,
                    NumeroImpresion = 1,
                    Deshabilitado = false
                };

                db.GruposPolizas.Add(grupoPoliza);
                await db.SaveChangesAsync();

                return grupoPoliza.Id; // Devolver el Id del registro creado
            }
            catch (Exception ex)
            {
                logger.LogError("Error al crear GrupoPoliza: {message}", ex.Message);
                return null;
            }
        }

        private async Task<int> GenerarNuevoIdAsync()
        {
            var maxId = await db.GruposPolizas.MaxAsync(g => (int?)g.Id) ?? 0;
            return maxId + 1;
        }


        public async Task<List<PolizaTipo>> ObtenerPolizaTiposAsync()
        {
            try
            {
                return await db.PolizasTipos.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Error al obtener PolizaTipo: {message}", ex.Message);
                return new List<PolizaTipo>();
            }
        }

        public async Task<List<VPoliza>> ObtenerVPolizasAsync()
        {
            try
            {
                // Obtener todas las VPolizas de la base de datos
                var vPolizas = await db.VPolizas.ToListAsync();
                return vPolizas;
            }
            catch (Exception ex)
            {
                logger.LogError("Error al obtener VPolizas: {message}", ex.Message);
                return new List<VPoliza>();
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

        public async Task<JsonResult> OnPutFinalizarConciliaciones(int id)
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
