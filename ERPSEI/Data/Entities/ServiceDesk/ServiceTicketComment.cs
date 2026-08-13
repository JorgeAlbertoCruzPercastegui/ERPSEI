namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceTicketComment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public ServiceTicket? Ticket { get; set; }

        public string UsuarioId { get; set; } = string.Empty;

        public string Comentario { get; set; } = string.Empty;

        public bool EsNotaInterna { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Eliminado { get; set; }
    }
}