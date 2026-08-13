namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceTicketStatus
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Codigo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool EsFinal { get; set; }

        public bool PausaSla { get; set; }

        public bool Activo { get; set; } = true;

        public int Orden { get; set; }

        public ICollection<ServiceTicket> Tickets { get; set; }
            = new List<ServiceTicket>();
    }
}