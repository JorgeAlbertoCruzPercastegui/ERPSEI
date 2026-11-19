
using System;
using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.TalentoHumano
{
    public class THRespuesta
    {
        public long Id { get; set; }

        public long SolicitudId { get; set; }

        public THSolicitud Solicitud { get; set; } = null!;

        public string TipoRespuesta { get; set; }
            = "Comentario";

        // Contenido confidencial:
        // No debe enviarse a correos ni auditoría general.
        public string? Contenido { get; set; }

        public string? UsuarioRespuestaId { get; set; }

        public AppUser? UsuarioRespuesta { get; set; }

        public bool EsAutomatica { get; set; }

        public bool EsRespuestaFinal { get; set; }

        public DateTime FechaRespuestaUtc { get; set; }
    }
}
