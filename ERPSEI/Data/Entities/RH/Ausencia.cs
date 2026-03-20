using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Entities.Usuarios;

namespace ERPSEI.Data.Entities.RH
{
    public class Ausencia
    {
        public int Id { get; set; }

        public int? EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }

        // NUEVO: jefe directo asignado a esta solicitud
        public int? JefeDirectoEmpleadoId { get; set; }
        public Empleado? JefeDirectoEmpleado { get; set; }

        public string Categoria { get; set; } = string.Empty;
        public string TipoCaptura { get; set; } = string.Empty;

        public int? TipoAusenciaId { get; set; }
        public TipoAusencia? TipoAusencia { get; set; }

        public int? TipoIncapacidadId { get; set; }
        public TipoIncapacidad? TipoIncapacidad { get; set; }

        public string? NumeroFolio { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraTermino { get; set; }

        public decimal? Dias { get; set; }
        public decimal? Horas { get; set; }

        public DateTime? FechaAplicacion { get; set; }

        public bool Suplencia { get; set; }

        public string EstadoJefeDirecto { get; set; } = "Pendiente";
        public string EstadoTH { get; set; } = "Pendiente";

        // quien aprobó o rechazó
        public string? UsuarioJefeDirectoId { get; set; }
        public AppUser? UsuarioJefeDirecto { get; set; }
        public DateTime? FechaRevisionJefeDirecto { get; set; }

        public string? UsuarioTHId { get; set; }
        public AppUser? UsuarioTH { get; set; }
        public DateTime? FechaRevisionTH { get; set; }

        public string? Comentario { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public string? UsuarioCreadorId { get; set; }
        public AppUser? UsuarioCreador { get; set; }
    }
}