using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_FirmasAprobacionPresupuestal")]
    public class AdqFirmaAprobacionPresupuestal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
        {
            get;
            set;
        }


        public int AprobacionPresupuestalDetalleId
        {
            get;
            set;
        }


        public int? FirmaUsuarioId
        {
            get;
            set;
        }


        [Required]
        [StringLength(450)]
        public string UsuarioFirmanteId
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(250)]
        public string NombreFirmante
        {
            get;
            set;
        } = string.Empty;


        [StringLength(256)]
        public string? EmailFirmante
        {
            get;
            set;
        }


        public int OrdenEtapa
        {
            get;
            set;
        }


        [Required]
        [StringLength(150)]
        public string NombreEtapa
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(30)]
        public string TipoFirma
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(1000)]
        public string RutaFirmaSnapshot
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(128)]
        public string HashFirma
        {
            get;
            set;
        } = string.Empty;


        [StringLength(128)]
        public string? HashContextoFirmado
        {
            get;
            set;
        }


        [Required]
        [StringLength(30)]
        public string Decision
        {
            get;
            set;
        } = string.Empty;


        public DateTime FechaFirma
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


        [StringLength(500)]
        public string? UserAgent
        {
            get;
            set;
        }


        public bool Eliminado
        {
            get;
            set;
        }


        [ForeignKey(nameof(AprobacionPresupuestalDetalleId))]
        public AdqAprobacionPresupuestalDetalle
            AprobacionPresupuestalDetalle
        {
            get;
            set;
        } = null!;


        [ForeignKey(nameof(FirmaUsuarioId))]
        public AdqFirmaUsuario? FirmaUsuario
        {
            get;
            set;
        }
    }
}