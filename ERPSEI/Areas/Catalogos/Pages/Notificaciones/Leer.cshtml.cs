using ERPSEI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Pages.Notificaciones
{
    [Authorize]
    public class LeerModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public LeerModel(ApplicationDbContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int id, string? returnUrl)
        {
            var userId = _userManager.GetUserId(User);

            var notificacionUsuario = await _db.NotificacionesIntranetUsuarios
                .FirstOrDefaultAsync(x =>
                    x.NotificacionIntranetId == id &&
                    x.UserId == userId);

            if (notificacionUsuario != null && !notificacionUsuario.Leida)
            {
                notificacionUsuario.Leida = true;
                notificacionUsuario.FechaLectura = DateTime.Now;

                await _db.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToPage("/Index");
        }
    }
}