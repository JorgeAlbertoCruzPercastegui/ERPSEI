using ERPSEI.Data;
using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Areas.Catalogos.Pages.GestorManualesPoliticas
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;

        public IndexModel(
            ApplicationDbContext db,
            IWebHostEnvironment env,
            UserManager<AppUser> userManager)
        {
            _db = db;
            _env = env;
            _userManager = userManager;
        }

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
        }

        public async Task OnGetAsync(int? id)
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
        }

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

            ManualPoliticaIntranet entity;

            if (Input.Id.HasValue)
            {
                entity = await _db.ManualesPoliticasIntranet.FirstOrDefaultAsync(x => x.Id == Input.Id.Value);

                if (entity == null)
                    return NotFound();
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

            await _db.SaveChangesAsync();

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

        public async Task<IActionResult> OnPostTogglePublicadoAsync(int id)
        {
            var item = await _db.ManualesPoliticasIntranet.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            item.Publicado = !item.Publicado;
            await _db.SaveChangesAsync();

            TempData["Ok"] = "Publicación actualizada.";
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
    }
}