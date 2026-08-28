using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_CotizacionDetalles")]
    public class AdqCotizacionDetalle
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


        [Required]
        [StringLength(500)]
        public string ProductoServicio
        {
            get;
            set;
        } = string.Empty;


        [StringLength(2000)]
        public string? Descripcion
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,4)")]
        public decimal Cantidad
        {
            get;
            set;
        }


        [Required]
        [StringLength(100)]
        public string Unidad
        {
            get;
            set;
        } = string.Empty;


        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario
        {
            get;
            set;
        }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Importe
        {
            get;
            set;
        }


        public int Orden
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

        public ICollection<AdqCotizacionAdjunto> Adjuntos
        {
            get;
            set;
        } = new List<AdqCotizacionAdjunto>();
    }
}