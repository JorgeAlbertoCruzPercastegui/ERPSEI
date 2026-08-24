using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqHistorial
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

        public string UsuarioId
        {
            get;
            set;
        } = string.Empty;

        public AppUser Usuario
        {
            get;
            set;
        } = null!;

        public string TipoEvento
        {
            get;
            set;
        } = string.Empty;

        public string Descripcion
        {
            get;
            set;
        } = string.Empty;

        public int? EstatusAnteriorId
        {
            get;
            set;
        }

        public int? EstatusNuevoId
        {
            get;
            set;
        }

        public DateTime FechaEvento
        {
            get;
            set;
        }

        public string? DireccionIp
        {
            get;
            set;
        }
    }
}