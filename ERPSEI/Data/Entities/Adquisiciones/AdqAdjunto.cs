using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqAdjunto
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

        public string NombreOriginal
        {
            get;
            set;
        } = string.Empty;

        public string NombreGuardado
        {
            get;
            set;
        } = string.Empty;

        public string RutaArchivo
        {
            get;
            set;
        } = string.Empty;

        public string? Extension
        {
            get;
            set;
        }

        public string? MimeType
        {
            get;
            set;
        }

        public long TamanoBytes
        {
            get;
            set;
        }

        public string UsuarioCargaId
        {
            get;
            set;
        } = string.Empty;

        public AppUser UsuarioCarga
        {
            get;
            set;
        } = null!;

        public DateTime FechaCarga
        {
            get;
            set;
        }

        public string TipoDocumento
        {
            get;
            set;
        } = "General";

        public bool Eliminado
        {
            get;
            set;
        }
    }
}