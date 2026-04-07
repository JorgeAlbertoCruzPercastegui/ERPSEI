using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Intranet;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPSEI.Areas.Catalogos.Pages.GestorEventos
{
    public class IndexModel : PageModel
    {
        private readonly IEventoIntranetManager _eventoManager;
        private readonly IWebHostEnvironment _environment;
        private readonly AppUserManager _userManager;

        public IndexModel(
            IEventoIntranetManager eventoManager,
            IWebHostEnvironment environment,
            AppUserManager userManager)
        {
            _eventoManager = eventoManager;
            _environment = environment;
            _userManager = userManager;
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
        }

        public void OnGet() { }

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

        public async Task<IActionResult> OnPostPublicarAsync(int id)
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            bool ok = await _eventoManager.PublicarAsync(id, usr?.Id);

            return new JsonResult(new
            {
                tieneError = !ok,
                mensaje = ok ? "Evento publicado correctamente." : "No se pudo publicar el evento."
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
    }
}