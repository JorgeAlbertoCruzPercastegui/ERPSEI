using ERPSEI.Data.Managers.Intranet;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPSEI.Areas.Intranet.Pages.Eventos
{
    public class IndexModel : PageModel
    {
        private readonly IEventoIntranetManager _eventoManager;
        private readonly AppUserManager _userManager;

        public IndexModel(IEventoIntranetManager eventoManager, AppUserManager userManager)
        {
            _eventoManager = eventoManager;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public int? OpenId { get; set; }
        public string NombreColaborador { get; set; } = "colaborador";
        public List<EventoVm> Eventos { get; set; } = new();

        public class EventoVm
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = string.Empty;
            public string? Descripcion { get; set; }
            public string? RutaPortada { get; set; }
            public string? TextoBoton { get; set; }
        }

        public async Task OnGetAsync()
        {
            var usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);
            if (usr != null)
            {
                NombreColaborador = usr.NormalizedUserName ?? usr.UserName ?? "colaborador";
            }

            var lista = await _eventoManager.GetPublicadosAsync();

            Eventos = lista.Select(x => new EventoVm
            {
                Id = x.Id,
                Titulo = x.Titulo,
                Descripcion = x.Descripcion,
                RutaPortada = x.RutaPortada,
                TextoBoton = string.IsNullOrWhiteSpace(x.TextoBoton) ? "Consulta aquí" : x.TextoBoton
            }).ToList();
        }

        public async Task<IActionResult> OnGetDetalleAsync(int id)
        {
            var entity = await _eventoManager.GetByIdAsync(id);

            if (entity == null || !entity.Activo || !entity.Publicado)
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
                fechaEvento = entity.FechaEvento.ToString("dd/MM/yyyy"),
                horaEvento = entity.HoraEvento.HasValue ? entity.HoraEvento.Value.ToString(@"hh\:mm") : "",
                rutaPortada = entity.RutaPortada,
                urlFormulario = entity.UrlFormulario,
                textoBoton = string.IsNullOrWhiteSpace(entity.TextoBoton) ? "Consulta aquí" : entity.TextoBoton
            });
        }
    }
}