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
using OfficeOpenXml.Style;
using System.Drawing;

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

        public bool EsJefeInmediato { get; set; }
        public bool PuedeAprobarJefeDirecto { get; set; }
        public bool PuedeAprobarTH { get; set; }
        public bool PuedeExportarDetalleVacaciones { get; set; }


        [BindProperty]
        public SolicitudVacaciones? SolicitudVacacionesList { get; set; }

        [BindProperty]
        public EditarSolicitudVacacionesModel InputEditarSolicitud { get; set; } = new();

        public class EditarSolicitudVacacionesModel
        {
            public int Id { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public string? ComentarioEmpleado { get; set; }
        }

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
            public bool EsVacacionAnticipada { get; set; }
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
            await ConfigurarPermisosVacacionesAsync();

            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario?.Empleado == null)
                return new JsonResult(new List<object>());

            int empleadoIdActual = usuario.Empleado.Id;

            var solicitudes = await db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                .Include(s => s.Autorizador)
                .Where(s => s.EmpleadoId == empleadoIdActual)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            var jsonVacaciones = solicitudes.Select(s =>
            {
                string estadoVisual = ObtenerEstadoVisualVacaciones(s);

                return new
                {
                    id = s.Id,
                    empleado = s.Empleado?.NombreCompleto ?? "-",
                    empleadoId = s.EmpleadoId,
                    fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                    fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                    fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                    diasSolicitados = s.DiasSolicitados,
                    estado = estadoVisual,
                    autorizador = s.Autorizador?.NombreCompleto ?? "-",
                    autorizadorId = s.AutorizadorId,
                    comentarioEmpleado = s.ComentarioEmpleado ?? "",

                    puedeEditar = s.EstadoJefeDirecto == "Pendiente",
                    puedeEliminar = s.EstadoJefeDirecto == "Pendiente",

                    // En la tabla personal NO se autoriza
                    puedeAprobarJefe = false,
                    puedeAprobarTH = false
                };
            }).ToList();

            return new JsonResult(jsonVacaciones);
        }

        private decimal ObtenerDiasVacacionesPorAntiguedad(int anios)
        {
            if (anios <= 0)
                return 0m;

            if (anios == 1)
                return 12m;

            if (anios == 2)
                return 14m;

            if (anios == 3)
                return 16m;

            if (anios == 4)
                return 18m;

            if (anios == 5)
                return 20m;

            if (anios >= 6 && anios <= 10)
                return 22m;

            if (anios >= 11 && anios <= 15)
                return 24m;

            if (anios >= 16 && anios <= 20)
                return 26m;

            if (anios >= 21 && anios <= 25)
                return 28m;

            if (anios >= 26 && anios <= 30)
                return 30m;

            if (anios >= 31 && anios <= 35)
                return 32m;

            return 34m; 
        }

        //Vacaciones Vencidas
        private async Task<decimal> ObtenerDiasVencidosAsync(int empleadoId)
        {
            var empleado = await db.Empleados.FirstOrDefaultAsync(e => e.Id == empleadoId);

            if (empleado == null)
                return 0m;

            var fechaHoy = DateTime.Today;
            var fechaIngreso = empleado.FechaIngreso.Date;
            decimal totalVencidos = 0m;

            int aniosCumplidos = fechaHoy.Year - fechaIngreso.Year;

            if (fechaHoy < fechaIngreso.AddYears(aniosCumplidos))
                aniosCumplidos--;

            for (int anio = 1; anio <= aniosCumplidos; anio++)
            {
                var fechaGeneracion = fechaIngreso.AddYears(anio);
                var fechaVencimiento = fechaGeneracion.AddYears(2);

                if (fechaHoy <= fechaVencimiento)
                    continue;

                decimal diasDelPeriodo = ObtenerDiasVacacionesPorAntiguedad(anio);

                var diasTomados = await db.SolicitudesVacaciones
                    .Where(s =>
                        s.EmpleadoId == empleadoId &&
                        s.EstadoJefeDirecto == "Aprobado" &&
                        s.EstadoTH == "Aprobado" &&
                        !s.EsVacacionAnticipada &&
                        s.FechaInicio >= fechaGeneracion &&
                        s.FechaInicio <= fechaVencimiento)
                    .SumAsync(s => (decimal?)s.DiasSolicitados) ?? 0m;

                decimal diasVencidos = Math.Max(diasDelPeriodo - diasTomados, 0m);

                if (diasVencidos <= 0)
                    continue;

                string periodo = $"{fechaGeneracion:dd/MM/yyyy} - {fechaVencimiento:dd/MM/yyyy}";

                bool yaExiste = await db.HistorialVacacionesVencidas.AnyAsync(h =>
                    h.EmpleadoId == empleadoId &&
                    h.FechaGeneracion == fechaGeneracion &&
                    h.FechaVencimiento == fechaVencimiento);

                if (!yaExiste)
                {
                    db.HistorialVacacionesVencidas.Add(new HistorialVacacionVencida
                    {
                        EmpleadoId = empleadoId,
                        FechaGeneracion = fechaGeneracion,
                        FechaVencimiento = fechaVencimiento,
                        DiasVencidos = diasVencidos,
                        Periodo = periodo,
                        Causa = "Días vencidos conforme a las políticas de la firma."
                    });
                }

                totalVencidos += diasVencidos;
            }

            await db.SaveChangesAsync();
            return totalVencidos;
        }

        private async Task ConfigurarPermisosVacacionesAsync()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario == null)
            {
                EsJefeInmediato = false;
                PuedeAprobarJefeDirecto = false;
                PuedeAprobarTH = false;
                PuedeExportarDetalleVacaciones = false;
                return;
            }

            var roles = await userManager.GetRolesAsync(usuario);

            bool esAdministrador = roles.Contains("Administrador") || roles.Contains("Master");
            bool esAdministradorTH = roles.Contains("Administrador TH");

            int? empleadoIdActual = usuario.Empleado?.Id;

            EsJefeInmediato = empleadoIdActual.HasValue &&
                await db.Empleados.AsNoTracking().AnyAsync(e => e.JefeId == empleadoIdActual.Value);

            PuedeAprobarJefeDirecto = EsJefeInmediato || esAdministrador;
            PuedeAprobarTH = esAdministradorTH || esAdministrador;
            PuedeExportarDetalleVacaciones = esAdministrador || esAdministradorTH;
        }

        public async Task OnGetAsync()
        {
            await ConfigurarPermisosVacacionesAsync();
        }

        public async Task<JsonResult> OnGetMisVacacionesListAsync()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario?.Empleado == null)
                return new JsonResult(new List<object>());

            int empleadoIdActual = usuario.Empleado.Id;

            var solicitudes = await db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                .Include(s => s.Autorizador)
                .Where(s => s.EmpleadoId == empleadoIdActual)
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            var json = solicitudes.Select(s => new
            {
                id = s.Id,
                empleado = s.Empleado?.NombreCompleto ?? "-",
                fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                diasSolicitados = s.DiasSolicitados,
                estado = ObtenerEstadoVisualVacaciones(s),
                autorizador = s.Autorizador?.NombreCompleto ?? "-",
                comentarioEmpleado = s.ComentarioEmpleado ?? "",
                puedeEditar = s.EstadoJefeDirecto == "Pendiente",
                puedeEliminar = s.EstadoJefeDirecto == "Pendiente",
                puedeAprobarJefe = false,
                puedeAprobarTH = false
            }).ToList();

            return new JsonResult(json);
        }

        public async Task<JsonResult> OnPostEditarSolicitudAsync()
        {
            try
            {
                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario?.Empleado == null)
                    return new JsonResult(new { tieneError = true, mensaje = "No se encontró el empleado actual." });

                var solicitud = await db.SolicitudesVacaciones
                    .FirstOrDefaultAsync(s => s.Id == InputEditarSolicitud.Id);

                if (solicitud == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Solicitud no encontrada." });

                if (solicitud.EmpleadoId != usuario.Empleado.Id)
                    return new JsonResult(new { tieneError = true, mensaje = "No puedes editar esta solicitud." });

                if (solicitud.EstadoJefeDirecto != "Pendiente")
                    return new JsonResult(new { tieneError = true, mensaje = "Solo puedes editar solicitudes pendientes." });

                if (InputEditarSolicitud.FechaFin < InputEditarSolicitud.FechaInicio)
                    return new JsonResult(new { tieneError = true, mensaje = "La fecha fin no puede ser menor que la fecha inicio." });

                var diasSolicitados = Enumerable
                    .Range(0, (InputEditarSolicitud.FechaFin - InputEditarSolicitud.FechaInicio).Days + 1)
                    .Select(offset => InputEditarSolicitud.FechaInicio.AddDays(offset))
                    .Count(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);

                solicitud.FechaInicio = InputEditarSolicitud.FechaInicio;
                solicitud.FechaFin = InputEditarSolicitud.FechaFin;
                solicitud.DiasSolicitados = diasSolicitados;
                solicitud.ComentarioEmpleado = InputEditarSolicitud.ComentarioEmpleado;

                db.SolicitudesVacaciones.Update(solicitud);
                await db.SaveChangesAsync();

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "La solicitud se actualizó correctamente."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al editar solicitud de vacaciones");
                return new JsonResult(new
                {
                    tieneError = true,
                    mensaje = "Ocurrió un error al editar la solicitud."
                });
            }
        }

        public async Task<JsonResult> OnGetSolicitudesAutorizarAsync()
        {
            await ConfigurarPermisosVacacionesAsync();

            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario == null)
                return new JsonResult(new List<object>());

            int? empleadoIdActual = usuario.Empleado?.Id;

            var roles = await userManager.GetRolesAsync(usuario);
            bool esAdministrador = roles.Contains("Administrador") || roles.Contains("Master");
            bool esTH = roles.Contains("Administrador TH");

            var query = db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                .Include(s => s.Autorizador)
                .AsQueryable();

            if (esAdministrador)
            {
                // ve todo
            }
            else if (esTH)
            {
                // ve todo
            }
            else if (EsJefeInmediato && empleadoIdActual.HasValue)
            {
                query = query.Where(s => s.JefeDirectoEmpleadoId == empleadoIdActual.Value);
            }
            else
            {
                return new JsonResult(new List<object>());
            }

            var solicitudes = await query
                .OrderByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            var result = solicitudes.Select(s => new
            {
                id = s.Id,
                empleado = s.Empleado?.NombreCompleto ?? "-",
                fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                diasSolicitados = s.DiasSolicitados,
                estado = ObtenerEstadoVisualVacaciones(s),
                autorizador = s.Autorizador?.NombreCompleto ?? "-",
                comentarioEmpleado = s.ComentarioEmpleado ?? "",

                puedeEditar = false,
                puedeEliminar = false,

                puedeAprobarJefe =
                    (esAdministrador || (empleadoIdActual.HasValue && s.JefeDirectoEmpleadoId == empleadoIdActual.Value)) &&
                    s.EstadoJefeDirecto == "Pendiente",

                puedeAprobarTH =
                    (esAdministrador || esTH) &&
                    s.EstadoJefeDirecto == "Aprobado" &&
                    s.EstadoTH == "Pendiente"
            }).ToList();

            return new JsonResult(result);
        }

        public async Task<JsonResult> OnGetDetalleVacacionAsync(int id)
        {
            await ConfigurarPermisosVacacionesAsync();

            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario == null)
                return new JsonResult(new { tieneError = true, mensaje = "Usuario no encontrado." });

            int? empleadoIdActual = usuario.Empleado?.Id;

            var roles = await userManager.GetRolesAsync(usuario);
            bool esAdministrador = roles.Contains("Administrador") || roles.Contains("Master");
            bool esAdministradorTH = roles.Contains("Administrador TH");

            var query = db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                .Include(s => s.Autorizador)
                .AsQueryable();

            if (!esAdministrador && !esAdministradorTH)
            {
                if (EsJefeInmediato && empleadoIdActual.HasValue)
                {
                    query = query.Where(s =>
                        s.EmpleadoId == empleadoIdActual.Value ||
                        s.JefeDirectoEmpleadoId == empleadoIdActual.Value);
                }
                else if (empleadoIdActual.HasValue)
                {
                    query = query.Where(s => s.EmpleadoId == empleadoIdActual.Value);
                }
            }

            var item = await query.FirstOrDefaultAsync(s => s.Id == id);

            if (item == null)
                return new JsonResult(new { tieneError = true, mensaje = "Solicitud no encontrada." });

            return new JsonResult(new
            {
                tieneError = false,
                id = item.Id,
                empleado = item.Empleado?.NombreCompleto ?? "",
                estado = ObtenerEstadoVisualVacaciones(item),
                fechaSolicitud = item.FechaSolicitud.ToString("dd/MM/yyyy"),
                fechaInicio = item.FechaInicio.ToString("dd/MM/yyyy"),
                fechaFin = item.FechaFin.ToString("dd/MM/yyyy"),
                diasSolicitados = item.DiasSolicitados,
                autorizador = item.Autorizador?.NombreCompleto ?? "",
                comentario = item.ComentarioEmpleado ?? "",

                puedeAprobarJefe =
                    (esAdministrador || EsJefeInmediato) &&
                    item.EstadoJefeDirecto == "Pendiente" &&
                    (esAdministrador || (empleadoIdActual.HasValue && item.JefeDirectoEmpleadoId == empleadoIdActual.Value)),

                puedeAprobarTH =
                    (esAdministrador || esAdministradorTH) &&
                    item.EstadoJefeDirecto == "Aprobado" &&
                    item.EstadoTH == "Pendiente"
            });
        }

        private string ObtenerEstadoVisualVacaciones(SolicitudVacaciones s)
        {
            if (s.EstadoJefeDirecto == "Rechazado")
                return "Rechazado por jefe directo";

            if (s.EstadoTH == "Rechazado")
                return "Rechazado por TH";

            if (s.EstadoJefeDirecto == "Pendiente")
                return "Pendiente jefe directo";

            if (s.EstadoJefeDirecto == "Aprobado" && s.EstadoTH == "Pendiente")
                return "Pendiente TH";

            if (s.EstadoJefeDirecto == "Aprobado" && s.EstadoTH == "Aprobado")
                return "Aprobado";

            return s.Estado.ToString();
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
            await ConfigurarPermisosVacacionesAsync();

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

                var roles = await userManager.GetRolesAsync(usuario);
                bool esAdministrador = roles.Contains("Administrador") || roles.Contains("Master");
                bool esAdministradorTH = roles.Contains("Administrador TH");

                var solicitudes = await solicitudVacacionesManager.GetAllAsync(InputFiltro);

                // La tabla superior siempre debe mostrar SOLO las vacaciones del usuario logueado
                solicitudes = solicitudes.Where(s => s.EmpleadoId == empleadoIdActual).ToList();

                var result = solicitudes.Select(s =>
                {
                    string estadoVisual = ObtenerEstadoVisualVacaciones(s);

                    return new
                    {
                        id = s.Id,
                        empleado = s.Empleado?.NombreCompleto ?? "-",
                        empleadoId = s.EmpleadoId,
                        fechaSolicitud = s.FechaSolicitud.ToString("dd/MM/yyyy"),
                        fechaInicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                        fechaFin = s.FechaFin.ToString("dd/MM/yyyy"),
                        diasSolicitados = s.DiasSolicitados,
                        estado = estadoVisual,
                        autorizador = s.Autorizador?.NombreCompleto ?? "-",
                        autorizadorId = s.AutorizadorId,
                        comentarioEmpleado = s.ComentarioEmpleado ?? "",

                        puedeEditar =
                            esAdministrador ||
                            (s.EmpleadoId == empleadoIdActual && s.EstadoJefeDirecto == "Pendiente"),

                        puedeEliminar =
                            esAdministrador ||
                            (s.EmpleadoId == empleadoIdActual && s.EstadoJefeDirecto == "Pendiente"),

                        puedeAprobarJefe =
                            (esAdministrador || EsJefeInmediato) &&
                            s.EstadoJefeDirecto == "Pendiente" &&
                            (esAdministrador || s.JefeDirectoEmpleadoId == empleadoIdActual),

                        puedeAprobarTH =
                            (esAdministrador || esAdministradorTH) &&
                            s.EstadoJefeDirecto == "Aprobado" &&
                            s.EstadoTH == "Pendiente"
                    };
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

        private async Task<decimal> OnGetObtenerDiasDisponiblesInternoAsync(int empleadoId)
        {
            var empleado = await db.Empleados.FirstOrDefaultAsync(e => e.Id == empleadoId);

            if (empleado == null)
                return 0m;

            var fechaHoy = DateTime.Today;
            string tipoAsignacion = await ObtenerTipoVisualizacionVacacionesAsync();

            decimal diasLegales = 0m;
            decimal diasProporcionales = 0m;

            int aniosCumplidos = fechaHoy.Year - empleado.FechaIngreso.Date.Year;

            if (fechaHoy < empleado.FechaIngreso.Date.AddYears(aniosCumplidos))
                aniosCumplidos--;

            decimal diasPorAnio = ObtenerDiasVacacionesPorAntiguedad(aniosCumplidos);
            decimal diasProximoAnio = ObtenerDiasVacacionesPorAntiguedad(aniosCumplidos + 1);

            if (aniosCumplidos >= 1)
            {
                var ultimoAniversario = empleado.FechaIngreso.Date.AddYears(aniosCumplidos);

                diasLegales = diasPorAnio;
                diasProporcionales = Math.Round(
                    (diasProximoAnio / 365m) * (decimal)(fechaHoy - ultimoAniversario).TotalDays, 1);
            }
            else
            {
                diasLegales = 0m;
                diasProporcionales = Math.Round(
                    (12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date).TotalDays, 1);
            }

            /*if (fechaHoy >= empleado.FechaIngreso.Date.AddYears(1))
            {
                diasLegales = 12m;
                diasProporcionales = Math.Round((12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date.AddYears(1)).TotalDays, 1);
            }
            else
            {
                diasLegales = 0m;
                diasProporcionales = Math.Round((12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date).TotalDays, 1);
            }*/

            decimal acumuladas = tipoAsignacion == "Legales"
                ? diasLegales
                : diasLegales + diasProporcionales;

            var diasTomados = await db.SolicitudesVacaciones
                .Where(s =>
                    s.EmpleadoId == empleadoId &&
                    s.Estado != EstadoSolicitud.Rechazado &&
                    !s.EsVacacionAnticipada)
                .SumAsync(s => (decimal?)s.DiasSolicitados) ?? 0m;

            decimal diasVencidos = await ObtenerDiasVencidosAsync(empleadoId);

            return Math.Max(acumuladas - diasTomados - diasVencidos, 0m);
            //return Math.Max(acumuladas - diasTomados, 0m);
        }

        private async Task AplicarDescuentoVacacionesAnticipadasAsync(int empleadoId)
        {
            var empleado = await db.Empleados.FirstOrDefaultAsync(e => e.Id == empleadoId);

            if (empleado == null)
                return;

            var fechaHoy = DateTime.Today;

            if (fechaHoy < empleado.FechaIngreso.Date.AddYears(1))
                return;

            var solicitudesAnticipadasPendientes = await db.SolicitudesVacaciones
                .Where(s =>
                    s.EmpleadoId == empleadoId &&
                    s.EsVacacionAnticipada &&
                    s.Estado == EstadoSolicitud.Aprobado &&
                    !s.DescuentoAnticipadoAplicado &&
                    s.DiasAnticipadosPendientesDescuento > 0)
                .ToListAsync();

            if (!solicitudesAnticipadasPendientes.Any())
                return;

            foreach (var solicitud in solicitudesAnticipadasPendientes)
            {
                var historial = new HistorialVacaciones
                {
                    EmpleadoId = solicitud.EmpleadoId,
                    FechaInicio = solicitud.FechaInicio,
                    FechaFin = solicitud.FechaFin,
                    DiasTomados = Convert.ToInt32(solicitud.DiasAnticipadosPendientesDescuento),
                    Observaciones = $"Descuento automático por vacaciones anticipadas aprobadas. Solicitud #{solicitud.Id}",
                    SolicitudVacacionesId = solicitud.Id,
                    AutorizadorId = solicitud.AutorizadorId ?? 0
                };

                db.HistorialesVacaciones.Add(historial);

                solicitud.DescuentoAnticipadoAplicado = true;
                solicitud.FechaAplicacionDescuentoAnticipado = fechaHoy;
                solicitud.DiasAnticipadosPendientesDescuento = 0m;
            }

            await db.SaveChangesAsync();
        }

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
                var fechaHoy = DateTime.Today;

                if (empleado.JefeId == null || empleado.JefeId == 0)
                {
                    resp.TieneError = true;
                    resp.Mensaje = "No se pudo identificar al jefe directo del empleado.";
                    return new JsonResult(resp);
                }

                if (InputSolicitud.FechaFin < InputSolicitud.FechaInicio)
                {
                    resp.TieneError = true;
                    resp.Mensaje = "La fecha fin no puede ser menor que la fecha inicio.";
                    return new JsonResult(resp);
                }

                var diasSolicitados = Enumerable
                    .Range(0, (InputSolicitud.FechaFin - InputSolicitud.FechaInicio).Days + 1)
                    .Select(offset => InputSolicitud.FechaInicio.AddDays(offset))
                    .Count(date => date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday);

                if (diasSolicitados <= 0)
                {
                    resp.TieneError = true;
                    resp.Mensaje = "Debes seleccionar al menos un día hábil.";
                    return new JsonResult(resp);
                }

                bool cumpleAnio = fechaHoy >= empleado.FechaIngreso.Date.AddYears(1);

                if (InputSolicitud.EsVacacionAnticipada)
                {
                    if (cumpleAnio)
                    {
                        resp.TieneError = true;
                        resp.Mensaje = "Las vacaciones anticipadas solo aplican para colaboradores que aún no cumplen un año.";
                        return new JsonResult(resp);
                    }

                    if (diasSolicitados > 12)
                    {
                        resp.TieneError = true;
                        resp.Mensaje = "No puedes solicitar más de 12 días de vacaciones anticipadas.";
                        return new JsonResult(resp);
                    }

                    var diasAnticipadosPendientes = await ObtenerDiasAnticipadosPendientesAsync(empleado.Id);

                    if ((diasAnticipadosPendientes + diasSolicitados) > 12)
                    {
                        resp.TieneError = true;
                        resp.Mensaje = $"Ya tienes {diasAnticipadosPendientes:0.##} día(s) de vacaciones anticipadas pendientes. El máximo acumulado es 12.";
                        return new JsonResult(resp);
                    }
                }
                else
                {
                    // Aquí se valida el flujo normal de vacaciones
                    decimal diasDisponibles = await OnGetObtenerDiasDisponiblesInternoAsync(empleado.Id);

                    if (diasSolicitados > diasDisponibles)
                    {
                        resp.TieneError = true;
                        resp.Mensaje = $"No cuentas con saldo suficiente. Saldo disponible: {diasDisponibles:0.##} día(s).";
                        return new JsonResult(resp);
                    }
                }

                var solicitud = new SolicitudVacaciones
                {
                    EmpleadoId = empleado.Id,
                    Empleado = empleado,
                    FechaSolicitud = fechaActual,
                    FechaInicio = InputSolicitud.FechaInicio,
                    FechaFin = InputSolicitud.FechaFin,
                    DiasSolicitados = diasSolicitados,
                    ComentarioEmpleado = InputSolicitud.ComentarioEmpleado,

                    JefeDirectoEmpleadoId = empleado.JefeId,
                    AutorizadorId = empleado.JefeId,
                    Estado = EstadoSolicitud.Pendiente,

                    EstadoJefeDirecto = "Pendiente",
                    EstadoTH = "Pendiente",

                    EsVacacionAnticipada = InputSolicitud.EsVacacionAnticipada,
                    DiasAnticipadosPendientesDescuento = InputSolicitud.EsVacacionAnticipada ? diasSolicitados : 0m,
                    FechaAplicacionDescuentoAnticipado = null,
                    DescuentoAnticipadoAplicado = false
                };

                await solicitudVacacionesManager.CreateAsync(solicitud);

                await EnviarCorreoSolicitudVacacionesAJefeAsync(solicitud.Id);

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

        private async Task EnviarCorreoSolicitudVacacionesAJefeAsync(int solicitudId)
        {
            var solicitud = await db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                    .ThenInclude(e => e.Usuario)
                .Include(s => s.Autorizador)
                    .ThenInclude(a => a.Usuario)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null)
                return;

            if (solicitud.Autorizador?.Usuario == null || string.IsNullOrWhiteSpace(solicitud.Autorizador.Usuario.Email))
                return;

            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";
            string urlAprobar = $"{baseUrl}/ERP/Vacaciones?solicitudId={solicitud.Id}&accionCorreo=aprobarJefe";
            string urlRechazar = $"{baseUrl}/ERP/Vacaciones?solicitudId={solicitud.Id}&accionCorreo=rechazarJefe";

            string subject = $"Solicitud de vacaciones pendiente - {solicitud.Empleado?.NombreCompleto}";

            string body = $@"
        <p>Estimado(a) {solicitud.Autorizador?.NombreCompleto},</p>

        <p>Se generó una nueva solicitud de vacaciones en la intranet.</p>

        <table border='1' cellpadding='8' cellspacing='0' style='border-collapse: collapse; font-family: Arial, sans-serif; font-size: 14px;'>
            <tr>
                <th style='background-color:#f2f2f2;'>Empleado</th>
                <td>{solicitud.Empleado?.NombreCompleto}</td>
            </tr>
            <tr>
                <th style='background-color:#f2f2f2;'>Fecha solicitud</th>
                <td>{solicitud.FechaSolicitud:dd/MM/yyyy}</td>
            </tr>
            <tr>
                <th style='background-color:#f2f2f2;'>Fecha inicio</th>
                <td>{solicitud.FechaInicio:dd/MM/yyyy}</td>
            </tr>
            <tr>
                <th style='background-color:#f2f2f2;'>Fecha fin</th>
                <td>{solicitud.FechaFin:dd/MM/yyyy}</td>
            </tr>
            <tr>
                <th style='background-color:#f2f2f2;'>Días solicitados</th>
                <td>{solicitud.DiasSolicitados}</td>
            </tr>
            <tr>
                <th style='background-color:#f2f2f2;'>Comentario</th>
                <td>{(string.IsNullOrWhiteSpace(solicitud.ComentarioEmpleado) ? "Sin comentario" : solicitud.ComentarioEmpleado)}</td>
            </tr>
        </table>

        <br>

        <div style='margin-top:20px; display:flex; gap:20px; flex-wrap:wrap;'>
            <a href='{urlAprobar}' style='padding:10px 20px; background-color:#28a745; color:white; text-decoration:none; border-radius:5px; font-weight:bold;'>
                ✅ Revisar para aprobar
            </a>

            <a href='{urlRechazar}' style='padding:10px 20px; background-color:#dc3545; color:white; text-decoration:none; border-radius:5px; font-weight:bold;'>
                ❌ Revisar para rechazar
            </a>
        </div>

        <br>
        <p style='color:gray;'>Este es un mensaje automático de la Intranet SEI Consulting Group.</p>";

            await _emailSender.SendEmailAsync(
                solicitud.Autorizador.Usuario.Email,
                subject,
                body
            );
        }

        private async Task EnviarCorreoSeguimientoVacacionesAsync(int solicitudId, string etapa, string accion)
        {
            var solicitud = await db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                    .ThenInclude(e => e.Usuario)
                .Include(s => s.Autorizador)
                    .ThenInclude(a => a.Usuario)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null)
                return;

            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";
            string urlModulo = $"{baseUrl}/ERP/Vacaciones";

            string estadoVisual = ObtenerEstadoVisualVacaciones(solicitud);

            // correo al colaborador
            if (solicitud.Empleado?.Usuario != null && !string.IsNullOrWhiteSpace(solicitud.Empleado.Usuario.Email))
            {
                string subjectEmpleado = etapa == "JefeDirecto"
                    ? $"Tu solicitud de vacaciones fue {(accion == "Aprobado" ? "aprobada" : "rechazada")} por tu jefe directo"
                    : $"Tu solicitud de vacaciones fue {(accion == "Aprobado" ? "aprobada" : "rechazada")} por Talento Humano";

                string bodyEmpleado = $@"
            <p>Hola {solicitud.Empleado?.NombreCompleto},</p>

            <p>Tu solicitud de vacaciones fue <strong>{accion}</strong> en la etapa de <strong>{(etapa == "JefeDirecto" ? "Jefe Directo" : "Talento Humano")}</strong>.</p>

            <table border='1' cellpadding='8' cellspacing='0' style='border-collapse: collapse; font-family: Arial, sans-serif; font-size: 14px;'>
                <tr>
                    <th style='background-color:#f2f2f2;'>Fecha inicio</th>
                    <td>{solicitud.FechaInicio:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <th style='background-color:#f2f2f2;'>Fecha fin</th>
                    <td>{solicitud.FechaFin:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <th style='background-color:#f2f2f2;'>Días solicitados</th>
                    <td>{solicitud.DiasSolicitados}</td>
                </tr>
                <tr>
                    <th style='background-color:#f2f2f2;'>Estado actual</th>
                    <td>{estadoVisual}</td>
                </tr>
                <tr>
                    <th style='background-color:#f2f2f2;'>Comentario</th>
                    <td>{(string.IsNullOrWhiteSpace(solicitud.ComentarioEmpleado) ? "Sin comentario" : solicitud.ComentarioEmpleado)}</td>
                </tr>
            </table>

            <br>
            <a href='{urlModulo}' style='padding:10px 20px; background-color:#1f4cd3; color:white; text-decoration:none; border-radius:5px; font-weight:bold;'>
                Ver en intranet
            </a>

            <br><br>
            <p style='color:gray;'>Este es un mensaje automático de la Intranet SEI Consulting Group.</p>";

                await _emailSender.SendEmailAsync(
                    solicitud.Empleado.Usuario.Email,
                    subjectEmpleado,
                    bodyEmpleado
                );
            }

            // si fue TH, también avisar al jefe/autorizador
            if (etapa == "TH" && solicitud.Autorizador?.Usuario != null && !string.IsNullOrWhiteSpace(solicitud.Autorizador.Usuario.Email))
            {
                string subjectJefe = $"Talento Humano {(accion == "Aprobado" ? "aprobó" : "rechazó")} la solicitud de {solicitud.Empleado?.NombreCompleto}";

                string bodyJefe = $@"
            <p>Hola {solicitud.Autorizador?.NombreCompleto},</p>

            <p>Talento Humano ha <strong>{(accion == "Aprobado" ? "aprobado" : "rechazado")}</strong> la solicitud de vacaciones del colaborador <strong>{solicitud.Empleado?.NombreCompleto}</strong>.</p>

            <table border='1' cellpadding='8' cellspacing='0' style='border-collapse: collapse; font-family: Arial, sans-serif; font-size: 14px;'>
                <tr>
                    <th style='background-color:#f2f2f2;'>Fecha inicio</th>
                    <td>{solicitud.FechaInicio:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <th style='background-color:#f2f2f2;'>Fecha fin</th>
                    <td>{solicitud.FechaFin:dd/MM/yyyy}</td>
                </tr>
                <tr>
                    <th style='background-color:#f2f2f2;'>Días solicitados</th>
                    <td>{solicitud.DiasSolicitados}</td>
                </tr>
                <tr>
                    <th style='background-color:#f2f2f2;'>Estado actual</th>
                    <td>{estadoVisual}</td>
                </tr>
            </table>

            <br>
            <a href='{urlModulo}' style='padding:10px 20px; background-color:#1f4cd3; color:white; text-decoration:none; border-radius:5px; font-weight:bold;'>
                Ver en intranet
            </a>

            <br><br>
            <p style='color:gray;'>Este es un mensaje automático de la Intranet SEI Consulting Group.</p>";

                await _emailSender.SendEmailAsync(
                    solicitud.Autorizador.Usuario.Email,
                    subjectJefe,
                    bodyJefe
                );
            }
        }

        public async Task<JsonResult> OnGetResumenVacaciones()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario == null || usuario.Empleado == null)
                return new JsonResult(new { error = "Empleado no encontrado." });

            var empleado = usuario.Empleado;

            await AplicarDescuentoVacacionesAnticipadasAsync(empleado.Id);

            var fechaHoy = DateTime.Now.Date;
            string tipoAsignacion = await ObtenerTipoVisualizacionVacacionesAsync();

            decimal diasLegales = 0m;
            decimal diasProporcionales = 0m;

            int aniosCumplidos = fechaHoy.Year - empleado.FechaIngreso.Date.Year;

            if (fechaHoy < empleado.FechaIngreso.Date.AddYears(aniosCumplidos))
                aniosCumplidos--;

            decimal diasPorAnio = ObtenerDiasVacacionesPorAntiguedad(aniosCumplidos);
            decimal diasProximoAnio = ObtenerDiasVacacionesPorAntiguedad(aniosCumplidos + 1);

            if (aniosCumplidos >= 1)
            {
                var ultimoAniversario = empleado.FechaIngreso.Date.AddYears(aniosCumplidos);

                diasLegales = diasPorAnio;
                diasProporcionales = Math.Round(
                    (diasProximoAnio / 365m) * (decimal)(fechaHoy - ultimoAniversario).TotalDays, 1);
            }
            else
            {
                diasLegales = 0m;
                diasProporcionales = Math.Round(
                    (12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date).TotalDays, 1);
            }

            /*if (fechaHoy >= empleado.FechaIngreso.Date.AddYears(1))
            {
                diasLegales = 12m;
                diasProporcionales = Math.Round(
                    (12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date.AddYears(1)).TotalDays, 1);
            }
            else
            {
                diasLegales = 0m;
                diasProporcionales = Math.Round(
                    (12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date).TotalDays, 1);
            }*/

            decimal acumuladas = tipoAsignacion == "Legales"
                ? diasLegales
                : diasLegales + diasProporcionales;

            // Tomadas reales:
            // - normales aprobadas
            // - anticipadas ya aplicadas al cumplir el año
            var diasTomados = await db.SolicitudesVacaciones
                .Where(s =>
                    s.EmpleadoId == empleado.Id &&
                    (
                        (!s.EsVacacionAnticipada && s.Estado == EstadoSolicitud.Aprobado)
                        ||
                        (s.EsVacacionAnticipada && s.DescuentoAnticipadoAplicado)
                    ))
                .SumAsync(s => (decimal?)s.DiasSolicitados) ?? 0m;

            // Futuras visuales:
            // anticipadas solicitadas o aprobadas, mientras no estén rechazadas ni aplicadas
            var diasFuturasVisuales = await db.SolicitudesVacaciones
                .Where(s =>
                    s.EmpleadoId == empleado.Id &&
                    s.EsVacacionAnticipada &&
                    s.Estado != EstadoSolicitud.Rechazado &&
                    !s.DescuentoAnticipadoAplicado)
                .SumAsync(s => (decimal?)s.DiasSolicitados) ?? 0m;

            // Futuras que sí descuentan saldo:
            // solo las aprobadas y aún no aplicadas
            var diasFuturasDescontables = await db.SolicitudesVacaciones
                .Where(s =>
                    s.EmpleadoId == empleado.Id &&
                    s.EsVacacionAnticipada &&
                    s.Estado == EstadoSolicitud.Aprobado &&
                    !s.DescuentoAnticipadoAplicado)
                .SumAsync(s => (decimal?)s.DiasSolicitados) ?? 0m;

            //decimal saldo = Math.Max(acumuladas - diasTomados - diasFuturasDescontables, 0m);
            decimal diasVencidos = await ObtenerDiasVencidosAsync(empleado.Id);
            decimal saldo = Math.Max(acumuladas - diasTomados - diasVencidos, 0m);

            return new JsonResult(new
            {
                Acumuladas = acumuladas,
                Tomadas = diasTomados,
                Vencidas = 0,
                Futuras = diasFuturasVisuales,
                vencidas = diasVencidos,
                saldo = saldo,
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

        public async Task<JsonResult> OnGetAvisoVacacionesPorVencerAsync()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario?.Empleado == null)
                return new JsonResult(new { mostrar = false });

            var empleado = usuario.Empleado;
            var fechaHoy = DateTime.Today;
            var fechaIngreso = empleado.FechaIngreso.Date;

            int aniosCumplidos = fechaHoy.Year - fechaIngreso.Year;

            if (fechaHoy < fechaIngreso.AddYears(aniosCumplidos))
                aniosCumplidos--;

            for (int anio = 1; anio <= aniosCumplidos; anio++)
            {
                var fechaGeneracion = fechaIngreso.AddYears(anio);
                var fechaVencimiento = fechaGeneracion.AddYears(2);

                var diasParaVencer = (fechaVencimiento - fechaHoy).TotalDays;

                if (diasParaVencer < 0 || diasParaVencer > 90)
                    continue;

                decimal diasDelPeriodo = ObtenerDiasVacacionesPorAntiguedad(anio);

                var diasTomados = await db.SolicitudesVacaciones
                    .Where(s =>
                        s.EmpleadoId == empleado.Id &&
                        s.EstadoJefeDirecto == "Aprobado" &&
                        s.EstadoTH == "Aprobado" &&
                        !s.EsVacacionAnticipada &&
                        s.FechaInicio >= fechaGeneracion &&
                        s.FechaInicio <= fechaVencimiento)
                    .SumAsync(s => (decimal?)s.DiasSolicitados) ?? 0m;

                decimal diasPendientes = Math.Max(diasDelPeriodo - diasTomados, 0m);

                if (diasPendientes > 0)
                {
                    return new JsonResult(new
                    {
                        mostrar = true,
                        dias = diasPendientes,
                        fechaVencimiento = fechaVencimiento.ToString("dd/MM/yyyy")
                    });
                }
            }

            return new JsonResult(new { mostrar = false });
        }

        public async Task<JsonResult> OnGetObtenerDiasDisponibles()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario == null || usuario.Empleado == null)
                return new JsonResult(0);

            var empleado = usuario.Empleado;

            // 🔥 Aplica descuentos automáticos si ya cumplió el año
            await AplicarDescuentoVacacionesAnticipadasAsync(empleado.Id);

            var fechaHoy = DateTime.Today;
            string tipoAsignacion = await ObtenerTipoVisualizacionVacacionesAsync();

            decimal diasLegales = 0m;
            decimal diasProporcionales = 0m;

            if (fechaHoy >= empleado.FechaIngreso.Date.AddYears(1))
            {
                diasLegales = 12m;
                diasProporcionales = Math.Round(
                    (12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date.AddYears(1)).TotalDays, 1);
            }
            else
            {
                diasLegales = 0m;
                diasProporcionales = Math.Round(
                    (12m / 365m) * (decimal)(fechaHoy - empleado.FechaIngreso.Date).TotalDays, 1);
            }

            decimal acumuladas = tipoAsignacion == "Legales"
                ? diasLegales
                : diasLegales + diasProporcionales;

            var diasTomados = await db.SolicitudesVacaciones
                .Where(s =>
                    s.EmpleadoId == empleado.Id &&
                    s.Estado == EstadoSolicitud.Aprobado &&
                    (
                        !s.EsVacacionAnticipada ||
                        (s.EsVacacionAnticipada && s.DescuentoAnticipadoAplicado)
                    ))
                .SumAsync(s => (decimal?)s.DiasSolicitados) ?? 0m;

            var diasFuturas = await db.SolicitudesVacaciones
                .Where(s =>
                    s.EmpleadoId == empleado.Id &&
                    s.EsVacacionAnticipada &&
                    s.Estado == EstadoSolicitud.Aprobado &&
                    !s.DescuentoAnticipadoAplicado)
                .SumAsync(s => (decimal?)s.DiasSolicitados) ?? 0m;

            decimal saldo = Math.Max(acumuladas - diasTomados - diasFuturas, 0m);

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

            int aniosCumplidos = fechaActual.Year - fechaIngreso.Year;

            if (fechaActual < fechaIngreso.AddYears(aniosCumplidos))
                aniosCumplidos--;

            decimal diasLegales = ObtenerDiasVacacionesPorAntiguedad(aniosCumplidos);
            decimal diasProximoAnio = ObtenerDiasVacacionesPorAntiguedad(aniosCumplidos + 1);

            //const decimal diasLegales = 12m;
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
                    var diasProporcionales = Math.Round((diasProximoAnio / 365m) * (decimal)(fechaActual - fechaAniversario).TotalDays, 1);
                    //var diasProporcionales = Math.Round((diasLegales / 365) * (decimal)(fechaActual - fechaAniversario).TotalDays, 1);

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

            await AplicarDescuentoVacacionesAnticipadasAsync(empleado.Id);

            var solicitudes = await db.SolicitudesVacaciones
                .Where(s => s.EmpleadoId == empleado.Id)
                .OrderByDescending(s => s.FechaInicio)
                .ToListAsync();

            var lista = solicitudes.Select(s => new
            {
                inicio = s.FechaInicio.ToString("dd/MM/yyyy"),
                fin = s.FechaFin.ToString("dd/MM/yyyy"),
                dias = s.DiasSolicitados,
                tipo = s.EsVacacionAnticipada
                    ? (s.DescuentoAnticipadoAplicado ? "Anticipadas (descontadas)" : "Anticipadas")
                    : "Legales",
                estado = ObtenerEstadoVisualVacaciones(s)
            }).ToList();

            return new JsonResult(lista);
        }

        public async Task<JsonResult> OnPostAprobarJefeDirectoAsync(int idSolicitud)
        {
            try
            {
                await ConfigurarPermisosVacacionesAsync();

                if (!PuedeAprobarJefeDirecto)
                    return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para aprobar como jefe directo." });

                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario?.Empleado == null)
                    return new JsonResult(new { tieneError = true, mensaje = "No se encontró el empleado del usuario actual." });

                var solicitud = await db.SolicitudesVacaciones
                    .Include(s => s.Empleado)
                    .FirstOrDefaultAsync(s => s.Id == idSolicitud);

                if (solicitud == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Solicitud no encontrada." });

                var roles = await userManager.GetRolesAsync(usuario);
                bool esAdministrador = roles.Contains("Administrador") || roles.Contains("Master");

                if (!esAdministrador)
                {
                    if (!solicitud.JefeDirectoEmpleadoId.HasValue || solicitud.JefeDirectoEmpleadoId.Value != usuario.Empleado.Id)
                    {
                        return new JsonResult(new
                        {
                            tieneError = true,
                            mensaje = "No eres el jefe directo asignado a esta solicitud."
                        });
                    }
                }

                if (solicitud.EstadoJefeDirecto != "Pendiente")
                {
                    return new JsonResult(new
                    {
                        tieneError = true,
                        mensaje = "La solicitud ya fue revisada por jefe directo."
                    });
                }

                solicitud.EstadoJefeDirecto = "Aprobado";
                solicitud.UsuarioJefeDirectoId = usuario.Id;
                solicitud.FechaRevisionJefeDirecto = DateTime.Now;

                // Sigue pendiente para TH
                solicitud.Estado = EstadoSolicitud.Pendiente;

                db.SolicitudesVacaciones.Update(solicitud);
                await db.SaveChangesAsync();
                await EnviarCorreoSeguimientoVacacionesAsync(solicitud.Id, "JefeDirecto", "Aprobado");

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "La solicitud fue aprobada por jefe directo."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al aprobar solicitud de vacaciones por jefe directo");
                return new JsonResult(new
                {
                    tieneError = true,
                    mensaje = "Ocurrió un error al aprobar la solicitud."
                });
            }
        }

        public async Task<JsonResult> OnPostRechazarJefeDirectoAsync(int idSolicitud)
        {
            try
            {
                await ConfigurarPermisosVacacionesAsync();

                if (!PuedeAprobarJefeDirecto)
                    return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para rechazar como jefe directo." });

                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario?.Empleado == null)
                    return new JsonResult(new { tieneError = true, mensaje = "No se encontró el empleado del usuario actual." });

                var solicitud = await db.SolicitudesVacaciones
                    .Include(s => s.Empleado)
                    .FirstOrDefaultAsync(s => s.Id == idSolicitud);

                if (solicitud == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Solicitud no encontrada." });

                var roles = await userManager.GetRolesAsync(usuario);
                bool esAdministrador = roles.Contains("Administrador") || roles.Contains("Master");

                if (!esAdministrador)
                {
                    if (!solicitud.JefeDirectoEmpleadoId.HasValue || solicitud.JefeDirectoEmpleadoId.Value != usuario.Empleado.Id)
                    {
                        return new JsonResult(new
                        {
                            tieneError = true,
                            mensaje = "No eres el jefe directo asignado a esta solicitud."
                        });
                    }
                }

                if (solicitud.EstadoJefeDirecto != "Pendiente")
                {
                    return new JsonResult(new
                    {
                        tieneError = true,
                        mensaje = "La solicitud ya fue revisada por jefe directo."
                    });
                }

                solicitud.EstadoJefeDirecto = "Rechazado";
                solicitud.UsuarioJefeDirectoId = usuario.Id;
                solicitud.FechaRevisionJefeDirecto = DateTime.Now;
                solicitud.Estado = EstadoSolicitud.Rechazado;

                db.SolicitudesVacaciones.Update(solicitud);
                await db.SaveChangesAsync();
                await EnviarCorreoSeguimientoVacacionesAsync(solicitud.Id, "JefeDirecto", "Rechazado");

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "La solicitud fue rechazada por jefe directo."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al rechazar solicitud de vacaciones por jefe directo");
                return new JsonResult(new
                {
                    tieneError = true,
                    mensaje = "Ocurrió un error al rechazar la solicitud."
                });
            }
        }

        public async Task<JsonResult> OnPostAprobarTHAsync(int idSolicitud)
        {
            try
            {
                await ConfigurarPermisosVacacionesAsync();

                if (!PuedeAprobarTH)
                    return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para aprobar como TH." });

                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Usuario no encontrado." });

                var solicitud = await db.SolicitudesVacaciones
                    .Include(s => s.Empleado)
                    .FirstOrDefaultAsync(s => s.Id == idSolicitud);

                if (solicitud == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Solicitud no encontrada." });

                if (solicitud.EstadoJefeDirecto != "Aprobado")
                {
                    return new JsonResult(new
                    {
                        tieneError = true,
                        mensaje = "El jefe directo aún no ha aprobado esta solicitud."
                    });
                }

                if (solicitud.EstadoTH != "Pendiente")
                {
                    return new JsonResult(new
                    {
                        tieneError = true,
                        mensaje = "La solicitud ya fue revisada por TH."
                    });
                }

                solicitud.EstadoTH = "Aprobado";
                solicitud.UsuarioTHId = usuario.Id;
                solicitud.FechaRevisionTH = DateTime.Now;
                solicitud.Estado = EstadoSolicitud.Aprobado;

                db.SolicitudesVacaciones.Update(solicitud);

                var historialExiste = await db.HistorialesVacaciones
                    .AnyAsync(h => h.SolicitudVacacionesId == solicitud.Id);

                if (!historialExiste)
                {
                    var autorizadorId = usuario.Empleado?.Id ?? solicitud.AutorizadorId ?? 0;

                    var historial = new HistorialVacaciones
                    {
                        EmpleadoId = solicitud.EmpleadoId,
                        FechaInicio = solicitud.FechaInicio,
                        FechaFin = solicitud.FechaFin,
                        DiasTomados = solicitud.DiasSolicitados,
                        Observaciones = solicitud.ComentarioEmpleado ?? "",
                        SolicitudVacacionesId = solicitud.Id,
                        AutorizadorId = autorizadorId
                    };

                    db.HistorialesVacaciones.Add(historial);
                }

                await db.SaveChangesAsync();
                await EnviarCorreoSeguimientoVacacionesAsync(solicitud.Id, "TH", "Aprobado");

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "La solicitud fue aprobada por TH."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al aprobar solicitud de vacaciones por TH");
                return new JsonResult(new
                {
                    tieneError = true,
                    mensaje = "Ocurrió un error al aprobar la solicitud."
                });
            }
        }

        private async Task<decimal> ObtenerDiasAnticipadosPendientesAsync(int empleadoId)
        {
            return await db.SolicitudesVacaciones
                .Where(s =>
                    s.EmpleadoId == empleadoId &&
                    s.EsVacacionAnticipada &&
                    s.Estado != EstadoSolicitud.Rechazado &&
                    !s.DescuentoAnticipadoAplicado)
                .SumAsync(s => (decimal?)s.DiasAnticipadosPendientesDescuento) ?? 0m;
        }

        public async Task<JsonResult> OnPostRechazarTHAsync(int idSolicitud)
        {
            try
            {
                await ConfigurarPermisosVacacionesAsync();

                if (!PuedeAprobarTH)
                    return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para rechazar como TH." });

                var userEmail = User.Identity?.Name;
                var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

                if (usuario == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Usuario no encontrado." });

                var solicitud = await db.SolicitudesVacaciones
                    .Include(s => s.Empleado)
                    .FirstOrDefaultAsync(s => s.Id == idSolicitud);

                if (solicitud == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Solicitud no encontrada." });

                if (solicitud.EstadoJefeDirecto != "Aprobado")
                {
                    return new JsonResult(new
                    {
                        tieneError = true,
                        mensaje = "El jefe directo aún no ha aprobado esta solicitud."
                    });
                }

                if (solicitud.EstadoTH != "Pendiente")
                {
                    return new JsonResult(new
                    {
                        tieneError = true,
                        mensaje = "La solicitud ya fue revisada por TH."
                    });
                }

                solicitud.EstadoTH = "Rechazado";
                solicitud.UsuarioTHId = usuario.Id;
                solicitud.FechaRevisionTH = DateTime.Now;
                solicitud.Estado = EstadoSolicitud.Rechazado;

                db.SolicitudesVacaciones.Update(solicitud);
                await db.SaveChangesAsync();
                await EnviarCorreoSeguimientoVacacionesAsync(solicitud.Id, "TH", "Rechazado");

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "La solicitud fue rechazada por TH."
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al rechazar solicitud de vacaciones por TH");
                return new JsonResult(new
                {
                    tieneError = true,
                    mensaje = "Ocurrió un error al rechazar la solicitud."
                });
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
        }*/

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

        public async Task<IActionResult> OnGetExportarDetalleVacacionesAsync()
        {
            await ConfigurarPermisosVacacionesAsync();

            if (!PuedeExportarDetalleVacaciones)
                return Forbid();

            ExcelPackage.License.SetNonCommercialOrganization("SEI Consulting Group");

            var solicitudes = await db.SolicitudesVacaciones
                .Include(s => s.Empleado)
                .Include(s => s.Autorizador)
                .OrderBy(s => s.Empleado != null ? s.Empleado.NombreCompleto : "")
                .ThenByDescending(s => s.FechaSolicitud)
                .ToListAsync();

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Detalle Vacaciones");

            // Título
            ws.Cells["A1"].Value = "Detalle de Vacaciones";
            ws.Cells["A1:L1"].Merge = true;
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A1"].Style.Font.Size = 16;
            ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Fecha de generación
            ws.Cells["A2"].Value = "Generado:";
            ws.Cells["B2"].Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            // Encabezados
            int row = 4;
            ws.Cells[row, 1].Value = "Id";
            ws.Cells[row, 2].Value = "Empleado";
            ws.Cells[row, 3].Value = "Fecha Solicitud";
            ws.Cells[row, 4].Value = "Fecha Inicio";
            ws.Cells[row, 5].Value = "Fecha Fin";
            ws.Cells[row, 6].Value = "Días Solicitados";
            ws.Cells[row, 7].Value = "Comentario Empleado";
            ws.Cells[row, 8].Value = "Estado Visual";
            ws.Cells[row, 9].Value = "Estado Jefe Directo";
            ws.Cells[row, 10].Value = "Estado TH";
            ws.Cells[row, 11].Value = "Autorizador";
            ws.Cells[row, 12].Value = "Jefe Directo Id";

            using (var range = ws.Cells[row, 1, row, 12])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(31, 76, 211));
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            row++;

            string empleadoActual = string.Empty;
            int inicioGrupo = row;

            foreach (var s in solicitudes)
            {
                string nombreEmpleado = s.Empleado?.NombreCompleto ?? "-";

                if (!string.IsNullOrWhiteSpace(empleadoActual) && empleadoActual != nombreEmpleado)
                {
                    // Línea separadora visual entre empleados
                    using (var separator = ws.Cells[row, 1, row, 12])
                    {
                        separator.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        separator.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
                    }
                    row++;
                }

                empleadoActual = nombreEmpleado;

                ws.Cells[row, 1].Value = s.Id;
                ws.Cells[row, 2].Value = nombreEmpleado;
                ws.Cells[row, 3].Value = s.FechaSolicitud.ToString("dd/MM/yyyy");
                ws.Cells[row, 4].Value = s.FechaInicio.ToString("dd/MM/yyyy");
                ws.Cells[row, 5].Value = s.FechaFin.ToString("dd/MM/yyyy");
                ws.Cells[row, 6].Value = s.DiasSolicitados;
                ws.Cells[row, 7].Value = s.ComentarioEmpleado ?? "";
                ws.Cells[row, 8].Value = ObtenerEstadoVisualVacaciones(s);
                ws.Cells[row, 9].Value = s.EstadoJefeDirecto ?? "";
                ws.Cells[row, 10].Value = s.EstadoTH ?? "";
                ws.Cells[row, 11].Value = s.Autorizador?.NombreCompleto ?? "-";
                ws.Cells[row, 12].Value = s.JefeDirectoEmpleadoId;

                for (int col = 1; col <= 12; col++)
                {
                    ws.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    ws.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                row++;
            }

            ws.Cells[ws.Dimension.Address].AutoFitColumns();

            var bytes = package.GetAsByteArray();
            var fileName = $"DetalleVacaciones_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName
            );
        }
    }
}