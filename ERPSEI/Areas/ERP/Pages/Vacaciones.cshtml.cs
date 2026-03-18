using Microsoft.AspNetCore.Mvc.RazorPages;
using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers;
using ERPSEI.Data.Managers.Empleados;
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
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using ERPSEI.Requests;
using OfficeOpenXml;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using iText.Layout;
using MathNet.Numerics.Distributions;
using ERPSEI.Data.Entities.Vacaciones;
using ERPSEI.Data.Managers.Vacaciones;
using ERPSEI.Areas.ERP.Pages;
using ERPSEI.Data.Managers.ActivosFijos;
using System.Security.Claims;
using iText.Commons.Actions.Contexts;

namespace ERPSEI.Areas.ERP.Pages
{
    [Authorize]
    public class VacacionesModel : ERPPageModel
    {
        private readonly IStringLocalizer<VacacionesModel> stringLocalizer;
        private readonly ILogger<VacacionesModel> logger;
        private readonly AppUserManager appUserManager;
        private readonly AppUserManager userManager;
        private readonly IStringLocalizer<VacacionesModel> localizer;
        private readonly ISolicitudVacacionesManager solicitudVacacionesManager;
        private readonly IEmailSender _emailSender;
        private readonly IPoliticaVacacionManager politicaVacacionManager;

        //private readonly IActivoFijoManager activoFijoManager;
        //private readonly ICategoriaActivosFijosManager categoriaActivoFijoManager;
        //private readonly ITipoActivosFijosManager tipoActivoFijoManager;
        private readonly IEmpleadoManager empleadoActivoFijoManager;


        //private readonly Data.ApplicationDbContext db;
        ApplicationDbContext db;


        [BindProperty]
        public SolicitudVacaciones? SolicitudVacacionesList { get; set; }

        [BindProperty]
        public InputFiltroVacacionesModel InputFiltro { get; set; }
        public class InputFiltroVacacionesModel
        {
            [Display(Name = "Empleado")]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Empleado { get; set; }

            [Display(Name = "Autorizador")]
            [StringLength(50, ErrorMessage = "FieldLength", MinimumLength = 3)]
            [RegularExpression(RegularExpressions.AlphanumSpaceCommaDotParenthesisAmpersandMiddleDash, ErrorMessage = "PersonName")]
            public string? Autorizador { get; set; }

            [Display(Name = "Estado")]
            public EstadoSolicitud? Estado { get; set; }

            [Display(Name = "Fecha Inicio")]
            [DataType(DataType.Date, ErrorMessage = "Debe seleccionar una fecha válida.")]
            public DateTime? FechaInicioDesde { get; set; }

            [Display(Name = "Fecha Fin")]
            [DataType(DataType.Date, ErrorMessage = "Debe seleccionar una fecha válida.")]
            public DateTime? FechaFinHasta { get; set; }

            // Validación adicional (opcional) para coherencia de fechas
            public bool RangoDeFechasEsValido =>
                !FechaInicioDesde.HasValue || !FechaFinHasta.HasValue || FechaInicioDesde <= FechaFinHasta;
        }

        [BindProperty]
        public VacacionesTableModel InputVacaciones { get; set; }

        public class VacacionesTableModel
        {
            [Display(Name = "ID")]
            public int Id { get; set; }

            [Display(Name = "Empleado")]
            [Required]
            [StringLength(100, ErrorMessage = "El nombre del empleado no puede exceder los 100 caracteres.")]
            public string Empleado { get; set; } = string.Empty;

            [Display(Name = "Fecha Inicio")]
            [DataType(DataType.Date, ErrorMessage = "Debe ser una fecha válida.")]
            public DateTime FechaInicio { get; set; }

            [Display(Name = "Fecha Fin")]
            [DataType(DataType.Date, ErrorMessage = "Debe ser una fecha válida.")]
            public DateTime FechaFin { get; set; }

            [Display(Name = "Días solicitados")]
            [Range(1, 365, ErrorMessage = "Los días solicitados deben ser entre 1 y 365.")]
            public int DiasSolicitados { get; set; }

            [Display(Name = "Estado")]
            [Required]
            [StringLength(20, ErrorMessage = "El estado no puede exceder los 20 caracteres.")]
            public string Estado { get; set; } = string.Empty;

            [Display(Name = "Autorizador")]
            [StringLength(100, ErrorMessage = "El nombre del autorizador no puede exceder los 100 caracteres.")]
            public string Autorizador { get; set; } = string.Empty;

            [Display(Name = "Fecha Solicitud")]
            [DataType(DataType.Date, ErrorMessage = "Debe ser una fecha válida.")]
            public DateTime FechaSolicitud { get; set; }
        }

        public List<Empleado> Empleados { get; set; } = new();
        public List<Empleado> Autorizadores { get; set; } = new();

