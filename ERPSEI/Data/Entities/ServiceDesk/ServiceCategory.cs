namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceCategory
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public int Orden { get; set; }

        public ICollection<ServiceSubcategory> Subcategorias { get; set; }
            = new List<ServiceSubcategory>();

        public ICollection<ServiceTicket> Tickets { get; set; }
            = new List<ServiceTicket>();
    }
}