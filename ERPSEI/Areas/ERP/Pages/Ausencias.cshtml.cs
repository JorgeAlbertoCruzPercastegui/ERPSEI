using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPSEI.Areas.ERP.Pages
{
    [Authorize]
    public class AusenciasModel : PageModel
    {
        [BindProperty]
        public InputFiltroAusenciasModel InputFiltro { get; set; } = new();

        public class InputFiltroAusenciasModel
        {
            public string? Empleado { get; set; }
            public string? Tipo { get; set; }
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
        }

        public void OnGet()
        {
        }

        public JsonResult OnGetAusenciasDias()
        {
            var data = new List<object>();

            return new JsonResult(data);
        }

        public JsonResult OnGetAusenciasHoras()
        {
            var data = new List<object>
            {
                new
                {
                    id = 1,
                    tipo = "Permiso llegada tardía",
                    fechaInicio = "29-01-2026",
                    horaInicio = "09:00 hrs.",
                    horaTermino = "13:00 hrs.",
                    horas = "04:00 hrs.",
                    estado = "Aprobado"
                }
            };

            return new JsonResult(data);
        }

        public JsonResult OnPostGuardarInasistencia()
        {
            return new JsonResult(new { tieneError = false, mensaje = "Inasistencia guardada correctamente." });
        }

        public JsonResult OnPostGuardarIncapacidad()
        {
            return new JsonResult(new { tieneError = false, mensaje = "Incapacidad guardada correctamente." });
        }

        public JsonResult OnPostGuardarPermiso()
        {
            return new JsonResult(new { tieneError = false, mensaje = "Permiso guardado correctamente." });
        }

        public JsonResult OnPostSolicitarPermiso()
        {
            return new JsonResult(new { tieneError = false, mensaje = "Permiso solicitado correctamente." });
        }
    }
}