namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceTicketAttachment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public ServiceTicket? Ticket { get; set; }

        public string NombreOriginal { get; set; } = string.Empty;

        public string NombreAlmacenado { get; set; } = string.Empty;

        public string RutaArchivo { get; set; } = string.Empty;

        public string? Extension { get; set; }

        public string? MimeType { get; set; }

        public long TamanoBytes { get; set; }

        public string UsuarioCargaId { get; set; } = string.Empty;

        public DateTime FechaCarga { get; set; }

        public bool Eliminado { get; set; }
    }
}