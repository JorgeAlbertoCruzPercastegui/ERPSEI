using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.Vacaciones
{
    public class SolicitudVacaciones
    {
        public int Id { get; set; }

        public int EmpleadoId { get; set; }
        public Empleado Empleado { get; set; }

        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int DiasSolicitados { get; set; }

        public string? ComentarioEmpleado { get; set; }
        public string? ComentarioAutorizador { get; set; }

        public EstadoSolicitud Estado { get; set; }

        public int? AutorizadorId { get; set; }
        public Empleado? Autorizador { get; set; }

        public DateTime? FechaRespuesta { get; set; }

        public string EstadoJefeDirecto { get; set; } = "Pendiente";
        public string EstadoTH { get; set; } = "Pendiente";

        public int? JefeDirectoEmpleadoId { get; set; }
        public Empleado? JefeDirectoEmpleado { get; set; }

        public string? UsuarioJefeDirectoId { get; set; }
        public AppUser? UsuarioJefeDirecto { get; set; }
        public DateTime? FechaRevisionJefeDirecto { get; set; }

        public string? UsuarioTHId { get; set; }
        public AppUser? UsuarioTH { get; set; }
        public DateTime? FechaRevisionTH { get; set; }
        public ICollection<HistorialVacaciones> Historiales { get; set; }
    }
}
