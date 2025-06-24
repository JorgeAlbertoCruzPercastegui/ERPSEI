using ERPSEI.Data.Entities.Empleados;

namespace ERPSEI.Data.Entities.Vacaciones
{
    public class HistorialVacaciones
    {
        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int DiasTomados { get; set; }
        public string? Observaciones { get; set; }

        public int SolicitudVacacionesId { get; set; }
        public SolicitudVacaciones Solicitud { get; set; }
        public int? AutorizadorId { get; set; }
    }
}
