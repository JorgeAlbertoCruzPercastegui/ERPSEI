using ERPSEI.Data;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ERPSEI.Email;

namespace ERPSEI.Areas.Catalogos.Pages.GestorManualesPoliticas
{
    [Authorize]
    [RequestSizeLimit(104857600)]
    [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;

        public IndexModel(
            ApplicationDbContext db,
            IWebHostEnvironment env,
            UserManager<AppUser> userManager,
            IEmailSender emailSender)
        {
            _db = db;
            _env = env;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public List<Area> AreasDisponibles { get; set; } = new();
        public List<ManualPoliticaIntranet> Lista { get; set; } = new();

        public bool EsEdicion { get; set; } = false;

        [BindProperty(SupportsGet = true)]
        public int? IdEdicion { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new();


        public class InputModel
        {
            public int? Id { get; set; }

            [Required(ErrorMessage = "El título es obligatorio.")]
            [MaxLength(250)]
            public string Titulo { get; set; } = string.Empty;

            [MaxLength(1000)]
            public string? Descripcion { get; set; }

            [Required(ErrorMessage = "Selecciona un tipo.")]
            public string Tipo { get; set; } = "Manual";

            [Required(ErrorMessage = "Selecciona un modo de visualización.")]
            public string ModoVisualizacion { get; set; } = "Pdf";

            public string? CodigoHtml { get; set; }

            [MaxLength(500)]
            public string? UrlExterna { get; set; }

            public IFormFile? ArchivoPdf { get; set; }

            public IFormFile? Portada { get; set; }

            public bool Activo { get; set; } = true;
            public bool Publicado { get; set; } = false;

            public int Orden { get; set; } = 1;

            public bool PublicacionGeneral { get; set; } = true;

            public List<int> AreasSeleccionadas { get; set; } = new();
        }

        public async Task OnGetAsync(int? id)
        {
            AreasDisponibles = await _db.Areas
                .AsNoTracking()
                .OrderBy(x => x.Nombre)
                .ToListAsync();

            Lista = await _db.ManualesPoliticasIntranet
                .AsNoTracking()
                .Include(x => x.AreasPermitidas)
                    .ThenInclude(x => x.Area)
                .OrderBy(x => x.Tipo)
                .ThenBy(x => x.Orden)
                .ThenByDescending(x => x.FechaCreacion)
                .ToListAsync();

            if (id.HasValue)
            {
                var item = await _db.ManualesPoliticasIntranet
                    .AsNoTracking()
                    .Include(x => x.AreasPermitidas)
                    .FirstOrDefaultAsync(x => x.Id == id.Value);

                if (item != null)
                {
                    EsEdicion = true;

                    Input = new InputModel
                    {
                        Id = item.Id,
                        Titulo = item.Titulo,
                        Descripcion = item.Descripcion,
                        Tipo = item.Tipo,
                        ModoVisualizacion = item.ModoVisualizacion,
                        CodigoHtml = item.CodigoHtml,
                        UrlExterna = item.UrlExterna,
                        Activo = item.Activo,
                        Publicado = item.Publicado,
                        Orden = item.Orden,
                        PublicacionGeneral = item.PublicacionGeneral,
                        AreasSeleccionadas = item.AreasPermitidas
                            .Select(x => x.AreaId)
                            .ToList()
                    };
                }
            }
        }
        /*public async Task OnGetAsync(int? id)
        {
            Lista = await _db.ManualesPoliticasIntranet
                .AsNoTracking()
                .OrderBy(x => x.Tipo)
                .ThenBy(x => x.Orden)
                .ThenByDescending(x => x.FechaCreacion)
                .ToListAsync();

            if (id.HasValue)
            {
                var item = await _db.ManualesPoliticasIntranet
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id.Value);

                if (item != null)
                {
                    EsEdicion = true;

                    Input = new InputModel
                    {
                        Id = item.Id,
                        Titulo = item.Titulo,
                        Descripcion = item.Descripcion,
                        Tipo = item.Tipo,
                        ModoVisualizacion = item.ModoVisualizacion,
                        CodigoHtml = item.CodigoHtml,
                        UrlExterna = item.UrlExterna,
                        Activo = item.Activo,
                        Publicado = item.Publicado,
                        Orden = item.Orden
                    };
                }
            }
        }*/

        public async Task<IActionResult> OnPostGuardarAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(Input.Id);
                return Page();
            }

            // Validaciones por modo
            if (Input.ModoVisualizacion == "Html" && string.IsNullOrWhiteSpace(Input.CodigoHtml))
            {
                ModelState.AddModelError(string.Empty, "Debes capturar el código HTML para el modo HTML.");
                await OnGetAsync(Input.Id);
                return Page();
            }

            if (Input.ModoVisualizacion == "Link" && string.IsNullOrWhiteSpace(Input.UrlExterna))
            {
                ModelState.AddModelError(string.Empty, "Debes capturar la URL para el modo Link.");
                await OnGetAsync(Input.Id);
                return Page();
            }

            if (Input.ModoVisualizacion == "Pdf" && Input.Id == null && Input.ArchivoPdf == null)
            {
                ModelState.AddModelError(string.Empty, "Debes subir un PDF para el modo PDF.");
                await OnGetAsync(Input.Id);
                return Page();
            }

            if (!Input.PublicacionGeneral && (Input.AreasSeleccionadas == null || !Input.AreasSeleccionadas.Any()))
            {
                ModelState.AddModelError(string.Empty, "Debes seleccionar al menos un área o publicar de forma general.");
                await OnGetAsync(Input.Id);
                return Page();
            }

            ManualPoliticaIntranet entity;

            bool estabaPublicadoAntes = false;

            if (Input.Id.HasValue)
            {
                //entity = await _db.ManualesPoliticasIntranet.FirstOrDefaultAsync(x => x.Id == Input.Id.Value);
                entity = await _db.ManualesPoliticasIntranet.Include(x => x.AreasPermitidas).FirstOrDefaultAsync(x => x.Id == Input.Id.Value);

                if (entity == null)
                    return NotFound();
                estabaPublicadoAntes = entity.Publicado;
            }
            else
            {
                AppUser? usr = await _userManager.GetUserAsync(User);

                entity = new ManualPoliticaIntranet
                {
                    UsuarioCreadorId = usr?.Id
                };

                _db.ManualesPoliticasIntranet.Add(entity);
            }

            if (Input.Portada != null)
            {
                string portadaExt = Path.GetExtension(Input.Portada.FileName).ToLowerInvariant();
                string[] permitidasPortada = [".jpg", ".jpeg", ".png", ".webp"];

                if (!permitidasPortada.Contains(portadaExt))
                {
                    ModelState.AddModelError(string.Empty, "La portada debe ser JPG, PNG o WEBP.");
                    await OnGetAsync(Input.Id);
                    return Page();
                }

                string pathPortadas = Path.Combine(_env.WebRootPath, "uploads", "manuales-politicas", "portadas");
                if (!Directory.Exists(pathPortadas))
                    Directory.CreateDirectory(pathPortadas);

                string portadaSafeName = $"{Guid.NewGuid():N}{portadaExt}";
                string portadaPhysicalPath = Path.Combine(pathPortadas, portadaSafeName);

                using (var stream = new FileStream(portadaPhysicalPath, FileMode.Create))
                {
                    await Input.Portada.CopyToAsync(stream);
                }

                // Si ya tenía portada anterior y quieres borrarla:
                if (!string.IsNullOrWhiteSpace(entity.RutaPortada))
                {
                    string portadaAnterior = Path.Combine(
                        _env.WebRootPath,
                        entity.RutaPortada.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (System.IO.File.Exists(portadaAnterior))
                        System.IO.File.Delete(portadaAnterior);
                }

                entity.NombrePortada = Input.Portada.FileName;
                entity.RutaPortada = $"/uploads/manuales-politicas/portadas/{portadaSafeName}";
            }

            if (Input.ArchivoPdf != null)
            {
                string pdfExt = Path.GetExtension(Input.ArchivoPdf.FileName).ToLowerInvariant();

                if (pdfExt != ".pdf")
                {
                    ModelState.AddModelError(string.Empty, "El archivo debe ser PDF.");
                    await OnGetAsync(Input.Id);
                    return Page();
                }

                string pathPdf = Path.Combine(_env.WebRootPath, "uploads", "manuales-politicas", "pdf");
                if (!Directory.Exists(pathPdf))
                    Directory.CreateDirectory(pathPdf);

                string pdfSafeName = $"{Guid.NewGuid():N}{pdfExt}";
                string pdfPhysicalPath = Path.Combine(pathPdf, pdfSafeName);

                using (var stream = new FileStream(pdfPhysicalPath, FileMode.Create))
                {
                    await Input.ArchivoPdf.CopyToAsync(stream);
                }

                // Si ya tenía pdf anterior y quieres borrarlo:
                if (!string.IsNullOrWhiteSpace(entity.RutaArchivoPdf))
                {
                    string pdfAnterior = Path.Combine(
                        _env.WebRootPath,
                        entity.RutaArchivoPdf.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                    );

                    if (System.IO.File.Exists(pdfAnterior))
                        System.IO.File.Delete(pdfAnterior);
                }

                entity.NombreArchivoPdf = Input.ArchivoPdf.FileName;
                entity.RutaArchivoPdf = $"/uploads/manuales-politicas/pdf/{pdfSafeName}";
            }

            entity.Titulo = Input.Titulo.Trim();
            entity.Descripcion = Input.Descripcion;
            entity.Tipo = Input.Tipo;
            entity.ModoVisualizacion = Input.ModoVisualizacion;
            entity.CodigoHtml = Input.ModoVisualizacion == "Html" ? Input.CodigoHtml : null;
            entity.UrlExterna = Input.ModoVisualizacion == "Link" ? Input.UrlExterna : null;
            entity.Activo = Input.Activo;
            entity.Publicado = Input.Publicado;
            entity.Orden = Input.Orden <= 0 ? 1 : Input.Orden;

            entity.PublicacionGeneral = Input.PublicacionGeneral;

            _db.ManualPoliticaAreas.RemoveRange(entity.AreasPermitidas);

            if (!Input.PublicacionGeneral && Input.AreasSeleccionadas != null)
            {
                foreach (var areaId in Input.AreasSeleccionadas.Distinct())
                {
                    entity.AreasPermitidas.Add(new ManualPoliticaArea
                    {
                        AreaId = areaId
                    });
                }
            }

            await _db.SaveChangesAsync();

            if (entity.Publicado && !estabaPublicadoAntes)
            {
                await CrearNotificacionPublicacionManualAsync(entity);
            }

            TempData["Ok"] = Input.Id.HasValue
                ? "Registro actualizado correctamente."
                : "Registro guardado correctamente.";

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostToggleActivoAsync(int id)
        {
            var item = await _db.ManualesPoliticasIntranet.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            item.Activo = !item.Activo;
            await _db.SaveChangesAsync();

            TempData["Ok"] = "Estatus actualizado.";
            return RedirectToPage();
        }

        /*public async Task<IActionResult> OnPostTogglePublicadoAsync(int id)
        {
            var item = await _db.ManualesPoliticasIntranet.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            item.Publicado = !item.Publicado;
            await _db.SaveChangesAsync();

            TempData["Ok"] = "Publicación actualizada.";
            return RedirectToPage();
        }*/

        public async Task<IActionResult> OnPostTogglePublicadoAsync(int id)
        {
            var item = await _db.ManualesPoliticasIntranet
                .Include(x => x.AreasPermitidas)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return NotFound();

            bool publicarAhora = !item.Publicado;

            item.Publicado = publicarAhora;

            await _db.SaveChangesAsync();

            if (publicarAhora)
            {
                await CrearNotificacionPublicacionManualAsync(item);
            }

            TempData["Ok"] = publicarAhora
                ? "Documento publicado correctamente. Se notificó a los usuarios."
                : "Documento despublicado correctamente.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var item = await _db.ManualesPoliticasIntranet.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(item.RutaPortada))
            {
                string portadaPhysical = Path.Combine(
                    _env.WebRootPath,
                    item.RutaPortada.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                if (System.IO.File.Exists(portadaPhysical))
                    System.IO.File.Delete(portadaPhysical);
            }

            if (!string.IsNullOrWhiteSpace(item.RutaArchivoPdf))
            {
                string pdfPhysical = Path.Combine(
                    _env.WebRootPath,
                    item.RutaArchivoPdf.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
                );

                if (System.IO.File.Exists(pdfPhysical))
                    System.IO.File.Delete(pdfPhysical);
            }

            _db.ManualesPoliticasIntranet.Remove(item);
            await _db.SaveChangesAsync();

            TempData["Ok"] = "Registro eliminado.";
            return RedirectToPage();
        }

        private async Task CrearNotificacionPublicacionManualAsync(ManualPoliticaIntranet documento)
        {
            var usuariosQuery = _db.Users
                .Include(u => u.Empleado)
                .Where(u =>
                    !string.IsNullOrWhiteSpace(u.Email) &&
                    u.Empleado != null &&
                    u.Empleado.Deshabilitado == 0)
                .AsQueryable();

            if (!documento.PublicacionGeneral)
            {
                var areasPermitidas = await _db.ManualPoliticaAreas
                    .Where(x => x.ManualPoliticaIntranetId == documento.Id)
                    .Select(x => x.AreaId)
                    .ToListAsync();

                usuariosQuery = usuariosQuery.Where(u =>
                    u.Empleado != null &&
                    u.Empleado.AreaId.HasValue &&
                    areasPermitidas.Contains(u.Empleado.AreaId.Value));
            }

            var usuarios = await usuariosQuery.ToListAsync();

            if (!usuarios.Any())
                return;

            string tipo = documento.Tipo ?? "Documento";

            string url = Url.Page(
                "/ManualesPoliticas",
                pageHandler: null,
                values: null,
                protocol: Request.Scheme
            ) ?? "/ManualesPoliticas";

            var notificacion = new NotificacionIntranet
            {
                Titulo = $"Nuevo {tipo} publicado",
                Descripcion = documento.Titulo,
                Tipo = tipo,
                Modulo = "Manuales / Políticas / Reglamentos",
                Url = "/ManualesPoliticas",
                Icono = tipo.Equals("Manual", StringComparison.OrdinalIgnoreCase)
                    ? "bi bi-journal-bookmark-fill"
                    : tipo.Equals("Politica", StringComparison.OrdinalIgnoreCase)
                        ? "bi bi-shield-check"
                        : "bi bi-file-earmark-text-fill",
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

            string cuerpo = $@"
        <div style='font-family:Arial,sans-serif;color:#1f1466;'>
            <div style='background:#1f1466;padding:18px 22px;border-radius:14px 14px 0 0;color:#ffffff;'>
                <h2 style='margin:0;font-size:22px;'>Nuevo {tipo} publicado</h2>
            </div>

            <div style='border:1px solid #e5e7eb;border-top:0;padding:24px;border-radius:0 0 14px 14px;background:#ffffff;'>
                <p style='font-size:15px;color:#374151;'>Hola,</p>

                <p style='font-size:15px;color:#374151;line-height:1.6;'>
                    Se ha publicado un nuevo documento en la intranet corporativa de SEI.
                </p>

                <div style='background:#f8f9ff;border-left:4px solid #1f1466;padding:16px;border-radius:10px;margin:18px 0;'>
                    <div style='font-size:18px;font-weight:700;color:#1f1466;margin-bottom:8px;'>
                        {documento.Titulo}
                    </div>

                    <div style='font-size:14px;color:#4b5563;line-height:1.5;'>
                        {documento.Descripcion}
                    </div>
                </div>

                <p style='margin-top:24px;'>
                    <a href='{url}'
                       style='display:inline-block;background:#1f1466;color:#ffffff;padding:12px 18px;border-radius:10px;text-decoration:none;font-weight:600;'>
                        Ver documento
                    </a>
                </p>

                <hr style='margin:28px 0;border:none;border-top:1px solid #e5e7eb;' />

                <p style='font-size:12px;color:#6b7280;'>
                    Este correo fue enviado automáticamente desde la Intranet SEI.
                </p>
            </div>
        </div>";

            var correos = usuarios
                .Where(x => !string.IsNullOrWhiteSpace(x.Email))
                .Select(x => x.Email!)
                .Distinct()
                .ToList();

            foreach (var correo in correos)
            {
                await _emailSender.SendEmailAsync(
                    correo,
                    $"Nuevo {tipo} publicado - Intranet SEI",
                    cuerpo
                );
            }
        }

        /*private async Task CrearNotificacionPublicacionManualAsync(ManualPoliticaIntranet documento)
        {
            var usuariosQuery = _db.Users
                .Include(u => u.Empleado)
                .AsQueryable();

            if (!documento.PublicacionGeneral)
            {
                var areasPermitidas = await _db.ManualPoliticaAreas
                    .Where(x => x.ManualPoliticaIntranetId == documento.Id)
                    .Select(x => x.AreaId)
                    .ToListAsync();

                usuariosQuery = usuariosQuery.Where(u =>
                    u.Empleado != null &&
                    u.Empleado.AreaId.HasValue &&
                    areasPermitidas.Contains(u.Empleado.AreaId.Value));
            }

            var usuarios = await usuariosQuery
                .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                .ToListAsync();

            if (!usuarios.Any())
                return;

            string tipo = documento.Tipo ?? "Documento";

            string url = Url.Page(
                "/ManualesPoliticas",
                pageHandler: null,
                values: null,
                protocol: Request.Scheme
            ) ?? "/ManualesPoliticas";

            var notificacion = new NotificacionIntranet
            {
                Titulo = $"Nuevo {tipo} publicado",
                Descripcion = documento.Titulo,
                Tipo = tipo,
                Modulo = "Manuales / Políticas / Reglamentos",
                Url = "/ManualesPoliticas",
                Icono = tipo.Equals("Manual", StringComparison.OrdinalIgnoreCase)
                    ? "bi bi-journal-bookmark-fill"
                    : tipo.Equals("Politica", StringComparison.OrdinalIgnoreCase)
                        ? "bi bi-shield-check"
                        : "bi bi-file-earmark-text-fill",
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

            // CORREO DE PRUEBAS
            string correoPruebas = "jcruz@asesorcliente.com";

            string cuerpo = $@"
                <div style='font-family:Arial,sans-serif;color:#1f1466;'>

                    <div style='background:#1f1466;
                                padding:18px 22px;
                                border-radius:14px 14px 0 0;
                                color:#ffffff;'>

                        <h2 style='margin:0;font-size:22px;'>
                            Nuevo {tipo} publicado
                        </h2>

                    </div>

                    <div style='border:1px solid #e5e7eb;
                                border-top:0;
                                padding:24px;
                                border-radius:0 0 14px 14px;
                                background:#ffffff;'>

                        <p style='font-size:15px;color:#374151;'>
                            Hola,
                        </p>

                        <p style='font-size:15px;color:#374151;line-height:1.6;'>
                            Se ha publicado un nuevo documento en la intranet corporativa de SEI.
                        </p>

                        <div style='background:#f8f9ff;
                                    border-left:4px solid #1f1466;
                                    padding:16px;
                                    border-radius:10px;
                                    margin:18px 0;'>

                            <div style='font-size:18px;
                                        font-weight:700;
                                        color:#1f1466;
                                        margin-bottom:8px;'>

                                {documento.Titulo}

                            </div>

                            <div style='font-size:14px;
                                        color:#4b5563;
                                        line-height:1.5;'>

                                {documento.Descripcion}

                            </div>

                        </div>

                        <p style='margin-top:24px;'>

                            <a href='{url}'
                               style='display:inline-block;
                                      background:#1f1466;
                                      color:#ffffff;
                                      padding:12px 18px;
                                      border-radius:10px;
                                      text-decoration:none;
                                      font-weight:600;'>

                                Ver documento

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
                $"Nuevo {tipo} publicado - Intranet SEI",
                cuerpo
            );
        }*/
    }
}