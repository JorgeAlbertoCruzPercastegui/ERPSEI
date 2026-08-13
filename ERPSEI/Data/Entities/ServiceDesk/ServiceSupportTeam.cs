namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceSupportTeam
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<ServiceSupportTeamUser> Usuarios { get; set; }
            = new List<ServiceSupportTeamUser>();

        public ICollection<ServiceTicket> Tickets { get; set; }
            = new List<ServiceTicket>();
    }
}