using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Documentos
{
    public class DocumentoVersion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DocumentoId { get; set; }

        [Required]
        [StringLength(20)]
        public string Version { get; set; } = "1.0";

        [Required]
        public int EstatusDocumentoId { get; set; }

        public DateTime? FechaPublicacion { get; set; }

        [StringLength(1000)]
        public string? Comentarios { get; set; }

        // Archivo
        [StringLength(260)]
        public string? NombreArchivo { get; set; }

        [StringLength(800)]
        public string? RutaArchivo { get; set; }

        [StringLength(100)]
        public string? MimeType { get; set; }

        public long? TamanoBytes { get; set; }

        public bool EsActual { get; set; } = false;
        public bool Activo { get; set; } = true;

        public string? CreadoPorId { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey(nameof(DocumentoId))]
        public Documento? Documento { get; set; }

        [ForeignKey(nameof(EstatusDocumentoId))]
        public EstatusDocumento? EstatusDocumento { get; set; }
    }
}