        //Modal crear, editar y eliminar
        [BindProperty]
        public InputSolicitudVacacionesModel InputSolicitud { get; set; }

        public int EmpleadoId { get; set; } // lo puedes setear al cargar la vista
        public decimal DiasDisponibles { get; set; } // lo puedes calcular desde el backend
        public class InputSolicitudVacacionesModel
        {
            [Required]
            [DataType(DataType.Date)]
            public DateTime FechaInicio { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime FechaFin { get; set; }

            [StringLength(300)]
            public string? ComentarioEmpleado { get; set; }

            public int EmpleadoId { get; set; }
        }

        public List<VacacionesAcumuladasModel> ListaVacacionesAcumuladas { get; set; } = new();

        public class VacacionesAcumuladasModel
        {
            public DateTime Fecha { get; set; }
            public decimal NumeroDias { get; set; }
            public string Tipo { get; set; } = string.Empty;
            public DateTime? Vencimiento { get; set; }
            public string Periodo { get; set; } = string.Empty;
        }

        public List<VacacionesTomadasModel> ListaVacacionesTomadas { get; set; } = new();
        public class VacacionesTomadasModel
        {
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public int DiasSolicitados { get; set; }
            public string Tipo { get; set; } = "Legales"; // Por default
            public string Estado { get; set; } = string.Empty;
        }


        public VacacionesModel(
                IStringLocalizer<VacacionesModel> _stringLocalizer,
                ILogger<VacacionesModel> _logger,
                AppUserManager _appUserManager,
                IStringLocalizer<VacacionesModel> _localizer,
                ApplicationDbContext _db,
                AppUserManager _userManager,
                IEmpleadoManager empleadoManager,
                ISolicitudVacacionesManager _solicitudVacacionesManager,
                IPoliticaVacacionManager _politicaVacacionManager,
                IEmailSender emailSender

            //IActivoFijoManager _activoFijoManager,
            //ICategoriaActivosFijosManager categoriaManager,
            //ITipoActivosFijosManager tipoManager
            )
        {
            stringLocalizer = _stringLocalizer;
            logger = _logger;
            appUserManager = _appUserManager;
            localizer = _localizer;
            db = _db;

            //activoFijoManager = _activoFijoManager;
            userManager = _userManager;
            empleadoActivoFijoManager = empleadoManager;
            solicitudVacacionesManager = _solicitudVacacionesManager;
            politicaVacacionManager = _politicaVacacionManager;
            _emailSender = emailSender;

            InputFiltro = new InputFiltroVacacionesModel();
            InputVacaciones = new VacacionesTableModel();
            SolicitudVacacionesList = new SolicitudVacaciones();
        }

        //Método para mostrar las solicitudes enviadas de vacaciones
        /*public async Task<JsonResult> OnGetVacacionesList()
        {
            var solicitudes = await db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                .Include(s => s.Autorizador)
                .ToListAsync();

            var jsonVacaciones = new List<object>();

            foreach (var s in solicitudes)
            {
                jsonVacaciones.Add(new
                {
                    id = s.Id,
                    empleado = s.Empleado?.NombreCompleto ?? "-",
                    fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                    fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                    fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                    diasSolicitados = s.DiasSolicitados,
                    estado = s.Estado.ToString(),
                    autorizador = s.Autorizador?.NombreCompleto ?? "-"
                });
            }

            return new JsonResult(jsonVacaciones);
        }*/
        public async Task<JsonResult> OnGetVacacionesList()
        {
            var userEmail = User.Identity?.Name;

            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario?.Empleado == null)
                return new JsonResult(new List<object>());

            int empleadoIdActual = usuario.Empleado.Id;
            bool esAdmin = User.IsInRole("Administrador");

            var solicitudesQuery = db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                .Include(s => s.Autorizador)
                .AsQueryable();

            if (!esAdmin)
            {
                solicitudesQuery = solicitudesQuery.Where(s =>
                    s.EmpleadoId == empleadoIdActual ||
                    s.AutorizadorId == empleadoIdActual
                );
            }

            var solicitudes = await solicitudesQuery
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            var jsonVacaciones = solicitudes.Select(s => new
            {
                id = s.Id,
                empleado = s.Empleado?.NombreCompleto ?? "-",
                empleadoId = s.EmpleadoId,
                fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                diasSolicitados = s.DiasSolicitados,
                estado = s.Estado.ToString(),
                autorizador = s.Autorizador?.NombreCompleto ?? "-",
                autorizadorId = s.AutorizadorId,
                comentarioEmpleado = s.ComentarioEmpleado ?? ""
            }).ToList();

            return new JsonResult(jsonVacaciones);
        }

