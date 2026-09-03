using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_AprobacionesPresupuestalesDetalle")]
    public class AdqAprobacionPresupuestalDetalle
    {
        [Key]
        [DatabaseGenerated(
            DatabaseGeneratedOption.Identity
        )]
        public int Id
        {
            get;
            set;
        }


        // =========================================================
        // PROCESO PRESUPUESTAL
        // =========================================================

        public int AprobacionPresupuestalId
        {
            get;
            set;
        }


        // =========================================================
        // NIVEL / ORDEN
        // =========================================================

        public int Orden
        {
            get;
            set;
        }


        [Required]
        [StringLength(100)]
        public string TipoAprobador
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(150)]
        public string NombreEtapa
        {
            get;
            set;
        } = string.Empty;


        // =========================================================
        // RESPONSABLE
        // =========================================================

        [StringLength(450)]
        public string? UsuarioAprobadorId
        {
            get;
            set;
        }


        // =========================================================
        // ESTADO
        // =========================================================

        [Required]
        [StringLength(30)]
        public string Estatus
        {
            get;
            set;
        } = "EnEspera";


        public bool EsActual
        {
            get;
            set;
        }


        // =========================================================
        // DECISIÓN
        // =========================================================

        [StringLength(3000)]
        public string? Comentario
        {
            get;
            set;
        }


        public DateTime? FechaDecision
        {
            get;
            set;
        }


        // =========================================================
        // AUDITORÍA
        // =========================================================

        public DateTime FechaCreacion
        {
            get;
            set;
        } = DateTime.Now;


        public bool Eliminado
        {
            get;
            set;
        }


        // =========================================================
        // NAVEGACIÓN
        // =========================================================

        [ForeignKey(
            nameof(
                AprobacionPresupuestalId
            )
        )]
        public AdqAprobacionPresupuestal? AprobacionPresupuestal
        {
            get;
            set;
        }
    }
}