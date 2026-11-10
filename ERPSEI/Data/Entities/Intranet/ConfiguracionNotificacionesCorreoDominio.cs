using System;
using System.ComponentModel.DataAnnotations;

namespace ERPSEI.Data.Entities.Intranet
{
    public class ConfiguracionNotificacionesCorreoDominio
    {
        public int Id { get; set; }

        public bool Habilitado { get; set; } = true;

        [Range(1, 365)]
        public int DiasAnticipacion { get; set; } = 10;

        [Range(0, 365)]
        public int DiasEscalamiento { get; set; } = 2;

        public bool RecordatoriosDiarios { get; set; } = true;

        public bool NotificarVencidos { get; set; } = true;

        public DateTime? FechaModificacion { get; set; }

        [MaxLength(450)]
        public string? UsuarioModificador { get; set; }
    }
}