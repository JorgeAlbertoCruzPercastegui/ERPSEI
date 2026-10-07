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

        public LeerModel(
            ApplicationDbContext db,
            UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // =========================================================
        // LEER NOTIFICACIÓN Y REDIRIGIR
        // GET
        // =========================================================

        public async Task<IActionResult> OnGetAsync(
            int id,
            string? returnUrl)
        {
            var userId =
                _userManager.GetUserId(User);

            var notificacionUsuario =
                await _db.NotificacionesIntranetUsuarios
                    .FirstOrDefaultAsync(x =>
                        x.NotificacionIntranetId == id &&
                        x.UserId == userId
                    );

            if (
                notificacionUsuario != null &&
                !notificacionUsuario.Leida
            )
            {
                notificacionUsuario.Leida = true;
                notificacionUsuario.FechaLectura =
                    DateTime.Now;

                await _db.SaveChangesAsync();
            }

            if (
                !string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl)
            )
            {
                return LocalRedirect(
                    returnUrl
                );
            }

            return RedirectToPage(
                "/Index"
            );
        }

        // =========================================================
        // MARCAR UNA NOTIFICACIÓN COMO LEÍDA
        // POST ?handler=MarcarLeida&id=1
        // =========================================================

        public async Task<IActionResult>
            OnPostMarcarLeidaAsync(int id)
        {
            var userId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "No fue posible identificar al usuario."
                })
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }

            if (id <= 0)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "La notificación no es válida."
                })
                {
                    StatusCode =
                        StatusCodes.Status400BadRequest
                };
            }

            var notificacionUsuario =
                await _db.NotificacionesIntranetUsuarios
                    .FirstOrDefaultAsync(x =>
                        x.NotificacionIntranetId == id &&
                        x.UserId == userId
                    );

            if (notificacionUsuario == null)
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "No se encontró la notificación."
                })
                {
                    StatusCode =
                        StatusCodes.Status404NotFound
                };
            }

            if (!notificacionUsuario.Leida)
            {
                notificacionUsuario.Leida = true;
                notificacionUsuario.FechaLectura =
                    DateTime.Now;

                await _db.SaveChangesAsync();
            }

            int totalNoLeidas =
                await _db.NotificacionesIntranetUsuarios
                    .AsNoTracking()
                    .CountAsync(x =>
                        x.UserId == userId &&
                        !x.Leida
                    );

            return new JsonResult(new
            {
                success = true,
                totalNoLeidas
            });
        }

        // =========================================================
        // MARCAR TODAS LAS NOTIFICACIONES COMO LEÍDAS
        // POST ?handler=MarcarTodasLeidas
        // =========================================================

        public async Task<IActionResult>
            OnPostMarcarTodasLeidasAsync()
        {
            var userId =
                _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return new JsonResult(new
                {
                    success = false,
                    message =
                        "No fue posible identificar al usuario."
                })
                {
                    StatusCode =
                        StatusCodes.Status401Unauthorized
                };
            }

            var notificaciones =
                await _db.NotificacionesIntranetUsuarios
                    .Where(x =>
                        x.UserId == userId &&
                        !x.Leida
                    )
                    .ToListAsync();

            if (notificaciones.Count > 0)
            {
                DateTime ahora =
                    DateTime.Now;

                foreach (
                    var notificacion
                    in notificaciones
                )
                {
                    notificacion.Leida = true;
                    notificacion.FechaLectura =
                        ahora;
                }

                await _db.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                success = true,
                totalNoLeidas = 0
            });
        }
    }
}