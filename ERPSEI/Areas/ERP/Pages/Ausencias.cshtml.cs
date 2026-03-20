using ERPSEI.Data;
using ERPSEI.Data.Entities.RH;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.ERP.Pages
{
    [Authorize]
    public class AusenciasModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly AppUserManager _userManager;

        public AusenciasModel(ApplicationDbContext db, AppUserManager userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public List<SelectListItem> TiposAusencia { get; set; } = new();
        public List<SelectListItem> TiposIncapacidad { get; set; } = new();

        public bool PuedeAprobarJefeDirecto { get; set; }
        public bool PuedeAprobarTH { get; set; }
        public bool PuedeVerTodasAusencias { get; set; }

        [BindProperty] public GuardarInasistenciaInput InasistenciaInput { get; set; } = new();
        [BindProperty] public GuardarIncapacidadInput IncapacidadInput { get; set; } = new();
        [BindProperty] public GuardarPermisoInput PermisoInput { get; set; } = new();
        [BindProperty] public SolicitarPermisoInput SolicitudPermisoInput { get; set; } = new();
        [BindProperty] public EditarAusenciaInput EditarInput { get; set; } = new();

        public class GuardarInasistenciaInput
        {
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public decimal? Dias { get; set; }
            public DateTime? FechaAplicacion { get; set; }
            public bool Suplencia { get; set; }
            public string? Comentario { get; set; }
        }

        public class GuardarIncapacidadInput
        {
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public int? TipoIncapacidadId { get; set; }
            public string? NumeroFolio { get; set; }
            public decimal? Dias { get; set; }
            public DateTime? FechaAplicacion { get; set; }
            public bool Suplencia { get; set; }
            public string? Comentario { get; set; }
        }

        public class GuardarPermisoInput
        {
            public int? TipoAusenciaId { get; set; }
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public string? HoraInicio { get; set; }
            public string? HoraTermino { get; set; }
            public decimal? Dias { get; set; }
            public DateTime? FechaAplicacion { get; set; }
            public bool Suplencia { get; set; }
            public string? Comentario { get; set; }
        }

        public class SolicitarPermisoInput
        {
            public int? TipoAusenciaId { get; set; }
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public string? HoraInicio { get; set; }
            public string? HoraTermino { get; set; }
            public DateTime? FechaAplicacion { get; set; }
            public string? Comentario { get; set; }
        }

        public class EditarAusenciaInput
        {
            public int Id { get; set; }
            public int? TipoAusenciaId { get; set; }
            public int? TipoIncapacidadId { get; set; }
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public string? HoraInicio { get; set; }
            public string? HoraTermino { get; set; }
            public decimal? Dias { get; set; }
            public DateTime? FechaAplicacion { get; set; }
            public string? NumeroFolio { get; set; }
            public bool Suplencia { get; set; }
            public string? Comentario { get; set; }
        }

        public async Task OnGetAsync()
        {
            await ConfigurarPermisosAsync();
            await CargarCatalogosAsync();
        }

        private async Task ConfigurarPermisosAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                PuedeAprobarJefeDirecto = false;
                PuedeAprobarTH = false;
                PuedeVerTodasAusencias = false;
                return;
            }

            var roles = await _userManager.GetRolesAsync(user);

            PuedeAprobarJefeDirecto = roles.Contains("JefeDirecto") || roles.Contains("Administrador");
            PuedeAprobarTH = roles.Contains("TH") || roles.Contains("Administrador");
            PuedeVerTodasAusencias = PuedeAprobarJefeDirecto || PuedeAprobarTH || roles.Contains("Administrador");
        }

        private async Task CargarCatalogosAsync()
        {
            TiposAusencia = await _db.TiposAusencias
                .Where(x => x.Activo)
                .OrderBy(x => x.Orden)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Nombre
                })
                .ToListAsync();

            TiposIncapacidad = await _db.TiposIncapacidades
                .Where(x => x.Activo)
                .OrderBy(x => x.Orden)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Nombre
                })
                .ToListAsync();
        }

        private string ObtenerEstadoVisual(Ausencia x)
        {
            if (x.EstadoJefeDirecto == "Rechazado")
                return "Rechazado por jefe directo";

            if (x.EstadoTH == "Rechazado")
                return "Rechazado por TH";

            if (x.EstadoJefeDirecto == "Pendiente")
                return "Pendiente jefe directo";

            if (x.EstadoJefeDirecto == "Aprobado" && x.EstadoTH == "Pendiente")
                return "Aprobado por jefe directo";

            if (x.EstadoJefeDirecto == "Aprobado" && x.EstadoTH == "Aprobado")
                return "Aprobado";

            return "Pendiente";
        }

        public async Task<JsonResult> OnGetTiposAusenciaAsync()
        {
            var data = await _db.TiposAusencias
                .Where(x => x.Activo)
                .OrderBy(x => x.Orden)
                .Select(x => new
                {
                    id = x.Id,
                    nombre = x.Nombre,
                    manejaHoras = x.ManejaHoras,
                    manejaDias = x.ManejaDias
                })
                .ToListAsync();

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnGetAusenciasDiasAsync()
        {
            await ConfigurarPermisosAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.EmpleadoId == null)
                return new JsonResult(new List<object>());

            var query = _db.Ausencias
                .Include(x => x.TipoAusencia)
                .Include(x => x.TipoIncapacidad)
                .Include(x => x.Empleado)
                .Include(x => x.JefeDirectoEmpleado)
                .Where(x => x.TipoCaptura == "Dias")
                .AsQueryable();

            // Administrador ve todo
            if (User.IsInRole("Administrador") || User.IsInRole("Administrador TH"))
            {
                // no filtramos
            }
            // Jefe directo ve solo lo que le toca autorizar
            else if (PuedeAprobarJefeDirecto)
            {
                query = query.Where(x => x.JefeDirectoEmpleadoId == user.EmpleadoId);
            }
            // TH ve lo que ya aprobó jefe directo
            else if (PuedeAprobarTH)
            {
                query = query.Where(x => x.EstadoJefeDirecto == "Aprobado");
            }
            // Empleado normal ve solo lo suyo
            else
            {
                query = query.Where(x => x.EmpleadoId == user.EmpleadoId);
            }

            var lista = await query
                .OrderByDescending(x => x.FechaInicio)
                .ToListAsync();

            var data = lista.Select(x =>
            {
                string estadoVisual = ObtenerEstadoVisual(x);

                return new
                {
                    id = x.Id,
                    tipo = x.Categoria == "Inasistencia"
                        ? "Inasistencia"
                        : x.Categoria == "Incapacidad"
                            ? "Incapacidad - " + (x.TipoIncapacidad != null ? x.TipoIncapacidad.Nombre : "")
                            : (x.TipoAusencia != null ? x.TipoAusencia.Nombre : x.Categoria),

                    fechaInicio = x.FechaInicio.HasValue ? x.FechaInicio.Value.ToString("dd-MM-yyyy") : "",
                    fechaFin = x.FechaFin.HasValue ? x.FechaFin.Value.ToString("dd-MM-yyyy") : "",
                    dias = x.Dias.HasValue ? x.Dias.Value.ToString("0.##") : "0",
                    estado = estadoVisual,

                    puedeEditar =
                        User.IsInRole("Administrador") ||
                        (!PuedeAprobarJefeDirecto && !PuedeAprobarTH && x.EmpleadoId == user.EmpleadoId && x.EstadoJefeDirecto == "Pendiente"),

                    puedeEliminar =
                        User.IsInRole("Administrador") ||
                        (!PuedeAprobarJefeDirecto && !PuedeAprobarTH && x.EmpleadoId == user.EmpleadoId && x.EstadoJefeDirecto == "Pendiente"),

                    puedeAprobarJefe =
                        PuedeAprobarJefeDirecto &&
                        x.JefeDirectoEmpleadoId == user.EmpleadoId &&
                        x.EstadoJefeDirecto == "Pendiente",

                    puedeAprobarTH =
                        PuedeAprobarTH &&
                        x.EstadoJefeDirecto == "Aprobado" &&
                        x.EstadoTH == "Pendiente"
                };
            }).ToList();

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnGetAusenciasHorasAsync()
        {
            await ConfigurarPermisosAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || user.EmpleadoId == null)
                return new JsonResult(new List<object>());

            var query = _db.Ausencias
                .Include(x => x.TipoAusencia)
                .Include(x => x.Empleado)
                .Include(x => x.JefeDirectoEmpleado)
                .Where(x => x.TipoCaptura == "Horas")
                .AsQueryable();

            // Administrador ve todo
            if (User.IsInRole("Administrador"))
            {
                // no filtramos
            }
            // Jefe directo ve solo lo que le toca autorizar
            else if (PuedeAprobarJefeDirecto)
            {
                query = query.Where(x => x.JefeDirectoEmpleadoId == user.EmpleadoId);
            }
            // TH ve lo que ya aprobó jefe directo
            else if (PuedeAprobarTH)
            {
                query = query.Where(x => x.EstadoJefeDirecto == "Aprobado");
            }
            // Empleado normal ve solo lo suyo
            else
            {
                query = query.Where(x => x.EmpleadoId == user.EmpleadoId);
            }

            var lista = await query
                .OrderByDescending(x => x.FechaInicio)
                .ToListAsync();

            var data = lista.Select(x =>
            {
                string estadoVisual = ObtenerEstadoVisual(x);

                return new
                {
                    id = x.Id,
                    tipo = x.TipoAusencia != null ? x.TipoAusencia.Nombre : x.Categoria,
                    fechaInicio = x.FechaInicio.HasValue ? x.FechaInicio.Value.ToString("dd-MM-yyyy") : "",
                    horaInicio = x.HoraInicio.HasValue ? x.HoraInicio.Value.ToString(@"hh\:mm") + " hrs." : "",
                    horaTermino = x.HoraTermino.HasValue ? x.HoraTermino.Value.ToString(@"hh\:mm") + " hrs." : "",
                    horas = x.Horas.HasValue ? x.Horas.Value.ToString("0.##") + " hrs." : "",
                    estado = estadoVisual,

                    puedeEditar =
                        User.IsInRole("Administrador") ||
                        (!PuedeAprobarJefeDirecto && !PuedeAprobarTH && x.EmpleadoId == user.EmpleadoId && x.EstadoJefeDirecto == "Pendiente"),

                    puedeEliminar =
                        User.IsInRole("Administrador") ||
                        (!PuedeAprobarJefeDirecto && !PuedeAprobarTH && x.EmpleadoId == user.EmpleadoId && x.EstadoJefeDirecto == "Pendiente"),

                    puedeAprobarJefe =
                        PuedeAprobarJefeDirecto &&
                        x.JefeDirectoEmpleadoId == user.EmpleadoId &&
                        x.EstadoJefeDirecto == "Pendiente",

                    puedeAprobarTH =
                        PuedeAprobarTH &&
                        x.EstadoJefeDirecto == "Aprobado" &&
                        x.EstadoTH == "Pendiente"
                };
            }).ToList();

            return new JsonResult(data);
        }

        public async Task<JsonResult> OnGetDetalleAsync(int id)
        {
            await ConfigurarPermisosAsync();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return new JsonResult(new { tieneError = true, mensaje = "Usuario no encontrado." });

            var query = _db.Ausencias
                .Include(x => x.Empleado)
                .Include(x => x.TipoAusencia)
                .Include(x => x.TipoIncapacidad)
                .Include(x => x.UsuarioCreador)
                .AsQueryable();

            if (!PuedeVerTodasAusencias && user.EmpleadoId != null)
                query = query.Where(x => x.EmpleadoId == user.EmpleadoId);

            var item = await query.FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return new JsonResult(new { tieneError = true, mensaje = "Registro no encontrado." });

            return new JsonResult(new
            {
                tieneError = false,
                id = item.Id,
                empleado = item.Empleado != null ? item.Empleado.Nombre : "",
                categoria = item.Categoria,
                tipo = item.TipoAusencia != null ? item.TipoAusencia.Nombre :
                       item.TipoIncapacidad != null ? item.TipoIncapacidad.Nombre : item.Categoria,
                estado = ObtenerEstadoVisual(item),
                fechaInicio = item.FechaInicio?.ToString("yyyy-MM-dd"),
                fechaFin = item.FechaFin?.ToString("yyyy-MM-dd"),
                horaInicio = item.HoraInicio?.ToString(@"hh\:mm"),
                horaTermino = item.HoraTermino?.ToString(@"hh\:mm"),
                dias = item.Dias,
                horas = item.Horas,
                fechaAplicacion = item.FechaAplicacion?.ToString("yyyy-MM-dd"),
                suplencia = item.Suplencia,
                numeroFolio = item.NumeroFolio,
                comentario = item.Comentario,
                tipoAusenciaId = item.TipoAusenciaId,
                tipoIncapacidadId = item.TipoIncapacidadId,
                usuarioCreador = item.UsuarioCreador != null ? item.UsuarioCreador.UserName : "",
                puedeAprobarJefe = PuedeAprobarJefeDirecto && item.EstadoJefeDirecto == "Pendiente",
                puedeAprobarTH = PuedeAprobarTH && item.EstadoJefeDirecto == "Aprobado" && item.EstadoTH == "Pendiente"
            });
        }

        public async Task<JsonResult> OnPostEditarAsync()
        {
            try
            {
                await ConfigurarPermisosAsync();

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Usuario no encontrado." });

                var item = await _db.Ausencias.FirstOrDefaultAsync(x => x.Id == EditarInput.Id);
                if (item == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Registro no encontrado." });

                if (!PuedeVerTodasAusencias)
                {
                    if (user.EmpleadoId == null || item.EmpleadoId != user.EmpleadoId)
                        return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para editar este registro." });

                    if (item.EstadoJefeDirecto != "Pendiente")
                        return new JsonResult(new { tieneError = true, mensaje = "Solo puedes editar registros pendientes." });
                }

                item.TipoAusenciaId = EditarInput.TipoAusenciaId;
                item.TipoIncapacidadId = EditarInput.TipoIncapacidadId;
                item.FechaInicio = EditarInput.FechaInicio;
                item.FechaFin = EditarInput.FechaFin;
                item.NumeroFolio = EditarInput.NumeroFolio;
                item.Dias = EditarInput.Dias;
                item.FechaAplicacion = EditarInput.FechaAplicacion;
                item.Suplencia = EditarInput.Suplencia;
                item.Comentario = EditarInput.Comentario;

                if (!string.IsNullOrWhiteSpace(EditarInput.HoraInicio) &&
                    !string.IsNullOrWhiteSpace(EditarInput.HoraTermino))
                {
                    item.HoraInicio = TimeSpan.Parse(EditarInput.HoraInicio);
                    item.HoraTermino = TimeSpan.Parse(EditarInput.HoraTermino);
                    item.Horas = Convert.ToDecimal((item.HoraTermino.Value - item.HoraInicio.Value).TotalHours);
                    item.TipoCaptura = "Horas";
                    item.Dias = null;
                }
                else
                {
                    item.HoraInicio = null;
                    item.HoraTermino = null;
                    item.Horas = null;
                    item.TipoCaptura = "Dias";
                }

                await _db.SaveChangesAsync();

                return new JsonResult(new { tieneError = false, mensaje = "Registro actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostEliminarAsync(int id)
        {
            try
            {
                await ConfigurarPermisosAsync();

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Usuario no encontrado." });

                var item = await _db.Ausencias.FirstOrDefaultAsync(x => x.Id == id);
                if (item == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Registro no encontrado." });

                if (!PuedeVerTodasAusencias)
                {
                    if (user.EmpleadoId == null || item.EmpleadoId != user.EmpleadoId)
                        return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para eliminar este registro." });

                    if (item.EstadoJefeDirecto != "Pendiente")
                        return new JsonResult(new { tieneError = true, mensaje = "Solo puedes eliminar registros pendientes." });
                }

                _db.Ausencias.Remove(item);
                await _db.SaveChangesAsync();

                return new JsonResult(new { tieneError = false, mensaje = "Registro eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostAprobarJefeDirectoAsync(int id)
        {
            try
            {
                await ConfigurarPermisosAsync();

                if (!PuedeAprobarJefeDirecto)
                    return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para aprobar como jefe directo." });

                var user = await _userManager.GetUserAsync(User);
                if (user?.EmpleadoId == null)
                    return new JsonResult(new { tieneError = true, mensaje = "El usuario actual no tiene empleado relacionado." });

                var item = await _db.Ausencias
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Registro no encontrado." });

                if (item.JefeDirectoEmpleadoId != user.EmpleadoId)
                    return new JsonResult(new { tieneError = true, mensaje = "No eres el jefe directo asignado a este registro." });

                if (item.EstadoJefeDirecto != "Pendiente")
                    return new JsonResult(new { tieneError = true, mensaje = "Este registro ya fue revisado por el jefe directo." });

                item.EstadoJefeDirecto = "Aprobado";
                item.UsuarioJefeDirectoId = user.Id;
                item.FechaRevisionJefeDirecto = DateTime.Now;

                await _db.SaveChangesAsync();

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "El jefe directo aprobó correctamente el registro."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostRechazarJefeDirectoAsync(int id)
        {
            try
            {
                await ConfigurarPermisosAsync();

                if (!PuedeAprobarJefeDirecto)
                    return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para rechazar como jefe directo." });

                var user = await _userManager.GetUserAsync(User);
                if (user?.EmpleadoId == null)
                    return new JsonResult(new { tieneError = true, mensaje = "El usuario actual no tiene empleado relacionado." });

                var item = await _db.Ausencias
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Registro no encontrado." });

                if (item.JefeDirectoEmpleadoId != user.EmpleadoId)
                    return new JsonResult(new { tieneError = true, mensaje = "No eres el jefe directo asignado a este registro." });

                if (item.EstadoJefeDirecto != "Pendiente")
                    return new JsonResult(new { tieneError = true, mensaje = "Este registro ya fue revisado por el jefe directo." });

                item.EstadoJefeDirecto = "Rechazado";
                item.UsuarioJefeDirectoId = user.Id;
                item.FechaRevisionJefeDirecto = DateTime.Now;

                await _db.SaveChangesAsync();

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "El jefe directo rechazó correctamente el registro."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostAprobarTHAsync(int id)
        {
            try
            {
                await ConfigurarPermisosAsync();

                if (!PuedeAprobarTH)
                    return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para aprobar como Talento Humano." });

                var user = await _userManager.GetUserAsync(User);
                var item = await _db.Ausencias.FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Registro no encontrado." });

                if (item.EstadoJefeDirecto != "Aprobado")
                    return new JsonResult(new { tieneError = true, mensaje = "Primero debe aprobar el jefe directo." });

                item.EstadoTH = "Aprobado";
                item.UsuarioTHId = user?.Id;
                item.FechaRevisionTH = DateTime.Now;

                await _db.SaveChangesAsync();

                return new JsonResult(new { tieneError = false, mensaje = "Talento Humano aprobó correctamente el permiso." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostRechazarTHAsync(int id)
        {
            try
            {
                await ConfigurarPermisosAsync();

                if (!PuedeAprobarTH)
                    return new JsonResult(new { tieneError = true, mensaje = "No tienes permisos para rechazar como Talento Humano." });

                var user = await _userManager.GetUserAsync(User);
                var item = await _db.Ausencias.FirstOrDefaultAsync(x => x.Id == id);

                if (item == null)
                    return new JsonResult(new { tieneError = true, mensaje = "Registro no encontrado." });

                if (item.EstadoJefeDirecto != "Aprobado")
                    return new JsonResult(new { tieneError = true, mensaje = "Primero debe aprobar el jefe directo." });

                item.EstadoTH = "Rechazado";
                item.UsuarioTHId = user?.Id;
                item.FechaRevisionTH = DateTime.Now;

                await _db.SaveChangesAsync();

                return new JsonResult(new { tieneError = false, mensaje = "Talento Humano rechazó correctamente el permiso." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostGuardarInasistenciaAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || user.EmpleadoId == null)
                    return new JsonResult(new { tieneError = true, mensaje = "No se encontró el empleado del usuario actual." });

                var jefeDirectoEmpleadoId = await ObtenerJefeDirectoEmpleadoIdAsync(user.EmpleadoId.Value);

                if (!jefeDirectoEmpleadoId.HasValue)
                    return new JsonResult(new { tieneError = true, mensaje = "El empleado no tiene jefe directo asignado." });

                var ausencia = new Ausencia
                {
                    EmpleadoId = user.EmpleadoId,
                    JefeDirectoEmpleadoId = jefeDirectoEmpleadoId,

                    Categoria = "Inasistencia",
                    TipoCaptura = "Dias",
                    FechaInicio = InasistenciaInput.FechaInicio,
                    FechaFin = InasistenciaInput.FechaFin,
                    Dias = InasistenciaInput.Dias,
                    FechaAplicacion = InasistenciaInput.FechaAplicacion,
                    Suplencia = InasistenciaInput.Suplencia,
                    Comentario = InasistenciaInput.Comentario,

                    EstadoJefeDirecto = "Pendiente",
                    EstadoTH = "Pendiente",

                    UsuarioCreadorId = user.Id,
                    FechaCreacion = DateTime.Now
                };

                _db.Ausencias.Add(ausencia);
                await _db.SaveChangesAsync();

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "Inasistencia registrada correctamente y enviada a aprobación del jefe directo."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostGuardarIncapacidadAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || user.EmpleadoId == null)
                    return new JsonResult(new { tieneError = true, mensaje = "No se encontró el empleado del usuario actual." });

                var jefeDirectoEmpleadoId = await ObtenerJefeDirectoEmpleadoIdAsync(user.EmpleadoId.Value);

                if (!jefeDirectoEmpleadoId.HasValue)
                    return new JsonResult(new { tieneError = true, mensaje = "El empleado no tiene jefe directo asignado." });

                var ausencia = new Ausencia
                {
                    EmpleadoId = user.EmpleadoId,
                    JefeDirectoEmpleadoId = jefeDirectoEmpleadoId,

                    Categoria = "Incapacidad",
                    TipoCaptura = "Dias",
                    TipoIncapacidadId = IncapacidadInput.TipoIncapacidadId,
                    NumeroFolio = IncapacidadInput.NumeroFolio,
                    FechaInicio = IncapacidadInput.FechaInicio,
                    FechaFin = IncapacidadInput.FechaFin,
                    Dias = IncapacidadInput.Dias,
                    FechaAplicacion = IncapacidadInput.FechaAplicacion,
                    Suplencia = IncapacidadInput.Suplencia,
                    Comentario = IncapacidadInput.Comentario,

                    EstadoJefeDirecto = "Pendiente",
                    EstadoTH = "Pendiente",

                    UsuarioCreadorId = user.Id,
                    FechaCreacion = DateTime.Now
                };

                _db.Ausencias.Add(ausencia);
                await _db.SaveChangesAsync();

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "Incapacidad registrada correctamente y enviada a aprobación del jefe directo."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostGuardarPermisoAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || user.EmpleadoId == null)
                    return new JsonResult(new { tieneError = true, mensaje = "No se encontró el empleado del usuario actual." });

                var jefeDirectoEmpleadoId = await ObtenerJefeDirectoEmpleadoIdAsync(user.EmpleadoId.Value);

                if (!jefeDirectoEmpleadoId.HasValue)
                    return new JsonResult(new { tieneError = true, mensaje = "El empleado no tiene jefe directo asignado." });

                TimeSpan? horaInicio = null;
                TimeSpan? horaTermino = null;
                decimal? horas = null;
                string tipoCaptura = "Dias";

                if (!string.IsNullOrWhiteSpace(PermisoInput.HoraInicio) &&
                    !string.IsNullOrWhiteSpace(PermisoInput.HoraTermino))
                {
                    horaInicio = TimeSpan.Parse(PermisoInput.HoraInicio);
                    horaTermino = TimeSpan.Parse(PermisoInput.HoraTermino);
                    horas = Convert.ToDecimal((horaTermino.Value - horaInicio.Value).TotalHours);
                    tipoCaptura = "Horas";
                }

                var ausencia = new Ausencia
                {
                    EmpleadoId = user.EmpleadoId,
                    JefeDirectoEmpleadoId = jefeDirectoEmpleadoId,

                    Categoria = "Permiso",
                    TipoCaptura = tipoCaptura,
                    TipoAusenciaId = PermisoInput.TipoAusenciaId,
                    FechaInicio = PermisoInput.FechaInicio,
                    FechaFin = PermisoInput.FechaFin,
                    HoraInicio = horaInicio,
                    HoraTermino = horaTermino,
                    Horas = horas,
                    Dias = tipoCaptura == "Dias" ? PermisoInput.Dias : null,
                    FechaAplicacion = PermisoInput.FechaAplicacion,
                    Suplencia = PermisoInput.Suplencia,
                    Comentario = PermisoInput.Comentario,

                    EstadoJefeDirecto = "Pendiente",
                    EstadoTH = "Pendiente",

                    UsuarioCreadorId = user.Id,
                    FechaCreacion = DateTime.Now
                };

                _db.Ausencias.Add(ausencia);
                await _db.SaveChangesAsync();

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "Permiso registrado correctamente y enviado a aprobación del jefe directo."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        public async Task<JsonResult> OnPostSolicitarPermisoAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null || user.EmpleadoId == null)
                    return new JsonResult(new { tieneError = true, mensaje = "No se encontró el empleado del usuario actual." });

                var jefeDirectoEmpleadoId = await ObtenerJefeDirectoEmpleadoIdAsync(user.EmpleadoId.Value);

                if (!jefeDirectoEmpleadoId.HasValue)
                    return new JsonResult(new { tieneError = true, mensaje = "El empleado no tiene jefe directo asignado." });

                TimeSpan? horaInicio = null;
                TimeSpan? horaTermino = null;
                decimal? horas = null;
                string tipoCaptura = "Dias";

                if (!string.IsNullOrWhiteSpace(SolicitudPermisoInput.HoraInicio) &&
                    !string.IsNullOrWhiteSpace(SolicitudPermisoInput.HoraTermino))
                {
                    horaInicio = TimeSpan.Parse(SolicitudPermisoInput.HoraInicio);
                    horaTermino = TimeSpan.Parse(SolicitudPermisoInput.HoraTermino);
                    horas = Convert.ToDecimal((horaTermino.Value - horaInicio.Value).TotalHours);
                    tipoCaptura = "Horas";
                }

                var ausencia = new Ausencia
                {
                    EmpleadoId = user.EmpleadoId,
                    JefeDirectoEmpleadoId = jefeDirectoEmpleadoId,

                    Categoria = "SolicitudPermiso",
                    TipoCaptura = tipoCaptura,
                    TipoAusenciaId = SolicitudPermisoInput.TipoAusenciaId,
                    FechaInicio = SolicitudPermisoInput.FechaInicio,
                    FechaFin = SolicitudPermisoInput.FechaFin,
                    HoraInicio = horaInicio,
                    HoraTermino = horaTermino,
                    Horas = horas,
                    FechaAplicacion = SolicitudPermisoInput.FechaAplicacion,
                    Suplencia = false,
                    Comentario = SolicitudPermisoInput.Comentario,

                    EstadoJefeDirecto = "Pendiente",
                    EstadoTH = "Pendiente",

                    UsuarioCreadorId = user.Id,
                    FechaCreacion = DateTime.Now
                };

                _db.Ausencias.Add(ausencia);
                await _db.SaveChangesAsync();

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = "Permiso solicitado correctamente y enviado a aprobación del jefe directo."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { tieneError = true, mensaje = ex.Message });
            }
        }

        private async Task<int?> ObtenerJefeDirectoEmpleadoIdAsync(int empleadoId)
        {
            return await _db.Empleados
                .Where(x => x.Id == empleadoId)
                .Select(x => x.JefeId)
                .FirstOrDefaultAsync();
        }
    }
}