namespace ERPSEI.Data.Entities.Metricas
{
    public class IntranetActividad
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? NombreEmpleado { get; set; }

        public string TipoEvento { get; set; } = string.Empty;
        public string Modulo { get; set; } = string.Empty;
        public string Ruta { get; set; } = string.Empty;

        public DateTime FechaHora { get; set; } = DateTime.Now;

        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
    }
}
