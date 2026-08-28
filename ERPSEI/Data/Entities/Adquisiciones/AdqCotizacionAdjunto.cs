using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_CotizacionAdjuntos")]
    public class AdqCotizacionAdjunto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get;
            set;
        }


        public int CotizacionId
        {
            get;
            set;
        }

        public int? CotizacionDetalleId
        {
            get;
            set;
        }

        [Required]
        [StringLength(300)]
        public string NombreOriginal
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(300)]
        public string NombreAlmacenado
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(500)]
        public string RutaArchivo
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(20)]
        public string Extension
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(150)]
        public string MimeType
        {
            get;
            set;
        } = string.Empty;


        public long TamanoBytes
        {
            get;
            set;
        }


        [Required]
        [StringLength(450)]
        public string UsuarioCargaId
        {
            get;
            set;
        } = string.Empty;


        public DateTime FechaCarga
        {
            get;
            set;
        }


        public bool Eliminado
        {
            get;
            set;
        }


        // =========================================================
        // NAVEGACIÓN
        // =========================================================

        [ForeignKey(nameof(CotizacionId))]
        public virtual AdqCotizacion? Cotizacion
        {
            get;
            set;
        }

        public AdqCotizacionDetalle? CotizacionDetalle
        {
            get;
            set;
        }
    }
}