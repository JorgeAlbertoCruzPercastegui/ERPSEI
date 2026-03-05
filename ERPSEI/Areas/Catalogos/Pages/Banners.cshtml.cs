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

namespace ERPSEI.Areas.Catalogos.Pages.Banners
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<AppUser> _userManager;

        public IndexModel(ApplicationDbContext db, IWebHostEnvironment env, UserManager<AppUser> userManager)
        {
            _db = db;
            _env = env;
            _userManager = userManager;
        }

        public List<Banner> Lista { get; set; } = new();

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [MaxLength(150)]
            public string? Titulo { get; set; }

            [MaxLength(500)]
            public string? Descripcion { get; set; }

            public DateTime? VigenciaInicio { get; set; }
            public DateTime? VigenciaFin { get; set; }

            public bool EsPermanente { get; set; }

            public int Orden { get; set; } = 1;

            [Required(ErrorMessage = "Selecciona una imagen.")]
            public IFormFile? Archivo { get; set; }
        }

        public async Task OnGetAsync()
        {
            Lista = await _db.Banners
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

            // Validación básica de vigencias
            if (!Input.EsPermanente && Input.VigenciaInicio.HasValue && Input.VigenciaFin.HasValue)
            {
                if (Input.VigenciaFin.Value < Input.VigenciaInicio.Value)
                {
                    ModelState.AddModelError(string.Empty, "La vigencia fin no puede ser menor a la vigencia inicio.");
                    await OnGetAsync();
                    return Page();
                }
            }

            // Guardar archivo físico en wwwroot/uploads/banners
            string uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "banners");
            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            string ext = Path.GetExtension(Input.Archivo!.FileName).ToLowerInvariant();
            string[] allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError(string.Empty, "Solo se permiten imágenes JPG, PNG o WEBP.");
                await OnGetAsync();
                return Page();
            }

            string safeFileName = $"{Guid.NewGuid():N}{ext}";
            string physicalPath = Path.Combine(uploadsRoot, safeFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await Input.Archivo.CopyToAsync(stream);
            }

            // Ruta web pública
            string webPath = $"/uploads/banners/{safeFileName}";

            // Guardar en BD
            AppUser? usr = await _userManager.GetUserAsync(User);

            var banner = new Banner
            {
                Titulo = Input.Titulo,
                Descripcion = Input.Descripcion,
                NombreArchivo = Input.Archivo.FileName,
                RutaArchivo = webPath,
                VigenciaInicio = Input.EsPermanente ? null : Input.VigenciaInicio,
                VigenciaFin = Input.EsPermanente ? null : Input.VigenciaFin,
                EsPermanente = Input.EsPermanente,
                Activo = true,
                Orden = Input.Orden <= 0 ? 1 : Input.Orden,
                UsuarioCreadorId = usr?.Id
            };

            _db.Banners.Add(banner);
            await _db.SaveChangesAsync();

            TempData["Ok"] = "Banner creado correctamente.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleActivoAsync(int id)
        {
            var b = await _db.Banners.FirstOrDefaultAsync(x => x.Id == id);
            if (b == null) return NotFound();

            b.Activo = !b.Activo;
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            var b = await _db.Banners.FirstOrDefaultAsync(x => x.Id == id);
            if (b == null) return NotFound();

            // borrar archivo físico si existe
            string physical = Path.Combine(
                _env.WebRootPath,
                b.RutaArchivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (System.IO.File.Exists(physical))
                System.IO.File.Delete(physical);

            _db.Banners.Remove(b);
            await _db.SaveChangesAsync();

            TempData["Ok"] = "Banner eliminado.";
            return RedirectToPage();
        }
    }
}