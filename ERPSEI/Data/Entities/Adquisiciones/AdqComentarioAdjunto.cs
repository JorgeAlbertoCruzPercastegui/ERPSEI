namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqComentarioAdjunto
    {
        public int Id
        {
            get;
            set;
        }

        public int ComentarioId
        {
            get;
            set;
        }

        public AdqComentario Comentario
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

        public DateTime FechaCarga
        {
            get;
            set;
        }

        public bool Eliminado
        {
            get;
            set;
        }
    }
}