        public async Task<JsonResult> OnGetPoliticaVacaciones(string tipoVacacion = "Legales")
        {
            try
            {
                var politica = await politicaVacacionManager.GetPorTipoAsync(tipoVacacion);

                if (politica == null)
                {
                    return new JsonResult(new
                    {
                        error = true,
                        mensaje = "No se encontró una política activa para el tipo de vacaciones solicitado."
                    });
                }

                var result = new
                {
                    error = false,
                    politicaId = politica.Id,
                    nombre = politica.Nombre,
                    tipoVacacion = politica.TipoVacacion,
                    detalles = politica.Detalles
                        .OrderBy(d => d.Orden)
                        .Select(d => new
                        {
                            aniosAntiguedad = d.AniosAntiguedad,
                            diasVacaciones = d.DiasVacaciones,
                            primaVacacional = d.PrimaVacacional,
                            diasAguinaldo = d.DiasAguinaldo
                        }).ToList()
                };

                return new JsonResult(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al consultar la política de vacaciones");
                return new JsonResult(new
                {
                    error = true,
                    mensaje = "Ocurrió un error al consultar la política."
                });
            }
        }

        private async Task<string> ObtenerTipoVisualizacionVacacionesAsync()
        {
            var config = await db.ConfiguracionesVacaciones.FirstOrDefaultAsync(c => c.Id == 1);

            if (config == null || string.IsNullOrWhiteSpace(config.TipoVisualizacion))
                return "LegalesProporcionales";

            return config.TipoVisualizacion;
        }

        public async Task<JsonResult> OnGetObtenerAsignacionVacaciones()
        {
            try
            {
                var config = await db.ConfiguracionesVacaciones.FirstOrDefaultAsync(c => c.Id == 1);

                if (config == null)
                {
                    return new JsonResult(new
                    {
                        error = false,
                        tipoAsignacion = "LegalesProporcionales"
                    });
                }

                return new JsonResult(new
                {
                    error = false,
                    tipoAsignacion = config.TipoVisualizacion
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al obtener configuración global de vacaciones");
                return new JsonResult(new
                {
                    error = true,
                    mensaje = "No se pudo obtener la configuración."
                });
            }
        }

        public async Task<JsonResult> OnPostGuardarAsignacionVacaciones(string tipoAsignacion)
        {
            ServerResponse resp = new(true, "No se pudo guardar la configuración.");

            try
            {
                if (tipoAsignacion != "Legales" && tipoAsignacion != "LegalesProporcionales")
                {
                    resp.TieneError = true;
                    resp.Mensaje = "Tipo de asignación inválido.";
                    return new JsonResult(resp);
                }

                var config = await db.ConfiguracionesVacaciones.FirstOrDefaultAsync(c => c.Id == 1);

                if (config == null)
                {
                    config = new ConfiguracionVacacion
                    {
                        Id = 1,
                        TipoVisualizacion = tipoAsignacion,
                        FechaActualizacion = DateTime.Now
                    };

                    db.ConfiguracionesVacaciones.Add(config);
                }
                else
                {
                    config.TipoVisualizacion = tipoAsignacion;
                    config.FechaActualizacion = DateTime.Now;

                    db.ConfiguracionesVacaciones.Update(config);
                }

                await db.SaveChangesAsync();

                resp.TieneError = false;
                resp.Mensaje = "Configuración guardada correctamente.";
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al guardar la configuración global de vacaciones");
                resp.TieneError = true;
                resp.Mensaje = "Ocurrió un error inesperado.";
            }

            return new JsonResult(resp);
        }

        /*public async Task<JsonResult> OnGetVacacionesList()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            var esAdmin = User.IsInRole("ADMINISTRADOR");

            var solicitudesQuery = db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                .Include(s => s.Autorizador)
                .AsQueryable();

            // Solo mostrar sus propias vacaciones si no es administrador
            if (!esAdmin && usuario?.Empleado != null)
                solicitudesQuery = solicitudesQuery.Where(s => s.EmpleadoId == usuario.Empleado.Id);

            var solicitudes = await solicitudesQuery.ToListAsync();

            var jsonVacaciones = solicitudes.Select(s => new
            {
                id = s.Id,
                empleado = s.Empleado?.NombreCompleto ?? "-",
                fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                diasSolicitados = s.DiasSolicitados,
                estado = s.Estado.ToString(),
                autorizador = s.Autorizador?.NombreCompleto ?? "-",
                comentarioEmpleado = s.ComentarioEmpleado?.ToString()
            }).ToList();

            return new JsonResult(jsonVacaciones);
        }*/


        /*public async Task<JsonResult> OnPostFiltrarVacaciones()
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);

            try
            {
                var solicitudes = await solicitudVacacionesManager.GetAllAsync(InputFiltro);

                var result = solicitudes.Select(s => new
                {
                    id = s.Id,
                    empleado = s.Empleado?.NombreCompleto ?? "-",
                    fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                    fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                    fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                    diasSolicitados = s.DiasSolicitados,
                    estado = s.Estado.ToString(),
                    autorizador = s.Autorizador?.NombreCompleto ?? "-"
                }).ToList();

                resp.Datos = result;
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al filtrar solicitudes de vacaciones");
            }

            return new JsonResult(resp);
        }*/

        public async Task<JsonResult> OnPostFiltrarVacaciones()
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);

            try
            {
                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario?.Empleado == null)
                {
                    resp.TieneError = true;
                    resp.Mensaje = "No se pudo identificar al empleado actual.";
                    return new JsonResult(resp);
                }

                int empleadoIdActual = usuario.Empleado.Id;
                bool esAdmin = User.IsInRole("Administrador");

                var solicitudes = await solicitudVacacionesManager.GetAllAsync(InputFiltro);

                if (!esAdmin)
                {
                    solicitudes = solicitudes.Where(s =>
                        s.EmpleadoId == empleadoIdActual ||
                        s.AutorizadorId == empleadoIdActual
                    ).ToList();
                }

                var result = solicitudes.Select(s => new
                {
                    id = s.Id,
                    empleado = s.Empleado?.NombreCompleto ?? "-",
                    empleadoId = s.EmpleadoId,
                    fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                    fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                    fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                    diasSolicitados = s.DiasSolicitados,
                    estado = s.Estado.ToString(),
                    autorizador = s.Autorizador?.NombreCompleto ?? "-",
                    autorizadorId = s.AutorizadorId,
                    comentarioEmpleado = s.ComentarioEmpleado ?? ""
                }).ToList();

                resp.Datos = result;
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al filtrar solicitudes de vacaciones");
            }

            return new JsonResult(resp);
        }

