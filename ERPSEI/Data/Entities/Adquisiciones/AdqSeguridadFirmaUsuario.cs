using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_SeguridadFirmaUsuario")]
    public class AdqSeguridadFirmaUsuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id
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
        [StringLength(1000)]
        public string PinHash
        {
            get;
            set;
        } = string.Empty;


        public int IntentosFallidos
        {
            get;
            set;
        }


        public DateTime? BloqueadoHasta
        {
            get;
            set;
        }


        public DateTime FechaConfiguracion
        {
            get;
            set;
        } = DateTime.Now;


        public DateTime? FechaModificacion
        {
            get;
            set;
        }


        public bool Activo
        {
            get;
            set;
        } = true;


        public bool Eliminado
        {
            get;
            set;
        }
    }
}