using ERPSEI.Data.Entities.Documentos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    [Table("EB_Empresas")]
    public class EbEmpresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string RazonSocial { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string NombreCorto { get; set; } = string.Empty;

        [Required]
        [StringLength(13)]
        public string Rfc { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Nivel { get; set; }

        [StringLength(500)]
        public string? ActividadComercial { get; set; }

        [StringLength(30)]
        public string? TelefonoBancos { get; set; }

        [StringLength(200)]
        [EmailAddress]
        public string? CorreoBancos { get; set; }

        public DateTime? FechaConstitucion { get; set; }

        [StringLength(200)]
        public string? NumeroEscritura { get; set; }

        [StringLength(500)]
        public string? DomicilioFiscal { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }

        public bool Deshabilitado { get; set; } = false;

        public bool Eliminado { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        [Required]
        [StringLength(450)]
        public string UsuarioCreacionId { get; set; } = string.Empty;

        [StringLength(450)]
        public string? UsuarioActualizacionId { get; set; }

        public ICollection<EbAccionista> Accionistas { get; set; }
            = new List<EbAccionista>();

        public ICollection<EbDocumento> Documentos { get; set; }
            = new List<EbDocumento>();

        public ICollection<EbBitacoraDocumento> BitacoraDocumental
        {
            get;
            set;
        } = new List<EbBitacoraDocumento>();
    }
}