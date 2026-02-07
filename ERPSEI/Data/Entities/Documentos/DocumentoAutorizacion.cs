using ERPSEI.Data.Entities.Usuarios;
using static ERPSEI.Areas.Reportes.Pages.DocumentacionModel;

namespace ERPSEI.Data.Entities.Documentos
{
    public class DocumentoAutorizacion
    {
        public int Id { get; set; }

        public int DocumentoId { get; set; }
        public Documento Documento { get; set; } = null!;

        public RolAutorizacion Rol { get; set; }

        public string Estado { get; set; } = "PENDIENTE";

        public string? AutorizadoPorId { get; set; }
        public AppUser? AutorizadoPor { get; set; }

        public DateTime? Fecha { get; set; }
        public string? Comentario { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
