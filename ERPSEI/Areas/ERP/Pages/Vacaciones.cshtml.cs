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
using ERPSEI.Data.Migrations;
using ERPSEI.Data.Managers.ActivosFijos;
using System.Security.Claims;

namespace ERPSEI.Areas.ERP.Pages
{
    public class VacacionesModel : ERPPageModel
    {
        private readonly IStringLocalizer<VacacionesModel> stringLocalizer;
        private readonly ILogger<VacacionesModel> logger;
        private readonly AppUserManager appUserManager;
        private readonly AppUserManager userManager;
        private readonly IStringLocalizer<VacacionesModel> localizer;
        private readonly ISolicitudVacacionesManager solicitudVacacionesManager;

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
                ISolicitudVacacionesManager _solicitudVacacionesManager

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

            InputFiltro = new InputFiltroVacacionesModel();
            InputVacaciones = new VacacionesTableModel();
            SolicitudVacacionesList = new SolicitudVacaciones();
        }

        //Método para mostrar las solicitudes enviadas de vacaciones
        public async Task<JsonResult> OnGetVacacionesList()
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
        }

        public async Task<JsonResult> OnPostFiltrarVacaciones()
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
        }

        public async Task<JsonResult> OnPostGuardarSolicitud()
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
        }

        public async Task<JsonResult> OnGetResumenVacaciones()
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
        }


        public async Task<JsonResult> OnGetObtenerDiasDisponibles()
        {
            var userEmail = User.Identity?.Name;
            var usuario = await userManager.FindByNameWithEmpleadoAsync(userEmail);

            if (usuario == null || usuario.Empleado == null)
                return new JsonResult(0); // o JsonResult(new { error = "Empleado no encontrado" });

            decimal dias = await solicitudVacacionesManager.CalcularDiasDisponiblesAsync(usuario.Empleado.Id);
            return new JsonResult(dias);
        }

        public async Task<JsonResult> OnGetVacacionesAcumuladas()
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
        }

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
