using ERPSEI.Data.Entities.Usuarios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPSEI.Data.Entities.Intranet
{
    public class NotificacionIntranet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Titulo { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } = string.Empty;
        // Manual, Comunicado, Evento

        [StringLength(100)]
        public string? Modulo { get; set; }
        // Manuales, Comunicados Internos, Eventos

        [StringLength(300)]
        public string? Url { get; set; }

        [StringLength(80)]
        public string Icono { get; set; } = "bi bi-bell-fill";

        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        public bool Activa { get; set; } = true;

        public string? UserIdCreador { get; set; }

        [ForeignKey(nameof(UserIdCreador))]
        public AppUser? UsuarioCreador { get; set; }

        public ICollection<NotificacionIntranetUsuario> UsuariosNotificados { get; set; } = new List<NotificacionIntranetUsuario>();
    }
}