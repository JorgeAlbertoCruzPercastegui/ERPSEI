using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Documentos
{
    public class DocumentoPalabraClave
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DocumentoId { get; set; }

        [Required]
        [StringLength(80)]
        public string Palabra { get; set; } = string.Empty;

        [ForeignKey(nameof(DocumentoId))]
        public Documento? Documento { get; set; }
    }
}
