using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Intranet;
using ERPSEI.Data.Managers.Usuarios;
using ERPSEI.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.Catalogos.Pages.GestorEventos
{
    public class IndexModel : PageModel
    {
        private readonly IEventoIntranetManager _eventoManager;
        private readonly IWebHostEnvironment _environment;
        private readonly AppUserManager _userManager;
        private readonly IIntranetNotificationService _notificationService;
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            IEventoIntranetManager eventoManager,
            IWebHostEnvironment environment,
            AppUserManager userManager,
            IIntranetNotificationService notificationService,
            ApplicationDbContext db,
            IEmailSender emailSender)
        {
            _eventoManager = eventoManager;
            _environment = environment;
            _userManager = userManager;
            _notificationService = notificationService;
            _db = db;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public string? TipoEvento { get; set; }
            public DateTime FechaEvento { get; set; } = DateTime.Today;
            public string? HoraEvento { get; set; }
            public string? FechaPublicacionProgramada { get; set; }
            public bool RequiereGeolocalizacion { get; set; }
            public string? Region { get; set; }
            public string? UrlFormulario { get; set; }
            //public string? TextoBoton { get; set; }
            public bool Activo { get; set; } = true;
            public IFormFile? Portada { get; set; }

            public bool NotificacionEnviada { get; set; } = false;
            public DateTime? FechaNotificacion { get; set; }
        }

        public async Task OnGetAsync()
        {
            await ProcesarEventosProgramadosAsync();
        }

        private async Task ProcesarEventosProgramadosAsync()
        {
            var ahora = DateTime.Now;

            var eventos = await _eventoManager.GetAllAsync(true);

            var pendientes = eventos
                .Where(x =>
                    x.Activo &&
                    x.EsProgramado &&
                    !x.Publicado &&
                    x.FechaPublicacionProgramada.HasValue &&
                    x.FechaPublicacionProgramada.Value <= ahora)
                .ToList();

            foreach (var evento in pendientes)
            {
                evento.Publicado = true;
                evento.FechaModificacion = ahora;

                if (!evento.NotificacionEnviada)
                {
                    await CrearNotificacionPublicacionEventoAsync(evento);

                    evento.NotificacionEnviada = true;
                    evento.FechaNotificacion = ahora;
                }

                await _eventoManager.UpdateAsync(evento);
            }
        }

        public async Task<IActionResult> OnGetEventosListAsync()
        {
            var lista = await _eventoManager.GetAllAsync(true);

            var result = lista.Select(x => new
            {
                id = x.Id,
                titulo = x.Titulo,
                tipoEvento = x.TipoEvento,
                fechaEvento = x.FechaEvento.ToString("dd/MM/yyyy"),
                horaEvento = x.HoraEvento.HasValue ? x.HoraEvento.Value.ToString(@"hh\:mm") : "",
                publicado = x.Publicado,
                activo = x.Activo,
                requiereGeolocalizacion = x.RequiereGeolocalizacion,
                region = x.Region,
                rutaPortada = x.RutaPortada,
                estatus = x.Activo ? "Activo" : "Inactivo",
                estadoPublicacion = x.Publicado ? "Publicado" : "Pendiente"
            });

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnGetEventoByIdAsync(int id)
        {
            var entity = await _eventoManager.GetByIdAsync(id);

            if (entity == null)
            {
                return new JsonResult(new { tieneError = true, mensaje = "No se encontró el evento." });
            }

            return new JsonResult(new
            {
                tieneError = false,
                id = entity.Id,
                titulo = entity.Titulo,
                descripcion = entity.Descripcion,
                tipoEvento = entity.TipoEvento,
                fechaEvento = entity.FechaEvento.ToString("yyyy-MM-dd"),
                horaEvento = entity.HoraEvento.HasValue ? entity.HoraEvento.Value.ToString(@"hh\:mm") : "",
                fechaPublicacionProgramada = entity.FechaPublicacionProgramada.HasValue
                    ? entity.FechaPublicacionProgramada.Value.ToString("yyyy-MM-ddTHH:mm")
                    : "",
                requiereGeolocalizacion = entity.RequiereGeolocalizacion,
                region = entity.Region,
                urlFormulario = entity.UrlFormulario,
                textoBoton = entity.TextoBoton,
                activo = entity.Activo,
                publicado = entity.Publicado,
                rutaPortada = entity.RutaPortada
            });
        }

        public async Task<IActionResult> OnPostSaveEventoAsync(bool publicar = false)
        {
            try
            {
                AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);

                if (string.IsNullOrWhiteSpace(Input.Titulo))
                    return new JsonResult(new { tieneError = true, mensaje = "El título es obligatorio." });

                EventoIntranet? entity = null;

                if (Input.Id > 0)
                {
                    entity = await _eventoManager.GetByIdAsync(Input.Id);
                    if (entity == null)
                        return new JsonResult(new { tieneError = true, mensaje = "No se encontró el evento." });
                }

                string? rutaPortada = entity?.RutaPortada;
                string? nombrePortada = entity?.NombrePortada;

                if (Input.Portada != null && Input.Portada.Length > 0)
                {
                    string extension = Path.GetExtension(Input.Portada.FileName).ToLowerInvariant();
                    string[] permitidas = [".jpg", ".jpeg", ".png", ".webp"];

                    if (!permitidas.Contains(extension))
                        return new JsonResult(new { tieneError = true, mensaje = "La portada solo permite JPG, JPEG, PNG o WEBP." });

                    string carpeta = Path.Combine(_environment.WebRootPath, "documentos", "eventos", "portadas");
                    if (!Directory.Exists(carpeta))
                        Directory.CreateDirectory(carpeta);

                    string nombreUnico = $"{Guid.NewGuid()}{extension}";
                    string rutaFisica = Path.Combine(carpeta, nombreUnico);

                    using (var stream = new FileStream(rutaFisica, FileMode.Create))
                    {
                        await Input.Portada.CopyToAsync(stream);
                    }

                    rutaPortada = $"/documentos/eventos/portadas/{nombreUnico}";
                    nombrePortada = Input.Portada.FileName;
                }

                TimeSpan? hora = null;
                if (!string.IsNullOrWhiteSpace(Input.HoraEvento) &&
                    TimeSpan.TryParse(Input.HoraEvento, out TimeSpan horaParseada))
                {
                    hora = horaParseada;
                }

                DateTime? fechaProgramada = null;
                if (!string.IsNullOrWhiteSpace(Input.FechaPublicacionProgramada) &&
                    DateTime.TryParse(Input.FechaPublicacionProgramada, out DateTime fechaProgramadaParseada))
                {
                    fechaProgramada = fechaProgramadaParseada;
                }

                if (Input.Id == 0)
                {
                    entity = new EventoIntranet
                    {
                        Titulo = Input.Titulo.Trim(),
                        Descripcion = Input.Descripcion?.Trim(),
                        TipoEvento = Input.TipoEvento,
                        FechaEvento = Input.FechaEvento,
                        HoraEvento = hora,
                        FechaPublicacionProgramada = fechaProgramada,
                        EsProgramado = fechaProgramada.HasValue,
                        Publicado = publicar,
                        RequiereGeolocalizacion = Input.RequiereGeolocalizacion,
                        Region = Input.Region,
                        UrlFormulario = Input.UrlFormulario,
                        //TextoBoton = string.IsNullOrWhiteSpace(Input.TextoBoton) ? "Consulta aquí" : Input.TextoBoton,
                        TextoBoton = "Consulta aquí",
                        RutaPortada = rutaPortada,
                        NombrePortada = nombrePortada,
                        Activo = Input.Activo,
                        FechaCreacion = DateTime.Now,
                        CreadoPorId = usr?.Id
                    };

                    await _eventoManager.AddAsync(entity);
                }
                else
                {
                    entity!.Titulo = Input.Titulo.Trim();
                    entity.Descripcion = Input.Descripcion?.Trim();
                    entity.TipoEvento = Input.TipoEvento;
                    entity.FechaEvento = Input.FechaEvento;
                    entity.HoraEvento = hora;
                    entity.FechaPublicacionProgramada = fechaProgramada;
                    entity.EsProgramado = fechaProgramada.HasValue;
                    entity.Publicado = publicar || entity.Publicado;
                    entity.RequiereGeolocalizacion = Input.RequiereGeolocalizacion;
                    entity.Region = Input.Region;
                    entity.UrlFormulario = Input.UrlFormulario;
                    //entity.TextoBoton = string.IsNullOrWhiteSpace(Input.TextoBoton) ? "Consulta aquí" : Input.TextoBoton;
                    entity.TextoBoton = "Consulta aquí";
                    entity.RutaPortada = rutaPortada;
                    entity.NombrePortada = nombrePortada;
                    entity.Activo = Input.Activo;
                    entity.FechaModificacion = DateTime.Now;
                    entity.ModificadoPorId = usr?.Id;

                    await _eventoManager.UpdateAsync(entity);
                }

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = publicar ? "Evento publicado correctamente." : "Evento guardado correctamente."
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    tieneError = true,
                    mensaje = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        /*public async Task<IActionResult> OnPostPublicarAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _eventoManager.PublicarAsync(id, usr?.Id);

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok ? "Evento publicado correctamente." : "No se pudo publicar el evento."
            });
        }*/

        /*public async Task<IActionResult> OnPostPublicarAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _eventoManager.PublicarAsync(id, usr?.Id);

            if (ok)
            {
                var entity = await _eventoManager.GetByIdAsync(id);

                if (entity != null && !entity.NotificacionEnviada)
                {
                    string baseUrl = $"{Request.Scheme}://{Request.Host}";
                    string urlDestino = $"{baseUrl}/Catalogos/Eventos?openId={entity.Id}";

                    await _notificationService.EnviarNotificacionEventoPruebaAsync(
                        entity.Titulo,
                        entity.Descripcion,
                        urlDestino);

                    entity.NotificacionEnviada = true;
                    entity.FechaNotificacion = DateTime.Now;
                    await _eventoManager.UpdateAsync(entity);
                }
            }

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok ? "Evento publicado correctamente." : "No se pudo publicar el evento."
            });
        }*/

        public async Task<IActionResult> OnPostPublicarAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _eventoManager.PublicarAsync(id, usr?.Id);

            if (ok)
            {
                var entity = await _eventoManager.GetByIdAsync(id);

                if (entity != null && !entity.NotificacionEnviada)
                {
                    await CrearNotificacionPublicacionEventoAsync(entity);

                    entity.NotificacionEnviada = true;
                    entity.FechaNotificacion = DateTime.Now;
                    await _eventoManager.UpdateAsync(entity);
                }
            }

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok
                    ? "Evento publicado correctamente. Se generó la notificación."
                    : "No se pudo publicar el evento."
            });
        }

        public async Task<IActionResult> OnPostToggleActivoAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _eventoManager.ToggleActivoAsync(id, usr?.Id);

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok ? "Estatus actualizado correctamente." : "No se pudo actualizar el estatus."
            });
        }

        public async Task<IActionResult> OnPostDeleteEventoAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _eventoManager.DeleteAsync(id, usr?.Id);

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok ? "Evento eliminado correctamente." : "No se pudo eliminar el evento."
            });
        }

        private async Task CrearNotificacionPublicacionEventoAsync(EventoIntranet evento)
        {
            var usuarios = await _db.Users
                .Include(u => u.Empleado)
                .Where(u =>
                    !string.IsNullOrWhiteSpace(u.Email) &&
                    u.Empleado != null &&
                    u.Empleado.Deshabilitado == 0)
                .ToListAsync();

            string urlInterna = $"/Catalogos/Eventos?openId={evento.Id}";
            string urlCorreo = $"{Request.Scheme}://{Request.Host}{urlInterna}";

            var notificacion = new NotificacionIntranet
            {
                Titulo = "Nuevo evento publicado",
                Descripcion = evento.Titulo,
                Tipo = "Evento",
                Modulo = "Eventos",
                Url = urlInterna,
                Icono = "bi bi-calendar-event-fill",
                FechaPublicacion = DateTime.Now,
                Activa = true,
                UserIdCreador = _userManager.GetUserId(User)
            };

            foreach (var usuario in usuarios)
            {
                notificacion.UsuariosNotificados.Add(new NotificacionIntranetUsuario
                {
                    UserId = usuario.Id,
                    Leida = false,
                    FechaCreacion = DateTime.Now
                });
            }

            _db.NotificacionesIntranet.Add(notificacion);
            await _db.SaveChangesAsync();

            string correoPruebas = "jcruz@asesorcliente.com";

            string cuerpo = $@"
                <div style='font-family:Arial,sans-serif;color:#1f1466;'>

                    <div style='background:#1f1466;padding:18px 22px;border-radius:14px 14px 0 0;color:#ffffff;'>
                        <h2 style='margin:0;font-size:22px;'>Nuevo evento publicado</h2>
                    </div>

                    <div style='border:1px solid #e5e7eb;border-top:0;padding:24px;border-radius:0 0 14px 14px;background:#ffffff;'>

                        <p style='font-size:15px;color:#374151;'>Hola,</p>

                        <p style='font-size:15px;color:#374151;line-height:1.6;'>
                            Se ha publicado un nuevo evento en la intranet corporativa de SEI.
                        </p>

                        <div style='background:#f8f9ff;border-left:4px solid #1f1466;padding:16px;border-radius:10px;margin:18px 0;'>

                            <div style='font-size:18px;font-weight:700;color:#1f1466;margin-bottom:8px;'>
                                {evento.Titulo}
                            </div>

                            <div style='font-size:14px;color:#4b5563;line-height:1.5;'>
                                {evento.Descripcion}
                            </div>

                            <div style='font-size:13px;color:#4b5563;margin-top:10px;'>
                                Fecha del evento: <strong>{evento.FechaEvento:dd/MM/yyyy}</strong>
                            </div>

                        </div>

                        <p style='margin-top:24px;'>
                            <a href='{urlCorreo}'
                               style='display:inline-block;background:#1f1466;color:#ffffff;padding:12px 18px;border-radius:10px;text-decoration:none;font-weight:600;'>
                                Ver evento
                            </a>
                        </p>

                        <hr style='margin:28px 0;border:none;border-top:1px solid #e5e7eb;' />

                        <p style='font-size:12px;color:#6b7280;'>
                            Este correo fue enviado automáticamente desde la Intranet SEI.
                        </p>

                    </div>
                </div>";

            await _emailSender.SendEmailAsync(
                correoPruebas,
                "Nuevo evento publicado - Intranet SEI",
                cuerpo
            );
        }
    }
}