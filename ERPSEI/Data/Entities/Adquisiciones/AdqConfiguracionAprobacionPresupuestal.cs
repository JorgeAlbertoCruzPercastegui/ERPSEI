using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_ConfiguracionAprobacionPresupuestal")]
    public class AdqConfiguracionAprobacionPresupuestal
    {
        [Key]
        public int Id
        {
            get;
            set;
        }


        public int Orden
        {
            get;
            set;
        }


        [Required]
        [StringLength(100)]
        public string TipoEtapa
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


        [Required]
        [StringLength(450)]
        public string UsuarioResponsableId
        {
            get;
            set;
        } = string.Empty;


        [StringLength(450)]
        public string? UsuarioAsistenteId
        {
            get;
            set;
        }


        public bool AsistenteRecibeCopia
        {
            get;
            set;
        }


        public int? RecibirCopiaDesdeOrden
        {
            get;
            set;
        }


        public bool Activo
        {
            get;
            set;
        } = true;


        public DateTime FechaCreacion
        {
            get;
            set;
        } = DateTime.Now;


        public DateTime? FechaModificacion
        {
            get;
            set;
        }


        [StringLength(450)]
        public string? UsuarioModificacionId
        {
            get;
            set;
        }


        public bool Eliminado
        {
            get;
            set;
        }
    }
}