using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Data.Managers.Empresas;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Pages.Shared;
using ERPSEI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;
using ERPSEI.Data.Entities.Empresas;
using ERPSEI.Data.Entities.TipoContratos;
using ERPSEI.Data.Managers.TipoContratos;
using TemplateEngine.Docx;
using ERPSEI.Data.Entities.Clientes;


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
        public class GuardarContratoRequest
        {
            // Prestador
            public int PrestadorId { get; set; }
            public string PrestadorNombre { get; set; }
            public string PrestadorRFC { get; set; }
            public string PrestadorDomicilio { get; set; }
            public string PrestadorRepresentante { get; set; }
            public string PrestadorEmail { get; set; }
            public DateTime? PrestadorFecha { get; set; }
            public DateTime? PrestadorFechaInicio { get; set; }
            public DateTime? PrestadorFechaFin { get; set; }
            public int? TipoContratoPrestadorId { get; set; }
            public int? PrestadorNoNotario { get; set; }
            public string? PrestadorNotario { get; set; }
            public string? PrestadorPaginaWeb { get; set; }

            // Prestatario
            public int PrestatarioId { get; set; }
            public string PrestatarioNombre { get; set; }
            public string PrestatarioRFC { get; set; }
            public string PrestatarioDomicilio { get; set; }
            public string PrestatarioRepresentante { get; set; }
            public string PrestatarioEmail { get; set; }
            public DateTime? PrestatarioFecha { get; set; }
            public DateTime? PrestatarioFechaInicio { get; set; }
            public DateTime? PrestatarioFechaFin { get; set; }
            public int? TipoContratoPrestatarioId { get; set; }
            public int? PrestatarioNoNotario { get; set; }
            public string? PrestatarioNotario { get; set; }
            public string? PrestatarioPaginaWeb { get; set; }
        }

        public class ActualizarContratoRequest
        {
            public EmpresaContrato Empresa { get; set; }
            public ClienteContrato Cliente { get; set; }
        }


        private readonly IWebHostEnvironment _hostingEnvironment;

        public GeneradorContratoModel(
            IStringLocalizer<GeneradorContratoModel> _stringLocalizer,
            ILogger<GeneradorContratoModel> _logger,
            AppUserManager _appUserManager,
            IStringLocalizer<GeneradorContratoModel> _localizer,
            Data.ApplicationDbContext _db,
            AppUserManager _userManager,
            IWebHostEnvironment hostingEnvironment,
            ITipoContratosManager _tipoContratosManager,
            IEmpresaContratosManager _empresaContratosManager,
            IClienteContratosManager _clienteContratosManager,
            IEmpresaManager _empresaManager
            ) 
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            appUserManager = _appUserManager;
            localizer = _localizer;
            db = _db;
            userManager = _userManager;

            InputFiltro = new InputFiltroModel();
            _hostingEnvironment = hostingEnvironment;
            tipoContratosManager = _tipoContratosManager;
            empresaContratosManager = _empresaContratosManager;
            clienteContratosManager = _clienteContratosManager;
            empresaManager = _empresaManager;

            EmpresaContratosList = new EmpresaContrato();
        }

        public async Task<JsonResult> OnGetEmpresaContratosList()
        {
            var empresas = await empresaContratosManager.GetAllAsync();
            empresas = empresas.Where(e => !e.Deshabilitado && !e.Estatus).ToList();

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
                    fechaInicio = e.FechaInicio?.ToString("dd/MM/yyyy") ?? "-",
                    fechaInicioJS = e.FechaInicio?.ToString("yyyy-MM-dd") ?? "-",
                    fechaFin = e.FechaFin?.ToString("dd/MM/yyyy") ?? "-",
                    fechaFinJS = e.FechaFin?.ToString("yyyy-MM-dd") ?? "-",
                    tipoContrato = e.TipoContrato?.Nombre ?? "-",
                    tipoContratoId = e.TipoContratoId,
                    deshabilitado = e.Deshabilitado.ToString()
                });
            }

            return new JsonResult(jsonEmpresas);
        }

        public async Task<JsonResult> OnPostFiltrarEmpresasContratos()
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);

            try
            {
                var empresas = await empresaContratosManager.GetAllAsync(InputFiltro);

                // ? Filtrar solo las empresas que no tienen contrato generado (Estatus = false)
                empresas = empresas
                    .Where(e => !e.Deshabilitado && !e.Estatus)
                    .ToList();

                var result = empresas.Select(e =>
                {
                    var cliente = db.ClienteContratos.FirstOrDefault(c => c.EmpresaContratoId == e.Id);

                    return new
                    {
                        id = e.Id,
                        razonSocial = e.RazonSocial ?? "-",
                        razonSocialPrestatario = cliente != null ? cliente.RazonSocial ?? "-" : "-",
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
                    };
                }).ToList();

                resp.Datos = result;
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al filtrar empresas contrato");
            }

            return new JsonResult(resp);
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
            //var clientes = await clienteContratosManager.GetByEmpresaContratoIdAsync(id);
            var clientes = await db.ClienteContratos
                .Include(c => c.TipoContrato)
                .Where(c => c.EmpresaContratoId == id)
                .ToListAsync();


            var result = clientes.Select(c => new {
                id = c.Id,
                nombre = c.RazonSocial ?? "-",
                rfc = c.RFC ?? "-",
                domicilioFiscal = c.DomicilioFiscal ?? "-",
                representanteLegal = c.RepresentanteLegal ?? "-",
                noNotario = c.NoNotario?.ToString() ?? "-",
                notario = c.Notario ?? "-",
                email = c.Email ?? "-",
                paginaWeb = c.PaginaWeb ?? "-",
                fechaConstitucion = c.FechaConstitucion?.ToString("yyyy-MM-dd") ?? "",
                fechaInicio = c.FechaInicio?.ToString("yyyy-MM-dd") ?? "",
                fechaFin = c.FechaFin?.ToString("yyyy-MM-dd") ?? "",
                tipoContratoId = c.TipoContratoId,
                tipoContrato = c.TipoContrato != null ? c.TipoContrato.Nombre : "-"

            });

            return new JsonResult(result);
        }

        /*public async Task<IActionResult> OnGetGenerarWordAsync(int clienteId, int empresaId)
        {
            try
            {
                var cliente = await db.ClienteContratos.FirstOrDefaultAsync(c => c.Id == clienteId);
                var empresa = await db.EmpresaContratos.FirstOrDefaultAsync(e => e.Id == empresaId);

                if (cliente == null || empresa == null)
                    return NotFound("Cliente o empresa no encontrados.");

                // 1. Actualizar estatus
                empresa.Estatus = true;
                cliente.Estatus = true;

                // 2. Insertar en historial
                var historial = new HistorialContratoGenerado
                {
                    EmpresaContratoId = empresa.Id,
                    ClienteContratoId = cliente.Id,
                    UsuarioGenerador = User.Identity?.Name ?? "Desconocido",
                    FechaGeneracion = DateTime.Now,
                    NumeroContrato = $"CTR-{empresa.Id}-{cliente.Id}-{DateTime.Now:yyyyMMddHHmmss}",
                    ArchivoGenerado = $"Contrato_{empresa.RazonSocial}_{cliente.RazonSocial}.docx",
                    Activo = true
                };

                db.HistorialContratoGenerados.Add(historial);
                await db.SaveChangesAsync();

                // 3. Crear contrato Word
                var templatePath = Path.Combine(_hostingEnvironment.WebRootPath, "templates", "CONTRATO_DE_PRESTACION_DE_SERVICIOS_MODELO_1_SERVICIOS_PROFESIONALES.docx");
                var outputPath = Path.Combine(Path.GetTempPath(), historial.ArchivoGenerado);

                System.IO.File.Copy(templatePath, outputPath, overwrite: true);

                using (var outputDocument = new TemplateProcessor(outputPath).SetRemoveContentControls(true))
                {
                    var content = new Content(
                        new FieldContent("Empresa", empresa.RazonSocial ?? "-"),
                        //new FieldContent("RFC", empresa.RFC ?? "-"),
                        //new FieldContent("Domicilio_Empresa", empresa.DomicilioFiscal ?? "-"),
                        new FieldContent("Cliente", cliente.RazonSocial ?? "-"),
                        //new FieldContent("RFC_Cliente", cliente.RFC ?? "-"),
                        //new FieldContent("Domicilio_Cliente", cliente.DomicilioFiscal ?? "-"),
                        new FieldContent("Representante_Empresa", empresa.RepresentanteLegal ?? "-"),
                        new FieldContent("Representante_Cliente", cliente.RepresentanteLegal ?? "-")
                    //new FieldContent("Notario_Empresa", empresa.Notario ?? "-"),
                    //new FieldContent("NoNotario_Empresa", empresa.NoNotario?.ToString() ?? "-"),
                    //new FieldContent("Notario_Cliente", cliente.Notario ?? "-"),
                    //new FieldContent("NoNotario_Cliente", cliente.NoNotario?.ToString() ?? "-")
                    );

                    outputDocument.FillContent(content);
                    outputDocument.SaveChanges();
                }

                var memory = new MemoryStream(await System.IO.File.ReadAllBytesAsync(outputPath));
                return File(memory,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    historial.ArchivoGenerado);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al generar contrato Word");
                return BadRequest($"Error al generar el contrato: {ex.Message}");
            }
        }*/

        public async Task<IActionResult> OnGetGenerarWordAsync(int clienteId, int empresaId)
        {
            try
            {
                var cliente = await db.ClienteContratos.FirstOrDefaultAsync(c => c.Id == clienteId);
                var empresa = await db.EmpresaContratos.FirstOrDefaultAsync(e => e.Id == empresaId);

                if (cliente == null || empresa == null)
                    return NotFound("Cliente o empresa no encontrados.");

                // 1. Actualizar estatus
                empresa.Estatus = true;
                cliente.Estatus = true;

                // 2. Insertar en historial
                var historial = new HistorialContratoGenerado
                {
                    EmpresaContratoId = empresa.Id,
                    ClienteContratoId = cliente.Id,
                    UsuarioGenerador = User.Identity?.Name ?? "Desconocido",
                    FechaGeneracion = DateTime.Now,
                    NumeroContrato = $"CTR-{empresa.Id}-{cliente.Id}-{DateTime.Now:yyyyMMddHHmmss}",
                    ArchivoGenerado = $"Contrato_{empresa.RazonSocial}_{cliente.RazonSocial}.docx",
                    Activo = true
                };

                db.HistorialContratoGenerados.Add(historial);
                await db.SaveChangesAsync();

                // 3. Seleccionar plantilla Word según la Razón Social
                string templateFileName;

                if (empresa.RazonSocial.Equals("CYBERTRADE INTERNATIONAL, S.A. DE C.V.", StringComparison.OrdinalIgnoreCase))
                {
                    templateFileName = "CONTRATO_DE_PRESTACION_DE_SERVICIOS_MODELO_2_SERVICIOS_PROFESIONALES.docx";
                }
                else if (empresa.RazonSocial.Contains("DOR DESARROLLOS INTEGRALES, S.A. DE C.V.", StringComparison.OrdinalIgnoreCase))
                {
                    templateFileName = "CONTRATO_DE_PRESTACION_DE_SERVICIOS_MODELO_1_SERVICIOS_PROFESIONALES.docx";
                }
                else
                {
                    // Plantilla por defecto en caso de no coincidir ninguna condición
                    templateFileName = "CONTRATO_DE_PRESTACION_DE_SERVICIOS_MODELO_1_SERVICIOS_PROFESIONALES.docx";
                }

                var templatePath = Path.Combine(_hostingEnvironment.WebRootPath, "templates", templateFileName);
                var outputPath = Path.Combine(Path.GetTempPath(), historial.ArchivoGenerado);

                System.IO.File.Copy(templatePath, outputPath, overwrite: true);

                // 4. Procesar contenido del contrato
                using (var outputDocument = new TemplateProcessor(outputPath).SetRemoveContentControls(true))
                {
                    var content = new Content(
                        new FieldContent("Empresa", empresa.RazonSocial ?? "-"),
                        new FieldContent("Cliente", cliente.RazonSocial ?? "-"),
                        new FieldContent("Representante_Empresa", empresa.RepresentanteLegal ?? "-"),
                        new FieldContent("Representante_Cliente", cliente.RepresentanteLegal ?? "-"),
                        new FieldContent("Empresa", empresa.RazonSocial ?? "-")
                        //new FieldContent("RFC", empresa.RFC ?? "-"),
                        //new FieldContent("Domicilio_Empresa", empresa.DomicilioFiscal ?? "-"),
                        //new FieldContent("RFC_Cliente", cliente.RFC ?? "-"),
                        //new FieldContent("Domicilio_Cliente", cliente.DomicilioFiscal ?? "-"),
                        //new FieldContent("Notario_Empresa", empresa.Notario ?? "-"),
                        //new FieldContent("NoNotario_Empresa", empresa.NoNotario?.ToString() ?? "-"),
                        //new FieldContent("Notario_Cliente", cliente.Notario ?? "-"),
                        //new FieldContent("NoNotario_Cliente", cliente.NoNotario?.ToString() ?? "-")
                    );

                    outputDocument.FillContent(content);
                    outputDocument.SaveChanges();
                }

                var memory = new MemoryStream(await System.IO.File.ReadAllBytesAsync(outputPath));
                return File(memory,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    historial.ArchivoGenerado);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al generar contrato Word");
                return BadRequest($"Error al generar el contrato: {ex.Message}");
            }
        }


        //Métodos para agregar en nuevo contrato

        public async Task<JsonResult> OnGetObtenerSiguientesIdsAsync()
        {
            var siguienteEmpresaId = await db.EmpresaContratos.MaxAsync(e => (int?)e.Id) ?? 0;
            var siguienteClienteId = await db.ClienteContratos.MaxAsync(c => (int?)c.Id) ?? 0;

            return new JsonResult(new
            {
                empresaId = siguienteEmpresaId + 1,
                clienteId = siguienteClienteId + 1
            });
        }

        public async Task<JsonResult> OnGetTiposContratoAsync()
        {
            var tipos = await tipoContratosManager.GetAllAsync();
            var result = tipos.Select(t => new { id = t.Id, nombre = t.Nombre }).ToList();
            return new JsonResult(result);
        }

        public async Task<JsonResult> OnGetEmpresasContratoAsync()
        {
            var empresas = await empresaContratosManager.GetAllAsync();
            var activas = empresas.Where(e => !e.Deshabilitado)
                                  .Select(e => new { id = e.Id, nombre = e.RazonSocial ?? "-" })
                                  .ToList();
            return new JsonResult(activas);
        }

        public async Task<JsonResult> OnGetClientesContratoAsync()
        {
            var clientes = await clienteContratosManager.GetAllAsync(); // Asegúrate de tener este método
            var activos = clientes.Select(c => new { id = c.Id, nombre = c.RazonSocial ?? "-" }).ToList();
            return new JsonResult(activos);
        }

        public async Task<JsonResult> OnGetEmpresasAsync()
        {
            var empresas = await empresaManager.GetAllAsync();
            var result = empresas.Select(e => new {
                id = e.Id,
                nombre = e.RazonSocial ?? "-",
                rfc = e.RFC ?? "-",
                domicilioFiscal = e.DomicilioFiscal ?? "-",
                representanteLegal = e.Administrador ?? "-",
                fechaConstitucion = e.FechaConstitucion, 
                correoElectronico = e.CorreoGeneral ?? "-",

            }).ToList();

            return new JsonResult(result);
        }

        public async Task<JsonResult> OnGetClientesAsync()
        {
            var empresas = await empresaManager.GetAllAsync();
            var result = empresas.Select(e => new {
                id = e.Id,
                nombre = e.RazonSocial ?? "-",
                crfc = e.RFC ?? "-",
                cdomicilioFiscal = e.DomicilioFiscal ?? "-",
                crepresentanteLegal = e.Administrador ?? "-",
                cfechaConstitucion = e.FechaConstitucion,
                ccorreoElectronico = e.CorreoGeneral ?? "-",

            }).ToList();

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnPostGuardarContratoAsync([FromBody] GuardarContratoRequest request)
        {
            var response = new ServerResponse(true, localizer["ErrorIneSavedUnSuccessfully"]);

            try
            {
                var empresa = new EmpresaContrato
                {
                    RazonSocial = request.PrestadorNombre,
                    RFC = request.PrestadorRFC,
                    DomicilioFiscal = request.PrestadorDomicilio,
                    RepresentanteLegal = request.PrestadorRepresentante,
                    Email = request.PrestadorEmail,
                    FechaConstitucion = request.PrestadorFecha,
                    FechaInicio = request.PrestadorFechaInicio,
                    FechaFin = request.PrestadorFechaFin,
                    TipoContratoId = request.TipoContratoPrestadorId,
                    NoNotario = request.PrestadorNoNotario,
                    Notario = request.PrestadorNotario,
                    PaginaWeb = request.PrestadorPaginaWeb,
                    Deshabilitado = false
                };

                await db.EmpresaContratos.AddAsync(empresa);
                await db.SaveChangesAsync();

                var cliente = new ClienteContrato
                {
                    RazonSocial = request.PrestatarioNombre,
                    RFC = request.PrestatarioRFC,
                    DomicilioFiscal = request.PrestatarioDomicilio,
                    RepresentanteLegal = request.PrestatarioRepresentante,
                    Email = request.PrestatarioEmail,
                    FechaConstitucion = request.PrestatarioFecha,
                    FechaInicio = request.PrestatarioFechaInicio,
                    FechaFin = request.PrestatarioFechaFin,
                    TipoContratoId = request.TipoContratoPrestatarioId,
                    EmpresaContratoId = empresa.Id,
                    NoNotario = request.PrestatarioNoNotario,
                    Notario = request.PrestatarioNotario,
                    PaginaWeb = request.PrestatarioPaginaWeb,
                    Deshabilitado = false
                };

                await db.ClienteContratos.AddAsync(cliente);
                await db.SaveChangesAsync();

                response.TieneError = false;
                response.Mensaje = localizer["ContactSavedSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, localizer["ErrorSaveContradUnSuccessfully"]);
                response.Mensaje = localizer["ErrorIneSavedUnSuccessfully"];
            }

            return new JsonResult(response);
        }

        public JsonResult OnGetListaTipoContratos()
        {
            var tipos = db.TipoContratos
                .Select(t => new { id = t.Id, nombre = t.Nombre })
                .ToList();

            return new JsonResult(tipos);
        }

        public async Task<JsonResult> OnGetObtenerTipoContratosAsync()
        {
            var tipos = await db.TipoContratos
                .Select(t => new { id = t.Id, nombre = t.Nombre })
                .ToListAsync();

            return new JsonResult(tipos);
        }



        public async Task<JsonResult> OnPostActualizarContratoAsync([FromBody] ActualizarContratoRequest request)
        {
            var response = new ServerResponse(true, localizer["ErrorIneSavedUnSuccessfully"]);

            try
            {
                var empresa = await db.EmpresaContratos.FirstOrDefaultAsync(c => c.Id == request.Empresa.Id);
                var cliente = await db.ClienteContratos.FirstOrDefaultAsync(c => c.Id == request.Cliente.Id);

                if (empresa == null || cliente == null)
                {
                    response.Mensaje = "Prestador o Prestatario no encontrado.";
                    return new JsonResult(response);
                }

                // Actualizar empresa (Prestador)
                empresa.RazonSocial = request.Empresa.RazonSocial;
                empresa.RFC = request.Empresa.RFC;
                empresa.DomicilioFiscal = request.Empresa.DomicilioFiscal;
                empresa.RepresentanteLegal = request.Empresa.RepresentanteLegal;
                empresa.Email = request.Empresa.Email;
                empresa.FechaConstitucion = request.Empresa.FechaConstitucion;
                empresa.FechaInicio = request.Empresa.FechaInicio;
                empresa.FechaFin = request.Empresa.FechaFin;
                empresa.TipoContratoId = request.Empresa.TipoContratoId;
                empresa.NoNotario = request.Empresa.NoNotario;
                empresa.Notario = request.Empresa.Notario;
                empresa.PaginaWeb = request.Empresa.PaginaWeb;

                db.EmpresaContratos.Update(empresa);

                // Actualizar cliente (Prestatario)
                cliente.RazonSocial = request.Cliente.RazonSocial;
                cliente.RFC = request.Cliente.RFC;
                cliente.DomicilioFiscal = request.Cliente.DomicilioFiscal;
                cliente.RepresentanteLegal = request.Cliente.RepresentanteLegal;
                cliente.Email = request.Cliente.Email;
                cliente.FechaConstitucion = request.Cliente.FechaConstitucion;
                cliente.FechaInicio = request.Cliente.FechaInicio;
                cliente.FechaFin = request.Cliente.FechaFin;
                cliente.TipoContratoId = request.Cliente.TipoContratoId;
                cliente.NoNotario = request.Cliente.NoNotario;
                cliente.Notario = request.Cliente.Notario;
                cliente.PaginaWeb = request.Cliente.PaginaWeb;

                db.ClienteContratos.Update(cliente);

                await db.SaveChangesAsync();

                response.TieneError = false;
                response.Mensaje = localizer["ContactUpdatedSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, localizer["ErrorSaveContradUnSuccessfully"]);
                response.Mensaje = localizer["ErrorIneSavedUnSuccessfully"];
            }

            return new JsonResult(response);
        }

        public async Task<JsonResult> OnPostGetPrestadoresSuggestion(string texto)
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
            try
            {
                resp.Datos = await GetEmpresasSuggestion(texto); // Reutilizado
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en GetPrestadoresSuggestion");
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> OnPostGetPrestatariosSuggestion(string texto)
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
            try
            {
                resp.Datos = await GetEmpresasSuggestion(texto); // Reutilizado
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en GetPrestatariosSuggestion");
            }

            return new JsonResult(resp);
        }

        private async Task<string> GetEmpresasSuggestion(string texto)
        {
            string jsonResponse;
            List<string> jsonEmpresas = [];

            var empresas = await db.Empresas
                .Where(e => e.RazonSocial.Contains(texto))
                .ToListAsync();

            foreach (var e in empresas)
            {
                string desc = e.RazonSocial ?? "-";
                jsonEmpresas.Add($"{{" +
                    $"\"id\": \"{e.Id}\", " +
                    $"\"value\": \"{desc}\", " +
                    $"\"label\": \"{desc}\"" +
                $"}}");
            }

            jsonResponse = $"[{string.Join(",", jsonEmpresas)}]";
            return jsonResponse;
        }



    }
}