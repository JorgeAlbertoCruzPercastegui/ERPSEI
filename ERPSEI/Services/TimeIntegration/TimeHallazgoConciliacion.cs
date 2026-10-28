
namespace ERPSEI.Data.Entities.TimeIntegration
{
    public sealed class TimeHallazgoConciliacion
    {
        public long Id { get; set; }

        // Ejemplo: horas, actividades.
        public string TablaOrigen { get; set; } = "";

        // ID original de CyberTrack.
        public long RegistroOrigenId { get; set; }

        // Ejemplo: PosibleEliminacion.
        public string TipoHallazgo { get; set; } = "";

        // Pendiente, Confirmado o Recuperado.
        public string Estado { get; set; } = "Pendiente";

        public DateTime PrimeraDeteccion { get; set; }

        public DateTime UltimaRevision { get; set; }

        public int NumeroDetecciones { get; set; } = 1;

        public DateTime? FechaConfirmacion { get; set; }

        public DateTime? FechaRecuperacion { get; set; }

        public string? Observaciones { get; set; }
    }
}
