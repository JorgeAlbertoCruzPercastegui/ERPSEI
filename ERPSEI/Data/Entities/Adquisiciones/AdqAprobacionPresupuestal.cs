using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_AprobacionesPresupuestales")]
    public class AdqAprobacionPresupuestal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SolicitudId { get; set; }

        public int CotizacionId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MontoSolicitado { get; set; }

        [Required]
        [StringLength(450)]
        public string UsuarioSolicitaId { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; }

        [StringLength(450)]
        public string? UsuarioAprobadorId { get; set; }

        public DateTime? FechaRespuesta { get; set; }

        [Required]
        [StringLength(50)]
        public string Estatus { get; set; } = "Pendiente";

        [StringLength(3000)]
        public string? ComentarioSolicitud { get; set; }

        [StringLength(3000)]
        public string? ComentarioRespuesta { get; set; }

        public bool Eliminado { get; set; }

        public AdqSolicitud Solicitud { get; set; } = null!;

        public AdqCotizacion Cotizacion { get; set; } = null!;

        public ICollection<AdqAprobacionPresupuestalDetalle>
        Detalles
            {
                get;
                set;
            } =
        new List<AdqAprobacionPresupuestalDetalle>();
    }
}