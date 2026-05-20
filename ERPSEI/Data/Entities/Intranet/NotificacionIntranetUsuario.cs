using ERPSEI.Data.Entities.Usuarios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Intranet
{
    public class NotificacionIntranetUsuario
    {
        [Key]
        public int Id { get; set; }

        public int NotificacionIntranetId { get; set; }

        [ForeignKey(nameof(NotificacionIntranetId))]
        public NotificacionIntranet Notificacion { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public AppUser Usuario { get; set; } = null!;

        public bool Leida { get; set; } = false;

        public DateTime? FechaLectura { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}