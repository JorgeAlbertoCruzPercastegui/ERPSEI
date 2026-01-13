using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Documentos
{
    public class EstatusDocumento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Nombre { get; set; } = string.Empty;

        public bool EsPublicable { get; set; } = false;

        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<DocumentoVersion>? Versiones { get; set; }
    }
}
