using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Intranet
{
    public class ManualPoliticaIntranet
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public string Tipo { get; set; } = "Manual"; // Manual / Politica / Reglamento

        public string ModoVisualizacion { get; set; } = "Html"; // Html / Link / Pdf

        public string? CodigoHtml { get; set; }
        public string? UrlExterna { get; set; }

        public string? NombreArchivoPdf { get; set; }
        public string? RutaArchivoPdf { get; set; }

        public string? NombrePortada { get; set; }
        public string? RutaPortada { get; set; }

        public bool Activo { get; set; } = true;
        public bool Publicado { get; set; } = false;
        public int Orden { get; set; } = 1;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public string? UsuarioCreadorId { get; set; }
        public AppUser? UsuarioCreador { get; set; }
        public bool PublicacionGeneral { get; set; } = true;

        public ICollection<ManualPoliticaArea> AreasPermitidas { get; set; } = new List<ManualPoliticaArea>();
    }
}
