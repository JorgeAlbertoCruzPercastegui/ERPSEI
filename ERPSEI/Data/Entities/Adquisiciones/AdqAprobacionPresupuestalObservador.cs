using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_AprobacionesPresupuestalesObservadores")]
    public class AdqAprobacionPresupuestalObservador
    {
        [Key]
        public int Id
        {
            get;
            set;
        }


        public int AprobacionPresupuestalId
        {
            get;
            set;
        }


        [Required]
        [StringLength(450)]
        public string UsuarioId
        {
            get;
            set;
        } = string.Empty;


        [Required]
        [StringLength(100)]
        public string TipoObservador
        {
            get;
            set;
        } = "Asistente";


        [Required]
        [StringLength(150)]
        public string NombreOrigen
        {
            get;
            set;
        } = string.Empty;


        public int OrdenActivacion
        {
            get;
            set;
        }


        public bool Activo
        {
            get;
            set;
        }


        public DateTime? FechaActivacion
        {
            get;
            set;
        }


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


        public AdqAprobacionPresupuestal AprobacionPresupuestal
        {
            get;
            set;
        } = null!;
    }
}