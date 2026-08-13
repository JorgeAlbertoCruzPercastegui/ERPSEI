namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceTicket
    {
        public int Id { get; set; }

        public string Folio { get; set; } = string.Empty;

        public int TicketTypeId { get; set; }

        public ServiceTicketType? TicketType { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        // Usuario que levantó el ticket
        public string UsuarioSolicitanteId { get; set; } = string.Empty;

        // Administrador/agente que atiende el ticket
        public string? UsuarioAsignadoId { get; set; }

        public int? SupportTeamId { get; set; }

        public ServiceSupportTeam? SupportTeam { get; set; }

        public int CategoryId { get; set; }

        public ServiceCategory? Category { get; set; }

        public int? SubcategoryId { get; set; }

        public ServiceSubcategory? Subcategory { get; set; }

        public int PriorityId { get; set; }

        public ServiceTicketPriority? Priority { get; set; }

        public int StatusId { get; set; }

        public ServiceTicketStatus? Status { get; set; }

        // Intranet / Teams / Email / API
        public string Origen { get; set; } = "Intranet";

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public DateTime? FechaAsignacion { get; set; }

        public DateTime? FechaPrimeraRespuesta { get; set; }

        public DateTime? FechaResolucion { get; set; }

        public DateTime? FechaCierre { get; set; }

        public DateTime? FechaLimiteRespuestaSla { get; set; }

        public DateTime? FechaLimiteResolucionSla { get; set; }

        public bool SlaRespuestaVencido { get; set; }

        public bool SlaResolucionVencido { get; set; }

        public string? Resolucion { get; set; }

        public string? UsuarioCierreId { get; set; }

        public bool Eliminado { get; set; }

        public DateTime? FechaEliminacion { get; set; }

        public ICollection<ServiceTicketComment> Comentarios { get; set; }
            = new List<ServiceTicketComment>();

        public ICollection<ServiceTicketAttachment> Adjuntos { get; set; }
            = new List<ServiceTicketAttachment>();

        public ICollection<ServiceTicketHistory> Historial { get; set; }
            = new List<ServiceTicketHistory>();
    }
}