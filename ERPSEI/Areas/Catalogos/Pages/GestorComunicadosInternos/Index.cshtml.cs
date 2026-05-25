using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Intranet;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ERPSEI.Data;
using ERPSEI.Email;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.Catalogos.Pages.GestorComunicadosInternos
{
    public class IndexModel : PageModel
    {
        private readonly IComunicadoInternoManager _comunicadoManager;
        private readonly IWebHostEnvironment _environment;
        private readonly AppUserManager _userManager;
        private readonly IIntranetNotificationService _notificationService;
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            IComunicadoInternoManager comunicadoManager,
            IWebHostEnvironment environment,
            AppUserManager userManager,
            IIntranetNotificationService notificationService,
            ApplicationDbContext db,
            IEmailSender emailSender)
        {
            _comunicadoManager = comunicadoManager;
            _environment = environment;
            _userManager = userManager;
            _notificationService = notificationService;
            _db = db;
            _emailSender = emailSender;
        }

        [BindProperty(SupportsGet = true)]
        public int? OpenId { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public DateTime FechaPublicacion { get; set; } = DateTime.Today;
            public string? HoraPublicacion { get; set; }
            public bool Activo { get; set; } = true;
            public bool Publicado { get; set; } = false;
            public bool EsPermanente { get; set; } = false;
            public IFormFile? Archivo { get; set; }
            public IFormFile? Portada { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetComunicadosListAsync()
        {
            var lista = await _comunicadoManager.GetAllAsync(true);

            var result = lista.Select(x => new
            {
                id = x.Id,
                titulo = x.Titulo,
                fechaPublicacion = x.FechaPublicacion.ToString("dd/MM/yyyy"),
                horaPublicacion = x.HoraPublicacion.HasValue ? x.HoraPublicacion.Value.ToString(@"hh\:mm") : "",
                nombreArchivo = x.NombreArchivo,
                rutaArchivo = x.RutaArchivo,
                rutaPortada = x.RutaPortada,
                extensionArchivo = x.ExtensionArchivo,
                activo = x.Activo,
                publicado = x.Publicado,
                esPermanente = x.EsPermanente,
                estatus = x.Activo ? "Activo" : "Inactivo",
                estadoPublicacion = x.Publicado ? "Publicado" : "Borrador"
            });

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnGetComunicadoByIdAsync(int id)
        {
            var entity = await _comunicadoManager.GetByIdAsync(id);

            if (entity == null)
            {
                return new JsonResult(new
                {
                    tieneError = true,
                    mensaje = "No se encontró el comunicado."
                });
            }

            return new JsonResult(new
            {
                tieneError = false,
                id = entity.Id,
                titulo = entity.Titulo,
                descripcion = entity.Descripcion,
                fechaPublicacion = entity.FechaPublicacion.ToString("yyyy-MM-dd"),
                horaPublicacion = entity.HoraPublicacion.HasValue ? entity.HoraPublicacion.Value.ToString(@"hh\:mm") : "",
                nombreArchivo = entity.NombreArchivo,
                rutaArchivo = entity.RutaArchivo,
                extensionArchivo = entity.ExtensionArchivo,
                activo = entity.Activo,
                publicado = entity.Publicado,
                esPermanente = entity.EsPermanente,
                rutaPortada = entity.RutaPortada,
                nombrePortada = entity.NombrePortada
            });
        }

        public async Task<IActionResult> OnPostSaveComunicadoAsync(bool publicar = false)
        {
            try
            {
                AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);

                if (string.IsNullOrWhiteSpace(Input.Titulo))
                {
                    return new JsonResult(new { tieneError = true, mensaje = "El título es obligatorio." });
                }

                ComunicadoInterno? entity = null;

                if (Input.Id > 0)
                {
                    entity = await _comunicadoManager.GetByIdAsync(Input.Id);
                    if (entity == null)
                    {
                        return new JsonResult(new { tieneError = true, mensaje = "No se encontró el comunicado a editar." });
                    }
                }

                string? rutaArchivo = entity?.RutaArchivo;
                string? nombreArchivo = entity?.NombreArchivo;
                string? extensionArchivo = entity?.ExtensionArchivo;

                string? rutaPortada = entity?.RutaPortada;
                string? nombrePortada = entity?.NombrePortada;

                // =========================
                // ARCHIVO PRINCIPAL
                // =========================
                if (Input.Archivo != null && Input.Archivo.Length > 0)
                {
                    string extension = Path.GetExtension(Input.Archivo.FileName).ToLowerInvariant();
                    string[] extensionesPermitidas = [".pdf", ".jpg", ".jpeg", ".png", ".webp"];

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        return new JsonResult(new
                        {
                            tieneError = true,
                            mensaje = "El archivo principal solo permite PDF, JPG, JPEG, PNG o WEBP."
                        });
                    }

                    string carpetaDestino = Path.Combine(_environment.WebRootPath, "documentos", "comunicadosinternos");
                    if (!Directory.Exists(carpetaDestino))
                        Directory.CreateDirectory(carpetaDestino);

                    string nombreUnico = $"{Guid.NewGuid()}{extension}";
                    string rutaFisica = Path.Combine(carpetaDestino, nombreUnico);

                    using (var stream = new FileStream(rutaFisica, FileMode.Create))
                    {
                        await Input.Archivo.CopyToAsync(stream);
                    }

                    rutaArchivo = $"/documentos/comunicadosinternos/{nombreUnico}";
                    nombreArchivo = Input.Archivo.FileName;
                    extensionArchivo = extension;
                }
                else if (Input.Id == 0)
                {
                    return new JsonResult(new
                    {
                        tieneError = true,
                        mensaje = "Debes seleccionar el archivo principal del comunicado."
                    });
                }

                // =========================
                // PORTADA
                // =========================
                if (Input.Portada != null && Input.Portada.Length > 0)
                {
                    string extensionPortada = Path.GetExtension(Input.Portada.FileName).ToLowerInvariant();
                    string[] extensionesPortadaPermitidas = [".jpg", ".jpeg", ".png", ".webp"];

                    if (!extensionesPortadaPermitidas.Contains(extensionPortada))
                    {
                        return new JsonResult(new
                        {
                            tieneError = true,
                            mensaje = "La portada solo permite archivos JPG, JPEG, PNG o WEBP."
                        });
                    }

                    string carpetaPortadas = Path.Combine(_environment.WebRootPath, "documentos", "comunicadosinternos", "portadas");
                    if (!Directory.Exists(carpetaPortadas))
                        Directory.CreateDirectory(carpetaPortadas);

                    string nombrePortadaUnico = $"{Guid.NewGuid()}{extensionPortada}";
                    string rutaPortadaFisica = Path.Combine(carpetaPortadas, nombrePortadaUnico);

                    using (var streamPortada = new FileStream(rutaPortadaFisica, FileMode.Create))
                    {
                        await Input.Portada.CopyToAsync(streamPortada);
                    }

                    rutaPortada = $"/documentos/comunicadosinternos/portadas/{nombrePortadaUnico}";
                    nombrePortada = Input.Portada.FileName;
                }

                TimeSpan? hora = null;
                if (!string.IsNullOrWhiteSpace(Input.HoraPublicacion) &&
                    TimeSpan.TryParse(Input.HoraPublicacion, out TimeSpan horaParseada))
                {
                    hora = horaParseada;
                }

                if (Input.Id == 0)
                {
                    entity = new ComunicadoInterno
                    {
                        Titulo = Input.Titulo.Trim(),
                        Descripcion = Input.Descripcion?.Trim(),
                        FechaPublicacion = Input.FechaPublicacion,
                        HoraPublicacion = hora,
                        RutaArchivo = rutaArchivo!,
                        NombreArchivo = nombreArchivo,
                        ExtensionArchivo = extensionArchivo,
                        RutaPortada = rutaPortada,
                        NombrePortada = nombrePortada,
                        Activo = Input.Activo,
                        Publicado = publicar,
                        EsPermanente = Input.EsPermanente,
                        FechaCreacion = DateTime.Now,
                        CreadoPorId = usr?.Id
                    };

                    await _comunicadoManager.AddAsync(entity);
                }
                else
                {
                    entity!.Titulo = Input.Titulo.Trim();
                    entity.Descripcion = Input.Descripcion?.Trim();
                    entity.FechaPublicacion = Input.FechaPublicacion;
                    entity.HoraPublicacion = hora;
                    entity.RutaArchivo = rutaArchivo!;
                    entity.NombreArchivo = nombreArchivo;
                    entity.ExtensionArchivo = extensionArchivo;
                    entity.RutaPortada = rutaPortada;
                    entity.NombrePortada = nombrePortada;
                    entity.Activo = Input.Activo;
                    entity.Publicado = publicar || entity.Publicado;
                    entity.EsPermanente = Input.EsPermanente;
                    entity.FechaModificacion = DateTime.Now;
                    entity.ModificadoPorId = usr?.Id;

                    await _comunicadoManager.UpdateAsync(entity);
                }

                return new JsonResult(new
                {
                    tieneError = false,
                    mensaje = publicar
                        ? "Comunicado publicado correctamente."
                        : "Comunicado guardado correctamente."
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

        public async Task<IActionResult> OnPostToggleActivoAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _comunicadoManager.ToggleActivoAsync(id, usr?.Id);

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok ? "Estatus actualizado correctamente." : "No se pudo actualizar el estatus."
            });
        }

        /*public async Task<IActionResult> OnPostPublicarAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _comunicadoManager.PublicarAsync(id, usr?.Id);

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok ? "Comunicado publicado correctamente." : "No se pudo publicar el comunicado."
            });
        }*/
        /*public async Task<IActionResult> OnPostPublicarAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _comunicadoManager.PublicarAsync(id, usr?.Id);

            if (ok)
            {
                var entity = await _comunicadoManager.GetByIdAsync(id);

                if (entity != null && !entity.NotificacionEnviada)
                {
                    string baseUrl = $"{Request.Scheme}://{Request.Host}";
                    string urlDestino = $"{baseUrl}/Catalogos/ComunicadosInternos?openId={entity.Id}";

                    await _notificationService.EnviarNotificacionComunicadoPruebaAsync(
                        entity.Titulo,
                        entity.Descripcion,
                        urlDestino);

                    entity.NotificacionEnviada = true;
                    entity.FechaNotificacion = DateTime.Now;
                    await _comunicadoManager.UpdateAsync(entity);
                }
            }

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok ? "Comunicado publicado correctamente." : "No se pudo publicar el comunicado."
            });
        }*/

        public async Task<IActionResult> OnPostPublicarAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _comunicadoManager.PublicarAsync(id, usr?.Id);

            if (ok)
            {
                var entity = await _comunicadoManager.GetByIdAsync(id);

                if (entity != null && !entity.NotificacionEnviada)
                {
                    await CrearNotificacionPublicacionComunicadoAsync(entity);

                    entity.NotificacionEnviada = true;
                    entity.FechaNotificacion = DateTime.Now;
                    await _comunicadoManager.UpdateAsync(entity);
                }
            }

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok
                    ? "Comunicado publicado correctamente. Se generó la notificación."
                    : "No se pudo publicar el comunicado."
            });
        }

        public async Task<IActionResult> OnPostDeleteComunicadoAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _comunicadoManager.DeleteAsync(id, usr?.Id);

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok ? "Comunicado eliminado correctamente." : "No se pudo eliminar el comunicado."
            });
        }

        private async Task CrearNotificacionPublicacionComunicadoAsync(ComunicadoInterno comunicado)
        {
            var usuarios = await _db.Users
                .Include(u => u.Empleado)
                .Where(u =>
                    !string.IsNullOrWhiteSpace(u.Email) &&
                    u.Empleado != null &&
                    u.Empleado.Deshabilitado == 0)
                .ToListAsync();

            string urlInterna = $"/Catalogos/ComunicadosInternos?openId={comunicado.Id}";
            string urlCorreo = $"{Request.Scheme}://{Request.Host}{urlInterna}";

            var notificacion = new NotificacionIntranet
            {
                Titulo = "Nuevo comunicado publicado",
                Descripcion = comunicado.Titulo,
                Tipo = "Comunicado",
                Modulo = "Comunicados Internos",
                Url = urlInterna,
                Icono = "bi bi-megaphone-fill",
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

            //string correoPruebas = "jcruz@asesorcliente.com";
            var correos = usuarios
            .Where(x => !string.IsNullOrWhiteSpace(x.Email))
            .Select(x => x.Email!)
            .Distinct()
            .ToList();

            string cuerpo = $@"
                <div style='font-family:Arial,sans-serif;color:#1f1466;'>

                    <div style='background:#1f1466;padding:18px 22px;border-radius:14px 14px 0 0;color:#ffffff;'>
                        <h2 style='margin:0;font-size:22px;'>Nuevo comunicado publicado</h2>
                    </div>

                    <div style='border:1px solid #e5e7eb;border-top:0;padding:24px;border-radius:0 0 14px 14px;background:#ffffff;'>

                        <p style='font-size:15px;color:#374151;'>Hola,</p>

                        <p style='font-size:15px;color:#374151;line-height:1.6;'>
                            Se ha publicado un nuevo comunicado interno en la intranet corporativa de SEI.
                        </p>

                        <div style='background:#f8f9ff;border-left:4px solid #1f1466;padding:16px;border-radius:10px;margin:18px 0;'>

                            <div style='font-size:18px;font-weight:700;color:#1f1466;margin-bottom:8px;'>
                                {comunicado.Titulo}
                            </div>

                            <div style='font-size:14px;color:#4b5563;line-height:1.5;'>
                                {comunicado.Descripcion}
                            </div>

                            <div style='font-size:13px;color:#4b5563;margin-top:10px;'>
                                Fecha de publicación: <strong>{comunicado.FechaPublicacion:dd/MM/yyyy}</strong>
                            </div>

                        </div>

                        <p style='margin-top:24px;'>
                            <a href='{urlCorreo}'
                               style='display:inline-block;background:#1f1466;color:#ffffff;padding:12px 18px;border-radius:10px;text-decoration:none;font-weight:600;'>
                                Ver comunicado
                            </a>
                        </p>

                        <hr style='margin:28px 0;border:none;border-top:1px solid #e5e7eb;' />

                        <p style='font-size:12px;color:#6b7280;'>
                            Este correo fue enviado automáticamente desde la Intranet SEI.
                        </p>

                    </div>
                </div>";

            foreach (var correo in correos)
            {
                await _emailSender.SendEmailAsync(
                    correo,
                    "Nuevo comunicado publicado - Intranet SEI",
                    cuerpo
                );
            }
        }
    }
}