using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqPermisoUsuario
    {
        public int Id
        {
            get;
            set;
        }

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

        public bool PuedeVisualizar
        {
            get;
            set;
        }

        public bool PuedeCrearSolicitud
        {
            get;
            set;
        }

        public bool PuedeGestionarSolicitudes
        {
            get;
            set;
        }

        public bool PuedeAprobar
        {
            get;
            set;
        }

        public bool PuedeAsignar
        {
            get;
            set;
        }

        public bool PuedeCotizar
        {
            get;
            set;
        }

        public bool PuedeGestionarProveedores
        {
            get;
            set;
        }

        public bool PuedeGenerarSolicitudPago
        {
            get;
            set;
        }

        public bool PuedeVerReportes
        {
            get;
            set;
        }

        public bool PuedeAdministrar
        {
            get;
            set;
        }

        public DateTime FechaCreacion
        {
            get;
            set;
        }

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