using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_AprobacionesPresupuestalesEventos")]
    public class AdqAprobacionPresupuestalEvento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id
        {
            get;
            set;
        }


        public int AprobacionPresupuestalId
        {
            get;
            set;
        }


        public int? AprobacionPresupuestalDetalleId
        {
            get;
            set;
        }


        [Required]
        [StringLength(100)]
        public string TipoEvento
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(500)]
        public string Descripcion
        {
            get;
            set;
        } = string.Empty;


        [StringLength(450)]
        public string? UsuarioId
        {
            get;
            set;
        }


        public int? OrdenEtapa
        {
            get;
            set;
        }


        [StringLength(150)]
        public string? NombreEtapa
        {
            get;
            set;
        }


        [StringLength(50)]
        public string? EstatusAnterior
        {
            get;
            set;
        }


        [StringLength(50)]
        public string? EstatusNuevo
        {
            get;
            set;
        }


        public DateTime FechaEvento
        {
            get;
            set;
        } = DateTime.Now;


        [StringLength(64)]
        public string? DireccionIp
        {
            get;
            set;
        }


        public bool Eliminado
        {
            get;
            set;
        }


        [ForeignKey(nameof(AprobacionPresupuestalId))]
        public AdqAprobacionPresupuestal
            AprobacionPresupuestal
        {
            get;
            set;
        } = null!;


        [ForeignKey(nameof(AprobacionPresupuestalDetalleId))]
        public AdqAprobacionPresupuestalDetalle?
            AprobacionPresupuestalDetalle
        {
            get;
            set;
        }
    }
}