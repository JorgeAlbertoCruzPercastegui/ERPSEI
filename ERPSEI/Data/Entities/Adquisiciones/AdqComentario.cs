using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqComentario
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

        public string Comentario
        {
            get;
            set;
        } = string.Empty;

        public bool EsNotaInterna
        {
            get;
            set;
        }

        public DateTime FechaCreacion
        {
            get;
            set;
        }

        public bool Eliminado
        {
            get;
            set;
        }

        public ICollection<AdqComentarioAdjunto>
            Adjuntos
        {
            get;
            set;
        } = new List<AdqComentarioAdjunto>();
    }
}