using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqAsignacion
    {
        public int Id
        {
            get;
            set;
        }

        public int SolicitudId
        {
            get;
            set;
        }

        public AdqSolicitud Solicitud
        {
            get;
            set;
        } = null!;

        public string UsuarioAsignadoId
        {
            get;
            set;
        } = string.Empty;

        public AppUser UsuarioAsignado
        {
            get;
            set;
        } = null!;

        public string UsuarioAsignadorId
        {
            get;
            set;
        } = string.Empty;

        public AppUser UsuarioAsignador
        {
            get;
            set;
        } = null!;

        public DateTime FechaAsignacion
        {
            get;
            set;
        }

        public DateTime? FechaFin
        {
            get;
            set;
        }

        public bool Activa
        {
            get;
            set;
        } = true;

        public string? Observaciones
        {
            get;
            set;
        }
    }
}