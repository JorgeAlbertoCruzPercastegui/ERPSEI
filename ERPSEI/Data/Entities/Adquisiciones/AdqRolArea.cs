using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    [Table("ADQ_RolesAreas")]
    public class AdqRolArea
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
        public string RolId
        {
            get;
            set;
        } = string.Empty;

        public int AreaId
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
        } = DateTime.UtcNow;

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

        [ForeignKey(nameof(RolId))]
        public AppRole Rol
        {
            get;
            set;
        } = null!;

        [ForeignKey(nameof(AreaId))]
        public Area Area
        {
            get;
            set;
        } = null!;
    }
}