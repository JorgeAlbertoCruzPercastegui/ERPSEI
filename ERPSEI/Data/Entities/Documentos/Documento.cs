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
        public int EstatusDocumentoId { get; set; }

        [Required]
        [StringLength(250)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [StringLength(150)]
        public string? Responsable { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [StringLength(300)]
        public string? Ubicacion { get; set; }

        [StringLength(300)]
        public string? NombreArchivo { get; set; }

        [StringLength(500)]
        public string? RutaArchivo { get; set; }

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

        [ForeignKey(nameof(EstatusDocumentoId))]
        public EstatusDocumento? EstatusDocumento { get; set; }
        public ICollection<DocumentoVersion>? Versiones { get; set; }
        public ICollection<DocumentoPalabraClave>? PalabrasClave { get; set; }

        public ICollection<DocumentoAutorizacion> Autorizaciones { get; set; } = new List<DocumentoAutorizacion>();


    }
}
