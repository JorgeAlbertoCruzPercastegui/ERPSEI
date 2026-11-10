using System;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Intranet
{
    public class DestinatarioNotificacionCorreoDominio
    {
        public int Id { get; set; }

        [MaxLength(450)]
        public string? UserId { get; set; }

        [MaxLength(256)]
        public string? CorreoElectronico { get; set; }

        [Required]
        [MaxLength(30)]
        public string TipoDestinatario { get; set; } = "Escalamiento";

        [MaxLength(150)]
        public string? Nombre { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [MaxLength(450)]
        public string? UsuarioCreador { get; set; }

        public DateTime? FechaModificacion { get; set; }

        [MaxLength(450)]
        public string? UsuarioModificador { get; set; }
    }
}