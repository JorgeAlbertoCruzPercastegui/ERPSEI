
using System;
using System.Collections.Generic;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.TalentoHumano
{
    public class THSolicitud
    {
        public long Id { get; set; }

        public string Folio { get; set; } = string.Empty;

        public TipoSolicitudTH Tipo { get; set; }

        public EstatusSolicitudTH Estatus { get; set; }
            = EstatusSolicitudTH.Enviada;

        // Identidad laboral
        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; } = null!;

        // Identidad de acceso a INTRANET
        public string SolicitanteUserId { get; set; }
            = string.Empty;

        public AppUser Solicitante { get; set; } = null!;

        // Datos históricos del colaborador
        public string NumeroEmpleado { get; set; }
            = string.Empty;

        public string NombreSolicitante { get; set; }
            = string.Empty;

        public string? AreaSolicitante { get; set; }

        public string? OficinaSolicitante { get; set; }

        // Responsable de atención
        public string? AtendidoPorUserId { get; set; }

        public AppUser? AtendidoPor { get; set; }

        // Fechas en UTC
        public DateTime FechaEnvioUtc { get; set; }

        public DateTime? FechaInicioAtencionUtc { get; set; }

        public DateTime? FechaRespuestaUtc { get; set; }

        public DateTime? FechaCierreUtc { get; set; }

        public DateTime? FechaActualizacionUtc { get; set; }

        // SLA acordado para la solicitud
        public int? DiasObjetivoSla { get; set; }

        public DateTime? FechaLimiteSlaUtc { get; set; }

        // Control de respuesta automática
        public bool RespondidaAutomaticamente { get; set; }

        // Concurrencia
        public byte[] RowVersion { get; set; }
            = Array.Empty<byte>();

        // Relaciones
        public ICollection<THHistorialSolicitud> Historial
        { get; set; } = new List<THHistorialSolicitud>();

        public ICollection<THRespuesta> Respuestas
        { get; set; } = new List<THRespuesta>();

        public ICollection<THDocumento> Documentos
        { get; set; } = new List<THDocumento>();
    }
}
