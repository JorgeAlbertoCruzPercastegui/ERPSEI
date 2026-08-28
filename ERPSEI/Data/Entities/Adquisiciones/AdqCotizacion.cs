using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_Cotizaciones")]
    public class AdqCotizacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get;
            set;
        }


        public int SolicitudId
        {
            get;
            set;
        }


        public int? ProveedorId
        {
            get;
            set;
        }


        [Required]
        [StringLength(250)]
        public string NombreProveedor
        {
            get;
            set;
        } = string.Empty;


        [StringLength(50)]
        public string? RfcProveedor
        {
            get;
            set;
        }


        [StringLength(250)]
        public string? ContactoProveedor
        {
            get;
            set;
        }


        [StringLength(250)]
        [EmailAddress]
        public string? EmailProveedor
        {
            get;
            set;
        }


        [StringLength(50)]
        public string? TelefonoProveedor
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal
        {
            get;
            set;
        }


        public bool AplicaIva
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(5,2)")]
        public decimal PorcentajeIva
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal ImporteIva
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Total
        {
            get;
            set;
        }


        [StringLength(3000)]
        public string? Observaciones
        {
            get;
            set;
        }


        public bool EsPrincipal
        {
            get;
            set;
        }


        public bool Finalizada
        {
            get;
            set;
        }


        public bool Eliminado
        {
            get;
            set;
        }


        [Required]
        [StringLength(450)]
        public string UsuarioCreadorId
        {
            get;
            set;
        } = string.Empty;


        public DateTime FechaCreacion
        {
            get;
            set;
        }


        public DateTime? FechaModificacion
        {
            get;
            set;
        }


        public DateTime? FechaFinalizacion
        {
            get;
            set;
        }


        // =========================================================
        // NAVEGACIÓN
        // =========================================================

        [ForeignKey(nameof(SolicitudId))]
        public virtual AdqSolicitud? Solicitud
        {
            get;
            set;
        }


        public virtual ICollection<AdqCotizacionDetalle> Detalles
        {
            get;
            set;
        } = new List<AdqCotizacionDetalle>();


        public virtual ICollection<AdqCotizacionAdjunto> Adjuntos
        {
            get;
            set;
        } = new List<AdqCotizacionAdjunto>();
    }
}