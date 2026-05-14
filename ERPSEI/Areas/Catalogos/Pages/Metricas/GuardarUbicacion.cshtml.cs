using ERPSEI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ERPSEI.Areas.Catalogos.Pages.Metricas
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class GuardarUbicacionModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public GuardarUbicacionModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public class UbicacionRequest
        {
            public decimal Latitud { get; set; }
            public decimal Longitud { get; set; }
        }

        public async Task<IActionResult> OnPostAsync([FromBody] UbicacionRequest request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return new JsonResult(new { ok = false, mensaje = "Usuario no autenticado." });

            var actividad = await _db.IntranetActividades
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.FechaHora)
                .FirstOrDefaultAsync();

            if (actividad == null)
                return new JsonResult(new { ok = false, mensaje = "No existe actividad para actualizar." });

            actividad.Latitud = request.Latitud;
            actividad.Longitud = request.Longitud;

            await _db.SaveChangesAsync();

            return new JsonResult(new { ok = true, mensaje = "Ubicación guardada." });
        }
    }
}