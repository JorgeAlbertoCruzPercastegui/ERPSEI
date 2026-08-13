namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceSubcategory
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public ServiceCategory? Category { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public int Orden { get; set; }

        public ICollection<ServiceTicket> Tickets { get; set; }
            = new List<ServiceTicket>();
    }
}