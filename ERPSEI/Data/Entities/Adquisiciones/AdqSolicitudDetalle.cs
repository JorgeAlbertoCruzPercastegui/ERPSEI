namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqSolicitudDetalle
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

        public string ProductoServicio
        {
            get;
            set;
        } = string.Empty;

        public decimal Cantidad
        {
            get;
            set;
        }

        public string Unidad
        {
            get;
            set;
        } = string.Empty;

        public string? Descripcion
        {
            get;
            set;
        }

        public int Orden
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