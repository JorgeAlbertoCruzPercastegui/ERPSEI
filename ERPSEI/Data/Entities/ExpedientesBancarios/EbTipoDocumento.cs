using ERPSEI.Data.Entities.Documentos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    [Table("EB_TiposDocumento")]
    public class EbTipoDocumento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Categoria { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public bool EsObligatorio { get; set; } = true;

        public bool RequiereFechaVencimiento { get; set; } = false;

        public bool PermiteMultiplesArchivos { get; set; } = false;

        public int Orden { get; set; }

        public bool Deshabilitado { get; set; } = false;

        public bool Eliminado { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        [Required]
        [StringLength(450)]
        public string UsuarioCreacionId { get; set; } = string.Empty;

        public ICollection<EbDocumento> Documentos { get; set; }
            = new List<EbDocumento>();

        public ICollection<EbBitacoraDocumento> Bitacora
        {
            get;
            set;
        } = new List<EbBitacoraDocumento>();
    }
}