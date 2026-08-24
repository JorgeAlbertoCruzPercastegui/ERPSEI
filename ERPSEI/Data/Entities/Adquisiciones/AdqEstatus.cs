namespace ERPSEI.Data.Entities.Adquisiciones
{
    public class AdqEstatus
    {
        public int Id
        {
            get;
            set;
        }

        public string Nombre
        {
            get;
            set;
        } = string.Empty;

        public string Codigo
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

        public bool Activo
        {
            get;
            set;
        } = true;

        public ICollection<AdqSolicitud>
            Solicitudes
        {
            get;
            set;
        } = new List<AdqSolicitud>();
    }
}