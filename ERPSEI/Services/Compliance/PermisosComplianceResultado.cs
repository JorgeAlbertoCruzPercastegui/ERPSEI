namespace ERPSEI.Services.Compliance
{
    public class PermisosComplianceResultado
    {
        public bool TieneAccesoModulo
        {
            get;
            set;
        }

        public bool EsAdministrador
        {
            get;
            set;
        }

        public bool PuedeAdministrarPermisos
        {
            get;
            set;
        }

        public bool PuedeVisualizar
        {
            get;
            set;
        }

        public bool PuedeCrearCargar
        {
            get;
            set;
        }

        public bool PuedeModificar
        {
            get;
            set;
        }

        public bool PuedeEliminar
        {
            get;
            set;
        }

        public bool PuedeDescargar
        {
            get;
            set;
        }

        public static PermisosComplianceResultado SinAcceso()
        {
            return new PermisosComplianceResultado
            {
                TieneAccesoModulo = false,
                EsAdministrador = false,
                PuedeAdministrarPermisos = false,
                PuedeVisualizar = false,
                PuedeCrearCargar = false,
                PuedeModificar = false,
                PuedeEliminar = false,
                PuedeDescargar = false
            };
        }

        public static PermisosComplianceResultado AccesoTotal()
        {
            return new PermisosComplianceResultado
            {
                TieneAccesoModulo = true,
                EsAdministrador = true,
                PuedeAdministrarPermisos = true,
                PuedeVisualizar = true,
                PuedeCrearCargar = true,
                PuedeModificar = true,
                PuedeEliminar = true,
                PuedeDescargar = true
            };
        }

        public static PermisosComplianceResultado
            AccesoVisualizacion()
        {
            return new PermisosComplianceResultado
            {
                TieneAccesoModulo = true,
                EsAdministrador = false,
                PuedeAdministrarPermisos = false,
                PuedeVisualizar = true,
                PuedeCrearCargar = false,
                PuedeModificar = false,
                PuedeEliminar = false,
                PuedeDescargar = false
            };
        }
    }
}