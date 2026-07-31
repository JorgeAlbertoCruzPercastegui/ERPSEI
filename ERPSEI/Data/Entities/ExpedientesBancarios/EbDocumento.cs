using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    [Table("EB_Documentos")]
    public class EbDocumento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int EmpresaId { get; set; }

        public int TipoDocumentoId { get; set; }

        [Required]
        [StringLength(300)]
        public string NombreOriginal { get; set; } = string.Empty;

        [Required]
        [StringLength(300)]
        public string NombreAlmacenado { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string RutaArchivo { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Extension { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string MimeType { get; set; } = string.Empty;

        public long TamanoBytes { get; set; }

        public int Version { get; set; } = 1;

        public DateTime FechaCarga { get; set; } = DateTime.Now;

        public DateTime? FechaVencimiento { get; set; }

        [StringLength(50)]
        public string Estado { get; set; } = "Vigente";

        [StringLength(1000)]
        public string? Observaciones { get; set; }

        public bool EsVersionActual { get; set; } = true;

        public bool Eliminado { get; set; } = false;

        public DateTime? FechaEliminacion { get; set; }

        [Required]
        [StringLength(450)]
        public string UsuarioCargaId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? UsuarioEliminacionId { get; set; }

        public EbEmpresa? Empresa { get; set; }

        public EbTipoDocumento? TipoDocumento { get; set; }

        public ICollection<EbBitacoraDocumento> Bitacora
        {
            get;
            set;
        } = new List<EbBitacoraDocumento>();
    }
}