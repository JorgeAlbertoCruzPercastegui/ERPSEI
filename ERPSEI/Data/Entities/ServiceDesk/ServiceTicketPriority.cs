namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceTicketPriority
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Codigo { get; set; } = string.Empty;

        public int Nivel { get; set; }

        public int MinutosRespuesta { get; set; }

        public int MinutosResolucion { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<ServiceTicket> Tickets { get; set; }
            = new List<ServiceTicket>();
    }
}