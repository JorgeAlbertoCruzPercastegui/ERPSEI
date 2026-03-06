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

namespace ERPSEI.Areas.Catalogos.Pages.HeaderImagenes
{
    [Authorize]
    public class HeaderImagenesModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;

        public HeaderImagenesModel(
            ApplicationDbContext db,
            IWebHostEnvironment env,
            UserManager<AppUser> userManager)
        {
            _db = db;
            _env = env;
            _userManager = userManager;
        }

        public List<HeaderImagen> Lista { get; set; } = new();

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [MaxLength(80)]
            public string Temporada { get; set; } = "Principal";

            [MaxLength(150)]
            public string? Titulo { get; set; }

            [MaxLength(500)]
            public string? Descripcion { get; set; }

            public DateTime? VigenciaInicio { get; set; }
            public DateTime? VigenciaFin { get; set; }

            public bool EsPermanente { get; set; }
            public bool Activo { get; set; } = true;

            public int Orden { get; set; } = 1;

            [Required(ErrorMessage = "Selecciona una imagen.")]
            public IFormFile? Archivo { get; set; }
        }

        public async Task OnGetAsync()
        {
            Lista = await _db.HeaderImagenes
                .AsNoTracking()
                .OrderByDescending(x => x.Activo)
                .ThenBy(x => x.Orden)
                .ThenByDescending(x => x.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostCrearAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            if (!Input.EsPermanente &&
                Input.VigenciaInicio.HasValue &&
                Input.VigenciaFin.HasValue)
            {
                if (Input.VigenciaFin < Input.VigenciaInicio)
                {
                    ModelState.AddModelError("", "La vigencia fin no puede ser menor que la de inicio.");
                    await OnGetAsync();
                    return Page();
                }
            }

            if (Input.Activo)
            {
                int activos = await _db.HeaderImagenes.CountAsync(x => x.Activo);
                if (activos >= 5)
                {
                    ModelState.AddModelError("", "Máximo 5 imágenes activas permitidas.");
                    await OnGetAsync();
                    return Page();
                }
            }

            string uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "header");

            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            string ext = Path.GetExtension(Input.Archivo!.FileName).ToLowerInvariant();

            string[] allowed = { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError("", "Solo se permiten imágenes JPG, PNG o WEBP.");
                await OnGetAsync();
                return Page();
            }

            string safeFileName = $"{Guid.NewGuid():N}{ext}";

            string physicalPath = Path.Combine(uploadsRoot, safeFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await Input.Archivo.CopyToAsync(stream);
            }

            string webPath = $"/uploads/header/{safeFileName}";

            AppUser? usr = await _userManager.GetUserAsync(User);

            var entity = new HeaderImagen
            {
                Temporada = Input.Temporada.Trim(),
                Titulo = Input.Titulo,
                Descripcion = Input.Descripcion,
                NombreArchivo = Input.Archivo.FileName,
                RutaArchivo = webPath,
                VigenciaInicio = Input.EsPermanente ? null : Input.VigenciaInicio,
                VigenciaFin = Input.EsPermanente ? null : Input.VigenciaFin,
                EsPermanente = Input.EsPermanente,
                Activo = Input.Activo,
                Orden = Input.Orden <= 0 ? 1 : Input.Orden,
                UsuarioCreadorId = usr?.Id
            };

            _db.HeaderImagenes.Add(entity);

            await _db.SaveChangesAsync();

            TempData["Ok"] = "Imagen guardada correctamente.";

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleActivoAsync(int id)
        {
            var item = await _db.HeaderImagenes.FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return NotFound();

            if (!item.Activo)
            {
                int activos = await _db.HeaderImagenes.CountAsync(x => x.Activo);

                if (activos >= 5)
                {
                    TempData["Err"] = "Solo se permiten máximo 5 imágenes activas.";
                    return RedirectToPage();
                }
            }

            item.Activo = !item.Activo;

            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var item = await _db.HeaderImagenes.FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                return NotFound();

            string physical = Path.Combine(
                _env.WebRootPath,
                item.RutaArchivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (System.IO.File.Exists(physical))
                System.IO.File.Delete(physical);

            _db.HeaderImagenes.Remove(item);

            await _db.SaveChangesAsync();

            TempData["Ok"] = "Imagen eliminada.";

            return RedirectToPage();
        }
    }
}