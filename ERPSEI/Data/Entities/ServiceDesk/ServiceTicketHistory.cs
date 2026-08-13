namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceTicketHistory
    {
        public long Id { get; set; }

        public int TicketId { get; set; }

        public ServiceTicket? Ticket { get; set; }

        public string? UsuarioId { get; set; }

        public string Accion { get; set; } = string.Empty;

        public string? Campo { get; set; }

        public string? ValorAnterior { get; set; }

        public string? ValorNuevo { get; set; }

        public string? Detalle { get; set; }

        public DateTime FechaHora { get; set; }

        public string? DireccionIp { get; set; }
    }
}