        /*public async Task<JsonResult> OnPostFiltrarVacaciones()
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
            try
            {
                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);
                var esAdmin = User.IsInRole("ADMINISTRADOR");

                var solicitudes = await solicitudVacacionesManager.GetAllAsync(InputFiltro);

                if (!esAdmin && usuario?.Empleado != null)
                    solicitudes = solicitudes.Where(s => s.EmpleadoId == usuario.Empleado.Id).ToList();

                var result = solicitudes.Select(s => new
                {
                    id = s.Id,
                    empleado = s.Empleado?.NombreCompleto ?? "-",
                    fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                    fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                    fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                    diasSolicitados = s.DiasSolicitados,
                    estado = s.Estado.ToString(),
                    autorizador = s.Autorizador?.NombreCompleto ?? "-"
                }).ToList();

                resp.Datos = result;
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al filtrar solicitudes de vacaciones");
            }

            return new JsonResult(resp);
        }*/

        /*public async Task<JsonResult> OnPostGuardarSolicitud()
        {
            ServerResponse resp = new(false, localizer["SolicitudVacacionesSavedUnsuccessfully"]);

            try
            {
                // Obtener usuario autenticado
                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario == null || usuario.Empleado == null)
                {
                    resp.TieneError = true;
                    resp.Mensaje = "No se pudo identificar al empleado actual.";
                    return new JsonResult(resp);
                }

                var empleado = usuario.Empleado;
                var fechaActual = DateTime.Now;

                // Calcular días solicitados (ej. 06/06 - 09/06 = 4 días)
                var diasSolicitados = Enumerable
                    .Range(0, (InputSolicitud.FechaFin - InputSolicitud.FechaInicio).Days + 1)
                    .Select(offset => InputSolicitud.FechaInicio.AddDays(offset))
                    .Count(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);


                var solicitud = new SolicitudVacaciones
                {
                    EmpleadoId = empleado.Id,
                    Empleado = empleado,
                    FechaSolicitud = fechaActual,
                    FechaInicio = InputSolicitud.FechaInicio,
                    FechaFin = InputSolicitud.FechaFin,
                    DiasSolicitados = diasSolicitados,
                    ComentarioEmpleado = InputSolicitud.ComentarioEmpleado,
                    ComentarioAutorizador = null,
                    Estado = EstadoSolicitud.Pendiente,
                    AutorizadorId = empleado.Id, 
                    FechaRespuesta = fechaActual
                };

                await solicitudVacacionesManager.CreateAsync(solicitud);

                resp.TieneError = false;
                resp.Mensaje = localizer["SolicitudVacacionesSavedSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al guardar la solicitud de vacaciones");
                resp.Mensaje = "Ocurrió un error inesperado.";
            }

            return new JsonResult(resp);
        }*/

