
using System;
using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.TalentoHumano
{
    public class THDocumento
    {
        public long Id { get; set; }

        public long SolicitudId { get; set; }

        public THSolicitud Solicitud { get; set; } = null!;

        public long? RespuestaId { get; set; }

        public THRespuesta? Respuesta { get; set; }

        public string NombreOriginal { get; set; }
            = string.Empty;

        public string? NombreAlmacenado { get; set; }

        public string? TipoContenido { get; set; }

        public long? TamanoBytes { get; set; }

        // Interno o OneDrive
        public string Origen { get; set; }
            = "Interno";

        // Identificadores persistentes de Microsoft Graph
        public string? DriveId { get; set; }

        public string? DriveItemId { get; set; }

        // Ubicación interna no pública
        public string? RutaAlmacenamiento { get; set; }

        public string? HashSha256 { get; set; }

        // Periodo documental, si corresponde
        public int? Anio { get; set; }

        public string? Periodo { get; set; }

        public string? Formato { get; set; }

        // Datos de trazabilidad
        public string? UsuarioCargaId { get; set; }

        public AppUser? UsuarioCarga { get; set; }

        public DateTime FechaRegistroUtc { get; set; }

        public bool EntregadoAlSolicitante { get; set; }

        public DateTime? FechaEntregaUtc { get; set; }
    }
}
