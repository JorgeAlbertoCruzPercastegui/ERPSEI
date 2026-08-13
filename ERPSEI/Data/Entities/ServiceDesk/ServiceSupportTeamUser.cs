namespace ERPSEI.Data.Entities.ServiceDesk
{
    public class ServiceSupportTeamUser
    {
        public int Id { get; set; }

        public int SupportTeamId { get; set; }

        public ServiceSupportTeam? SupportTeam { get; set; }

        public string UserId { get; set; } = string.Empty;

        public bool EsResponsable { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaAsignacion { get; set; }
    }
}