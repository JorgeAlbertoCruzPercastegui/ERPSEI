using ERPSEI.Data.Entities.Intranet;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Intranet;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPSEI.Areas.Intranet.Pages.ComunicadosInternos
{
    public class IndexModel : PageModel
    {
        private readonly IComunicadoInternoManager _comunicadoManager;
        private readonly AppUserManager _userManager;

        public IndexModel(
            IComunicadoInternoManager comunicadoManager,
            AppUserManager userManager)
        {
            _comunicadoManager = comunicadoManager;
            _userManager = userManager;
        }

        public string NombreColaborador { get; set; } = "colaborador";

        [BindProperty(SupportsGet = true)]
        public int? Mes { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? OpenId { get; set; }

        public List<ComunicadoCardVm> Comunicados { get; set; } = new();

        public class ComunicadoCardVm
        {
            public int Id { get; set; }
            public string Titulo { get; set; } = string.Empty;
            public string FechaPublicacion { get; set; } = string.Empty;
            public string? RutaArchivo { get; set; }
            public string? RutaPortada { get; set; }
            public bool EsPdf { get; set; }
            public bool EsPermanente { get; set; }
        }

        public async Task OnGetAsync()
        {
            AppUser? usr = await _userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);

            if (usr != null)
            {
                string nombre = usr.NormalizedUserName ?? usr.UserName ?? "colaborador";
                NombreColaborador = nombre;
            }

            var lista = await _comunicadoManager.GetPublicadosVisiblesAsync(Mes);

            Comunicados = lista.Select(x => new ComunicadoCardVm
            {
                Id = x.Id,
                Titulo = x.Titulo,
                FechaPublicacion = x.FechaPublicacion.ToString("dd/MM/yyyy"),
                RutaArchivo = x.RutaArchivo,
                RutaPortada = !string.IsNullOrWhiteSpace(x.RutaPortada) ? x.RutaPortada : x.RutaArchivo,
                EsPdf = (x.ExtensionArchivo ?? "").Equals(".pdf", StringComparison.OrdinalIgnoreCase),
                EsPermanente = x.EsPermanente
            }).ToList();
        }

        public async Task<IActionResult> OnGetListaAsync(int? mes)
        {
            var lista = await _comunicadoManager.GetPublicadosVisiblesAsync(mes);

            var result = lista.Select(x => new
            {
                id = x.Id,
                titulo = x.Titulo,
                fechaPublicacion = x.FechaPublicacion.ToString("dd/MM/yyyy"),
                rutaArchivo = x.RutaArchivo,
                rutaPortada = !string.IsNullOrWhiteSpace(x.RutaPortada) ? x.RutaPortada : x.RutaArchivo,
                esPdf = (x.ExtensionArchivo ?? "").Equals(".pdf", StringComparison.OrdinalIgnoreCase),
                esPermanente = x.EsPermanente
            });

            return new JsonResult(result);
        }

        public async Task<IActionResult> OnGetDetalleAsync(int id)
        {
            var entity = await _comunicadoManager.GetByIdAsync(id);

            if (entity == null || !entity.Activo || !entity.Publicado)
            {
                return new JsonResult(new
                {
                    tieneError = true,
                    mensaje = "No se encontró el comunicado."
                });
            }

            return new JsonResult(new
            {
                tieneError = false,
                id = entity.Id,
                titulo = entity.Titulo,
                descripcion = entity.Descripcion,
                fechaPublicacion = entity.FechaPublicacion.ToString("dd/MM/yyyy"),
                horaPublicacion = entity.HoraPublicacion.HasValue ? entity.HoraPublicacion.Value.ToString(@"hh\:mm") : "",
                rutaArchivo = entity.RutaArchivo,
                rutaPortada = entity.RutaPortada,
                esPdf = (entity.ExtensionArchivo ?? "").Equals(".pdf", StringComparison.OrdinalIgnoreCase),
                esPermanente = entity.EsPermanente
            });
        }
    }
}