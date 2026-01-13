using ERPSEI.Data.Entities.Empleados;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Documentos
{
    public class Documento
    {
        [Key]
        public int Id { get; set; }

        // FK a tu Area existente (ERPSEI.Data.Entities.Empleados.Area)
        [Required]
        public int AreaId { get; set; }

        [Required]
        public int TipoDocumentoId { get; set; }

        [Required]
        [StringLength(250)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string? CreadoPorId { get; set; }
        public string? ModificadoPorId { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Navegación
        [ForeignKey(nameof(AreaId))]
        public Area? Area { get; set; }

        [ForeignKey(nameof(TipoDocumentoId))]
        public TipoDocumento? TipoDocumento { get; set; }

        public ICollection<DocumentoVersion>? Versiones { get; set; }
        public ICollection<DocumentoPalabraClave>? PalabrasClave { get; set; }
    }
}