        public async Task<JsonResult> OnPostGuardarSolicitud()
        {
            ServerResponse resp = new(false, localizer["SolicitudVacacionesSavedUnsuccessfully"]);

            try
            {
                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario == null || usuario.Empleado == null)
                {
                    resp.TieneError = true;
                    resp.Mensaje = "No se pudo identificar al empleado actual.";
                    return new JsonResult(resp);
                }

                var empleado = usuario.Empleado;
                var fechaActual = DateTime.Now;

                if (empleado.JefeId == null || empleado.JefeId == 0)
                {
                    resp.TieneError = true;
                    resp.Mensaje = "No se pudo identificar al jefe directo del empleado. Verifica la información del empleado.";
                    return new JsonResult(resp);
                }

                var diasSolicitados = Enumerable
                    .Range(0, (InputSolicitud.FechaFin - InputSolicitud.FechaInicio).Days + 1)
                    .Select(offset => InputSolicitud.FechaInicio.AddDays(offset))
                    .Count(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);

                var solicitud = new SolicitudVacaciones
                {
                    EmpleadoId = empleado.Id,
                    Empleado = empleado,
                    FechaSolicitud = fechaActual,
                    FechaInicio = InputSolicitud.FechaInicio,
                    FechaFin = InputSolicitud.FechaFin,
                    DiasSolicitados = diasSolicitados,
                    ComentarioEmpleado = InputSolicitud.ComentarioEmpleado,
                    ComentarioAutorizador = null,
                    Estado = EstadoSolicitud.Pendiente,
                    AutorizadorId = empleado.JefeId,
                    FechaRespuesta = fechaActual
                };

                await solicitudVacacionesManager.CreateAsync(solicitud);

                var autorizador = await db.Empleados
                    .Include(e => e.Usuario)
                    .FirstOrDefaultAsync(e => e.Id == solicitud.AutorizadorId);

                if (autorizador != null && !string.IsNullOrWhiteSpace(autorizador.Usuario?.Email))
                {
                    var fechaHoy = DateTime.Now;
                    decimal diasAcumulados = Math.Round((12m / 365m) * (fechaHoy - empleado.FechaIngreso).Days, 1);

                    var diasTomados = await db.SolicitudesVacaciones
                        .Where(s => s.EmpleadoId == empleado.Id && s.Estado != EstadoSolicitud.Rechazado)
                        .SumAsync(s => s.DiasSolicitados);

                    decimal diasDisponibles = Math.Max(diasAcumulados - diasTomados, 0);

                    string subject = "Solicitud de vacaciones pendiente de autorización";

                    var request = HttpContext.Request;
                    string baseUrl = $"{request.Scheme}://{request.Host}";
                    string urlAutorizacion = $"{baseUrl}/ERP/Vacaciones/Autorizar?id={solicitud.Id}";
                    string urlCancelar = $"{baseUrl}/ERP/Vacaciones/Cancelar?id={solicitud.Id}";

                    string message = $@"
                <p>Estimado(a) {autorizador.NombreCompleto},</p>
                <p>Se ha generado una nueva solicitud de vacaciones por parte del empleado:</p>
                <table border='1' cellpadding='8' cellspacing='0' style='border-collapse: collapse; font-family: Arial, sans-serif; font-size: 14px;'>
                    <tr>
                        <th style='background-color:#f2f2f2;'>Solicitante</th>
                        <td>{empleado.NombreCompleto}</td>
                    </tr>
                    <tr>
                        <th style='background-color:#f2f2f2;'>Rango de vacaciones</th>
                        <td>{InputSolicitud.FechaInicio:dd/MM/yyyy} al {InputSolicitud.FechaFin:dd/MM/yyyy}</td>
                    </tr>
                    <tr>
                        <th style='background-color:#f2f2f2;'>Días solicitados</th>
                        <td>{diasSolicitados} día(s)</td>
                    </tr>
                    <tr>
                        <th style='background-color:#f2f2f2;'>Días disponibles (saldo)</th>
                        <td>{diasDisponibles:0.0} día(s)</td>
                    </tr>
                    <tr>
                        <th style='background-color:#f2f2f2;'>Comentario</th>
                        <td>{(string.IsNullOrWhiteSpace(InputSolicitud.ComentarioEmpleado) ? "Ninguno" : InputSolicitud.ComentarioEmpleado)}</td>
                    </tr>
                </table>
                <br>
                <p style='font-weight:bold;'>Acciones disponibles:</p>
                <div style='margin-top:20px; display: flex; gap: 20px;'>
                    <a href='{urlAutorizacion}' style='padding: 10px 20px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                        ✅ Autorizar solicitud
                    </a>
                    <font color='white'>.........</font>
                    <a href='{urlCancelar}' style='padding: 10px 20px; background-color: #dc3545; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                        ❌ Cancelar solicitud
                    </a>
                </div>
                <br>
                <p style='color:gray;'>Este es un mensaje automático del sistema ERP.</p>";

                    _emailSender.SendEmailAsync(
                        autorizador.Usuario.Email,
                        subject,
                        message
                    );
                }

                resp.TieneError = false;
                resp.Mensaje = localizer["SolicitudVacacionesSavedSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al guardar la solicitud de vacaciones");
                resp.Mensaje = "Ocurrió un error inesperado.";
            }

            return new JsonResult(resp);
        }

