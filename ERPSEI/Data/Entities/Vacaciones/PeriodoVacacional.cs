using ERPSEI.Data.Entities.Empleados;

namespace ERPSEI.Data.Entities.Vacaciones
{
    public class PeriodoVacacional
    {
        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }

        public DateTime FechaInicioPeriodo { get; set; }
        public DateTime FechaFinPeriodo { get; set; }

        public int DiasDisponibles { get; set; }
        public int DiasTomados { get; set; }

        public bool Activo { get; set; }
    }
}
