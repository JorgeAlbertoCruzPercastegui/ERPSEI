namespace ERPSEI.Data.Entities.ExpedientesBancarios
{
    public class EbPermisoComplianceUsuario
    {
        public int Id
        {
            get;
            set;
        }

        /*
         * Id del usuario de ASP.NET Identity.
         */
        public string UsuarioId
        {
            get;
            set;
        } = string.Empty;

        /*
         * Permite consultar empresas, accionistas,
         * documentos e historial documental.
         */
        public bool PuedeVisualizar
        {
            get;
            set;
        }

        /*
         * Permite crear empresas, accionistas
         * y cargar documentos o nuevas versiones.
         */
        public bool PuedeCrearCargar
        {
            get;
            set;
        }

        /*
         * Permite modificar empresas,
         * accionistas y datos documentales editables.
         */
        public bool PuedeModificar
        {
            get;
            set;
        }

        /*
         * Permite eliminar empresas, accionistas
         * y documentos.
         */
        public bool PuedeEliminar
        {
            get;
            set;
        }

        /*
         * Permite descargar documentos.
         */
        public bool PuedeDescargar
        {
            get;
            set;
        }

        /*
         * Control administrativo.
         */
        public DateTime FechaCreacion
        {
            get;
            set;
        } = DateTime.Now;

        public DateTime? FechaModificacion
        {
            get;
            set;
        }

        public string? UsuarioModificacionId
        {
            get;
            set;
        }
    }
}