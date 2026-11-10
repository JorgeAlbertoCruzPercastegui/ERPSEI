using System;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Intranet
{
    public class HistorialNotificacionCorreoDominio
    {
        public long Id { get; set; }

        public int CorreoDominioId { get; set; }

        public DateTime FechaCaducacion { get; set; }

        public DateTime FechaAviso { get; set; }

        [Required]
        [MaxLength(256)]
        public string DestinatarioClave { get; set; } = "";

        [MaxLength(256)]
        public string? CorreoElectronico { get; set; }

        [Required]
        [MaxLength(30)]
        public string TipoAviso { get; set; } = "";

        [Required]
        [MaxLength(30)]
        public string Canal { get; set; } = "Correo";

        [Required]
        [MaxLength(30)]
        public string Estado { get; set; } = "Pendiente";

        public int Intentos { get; set; }

        public DateTime? FechaEnvio { get; set; }

        [MaxLength(2000)]
        public string? Error { get; set; }
    }
}