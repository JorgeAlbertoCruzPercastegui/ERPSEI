
using System;
using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.TalentoHumano
{
    public class THHistorialSolicitud
    {
        public long Id { get; set; }

        public long SolicitudId { get; set; }

        public THSolicitud Solicitud { get; set; } = null!;

        public string? UsuarioId { get; set; }

        public AppUser? Usuario { get; set; }

        public string RolEjecutor { get; set; }
            = string.Empty;

        public string Accion { get; set; }
            = string.Empty;

        public EstatusSolicitudTH? EstatusAnterior
        { get; set; }

        public EstatusSolicitudTH? EstatusNuevo
        { get; set; }

        public string? Comentario { get; set; }

        public DateTime FechaHoraUtc { get; set; }

        public bool EjecutadoPorSistema { get; set; }

        public string? IpOrigen { get; set; }
    }
}
