using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqAprobacion
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

        public string TipoAprobacion
        {
            get;
            set;
        } = string.Empty;

        public int Orden
        {
            get;
            set;
        }

        public string UsuarioAprobadorId
        {
            get;
            set;
        } = string.Empty;

        public AppUser UsuarioAprobador
        {
            get;
            set;
        } = null!;

        public string Estatus
        {
            get;
            set;
        } = "Pendiente";

        public string? Comentario
        {
            get;
            set;
        }

        public DateTime FechaCreacion
        {
            get;
            set;
        }

        public DateTime? FechaRespuesta
        {
            get;
            set;
        }
    }
}