namespace ERPSEI.Data.Entities.Metricas
{
    public class IntranetAuditoria
    {
        public int Id { get; set; }

        public string? UsuarioEjecutorId { get; set; }
        public string? UsuarioEjecutor { get; set; }

        public string Modulo { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;

        public string? Entidad { get; set; }
        public string? RegistroId { get; set; }
        public string? RegistroNombre { get; set; }

        public string? CampoModificado { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        public string? Ip { get; set; }
        public string? UserAgent { get; set; }

        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
    }
}