        public async Task<JsonResult> OnGetResumenVacaciones()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario == null || usuario.Empleado == null)
                return new JsonResult(new { error = "Empleado no encontrado." });

            var empleado = usuario.Empleado;
            var fechaHoy = DateTime.Now.Date;

            string tipoAsignacion = await ObtenerTipoVisualizacionVacacionesAsync();

            decimal diasLegales = 0m;
            decimal diasProporcionales = 0m;

            if (fechaHoy >= empleado.FechaIngreso.Date.AddYears(1))
            {
                diasLegales = 12m;
                diasProporcionales = Math.Round((12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date.AddYears(1)).TotalDays, 1);
            }
            else
            {
                diasLegales = 0m;
                diasProporcionales = Math.Round((12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date).TotalDays, 1);
            }

            decimal acumuladas = tipoAsignacion == "Legales"
                ? diasLegales
                : diasLegales + diasProporcionales;

            var diasTomados = await db.SolicitudesVacaciones
                .Where(s => s.EmpleadoId == empleado.Id && s.Estado != EstadoSolicitud.Rechazado)
                .SumAsync(s => s.DiasSolicitados);

            decimal saldo = Math.Max(acumuladas - diasTomados, 0);

            return new JsonResult(new
            {
                Acumuladas = acumuladas,
                Tomadas = diasTomados,
                Vencidas = 0,
                Futuras = 0,
                Saldo = saldo,
                Fecha = DateTime.Now.ToString("dd-MM-yyyy"),
                TipoAsignacion = tipoAsignacion
            });
        }

        /*public async Task<JsonResult> OnGetResumenVacaciones()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);
            if (usuario == null || usuario.Empleado == null)
                return new JsonResult(new { error = "Empleado no encontrado." });

            var empleado = usuario.Empleado;
            var fechaHoy = DateTime.Now;

            // 1. Días acumulados proporcionales
            decimal diasAcumulados = Math.Round((12m / 365m) * (fechaHoy - empleado.FechaIngreso).Days, 1);

            // 2. Días tomados (aprobados o solicitados)
            var diasTomados = await db.SolicitudesVacaciones
                .Where(s => s.EmpleadoId == empleado.Id && s.Estado != EstadoSolicitud.Rechazado)
                .SumAsync(s => s.DiasSolicitados);

            // 3. Saldo actual
            decimal saldo = Math.Max(diasAcumulados - diasTomados, 0);

            return new JsonResult(new
            {
                Acumuladas = diasAcumulados,
                Tomadas = diasTomados,
                Vencidas = 0,
                Futuras = 0,
                Saldo = saldo,
                Fecha = DateTime.Now.ToString("dd-MM-yyyy")
            });
        }*/

        public async Task<JsonResult> OnGetObtenerDiasDisponibles()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario == null || usuario.Empleado == null)
                return new JsonResult(0);

            var empleado = usuario.Empleado;
            var fechaHoy = DateTime.Today;

            string tipoAsignacion = await ObtenerTipoVisualizacionVacacionesAsync();

            decimal diasLegales = 0m;
            decimal diasProporcionales = 0m;

            if (fechaHoy >= empleado.FechaIngreso.Date.AddYears(1))
            {
                diasLegales = 12m;
                diasProporcionales = Math.Round((12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date.AddYears(1)).TotalDays, 1);
            }
            else
            {
                diasLegales = 0m;
                diasProporcionales = Math.Round((12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date).TotalDays, 1);
            }

            decimal acumuladas = tipoAsignacion == "Legales"
                ? diasLegales
                : diasLegales + diasProporcionales;

            var diasTomados = await db.SolicitudesVacaciones
                .Where(s => s.EmpleadoId == empleado.Id && s.Estado != EstadoSolicitud.Rechazado)
                .SumAsync(s => s.DiasSolicitados);

            decimal saldo = Math.Max(acumuladas - diasTomados, 0);

            return new JsonResult(saldo);
        }

        /*public async Task<JsonResult> OnGetObtenerDiasDisponibles()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario == null || usuario.Empleado == null)
                return new JsonResult(0); // o JsonResult(new { error = "Empleado no encontrado" });

            decimal dias = await solicitudVacacionesManager.CalcularDiasDisponiblesAsync(usuario.Empleado.Id);
            return new JsonResult(dias);
        }*/

        public async Task<JsonResult> OnGetVacacionesAcumuladas()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario?.Empleado == null)
                return new JsonResult(new { error = "Empleado no encontrado." });

            var empleado = usuario.Empleado;
            var fechaIngreso = empleado.FechaIngreso.Date;
            var fechaActual = DateTime.Today;

            string tipoAsignacion = await ObtenerTipoVisualizacionVacacionesAsync();

            const decimal diasLegales = 12m;
            ListaVacacionesAcumuladas.Clear();

            if (fechaActual >= fechaIngreso.AddYears(1))
            {
                var fechaAniversario = fechaIngreso.AddYears(1);
                var vencimiento = fechaAniversario.AddMonths(18);

                ListaVacacionesAcumuladas.Add(new VacacionesAcumuladasModel
                {
                    Fecha = fechaAniversario,
                    NumeroDias = diasLegales,
                    Tipo = "Legales",
                    Vencimiento = vencimiento,
                    Periodo = $"{fechaAniversario.Year}-{vencimiento.Year}"
                });

                if (tipoAsignacion == "LegalesProporcionales")
                {
                    var diasProporcionales = Math.Round((diasLegales / 365) * (decimal)(fechaActual - fechaAniversario).TotalDays, 1);

                    if (diasProporcionales > 0)
                    {
                        ListaVacacionesAcumuladas.Add(new VacacionesAcumuladasModel
                        {
                            Fecha = fechaActual,
                            NumeroDias = diasProporcionales,
                            Tipo = "Legales (Proporcionales)",
                            Vencimiento = null,
                            Periodo = ""
                        });
                    }
                }
            }
            else
            {
                if (tipoAsignacion == "LegalesProporcionales")
                {
                    var diasTrabajados = (fechaActual - fechaIngreso).TotalDays;
                    var diasProporcionales = Math.Round((diasLegales / 365) * (decimal)diasTrabajados, 1);

                    ListaVacacionesAcumuladas.Add(new VacacionesAcumuladasModel
                    {
                        Fecha = fechaActual,
                        NumeroDias = diasProporcionales,
                        Tipo = "Legales (Proporcionales)",
                        Vencimiento = null,
                        Periodo = ""
                    });
                }
            }

            return new JsonResult(ListaVacacionesAcumuladas);
        }

        /*public async Task<JsonResult> OnGetVacacionesAcumuladas()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario?.Empleado == null)
                return new JsonResult(new { error = "Empleado no encontrado." });

            var empleado = usuario.Empleado;
            var fechaIngreso = empleado.FechaIngreso.Date;
            var fechaActual = DateTime.Today;

            const decimal diasLegales = 12m;

            if (fechaActual >= fechaIngreso.AddYears(1))
            {
                // Cumplió al menos 1 año
                var fechaAniversario = fechaIngreso.AddYears(1);
                var vencimiento = fechaAniversario.AddMonths(18);

                ListaVacacionesAcumuladas.Add(new VacacionesAcumuladasModel
                {
                    Fecha = fechaAniversario,
                    NumeroDias = diasLegales,
                    Tipo = "Legales",
                    Vencimiento = vencimiento,
                    Periodo = $"{fechaAniversario.Year}-{vencimiento.Year}"
                });

                var diasProporcionales = Math.Round((diasLegales / 365) * (decimal)(fechaActual - fechaAniversario).TotalDays, 1);

                if (diasProporcionales > 0)
                {
                    ListaVacacionesAcumuladas.Add(new VacacionesAcumuladasModel
                    {
                        Fecha = fechaActual,
                        NumeroDias = diasProporcionales,
                        Tipo = "Legales (Proporcionales)",
                        Vencimiento = null,
                        Periodo = ""
                    });
                }
            }
            else
            {
                // Solo proporcionales si no ha cumplido 1 año
                var diasTrabajados = (fechaActual - fechaIngreso).TotalDays;
                var diasProporcionales = Math.Round((diasLegales / 365) * (decimal)diasTrabajados, 1);

                ListaVacacionesAcumuladas.Add(new VacacionesAcumuladasModel
                {
                    Fecha = fechaActual,
                    NumeroDias = diasProporcionales,
                    Tipo = "Legales (Proporcionales)",
                    Vencimiento = null,
                    Periodo = ""
                });
            }

            return new JsonResult(ListaVacacionesAcumuladas);
        }*/

        public async Task<JsonResult> OnGetVacacionesTomadas()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario?.Empleado == null)
                return new JsonResult(new { error = "Empleado no encontrado." });

            var empleado = usuario.Empleado;

            var solicitudes = await db.SolicitudesVacaciones
                .Where(s => s.EmpleadoId == empleado.Id)
                .OrderByDescending(s => s.FechaInicio)
                .ToListAsync();

            var lista = solicitudes.Select(s => new
            {
                inicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                fin = s.FechaFin.ToString("dd/MM/yyyy"),
                dias = s.DiasSolicitados,
                tipo = "Legales",
                estado = s.Estado.ToString()
            }).ToList();

            return new JsonResult(lista);
        }

        public async Task<IActionResult> OnPostAutorizarSolicitudAsync(int idSolicitud, bool autorizar)
        {
            try
            {
                var solicitud = await db.SolicitudesVacaciones
                    .Include(s => s.Empleado)
                    .FirstOrDefaultAsync(s => s.Id == idSolicitud);

                if (solicitud == null)
                {
                    return new JsonResult(new { exito = false, mensaje = "Solicitud no encontrada." });
                }

                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario?.Empleado == null)
                {
                    return new JsonResult(new { exito = false, mensaje = "Usuario no encontrado." });
                }

                var empleadoIdActual = usuario.Empleado.Id;
                var esAdmin = User.IsInRole("ADMINISTRADOR");

                if (!esAdmin && solicitud.AutorizadorId != empleadoIdActual)
                {
                    return new JsonResult(new
                    {
                        exito = false,
                        mensaje = "No tienes permiso para autorizar esta solicitud."
                    });
                }

                if (solicitud.Estado != EstadoSolicitud.Pendiente)
                {
                    return new JsonResult(new
                    {
                        exito = false,
                        mensaje = "La solicitud ya fue procesada."
                    });
                }

                solicitud.Estado = autorizar ? EstadoSolicitud.Aprobado : EstadoSolicitud.Rechazado;
                solicitud.FechaRespuesta = DateTime.Now;

                db.SolicitudesVacaciones.Update(solicitud);
                await db.SaveChangesAsync();

                var historial = new HistorialVacaciones
                {
                    EmpleadoId = solicitud.EmpleadoId,
                    FechaInicio = solicitud.FechaInicio,
                    FechaFin = solicitud.FechaFin,
                    DiasTomados = solicitud.DiasSolicitados,
                    Observaciones = solicitud.ComentarioEmpleado ?? "",
                    SolicitudVacacionesId = solicitud.Id,
                    AutorizadorId = empleadoIdActual
                };

                db.HistorialesVacaciones.Add(historial);
                await db.SaveChangesAsync();

                return new JsonResult(new { exito = true });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar el estado de la solicitud");
                return new JsonResult(new { exito = false, mensaje = "Error inesperado." });
            }
        }

        /*public async Task<IActionResult> OnPostAutorizarSolicitudAsync(int idSolicitud, bool autorizar)
        {
            try
            {
                var solicitud = await db.SolicitudesVacaciones
                    .Include(s => s.Empleado)
                    .FirstOrDefaultAsync(s => s.Id == idSolicitud);

                if (solicitud == null)
                {
                    return new JsonResult(new { exito = false, mensaje = "Solicitud no encontrada." });
                }

                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario?.Empleado == null)
                {
                    return new JsonResult(new { exito = false, mensaje = "Usuario no encontrado." });
                }

                solicitud.Estado = autorizar ? EstadoSolicitud.Aprobado : EstadoSolicitud.Rechazado;
                solicitud.FechaRespuesta = DateTime.Now;

                db.SolicitudesVacaciones.Update(solicitud);
                await db.SaveChangesAsync();

                // ✅ Insertar en HistorialVacaciones para ambos casos
                var historial = new HistorialVacaciones
                {
                    EmpleadoId = solicitud.EmpleadoId,                 // Quien solicitó
                    FechaInicio = solicitud.FechaInicio,
                    FechaFin = solicitud.FechaFin,
                    DiasTomados = solicitud.DiasSolicitados,
                    Observaciones = solicitud.ComentarioEmpleado ?? "",
                    SolicitudVacacionesId = solicitud.Id,
                    AutorizadorId = usuario.Empleado.Id,               // Quien autoriza o rechaza
                };

                db.HistorialesVacaciones.Add(historial);
                await db.SaveChangesAsync();

                return new JsonResult(new { exito = true });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al actualizar el estado de la solicitud");
                return new JsonResult(new { exito = false, mensaje = "Error inesperado." });
            }
        }*/


        //Autocompletado Empleado y Autorizador
        public async Task<JsonResult> OnPostGetUsuariosSuggestion(string texto)
        {
            ServerResponse resp = new(true, localizer["ConsultadoUnsuccessfully"]);
            try
            {
                resp.Datos = await GetUsuariosSuggestion(texto);
                resp.TieneError = false;
                resp.Mensaje = localizer["ConsultadoSuccessfully"];
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }

            return new JsonResult(resp);
        }

        private async Task<string> GetUsuariosSuggestion(string texto)
        {
            string jsonResponse;
            List<string> jsonUsuarios = [];

            List<AppUser> usuarios = await userManager.SearchUsuarios(texto);

            if (usuarios != null)
            {
                foreach (AppUser u in usuarios)
                {
                    string desc = u.Empleado?.NombreCompleto.Length >= 1 ? $"{u.Empleado?.NombreCompleto}" : $"{u.UserName}";
                    jsonUsuarios.Add($"{{" +
                                        $"\"id\": \"{u.Id}\", " +
                                        $"\"value\": \"{desc}\", " +
                                        $"\"label\": \"{desc}\"" +
                                    $"}}");
                }
            }

            jsonResponse = $"[{string.Join(",", jsonUsuarios)}]";

            return jsonResponse;
        }
    }
}